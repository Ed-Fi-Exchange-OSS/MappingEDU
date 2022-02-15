// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.show-errors
//

var m = angular.module('app.directives.show-errors', []);


// ****************************************************************************
// Directive ma-show-errors
//

m.directive('maShowErrors', ['$timeout', $timeout => ({
    restrict: 'A',
    require: '^form',
    scope: {},
    link(scope, element, attrs, formCtrl) {

        var inputElement = element.find('input, textarea, select');
        var inputName = inputElement.attr('name');

        function showValidation() {
            if (!_.isUndefined(formCtrl[inputName])) {
                element.toggleClass('has-error', formCtrl[inputName].$invalid);
                element.toggleClass('has-success', formCtrl[inputName].$valid);
            }
        }

        inputElement.bind('blur', () => {
            $timeout(() => {
                showValidation();
            }, 200, false);
        });

        inputElement.bind('keyup change', () => {
            if (formCtrl[inputName].$valid) {
                element.removeClass('has-error');
                element.addClass('has-success');
            }
            if (formCtrl[inputName].$error.maxlength) {
                showValidation();
            }
        });

        scope.$on('show-errors-check-valid', () => {
            showValidation();
        });

        scope.$on('show-errors-reset', () => {
            $timeout(() => {
                element.removeClass('has-error');
                element.removeClass('has-success');
            }, 0, false);
        });
    }
})]);
