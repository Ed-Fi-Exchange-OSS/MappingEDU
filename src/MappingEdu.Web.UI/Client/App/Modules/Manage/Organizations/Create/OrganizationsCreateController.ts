// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.manage.organizations.create
//

var m = angular.module('app.modules.manage.organizations.create', []);


// ****************************************************************************
// Configure 
//

m.config(['$urlRouterProvider', '$stateProvider', 'settings', ($urlRouterProvider: ng.ui.IUrlRouterProvider, $stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $stateProvider
        .state('app.manage.organizations.create', {
            url: '/create',
            data: {
                title: 'Create Organization'
            },
            views: {
                'content@': {
                    templateUrl: `${settings.moduleBaseUri}/Manage/Organizations/Create/OrganizationsCreateView.tpl.html`,
                    controller: 'app.modules.manage.organizations.create',
                    controllerAs: 'organizationCreateViewModel'
                }
            }
        });
}]);


// ****************************************************************************
// Controller app.modules.manage.organizations.create
//

m.controller('app.modules.manage.organizations.create', ['services', (services: IServices) => {

        services.logger.debug('Loaded controller app.modules.manage.organizations.create.');

    }
]);
