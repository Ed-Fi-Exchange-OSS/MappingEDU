// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modals.system-item-form
//

var m = angular.module('app.modals.system-item-form', []);

// ****************************************************************************
// Service app.modals.system-item-form
//

m.factory('app.modals.system-item-form', ['settings', 'services', (settings, services) => {
    return (dataStandardId, systemItem, itemTypeId) => {

        systemItem.DataStandardId = dataStandardId;
        systemItem.ItemTypeId = itemTypeId;

        var modal: ng.ui.bootstrap.IModalSettings = {
            backdrop: 'static',
            keyboard: false,
            controller: 'app.modals.system-item-form',
            resolve: {
                systemItem: () => { return systemItem }
            }
        }

        if (itemTypeId === 1) {modal.templateUrl = `${settings.modalBaseUri}/SystemItemForm/ElementGroupForm.tpl.html`;}
        else if (itemTypeId === 2 || itemTypeId === 3) modal.templateUrl = `${settings.modalBaseUri}/SystemItemForm/EntityForm.tpl.html`;
        else if (itemTypeId === 4) modal.templateUrl = `${settings.modalBaseUri}/SystemItemForm/ElementForm.tpl.html`;
        else if (itemTypeId === 5) modal.templateUrl = `${settings.modalBaseUri}/SystemItemForm/EnumerationForm.tpl.html`;
        else {
            services.logger.error('Unknown Item Type');
            return null;
        }

        if (itemTypeId > 3) modal.size = 'lg';


        return services.modal.open(modal);
    }
}]);

// ****************************************************************************
// Controller app.modals.system-item-form
//

m.controller('app.modals.system-item-form', ['$scope', '$uibModalInstance', 'enumerations', 'repositories', 'services', 'systemItem',
    ($scope, $uibModalInstance: angular.ui.bootstrap.IModalServiceInstance, enumerations: IEnumerations, repositories: IRepositories, services: IServices, systemItem) => {

        $scope.systemItem = angular.copy(systemItem);

        $scope.loadEnumerations = () => {
            if (!$scope.enumerations) {
                repositories.element.systemItemEnumeration.find(systemItem.DataStandardId).then(data => {
                    $scope.enumerations = data;
                }, error => {
                    services.logger.error('Error loading system item enumerations.', error.data);
                });
            }
        }

        var type = '';
        if (systemItem.ItemTypeId === 1) type = 'element Group';
        else if (systemItem.ItemTypeId < 4) type = 'entity';
        else if (systemItem.ItemTypeId === 5) type = 'enumeration';
        else {
            type = 'element';
            $scope.types = enumerations.ItemDataType;
            if (!$scope.systemItem.ItemDataTypeId) $scope.systemItem.ItemDataTypeId = null;
            if ($scope.systemItem.ItemDataTypeId === 17) $scope.loadEnumerations();
        }

        $scope.save = () => {
            if ($scope.systemItem.SystemItemId)
                return repositories.systemItem.save($scope.systemItem).then((data) => {
                    services.logger.success(`Successfully saved ${type}.`);
                    $uibModalInstance.close(data);
                }, error => {
                    services.logger.error(`Error saving ${type}.`, error);
                });
            else {
                return repositories.systemItem.create($scope.systemItem).then((data) => {
                    services.logger.success(`Successfully created ${type}.`);
                    $uibModalInstance.close(data);
                }, error => {
                    services.logger.error(`Error creating ${type}.`, error);
                }); 
            }
        };

        $scope.cancel = () => {
            $uibModalInstance.dismiss();
        }
    }
]);
