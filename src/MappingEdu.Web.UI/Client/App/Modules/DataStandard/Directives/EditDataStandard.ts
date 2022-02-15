// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.data-standard.directives
//

var m = angular.module('app.modules.data-standard.directives.edit-data-standard', []);


// ****************************************************************************
// Directive ma-edit-data-standard
//

m.directive('maEditDataStandard', ['settings', (settings: ISystemSettings) => ({
    restrict: 'E',
    templateUrl: `${settings.moduleBaseUri}/dataStandard/directives/editDataStandard.tpl.html`,
    scope: {
        standard: '=',
        modal: '='
    },
    controller: 'app.modules.data-standard.create',
    controllerAs: 'dataStandardViewModel'
})]);
