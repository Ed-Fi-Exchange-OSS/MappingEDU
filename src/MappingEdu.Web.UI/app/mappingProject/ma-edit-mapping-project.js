﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appMappingProject').directive('maEditMappingProject', function () {
    return {
        restrict: 'E',
        templateUrl: 'app/mappingProject/ma-edit-mapping-project.html',
        scope: {
            mappingProject: '=',
            modal: '='
        },
        controller: 'mappingProjectAdminController',
        controllerAs: 'mappingProjectAdminViewModel'  
    }
})