// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appDataStandard').directive('maDataStandardFullname', function() {
    return {
        restrict: 'A',
        template: '{{standard ? "" : "None" }}{{standard ? standard.SystemName : ""}} {{standard ? standard.SystemVersion : "" }}',
        scope: {
            standard: '='
        },
    }
});