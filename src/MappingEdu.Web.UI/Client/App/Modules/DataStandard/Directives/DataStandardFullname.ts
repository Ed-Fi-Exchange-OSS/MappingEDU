// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.data-standard.directives.full-name
//

var m = angular.module('app.modules.data-standard.directives.full-name', []);


// ****************************************************************************
// Directive ma-data-standard-funame
//

m.directive('maDataStandardFullname', () => ({
    restrict: 'A',
    template: '{{standard ? "" : "None" }}{{standard ? standard.SystemName : ""}} {{standard ? standard.SystemVersion : "" }}',
    scope: {
        standard: '='
    }
}));
