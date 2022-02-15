// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.detail.share
//

var m = angular.module('app.modules.mapping-project.detail.share', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.mapping-project.detail.share', { 
            url: '/share',
            data: {
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Edit')].Id,
                roles: ['user'],
                title: 'Mapping Project Share'
            },
            templateUrl: `${settings.moduleBaseUri}/mappingProject/detail/share/mappingProjectShareView.tpl.html`,
            controller: 'app.modules.mapping-project.detail.share',
            controllerAs: 'mappingProjectShareViewModel',
            resolve: {
                users: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.mappingProject.getUsers($stateParams.id);
                }],
                me: ['services', (services: IServices) => {
                    return services.profile.me();
                }]
            }
        });
}]);


// ****************************************************************************
// Controller app.modules.mapping-project.detail.share
//

m.controller('app.modules.mapping-project.detail.share', ['$scope', '$stateParams', 'repositories', 'services', 'users', 'me', 'modals',
    function ($scope, $stateParams: any, repositories: IRepositories, services: IServices, users, me, modals: IModals) {

        services.logger.debug('Loaded controller app.modules.mapping-project.detail.share');
        $scope.$parent.mappingProjectDetailViewModel.setTitle('SHARE');

        var vm = this;

        vm.mappingProject = $scope.$parent.mappingProjectDetailViewModel.mappingProject;
        vm.mappingProject.Users = users;
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
                emptyTable: 'This Mapping Project has not been shared with anyone'
            })
            .withOption('aoColumns', [
                null, { "sSortDataType": 'dom-select' }, null
            ]);

        vm.columnDefs = [
            services.datatables.columnDefBuilder.newColumnDef(2).notSortable()
        ];

        //If current user is in the list set their roles
        var index = vm.mappingProject.Users.map(x => x.Id).indexOf(vm.me.Id);
        if (index >= 0)
            vm.me.Role = vm.mappingProject.Users[index].Role;

        vm.updateUser = (user) => {
            repositories.mappingProject.addUser(vm.mappingProject.MappingProjectId, user).then(() => {
                services.logger.success('Updated access level.');
            });
        }

        vm.removeUser = (user, index) => {
            vm.mappingProject.Users.splice(index, 1);
            repositories.mappingProject.removeUser(vm.mappingProject.MappingProjectId, user.Id).then(() => {
                services.logger.success('Removed access.');
                services.logger.warning(`<b>Reminder!</b> User still has access to ` +
                    `${vm.mappingProject.SourceDataStandard.SystemName} ${vm.mappingProject.SourceDataStandard.SystemVersion} and ` +
                    `${vm.mappingProject.TargetDataStandard.SystemName} ${vm.mappingProject.TargetDataStandard.SystemVersion}`);

            });
        }

        vm.togglePublic = () => {
            repositories.mappingProject.togglePublic(vm.mappingProject.MappingProjectId).then(() => {
                vm.mappingProject.IsPublic = !vm.mappingProject.IsPublic;
                services.logger.success('Changed public status.');
            }, error => {
                services.logger.error('Error changing public status.', error);
            });
        }

        vm.share = () => {

            var instance = modals.share(repositories.mappingProject, 'Project', vm.mappingProject.MappingProjectId);

            //On Return
            instance.result.then((shares) => {
                angular.forEach(shares, share => {
                    var index = vm.mappingProject.Users.map(x => x.Id).indexOf(share.Id);
                    if (index === -1) vm.mappingProject.Users.push(angular.copy(share));
                    else vm.mappingProject.Users[index] = angular.copy(share);
                });
            });
        }         
    }
]);
