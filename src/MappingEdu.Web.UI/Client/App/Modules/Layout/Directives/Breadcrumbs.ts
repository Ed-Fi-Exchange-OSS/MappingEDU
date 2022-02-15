// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.layout.breadcrumb
//

var m = angular.module('app.layout.breadcrumb', []);


// ****************************************************************************
// Directive ma-breadcrumb
//

m.directive('maBreadcrumb', ['settings', (settings: ISystemSettings) => ({
    restrict: 'E',
    scope: { links: '=' },
    templateUrl: `${settings.moduleBaseUri}/Layout/Directives/Breadcrumbs.tpl.html`
})]);
