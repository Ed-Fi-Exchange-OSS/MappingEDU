// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.auto-expand
//

var m = angular.module('app.directives.auto-expand', []);


// ****************************************************************************
// Directive ma-auto-expand
//

interface IAutoExpandAttributes extends ng.IAttributes {
    ngModel: any
}

m.directive('maAutoExpand', () => ({
    restrict: 'A',
    link($scope, elem, attrs: IAutoExpandAttributes) {
        elem.bind('keyup', $event => {
            var element = $event.target;
            var height = $(element)[0].scrollHeight;
            $(element).height(0);
            $(element).height(height);
        });

        // Expand the text area as soon as it is added to the DOM
        $scope.$watch(attrs.ngModel, () => {
            var element = elem;
            var height = $(element)[0].scrollHeight;
            $(element).height(0);
            $(element).height(height);
        });
    }
}));
