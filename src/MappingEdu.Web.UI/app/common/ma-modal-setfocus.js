// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

(function () {
    angular.module('appMappingEdu')
        .directive('modalSetFocus', ['$timeout', '$parse', function($timeout, $parse) {
                return {
                    restrict: 'A',
                    link: function(scope, element, attrs) {
                        var model = $parse(attrs.modalSetFocus);
                        scope.$watch(model, function(value) {
                            if (value === true) {
                                $timeout(function() {
                                    element[0].focus();
                                    element[0].select();
                                }, 500);
                            }
                        });
                    }
                };
            }
        ]);
}());