// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.entity
//

var m = angular.module('app.modules.entity', [
    'app.modules.entity.detail'
]);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'enumerations', ($stateProvider: ng.ui.IStateProvider, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.entity', {
            url: '/entity',
            data: {
                roles: ['user', 'guest'],
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Guest')].Id
            }
        });

}]);