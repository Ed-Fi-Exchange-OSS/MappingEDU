// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.is-available

var m = angular.module('app.directives.is-available', []);

m.directive('maIsAvailable', ['services', (services) => {
        return {
            scope: {
                isAvailableFunction: '=',
                current: '='
            },
            require: 'ngModel',
            link: (scope, el, attr, ctrl) => {
                ctrl.$asyncValidators.isAvailable = (modelValue) => {

                    var def = services.q.defer();

                    if (scope.current && scope.current == modelValue) {
                        def.resolve();
                    }
                    else {
                        scope.isAvailableFunction(modelValue).then(() => {
                            def.reject();
                        }, () => {
                            def.resolve();
                        });
                    }

                    return def.promise;
                }
            }
        }
    }
]);
