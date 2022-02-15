// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.close-dropdown
//

var m = angular.module('app.directives.close-dropdown', []);


// ****************************************************************************
// Directive ma-close-dropdown
//

m.directive('maCloseDropdown', () => {

    function handleClick(e) {
        var filterDivs = $('.dropdiv');
        filterDivs.each(function () {
            if ($(this).hasClass('in')) {
                if ($(e.target).closest($(this)).length === 0) {
                    $(this).removeClass('in');
                }
            }
        });
    }

    return {
        restrict: 'A',
        link($scope, elem, attrs) {
            $(document).off('click', handleClick);
            $(document).on('click', handleClick);
        }
    }
});
