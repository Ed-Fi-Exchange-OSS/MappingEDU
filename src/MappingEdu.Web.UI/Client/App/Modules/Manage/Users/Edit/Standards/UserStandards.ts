// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.manage.users.standards
//

var m = angular.module('app.modules.manage.users.edit.standards', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $stateProvider
        .state('app.manage.users.edit.standards', {
            url: '/standards',
            data: {
                title: 'User Standards'
            },
            views: {
                'user-info': {
                    templateUrl: `${settings.moduleBaseUri}/Manage/Users/Edit/Standards/UserStandards.tpl.html`,
                    controller: 'app.modules.manage.users.edit.user-standards',
                    controllerAs: 'userStandardsViewModel',
                },
                'guest-info': {
                    templateUrl: `${settings.moduleBaseUri}/Manage/Users/Edit/Standards/PublicUserStandards.tpl.html`,
                    controller: 'app.modules.manage.users.edit.guest-standards',
                    controllerAs: 'userStandardsViewModel',
                }
            },
            resolve: {
                standards: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.dataStandard.getAll();
                }],
                userStandards: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.users.getStandards($stateParams.id);
                }]
            }
        });
}]);

// ****************************************************************************
// Controller app.modules.manage.users.edit.user-standards
//

m.controller('app.modules.manage.users.edit.user-standards', ['$scope', '$stateParams', 'standards', 'userStandards', 'repositories', 'services',
    function ($scope, $stateParams, standards, userStandards, repositories: IRepositories, services: IServices) {

        services.logger.debug('Loading app.modules.manage.users.edit.standards');
        $scope.standard = {};

        var vm = this;
        vm.userStandards = userStandards;
        vm.standards = standards;

        vm.permissonOptions = [
            {
                value: 1,
                label: 'Can View'
            },
            {
                value: 2,
                label: 'Can Edit'
            },
            {
                value: 99,
                label: 'Owner'
            }
        ];

        vm.dtOptions = services.datatables.optionsBuilder.newOptions()
            .withOption('language', {
                emptyTable: 'This user has not been shared on any standards'
            })
            .withOption('aoColumns', [
                null, null, { "sSortDataType": 'dom-select' }, null
            ]);

        vm.getUserStandards = () => {
            repositories.users.getStandards($stateParams.id).then((standards: Array<IUserDataStandard>) => {
                vm.userStandards = angular.copy(standards);
            }, error => {
                services.logger.error('Error loading standards shared with the user.', error.data);
            });
        }

        vm.addUserToStandard = (email, standard) => {
            var user: IShareStandardToUser = { Email: email, Role: standard.Role };
            return repositories.dataStandard.addUser(standard.Id, user).then(() => {
                services.logger.success('Added user to standard.');
                vm.getUserStandards();
                standard.Id = null;
                standard.Role = null;
            }, error => {
                services.logger.error('Error updating access level.', error.data);
            });
        }

        vm.updateStandardRole = (email, standard) => {
            var user: IShareStandardToUser = { Email: email, Role: standard.Role };
            repositories.dataStandard.addUser(standard.Id, user).then(() => {
                services.logger.success('Updated acess level.');
            }, error => {
                services.logger.error('Error updating access level.', error.data);
            });
        }

        vm.removeStandardRole = (standard, user, index) => {
            repositories.dataStandard.removeUser(standard.Id, user.Id).then(() => {
                vm.userStandards.splice(index, 1);
                services.logger.success('Removed access level.');
            }, error => {
                services.logger.error('Error removing access level.', error.data);
            });
        }
    }
]);

// ****************************************************************************
// Controller app.modules.manage.users.edit.guest-standards
//

m.controller('app.modules.manage.users.edit.guest-standards', ['$scope', '$stateParams', 'standards', 'repositories', 'services',
    function ($scope, $stateParams, standards, repositories: IRepositories, services: IServices) {

        services.logger.debug('Loading app.modules.manage.users.edit.guest-standards');

        $scope.selected = {};

        var vm = this;
        vm.standards = standards;
        vm.instance = {};

        vm.dtOptions = services.datatables.optionsBuilder.newOptions()
            .withOption('language', {
                emptyTable: 'This user has not been shared on any projects'
            });

        vm.togglePublic = (standard) => {
            repositories.dataStandard.togglePublic(standard.DataStandardId).then(() => {
                standard.IsPublic = !standard.IsPublic;
                $scope.selected = {};
                vm.instance.rerender();
                services.logger.success('Changed public status.');
            }, error => {
                services.logger.error('Error changing public status.', error);
            });
        }
    }
]);