// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.modules.login.reset-password
//

var m = angular.module('app.modules.login.reset-password', []);


// ****************************************************************************
// Configuration
//

m.config(['$stateProvider', 'settings', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $stateProvider
        .state('app.login.reset-password', {
            url: '/reset-password/:userId?code=',
            data: {
                roles: []
            },
            views: {
                'fullscreen@': {
                    templateUrl: `${settings.moduleBaseUri}/login/ResetPassword/ResetPasswordView.tpl.html`,
                    controller: 'app.login.reset-password'
                }
            }
        });
}]);


// ****************************************************************************
// Controller app.login.reset-password
//

m.controller('app.login.reset-password', ['$', '$stateParams', '$scope', 'repositories', 'services',
    ($, $stateParams, $scope: any, repositories: IRepositories, services: IServices) => {

        services.logger.debug('Loading app.login.reset-password controller.');

    $scope.user = {
        Id: $stateParams.userId
    };

    $scope.resetPassword = () => {
        return repositories.users.resetPassword($stateParams.code, $scope.user).then(() => {
            services.state.go('app.login');
            services.logger.success('You have successfully reset your password');
        }, error => {
            services.logger.error('We encountered an error reseting your password.', error.data);
        });
    }

    repositories.systemConstant.find('Terms of Use').then(data => {
        $scope.termsOfUse = data;
        services.timeout(() => {
            $('#termsOfUse').perfectScrollbar();
        }, 100);
    });

}]);
