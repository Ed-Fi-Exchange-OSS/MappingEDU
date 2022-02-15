// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module 'app.modules.element.detail.mapping.project.enumeration'
//

var m = angular.module('app.modules.element.detail.mapping.project.enumeration', []);


// ****************************************************************************
// Directive mapping-enumeration
//


m.directive('mappingEnumeration', ['settings', (settings: ISystemSettings) => {
    return {
        restrict: 'E',
        scope: {
            systemItem: '=',
            enumerations: '=',
            mapping: '=',
            mappingProjectId: '=',
            readOnly: '='
        },
        templateUrl: `${settings.moduleBaseUri}/Element/Detail/Mapping/Project/EnumerationMapping/EnumerationMapping.tpl.html`,
        controller: 'app.modules.element.detail.mapping.project.enumeration',
        controllerAs: 'vm'
    }
}
]);


// ****************************************************************************
// Controller app.modules.element.detail.mapping.project.enumeration
//

m.controller('app.modules.element.detail.mapping.project.enumeration', ['$scope', 'enumerations', 'modals', 'repositories', 'services', 'settings',
    function ($scope, enumerations: IEnumerations, modals: IModals, repositories: IRepositories, services: IServices, settings: ISystemSettings) {

        services.logger.debug('Loaded app.modules.element.detail.mapping.project.enumeration');

        var vm = this;
        vm.mappingProjectId = $scope.mappingProjectId;
        vm.enumerations = $scope.enumerations;
        vm.mapping = $scope.mapping;
        vm.readOnly = $scope.readOnly;
        vm.enumerationMappingStatusTypes = services.underscore.filter(<Array<any>>enumerations.EnumerationMappingStatusType, item => item.Id);

        vm.enumerationsTableOptions = services.datatables.optionsBuilder.newOptions();
        vm.enumerationsTableColumnDefs = [
            services.datatables.columnDefBuilder.newColumnDef(3).notSortable(),
            services.datatables.columnDefBuilder.newColumnDef(4).notSortable()
        ];

        if (vm.readOnly) {
            vm.enumerationsTableColumnDefs[0].notVisible();
            vm.enumerationsTableColumnDefs[1].notVisible();
        }

        function mapEnumerationItems() {
            angular.forEach(vm.enumerations, enumeration => {
                var mapping = services.underscore.find(<Array<any>>vm.mapping.EnumerationItemMappings, enumMap => (enumeration.SystemEnumerationItemId === enumMap.SourceSystemEnumerationItemId));
                if (mapping) enumeration.Mapping = mapping;
                else enumeration.Mapping = {
                    SourceCodeValue: enumeration.CodeValue,
                    SourceSystemEnumerationItemId: enumeration.SystemEnumerationItemId
                }
            });
        }

        if(vm.mapping && vm.mapping.SystemItemMapId) mapEnumerationItems();

        vm.getTargetPath = targetSystemItem => {
            if (!targetSystemItem) return '';
            else if (targetSystemItem.ParentSystemItem) return vm.getTargetPath(targetSystemItem.ParentSystemItem) + '.' + targetSystemItem.ItemName;
            else return targetSystemItem.ItemName;
        };

        vm.runEnumerationAutoMapper = () => {

            var instance = modals.enumerationAutoMapper(vm.enumerations, vm.mapping.TargetEnumerationItems);

            //On Return
            instance.result.then((acceptedEnumerations) => {
                var count = acceptedEnumerations.length;
                var errors = 0;
                angular.forEach(acceptedEnumerations, enumeration => {
                    enumeration.Mapping.SystemItemMapId = vm.mapping.SystemItemMapId;
                    repositories.element.enumerationItemMapping.create(vm.mapping.SystemItemMapId, enumeration.Mapping)
                        .then((data) => {
                            var enumerationItem = services.underscore.find(<Array<any>>vm.enumerations, enumerationItem => (enumerationItem.SystemEnumerationItemId === data.SourceSystemEnumerationItemId));
                            enumerationItem.Mapping = data;
                        }, error => {
                            errors++;
                        })
                        .finally(() => {
                            count--;
                            if (count === 0) {
                                if (errors === 0) services.logger.success('Created enumeration item mappings.');
                                else if (errors < acceptedEnumerations.length) services.logger.warning('Error creating some enumeration item mappings.');
                                else services.logger.error('Error creating enumeration item mapping.');
                            }
                        });
                });
            });
        }

        vm.edit = (enumeration) => {
            var instance = modals.enumerationMapForm(enumeration, vm.mapping.TargetEnumerationItems, vm.mapping.SystemItemMapId);
            instance.result.then(enumerationMapping => {
                enumeration.Mapping = enumerationMapping;
            });
        }

        vm.delete = (enumeration) => {
            repositories.element.enumerationItemMapping.remove(enumeration.Mapping.SystemItemMapId, enumeration.Mapping.SystemEnumerationItemMapId).then(() => {
                services.logger.success('Removed enumeration detail mapping.');
                enumeration.Mapping = {
                    SourceCodeValue: enumeration.Mapping.SourceCodeValue,
                    SourceSystemEnumerationItemId: enumeration.Mapping.SourceSystemEnumerationItemId
                }
            }, error => {
                services.logger.error('Error removing enumeration item mapping.', error.data);
            });
        }
    }
])