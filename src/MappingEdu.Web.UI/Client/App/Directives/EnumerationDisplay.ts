// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.enumeration-display
//

var m = angular.module('app.directives.enumeration-display', []);


// ****************************************************************************
// Directive ma-enumeration-display
//

m.directive('maEnumerationDisplay', ['_', 'enumerations', (_, enumerations: IEnumerations) => ({
    restrict: 'A',
    template: '{{enumDisplay}}',
    scope: {
        enumId: '=',
        type: '@'       
    },
    link(scope) {
        var enumItem = _.find(enumerations[scope.type], x => (x.Id.toString() === scope.enumId.toString()));
        if (enumItem) scope.enumDisplay = enumItem.DisplayText;
    }
})]);
