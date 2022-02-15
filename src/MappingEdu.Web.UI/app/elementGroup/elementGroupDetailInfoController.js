// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appElementGroup').controller('elementGroupDetailInfoController', [
    '_', '$', '$scope', '$timeout', '$state', '$stateParams', 'newSystemItemService', 'elementService', 'elementDetailService', 'handleErrorService', 'sessionService', 'breadcrumbService',
    function(_, $, $scope, $timeout, $state, $stateParams, newSystemItemService, elementService, elementDetailService, handleErrorService, sessionService, breadcrumbService) {
        var elementGroupDetailInfoViewModel = this;
        elementGroupDetailInfoViewModel.emptyGuid = '{00000000-0000-0000-0000-000000000000}';

        elementGroupDetailInfoViewModel.mappingProjectId = $stateParams.mappingProjectId || elementGroupDetailInfoViewModel.emptyGuid;
        elementGroupDetailInfoViewModel.dataStandardId = $stateParams.dataStandardId || elementGroupDetailInfoViewModel.emptyGuid;
        elementGroupDetailInfoViewModel.elementGroup = $scope.$parent.elementGroupDetailViewModel.elementGroup;

        if (elementGroupDetailInfoViewModel.mappingProjectId == elementGroupDetailInfoViewModel.emptyGuid)
            $timeout(function() {
                document.title = 'Data Standard Element Group Details - MappingEDU';
            }, 100);

        elementGroupDetailInfoViewModel.id = $stateParams.id;

        $scope.$on('element-group-fetched', function(event, data) {
            elementGroupDetailInfoViewModel.elementGroup = data;
        });

        elementGroupDetailInfoViewModel.emitElementGroupChangedEvent = function() {
            $scope.$emit('element-group-changed');
        };

        elementGroupDetailInfoViewModel.showEntities = true;
        elementGroupDetailInfoViewModel.toggleEntitiesCaret = function() {
            elementGroupDetailInfoViewModel.showEntities = !elementGroupDetailInfoViewModel.showEntities;
        }

        elementGroupDetailInfoViewModel.showEnumerations = true;
        elementGroupDetailInfoViewModel.toggleEnumerationsCaret = function() {
            elementGroupDetailInfoViewModel.showEnumerations = !elementGroupDetailInfoViewModel.showEnumerations;
        }

        $('#entitiesTable').on('page.dt', function() {
            var table = $(this).DataTable();
            var info = table.page.info();
            sessionService.cloneToSession('entitiesPage', elementGroupDetailInfoViewModel.id, info.page);
        });

        $('#entitiesTable').on('init.dt', function() {
            var entityPage = sessionService.cloneFromSession('entitiesPage', elementGroupDetailInfoViewModel.id);
            if (entityPage) {
                var table = $(this).DataTable();
                table.page(entityPage).draw(false);
            }
        });

        $('#enumerationsTable').on('page.dt', function() {
            var table = $(this).DataTable();
            var info = table.page.info();
            sessionService.cloneToSession('enumerationPage', elementGroupDetailInfoViewModel.id, info.page);
        });

        $('#enumerationsTable').on('init.dt', function() {
            var enumerationPage = sessionService.cloneFromSession('enumerationPage', elementGroupDetailInfoViewModel.id);
            if (enumerationPage) {
                var table = $(this).DataTable();
                table.page(enumerationPage).draw(false);
            }
        });

        elementGroupDetailInfoViewModel.edit = function(element) {
            elementGroupDetailInfoViewModel.editElement = element;
            elementGroupDetailInfoViewModel.editElement.ElementDetails.ItemDataTypeId = element.ElementDetails.ItemDataType.Id;
            elementGroupDetailInfoViewModel.isEnumeration = (elementGroupDetailInfoViewModel.editElement.ElementDetails.ItemType.Name == 'Enumeration');
            elementGroupDetailInfoViewModel.adding = false;
        }

        elementGroupDetailInfoViewModel.saveElementDetail = function() {
            var element = elementGroupDetailInfoViewModel.editElement;
            $scope.$broadcast('show-errors-check-valid');

            if ($scope.elementForm.$invalid)
                return;

            if (element.ElementDetails.SystemItemId) {
                elementDetailService.update(element.ElementDetails.SystemItemId, element.ElementDetails)
                    .then(function(data) {
                        $("#itemModal").modal('hide');
                        elementGroupDetailInfoViewModel.emitElementGroupChangedEvent();
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, elementGroupDetailInfoViewModel);
                    });
            } else {
                element.ElementDetails.ParentSystemItemId = elementGroupDetailInfoViewModel.id;
                newSystemItemService.add(element.ElementDetails)
                    .then(function(data) {
                        $("#itemModal").modal('hide');
                        var itemTypeName = _.find(Application.Enumerations.ItemType, function(type) {
                            return type.Id === data.ItemTypeId;
                        }).DisplayText;
                        if (itemTypeName == 'Entity') {
                            if (elementGroupDetailInfoViewModel.mappingProjectId != elementGroupDetailInfoViewModel.emptyGuid) {
                                $state.go('entityDetail.info', {
                                    id: data.SystemItemId,
                                    mappingProjectId: elementGroupDetailInfoViewModel.mappingProjectId
                                });
                            } else {
                                $state.go('entityDetail.info', {
                                    id: data.SystemItemId,
                                    dataStandardId: elementGroupDetailInfoViewModel.dataStandardId
                                });
                            }
                        }
                        if (itemTypeName == 'Enumeration') {
                            if (elementGroupDetailInfoViewModel.mappingProjectId != elementGroupDetailInfoViewModel.emptyGuid) {
                                $state.go('elementDetail.info', {
                                    current: data.SystemItemId,
                                    listContextId: elementGroupDetailInfoViewModel.id,
                                    mappingProjectId: elementGroupDetailInfoViewModel.mappingProjectId
                                });
                            } else {
                                $state.go('elementDetail.info', {
                                    current: data.SystemItemId,
                                    listContextId: elementGroupDetailInfoViewModel.id,
                                    dataStandardId: elementGroupDetailInfoViewModel.dataStandardId
                                });
                            }
                        }
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, elementGroupDetailInfoViewModel);
                    });
            }
        };

        elementGroupDetailInfoViewModel.cancelElementDetail = function() {
            $scope.$broadcast('show-errors-reset');
            $("#itemModal").modal('hide');
            elementGroupDetailInfoViewModel.emitElementGroupChangedEvent();
            elementGroupDetailInfoViewModel.editElement = {};
            elementGroupDetailInfoViewModel.editElement.ElementDetails = {};
            elementGroupDetailInfoViewModel.editElement.ElementDetails.ItemName = '';
            elementGroupDetailInfoViewModel.editElement.ElementDetails.TechnicalName = '';
            elementGroupDetailInfoViewModel.editElement.ElementDetails.ItemDataTypeId = null;
            elementGroupDetailInfoViewModel.editElement.ElementDetails.FieldLength = '';
            elementGroupDetailInfoViewModel.editElement.ElementDetails.DataTypeSource = '';
            elementGroupDetailInfoViewModel.editElement.ElementDetails.ItemUrl = '';
        };

        elementGroupDetailInfoViewModel.delete = function(element) {
            elementService.delete(element.ElementDetails.SystemItemId)
                .then(function() {
                    elementGroupDetailInfoViewModel.emitElementGroupChangedEvent();
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, elementGroupDetailInfoViewModel);
                });
        }

        elementGroupDetailInfoViewModel.addEntity = function() {
            elementGroupDetailInfoViewModel.editElement = {};
            elementGroupDetailInfoViewModel.editElement.ElementDetails = {};
            elementGroupDetailInfoViewModel.editElement.ElementDetails.ItemTypeName = 'Entity';
            elementGroupDetailInfoViewModel.editElement.ElementDetails.ItemDataTypeId = null;
            elementGroupDetailInfoViewModel.adding = true;
            elementGroupDetailInfoViewModel.isEnumeration = false;
        }

        elementGroupDetailInfoViewModel.addEnumeration = function() {
            elementGroupDetailInfoViewModel.editElement = {};
            elementGroupDetailInfoViewModel.editElement.ElementDetails = {};
            elementGroupDetailInfoViewModel.editElement.ElementDetails.ItemTypeName = 'Enumeration';
            elementGroupDetailInfoViewModel.editElement.ElementDetails.ItemDataTypeId = null;
            elementGroupDetailInfoViewModel.adding = true;
            elementGroupDetailInfoViewModel.isEnumeration = true;
        }

        breadcrumbService.withCurrent();
    }
]);