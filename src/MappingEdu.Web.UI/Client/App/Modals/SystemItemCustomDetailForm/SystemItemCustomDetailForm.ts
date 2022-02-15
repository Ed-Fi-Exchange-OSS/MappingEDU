// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modals.system-item-custom-detail-form
//

var m = angular.module('app.modals.system-item-custom-detail-form', []);

// ****************************************************************************
// Service app.modals.system-item-custom-detail-form
//

m.factory('app.modals.system-item-custom-detail-form', ['settings', 'services', (settings, services) => {
    return (systemItemId, systemItemCustomDetails) => {

        var modal: ng.ui.bootstrap.IModalSettings = {
            backdrop: 'static',
            keyboard: false,
            controller: 'app.modals.system-item-custom-detail-form',
            templateUrl: `${settings.modalBaseUri}/SystemItemCustomDetailForm/SystemItemCustomDetailForm.tpl.html`,
            size: 'lg',
            resolve: {
                systemItemId: () => { return systemItemId },
                systemItemCustomDetails: () => { return systemItemCustomDetails } 
            }
        }


        return services.modal.open(modal);
    }
}]);

// ****************************************************************************
// Controller app.modals.system-item-custom-detail-form
//

m.controller('app.modals.system-item-custom-detail-form', ['$scope', '$uibModalInstance', 'enumerations', 'repositories', 'services', 'systemItemId', 'systemItemCustomDetails',
    ($scope, $uibModalInstance: angular.ui.bootstrap.IModalServiceInstance, enumerations: IEnumerations, repositories: IRepositories, services: IServices, systemItemId, systemItemCustomDetails) => {

        $scope.systemItemCustomDetails = angular.copy(systemItemCustomDetails);
        
        $scope.save = () => {
            services.underscore.each(<Array<any>>$scope.systemItemCustomDetails, item => {
                if (item.CustomDetailMetadata.IsBoolean)
                    item.Value = item.boolValue ? '1' : '0';
            });

            var saveContainer = {
                SystemItemId: systemItemId,
                SystemItemCustomDetails: $scope.systemItemCustomDetails
            }

            repositories.element.systemItemDetail.save(saveContainer)
                .then(data => {
                    services.logger.success('Saved system item detail container.');
                    $uibModalInstance.close(data);
                }, error => {
                    services.logger.error('Error saving system item detail container.', error.data);
                });
        };

        $scope.cancel = () => {
            $uibModalInstance.dismiss();
        }
    }
]);
