// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.standard-access

var m = angular.module('app.directives.standard-access', []);

m.directive('maStandardAccess', ['services', 'enumerations', (services: IServices, enumerations: IEnumerations) => {
    return {
            scope: {
                standardId: '=',
                access: '@'
            },
            link: (scope, elem, attrs) => {
                if (!scope.access) scope.access = 'Guest';
                var accessLevel = enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf(scope.access)].Id;
                if (scope.standardId != null && scope.standardId !== '{00000000-0000-0000-0000-000000000000}') {
                    services.profile.dataStandardAccess(scope.standardId).then(data => {
                        services.profile.me().then((me) => {
                            if ((data && data.Role >= accessLevel) || me.IsAdministrator) elem.show();
                            else elem.remove();
                        });
                    });
                }
            }
        }
    }
]);


// ****************************************************************************
// Module app.directives.project-access

var m = angular.module('app.directives.project-access', []);

m.directive('maProjectAccess', ['services', 'enumerations', (services: IServices, enumerations: IEnumerations) => {
    return {
        scope: {
            projectId: '=',
            access: '@'
        },
        link: (scope, elem, attrs) => {
            if (!scope.access) scope.access = 'Guest';
            var accessLevel = enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf(scope.access)].Id;
            if (scope.projectId != null && scope.projectId !== '{00000000-0000-0000-0000-000000000000}') {
                services.profile.mappingProjectAccess(scope.projectId).then(data => {
                    services.profile.me().then((me) => {
                        if ((data && data.Role >= accessLevel) || me.IsAdministrator) elem.show();
                        else elem.remove();
                    });
                });
            }
        }
    }
}
]);

