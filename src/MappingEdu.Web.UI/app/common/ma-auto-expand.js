// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

(function () {
    angular.module('appCommon')
        .directive('maAutoExpand', function() {
            return {
                restrict: 'A',
                link: function($scope, elem, attrs) {
                    elem.bind('keyup', function($event) {
                        var element = $event.target;

                        $(element).height(0);
                        var height = $(element)[0].scrollHeight;
                        $(element).height(height);
                    });

                    // Expand the text area as soon as it is added to the DOM
                    $scope.$watch(attrs.ngModel, function() {
                        var element = elem;

                        $(element).height(0);
                        var height = $(element)[0].scrollHeight;
                        $(element).height(height);
                    });
                }
            };
        });
}());