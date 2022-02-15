// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appDataStandard').controller('dataStandardElementGroupsController', [
    '_', '$scope', '$state', '$stateParams', 'elementGroupService', 'dataStandardService', 'breadcrumbService', 'handleErrorService',
    function(_, $scope, $state, $stateParams, elementGroupService, dataStandardService, breadcrumbService, handleErrorService) {

        var dataStandardElementGroupsViewModel = this;
        dataStandardElementGroupsViewModel.DataStandardId = $stateParams.id;
        dataStandardElementGroupsViewModel.editSetFocus = false;

        dataStandardElementGroupsViewModel.getDataStandard = function() {
            dataStandardService.get(dataStandardElementGroupsViewModel.DataStandardId)
                .then(function(data) {
                    dataStandardElementGroupsViewModel.dataStandard = data;
                    dataStandardElementGroupsViewModel.getElementGroups();
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, dataStandardElementGroupsViewModel);
                });
        };

        dataStandardElementGroupsViewModel.getDataStandard();

        dataStandardElementGroupsViewModel.getElementGroups = function() {
            elementGroupService.getAll(dataStandardElementGroupsViewModel.DataStandardId)
                .then(function(data) {
                    var elementListState = "dataStandard.elementList";
                    var elementGroupState = "elementGroupDetail.info";
                    dataStandardElementGroupsViewModel.elementGroups = data;

                    var allElementsGroup = {
                        SystemItemId: 0,
                        ItemName: "All Element Groups",
                        Definition: "Collection of All Element Groups assigned to this DataStandard."
                    };

                    dataStandardElementGroupsViewModel.elementGroups.unshift(allElementsGroup);

                    _.each(dataStandardElementGroupsViewModel.elementGroups, function(elementGroup) {
                        elementGroup.DataStandardId = dataStandardElementGroupsViewModel.dataStandard.DataStandardId,
                            elementGroup.itemNameSref = elementListState
                                + '({ id:\'' + elementGroup.DataStandardId + '\', filter: \'' + elementGroup.SystemItemId
                                + '\' })';
                        elementGroup.elementGroupDetailSref = elementGroupState
                            + '({ dataStandardId:\'' + elementGroup.DataStandardId
                            + '\', id: \'' + elementGroup.SystemItemId + '\' })';
                        elementGroup.listSref = elementGroup.itemNameSref;
                    });
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, dataStandardElementGroupsViewModel);
                });
        };

        angular.element(document.querySelector("#uploadElementGroup"))
            .on('hidden.bs.modal', function() {
                dataStandardElementGroupsViewModel.getDataStandard();
            });

        dataStandardElementGroupsViewModel.create = function() {
            dataStandardElementGroupsViewModel.currentElementGroup = {
                DataStandardId: dataStandardElementGroupsViewModel.DataStandardId
            };
            dataStandardElementGroupsViewModel.editSetFocus = true;
            dataStandardElementGroupsViewModel.editModalTitle = 'Create New Element Group';
        };

        dataStandardElementGroupsViewModel.edit = function(elementGroup) {
            dataStandardElementGroupsViewModel.currentElementGroup = elementGroup;
            dataStandardElementGroupsViewModel.editSetFocus = true;
            dataStandardElementGroupsViewModel.editModalTitle = 'Edit Element Group';
        };

        dataStandardElementGroupsViewModel.save = function(elementGroup) {
            $scope.$broadcast('show-errors-check-valid');

            if (_.isUndefined(elementGroup) || $scope.editElementGroupForm.$invalid)
                return;

            if (!_.isUndefined(elementGroup.SystemItemId) && elementGroup.SystemItemId.length > 0) {
                elementGroupService.update(elementGroup.DataStandardId, elementGroup)
                    .then(function() {
                        dataStandardElementGroupsViewModel.success = true;
                        exitModal();
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, dataStandardElementGroupsViewModel);
                    });
            } else {
                elementGroupService.add(elementGroup)
                    .then(function() {
                        dataStandardElementGroupsViewModel.success = true;
                        exitModal();
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, dataStandardElementGroupsViewModel);
                    });
            }
        };

        dataStandardElementGroupsViewModel.cancel = function() {
            $scope.$broadcast('show-errors-reset');
            exitModal();
        };

        function exitModal() {
            angular.element('#editElementGroup').modal('hide');
            dataStandardElementGroupsViewModel.editSetFocus = false;
            dataStandardElementGroupsViewModel.getDataStandard();
        }

        dataStandardElementGroupsViewModel.delete = function(elementGroup) {
            elementGroupService.delete(elementGroup.MappedSystemId, elementGroup.SystemItemId)
                .then(function() {
                    dataStandardElementGroupsViewModel.getDataStandard();
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, dataStandardElementGroupsViewModel);
                });
        };

        breadcrumbService.withCurrent();
    }
]);

