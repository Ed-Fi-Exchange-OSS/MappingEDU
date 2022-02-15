// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.detail.info
//

var m = angular.module('app.modules.mapping-project.detail.info', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.mapping-project.detail.info', { //mappingProject.info
            url: '/info',
            data: {
                title: 'Mapping Project Info',
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('View')].Id
            },
            templateUrl: `${settings.moduleBaseUri}/mappingProject/detail/info/mappingProjectInfoView.tpl.html`,
            controller: 'app.modules.mapping-project.detail.info',
            controllerAs: 'mappingProjectInfoViewModel',
            resolve: {
                creator: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.mappingProject.getCreator($stateParams.id);
                }],
                owners: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.mappingProject.getOwners($stateParams.id);
                }]
            }
        });
}]);


// ****************************************************************************
// Controller app.modules.mapping-project.detail.info
//

m.controller('app.modules.mapping-project.detail.info', ['$scope', 'services', 'creator', 'owners',
    function ($scope, services: IServices, creator, owners) {
        services.logger.debug('Loaded controller app.modules.mapping-project.detail.info');
        $scope.$parent.mappingProjectDetailViewModel.setTitle('INFO');
        $scope.creator = creator;
        $scope.owners = owners;
    }
]);
