// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.entity.detail.info
//

var m = angular.module('app.modules.entity.detail.info', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $stateProvider
        .state('app.entity.detail.info', {
            url: '/info',
            data: {
                title: 'Entity Detail Mapping'
            },
            templateUrl: `${settings.moduleBaseUri}/Entity/Detail/Info/EntityDetailInfoView.tpl.html`,
            controller: 'app.modules.entity.detail.info',
            controllerAs: 'entityDetailInfoViewModel',
            resolve: {
                init: ['services', '$stateParams', (services: IServices, $stateParams) => {
                    if ($stateParams.dataStandardId)
                        return services.profile.dataStandardAccess($stateParams.dataStandardId);
                    else
                        return services.profile.mappingProjectAccess($stateParams.mappingProjectId);
                }]
            }

});
}]);


// ****************************************************************************
// Controller app.modules.entity.detail.info
//

m.controller('app.modules.entity.detail.info', ['$', '$scope', '$stateParams', 'enumerations', 'modals', 'repositories', 'services','entity', 'model',
    function ($, $scope, $stateParams, enumerations: IEnumerations, modals: IModals, repositories: IRepositories, services: IServices, entity, model) {

        services.logger.debug('Loaded controller app.modules.entity.detail.info');

        var vm = this;

        vm.mappingProjectId = $stateParams.mappingProjectId;
        vm.dataStandardId = $stateParams.dataStandardId;

        if (vm.dataStandardId) vm.dataStandard = model;
        else vm.mappingProject = model;

        vm.entity = entity;
        vm.id = $stateParams.id;

        vm.setHref = (systemItem) => {
            var state = (systemItem.ItemTypeId < 4) ? 'app.entity.detail' : 'app.element.detail';
            systemItem.Href = services.state.href(state, {
                id: systemItem.SystemItemId,
                elementId: systemItem.SystemItemId,
                mappingProjectId: vm.mappingProjectId,
                dataStandardId: vm.dataStandardId
            });
        }

        angular.forEach(vm.entity.ChildSystemItems, child => { vm.setHref(child); });
        
        vm.instanceCallback = (instance) => {

            function setList() {
                var elements = [];
                var order = instance.DataTable.rows()[0];
                for (var i = 0; i < order.length; i++) {
                    if (vm.entity.ChildSystemItems[order[i]].ItemTypeId > 3)
                        elements.push({ ElementId: vm.entity.ChildSystemItems[order[i]].SystemItemId });
                }

                if (vm.mappingProjectId) services.session.cloneToSession('elementQueues', vm.mappingProjectId, elements);
                else services.session.cloneToSession('elementLists', vm.dataStandardId, elements);
            }

            setList();

            instance.DataTable.on('draw.dt', () => {
                setList();
            });

            instance.DataTable.on('init.dt', () => {
                var entityPage = services.session.cloneFromSession('entityPage', this.id);
                if (entityPage) instance.Datatable.page(entityPage).draw(false);
            });

            instance.DataTable.on('pgae.dt', () => {
                var info = instance.Datatable.page.info();
                services.session.cloneToSession('entityPage', this.id, info.page);
            });
        }

        vm.add = (itemTypeId) => {
            var instance = modals.systemItemForm(vm.dataStandardId, {
                ParentSystemItemId: entity.SystemItemId
            }, itemTypeId);
            instance.result.then((data) => {
                vm.entity.ChildSystemItems.push(data);
            });
        }

        vm.edit = element => {
            var instance = modals.systemItemForm(vm.dataStandardId, element, element.ItemTypeId);
            instance.result.then((data) => {
                element.ItemName = data.ItemName;
                element.Definition = data.Definition;
                element.IsExtended = data.IsExtended;
                element.DataTypeSource = data.DataTypeSource;
                element.FieldLength = data.FieldLength;
            });
        }

        vm.delete = (element, index) => {
            repositories.element.remove(element).then(() => {
                services.logger.success('Removed element.');
                vm.entity.ChildSystemItems.splice(index, 1);
            }, error => {
                services.logger.error('Error removing element.', error.data);
            });
        }

        vm.versionModal = (delta, isPrevious) => {
            if (!delta) delta = {};
            else {
                delta.Segments = isPrevious ? delta.OldSystemItemPathSegments : delta.NewSystemItemPathSegments;
                delta.ItemChangeTypeId = delta.ItemChangeType.Id;
            }

            var standard;

            if (isPrevious) {
                delta.NewSystemItemId = vm.id;
                standard = vm.dataStandard.PreviousDataStandard;
            } else {
                delta.OldSystemItemId = vm.id;
                standard = vm.dataStandard.NextDataStandard;
            }

            var instance = modals.deltaForm(isPrevious, delta, standard);

            //On Return
            instance.result.then((data) => {
                if (isPrevious) {
                    if (delta.PreviousVersionId) {
                        delta.OldSystemItemPathSegments = data.OldSystemItemPathSegments;
                        delta.OldSystemItemId = data.OldSystemItemId;
                        delta.Description = data.Description;
                        delta.ItemChangeType = data.ItemChangeType;
                    } else vm.entity.PreviousVersions.push(data);
                } else {
                    if (delta.NextVersionId) {
                        delta.NewSystemItemPathSegments = data.NewSystemItemPathSegments;
                        delta.NewSystemItemId = data.NewSystemItemId;
                        delta.Description = data.Description;
                        delta.ItemChangeType = data.ItemChangeType;
                    } else vm.entity.NextVersions.push(data);
                }
            });
        }

        vm.deleteNextVersion = (version, index) => {
            repositories.element.nextVersionDelta.remove(vm.id, version.NextVersionId).then(() => {
                services.logger.success('Removed next version delta.');
                vm.entity.NextVersions.splice(index, 1);
            }, error => {
                services.logger.error('Error removing next version delta.', error.data);
            });
        }

        vm.deletePreviousVersion = (version, index) => {
            repositories.element.previousVersionDelta.remove(vm.id, version.PreviousVersionId).then(() => {
                services.logger.success('Removed previous version delta.');
                vm.entity.PreviousVersions.splice(index, 1);
            }, error => {
                services.logger.error('Error removing previous version delta.', error.data);
            });
        }
    }
]);