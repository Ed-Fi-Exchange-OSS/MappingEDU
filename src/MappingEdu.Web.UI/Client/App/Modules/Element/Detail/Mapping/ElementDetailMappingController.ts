// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.element.detail.mapping
//

var m = angular.module('app.modules.element.detail.mapping', [
    'app.modules.element.detail.mapping.project',
    'app.modules.element.detail.mapping.standard'
]);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $stateProvider
        .state('app.element.detail.mapping', {
            url: '/mapping',
            data: {
                title: 'Element Detail Mapping'
            },
            templateUrl: ($stateParams: any) => {
                if ($stateParams.dataStandardId) return settings.moduleBaseUri + '/Element/Detail/Mapping/Standard/ElementDetailStandardMappingView.tpl.html';
                else if ($stateParams.mappingProjectId) return settings.moduleBaseUri + '/Element/Detail/Mapping/Project/ElementDetailProjectMappingView.tpl.html';
            },
            controllerProvider: ($stateParams) => {
                if ($stateParams.dataStandardId) return 'app.modules.element.detail.mapping.standard';
                else if ($stateParams.mappingProjectId) return 'app.modules.element.detail.mapping.project';
            },
            controllerAs: 'elementDetailMappingViewModel',
            resolve: {
                mappings: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    if ($stateParams.dataStandardId) return repositories.element.mapping.find($stateParams.elementId);
                    else return null;
                }],
                mapping: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    if ($stateParams.mappingProjectId) return repositories.element.mapping.findByProject($stateParams.elementId, $stateParams.mappingProjectId);
                    else return null;
                }],
                enums: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.element.enumerationItem.getAll($stateParams.elementId);
                }]
            }
        });
}]);