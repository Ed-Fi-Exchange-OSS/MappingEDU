// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modals.custom-detail-form
//

var m = angular.module('app.modals.custom-detail-form', []);

// ****************************************************************************
// Service app.modals.custom-detail-form
//

m.factory('app.modals.custom-detail-form', ['settings', 'services', (settings, services) => {
    return (dataStandardId, customDetail) => {

        var modal: ng.ui.bootstrap.IModalSettings = {
            backdrop: 'static',
            keyboard: false,
            controller: 'app.modals.custom-detail-form',
            templateUrl: `${settings.modalBaseUri}/CustomDetailForm/CustomDetailForm.tpl.html`,
            resolve: {
                dataStandardId: () => { return dataStandardId },
                customDetail: () => { return customDetail } 
            }
        }


        return services.modal.open(modal);
    }
}]);

// ****************************************************************************
// Controller app.modals.custom-detail-form
//

m.controller('app.modals.custom-detail-form', ['$scope', '$uibModalInstance', 'repositories', 'services', 'dataStandardId', 'customDetail',
    ($scope, $uibModalInstance: angular.ui.bootstrap.IModalServiceInstance, repositories: IRepositories, services: IServices, dataStandardId, customDetail) => {

        $scope.customDetail = angular.copy(customDetail);
        
        $scope.save = () => {
            if ($scope.customDetail.CustomDetailMetadataId) {
                repositories.customDetailMetadata.save(dataStandardId, $scope.customDetail).then(data => {
                    services.logger.success('Saved custom detail.');
                    $uibModalInstance.close(data);
                }, error => {
                    services.logger.error('Error saving custom detail.', error.data);
                });
            } else {
                repositories.customDetailMetadata.create(dataStandardId, $scope.customDetail).then(data => {
                    services.logger.success('Created custom detail.');
                    $uibModalInstance.close(data);
                }, error => {
                    services.logger.error('Error creating custom detail.', error.data);
                });
            }
        };

        $scope.cancel = () => {
            $uibModalInstance.dismiss();
        }
    }
]);
