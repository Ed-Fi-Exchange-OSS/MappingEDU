// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appMappingProject').controller('mappingProjectDetailController', [
    '_', '$scope', '$state', '$stateParams', 'mappingProjectService', 'breadcrumbService', 'handleErrorService',
    function(_, $scope, $state, $stateParams, mappingProjectService, breadcrumbService, handleErrorService) {

        var mappingProjectDetailViewModel = this;
        mappingProjectDetailViewModel.id = $stateParams.id;
        mappingProjectDetailViewModel.pageTitle = 'MAPPING PROJECT DETAIL';

        mappingProjectDetailViewModel.getMappingProjectDetails = function() {
            mappingProjectDetailViewModel.loading = true;
            mappingProjectService.get(mappingProjectDetailViewModel.id)
                .then(function(data) {
                    mappingProjectDetailViewModel.mappingProject = data;
                    breadcrumbService.withMappingProject(mappingProjectDetailViewModel.mappingProject);
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, mappingProjectDetailViewModel);
                })
                .finally(function() {
                    mappingProjectDetailViewModel.loading = false;
                });
        }

        mappingProjectDetailViewModel.load = function() {
            mappingProjectDetailViewModel.getMappingProjectDetails();
        }

        mappingProjectDetailViewModel.load();

        $scope.$on('workflow-status-updated', function() {
            mappingProjectDetailViewModel.load();
        });

        mappingProjectDetailViewModel.onPage = function(sref) {
            return $state.current.name === sref;
        }

        mappingProjectDetailViewModel.tabs = [
            { link: 'mappingProject.info', label: 'Info' },
            { link: 'mappingProject.dashboard', label: 'Dashboard' },
            { link: 'mappingProject.mappingSummary', label: 'Mapping Summary' },
            { link: 'mappingProject.reports', label: 'Reports' },
            { link: 'mappingProject.actions', label: 'Actions' }
        ];

        mappingProjectDetailViewModel.showTabs = function() {
            return _.some(mappingProjectDetailViewModel.tabs, function(tab) {
                return $state.is(tab.link);
            });
        };

        angular.element(document.querySelector("#editProjectModal")).on('hidden.bs.modal', function() {
            mappingProjectDetailViewModel.load();
        });

    }
]);