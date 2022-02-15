// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.disable-animation
//

var m = angular.module('app.directives.disable-animation', []);


// ****************************************************************************
// Directive ma-disable-animation
//

m.directive('maDisableAnimation', ['services', (services: IServices) => ({
    restrict: 'A',
    link($scope, elem, attrs: IAutoExpandAttributes) {
        services.animate.enabled(false, elem);
    }
})]);
