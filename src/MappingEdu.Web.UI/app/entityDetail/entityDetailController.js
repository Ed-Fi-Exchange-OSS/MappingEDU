// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appEntityDetail').controller('entityDetailController', [
    '$scope', '$state', '$stateParams', 'mappingProjectService', 'dataStandardService', 'entityService', 'elementDetailService', 'handleErrorService', 'breadcrumbService', 'sessionService',
    function($scope, $state, $stateParams, mappingProjectService, dataStandardService, entityService, elementDetailService, handleErrorService, breadcrumbService, sessionService) {
        var entityDetailViewModel = this;

        entityDetailViewModel.emptyGuid = '{00000000-0000-0000-0000-000000000000}';

        entityDetailViewModel.mappingProjectId = $stateParams.mappingProjectId || entityDetailViewModel.emptyGuid;
        entityDetailViewModel.dataStandardId = $stateParams.dataStandardId || entityDetailViewModel.emptyGuid;
        entityDetailViewModel.id = $stateParams.id;

        entityDetailViewModel.pageTitle = 'Entity Detail';

        entityDetailViewModel.getMappingProjectDetails = function() {
            entityDetailViewModel.loading = true;
            mappingProjectService.get(entityDetailViewModel.mappingProjectId)
                .then(function(data) {
                    entityDetailViewModel.mappingProject = data;
                    breadcrumbService.withMappingProject(entityDetailViewModel.mappingProject, 'dashboard');

                    angular.element("#editProjectModal").on('hidden.bs.modal', function() {
                        entityDetailViewModel.getMappingProjectDetails();
                    });
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, entityDetailViewModel);
                })
                .finally(function() {
                    entityDetailViewModel.loading = false;
                });
        }

        entityDetailViewModel.getDataStandardDetails = function() {
            entityDetailViewModel.loading = true;
            dataStandardService.get(entityDetailViewModel.dataStandardId)
                .then(function(data) {
                    entityDetailViewModel.dataStandard = data;
                    $scope.dataStandard = entityDetailViewModel.dataStandard;
                    breadcrumbService.withDataStandard(entityDetailViewModel.dataStandard);

                    angular.element("#editDataStandardModal").on('hidden.bs.modal', function() {
                        entityDetailViewModel.getDataStandardDetails();
                    });
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, entityDetailViewModel);
                })
                .finally(function() {
                    entityDetailViewModel.loading = false;
                });
        }

        entityDetailViewModel.getEntity = function() {
            entityDetailViewModel.loading = true;
            entityService.get(entityDetailViewModel.mappingProjectId, entityDetailViewModel.id)
                .then(function(data) {
                    entityDetailViewModel.entity = data;
                    _.map(entityDetailViewModel.entity.Elements, buildSrefs);
                    var ids = [];
                    _.each(entityDetailViewModel.entity.Elements, function(item) {
                        if (item.ElementDetails.ItemType.Name == 'Element')
                            ids.push({ ElementId: item.ElementDetails.SystemItemId });
                    });
                    if (entityDetailViewModel.mappingProjectId == entityDetailViewModel.emptyGuid) {
                        sessionService.cloneToSession('elementLists', entityDetailViewModel.entity.SystemItemId, ids);
                    } else {
                        sessionService.cloneToSession('elementQueues', entityDetailViewModel.entity.SystemItemId, ids);
                    }
                    entityDetailViewModel.loading = false;
                    broadcastEntityFetchedEvent();
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, entityDetailViewModel);
                })
                .finally(function() {
                    entityDetailViewModel.loading = false;
                });
        }

        function broadcastEntityFetchedEvent() {
            $scope.$broadcast('entity-fetched', entityDetailViewModel.entity);
        }

        $scope.$on('entity-changed', function() {
            entityDetailViewModel.getEntity();
        });

        entityDetailViewModel.load = function() {
            if (entityDetailViewModel.mappingProjectId != entityDetailViewModel.emptyGuid)
                entityDetailViewModel.getMappingProjectDetails();
            else
                entityDetailViewModel.getDataStandardDetails();
            entityDetailViewModel.getEntity();
        }

        if (entityDetailViewModel.dataStandardId != entityDetailViewModel.emptyGuid) {
            entityDetailViewModel.onPage = function(sref) {
                return $state.current.name === sref;
            }
            entityDetailViewModel.tabs = [
                { link: 'entityDetail.info', label: 'Details' },
                { link: 'entityDetail.actions', label: 'Actions' }
            ];

            entityDetailViewModel.showTabs = function() {
                var show = false;
                angular.forEach(entityDetailViewModel.tabs, function(tab) {
                    if ($state.is(tab.link)) {
                        show = true;
                        return;
                    }
                });
                return show;
            };
        }

        entityDetailViewModel.load();

        function buildSrefs(item) {

            var detailInfoState = '';
            var parameters = {};

            if (item.ElementDetails.ItemType.Name == 'Element') {
                detailInfoState = 'elementDetail.info';
                parameters.current = item.ElementDetails.SystemItemId;
            } else {
                detailInfoState = 'entityDetail.info';
                parameters.id = item.ElementDetails.SystemItemId;
            }

            parameters.listContextId = entityDetailViewModel.entity.SystemItemId;

            if (entityDetailViewModel.mappingProjectId != entityDetailViewModel.emptyGuid)
                parameters.mappingProjectId = entityDetailViewModel.mappingProjectId;
            else
                parameters.dataStandardId = entityDetailViewModel.dataStandardId;

            var params = '(' + angular.toJson(parameters) + ')';
            item.nameSref = detailInfoState + params;

            return item;
        }

        entityDetailViewModel.save = function() {
            $scope.$broadcast('show-errors-check-valid');

            if ($scope.entityForm.$invalid)
                return;

            elementDetailService.update(entityDetailViewModel.entity.SystemItemId, entityDetailViewModel.entity)
                .then(function(data) {
                    $("#entityModal").modal('hide');
                    entityDetailViewModel.getEntity();
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, entityDetailViewModel);
                });
        };

        entityDetailViewModel.cancel = function() {
            $scope.$broadcast('show-errors-reset');
            $("#entityModal").modal('hide');
            entityDetailViewModel.getEntity();
        };

    }
]);