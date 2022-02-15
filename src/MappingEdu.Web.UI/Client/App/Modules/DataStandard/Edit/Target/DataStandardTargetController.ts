// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.data-standard.edit.target
//

var m = angular.module('app.modules.data-standard.edit.target', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.data-standard.edit.target', {
            url: '/target',
            data: {
                title: 'Target Mapping Projects',
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Guest')].Id
            },
            templateUrl: `${settings.moduleBaseUri}/DataStandard/Edit/Target/DataStandardTargetView.tpl.html`,
            controller: 'app.modules.data-standard.edit.target',
            controllerAs: 'dataStandardTargetViewModel',
            resolve: {
                target: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.dataStandard.target.get($stateParams.dataStandardId);
                }]
            }
        });
}]);


// ****************************************************************************
// Controller app.modules.data-standard.edit.target
//

m.controller('app.modules.data-standard.edit.target', ['$scope', '$stateParams', 'target', 'services', 'repositories',
    function ($scope, $stateParams, target, services: IServices, repositories: IRepositories) {

        services.logger.debug('Lodaed app.modules.data-standard.edit.target controller');
        $scope.$parent.dataStandardDetailViewModel.setTitle('TARGET MAPPINGS');

        var vm = this;

        vm.dataStandardTargetDisplay = target;
        var projectInfoState = 'app.mapping-project.detail.info';
        var projectSummaryState = 'app.mapping-project.detail.mapping-summary';
        var projectReviewQueueState = 'app.mapping-project.detail.review-queue';
        services.underscore.each(<Array<any>>vm.dataStandardTargetDisplay, project => {
            var projectId = `({ id: '${project.MappingProjectId}' })`;
            project.projectSref = projectInfoState + projectId;
            project.listSref = projectReviewQueueState + projectId;
            project.mapSummarySref = projectSummaryState + projectId;
        });

        vm.goToUrl = stateName => {
            services.state.go(stateName);
        };
    }
]);