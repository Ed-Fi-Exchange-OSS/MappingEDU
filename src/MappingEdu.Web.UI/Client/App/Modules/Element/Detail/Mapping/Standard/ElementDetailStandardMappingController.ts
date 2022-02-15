// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.element.detail.mapping.standard
//

var m = angular.module('app.modules.element.detail.mapping.standard', []);


// ****************************************************************************
// Controller app.modules.element.detail.mapping.standard
//

m.controller('app.modules.element.detail.mapping.standard', ['enumerations','services', 'mappings',
    function (enumerations: IEnumerations, services: IServices, mappings) {

        services.logger.debug('Loaded app.modules.element.detail.mapping.standard');

        var vm = this;
        vm.mappings = mappings;
        vm.enumerationMappingStatusTypes = services.underscore.filter(<Array<any>>enumerations.EnumerationMappingStatusType, item => item.Id);

        vm.getTargetPath = targetSystemItem => {

            if (!targetSystemItem) return '';

            if (targetSystemItem.ParentSystemItem)
                return vm.getTargetPath(targetSystemItem.ParentSystemItem) + '.' + targetSystemItem.ItemName;

            return targetSystemItem.ItemName;
        };

        vm.viewNote = (note) => {
            if (!note) return '';

            var html = angular.copy(note);
            html = html.split('[~').join('<b>@');
            html = html.split(']').join('</b>');
            html = html.split('\n').join('<br/>');

            return html;
        }
    }
]);
