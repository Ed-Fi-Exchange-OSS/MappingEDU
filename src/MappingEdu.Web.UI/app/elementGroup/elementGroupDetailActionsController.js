// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appElementGroup').controller('elementGroupDetailActionsController', [
    '$timeout', '$state', '$stateParams', 'elementGroupService', 'breadcrumbService', 'handleErrorService',
    function($timeout, $state, $stateParams, elementGroupService, breadcrumbService, handleErrorService) {
        var elementGroupDetailActionsViewModel = this;

        elementGroupDetailActionsViewModel.id = $stateParams.id;
        elementGroupDetailActionsViewModel.dataStandardId = $stateParams.dataStandardId;
        elementGroupDetailActionsViewModel.showDelete = true;
        elementGroupDetailActionsViewModel.toggleDeleteCaret = function() {
            elementGroupDetailActionsViewModel.showDelete = !elementGroupDetailActionsViewModel.showDelete;
        }

        elementGroupDetailActionsViewModel.delete = function() {
            elementGroupService.delete(elementGroupDetailActionsViewModel.dataStandardId, elementGroupDetailActionsViewModel.id)
                .then(function() {
                    elementGroupDetailActionsViewModel.deleted = true;
                    $timeout(function() { $state.go('dataStandard.elementGroups', { id: elementGroupDetailActionsViewModel.dataStandardId }); }, 1000);
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, elementGroupDetailActionsViewModel);
                });
        }

        breadcrumbService.withCurrent();
    }
]);
