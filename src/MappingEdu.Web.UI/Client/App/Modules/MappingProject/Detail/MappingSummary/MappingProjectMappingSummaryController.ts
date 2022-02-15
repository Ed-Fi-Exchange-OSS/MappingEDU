// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.detail.mapping-summary
//

var m = angular.module('app.modules.mapping-project.detail.mapping-summary', [
    'app.mapping-project.detail.mapping-summary.service',
    'app.modules.mapping-project.detail.mapping-summary.actions'
]);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.mapping-project.detail.mapping-summary', { //mappingProject.mappingSummary
            url: '/summary',
            data: {
                roles: ['user'],
                title: 'Mapping Project Mapping Summary',
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('View')].Id
            },
            templateUrl: `${settings.moduleBaseUri}/mappingProject/detail/mappingSummary/mappingProjectMappingSummaryView.tpl.html`,
            controller: 'app.modules.mapping-project.detail.mapping-summary',
            controllerAs: 'mappingProjectMappingSummaryViewModel'
        });
}]);


// ****************************************************************************
// Controller app.modules.mapping-project.detail.mapping-summary
//

m.controller('app.modules.mapping-project.detail.mapping-summary', ['$scope', '$stateParams', 'repositories', 'services', 'mappingSummaryService', 'mappingSummaryService',
    function ($scope, $stateParams, repositories: IRepositories, services: IServices, mappingSummaryService: IMappingSummaryService) {

        services.logger.debug('Loaded contoller app.modules.mapping-project.detail.mapping-summary');
        $scope.$parent.mappingProjectDetailViewModel.setTitle('SUMMARY');

        var vm = this;
        vm.id = $stateParams.id;
        vm.loading = 0;
        vm.pieChart = 'status';

        vm.getSummary = (itemType, entityId) => {
            vm.loading++;
            return repositories.mappingProject.summary.get(vm.id, itemType, entityId)
                .then(summary => {
                    if (!entityId) {
                        angular.forEach(summary, row => {
                            row.Hrefs = mappingSummaryService.buildDomainHrefs(vm.id, row, vm.itemType);
                            mappingSummaryService.setPercents(row);
                        });
                    }
                    return summary;
                })
                .finally(() => { vm.loading--; });
        }

        vm.saveFilter = () => {

            var detailSummary = angular.copy(vm.selectedSummaryDetail);
            if (detailSummary && detailSummary.Parent) {
                detailSummary.Parent = {
                    ItemName: detailSummary.Parent.ItemName,
                    SystemItemId: detailSummary.Parent.SystemItemId
                };
            }

            var summaryFilter = services.session.cloneFromSession('mappingSummary', vm.id);
            if (summaryFilter) vm.pieChart = summaryFilter.pieChart ? summaryFilter.pieChart : 'status';

            var filter = {
                itemType: vm.itemType,
                selectedGroups: vm.selectedGroups,
                mappingFilter: vm.mappingFilter,
                showDetails: vm.showDetails,
                selectedSummaryDetail: detailSummary,
                pieChart: vm.pieChart
            }

            services.session.cloneToSession('mappingSummary', vm.id, filter);
        }

        vm.changeItemType = (itemType) => {
            vm.itemType = itemType;

            var summaryFilter = services.session.cloneFromSession('mappingSummary', vm.id);
            if (summaryFilter)
                vm.selectedGroups = summaryFilter.selectedGroups;

            vm.saveFilter();
            vm.applyFilter();
        }

        vm.changeMappingFilter = (filter) => {
            vm.mappingFilter = filter;

            var summaryFilter = services.session.cloneFromSession('mappingSummary', vm.id);
            if (summaryFilter)
                vm.selectedGroups = summaryFilter.selectedGroups;

            vm.saveFilter();
        }

        vm.expandAll = () => {
            vm.selectedGroups = [];
            angular.forEach(vm.summary, domain => {
                if (domain.ItemName === 'All') return;
                if (!domain.SubSummary) {
                    vm.getSummary(vm.itemType, domain.SystemItemId).then((subSummary) => {
                        angular.forEach(subSummary, entity => {
                            entity.Parent = domain;
                            entity.Hrefs = mappingSummaryService.buildEntityHrefs(vm.id, domain, entity, vm.itemType);
                            mappingSummaryService.setPercents(entity);
                        });
                        domain.SubSummary = subSummary;
                    });
                }
                domain.Selected = true;
                vm.selectedGroups.push(domain.SystemItemId);
            });
            vm.saveFilter();
        }

        vm.collapseAll = () => {
            vm.selectedGroups = [];
            angular.forEach(vm.summary, domain => {
                domain.Selected = false;
            });
            vm.saveFilter();
        }

        vm.showMoreDetails = (summaryDetail) => {
            vm.showDetails = true;
            vm.selectedSummaryDetail = summaryDetail;

            vm.loading++;
            repositories.mappingProject.summary.getDetail(vm.id, summaryDetail.SystemItemId, vm.itemType).then(details => {
                summaryDetail.Values = mappingSummaryService.buildValuesMatrix(vm.id, summaryDetail, vm.itemType);
                summaryDetail.StatusValues = [];
                summaryDetail.MethodValues = [];

                angular.forEach(details, detail => {                    
                    summaryDetail.Values[detail.MappingMethodTypeId][detail.WorkflowStatusTypeId].Value = detail.Total;
                    summaryDetail.Values[detail.MappingMethodTypeId][5].Value += detail.Total;
                    summaryDetail.Values[5][detail.WorkflowStatusTypeId].Value += detail.Total;
                    summaryDetail.Values[5][5].Value += detail.Total;
                });

                for (var i = 0; i < 5; i++) {
                    summaryDetail.StatusValues.push(summaryDetail.Values[5][i].Value);
                    if (i !== 2)
                        summaryDetail.MethodValues.push(summaryDetail.Values[i][5].Value);
                }

            }).finally(() => { vm.loading-- });

            vm.saveFilter();
        }

        vm.applyFilter = () => {
            var summaryFilter = services.session.cloneFromSession('mappingSummary', vm.id);
            if (summaryFilter) {
                vm.mappingFilter = summaryFilter.mappingFilter ? summaryFilter.mappingFilter : 'full';
                vm.itemType = summaryFilter.itemType;
                vm.selectedGroups = summaryFilter.selectedGroups ? summaryFilter.selectedGroups : [];
                vm.showDetails = summaryFilter.showDetails;
                vm.selectedSummaryDetail = vm.selectedSummaryDetail ? vm.selectedSummaryDetail : summaryFilter.selectedSummaryDetail;
                vm.pieChart = summaryFilter.pieChart ? summaryFilter.pieChart : 'status';
            } else {
                vm.mappingFilter = 'full';
                vm.itemType = 4;
                vm.selectedGroups = [];
                vm.showDetails = false;
                vm.pieChart = 'status';
            }

            vm.headerHrefs = mappingSummaryService.buildHeaderHrefs(vm.id, vm.itemType);

            vm.getSummary(vm.itemType).then((summary) => {
                vm.summary = summary;
                angular.forEach(vm.summary, domain => {
                    if (vm.selectedGroups.indexOf(domain.SystemItemId) > -1) {
                        domain.Selected = true;
                        vm.getSummary(vm.itemType, domain.SystemItemId).then((subSummary) => {
                            angular.forEach(subSummary, entity => {
                                entity.Parent = domain;
                                entity.Hrefs = mappingSummaryService.buildEntityHrefs(vm.id, domain, entity, vm.itemType);
                                mappingSummaryService.setPercents(entity);
                            });
                            domain.SubSummary = subSummary;
                        });
                    }
                });
            });

            if (vm.showDetails) vm.showMoreDetails(vm.selectedSummaryDetail);
        }

        vm.applyFilter();
    }
]);
