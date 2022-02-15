// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.handle-errors
//

var m = angular.module('app.directives.handle-errors', []);


// ****************************************************************************
// Directive ma-handle-errors
//

m.directive('maHandleErrors', ['settings', (settings: ISystemSettings) => ({
    restrict: 'A',
    scope: {
        errorData: '='
    },
    templateUrl: `${settings.directiveBaseUri}/HandleErrors/HandleErrors.tpl.html`
})]);
