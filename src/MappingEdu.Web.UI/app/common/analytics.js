// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appAnalytics').config([
    '$analyticsProvider', function($analyticsProvider) {
        $analyticsProvider.registerPageTrack(function (path) {
            logger.add({ Source: "Page Viewed", Message: path });
        });

        $analyticsProvider.registerEventTrack(function (action, properties) {
            logger.add({ Source: "Event Fired", Message: angular.toJson({action: action, properties: properties}), Level: properties.level || 'Info'});
        });

    }
]);

angular.module('appAnalytics').run(['loggingService', function (loggingService) {
    var logger = window.logger || loggingService;
    window.logger = logger;
}]);


