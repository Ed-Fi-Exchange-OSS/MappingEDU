// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.data-standard.edit.info
//

var m = angular.module('app.modules.data-standard.edit.info', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.data-standard.edit.info', {
            url: '/info',
            data: {
                title: 'Data Standard Info',
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Guest')].Id
            },
            templateUrl: `${settings.moduleBaseUri}/DataStandard/Edit/Info/DataStandardInfoView.tpl.html`,
            controller: 'app.modules.data-standard.edit.info',
            controllerAs: 'dataStandardInfoViewModel',
            resolve: {
                creator: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.dataStandard.getCreator($stateParams.dataStandardId);
                }],
                owners: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.dataStandard.getOwners($stateParams.dataStandardId);
                }]
            }
        });
}]);


// ****************************************************************************
// Controller app.modules.data-standard.edit.info
//

m.controller('app.modules.data-standard.edit.info', ['$scope', 'services', 'creator', 'owners',
    function ($scope, services: IServices, creator, owners) {
        services.logger.debug('Loaded controller app.modules.data-standard.edit.info');
        $scope.$parent.dataStandardDetailViewModel.setTitle('INFO');
        $scope.creator = creator;
        $scope.owners = owners;
    }
]);
