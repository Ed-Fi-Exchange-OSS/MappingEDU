// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appMappingProject').controller('mappingProjectDashboardController', [
    '_', '$stateParams', 'mappingProjectDashboardService', 'breadcrumbService', 'handleErrorService',
    function(_, $stateParams, mappingProjectDashboardService, breadcrumbService, handleErrorService) {
        var mappingProjectDashboardViewModel = this;

        mappingProjectDashboardViewModel.id = $stateParams.id;

        mappingProjectDashboardViewModel.getMappingProjectDashboard = function() {
            mappingProjectDashboardViewModel.loading = true;
            mappingProjectDashboardService.get(mappingProjectDashboardViewModel.id)
                .then(function(result) {
                    mappingProjectDashboardViewModel.dashboard = result;

                    _.map(mappingProjectDashboardViewModel.dashboard.WorkQueue, buildSrefs);

                    _.map(mappingProjectDashboardViewModel.dashboard.ElementGroups, buildSrefs);

                    _.map(mappingProjectDashboardViewModel.dashboard.Statuses, buildSrefs);

                    var maxLen = Math.max(mappingProjectDashboardViewModel.dashboard.ElementGroups.length,
                        mappingProjectDashboardViewModel.dashboard.Statuses.length) * 1.5;

                    mappingProjectDashboardViewModel.verticalRange = [];

                    for (var i = 0; i < maxLen; i++) {
                        mappingProjectDashboardViewModel.verticalRange.push(i);
                    }
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, mappingProjectDashboardViewModel);
                })
                .finally(function() {
                    mappingProjectDashboardViewModel.loading = false;
                });
        }

        function buildSrefs(item) {
            var elementDetailInfoState = 'elementDetail.info';
            var mappingProjectReviewQueue = 'mappingProject.reviewQueue';
            var parameters = {
                mappingProjectId: mappingProjectDashboardViewModel.id,
                filter: item.Filter
            };

            var params = '(' + angular.toJson(parameters) + ')';

            item.nameSref = elementDetailInfoState + params;
            item.listSref = mappingProjectReviewQueue + params;

            parameters.resume = true;
            item.resumeMapSref = elementDetailInfoState + '(' + angular.toJson(parameters) + ')';
            return item;
        }

        mappingProjectDashboardViewModel.getMappingProjectDashboard();

        breadcrumbService.withCurrent();
    }
]);
