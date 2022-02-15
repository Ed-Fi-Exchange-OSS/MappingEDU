// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appDataStandard').controller('dataStandardDetailController', [
    '$scope', '$state', '$stateParams', 'dataStandardService', 'breadcrumbService', 'handleErrorService',
    function($scope, $state, $stateParams, dataStandardService, breadcrumbService, handleErrorService) {

        var dataStandardDetailViewModel = this;
        dataStandardDetailViewModel.id = $stateParams.id;
        dataStandardDetailViewModel.pageTitle = 'Standard and Element Group Detail';

        dataStandardDetailViewModel.getDataStandardDetails = function() {
            dataStandardDetailViewModel.loading = true;
            dataStandardService.get(dataStandardDetailViewModel.id)
                .then(function(data) {
                    dataStandardDetailViewModel.dataStandard = data;
                    breadcrumbService.withDataStandard(data);
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, dataStandardDetailViewModel);
                })
                .finally(function() {
                    dataStandardDetailViewModel.loading = false;
                });
        }

        dataStandardDetailViewModel.getDataStandardDetails();

        dataStandardDetailViewModel.onPage = function(sref) {
            return $state.current.name === sref;
        }

        dataStandardDetailViewModel.tabs = [
            { link: 'dataStandard.info', label: 'Data Standard Info' },
            { link: 'dataStandard.sourceProjects', label: 'Source Mapping Projects' },
            { link: 'dataStandard.targetProjects', label: 'Target Mapping Projects' },
            { link: 'dataStandard.elementGroups', label: 'Element Group' }
        ];

        dataStandardDetailViewModel.showTabs = function() {
            var show = false;
            angular.forEach(dataStandardDetailViewModel.tabs, function(tab) {
                if ($state.is(tab.link)) {
                    show = true;
                    return;
                }
            });
            return show;
        };

        angular.element(document.querySelector("#editDataStandardModal")).on('hidden.bs.modal', function() {
            dataStandardDetailViewModel.getDataStandardDetails();
        });
    }
]);