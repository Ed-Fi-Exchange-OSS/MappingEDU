// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.element-group.detail.actions
//

var m = angular.module('app.modules.element-group.detail.actions', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.element-group.detail.actions', {
            url: '/actions',
            data: {
                title: 'Element Detail Mapping',
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Edit')].Id
            },
            templateUrl: `${settings.moduleBaseUri}/ElementGroup/Detail/Actions/ElementGroupDetailActionsView.tpl.html`,
            controller: 'app.modules.element-group.detail.actions',
            controllerAs: 'elementDetailActionsViewModel'
        });
}]);


// ****************************************************************************
// Controller app.modules.element-group.detail.actions
//

m.controller('app.modules.element-group.detail.actions', ['$scope', '$stateParams', 'repositories', 'services', 'modals', 'elementGroup', 'model',
    function ($scope, $stateParams, repositories: IRepositories, services: IServices, modals: IModals, elementGroup, model) {

        services.logger.debug('Loaded controller app.modules.element-group.detail.actions');

        var vm = this;

        vm.id = $stateParams.id;
        vm.dataStandardId = $stateParams.dataStandardId;
        vm.mappingProjectId = $stateParams.mappingProjectId;

        if ($stateParams.dataStandardId)
            vm.dataStandard = model;
        else
            vm.mappingProject = model;

        vm.elementGroup = elementGroup;

        vm.delete = () => {
            repositories.elementGroup.remove(vm.dataStandardId, vm.id)
                .then(() => {
                    services.logger.success('Removed element group.');
                    vm.deleted = true;
                    services.timeout(() => { services.state.go('app.data-standard.edit.groups', { dataStandardId: vm.dataStandardId }); }, 1000);
                }, error => {
                    services.logger.error('Error removing element group.', error.data);
                })
                .catch(function (error) {
                    services.errors.handleErrors(error, this);
                });
        }

        vm.modifyMappings = () => {
         
            var instance = modals.bulkMapping(vm.mappingProject, vm.elementGroup, 1);

            instance.result.then((data) => {
                vm.showResultsMsg = true;
                vm.resultsCount = data.CountUpdated;

                repositories.systemItem.detail($stateParams.id, $stateParams.mappingProjectId).then(group => {
                    elementGroup.ChildSystemItems = group.ChildSystemItems;
                });
            });
        }
    }
]);
