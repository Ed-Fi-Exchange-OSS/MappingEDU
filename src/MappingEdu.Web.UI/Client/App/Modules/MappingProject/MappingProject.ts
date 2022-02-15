// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project
//

var m = angular.module('app.modules.mapping-project', [
    'app.modules.mapping-project.directives.clone-project',
    'app.modules.mapping-project.directives.edit-project',
    'app.modules.mapping-project.directives.project-status',
    'app.modules.mapping-project.create',
    'app.modules.mapping-project.detail',
    'app.modules.mapping-project.list']);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', ($stateProvider: ng.ui.IStateProvider) => {

    $stateProvider
        .state('app.mapping-project', {
            url: '/mapping-project',
            data: {
                roles: ['user', 'guest']
            }
        });

}]);
