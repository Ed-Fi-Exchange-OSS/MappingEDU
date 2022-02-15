// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module("appHome").controller('homeController', [
    'filterFilter', 'breadcrumbService', 'dataStandardService', 'mappingProjectService', 'handleErrorService',
    function(filterFilter, breadcrumbService, dataStandardService, mappingProjectService, handleErrorService) {
        var homeViewModel = this;

        homeViewModel.listVisibleLength = 5;
        homeViewModel.allDataStandardsVisible = false;
        homeViewModel.allMappingProjectsVisible = false;
        homeViewModel.allActiveMappingProjectsVisible = false;

        homeViewModel.getDataStandards = function() {
            homeViewModel.loading = true;
            dataStandardService.getAll()
                .then(function(data) {
                    for (var i = 0; i < data.length; i++) {
                        var dataStandard = data[i];
                        dataStandard.sref = 'dataStandard({ id: \'' + dataStandard.DataStandardId + '\'})';
                    }
                    homeViewModel.dataStandards = data;
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, homeViewModel);
                })
                .finally(function() {
                    homeViewModel.loading = false;
                });
        }

        homeViewModel.getMappingProjects = function() {
            homeViewModel.loading = true;
            mappingProjectService.getAll()
                .then(function(data) {
                    for (var i = 0; i < data.length; i++) {
                        var mappingProject = data[i];
                        mappingProject.sref = 'mappingProject({ id: \'' + mappingProject.MappingProjectId + '\'})';
                        mappingProject.resumeSref = 'mappingProject.dashboard({ id: \'' + mappingProject.MappingProjectId + '\'})';
                    }
                    homeViewModel.mappingProjects = data;
                    homeViewModel.activeProjects = filterFilter(data, { ProjectStatusTypeName: 'Active' });
                    homeViewModel.loading = false;
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, homeViewModel);
                })
                .finally(function() {
                    homeViewModel.loading = false;
                });
        }

        homeViewModel.getDataStandards();
        homeViewModel.getMappingProjects();

        breadcrumbService.clear();
    }
]);