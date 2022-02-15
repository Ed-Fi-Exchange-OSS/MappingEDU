// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appMappingProject').controller('mappingProjectReportsController', [
    '$stateParams', 'mappingProjectReportsService', 'breadcrumbService', 'handleErrorService',
    function($stateParams, mappingProjectReportsService, breadcrumbService, handleErrorService) {
        var mappingProjectReportsViewModel = this;

        mappingProjectReportsViewModel.id = $stateParams.id;

        mappingProjectReportsViewModel.getMappingProjectReports = function() {
            mappingProjectReportsService.get(mappingProjectReportsViewModel.id)
                .then(function(data) {
                    mappingProjectReportsViewModel.reports = data;
                    mappingProjectReportsViewModel.showUnifiedDataDictionaryDownloadWarning =
                        mappingProjectReportsViewModel.reports.PercentComplete < 1;
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, mappingProjectReportsViewModel);
                });
        };

        mappingProjectReportsViewModel.getMappingProjectReports();

        breadcrumbService.withCurrent();

        $('[data-toggle="popover"]').popover();
    }
]);
