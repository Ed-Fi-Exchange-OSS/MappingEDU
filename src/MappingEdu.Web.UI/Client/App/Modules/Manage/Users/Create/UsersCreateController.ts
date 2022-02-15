// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.manage.users.create
//

var m = angular.module('app.modules.manage.users.create', []);


// ****************************************************************************
// Configure 
//

m.config(['$urlRouterProvider', '$stateProvider', 'settings', ($urlRouterProvider: ng.ui.IUrlRouterProvider, $stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $stateProvider
        .state('app.manage.users.create', {
            url: '/create',
            data: {
                title: 'Create User'
            },
            views: {
                'content@': {
                    templateUrl: `${settings.moduleBaseUri}/Manage/Users/Create/UsersCreateView.tpl.html`,
                    controller: 'app.modules.manage.users.create',
                    controllerAs: 'usersCreateViewModel'
                }
            }
        });
}]);


// ****************************************************************************
// Controller app.modules.manage.users.create
//

m.controller('app.modules.manage.users.create', ['services', (services: IServices) => {

        services.logger.debug('Loaded controller app.modules.manage.users.create');
    }
]);
