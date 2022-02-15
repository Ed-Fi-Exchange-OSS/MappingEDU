// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.detail.mapping-summary.actions
//

var m = angular.module('app.modules.mapping-project.detail.mapping-summary.actions', [
    'app.modules.mapping-project.detail.mapping-summary.detail',
    'app.modules.mapping-project.detail.mapping-summary.full',
    'app.modules.mapping-project.detail.mapping-summary.method',
    'app.modules.mapping-project.detail.mapping-summary.status'
]);


// ****************************************************************************
// Controller app.modules.mapping-project.detail.mapping-summary
//

m.controller('app.modules.mapping-project.detail.mapping-summary.actions', ['$scope', '$stateParams', 'repositories', 'services', 'mappingSummaryService',
    function($scope, $stateParams, repositories: IRepositories, services: IServices, mappingSummaryService: IMappingSummaryService) {

        services.logger.debug('Loaded contoller app.modules.mapping-project.detail.mapping-summary.actions');

        var vm = this;
        vm.hover = null;
        vm.id = $stateParams.id;


        vm.getEntities = (domain, index) => {

            var summaryFilter = services.session.cloneFromSession('mappingSummary', vm.id);

            if (!summaryFilter) {
                summaryFilter = {
                    selectedGroups: []
                };
            }

            if (domain.SystemItemId === '00000000-0000-0000-0000-000000000000') return;

            if (!domain.SubSummary) {
                $scope.loading++;
                vm.getSummary($scope.itemType, domain.SystemItemId).then(summary => {
                    angular.forEach(summary, entity => {
                        entity.Parent = domain;
                        entity.Hrefs = mappingSummaryService.buildEntityHrefs(vm.id, domain, entity, $scope.itemType);
                        mappingSummaryService.setPercents(entity);
                    });

                    $scope.summary[index].SubSummary = summary;
                }).finally(() => { $scope.loading-- });   
            }

            domain.Selected = !domain.Selected;

            if (domain.Selected) summaryFilter.selectedGroups.push(domain.SystemItemId);
            else {
                var groupIndex = summaryFilter.selectedGroups.indexOf(domain.SystemItemId);
                if (groupIndex > -1) summaryFilter.selectedGroups.splice(groupIndex, 1);
            }

            services.session.cloneToSession('mappingSummary', vm.id, summaryFilter);
        }


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

        vm.expandAll = () => {
            vm.selectedGroups = [];
            angular.forEach($scope.summary, domain => {
                if (domain.ItemName === 'All') return;
                if (!domain.SubSummary) {
                    vm.getSummary($scope.itemType, domain.SystemItemId).then((subSummary) => {
                        angular.forEach(subSummary, entity => {
                            entity.Parent = domain;
                            entity.Hrefs = mappingSummaryService.buildEntityHrefs(vm.id, domain, entity, $scope.itemType);
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
            angular.forEach($scope.summary, domain => {
                domain.Selected = false;
            });
            vm.saveFilter();
        }


        vm.saveFilter = () => {

            var summaryFilter = services.session.cloneFromSession('mappingSummary', vm.id);
            summaryFilter.selectedGroups = vm.selectedGroups;

            services.session.cloneToSession('mappingSummary', vm.id, summaryFilter);
        }
    }
]);
