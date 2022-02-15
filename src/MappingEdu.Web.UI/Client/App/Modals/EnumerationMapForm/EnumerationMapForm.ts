// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module aapp.modules.elements.modals.enumeration-map-form
//

var m = angular.module('app.modals.enumeration-map-form', []);

// ****************************************************************************
// Service app.modals.delta-form
//

m.factory('app.modals.enumeration-map-form', ['settings', 'services', (settings, services) => {
    return (enumeration, targetEnumerationItems, systemItemMapId) => {

        if (!enumeration.Mapping)
            enumeration.Mapping = {
                SourceCodeValue: enumeration.CodeValue,
                SourceSystemEnumerationItemId: enumeration.SystemEnumerationItemId
            };

        enumeration.Mapping.SystemItemMapId = systemItemMapId;

        var modal: ng.ui.bootstrap.IModalSettings = {
            backdrop: 'static',
            keyboard: false,
            templateUrl: `${settings.modalBaseUri}/EnumerationMapForm/EnumerationMapForm.tpl.html`,
            controller: 'app.modals.enumeration-map-form',
            size: 'lg',
            resolve: {
                enumeration: () => { return enumeration; },
                targetEnumerationItems: () => { return targetEnumerationItems; }
            }
        }
        return services.modal.open(modal);
    }
}]);

// ****************************************************************************
// Controller app.modals.enumeration-map
//

m.controller('app.modals.enumeration-map-form', ['$scope', '$uibModalInstance', 'enumerations', 'repositories', 'services', 'enumeration', 'targetEnumerationItems',
    ($scope, $uibModalInstance: angular.ui.bootstrap.IModalServiceInstance, enumerations: IEnumerations, repositories: IRepositories, services: IServices, enumeration, targetEnumerationItems) => {

        services.logger.debug('Loaded controller app.modal.enumeration-map');

        $scope.enumeration = angular.copy(enumeration);
        $scope.targetEnumerationItems = targetEnumerationItems;
        $scope.enumerationMappingStatusReasonTypes = services.underscore.filter(<Array<any>>enumerations.EnumerationMappingStatusReasonType, item => item.Id);
        $scope.enumerationMappingStatusTypes = services.underscore.filter(<Array<any>>enumerations.EnumerationMappingStatusType, item => item.Id);

        $scope.getTargetPath = targetSystemItem => {
            if (!targetSystemItem)
                return '';

            if (targetSystemItem.ParentSystemItem)
                return $scope.getTargetPath(targetSystemItem.ParentSystemItem) + '.' + targetSystemItem.ItemName;

            return targetSystemItem.ItemName;
        };

        $scope.close = () => {
            $uibModalInstance.dismiss();
        }

        $scope.save = () => {
            if ($scope.enumeration.Mapping.SystemEnumerationItemMapId) {
                return repositories.element.enumerationItemMapping.save($scope.enumeration.Mapping.SystemItemMapId,
                    $scope.enumeration.Mapping.SystemEnumerationItemMapId,
                    $scope.enumeration.Mapping)
                    .then((data) => {
                        services.logger.success('Updated enumeration item mapping.');
                        $uibModalInstance.close(data);
                    }, error => {
                        services.logger.success('Error updating enumeration item mapping.');
                    });
            } else {
                repositories.element.enumerationItemMapping.create($scope.enumeration.Mapping.SystemItemMapId,
                    $scope.enumeration.Mapping)
                    .then((data) => {
                        services.logger.success('Created enumeration item mapping.');
                        $uibModalInstance.close(data);
                    }, error => {
                        services.logger.error('Error creating enumeration item mapping.', error.data);
                    });
            }
        }
    }
]);
