// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.data-standard.directives
//

var m = angular.module('app.modules.data-standard.directives.standard-name-taken', []);


// ****************************************************************************
// Directive ma-edit-data-standard
//

m.directive('maEditDataStandard', () => ({
    restrict: 'E',
    templateUrl: 'client/app/modules/dataStandard/directives/editDataStandard.tpl.html', // TODO: Use prefix constant from settings 
    scope: {
        standard: '=',
        modal: '='
    },
    controller: 'app.modules.data-standard.edit',
    controllerAs: 'dataStandardViewModel'
}));
