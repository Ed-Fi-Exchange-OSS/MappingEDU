// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.element.detail.actions
//

var m = angular.module('app.modules.element.detail.actions', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.element.detail.actions', {
            url: '/actions',
            data: {
                title: 'Element Detail Mapping',
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Edit')].Id
            },
            templateUrl: `${settings.moduleBaseUri}/Element/Detail/Actions/ElementDetailActionsView.tpl.html`,
            controller: 'app.modules.element.detail.actions',
            controllerAs: 'elementDetailActionsViewModel',
            resolve: {
                mapping: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    if ($stateParams.mappingProjectId) return repositories.element.mapping.findByProject($stateParams.elementId, $stateParams.mappingProjectId);
                    else return null;
                }]
            }
        });
}]);


// ****************************************************************************
// Controller app.modules.element.detail.actions
//

m.controller('app.modules.element.detail.actions', ['$scope', '$stateParams', 'repositories', 'services', 'mapping', 'element',
    function ($scope, $stateParams, repositories: IRepositories, services: IServices, mapping, element) {

        services.logger.debug('Loaded controller app.modules.element.detail.actions');

        var vm = this;
        vm.mappingProjectId = $stateParams.mappingProjectId;
        vm.dataStandardId = $stateParams.dataStandardId;
        vm.systemItemId = $stateParams.elementId;
        vm.mapping = mapping;

        vm.delete = () => {
            if (vm.dataStandardId) {
                repositories.systemItem.remove(element).then(() => {
                    services.logger.success('Removed element.');
                    var value = services.session.cloneFromSession('navigation', 'goBack');
                    services.timeout(() => {
                        services.state.go(value.state.name, value.params);
                    }, 1000);
                }, error => {
                    services.logger.error('Error removing element.', error.data);
                });
            } else {
                if (vm.mapping) {
                    repositories.element.mapping.remove(vm.systemItemId, vm.mapping.SystemItemMapId).then(() => {
                        services.logger.success('Deleted mapping.');
                        mapping = {};
                        services.timeout(() => {
                            services.state.go('app.element.detail.mapping', { elementId: vm.systemItemId, mappingProjectId: vm.mappingProjectId });
                        }, 1000);
                    }, error => {
                        services.logger.error('Error removing mapping.', error.data);
                    });   
                }
                else services.logger.warning('To mapping to delete.');
            }
        }
    }
]);