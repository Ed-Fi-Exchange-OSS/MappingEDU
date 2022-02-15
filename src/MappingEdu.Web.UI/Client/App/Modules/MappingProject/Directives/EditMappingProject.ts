// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.directives.edit-project
//

var m = angular.module('app.modules.mapping-project.directives.edit-project', []);


// ****************************************************************************
// Directive ma-edit-mapping-project
//

m.directive('maEditMappingProject', ['settings', (settings: ISystemSettings) => ({
    restrict: 'E',
    templateUrl: `${settings.moduleBaseUri}/mappingProject/directives/EditMappingProject.tpl.html`,
    scope: {
        mappingProject: '=',
        modal: '='
    },
    controller: 'app.modules.mapping-project.detail.admin',
    controllerAs: 'mappingProjectAdminViewModel'
})]);
