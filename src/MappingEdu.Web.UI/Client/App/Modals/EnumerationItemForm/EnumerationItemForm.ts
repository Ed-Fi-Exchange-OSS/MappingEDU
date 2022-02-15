// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modals.enumeration-item-form
//

var m = angular.module('app.modals.enumeration-item-form', []);

// ****************************************************************************
// Service app.modals.enumeration-item-form
//

m.factory('app.modals.enumeration-item-form', ['settings', 'services', (settings, services) => {
    return (enumerationItem, systemItemId) => {
        
        enumerationItem.SystemItemId = systemItemId;

        var modal: ng.ui.bootstrap.IModalSettings = {
            backdrop: 'static',
            keyboard: false,
            controller: 'app.modals.enumeration-item-form',
            templateUrl: `${settings.modalBaseUri}/EnumerationItemForm/EnumerationItemForm.tpl.html`,
            resolve: {
                enumerationItem: () => { return enumerationItem }
            }
        }
        return services.modal.open(modal);
    }
}]);

// ****************************************************************************
// Controller app.modals.enumeration-itemitem-form
//

m.controller('app.modals.enumeration-item-form', ['$scope', '$uibModalInstance', 'enumerations', 'repositories', 'services', 'enumerationItem',
    ($scope, $uibModalInstance: angular.ui.bootstrap.IModalServiceInstance, enumerations: IEnumerations, repositories: IRepositories, services: IServices, enumerationItem) => {

        $scope.enumerationItem = angular.copy(enumerationItem);

        $scope.save = (enumerationItem) => {
            if (enumerationItem.SystemEnumerationItemId) {
                return repositories.element.enumerationItem.save(enumerationItem.SystemItemId, enumerationItem.SystemEnumerationItemId, enumerationItem)
                    .then(data => {
                        services.logger.success('Saved enumeration item.');
                        $uibModalInstance.close(data);
                    }, error => {
                        services.logger.error('Error saving enumeration item.', error.data);
                    });
            } else {
                repositories.element.enumerationItem.create(enumerationItem.SystemItemId, enumerationItem).then(data => {
                    services.logger.success('Created enumeration item.');
                    $uibModalInstance.close(data);
                }, error => {
                    services.logger.error('Error creating enumeration item.', error.data);
                });
            }   
        }

        $scope.cancel = () => {
            $uibModalInstance.dismiss();
        }
    }
]);
