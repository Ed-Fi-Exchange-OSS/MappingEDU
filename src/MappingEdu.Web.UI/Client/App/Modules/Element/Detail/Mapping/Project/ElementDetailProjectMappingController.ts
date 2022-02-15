// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.element.detail.mapping.project
//

var m = angular.module('app.modules.element.detail.mapping.project', [
    'app.modules.element.detail.mapping.project.business-logic',
    'app.modules.element.detail.mapping.project.enumeration',
    'app.modules.element.detail.mapping.project.workflow-status'
]);


// ****************************************************************************
// Controller app.modules.element.detail.mapping.project
//

m.controller('app.modules.element.detail.mapping.project', ['$stateParams', 'services', 'element', 'enums', 'mapping', 'model', 'readOnly', 'templates',
    function ($stateParams, services: IServices, element, enums, mapping, model, readOnly, templates ) {

        services.logger.debug('Loaded app.modules.element.detail.mapping.project');

        var vm = this;
        vm.mappingProjectId = $stateParams.mappingProjectId;
        vm.systemItem = element;
        vm.mapping = mapping || {
            MappingProjectId: $stateParams.mappingProjectId
        };
        vm.mappingProject = model;
        vm.readOnly = readOnly;
        vm.templates = templates;
        vm.enumerations = enums;

    }
]);
