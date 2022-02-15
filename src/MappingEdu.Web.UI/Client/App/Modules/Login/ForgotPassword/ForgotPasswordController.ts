// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.modules.login.forgot-password
//

var m = angular.module('app.modules.login.forgot-password', []);


// ****************************************************************************
// Configuration
//

m.config(['$stateProvider', 'settings', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $stateProvider
        .state('app.login.forgot-password', {
            url: '/forgot-password',
            data: {
                roles: []
            },
            views: {
                'fullscreen@': {
                    templateUrl: `${settings.moduleBaseUri}/login/ForgotPassword/ForgotPasswordView.tpl.html`,
                    controller: 'app.login.forgot-password'
                }
            }
        });
}]);


// ****************************************************************************
// Controller app.login.forgot-password
//

m.controller('app.login.forgot-password', ['$scope', 'repositories', 'services',
    ($scope: any, repositories: IRepositories, services: IServices) => {

        services.logger.debug('Loading app.login.forgot-password controller.');

    $scope.forgotPassword = (email) => {
        return repositories.users.forgotPassword(email).then(() => {
            services.logger.success('We sent you an e-mail with instructions on how to reset your password.');
        }, error => {
            services.logger.error('We encountered an errpr sending a password reset e-mail. Please try again later.', error.data);
        });
    }

}]);
