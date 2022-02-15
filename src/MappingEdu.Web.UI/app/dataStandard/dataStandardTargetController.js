// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appDataStandard').controller('dataStandardTargetController', [
    '_', '$state', '$stateParams', 'dataStandardTargetService', 'breadcrumbService', 'handleErrorService',
    function(_, $state, $stateParams, dataStandardTargetService, breadcrumbService, handleErrorService) {

        var dataStandardTargetViewModel = this;
        dataStandardTargetViewModel.id = $stateParams.id;

        dataStandardTargetViewModel.goToUrl = function(stateName) {
            $state.go(stateName);
        };

        dataStandardTargetViewModel.getDataStandardTargetMappingProjects = function() {
            dataStandardTargetService.get(dataStandardTargetViewModel.id)
                .then(function(data) {
                    dataStandardTargetViewModel.dataStandardTargetDisplay = data;
                    var projectInfoState = 'mappingProject.info';
                    var projectSummaryState = 'mappingProject.mappingSummary';
                    var projectReviewQueueState = 'mappingProject.reviewQueue';
                    dataStandardTargetViewModel.dataStandardSourceDisplay = data;
                    _.each(dataStandardTargetViewModel.dataStandardTargetDisplay, function(project) {
                        var projectId = '({ id: \'' + project.MappingProjectId + '\' })';
                        project.projectSref = projectInfoState + projectId;
                        project.listSref = projectReviewQueueState + projectId;
                        project.mapSummarySref = projectSummaryState + projectId;
                    });
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, dataStandardTargetViewModel);
                });
        };

        dataStandardTargetViewModel.getDataStandardTargetMappingProjects();

        breadcrumbService.withCurrent();
    }
]);