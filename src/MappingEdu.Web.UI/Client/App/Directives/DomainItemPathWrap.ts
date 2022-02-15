// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.path-wrap

var m = angular.module('app.directives.path-wrap', []);

m.directive('maDomainItemPathWrap', ['services', (services) => {
        return {
            scope: {
                path: '@'
            },
            link: (scope, el: ng.IAugmentedJQuery, attr, ctrl) => {
                var splitPath = scope.path.split('.');
                var html = '<div style="display: inline-block">';
                html += splitPath.join('.</div><div style="display: inline-block">');
                html += '</div>';

                el.html(html);
            }
        }
    }
]);
