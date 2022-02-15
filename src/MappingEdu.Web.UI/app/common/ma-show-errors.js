// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

(function () {
    angular.module('appMappingEdu')
        .directive('maShowErrors', ['$timeout', function($timeout) {
                return {
                    restrict: 'A',
                    require: '^form',
                    scope: {},
                    link: function(scope, element, attrs, formCtrl) {
                        var inputElement = element.find('input, textarea, select');

                        var inputName = inputElement.attr('name');

                        inputElement.bind('blur', function() {
                            $timeout(function() {
                                showValidation();
                            }, 200, false);
                        });

                        function showValidation() {
                            if (!_.isUndefined(formCtrl[inputName])) {
                                element.toggleClass('has-error', formCtrl[inputName].$invalid);
                                element.toggleClass('has-success', formCtrl[inputName].$valid);
                            }
                        }

                        inputElement.bind('keyup change', function() {
                            if (formCtrl[inputName].$valid) {
                                element.removeClass('has-error');
                                element.addClass('has-success');
                            }
                            if (formCtrl[inputName].$error.maxlength) {
                                showValidation();
                            }
                        });

                        scope.$on('show-errors-check-valid', function() {
                            showValidation();
                        });

                        scope.$on('show-errors-reset', function() {
                            $timeout(function() {
                                element.removeClass('has-error');
                                element.removeClass('has-success');
                            }, 0, false);
                        });
                    }
                }
            }
        ]);
}());