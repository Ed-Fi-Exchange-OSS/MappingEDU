// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.element.detail.enumeration-details
//

var m = angular.module('app.modules.element.detail.enumeration-details', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $stateProvider
        .state('app.element.detail.enumeration-details', {
            url: '/details',
            data: {
                title: 'Element Detail Mapping'
            },
            templateUrl: `${settings.moduleBaseUri}/Element/Detail/Enumerations/EnumerationDetailsView.tpl.html`,
            controller: 'app.modules.element.detail.enumeration-details',
            controllerAs: 'enumerationDetailsViewModel',
            resolve: {
                enums: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.element.enumerationItem.getAll($stateParams.elementId);
                }],
                reroute: ['$stateParams', 'services', 'element', ($stateParams, services: IServices, element) => {
                    if (element.ItemTypeId === 4) {
                        if ($stateParams.dataStandardId) services.state.go('app.element.detail.info', {elementId: element.SystemItemId});
                        else services.state.go('app.element.detail.mapping', { elementId: element.SystemItemId });
                    }
                }]
            }
        });
}]);

// ****************************************************************************
// Controller app.modules.element.detail.enumeration-details
//

m.controller('app.modules.element.detail.enumeration-details', ['$scope', '$stateParams', 'modals', 'repositories', 'services', 'readOnly', 'element', 'enums',
    function enumerationDetailsController($scope, $stateParams, modals: IModals, repositories: IRepositories, services: IServices, readOnly, element, enums) {

        services.logger.debug('Loaded controller app.modules.element.detail.enumeration-details');

        var vm = this;

        vm.mappingProjectId = $stateParams.mappingProjectId;;
        vm.dataStandardId = $stateParams.dataStandardId;
        vm.readOnly = readOnly;
        vm.enumerations = enums;
        vm.systemItem = element;

        vm.edit = enumerationItem => {
            var instance = modals.enumerationItemForm(enumerationItem, vm.systemItem.SystemItemId);
            instance.result.then((data) => {
                enumerationItem.CodeValue = data.CodeValue;
                enumerationItem.ShortDescription = data.ShortDescription;
                enumerationItem.Description = data.Description;
            });
        };

        vm.add = () => {
            var instance = modals.enumerationItemForm({}, vm.systemItem.SystemItemId);
            instance.result.then((data) => {
                vm.enumerations.push(data);
            });
        };

        vm.delete = (enumerationItem, index) => {
            repositories.element.enumerationItem.remove(vm.systemItem.SystemItemId, enumerationItem.SystemEnumerationItemId).then(() => {
                services.logger.success('Removed enumeration item.');
                vm.enumerations.splice(index, 1);
            }, error => {
                services.logger.error('Error removing enumeration item.', error.data);
            });
        }

        vm.dtOptions = services.datatables.optionsBuilder.newOptions();

        vm.dtColumnDefs = [
            services.datatables.columnDefBuilder.newColumnDef(3).notSortable(),
            services.datatables.columnDefBuilder.newColumnDef(4).notSortable()
        ];

        if ($stateParams.dataStandardId) {
            services.profile.me().then(me => {
                vm.me = me;
                services.profile.dataStandardAccess(vm.dataStandardId).then((data) => {
                    vm.readOnly = (data.Role < 2 && !me.IsAdministrator);

                    if (vm.readOnly) {
                        vm.dtColumnDefs[0].notVisible();
                        vm.dtColumnDefs[1].notVisible();
                    }
                });
            });
        } else {
            vm.dtColumnDefs[0].notVisible();
            vm.dtColumnDefs[1].notVisible();
        }

    }
]);