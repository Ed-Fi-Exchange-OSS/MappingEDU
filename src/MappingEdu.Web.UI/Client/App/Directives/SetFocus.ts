// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.set-focus
//

var m = angular.module('app.directives.set-focus', []);


// ****************************************************************************
// Directive ma-set-focus
//

m.directive('setFocus', () => ({
    restrict: 'A',
    require: '^form',
    scope: {},
    link(scope, element) {
        element[0].focus();
    }
}));
