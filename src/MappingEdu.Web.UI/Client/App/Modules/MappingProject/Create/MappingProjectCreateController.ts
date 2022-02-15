// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.create
//

var m = angular.module('app.modules.mapping-project.create', []);

// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.mapping-project.create', { //createMappingProject
            url: '/create',
            data: {
                roles: ['user'],
                title: 'Create Mapping Project'
            },
            views: {
                'content@': {
                    templateUrl: `${settings.moduleBaseUri}/mappingProject/create/mappingProjectCreateView.tpl.html`,
                    controller: 'app.modules.mapping-project.create',
                    controllerAs: 'mappingProjectDetailViewModel'
                }
            }
        });
}]);


// ****************************************************************************
// Controller createNewMappingProjectController
//

m.controller('app.modules.mapping-project.create', ['services', function(services: IServices) {

    services.logger.debug('Loaded controller app.modules.mapping-project.create');

}]);
