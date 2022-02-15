// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appCommon').directive('maGoBack', ['$state', 'sessionService', function($state, sessionService) {
        return {
            restrict: 'A',
            link: function(scope, element) {
                var value = sessionService.cloneFromSession("navigation", "goBack");
                if (value && value.state && value.state.name) {
                    element.attr('href', $state.href(value.state.name, value.params));
                    element.html('Back to ' + (value.state.title || 'previous page'));
                }
            }
        }
    }
]);
