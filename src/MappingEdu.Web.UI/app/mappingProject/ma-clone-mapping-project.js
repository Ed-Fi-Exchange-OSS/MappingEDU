// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appMappingProject').directive('maCloneMappingProject', function() {
    return {
        restrict: 'E',
        templateUrl: 'app/mappingProject/ma-clone-mapping-project.html',
        scope: {
            mappingProject: '=',
            modal: '=',
            cloneCallback: '='
        },
        controller: 'mappingProjectAdminController',
        controllerAs: 'mappingProjectCloneViewModel',     
    }
});