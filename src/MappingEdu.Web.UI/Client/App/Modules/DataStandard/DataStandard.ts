// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.data-standard
//

var m = angular.module('app.modules.data-standard', [
    'app.modules.data-standard.create',
    'app.modules.data-standard.directives.edit-data-standard',
    'app.modules.data-standard.directives.full-name',
    'app.modules.data-standard.edit',
    'app.modules.data-standard.list',
    'app.modules.data-standard.modal'
]);


// ****************************************************************************
// Configure 
//

m.config(['$urlRouterProvider', '$stateProvider', ($urlRouterProvider: ng.ui.IUrlRouterProvider, $stateProvider: ng.ui.IStateProvider) => {

    $urlRouterProvider.when('/data-standard', '/data-standard/create');

    $stateProvider
        .state('app.data-standard', {
            url: '/data-standard',
            data: {
                roles: ['user', 'guest']
            }
        });

}]);
