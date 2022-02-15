// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.element-group
//

var m = angular.module('app.modules.element-group', [
    'app.modules.element-group.detail'
]);

// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', ($stateProvider: ng.ui.IStateProvider) => {

    $stateProvider
        .state('app.element-group', {
            url: '/element-group',
            data: {
                roles: ['user', 'guest']
            }
        });

}]);
