// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.handle-success
//

var m = angular.module('app.directives.handle-success', []);


// ****************************************************************************
// Directive ma-handle-success
//

m.directive('maHandleSuccess', ['settings', (settings: ISystemSettings) => ({
    restrict: 'A',
    scope: {
        successData: '='
    },
    templateUrl: `${settings.directiveBaseUri}/HandleSuccess/HandleSuccess.tpl.html`
})]);