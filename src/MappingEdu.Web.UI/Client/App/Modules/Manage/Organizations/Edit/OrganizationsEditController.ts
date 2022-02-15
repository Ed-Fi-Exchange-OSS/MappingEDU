// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.manage.organizations.edit
//

var m = angular.module('app.modules.manage.organizations.edit', []);


// ****************************************************************************
// Configure 
//

m.config(['$urlRouterProvider', '$stateProvider', 'settings', ($urlRouterProvider: ng.ui.IUrlRouterProvider, $stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $urlRouterProvider.when('/manage/organizations/detail/:id', '/manage/organizations/detail/:id/users');

    $stateProvider
        .state('app.manage.organizations.edit', {
            url: '/detail/:id',
            data: {
                title: 'Edit Organization'
            },
            views: {
                'content@': {
                    templateUrl: `${settings.moduleBaseUri}/Manage/Organizations/Edit/OrganizationsEditView.tpl.html`,
                    controller: 'app.modules.manage.organizations.edit',
                    controllerAs: 'organizationEditViewModel'
                }
            },
            resolve: {
                organization: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.organizations.find($stateParams.id);
                }]
            }
        });
}]);

// ****************************************************************************
// Controller app.modules.manage.organizations.edit
//

m.controller('app.modules.manage.organizations.edit', ['$scope', '$stateParams', 'repositories', 'services', 'settings', 'organization',
    function ($scope: any, $stateParams: any, repositories: IRepositories, services: IServices, settings: ISystemSettings, organization) {

        services.logger.debug('Loaded controller app.modules.manage.organizations.edit.');

        var vm = this;

        vm.organization = organization;
        vm.organization.StringDomains = vm.organization.Domains.join(settings.deliminator);

        vm.tabs = [
            { link: 'app.manage.organizations.edit.users', label: 'Users' },
        ];

        vm.onPage = sref => (services.state.current.name === sref);
        vm.showTabs = () => services.underscore.some(<Array<any>>vm.tabs, tab => services.state.is(tab.link));
    }
]);
