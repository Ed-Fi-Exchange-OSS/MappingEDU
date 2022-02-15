// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.errors.unknown
//

var m = angular.module('app.modules.errors.unknown', []);


// ****************************************************************************
// Configuration
//

m.config(['$stateProvider', 'settings', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $stateProvider
        .state('app.errors.unknown', {
            url: '/unknown',
            data: {
                roles: []
            },
            views: {
                'content@': {
                    templateUrl: `${settings.moduleBaseUri}/Errors/UnknownError/UnknownError.tpl.html`,
                    controller: 'app.errors.unknown'
                }
            }
        });         
}]);


// ****************************************************************************
// Controller app.errors.unknown
//

m.controller('app.errors.unknown', ['$scope', 'security', 'repositories', 'services',
    ($scope, security: ISecurity, repositories: IRepositories, services: IServices) => {

        services.logger.debug('Loading controller app.errors.access-denied');

        $scope.error = services.session.cloneFromSession('navigation', 'error');

        if ($scope.error && $scope.error.to && $scope.error.to.state)
            $scope.error.to.href = services.state.href($scope.error.to.state.name, $scope.error.to.params);

        if ($scope.error && $scope.error.from && $scope.error.from.state)
            $scope.error.from.href = services.state.href($scope.error.from.state.name, $scope.error.from.params);
    }
]);
