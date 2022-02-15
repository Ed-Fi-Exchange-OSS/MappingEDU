// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.login.confirm-email
//

var m = angular.module('app.modules.login.confirm-email', []);

// ****************************************************************************
// Configuration
//

m.config(['$stateProvider', 'settings', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $stateProvider
        .state('app.login.confirm-email', {
            url: '/confirm-email/:userId?code',
            data: {
                roles: []
            },
            views: {
                'fullscreen@': {
                    templateUrl: `${settings.moduleBaseUri}/login/confirmemail/confirmemail.tpl.html`,
                    controller: 'app.login.confirm-email'
                }
            }     
    });
}]);


// ****************************************************************************
// Controller app.login
//

m.controller('app.login.confirm-email', ['$', '$scope', '$stateParams','repositories', 'services',
    ($, $scope, $stateParams, repositories: IRepositories, services: IServices) => {

        services.logger.debug('Loaded controller app.login.confirm-email.');

        $scope.user = {
            Id: $stateParams.userId
        };

        $scope.confirmEmail = () => {
            return repositories.users.confirmEmail($stateParams.code, $scope.user).then(() => {
                services.state.go('app.login');
                services.logger.success('Authenticated account.');
            }, error => {
                services.logger.error('Error! ', error.data);
            });
        }

        repositories.systemConstant.find('Terms of Use').then(data => {
            $scope.termsOfUse = data;
            services.timeout(() => {
                $('#termsOfUse').perfectScrollbar();
            }, 100);
        });
    }]); 
