// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.login
//

var m = angular.module('app.modules.login', [
    'app.modules.login.reset-password',
    'app.modules.login.forgot-password',
    'app.modules.login.confirm-email'
]);

// ****************************************************************************
// Configuration
//

m.config(['$stateProvider', 'settings', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $stateProvider
        .state('app.login', {
            url: '/login?token',
            data: {
                roles: []
            },
            views: {
                'fullscreen@': {
                    templateUrl: `${settings.moduleBaseUri}/login/login.tpl.html`,
                    controller: 'app.login'
                }
            },
            resolve: {
                guestIsActive: ['repositories', (repositories: IRepositories) => {
                    return repositories.users.getGuestIsActive();
                }]
            }
        });
}]);


// ****************************************************************************
// Controller app.login
//

m.controller('app.login', ['$scope', 'security', 'repositories', 'services', 'settings', 'guestIsActive',
    ($scope, security: ISecurity, repositories: IRepositories, services: IServices, settings: ISystemSettings, guestIsActive) => {

        services.logger.debug('Loading app.login controller.');

        $scope.loading = undefined;

        $scope.guestIsActive = guestIsActive;
        
        $scope.auth = {};
        $scope.authenticate = (isGuest: boolean) => {

            if (isGuest) {
                $scope.username = 'guest@example.com';
                $scope.password = 'guest9999';
            } else {
                $scope.username = $scope.auth.username;
                $scope.password = $scope.auth.password;
            }

            return repositories.authentication.authenticate($scope.username, $scope.password).then((data: IAuthenticationToken) => {

                services.logger.success(`Logged in as: ${$scope.username}`);
                services.logger.debug(`Using token: ${data.access_token}`);
                security.principal.authenticate({ name: $scope.username, token: data.access_token, roles: ['user'] }); // base role

                // get roles and profile configuration  
                services.profile.clearProfile();
                services.profile.me().then((me: ICurrentUser) => {

                    services.logging.add({
                        Source: 'app.login',
                        Message: 'Logged in',
                        Level: 'INFO'
                    });

                    //Remove Trailing Spaces
                    $scope.username = $scope.username.replace(/\s+$/, '');

                    var roles = me.Roles;
                    services.logger.debug(`Using roles: ${JSON.stringify(roles)}`);
                    security.principal.authenticate({ name: $scope.username, token: data.access_token, roles: roles });
                    services.events.emit('event:login');
                    services.state.go('app.home', {});
                });

            }, error => {
                services.logger.error('An error occurred authenticating', error.data);
            });
        };
    }]); 
