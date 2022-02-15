// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.modal-set-focus
//

var m = angular.module('app.directives.modal-set-focus', []);


// ****************************************************************************
// Directive ma-modal-set-focus
//

m.directive('modalSetFocus', ['$timeout', '$parse', ($timeout, $parse) => ({
        restrict: 'A',
        link(scope, element, attrs) {
            var model = $parse(attrs.modalSetFocus);
            scope.$watch(model, value => {
                if (value === true) {
                    $timeout(() => {
                        element[0].focus();
                        element[0].select();
                    }, 500);
                }
            });
        }
    })
]);
