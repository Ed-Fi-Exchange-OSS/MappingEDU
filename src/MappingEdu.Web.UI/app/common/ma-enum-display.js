// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

(function () {
    angular.module('appCommon').
        directive('maEnumDisplay', ['_', function(_) {
        return {
            restrict: 'A',
            scope: {},
            link: function (scope, element, attrs) {
                var enumItem = _.find(angular.fromJson(attrs.enum), function (x) {
                    return x.Id == attrs.enumId;
                });
                if(enumItem)
                    element.text(enumItem.DisplayText);
                else {
                    element.text('');
                }
            }
        }
    }]);
}());