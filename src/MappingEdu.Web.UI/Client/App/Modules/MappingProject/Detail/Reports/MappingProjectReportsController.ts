// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.detail.reports
//

var m = angular.module('app.modules.mapping-project.detail.reports', [
    'app.modules.mapping-project.detail.reports.create'
]);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.mapping-project.detail.reports', { //mappingProject.reports
            url: '/reports',
            data: {
                title: 'Mapping Project Reports',
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Guest')].Id
            },
            templateUrl: `${settings.moduleBaseUri}/mappingProject/detail/reports/mappingProjectReportsView.tpl.html`,
            controller: 'app.modules.mapping-project.detail.reports',
            controllerAs: 'mappingProjectReportsViewModel'
        });
}]);


// ****************************************************************************
// Controller app.modules.mapping-project.detail.reports
//

m.controller('app.modules.mapping-project.detail.reports', ['$scope', '$stateParams', 'repositories', 'services', 'mappingProject', 'settings',
    function ($scope, $stateParams, repositories: IRepositories, services: IServices, mappingProject, settings: ISystemSettings) {

        services.logger.debug('Loaded controller app.modules.mapping-project.detail.reports');
        $scope.$parent.mappingProjectDetailViewModel.setTitle('REPORTS');

        var vm = this;

        vm.mappingProject = mappingProject;

        vm.id = $stateParams.id;

        //TSMQR = Target Standard Mapping Queue Report
        vm.getReport = (isTargetReport) => {
            var report: ng.ui.bootstrap.IModalSettings = {
                backdrop: true,
                animation: true,
                keyboard: false,
                size: 'lg',
                templateUrl: `${settings.moduleBaseUri}/MappingProject/Detail/Reports/CreateReport.tpl.html`,
                controller: 'app.modules.mapping-project.detail.reports.create',
                resolve: {
                    mappingProjectId: () => {
                        return vm.id;
                    },
                    isTargetReport: () => {
                        return isTargetReport;
                    },
                    customDetailMetadata: () => {
                        return repositories.customDetailMetadata.getAllByDataStandard((isTargetReport) ? mappingProject.TargetDataStandardId : mappingProject.SourceDataStandardId);
                    },
                    viewOnly: () => {
                        return services.profile.mappingProjectAccess($stateParams.id).then((data) => {
                            return data.Role < 2;
                        });
                    }
                }
            }
            var modal = services.modal.open(report);
            modal.result.then(() => {});
        }

        vm.downloadCeds = () => {
            return repositories.mappingProject.reports.getCedsToken(vm.id).then((data) => {
                var link = document.createElement('a');
                link.href = `${settings.apiBaseUri}/MappingProjectReports/${vm.id}/ceds/${data}`;
                document.body.appendChild(link);
                link.click();
            });
        }

        var popoverElement = <any>$('[data-toggle="popover"]');
        popoverElement.popover();
    }
]);
