// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appElementDetail').controller('elementDetailActionsController', [
    '_', '$scope', '$state', '$stateParams', '$timeout', 'elementService', 'enumerationService', 'sessionService', 'breadcrumbService', 'handleErrorService',
    function(_, $scope, $state, $stateParams, $timeout, elementService, enumerationService, sessionService, breadcrumbService, handleErrorService) {
        var elementDetailActionsViewModel = this;

        elementDetailActionsViewModel.emptyGuid = '{00000000-0000-0000-0000-000000000000}';

        elementDetailActionsViewModel.mappingProjectId = $stateParams.mappingProjectId || elementDetailActionsViewModel.emptyGuid;
        elementDetailActionsViewModel.dataStandardId = $stateParams.dataStandardId || elementDetailActionsViewModel.emptyGuid;
        elementDetailActionsViewModel.id = elementDetailActionsViewModel.mappingProjectId != elementDetailActionsViewModel.emptyGuid
            ? elementDetailActionsViewModel.mappingProjectId
            : elementDetailActionsViewModel.dataStandardId;
        elementDetailActionsViewModel.current = $stateParams.current;
        elementDetailActionsViewModel.dataStandardId = $stateParams.dataStandardId;
        elementDetailActionsViewModel.showDelete = true;

        elementDetailActionsViewModel.element = $scope.$parent.elementDetailViewModel.element;

        elementDetailActionsViewModel.loadEnumeration = function(enumeration) {
            elementDetailActionsViewModel.enumeration = enumeration;
        };

        elementDetailActionsViewModel.loadEnumeration($scope.$parent.elementDetailViewModel.enumeration);


        $scope.$on('element-fetched', function(event, data) {
            handleErrorService.clearErrors(elementDetailActionsViewModel);
            elementDetailActionsViewModel.element = data;
        });

        $scope.$on('enumeration-fetched', function(event, data) {
            elementDetailActionsViewModel.loadEnumeration(data);
        });

        elementDetailActionsViewModel.toggleDeleteCaret = function() {
            elementDetailActionsViewModel.showDelete = !elementDetailActionsViewModel.showDelete;
        }

        elementDetailActionsViewModel.delete = function() {
            if (elementDetailActionsViewModel.enumeration) {
                enumerationService.delete(elementDetailActionsViewModel.enumeration.SystemItemId)
                    .then(function() {
                        elementDetailActionsViewModel.deleted = true;
                        $timeout(function() { $state.go('dataStandard.elementList', { id: elementDetailActionsViewModel.dataStandardId }); }, 1000);
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, elementDetailActionsViewModel);
                    });
            } else {
                elementService.delete(elementDetailActionsViewModel.element.SystemItemId)
                    .then(function() {
                        elementDetailActionsViewModel.deleted = true;
                        $timeout(function() { $state.go('dataStandard.elementList', { id: elementDetailActionsViewModel.dataStandardId }); }, 1000);
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, elementDetailActionsViewModel);
                    });
            }
        }

        breadcrumbService.withCurrent();
    }
]);