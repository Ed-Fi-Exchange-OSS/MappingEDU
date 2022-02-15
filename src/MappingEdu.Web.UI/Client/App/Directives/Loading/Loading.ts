// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.loading
//

var m = angular.module('app.directives.loading', []);


// ****************************************************************************
// Directive ma-loading
//

m.directive('maLoading', ['settings', (settings: ISystemSettings) => ({
    restrict: 'E',
    scope: {
        viewModel: '=viewModel'
    },
    templateUrl: `${settings.directiveBaseUri}/Loading/Loading.tpl.html`
})]);
