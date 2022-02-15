// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appDataStandard').controller('dataStandardSourceController', [
    '_', '$state', '$stateParams', 'dataStandardSourceService', 'breadcrumbService', 'handleErrorService',
    function(_, $state, $stateParams, dataStandardSourceService, breadcrumbService, handleErrorService) {

        var dataStandardSourceViewModel = this;
        dataStandardSourceViewModel.id = $stateParams.id;

        dataStandardSourceViewModel.getDataStandardSourceMappingProjects = function() {
            dataStandardSourceService.get(dataStandardSourceViewModel.id)
                .then(function(data) {
                    var projectInfoState = 'mappingProject.info';
                    var projectSummaryState = 'mappingProject.mappingSummary';
                    var projectReviewQueueState = 'mappingProject.reviewQueue';
                    dataStandardSourceViewModel.dataStandardSourceDisplay = data;
                    _.each(dataStandardSourceViewModel.dataStandardSourceDisplay, function(project) {
                        var projectId = '({ id: \'' + project.MappingProjectId + '\' })';
                        project.projectSref = projectInfoState + projectId;
                        project.listSref = projectReviewQueueState + projectId;
                        project.mapSummarySref = projectSummaryState + projectId;
                    });
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, dataStandardSourceViewModel);
                });
        };

        dataStandardSourceViewModel.getDataStandardSourceMappingProjects();

        breadcrumbService.withCurrent();
    }
]);