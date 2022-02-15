// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appElementDetail').controller('enumerationUsageController', [
    '_', '$scope', '$stateParams', 'sessionService', 'breadcrumbService', 
    function enumerationUsageController(_, $scope, $stateParams, sessionService, breadcrumbService) {
        var enumerationUsageViewModel = this;
        enumerationUsageViewModel.emptyGuid = '{00000000-0000-0000-0000-000000000000}';
        enumerationUsageViewModel.mappingProjectId = $stateParams.mappingProjectId || enumerationUsageViewModel.emptyGuid;;
        enumerationUsageViewModel.dataStandardId = $stateParams.dataStandardId || enumerationUsageViewModel.emptyGuid;;
        enumerationUsageViewModel.id = enumerationUsageViewModel.mappingProjectId ?
            enumerationUsageViewModel.mappingProjectId : enumerationUsageViewModel.dataStandardId;

        $scope.$on('enumeration-fetched', function (event, data) {
            enumerationUsageViewModel.enumeration = data;
        });

        enumerationUsageViewModel.loadFromSession = function () {
            var currentEnumeration = sessionService.cloneFromSession('enumerationDetail', enumerationUsageViewModel.id);
            if (currentEnumeration)
                enumerationUsageViewModel.enumeration = currentEnumeration;
        };

        enumerationUsageViewModel.showEnumerationUsage = true;
        enumerationUsageViewModel.toggleEnumerationUsageCaret = function () {
            enumerationUsageViewModel.showEnumerationUsage = !enumerationUsageViewModel.showEnumerationUsage;
        };

        enumerationUsageViewModel.loadFromSession();

        breadcrumbService.withCurrent();
    }
]);