// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appMappingProject').controller('mappingProjectActionsController', [
    '_', '$scope', '$timeout', '$state', '$stateParams', 'mappingProjectService', 'approveAllSystemItemMapsService', 'breadcrumbService', 'handleErrorService',
    function(_, $scope, $timeout, $state, $stateParams, mappingProjectService, approveAllSystemItemMapsService, breadcrumbService, handleErrorService) {
        var mappingProjectActionsViewModel = this;

        mappingProjectActionsViewModel.showApproveMsg = false;

        mappingProjectActionsViewModel.deleteMappingProject = function(mappingProject) {
            mappingProjectService.delete(mappingProject.MappingProjectId)
                .then(function() {
                    $scope.mappingProjectDetailViewModel.load();
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, mappingProjectActionsViewModel);
                });
        };

        mappingProjectActionsViewModel.approveReadyForReview = function(mappingProject) {
            approveAllSystemItemMapsService.update(mappingProject.MappingProjectId, mappingProject)
                .then(function(data) {
                    mappingProjectActionsViewModel.approveAllMsg = data.CountUpdated > 0 ? data.CountUpdated + ' mappings were approved successfully.' : 'There were no mappings ready for approval';
                    mappingProjectActionsViewModel.showApproveMsg = true;
                    $scope.$emit('workflow-status-updated');
                    $timeout(function() { mappingProjectActionsViewModel.showApproveMsg = false; }, 5000);
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, mappingProjectActionsViewModel);
                });
        }

        mappingProjectActionsViewModel.closeProject = function(mappingProject) {
            mappingProject.ProjectStatusTypeId = _.find(Application.Enumerations.ProjectStatusType, function(status) {
                return status.DisplayText === 'Closed';
            }).Id;

            mappingProjectService.update(mappingProject.MappingProjectId, mappingProject)
                .then(function() {
                    $scope.mappingProjectDetailViewModel.load();
                    mappingProjectActionsViewModel.closedMsg = mappingProject.ProjectName + ' has been closed.';
                    mappingProjectActionsViewModel.showClosedMsg = true;
                    $timeout(function() { mappingProjectActionsViewModel.showClosedMsg = false; }, 5000);
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, mappingProjectActionsViewModel);
                });
        };

        mappingProjectActionsViewModel.reopenProject = function(mappingProject) {
            mappingProject.ProjectStatusTypeId = _.find(Application.Enumerations.ProjectStatusType, function(status) {
                return status.DisplayText === 'Active';
            }).Id;

            mappingProjectService.update(mappingProject.MappingProjectId, mappingProject)
                .then(function() {
                    $scope.mappingProjectDetailViewModel.load();
                    mappingProjectActionsViewModel.openedMsg = mappingProject.ProjectName + ' has been re-opened.';
                    mappingProjectActionsViewModel.showOpenedMsg = true;
                    $timeout(function() { mappingProjectActionsViewModel.showOpenedMsg = false; }, 5000);
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, mappingProjectActionsViewModel);
                });
        };

        mappingProjectActionsViewModel.getMappingProject = function() {
            mappingProjectService.get($stateParams.id)
                .then(function(result) {
                    mappingProjectActionsViewModel.currentMappingProject = result;
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, mappingProjectActionsViewModel);
                });
        };

        mappingProjectActionsViewModel.cloneCallback = function(result) {
            if (result.statusText == "OK")
                mappingProjectActionsViewModel.cloneProjectMessage = "Mapping project cloned successfully.";
            else
                mappingProjectActionsViewModel.cloneProjectMessage = "There was an error when cloning the mapping project: " + result.statusText;
            mappingProjectActionsViewModel.showCloneMessage = true;
            $timeout(function() { mappingProjectActionsViewModel.showCloneMessage = false; }, 5000);
        };

        mappingProjectActionsViewModel.getMappingProject();

        breadcrumbService.withCurrent();
    }
]);
