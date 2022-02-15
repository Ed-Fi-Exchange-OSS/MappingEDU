// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.data-standard.edit.extensions
//

var m = angular.module('app.modules.data-standard.edit.extensions', [
    'app.modules.data-standard.edit.extensions.list',
    'app.modules.data-standard.edit.extensions.report'
]);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {


    $stateProvider
        .state('app.data-standard.edit.extensions', {
            abstract: true,
            url: '/extensions',
            template: '<div ui-view></div>',
            data: {
                title: 'Extensions'
            },
            resolve: {
                extensions: ['repositories', '$stateParams', (repostiories: IRepositories, $stateParams) => {
                    return repostiories.dataStandard.extensions.getAll($stateParams.dataStandardId);
                }],
                access: ['services', '$stateParams', (services: IServices, $stateParams) => {
                    return services.profile.dataStandardAccess($stateParams.dataStandardId);
                }]
            }
        });
}]);