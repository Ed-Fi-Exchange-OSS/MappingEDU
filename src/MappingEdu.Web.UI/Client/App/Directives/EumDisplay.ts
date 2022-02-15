// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.enum-display
//

var m = angular.module('app.directives.enum-display', []);


// ****************************************************************************
// Directive ma-enum-display
//

m.directive('maEnumDisplay', ['_', _ => ({
    restrict: 'A',
    scope: {},
    link(scope, element, attrs) {
        var enumItem = _.find(angular.fromJson(attrs.enum), x => {
            if (x.Id == attrs.enumId)
                return true;
            else
                return false;
        });

        if (enumItem)
            element.text(enumItem.DisplayText);
        else {
            element.text('');
        }
    }
})]);
