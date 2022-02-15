// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.detail.mapping-summary.full

var m = angular.module('app.modules.mapping-project.detail.mapping-summary.method', []);

m.directive('methodSummary', ['settings', (settings: ISystemSettings) => {
    return {
        restrict: 'E',
        templateUrl: `${settings.moduleBaseUri}/MappingProject/Detail/MappingSummary/Summaries/MethodSummary/MethodSummaryView.tpl.html`,
        scope: {
            summary: '=',
            itemType: '=',
            loading: '=',
            headerHrefs: '=',
            showMoreDetails: '='
        },
        controller: 'app.modules.mapping-project.detail.mapping-summary.actions as vm'
    }
}]);