// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.multi-select
//

var m = angular.module('app.directives.multi-select', []);


// ****************************************************************************
// Directive ma-modal-set-focus
//

m.directive('multiSelect', ['settings', '$timeout', (settings: ISystemSettings, $timeout) => {
    return {
        restrict: 'E',
        transclude: true,
        templateUrl: `${settings.directiveBaseUri}/MultiSelect/MultiSelect.tpl.html`,
        scope: {
            options: '=',
            model: '=',
            optionsHeader: '@',
            selectionHeader: '@',
        },
        link: (scope: any, element, attrs) => {
            var select = <any>element.find('select');
            select.multiSelect({
                selectableHeader: `<div class="multiselect-header">${scope.optionsHeader}</div>`,
                selectionHeader: `<div class="multiselect-header">${scope.selectionHeader}</div>`
            });
            scope.$watch('availableOptions', () => {
                select.multiSelect('refresh');
            });
            $timeout(() => { select.multiSelect('refresh'); }, 500);
        }
    }
}]);