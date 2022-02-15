// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.manage.users.organizations
//

var m = angular.module('app.modules.manage.users.edit.organizations', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $stateProvider
        .state('app.manage.users.edit.organizations', {
            url: '/organizations',
            data: {
                title: 'User Organizations'
            },
            templateUrl: `${settings.moduleBaseUri}/Manage/Users/Edit/Organizations/UserOrganizations.tpl.html`,
            controller: 'app.modules.manage.users.edit.organizations',
            controllerAs: 'userOrganizationsViewModel',
            resolve: {
                organizations: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.organizations.getAll();
                }],
                userOrganizations: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.users.getOrganizations($stateParams.id);
                }]
            }
        });
}]);

// ****************************************************************************
// Controller app.modules.manage.users.edit.organizations
//

m.controller('app.modules.manage.users.edit.organizations', ['$scope', '$stateParams', 'organizations', 'userOrganizations', 'repositories', 'services',
    function ($scope, $stateParams, organizations, userOrganizations, repositories: IRepositories, services: IServices) {

        services.logger.debug('Loading app.modules.manage.users.edit.organizations');
        $scope.organization = {};

        var vm = this;
        vm.allOrganizations = angular.copy(organizations);
        vm.userOrganizations = userOrganizations;


        vm.dtOptions = services.datatables.optionsBuilder.newOptions()
            .withOption('language', {
                emptyTable: 'This user is not apart of any organizations'
            });

        vm.setOrganizations = () => {
            vm.organizations = angular.copy(vm.allOrganizations);
            angular.forEach(vm.userOrganizations, (organization) => {
                var index = vm.organizations.map(x => x.Id).indexOf(organization.Id);
                if (index >= 0)
                    vm.organizations.splice(index, 1);
            });
        }

        vm.setOrganizations();

        vm.addUserToOrganization = (organization) => {
            return repositories.organizations.addUser(organization.Id, $stateParams.id).then(() => {

                services.logger.success('Added user to organization.');
                $scope.organization.Id = null;

                repositories.users.getOrganizations($stateParams.id).then( data => {
                    vm.userOrganizations = data;
                    vm.setOrganizations();
                }, error => {
                    services.logger.error('Error loading user organizations.', error.data);
                });

            }, error => {
                services.logger.error('Error adding user to organization.', error.data);
            });
        }

        vm.deleteOrganization = (organization, index) => {
            repositories.organizations.removeUser(organization.Id, $stateParams.id).then(() => {
                services.logger.success('Removed user from organization.');
                vm.userOrganizations.splice(index, 1);
                vm.setOrganizations();
            });
        }
    }
]);