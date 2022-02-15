// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appElementGroup').controller('elementGroupDetailController', [
    '$scope', '$state', '$stateParams', 'mappingProjectService', 'dataStandardService', 'entityService', 'elementGroupService', 'handleErrorService', 'breadcrumbService', 'sessionService',
    function($scope, $state, $stateParams, mappingProjectService, dataStandardService, entityService, elementGroupService, handleErrorService, breadcrumbService, sessionService) {
        var elementGroupDetailViewModel = this;

        elementGroupDetailViewModel.emptyGuid = '{00000000-0000-0000-0000-000000000000}';

        elementGroupDetailViewModel.mappingProjectId = $stateParams.mappingProjectId || elementGroupDetailViewModel.emptyGuid;
        elementGroupDetailViewModel.dataStandardId = $stateParams.dataStandardId || elementGroupDetailViewModel.emptyGuid;
        elementGroupDetailViewModel.id = $stateParams.id;

        elementGroupDetailViewModel.pageTitle = 'Element Group Detail';

        elementGroupDetailViewModel.getMappingProjectDetails = function() {
            mappingProjectService.get(elementGroupDetailViewModel.mappingProjectId)
                .then(function(data) {
                    elementGroupDetailViewModel.mappingProject = data;
                    breadcrumbService.withMappingProject(elementGroupDetailViewModel.mappingProject, 'dashboard');
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, elementGroupDetailViewModel);
                });
        }

        elementGroupDetailViewModel.getDataStandardDetails = function() {
            dataStandardService.get(elementGroupDetailViewModel.dataStandardId)
                .then(function(data) {
                    elementGroupDetailViewModel.dataStandard = data;
                    $scope.dataStandard = elementGroupDetailViewModel.dataStandard;
                    breadcrumbService.withDataStandard(elementGroupDetailViewModel.dataStandard);
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, elementGroupDetailViewModel);
                });
        }

        elementGroupDetailViewModel.getElementGroup = function() {
            if (!elementGroupDetailViewModel.loading)
                elementGroupDetailViewModel.loading = true;
            entityService.get(elementGroupDetailViewModel.dataStandardId, elementGroupDetailViewModel.id)
                .then(function(data) {
                    elementGroupDetailViewModel.elementGroup = data;
                    _.map(elementGroupDetailViewModel.elementGroup.Elements, buildSrefs);
                    var ids = [];
                    _.each(elementGroupDetailViewModel.elementGroup.Elements, function(item) {
                        if (item.ElementDetails.ItemType.Name == 'Enumeration')
                            ids.push({ ElementId: item.ElementDetails.SystemItemId });
                    });
                    if (elementGroupDetailViewModel.mappingProjectId == elementGroupDetailViewModel.emptyGuid) {
                        sessionService.cloneToSession('elementLists', elementGroupDetailViewModel.elementGroup.SystemItemId, ids);
                    } else {
                        sessionService.cloneToSession('elementQueues', elementGroupDetailViewModel.elementGroup.SystemItemId, ids);
                    }
                    elementGroupDetailViewModel.loading = false;
                    broadcastElementGroupFetchedEvent();
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, elementGroupDetailViewModel);
                })
                .finally(function() {
                    elementGroupDetailViewModel.loading = false;
                });
        }

        function broadcastElementGroupFetchedEvent() {
            $scope.$broadcast('element-group-fetched', elementGroupDetailViewModel.elementGroup);
        }

        $scope.$on('element-group-changed', function() {
            elementGroupDetailViewModel.getElementGroup();
        });

        elementGroupDetailViewModel.load = function() {
            elementGroupDetailViewModel.loading = true;
            if (elementGroupDetailViewModel.mappingProjectId != elementGroupDetailViewModel.emptyGuid)
                elementGroupDetailViewModel.getMappingProjectDetails();
            else
                elementGroupDetailViewModel.getDataStandardDetails();
            elementGroupDetailViewModel.getElementGroup();
        }

        if (elementGroupDetailViewModel.dataStandardId != elementGroupDetailViewModel.emptyGuid) {
            elementGroupDetailViewModel.onPage = function(sref) {
                return $state.current.name === sref;
            }
            elementGroupDetailViewModel.tabs = [
                { link: 'elementGroupDetail.info', label: 'Details' },
                { link: 'elementGroupDetail.actions', label: 'Actions' }
            ];

            elementGroupDetailViewModel.showTabs = function() {
                var show = false;
                angular.forEach(elementGroupDetailViewModel.tabs, function(tab) {
                    if ($state.is(tab.link)) {
                        show = true;
                        return;
                    }
                });
                return show;
            };
        }

        elementGroupDetailViewModel.load();

        function buildSrefs(item) {

            var detailInfoState = '';
            var parameters = {};

            if (item.ElementDetails.ItemType.Name == 'Element' || item.ElementDetails.ItemType.Name == 'Enumeration') {
                detailInfoState = 'elementDetail.info';
                parameters.current = item.ElementDetails.SystemItemId;
            } else {
                detailInfoState = 'entityDetail.info';
                parameters.id = item.ElementDetails.SystemItemId;
            }

            parameters.listContextId = elementGroupDetailViewModel.elementGroup.SystemItemId;

            if (elementGroupDetailViewModel.mappingProjectId != elementGroupDetailViewModel.emptyGuid)
                parameters.mappingProjectId = elementGroupDetailViewModel.mappingProjectId;
            else
                parameters.dataStandardId = elementGroupDetailViewModel.dataStandardId;

            var params = '(' + angular.toJson(parameters) + ')';
            item.nameSref = detailInfoState + params;

            return item;
        }

        elementGroupDetailViewModel.edit = function(elementGroup) {
            elementGroupDetailViewModel.editElementGroup = elementGroup;
        };

        elementGroupDetailViewModel.save = function(elementGroup) {
            elementGroupDetailViewModel.validating = true;
            $scope.$broadcast('show-errors-check-valid');

            elementGroupService.update(elementGroupDetailViewModel.dataStandardId, elementGroup)
                .then(function() {
                    elementGroupDetailViewModel.success = true;
                    $('#elementGroupModal').modal('hide');
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, elementGroupDetailViewModel);
                })
                .finally(function() {
                    elementGroupDetailViewModel.validating = false;
                });

        };

        elementGroupDetailViewModel.cancel = function() {
            $scope.$broadcast('show-errors-reset');
            $("#elementGroupModal").modal('hide');
        };

    }
]);