// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module 'app.modules.element.detail.mapping.project.business-logic'
//

var m = angular.module('app.modules.element.detail.mapping.project.business-logic', []);


// ****************************************************************************
// Directive mapping-business-logic
//


m.directive('mappingBusinessLogic', ['settings', (settings: ISystemSettings) => {
    return {
        restrict: 'E',
        scope: {
            systemItem: '=',
            templates: '=',
            mapping: '=',
            mappingProject: '=',
            readOnly: '=',
            enumerations: '=?'
        },
        templateUrl: `${settings.moduleBaseUri}/Element/Detail/Mapping/Project/BusinessLogic/BusinessLogic.tpl.html`,
        controller: 'app.modules.element.detail.mapping.project.business-logic',
        controllerAs: 'vm'
    }
}
]);


// ****************************************************************************
// Controller ma-business-logic
//

m.controller('app.modules.element.detail.mapping.project.business-logic', ['$scope', 'enumerations', 'repositories', 'services', 'settings',
    function ($scope, enumerations: IEnumerations, repositories: IRepositories, services: IServices, settings: ISystemSettings) {

        services.logger.debug('Loaded app.modules.element.detail.mapping.project.business-logic');

        var vm = this;
        vm.mapping = $scope.mapping || { MappingProjectId: $scope.mappingProject.MappingProjectId };
        vm.mappingProject = $scope.mappingProject;
        vm.readOnly = $scope.readOnly;
        vm.systemItem = $scope.systemItem;
        vm.templates = $scope.templates;
        vm.enumerations = $scope.enumerations;

        vm.businessLogic = angular.copy(vm.mapping.BusinessLogic);
        vm.omissionReason = angular.copy(vm.mapping.OmissionReason);
        vm.mappingMethodTypeId = angular.copy(vm.mapping.MappingMethodTypeId);

        services.profile.me().then(me => { vm.me = me; });
        vm.mappingMethodTypes = services.underscore.filter(enumerations.MappingMethodType, item => (item.Id !== 0));

        vm.createBusinessLogicLabel = () => {
            if (vm.mapping.MappingMethodTypeId === 2) vm.businessLogicLabel = 'Inclusion Detail';
            else if (vm.mapping.MappingMethodTypeId === 3) vm.businessLogicLabel = 'Extension Detail';
            else if (vm.mapping.MappingMethodTypeId === 4) vm.businessLogicLabel = 'Prior Logic (readonly)';
            else vm.businessLogicLabel = 'Business Logic';
        }
        vm.createBusinessLogicLabel();

        vm.toggleSideOptions = ($event) => {
            $event.preventDefault();
            vm.showBusinessOptions = true;
            vm.isSideOptionsOpen = !vm.isSideOptionsOpen;
        }

        vm.save = () => {
            if (vm.matchmakerOpen) return;

            if (!vm.mappingMethodTypeId) {
                vm.mapping.BusinessLogic = angular.copy(vm.businessLogic);
                vm.cancel();
                services.timeout(() => $scope.$apply());
            } else if ((vm.businessLogic === vm.mapping.BusinessLogic && vm.omissionReason === vm.mapping.OmissionReason &&
                vm.mappingMethodTypeId === vm.mapping.MappingMethodTypeId)) {
                vm.cancel();
                services.timeout(() => $scope.$apply());
            } else {
                var businessLogic = angular.copy(vm.mapping.BusinessLogic);
                var omissionReason = angular.copy(vm.mapping.OmissionReason);
                var mappingMethodTypeId = angular.copy(vm.mapping.MappingMethodTypeId);

                vm.mapping.BusinessLogic = angular.copy(vm.businessLogic);
                vm.mapping.OmissionReason = angular.copy(vm.omissionReason);
                vm.mapping.MappingMethodTypeId = angular.copy(vm.mappingMethodTypeId);
                vm.createBusinessLogicLabel();

                services.loading.start('saving');
                if (vm.mapping.SystemItemMapId) {
                    repositories.element.mapping.save(vm.systemItem.SystemItemId, vm.mapping.SystemItemMapId, vm.mapping).then((data) => {
                        services.logger.success('Updated mapping.');
                        vm.mapping.UpdateBy = vm.me.FirstName[0] + '. ' + vm.me.LastName;
                        vm.mapping.UpdateDate = new Date();
                        vm.mapping.TargetEnumerationItems = data.TargetEnumerationItems;
                        vm.mapping.EnumerationItemMappings = data.EnumerationItemMappings;
                        if (vm.enumerations) mapEnumerationItems();

                        vm.cancel();
                    }, error => {
                        services.logger.error('Error updating mapping.', error);
                        vm.mapping.BusinessLogic = angular.copy(businessLogic);
                        vm.mapping.OmissionReason = angular.copy(omissionReason);
                        vm.mapping.MappingMethodTypeId = angular.copy(mappingMethodTypeId);
                    }).finally(() => {
                        services.timeout(() => {
                            vm.saving = false;
                            services.loading.finish('saving');
                        }, 200);
                    });
                } else {
                    repositories.element.mapping.create(vm.systemItem.SystemItemId, vm.mapping).then((data) => {
                        services.logger.success('Created mapping.');

                        vm.mapping.SystemItemMapId = data.SystemItemMapId;
                        vm.mapping.CreateBy = vm.me.FirstName[0] + '. ' + vm.me.LastName;
                        vm.mapping.CreateDate = new Date();
                        vm.mapping.UpdateBy = vm.me.FirstName[0] + '. ' + vm.me.LastName;
                        vm.mapping.UpdateDate = new Date();
                        vm.mapping.TargetEnumerationItems = data.TargetEnumerationItems;
                        vm.mapping.EnumerationItemMappings = data.EnumerationItemMappings;
                        vm.mapping.WorkflowStatusTypeId = data.WorkflowStatusTypeId;
                        vm.mapping.WorkflowStatusType = data.WorkflowStatusType;
                        vm.mapping.SourceSystemItemId = data.SourceSystemItemId;
                        vm.mapping.MapNotes = [];

                        if (vm.enumerations) mapEnumerationItems();

                        vm.cancel();
                    }, error => {
                        services.logger.error('Error creating mapping.', error);

                        vm.mapping.BusinessLogic = angular.copy(businessLogic);
                        vm.mapping.OmissionReason = angular.copy(omissionReason);
                        vm.mapping.MappingMethodTypeId = angular.copy(mappingMethodTypeId);

                    }).finally(() => {
                        services.timeout(() => {
                            vm.saving = false;
                            services.loading.finish('saving');
                        }, 200);
                    });
                }
            }
        }

        vm.cancel = () => {
            vm.showOptions = false;
            vm.showBusinessOptions = false;
            vm.isSideOptionsOpen = false;
            vm.businessLogic = angular.copy(vm.mapping.BusinessLogic);
            vm.omissionReason = angular.copy(vm.mapping.OmissionReason);
            vm.mappingMethodTypeId = angular.copy(vm.mapping.MappingMethodTypeId);
        }

        function setCursorPositionAndSelection(control) {
            if (services.underscore.isUndefined(control))
                return;
            var startPos = control.value.indexOf('{select-start}');
            if (startPos > -1)
                control.value = control.value.replace('{select-start}', '');
            var endPos = control.value.indexOf('{select-end}');
            if (endPos == -1)
                endPos = startPos;
            else
                control.value = control.value.replace('{select-end}', '');

            if (startPos == -1)
                return;

            services.timeout(() => { services.utils.setCaretPosition(control, startPos, endPos); }, 0);
        }

        function insertTemplate(data) {
            var blTextArea = <any>angular.element('#businessLogic'); // TODO: Cast properly (cpt)
            blTextArea.focus();
            var selEndPosition = services.utils.getCaretPosition(blTextArea[0]);
            if (selEndPosition >= 0) {
                blTextArea[0].value = services.utils.insertIntoBusinessLogic(blTextArea[0].value, data, selEndPosition);
                setCursorPositionAndSelection(blTextArea[0]);
                vm.businessLogic = blTextArea[0].value;
                return;
            }
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

        vm.addTemplate = template => {
            insertTemplate(template.Template);
            vm.isSideOptionsOpen = false;
        }

        vm.showMatchmakerModal = () => {

            vm.matchmakerOpen = true;
            vm.showBusinessOptions = true;

            var model = {
                backdrop: 'static',
                keyboard: false,
                templateUrl: `${settings.moduleBaseUri}/element/matchmaker/matchmakerView.tpl.html`,
                controller: 'app.modules.element.matchmaker as vm',
                windowClass: 'modal-xxl',
                resolve: {
                    project: () => { return vm.mappingProject },
                    element: () => { return vm.systemItem },
                    hint: () => { return repositories.entity.hint.filter(vm.mappingProject.MappingProjectId, vm.systemItem.SystemItemId) }
                }
            }

            var instance = services.modal.open(model);
            instance.result.then((selectedElements) => {
                if (selectedElements[0].IsMapping) {
                    var mapping = selectedElements[0];
                    vm.mappingMethodTypeId = mapping.MappingMethodTypeId;
                    vm.businessLogic = mapping.BusinessLogic;
                    vm.omissionReason = mapping.OmissionReason;
                } else {
                    angular.forEach(selectedElements, element => {
                        if (!vm.businessLogic) {
                            vm.businessLogic = '';
                            vm.businessLogic = services.utils.appendPathToEndOfBusinessLogic('', element.DomainItemPath);
                        } else if (!vm.businessLogic) {
                            vm.businessLogic = services.utils.appendPathToEndOfBusinessLogic('', element.DomainItemPath);
                        } else {
                            var blTextArea = angular.element('#businessLogic');
                            var businessLogic = vm.businessLogic;

                            if (blTextArea[0]) {
                                var selEndPosition = services.utils.getCaretPosition(blTextArea[0]);
                                if (selEndPosition >= 0) {
                                    vm.businessLogic = services.utils.insertPathIntoBusinessLogic(businessLogic, element.DomainItemPath, selEndPosition);
                                    return;
                                }
                            } else {
                                vm.businessLogic =
                                    services.utils.appendPathToEndOfBusinessLogic(businessLogic, element.DomainItemPath);
                            }
                        }
                    });
                }


            }).finally(() => {
                services.timeout(() => {
                    vm.matchmakerOpen = false;
                }, 500);
            });
        }

        vm.typedSystemItems = {};

        vm.getTypedSystemItems = (searchText) => {
            if (searchText.indexOf('.') > -1) return;

            var blTextArea = angular.element('#businessLogic');
            var domainItemPath = null;
            if (!services.underscore.isUndefined(blTextArea[0])) {
                var lastPosition = services.utils.getCaretPosition(blTextArea[0]);
                var firstPosition = null;

                for (var i = lastPosition; i >= 0; i--) {

                    if (vm.businessLogic.charAt(i) === ']') break;

                    if (vm.businessLogic.charAt(i) === '[') {
                        firstPosition = i + 1;
                        break;
                    }
                }

                if (firstPosition == null) return;

                domainItemPath = vm.businessLogic.substring(firstPosition, lastPosition);
            }

            return repositories.element.search(vm.mappingProject.TargetDataStandardId, null, searchText, domainItemPath).then(data => {
                vm.typedSystemItems = [];
                angular.forEach(data, item => {
                    item.label = item.ItemName;
                    if (item.ItemTypeId == 5) item.label += ' [ENUM]';
                    vm.typedSystemItems.push(item);
                });
                return services.q.when(vm.typedSystemItems);
            });
        }

        vm.selectTypedItem = (item) => {
            if (item.ItemTypeId === 1) return `[${item.ItemName}.`;
            else if (item.ItemTypeId < 4) return `.${item.ItemName}.`;
            else return `.${item.ItemName}] `;
        }

        vm.getTypedTemplates = (searchText) => {

            var typedTemplates = [];
            typedTemplates = services.underscore.filter(<Array<any>>vm.templates, (template => {
                template.label = template.Title;
                return template.Title.toLowerCase().indexOf(searchText.toLowerCase()) > -1;
            }));

            typedTemplates = services.underscore.sortBy(typedTemplates, template => { return template.Title });

            vm.typedTemplates = typedTemplates;
            return services.q.when(vm.typedTemplates);
        }

        vm.selectTemplate = (template) => {
            services.timeout(() => { insertTemplate(template.Template) }, 1);
            return '';
        }
    }
])
