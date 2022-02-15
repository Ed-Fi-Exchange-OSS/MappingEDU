// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.entity.detail.actions
//

var m = angular.module('app.modules.entity.detail.actions', []);


// ****************************************************************************
// Configure 
//

m.config(['$urlRouterProvider', '$stateProvider', 'settings', 'enumerations', ($urlRouterProvider: ng.ui.IUrlRouterProvider, $stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $urlRouterProvider.when('/entity/:id?mappingProjectId&dataStandardId', '/entity/:id/info?mappingProjectId&dataStandardId');

    $stateProvider
        .state('app.entity.detail.actions', {
            url: '/actions',
            data: {
                title: 'Element Group Detail',
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Edit')].Id
            },
            templateUrl: `${settings.moduleBaseUri}/Entity/Detail/Actions/EntityDetailActionsView.tpl.html`,
            controller: 'app.modules.entity.detail.actions',
            controllerAs: 'entityDetailActionsViewModel'
        });
}]);


// ****************************************************************************
// Controller app.modules.entity.detail.actions
//

m.controller('app.modules.entity.detail.actions', ['$scope', '$stateParams', 'repositories', 'services', 'modals', 'entity', 'model',
    function ($scope, $stateParams, repositories: IRepositories, services: IServices, modals: IModals, entity, model) {

        services.logger.debug('Loaded controller app.modules.entity.detail.actions');

        var vm = this;

        vm.id = $stateParams.id;
        vm.dataStandardId = angular.copy($stateParams.dataStandardId);
        vm.mappingProjectId = angular.copy($stateParams.mappingProjectId);
        vm.entity = entity;

        if (vm.dataStandardId) vm.dataStandard = model;
        else vm.mappingProject = model;

        vm.delete = () => {
            repositories.systemItem.remove({ SystemItemId: vm.id })
                .then(() => {
                    services.state.go('app.data-standard.edit.groups', { dataStandardId: vm.dataStandardId });
                    services.logger.success('Rmoved entity.');
                    vm.deleted = true;
                }, error => {
                    services.logger.error('Error removing entity.', error.data);
                })
                .catch(error => {
                    services.errors.handleErrors(error, vm);
                });
        }

        vm.modifyMappings = () => {

            var instance = modals.bulkMapping(
                vm.mappingProject,
                vm.entity,
                2);

            instance.result.then((data) => {
                vm.showResultsMsg = true;
                vm.resultsCount = data.CountUpdated;

                repositories.systemItem.detail(vm.id, vm.mappingProjectId).then(detail => {
                    entity.ChildSystemItems = detail.ChildSystemItems;
                });
            });
        }
    }
]);
