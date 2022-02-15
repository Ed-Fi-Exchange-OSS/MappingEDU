// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appEntityDetail').controller('entityDetailActionsController', [
    '$timeout', '$state', '$stateParams', 'entityService', 'breadcrumbService',
    function($timeout, $state, $stateParams, entityService, breadcrumbService) {
        var entityDetailActionsViewModel = this;

        entityDetailActionsViewModel.id = $stateParams.id;
        entityDetailActionsViewModel.dataStandardId = $stateParams.dataStandardId;
        entityDetailActionsViewModel.showDelete = true;
        entityDetailActionsViewModel.toggleDeleteCaret = function() {
            entityDetailActionsViewModel.showDelete = !entityDetailActionsViewModel.showDelete;
        }

        entityDetailActionsViewModel.delete = function() {
            entityService.delete(entityDetailActionsViewModel.id)
                .then(function() {
                    entityDetailActionsViewModel.deleted = true;
                    $timeout(function() { $state.go('dataStandard.elementList', { id: entityDetailActionsViewModel.dataStandardId }); }, 1000);
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, entityDetailActionsViewModel);
                });
        }

        breadcrumbService.withCurrent();
    }
]);
