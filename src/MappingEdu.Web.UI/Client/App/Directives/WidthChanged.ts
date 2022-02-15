// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.directives.width-changed

var m = angular.module('app.directives.width-changed', []);

m.directive('maWidthChanged', ['services', 'enumerations', (services: IServices, enumerations: IEnumerations) => {
    return {
        scope: {
            maWidthChanged: '='
        },
        link: (scope, elem, attrs) => {
            var element = angular.element(elem);
            scope.$watch(() => {
                return element.width();
            }, (newVal, oldVal) => {
                scope.maWidthChanged(newVal, oldVal);
            });
        }
    }
}
]);
