// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.data-standard.edit
//

var m = angular.module('app.modules.data-standard.edit', [
    'app.modules.data-standard.edit.actions',
    'app.modules.data-standard.edit.browse',
    'app.modules.data-standard.edit.custom-details',
    'app.modules.data-standard.edit.elements',
    'app.modules.data-standard.edit.extensions',
    'app.modules.data-standard.edit.groups',
    'app.modules.data-standard.edit.info',
    'app.modules.data-standard.edit.projects',
    'app.modules.data-standard.edit.share',
    'app.modules.data-standard.edit.source',
    'app.modules.data-standard.edit.target'
]);


// ****************************************************************************
// Configure 
//

m.config(['$urlRouterProvider', '$stateProvider', 'settings', ($urlRouterProvider: ng.ui.IUrlRouterProvider, $stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $urlRouterProvider.when('/data-standard/detail/:id', '/data-standard/detail/:id/info');

    $stateProvider
        .state('app.data-standard.edit', {
            url: '/detail/:dataStandardId',
            data: {
                roles: ['user','guest'],
                title: 'Standard and Element Group Detail'
            },
            views: {
                'content@': {
                    templateUrl: `${settings.moduleBaseUri}/DataStandard/Edit/DataStandardEditView.tpl.html`,
                    controller: 'app.modules.data-standard.edit',
                    controllerAs: 'dataStandardDetailViewModel'
                }
            },
            resolve: {
                standard: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.dataStandard.find($stateParams.dataStandardId);
                }],
                init: ['services', '$stateParams', (services: IServices, $stateParams) => {
                    return services.profile.dataStandardAccess($stateParams.dataStandardId);
                }]
            }
            
});
}]);


// ****************************************************************************
// Controller app.modules.data-standard.edit
//

m.controller('app.modules.data-standard.edit', ['$scope', '$stateParams', 'repositories', 'services', 'enumerations', 'standard',
    function ($scope, $stateParams, repositories: IRepositories, services: IServices, enumerations: IEnumerations, standard) {

        services.logger.debug('Loaded controller app.modules.data-standard.edit');

        var vm = this;

        vm.id = $stateParams.dataStandardId;
        vm.dataStandard = standard;

        vm.setTitle = title => {
            vm.pageTitle = `Data Standard ${title}`;
        }

        services.profile.updateDataStandardAccess(vm.id);
        
        vm.getDataStandardDetails = () => {
            if (!vm.id)
                return;
            vm.loading = true;
            
            repositories.dataStandard.find(vm.id)
                .then(data => {
                    vm.dataStandard = data;
                }, error => {
                    // services.logger.error('Error loading data standard.', error.data); //TODO: Fix the directive (cpt)
                })
                .catch(error => {
                    services.errors.handleErrors(error, vm);
                })
                .finally(() => {
                    vm.loading = false;
                });
        }

        vm.onPage = sref => {return services.state.current.name === sref; }

        services.profile.me().then((data) => {
            vm.me = data;
            vm.tabs = [
                { link: 'app.data-standard.edit.info', label: 'Data Standard Info' },
                { link: 'app.data-standard.edit.groups', label: 'Element Groups' },
                { link: 'app.data-standard.edit.custom-details', label: 'Custom Details' },
                { link: 'app.data-standard.edit.projects', label: 'Mapping Projects' }
            ];

            var index = data.MappedSystems.map(x => x.Id).indexOf(vm.id);
            if (index >= 0) {
                var standard = data.MappedSystems[index];

                if (standard.Role > 2 || data.IsAdministrator) {
                    vm.tabs.push({ link: 'app.data-standard.edit.extensions.list', label: 'Extensions' });
                    vm.tabs.push({ link: 'app.data-standard.edit.actions', label: 'Actions' });
                    vm.tabs.push({ link: 'app.data-standard.edit.share', label: 'Share' });
                } else if (standard.Role > 1) {
                    if (vm.dataStandard.AreExtensionsPublic) vm.tabs.push({ link: 'app.data-standard.edit.extensions.report', label: 'Extensions' });
                    vm.tabs.push({ link: 'app.data-standard.edit.actions', label: 'Actions' });
                    vm.tabs.push({ link: 'app.data-standard.edit.share', label: 'Share' });
                } else {
                    if (vm.dataStandard.AreExtensionsPublic) vm.tabs.push({ link: 'app.data-standard.edit.extensions.report', label: 'Extensions' });
                    vm.tabs.push({ link: 'app.data-standard.edit.actions', label: 'Actions' });
                }
            } else if (data.IsAdministrator) {
                vm.tabs.push({ link: 'app.data-standard.edit.extensions.list', label: 'Extensions' });
                vm.tabs.push({ link: 'app.data-standard.edit.actions', label: 'Actions' });
                vm.tabs.push({ link: 'app.data-standard.edit.share', label: 'Share' });
            } else if (vm.dataStandard.IsPublic) {
                if (vm.dataStandard.AreExtensionsPublic) vm.tabs.push({ link: 'app.data-standard.edit.extensions.report', label: 'Extensions' });
                vm.tabs.push({ link: 'app.data-standard.edit.actions', label: 'Actions' });
            } else {
                if (vm.dataStandard.AreExtensionsPublic) vm.tabs.push({ link: 'app.data-standard.edit.extensions.report', label: 'Extensions' });
                vm.tabs = [];
            }
        });

        vm.showTabs = () => {
            var show = false;
            angular.forEach(vm.tabs, tab => {
                if (services.state.is(tab.link)) {
                    show = true;
                    return;
                }
            });
            return show;
        };
        
        angular.element(document.querySelector('#editDataStandardModal')).on('hidden.bs.modal', () => {
            vm.getDataStandardDetails();
        });

        vm.goToDataStandard = () => {
            services.state.go('app.data-standard.edit.info', { dataStandardId: vm.dataStandard.DataStandardId });
        }

        vm.getAccess = (standard) => {
            var html = '';
            if (standard.Role > 0) html += enumerations.UserAccess[enumerations.UserAccess.map(x => x.Id).indexOf(standard.Role)].DisplayText;
            if (vm.me && vm.me.IsAdministrator && standard.Role > 0) html += ', ';
            if (vm.me && vm.me.IsAdministrator) html += 'Admin';
            if (standard.IsPublic && (standard.Role > 0 || vm.me.IsAdministrator)) html += ', ';
            if (standard.IsPublic) html += 'Public';
            return html;
        }
    }
]);