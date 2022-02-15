// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.detail.settings
//

var m = angular.module('app.modules.mapping-project.detail.settings', [
    'app.modules.mapping-project.detail.settings.entity-hints',
    'app.modules.mapping-project.detail.settings.select-entity',
    'app.modules.mapping-project.detail.settings.synonyms',
    'app.modules.mapping-project.detail.settings.synonym-form',
    'app.modules.mapping-project.detail.settings.templates',
    'app.modules.mapping-project.detail.settings.template-form']);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.mapping-project.detail.settings', { 
            url: '/settings',
            data: {
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Edit')].Id,
                roles: ['user'],
                title: 'Mapping Project Settings'
            },
            templateUrl: `${settings.moduleBaseUri}/mappingProject/detail/settings/mappingProjectSettingsView.tpl.html`,
            controller: 'app.modules.mapping-project.detail.settings',
            controllerAs: 'mappingProjectSettingsViewModel'
        });
}]);


// ****************************************************************************
// Controller app.modules.mapping-project.detail.settings
//

m.controller('app.modules.mapping-project.detail.settings', ['$scope', 'settings', '$stateParams', 'repositories', 'services',
    function ($scope, settings: ISystemSettings, $stateParams: any, repositories: IRepositories, services: IServices) {

        services.logger.debug('Loaded controller app.modules.mapping-project.detail.settings');
        $scope.$parent.mappingProjectDetailViewModel.setTitle('SETTINGS');

        var vm = this;
        vm.mappingProjectId = $stateParams.id;

        vm.mappingProject = $scope.$parent.mappingProjectDetailViewModel.mappingProject;

        vm.entityHintModal = () => {
            var model = {
                backdrop: 'static',
                keyboard: false,
                templateUrl: `${settings.moduleBaseUri}/mappingProject/detail/settings/entity/EntityHints.tpl.html`,
                controller: 'app.modules.mapping-project.detail.settings.entity-hints',
                windowClass: 'modal-xl',
                resolve: {
                    project: () => { return vm.mappingProject; },
                    hints: () => {
                        return repositories.entity.hint.getAll(vm.mappingProjectId);
                   }
                }
            }
            services.modal.open(model);
        }

        vm.synonymModal = () => {

            var model = {
                backdrop: 'static',
                keyboard: false,
                templateUrl: `${settings.moduleBaseUri}/mappingProject/detail/settings/synonym/synonyms.tpl.html`,
                controller: 'app.modules.mapping-project.detail.settings.synonyms',
                size: 'lg',
                resolve: {
                    project: () => { return vm.mappingProject },
                    synonyms: () => { return repositories.mappingProject.synonym.getAll(vm.mappingProjectId); }
                }
            }
           services.modal.open(model);
        }

        vm.templateModal = () => {

            var model = {
                backdrop: 'static',
                keyboard: false,
                templateUrl: `${settings.moduleBaseUri}/mappingProject/detail/settings/template/templates.tpl.html`,
                controller: 'app.modules.mapping-project.detail.settings.templates',
                size: 'lg',
                resolve: {
                    project: () => { return vm.mappingProject },
                    templates: () => { return repositories.mappingProject.template.getAll(vm.mappingProjectId); }
                }
            }
            services.modal.open(model);
        }
    }
]);
