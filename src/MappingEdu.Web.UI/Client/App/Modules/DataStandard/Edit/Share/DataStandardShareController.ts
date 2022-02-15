// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.data-standard.edit.share
//

var m = angular.module('app.modules.data-standard.edit.share', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.data-standard.edit.share', {
            url: '/share',
            data: {
                title: 'Share',
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Edit')].Id
            },
            templateUrl: `${settings.moduleBaseUri}/DataStandard/Edit/Share/DataStandardShareView.tpl.html`,
            controller: 'app.data-standard.edit.share',
            controllerAs: 'dataStandardShareViewModel',
            resolve: {
                users: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.dataStandard.getUsers($stateParams.dataStandardId);
                }],
                me: ['services', (services: IServices) => {
                    return services.profile.me();
                }]
            }
        });
}]);


// ****************************************************************************
// Controller app.data-standard.edit.share
//

m.controller('app.data-standard.edit.share', ['$scope', '$stateParams', 'me', 'users', 'repositories', 'services', 'modals',
    function ($scope, $stateParams: any, me, users, repositories: IRepositories, services: IServices, modals: IModals) {

        services.logger.debug('Loaded controller app.data-standard.edit.share');
        services.timeout(() => { $scope.selected = {} }, 1000);
        $scope.$parent.dataStandardDetailViewModel.setTitle('SHARE');

        var vm = this;

        vm.usersByEmail = [];
        vm.dataStandard = $scope.$parent.dataStandardDetailViewModel.dataStandard;
        vm.dataStandard.Users = users;

        vm.me = me;

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
                emptyTable: 'This Data Standard has not been shared with anyone'
            })
            .withOption('aoColumns', [
                null, { "sSortDataType": 'dom-select' }, null
            ]);

        vm.columnDefs = [
            services.datatables.columnDefBuilder.newColumnDef(2).notSortable()
        ];
    
        //If current user is in the list sets their role
        var index = vm.dataStandard.Users.map(x => x.Id).indexOf(vm.me.Id);
        if (index >= 0)
            vm.me.Role = vm.dataStandard.Users[index].Role;

        vm.updateUser = (user) => {
            repositories.dataStandard.addUser(vm.dataStandard.DataStandardId, user).then(() => {
                services.logger.success('Updated access level.');
            }, error => {
                services.logger.error('Error updating access level.');
            });
        }

        vm.removeUser = (user, index) => {
            repositories.dataStandard.removeUser(vm.dataStandard.DataStandardId, user.Id).then(() => {
                vm.dataStandard.Users.splice(index, 1);
                services.logger.success('Removed access level.');
            }, error => {
                services.logger.error('Error removing access level.', error);
            });
        }

        vm.togglePublic = () => {
            repositories.dataStandard.togglePublic(vm.dataStandard.DataStandardId).then(() => {
                vm.dataStandard.IsPublic = !vm.dataStandard.IsPublic;
                services.logger.success('Changed public status.');
            }, error => {
                services.logger.error('Error changing public status.', error);
            });
        }

        vm.togglePublicExtensions = () => {
            repositories.dataStandard.togglePublicExtensions(vm.dataStandard.DataStandardId).then(() => {
                vm.dataStandard.AreExtensionsPublic = !vm.dataStandard.AreExtensionsPublic;
                services.logger.success('Changed public status.');
            }, error => {
                services.logger.error('Error changing public status.', error);
            });
        }

        vm.share = () => {

            var instance = modals.share(repositories.dataStandard, 'Standard', vm.dataStandard.DataStandardId);

            //On Return
            instance.result.then((shares) => {
                angular.forEach(shares, share => {
                    var index = vm.dataStandard.Users.map(x => x.Id).indexOf(share.Id);
                    if (index === -1) vm.dataStandard.Users.push(angular.copy(share));
                    else vm.dataStandard.Users[index] = angular.copy(share);
                });
            });
        }

        vm.shareExtension = () => {

            var instance = modals.share(repositories.dataStandard, 'Standard', vm.dataStandard.DataStandardId);

            //On Return
            instance.result.then((shares) => {
                angular.forEach(shares, share => {
                    var index = vm.dataStandard.Users.map(x => x.Id).indexOf(share.Id);
                    if (index === -1) vm.dataStandard.Users.push(angular.copy(share));
                    else vm.dataStandard.Users[index] = angular.copy(share);
                });
            });
        }
 }]);
