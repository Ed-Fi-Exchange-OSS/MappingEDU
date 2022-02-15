// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.manage.organizations
//

var m = angular.module('app.modules.manage.organizations', [
    'app.modules.manage.organizations.create',
    'app.modules.manage.organizations.edit',
    'app.modules.manage.organizations.edit.users',
    'app.modules.manage.organizations.directives.organization-form',
]);


// ****************************************************************************
// Configure 
//

m.config(['$urlRouterProvider', '$stateProvider', 'settings', ($urlRouterProvider: ng.ui.IUrlRouterProvider, $stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $stateProvider
        .state('app.manage.organizations', {
            url: '/organizations',
            data: {
                title: 'Manage Organizations'
            },
            templateUrl: `${settings.moduleBaseUri}/Manage/Organizations/OrganizationsManageView.tpl.html`,
            controller: 'app.modules.manage.organizations',
            controllerAs: 'manageOrganizationsViewModel',
            resolve: {
                organizations: ['repositories', (repositories: IRepositories) => {
                    return repositories.organizations.getAll();
                }]
            }
        });
}]);


// ****************************************************************************
// Controller app.modules.manage.organizations
//

m.controller('app.modules.manage.organizations', ['repositories', 'services', 'organizations',
    function (repositories: IRepositories, services: IServices, organizations) {

        services.logger.debug('Loading controller app.modules.manage.organizations.');

        var vm = this;
        vm.organizations = organizations;

        vm.organizationHref = (user: IUser) => {
            return services.state.href('app.manage.organizations.edit', { id: user.Id });
        }

        vm.delete = (organization, index) => {
            repositories.organizations.remove(organization).then((data) => {
                vm.organizations.splice(index, 1);
                services.logger.success('Deleted organization.');
            }, (error) => {
                services.logger.error('Error deleting organization.', error.data);
            });
        }

    }
]);