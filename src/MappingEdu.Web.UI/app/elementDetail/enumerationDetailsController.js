// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


angular.module('appElementDetail').controller('enumerationDetailsController', ['_', '$scope', '$timeout', '$state', '$stateParams', 'enumerationItemService', 'mapNoteService', 'handleErrorService', 'breadcrumbService', 'sessionService',
function enumerationDetailsController(_, $scope, $timeout, $state, $stateParams, enumerationItemService, mapNoteService, handleErrorService, breadcrumbService, sessionService) {
    var enumerationDetailsViewModel = this;
    enumerationDetailsViewModel.emptyGuid = '{00000000-0000-0000-0000-000000000000}';
    enumerationDetailsViewModel.mappingProjectId = $stateParams.mappingProjectId || enumerationDetailsViewModel.emptyGuid;;
    enumerationDetailsViewModel.dataStandardId = $stateParams.dataStandardId || enumerationDetailsViewModel.emptyGuid;;
    enumerationDetailsViewModel.id = enumerationDetailsViewModel.mappingProjectId ?
        enumerationDetailsViewModel.mappingProjectId : enumerationDetailsViewModel.dataStandardId;

    $scope.$on('element-fetched', function (event, data) {
        handleErrorService.clearErrors(enumerationDetailsViewModel);
        enumerationDetailsViewModel.element = data;
    });

    $scope.$on('enumeration-fetched', function (event, data) {
        enumerationDetailsViewModel.enumeration = data;
    });

    enumerationDetailsViewModel.loadFromSession = function () {
        var currentElement = sessionService.cloneFromSession('elementDetail', enumerationDetailsViewModel.id);
        if (currentElement)
            enumerationDetailsViewModel.element = currentElement;

        var currentEnumeration = sessionService.cloneFromSession('enumerationDetail', enumerationDetailsViewModel.id);
        if (currentEnumeration)
            enumerationDetailsViewModel.enumeration = currentEnumeration;
    };

    enumerationDetailsViewModel.edit = function (enumerationItem) {
        enumerationDetailsViewModel.enumerationItemCurrent = enumerationItem;
        enumerationDetailsViewModel.enumerationItemEdit = angular.copy(enumerationItem);
    };

    enumerationDetailsViewModel.add = function () {
        enumerationDetailsViewModel.enumerationItemEdit = { CodeValue: '', ShortDescription: '', Description: '' };
    };

    enumerationDetailsViewModel.save = function (enumerationItem) {
        $scope.$broadcast('show-errors-check-valid');
        if (_.isUndefined(enumerationItem) || $scope.enumerationForm.$invalid)
            return;

        var timer = $timeout(showLoading, 100);
        if (enumerationItem.SystemEnumerationItemId) {
            enumerationItemService.update(
                    enumerationDetailsViewModel.enumeration.SystemItemId, enumerationItem.SystemEnumerationItemId, enumerationItem)
                .then(function (data) {
                    enumerationDetailsViewModel.enumerationItemCurrent.CodeValue = data.CodeValue;
                    enumerationDetailsViewModel.enumerationItemCurrent.ShortDescription = data.ShortDescription;
                    enumerationDetailsViewModel.enumerationItemCurrent.Description = data.Description;
                })
                .catch(function (error) {
                    handleErrorService.handleErrors(error, enumerationDetailsViewModel);
                })
                .finally(function () {
                    $timeout.cancel(timer);
                    enumerationDetailsViewModel.loading = false;
                    closeEditModal();
                });
        } else {
            enumerationItemService.add(enumerationDetailsViewModel.enumeration.SystemItemId, enumerationItem)
                .then(function (data) {
                    enumerationDetailsViewModel.enumeration.EnumerationItems.push(data);
                })
                .catch(function (error) {
                    handleErrorService.handleErrors(error, enumerationDetailsViewModel);
                })
                .finally(function () {
                    $timeout.cancel(timer);
                    enumerationDetailsViewModel.loading = false;
                    closeEditModal();
                });
        }
    };

    function showLoading() {
        enumerationDetailsViewModel.loading = true;
    }

    enumerationDetailsViewModel.cancel = function () {
        $scope.$broadcast('show-errors-reset');
        closeEditModal();
    };

    enumerationDetailsViewModel.delete = function (enumerationItem) {
        if (_.isUndefined(enumerationItem))
            return;

        enumerationItemService.delete(
            enumerationDetailsViewModel.enumeration.SystemItemId, enumerationItem.SystemEnumerationItemId)
            .then(function () {
                var index = enumerationDetailsViewModel.enumeration.EnumerationItems.indexOf(enumerationItem);
                if (index >= 0)
                    enumerationDetailsViewModel.enumeration.EnumerationItems.splice(index, 1);
            })
            .catch(function (error) {
                handleErrorService.handleErrors(error, enumerationDetailsViewModel);
            })
            .finally(function () {
                $timeout.cancel(timer);
                enumerationDetailsViewModel.loading = false;
                closeEditModal();
            });
    }

    function closeEditModal() {
        angular.element('#editEnumerationItemModal').modal('hide');
    }

    enumerationDetailsViewModel.showEnumerationItems = true;
    enumerationDetailsViewModel.toggleEnumerationItemsCaret = function () {
        enumerationDetailsViewModel.showEnumerationItems = !enumerationDetailsViewModel.showEnumerationItems;
    };

    enumerationDetailsViewModel.loadFromSession();

    breadcrumbService.withCurrent();
}]);