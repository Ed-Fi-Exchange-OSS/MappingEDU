// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.focus-if
//

var m = angular.module('app.directives.focus-if', []);


// ****************************************************************************
// Directive ma-focus-if
//

m.directive('maFocusIf', (services) => ({
    restrict: 'A',
    link(scope, element, attrs : any) { // TODO: Need to cast appropriately (cpt)
        scope.$watch(attrs.maFocusIf, value => {
            if (value === true) {
                services.timeout(() => {
                    element[0].focus();
                }, 10);
            }
        });
    }
}));
