// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.data-standard.edit.source
//

var m = angular.module('app.modules.data-standard.edit.source', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.data-standard.edit.source', {
            url: '/source',
            data: {
                title: 'Source Mapping Porjects',
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Guest')].Id
            },
            templateUrl: `${settings.moduleBaseUri}/DataStandard/Edit/Source/DataStandardSourceView.tpl.html`,
            controller: 'app.modules.data-standard.edit.source',
            controllerAs: 'dataStandardSourceViewModel',
            resolve: {
                source: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.dataStandard.source.get($stateParams.dataStandardId);
                }]
            }
        });
}]);


// ****************************************************************************
// Controller app.modules.data-standard.edit.source
//

m.controller('app.modules.data-standard.edit.source', ['$scope', 'source', 'services',
    function ($scope, source, services: IServices) {

        services.logger.debug('Loading controller app.modules.data-standard.edit.source');
        $scope.$parent.dataStandardDetailViewModel.setTitle('SOURCE MAPPINGS');

        var vm = this;
        vm.dataStandardSourceDisplay = source;
        var projectInfoState = 'app.mapping-project.detail.info';
        var projectSummaryState = 'app.mapping-project.detail.mapping-summary';
        var projectReviewQueueState = 'app.mapping-project.detail.review-queue';
        services.underscore.each(<Array<any>>vm.dataStandardSourceDisplay, project => {
            var projectId = `({ id: '${project.MappingProjectId}' })`;
            project.projectSref = projectInfoState + projectId;
            project.listSref = projectReviewQueueState + projectId;
            project.mapSummarySref = projectSummaryState + projectId;
        });
    }
]);