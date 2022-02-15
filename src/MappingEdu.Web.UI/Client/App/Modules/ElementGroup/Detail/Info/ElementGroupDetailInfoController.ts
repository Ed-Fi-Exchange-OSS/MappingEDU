// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.element-group.detail.info
//

var m = angular.module('app.modules.element-group.detail.info', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.element-group.detail.info', {
            url: '/info',
            data: {
                title: 'Element Detail Mapping',
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Guest')].Id
            },
            templateUrl: `${settings.moduleBaseUri}/ElementGroup/Detail/Info/ElementGroupDetailInfoView.tpl.html`,
            controller: 'app.modules.element-group.detail.info',
            controllerAs: 'elementGroupDetailInfoViewModel'
        });
}]);


// ****************************************************************************
// Controller app.modules.element-group.detail.info
//

m.controller('app.modules.element-group.detail.info', ['$stateParams', 'modals', 'repositories', 'services', 'access', 'elementGroup',
    function ($stateParams, modals: IModals, repositories: IRepositories, services: IServices, access, elementGroup) {

        services.logger.debug('Loaded app.modules.element-group.detail.info');

        var vm = this;

        vm.mappingProjectId = $stateParams.mappingProjectId;
        vm.dataStandardId = $stateParams.dataStandardId;
        vm.elementGroup = elementGroup;
        vm.children = elementGroup.ChildSystemItems;

        services.profile.me().then(me => {
            vm.dtColumnDefs = [
                services.datatables.columnDefBuilder.newColumnDef(0),
                services.datatables.columnDefBuilder.newColumnDef(1).withOption('searchable', false)
            ];

            if (vm.dataStandardId && (access.Role > 1 || me.IsAdministrator)) {
                vm.dtColumnDefs.push(services.datatables.columnDefBuilder.newColumnDef(2));
                vm.dtColumnDefs.push(services.datatables.columnDefBuilder.newColumnDef(3));
            }
        });

        vm.dtOptions = services.datatables.optionsBuilder.newOptions();

        vm.enumerationsTableCallback = (instance) => {

            function setList() {
                var elements = [];
                var order = instance.DataTable.rows()[0];
                for (var i = 0; i < order.length; i++) {
                    elements.push({
                        ElementId: vm.enumerations[order[i]].SystemItemId
                    });
                }

                if (vm.mappingProjectId) services.session.cloneToSession('elementQueues', vm.mappingProjectId, elements);
                else services.session.cloneToSession('elementLists', vm.dataStandardId, elements);
            }

            setList();

            instance.DataTable.on('draw.dt', () => {
                setList();
            });

            instance.DataTable.on('init.dt', () => {
                var enumerationPage = services.session.cloneFromSession('enumerationPage', this.id);
                if (enumerationPage) instance.Datatable.page(enumerationPage).draw(false);
            });

            instance.DataTable.on('pgae.dt', () => {
                var info = instance.Datatable.page.info();
                services.session.cloneToSession('enumerationPage', this.id, info.page);
            });
        }

        vm.entitiesTableCallback = (instance) => {
            instance.DataTable.on('init.dt', () => {
                var entityPage = services.session.cloneFromSession('entitiesPage', this.id);
                if (entityPage) instance.Datatable.page(entityPage).draw(false);
            });

            instance.DataTable.on('pgae.dt', () => {
                var info = instance.Datatable.page.info();
                services.session.cloneToSession('entitiesPage', this.id, info.page);
            });
        }

        vm.setHref = (systemItem) => {
            var state = systemItem.ItemTypeId === 2 ? 'app.entity.detail' : 'app.element.detail';
            systemItem.Href = services.state.href(state, {
                id: systemItem.SystemItemId,
                elementId: systemItem.SystemItemId,
                mappingProjectId: vm.mappingProjectId,
                dataStandardId: vm.dataStandardId
            });
        }

        vm.entities = [];
        vm.enumerations = [];

        angular.forEach(vm.children, child => {
            vm.setHref(child);
            if (child.ItemTypeId === 2) vm.entities.push(child);
            else vm.enumerations.push(child);
        });

        vm.edit = (systemItem, itemType) => {
            var instance = modals.systemItemForm(vm.dataStandardId, systemItem, itemType);
            instance.result.then((data) => {
                systemItem.ItemName = data.ItemName;
                systemItem.Definition = data.Definition;
                systemItem.IsExtended = data.IsExtended;
            });
        }

        vm.delete = (systemItem, index) => {
            repositories.element.remove(systemItem)
                .then(() => {
                    services.logger.success('Removed element.');
                    if (systemItem.ItemTypeId === 2) vm.entities.splice(index, 2);
                    else vm.enumerations.splice(index, 1);
                }, error => {
                    services.logger.error('Error removing element.', error.data);
                });
        }

        vm.add = (itemType) => {
            var instance = modals.systemItemForm(vm.dataStandardId, {
                ParentSystemItemId: elementGroup.SystemItemId
            }, itemType);
            instance.result.then((data) => {
                vm.setHref(data);

                vm.elementGroup.ChildSystemItems.push(data);
                if (itemType === 2) vm.entities.push(data);
                else vm.enumerations.push(data);
            });
        }

    }
]);