// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.role
//

var m = angular.module('app.directives.role', []);


// ****************************************************************************
// Directive ma-role
//

m.directive('maRole', (principal: IPrincipal) => ({

    restrict: 'A',

    link(
        scope,
        element,
        attrs: any
        
    ) {
        var roles = attrs.maRole.split(',');
        if (principal.isInAnyRole(roles)) {
            element.show();
        } else {
            element.remove();
        }
    }
}));
