// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.manage.users.edit
//

var m = angular.module('app.modules.manage.users.edit', [
    'app.modules.manage.users.edit.organizations',
    'app.modules.manage.users.edit.projects',
    'app.modules.manage.users.edit.standards'
]);


// ****************************************************************************
// Configure 
//

m.config(['$urlRouterProvider', '$stateProvider', 'settings', ($urlRouterProvider: ng.ui.IUrlRouterProvider, $stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $urlRouterProvider.when('/manage/users/detail/:id', '/manage/users/detail/:id/organizations');

    $stateProvider
        .state('app.manage.users.edit', {
            url: '/detail/:id',
            data: {
                title: 'Edit User'
            },
            views: {
                'content@': {
                    templateUrl: `${settings.moduleBaseUri}/Manage/Users/Edit/UsersEditView.tpl.html`,
                    controller: 'app.modules.manage.users.edit',
                    controllerAs: 'usersEditViewModel'
                }
            },
            resolve: {
                user: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.users.find($stateParams.id);
                }]
            }
        });
}]);


// ****************************************************************************
// Controler app.modules.manage.users.edit
//

m.controller('app.modules.manage.users.edit', ['$scope', '$stateParams', 'repositories', 'services', 'user',
    function ($scope, $stateParams, repositories: IRepositories, services: IServices, user) {

        services.logger.debug('Loaded controller app.modules.manage.users.edit');

        var vm = this;

        vm.user = user;

        vm.tabs = [
            { link: 'app.manage.users.edit.organizations', label: 'Organizations' },
            { link: 'app.manage.users.edit.projects', label: 'Projects' },
            { link: 'app.manage.users.edit.standards', label: 'Standards' }
        ];

        vm.onPage = sref => (services.state.current.name === sref);
        vm.showTabs = () => services.underscore.some(<Array<any>>vm.tabs, tab => services.state.is(tab.link));

        vm.resetPassword = () => {
            return repositories.users.forgotPassword(vm.user.Email).then(() => {
                services.logger.success('Sent reset email.');
            }, (error) => {
                services.logger.error('Error sending reset email.', error.data);
            });
        }

        vm.resendEmail = () => {
            return repositories.users.resendEmail(vm.user.Id).then(() => {
                services.logger.success('Sent reset email.');
            }, (error) => {
                services.logger.error('Error sending reset email.', error.data);
            });
        }

        vm.toggleActive = () => {
            return repositories.users.toggleActive(vm.user.Id).then(() => {
                user.IsActive = !user.IsActive;
                if (user.IsActive) services.logger.success('Activated account.');
                else services.logger.success('Deactivated account.');
            }, (error) => {
                if (user.IsActive) services.logger.error('Error deactivating account.', error.data);
                else services.logger.error('Error activating account.', error.data);
            });
        }
    }
]);
