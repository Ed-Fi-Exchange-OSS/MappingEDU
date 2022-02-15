// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.element-group
//

var m = angular.module('app.modules.entity.detail', [
    'app.modules.entity.detail.actions',
    'app.modules.entity.detail.info'
]);


// ****************************************************************************
// Configure 
//

m.config(['$urlRouterProvider', '$stateProvider', 'settings', ($urlRouterProvider: ng.ui.IUrlRouterProvider, $stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $urlRouterProvider.when('/entity/:id?mappingProjectId&dataStandardId', '/entity/:id/info?mappingProjectId&dataStandardId');

    $stateProvider
        .state('app.entity.detail', {
            url: '/:id?mappingProjectId&dataStandardId',
            data: {
                title: 'Standard and Element Group Detail'
            },
            views: {
                'content@': {
                    templateUrl: `${settings.moduleBaseUri}/Entity/Detail/EntityDetailView.tpl.html`,
                    controller: 'app.modules.entity.detail',
                    controllerAs: 'entityDetailViewModel'
                }
            },
            resolve: {
                access: ['services', '$stateParams', (services: IServices, $stateParams) => {
                    if ($stateParams.dataStandardId)
                        return services.profile.dataStandardAccess($stateParams.dataStandardId);
                    else
                        return services.profile.mappingProjectAccess($stateParams.mappingProjectId);
                }],
                entity: ['repositories', '$stateParams', (repositories: IRepositories, $stateParams) => {
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
// Controller app.modules.entity.detail
//

m.controller('app.modules.entity.detail', ['$scope', '$stateParams', 'modals', 'repositories', 'services', 'model', 'entity',
    function ($scope, $stateParams, modals: IModals, repositories: IRepositories, services: IServices, model, entity) {

        services.logger.debug('Loaded controller app.modules.entity.detail');

        var vm = this;
        vm.pageTitle = 'Entity Detail';

        vm.mappingProjectId = $stateParams.mappingProjectId;
        vm.dataStandardId = $stateParams.dataStandardId;
        vm.systemItemId = $stateParams.id;
        vm.entity = entity;
        vm.entity.PathSegments.pop(); //Removes last one from path

        if (vm.mappingProjectId) vm.mappingProject = model;
        else vm.dataStandard = model;

        services.profile.me().then((data) => {
            var index = (vm.dataStandardId !== vm.emptyGuid) ? data.MappedSystems.map(x => x.Id).indexOf(vm.dataStandardId) : data.Projects.map(x => x.Id).indexOf(vm.mappingProjectId);
            vm.onPage = sref => (services.state.current.name === sref);
            if (index >= 0) {
                var model = (vm.dataStandardId !== vm.emptyGuid) ? data.MappedSystems[index] : data.Projects[index];
                if (model.Role > 1 || data.IsAdministrator) {
                    vm.tabs = [
                        { link: 'app.entity.detail.info', label: 'Details' },
                        { link: 'app.entity.detail.actions', label: 'Actions' }
                    ];
                } else if (model.Role === 1) {
                    vm.tabs = [
                        { link: 'app.entity.detail.info', label: 'Details' }
                    ];
                } else {
                    vm.tabs = [];
                }
            }
            else if (data.IsAdministrator) {
                vm.tabs = [
                    { link: 'app.entity.detail.info', label: 'Details' },
                    { link: 'app.entity.detail.actions', label: 'Actions' }
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

        vm.edit = () => {
            var instance = modals.systemItemForm(vm.dataStandardId, vm.entity, vm.entity.ItemTypeId);
            instance.result.then((data) => {
                vm.entity.ItemName = data.ItemName;
                vm.entity.IsExtended = data.IsExtended;
                vm.entity.Definition = data.Defintion;
            });
        }
    }
]);