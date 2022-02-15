// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.manage.standards
//

var m = angular.module('app.modules.manage.standards', []);


// ****************************************************************************
// Configure 
//
// Note: Eventually the 2 states should be combined
// 

m.config(['$urlRouterProvider', '$stateProvider', 'settings', ($urlRouterProvider: ng.ui.IUrlRouterProvider, $stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $stateProvider
        .state('app.manage.standards', {
            url: '/standards',
            data: {
                title: 'Manage Standards'
            },
            templateUrl: `${settings.moduleBaseUri}/Manage/Standards/StandardsManageView.tpl.html`,
            controller: 'app.modules.manage.standards',
            controllerAs: 'manageStandardsViewModel',
            resolve: {
                standards: [
                    'repositories', (repositories: IRepositories) => {
                        return repositories.dataStandard.getAll();
                    }
                ]
            }
        });
}]);


// ****************************************************************************
// Controler app.modules.manage.standards
//

m.controller('app.modules.manage.standards', ['standards', 'repositories', 'services',
    function (standards, repositories: IRepositories, services: IServices) {

        services.logger.debug('Loaded controller app.modules.manage.standards');

        var vm = this;
        vm.standards = standards;
        vm.filter = 'All';

        vm.standardHref = (standard) => {
            return services.state.href('app.data-standard.edit', { dataStandardId: standard.DataStandardId });
        }

        vm.delete = (standard, index) => {
            repositories.dataStandard.remove(standard.DataStandardId).then(data => {
                services.logger.success('Deleted data standard.');
                vm.standards.splice(index, 1);
            }, error => {
                services.logger.error('Error deleting data standard.', error.data);
            });
        }

        vm.filterBy = (filter) => {
            vm.loading = true;
            if (filter === 'Orphaned') {
                repositories.dataStandard.getAllOrphaned().then(data => {
                    vm.standards = data;
                    vm.filter = 'Orphaned';
                }).finally(() => vm.loading = false);
            } else if (filter === 'Public') {
                repositories.dataStandard.getAllPublic().then(data => {
                    vm.standards = data;
                    vm.filter = 'Public';
                }).finally(() => vm.loading = false);
            } else {
                repositories.dataStandard.getAll().then(data => {
                    vm.standards = data;
                    vm.filter = 'All';
                }).finally(() => vm.loading = false);
            }
        }
    }
]);
