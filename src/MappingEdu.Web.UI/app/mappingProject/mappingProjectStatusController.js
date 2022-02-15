// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appMappingProject').controller('mappingProjectStatusController', [
    '_', '$timeout', '$scope', 'mappingProjectService', 'mappingProjectStatusService', 'handleErrorService',
    function(_, $timeout, $scope, mappingProjectService, mappingProjectStatusService, handleErrorService) {
        var mappingProjectStatusViewModel = this;

        $scope.$watch('mappingProject', function() {
            mappingProjectStatusViewModel.getMappingProjectStatus();
        });

        mappingProjectStatusViewModel.getMappingProjectStatus = function() {
            if (!$scope.mappingProject) {
                mappingProjectStatusViewModel.isReadyForClose = false;
                return;
            }

            var mappingProject = $scope.mappingProject;
            mappingProjectStatusService.get(mappingProject.MappingProjectId)
                .then(function(data) {
                    mappingProjectStatusViewModel.isReadyForClose = data.Approved && (mappingProject.ProjectStatusTypeName == 'Active');
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, mappingProjectStatusViewModel);
                });
        }

        mappingProjectStatusViewModel.getMappingProjectStatus();

        mappingProjectStatusViewModel.closeProject = function() {
            var mappingProject = $scope.mappingProject;
            mappingProject.ProjectStatusTypeId = _.find(Application.Enumerations.ProjectStatusType, function(status) {
                return status.DisplayText === 'Closed';
            }).Id;

            mappingProjectService.update(mappingProject.MappingProjectId, mappingProject)
                .then(function(data) {
                    $scope.mappingProject = data;
                    mappingProjectStatusViewModel.isReadyForClose = false;
                    mappingProjectStatusViewModel.closedMsg = mappingProject.ProjectName + ' has been closed.';
                    mappingProjectStatusViewModel.showClosedMsg = true;
                    $timeout(function() { mappingProjectStatusViewModel.showClosedMsg = false; }, 5000);
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, mappingProjectStatusViewModel);
                });
        };
    }
]);
