// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.manage.organizations.edit.users
//

var m = angular.module('app.modules.manage.organizations.edit.users', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $stateProvider
        .state('app.manage.organizations.edit.users', {
            url: '/users',
            data: {
                title: 'Organization Users'
            },
            templateUrl: `${settings.moduleBaseUri}/Manage/Organizations/Edit/Users/OrganizationUsers.tpl.html`,
            controller: 'app.modules.manage.organizations.edit.users',
            controllerAs: 'organizationUsersViewModel',
            resolve: {
                users: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.users.getAll();
                }],
                organizationUsers: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.organizations.getUsers($stateParams.id);
                }]
            }
        });
}]);

// ****************************************************************************
// Controller app.modules.manage.organizations.edit.users
//

m.controller('app.modules.manage.organizations.edit.users', ['$scope', '$stateParams', 'users', 'organizationUsers', 'repositories', 'services',
    function ($scope, $stateParams, users, organizationUsers, repositories: IRepositories, services: IServices) {

        services.logger.debug('Loading app.modules.manage.organizations.edit.users');
        $scope.user = {};

        var vm = this;
        vm.allUsers = angular.copy(users);
        angular.forEach(vm.allUsers, (user) => {
            user.FullName = user.FirstName + ' ' + user.LastName;
        });
        vm.organizationUsers = organizationUsers;

        vm.dtOptions = services.datatables.optionsBuilder.newOptions()
            .withOption('language', {
                emptyTable: 'This organization does not contain any users'
            });

        vm.setUsers = () => {
            vm.users = angular.copy(vm.allUsers);
            angular.forEach(vm.organizationUsers, (user) => {
                var index = vm.users.map(x => x.Id).indexOf(user.Id);
                if (index >= 0)
                    vm.users.splice(index, 1);
            });
        }

        vm.setUsers();

        vm.addUserToOrganization = (user) => {
            return repositories.organizations.addUser($stateParams.id, user.Id).then(() => {

                services.logger.success('Added user to organization.');
                $scope.user.Id = null;

                repositories.organizations.getUsers($stateParams.id).then( data => {
                    vm.organizationUsers = data;
                    vm.setUsers();
                }, error => {
                    services.logger.error('Error loading organization users.', error.data);
                });
            }, error => {
                services.logger.error('Error adding user to organization', error.data);
            });
        }

        vm.deleteUser = (user, index) => {
            repositories.organizations.removeUser($stateParams.id, user.Id).then(() => {
                services.logger.success('Removed from organization.');
                vm.organizationUsers.splice(index, 1);
                vm.setUsers();
            });
        }
    }
]);