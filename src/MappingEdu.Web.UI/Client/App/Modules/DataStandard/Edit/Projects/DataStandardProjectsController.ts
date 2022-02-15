// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.data-standard.edit.projects
//

var m = angular.module('app.modules.data-standard.edit.projects', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.data-standard.edit.projects', {
            url: '/source',
            data: {
                title: 'Mapping Projects',
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Guest')].Id
            },
            templateUrl: `${settings.moduleBaseUri}/DataStandard/Edit/Projects/DataStandardProjectsView.tpl.html`,
            controller: 'app.modules.data-standard.edit.projects',
            controllerAs: 'dataStandardProjectsViewModel',
            resolve: {
                source: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.dataStandard.source.get($stateParams.dataStandardId);
                }],
                target: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.dataStandard.target.get($stateParams.dataStandardId);
                }]
            }
        });
}]);


// ****************************************************************************
// Controller app.modules.data-standard.edit.projects
//

m.controller('app.modules.data-standard.edit.projects', ['$scope', 'source', 'target', 'services',
    function ($scope, source, target, services: IServices) {

        services.logger.debug('Loading controller app.modules.data-standard.edit.projects');
        $scope.$parent.dataStandardDetailViewModel.setTitle('Projects');

        var vm = this;
        vm.dataStandardSourceDisplay = source;
        vm.dataStandardTargetDisplay = target;

        var projectInfoState = 'app.mapping-project.detail.info';
        var projectSummaryState = 'app.mapping-project.detail.mapping-summary';
        var projectReviewQueueState = 'app.mapping-project.detail.review-queue';

        services.underscore.each(<Array<any>>vm.dataStandardSourceDisplay, project => {
            var projectId = `({ id: '${project.MappingProjectId}' })`;
            project.projectSref = projectInfoState + projectId;
            project.listSref = projectReviewQueueState + projectId;
            project.mapSummarySref = projectSummaryState + projectId;
        });

        services.underscore.each(<Array<any>>vm.dataStandardTargetDisplay, project => {
            var projectId = `({ id: '${project.MappingProjectId}' })`;
            project.projectSref = projectInfoState + projectId;
            project.listSref = projectReviewQueueState + projectId;
            project.mapSummarySref = projectSummaryState + projectId;
        });
    }
]);