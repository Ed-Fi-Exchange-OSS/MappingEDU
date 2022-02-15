// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.element-group
//

var m = angular.module('app.modules.element-group.detail', [
    'app.modules.element-group.detail.actions',
    'app.modules.element-group.detail.info'
]);


// ****************************************************************************
// Configure 
//

m.config(['$urlRouterProvider', '$stateProvider', 'settings', 'enumerations',
    ($urlRouterProvider: ng.ui.IUrlRouterProvider, $stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $urlRouterProvider.when('/element-group/:id?mappingProjectId&dataStandardId', '/element-group/:id/info?mappingProjectId&dataStandardId');

    $stateProvider
        .state('app.element-group.detail', {
            url: '/:id?mappingProjectId&dataStandardId',
            data: {
                title: 'Standard and Element Group Detail',
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Guest')].Id
            },
            views: {
                'content@': {
                    templateUrl: `${settings.moduleBaseUri}/ElementGroup/Detail/ElementGroupDetailView.tpl.html`,
                    controller: 'app.modules.element-group.detail',
                    controllerAs: 'elementGroupDetailViewModel'
                }
            },
            resolve: {
                access: ['services', '$stateParams', (services: IServices, $stateParams) => {
                    if ($stateParams.dataStandardId)
                        return services.profile.dataStandardAccess($stateParams.dataStandardId);
                    else
                        return services.profile.mappingProjectAccess($stateParams.mappingProjectId);
                }],
                elementGroup: ['repositories', '$stateParams', (repositories: IRepositories, $stateParams) => {
                    return repositories.systemItem.detail($stateParams.id, $stateParams.mappingProjectId);
                }],
                model: ['repositories', '$stateParams', (repositories: IRepositories, $stateParams) => {
                    if ($stateParams.mappingProjectId)
                        return repositories.mappingProject.find($stateParams.mappingProjectId);
                    else
                        return repositories.dataStandard.find($stateParams.dataStandardId);
                }]
            }
        });
}]);


// ****************************************************************************
// Controller app.modules.element-group.detail
//

m.controller('app.modules.element-group.detail', [ '$scope', '$stateParams', 'repositories', 'services', 'modals', 'elementGroup', 'model',
    function ($scope, $stateParams, repositories: IRepositories, services: IServices, modals: IModals, elementGroup, model) {

        services.logger.debug('Loaded controller app.modules.element-group.detail');

        var vm = this;
        vm.pageTitle = 'Element Group Detail';

        vm.mappingProjectId = $stateParams.mappingProjectId;
        vm.dataStandardId = $stateParams.dataStandardId;
        vm.systemItemId = $stateParams.id;
        vm.elementGroup = elementGroup;

        if (vm.mappingProjectId) vm.mappingProject = model;
        else vm.dataStandard = model;

        services.profile.me().then((data) => {
            var index = (vm.dataStandardId !== vm.emptyGuid) ? data.MappedSystems.map(x => x.Id).indexOf(vm.dataStandardId) : data.Projects.map(x => x.Id).indexOf(vm.mappingProjectId);
            vm.onPage = sref => (services.state.current.name === sref);
            if (index >= 0) {
                var model = (vm.dataStandardId !== vm.emptyGuid) ? data.MappedSystems[index] : data.Projects[index];
                if (model.Role > 1 || data.IsAdministrator) {
                    vm.tabs = [
                        { link: 'app.element-group.detail.info', label: 'Details' },
                        { link: 'app.element-group.detail.actions', label: 'Actions' }
                    ];
                } else if (model.Role == 1) {
                    vm.tabs = [
                        { link: 'app.element-group.detail.info', label: 'Details' }
                    ];
                } else {
                    vm.tabs = [];
                }
            }
            else if (data.IsAdministrator) {
                vm.tabs = [
                    { link: 'app.element-group.detail.info', label: 'Details' },
                    { link: 'app.element-group.detail.actions', label: 'Actions' }
                ];
            }

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
        });

        vm.edit = elementGroup => {
            var instance = modals.systemItemForm(vm.dataStandardId, elementGroup, 1);
            instance.result.then(group => {
                vm.elementGroup.ItemName = group.ItemName;
                vm.elementGroup.IsExtended = group.IsExtended;
                vm.elementGroup.Definition = group.Definition;
            });
        };
    }
]);