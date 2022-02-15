// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modals.bulk-mapping
//

var m = angular.module('app.modals.bulk-mapping', []);

// ****************************************************************************
// Service app.modals.bulk-mapping
//

m.factory('app.modals.bulk-mapping', ['settings', 'services', (settings, services) => {
    return (project, systemItem, itemTypeId) => {

        console.log('here');

        var modal: ng.ui.bootstrap.IModalSettings = {
            backdrop: 'static',
            keyboard: false,
            size: 'lg',
            controller: 'app.modals.bulk-mapping',
            controllerAs: 'ctrl',
            templateUrl: `${settings.modalBaseUri}/BulkMapping/BulkMappingView.tpl.html`,
            resolve: {
                project: () => { return project },
                systemItem: () => { return systemItem },
                itemTypeId: () => { return itemTypeId }
            }
        }
        return services.modal.open(modal);
    }
}]);

// ****************************************************************************
// Controller app.modals.bulk-mapping
//

m.controller('app.modals.bulk-mapping', ['$scope', '$uibModalInstance', 'repositories', 'services', 'enumerations', 'project', 'systemItem', 'itemTypeId',
    function($scope, $uibModalInstance: angular.ui.bootstrap.IModalServiceInstance, repositories: IRepositories, services: IServices, enumerations: IEnumerations, project, systemItem, itemTypeId) {

        services.logger.debug('Loaded controller app.modals.bulk-mapping');

        var ctrl = this;

        ctrl.state = 'select';
        ctrl.systemItem = systemItem;
        ctrl.mappedEffected = 0;
        ctrl.unmappedEffected = 0;
        ctrl.project = project;
        ctrl.itemTypeId = itemTypeId;

        console.log(ctrl.systemItem);

        ctrl.workflowStatuses = services.underscore.select(enumerations.WorkflowStatusType, type => type.Id > 0 && type.Id < 5);
        ctrl.mappingMethods = services.underscore.select(enumerations.MappingMethodTypeInQueue, type => type.Id !== 2 && type.Id > 0);
        ctrl.mappingMethodsUnmapped = services.underscore.select(enumerations.MappingMethodTypeInQueue, type => type.Id > 2);

        ctrl.mapped = {
            MappingProjectId: project.MappingProjectId,
            SystemItemIds: [systemItem.SystemItemId],
            Statuses: [],
            Methods: [],
            ChangeStatus: {},
            ItemType: { Id: 0, DisplayText: 'Element/Enumeration' }
        };

        ctrl.unmapped = {
            MappingProjectId: project.MappingProjectId,
            SystemItemIds: [systemItem.SystemItemId],
            ItemType: { Id: 0, DisplayText: 'Element/Enumeration' }
        };

        ctrl.itemTypes = [
            { Id: 4, DisplayText: 'Element' },
            { Id: 5, DisplayText: 'Enumeration' },
            { Id: 0, DisplayText: 'Element/Enumeration'}
        ];

        ctrl.getUpdateCount = () => {
            if (ctrl.mapped.Statuses.length === 0 || ctrl.mapped.Methods.length === 0)
                ctrl.mappedEffected = 0;
            else {
                ctrl.loading = true;
                repositories.mappingProject.systemItemMaps.getUpdateCount(project.MappingProjectId, ctrl.mapped).then(data => {
                    ctrl.loading = false;
                    ctrl.mappedEffected = data.CountUpdated;
                    services.timeout(() => { $scope.$apply() });
                });   
            }
        }

        ctrl.getAddCount = () => {
            ctrl.loading = true;
            repositories.mappingProject.systemItemMaps.getAddCount(project.MappingProjectId, ctrl.unmapped).then(data => {
                ctrl.loading = false;
                ctrl.unmappedEffected = data.CountUpdated;
                services.timeout(() => { $scope.$apply() });
            });
        }

        ctrl.getAddCount();

        ctrl.update = () => {
            if (ctrl.state === 'mapped') {
                return repositories.mappingProject.systemItemMaps.updateMappings(project.MappingProjectId, ctrl.mapped).then(data => {
                    $uibModalInstance.close(data);
                });
            } else {
                return repositories.mappingProject.systemItemMaps.addMappings(project.MappingProjectId, ctrl.unmapped).then(data => {
                    $uibModalInstance.close(data);
                });
            }
        }

        ctrl.next = () => {
            ctrl.state = angular.copy(ctrl.option);
        }

        ctrl.prev = () => {
            ctrl.state = 'select';
        }

        ctrl.close = () => {
            $uibModalInstance.dismiss();
        }
    }
]);
