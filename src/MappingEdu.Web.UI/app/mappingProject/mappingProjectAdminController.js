// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appMappingProject').controller('mappingProjectAdminController', [
    '_', '$timeout', '$scope', '$state', '$stateParams', 'mappingProjectService', 'dataStandardService', 'handleErrorService',
    function(_, $timeout, $scope, $state, $stateParams, mappingProjectService, dataStandardService, handleErrorService) {

        var mappingProjectAdminViewModel = this;

        mappingProjectAdminViewModel.id = $stateParams.id;

        mappingProjectAdminViewModel.isNew = (undefined === mappingProjectAdminViewModel.id);
        mappingProjectAdminViewModel.canChangeDataStandards = mappingProjectAdminViewModel.isNew;

        mappingProjectAdminViewModel.getCurrentMappingProjects = function() {
            mappingProjectService.getAll()
                .then(function(result) {
                    mappingProjectAdminViewModel.currentMappingProjects = result;
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, mappingProjectAdminViewModel);
                });
        };

        mappingProjectAdminViewModel.getCurrentMappingProjects();

        mappingProjectAdminViewModel.getDataStandards = function() {
            dataStandardService.getAll()
                .then(function(result) {
                    mappingProjectAdminViewModel.srcDataStandards = result;
                    if (mappingProjectAdminViewModel.srcDataStandards.length > 0) {
                        mappingProjectAdminViewModel.targetDataStandards = mappingProjectAdminViewModel.srcDataStandards.slice(0);
                    }
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, mappingProjectAdminViewModel);
                });
        };

        mappingProjectAdminViewModel.getDataStandards();

        mappingProjectAdminViewModel.save = function(mappingProject) {
            $scope.$broadcast('show-errors-check-valid');
            if (_.isUndefined(mappingProject) || $scope.mappingProjectForm.$invalid)
                return;

            if (mappingProject.SourceDataStandardId
                === mappingProject.TargetDataStandardId) {
                handleErrorService.addError('Source Data Standard cannot be the same as Target Data Standard.', mappingProjectAdminViewModel);
            }

            if (!validateUniqueProjectName(mappingProject)) {
                handleErrorService.addError('Project Name duplicates existing project. Please re-name this Mapping Project.', mappingProjectAdminViewModel);
            }

            if (!_.isUndefined(mappingProjectAdminViewModel.errorData) && mappingProjectAdminViewModel.errorData.hasError) {
                return;
            }

            if (!_.isUndefined(mappingProject.MappingProjectId)) {
                mappingProjectService.update(mappingProject.MappingProjectId, mappingProject).then(
                        function() {
                            mappingProjectAdminViewModel.success = true;
                            angular.element(document.querySelector('#editProjectModal')).modal('hide');
                        })
                    .catch(
                        function(error) {
                            handleErrorService.handleErrors(error, mappingProjectAdminViewModel);
                        });
            } else {
                mappingProject.ProjectStatusTypeId = _.find(Application.Enumerations.ProjectStatusType, function(status) {
                    return status.DisplayText === 'Active';
                }).Id;
                mappingProjectService.add(mappingProject).then(
                        function(data) {
                            mappingProjectAdminViewModel.success = true;
                            goToMappingDashboardPage(data.MappingProjectId);
                        })
                    .catch(
                        function(error) {
                            handleErrorService.handleErrors(error, mappingProjectAdminViewModel);
                        });
            }
        };

        mappingProjectAdminViewModel.cloneMappingProject = function(mappingProject) {
            $scope.$broadcast('show-errors-check-valid');
            if (_.isUndefined(mappingProject) || $scope.mappingProjectForm.$invalid)
                return;
            if (!validateUniqueProjectName({ ProjectName: mappingProject.newCloneName })) {
                handleErrorService.addError('Project Name duplicates existing project. Please re-name this Mapping Project.', mappingProjectAdminViewModel);
            }

            var postData = {
                MappingProjectId: mappingProject.MappingProjectId,
                CloneProjectName: mappingProject.newCloneName
            }

            mappingProjectAdminViewModel.loading = true;
            mappingProjectService.cloneMappingProject(postData)
                .then(function(result) {
                    mappingProjectAdminViewModel.loading = false;
                    angular.element(document.querySelector('#cloneProjectModal')).modal('hide');
                    $scope.cloneCallback(result);
                })
                .catch(function(error) {
                    mappingProjectAdminViewModel.loading = false;
                    handleErrorService.handleErrors(error, mappingProjectAdminViewModel);
                });
        };

        mappingProjectAdminViewModel.cancelClone = function() {
            $scope.$broadcast('show-errors-reset');
            angular.element(document.querySelector("#cloneProjectModal")).modal('hide');
        };


        function goToMappingDashboardPage(mappingProjectId) {
            $state.go('mappingProject.dashboard', { id: mappingProjectId });
        };

        mappingProjectAdminViewModel.goToCreateNewDataStandard = function() {
            $state.go('createDataStandard');
        }

        mappingProjectAdminViewModel.cancel = function(mappingProject) {
            $scope.$broadcast('show-errors-reset');
            if (!_.isUndefined(mappingProject) && !_.isUndefined(mappingProject.MappingProjectId)) {
                angular.element(document.querySelector("#editProjectModal")).modal('hide');
            } else
                goToHomePage();
        };

        function goToHomePage() {
            $state.go('home');
        }

        function validateUniqueProjectName(changedProject) {
            if (changedProject.ProjectName === undefined
                || changedProject.ProjectName === null
                || changedProject.ProjectName.length < 1) {
                return true;
            }

            if (mappingProjectAdminViewModel.currentMappingProjects === undefined
                || mappingProjectAdminViewModel.currentMappingProjects === null
                || mappingProjectAdminViewModel.currentMappingProjects.length < 1) {
                return true;
            }

            var isValid = true;
            _.each(mappingProjectAdminViewModel.currentMappingProjects, function(mProj) {
                if (mProj.ProjectName.toLowerCase() === changedProject.ProjectName.toLowerCase()
                    && mProj.MappingProjectId != changedProject.MappingProjectId) {
                    isValid = false;
                }
            });

            return isValid;
        }
    }
]);
