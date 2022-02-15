// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.detail.actions.bulk-update
//

var m = angular.module('app.modules.mapping-project.detail.actions.bulk-update', []);

m.controller('app.modules.mapping-project.detail.actions.bulk-update', ['$', '$scope', 'repositories', 'services', 'enumerations', 'project', '$uibModalInstance',
    function($, $scope, repositories: IRepositories, services: IServices, enumerations: IEnumerations, project, $uibModalInstance: angular.ui.bootstrap.IModalServiceInstance) {

        services.logger.debug('Loaded controller app.modules.mapping-project.detail.actions.bulk-update');

        var ctrl = this;

        ctrl.view = 'select-entity';
        ctrl.project = project;
        ctrl.mappedEffected = 0;
        ctrl.unmappedEffected = 0;
        ctrl.active = { tab: 'browse' };
        ctrl.mappingOption;

        ctrl.workflowStatuses = services.underscore.select(enumerations.WorkflowStatusType, type => type.Id > 0 && type.Id < 5);
        ctrl.mappingMethods = services.underscore.select(enumerations.MappingMethodTypeInQueue, type => type.Id !== 2 && type.Id > 0);
        ctrl.mappingMethodsUnmapped = services.underscore.select(enumerations.MappingMethodTypeInQueue, type => type.Id > 2);

        ctrl.mapped = {
            MappingProjectId: project.MappingProjectId,
            Statuses: [],
            Methods: [],
            ChangeStatus: {},
            ItemType: { Id: 0, DisplayText: 'Element/Enumeration' }
        }

        ctrl.unmapped = {
            MappingProjectId: project.MappingProjectId,
            ItemType: { Id: 0, DisplayText: 'Element/Enumeration' }
        }
    
        ctrl.itemTypes = [
            { Id: 4, DisplayText: 'Element' },
            { Id: 5, DisplayText: 'Enumeration' },
            { Id: 0, DisplayText: 'Element/Enumeration' }
        ];

        ctrl.elementGroups = [];
        repositories.elementGroup.getAll(project.SourceDataStandardId).then((data) => {
            ctrl.elementGroups = data;
        });

        var visitedBrowse = false;

        ctrl.isDisabled = tab => {
            if (tab === 'browse') {
                if (ctrl.selectedBrowseItem && ctrl.selectedBrowseItem.ItemTypeId < 4) return false;
                else return true;
            } else if (tab === 'search') {
                if (ctrl.selectedSearchItems && ctrl.selectedSearchItems.length > 0) return false;
                else return true;
            }
        }

        $scope.$watch('ctrl.browseElement', (newVal, oldVal) => {
            if (newVal) {
                if (ctrl.active) ctrl.active.tab = 'browse';
            }
        });

        ctrl.setTab = (tab) => {
            ctrl.active.tab = tab;
            if (tab === 'browse' && !visitedBrowse) {
                visitedBrowse = true;
            }
        }

        ctrl.select = (tab) => {
            ctrl.selectedItems = [];
            if (tab === 'all') ctrl.selectedItems = 'all';
            var selectedItemIds = [];
            ctrl.view = 'select-bulk-option';
            
            if (tab === 'browse') {
                ctrl.selectedItems = [ctrl.selectedBrowseItem];
                selectedItemIds = [ctrl.selectedBrowseItem.SystemItemId];
            } else if (tab === 'search') {
                ctrl.selectedItems = angular.copy(ctrl.selectedSearchItems);

                var sortedSearchItems = services.underscore.sortBy(<Array<any>>ctrl.selectedSearchItems, 'DomainItemPath');
                var removedDuplicatesList = [];

                angular.forEach(sortedSearchItems, (item, index) => {
                    if ((index > 0 && !item.DomainItemPath.startsWith(removedDuplicatesList[removedDuplicatesList.length - 1].DomainItemPath + '.')
                        || index === 0 )) {
                        selectedItemIds.push(item.SystemItemId);
                        removedDuplicatesList.push(item);
                    }
                });

            }

            ctrl.mapped.SystemItemIds = selectedItemIds;
            ctrl.unmapped.SystemItemIds = selectedItemIds;

            ctrl.getAddCount();

            $('#select-entity').toggle(800);
            $('#select-bulk-option').toggle(800);
        }

        ctrl.prev = () => {
            if (ctrl.view === 'select-bulk-option') {
                ctrl.view = 'select-entity';

                $('#select-entity').toggle(800);

                if (ctrl.showSelected) {
                    ctrl.showSelected = false;
                    $('#selected-entities').toggle(800);
                }

            } else {
                ctrl.view = 'select-bulk-option';
                $('#select-mappings').toggle(800);
            }

            $('#select-bulk-option').toggle(800);
        }

        ctrl.next = () => {

            //Workaround for angular ui selects
            if (ctrl.mappingOption === 'mapped' && $('#select-mappings').is(':hidden'))
                services.timeout(() => { ctrl.showMultiselects = true; }, 800);
            else 
                ctrl.showMultiselects = false;

            ctrl.view = 'select-mappings';
            $('#select-bulk-option').toggle(800);
            $('#select-mappings').toggle(800);
            
        }

        ctrl.showSelectedEntities = () => {
            ctrl.showSelected = !ctrl.showSelected;

            $('#selected-entities').toggle(800);
        }

        ctrl.selected = (tab) => {
            if (tab === 'search') {
                if (ctrl.selectedSearchItems && ctrl.selectedSearchItems.length >= 2) return `(${ctrl.selectedSearchItems.length})`;
            }
        }

        ctrl.getUpdateCount = () => {
            if (ctrl.mapped.Statuses.length === 0 || ctrl.mapped.Methods.length === 0)
                ctrl.mappedEffected = 0;
            else {
                ctrl.loading = true;
                repositories.mappingProject.systemItemMaps.getUpdateCount(project.MappingProjectId, ctrl.mapped).then(data => {
                    ctrl.loading = false;
                    ctrl.mappedEffected = data.CountUpdated;
                });
            }
        }

        ctrl.getAddCount = () => {
            ctrl.loading = true;
            repositories.mappingProject.systemItemMaps.getAddCount(project.MappingProjectId, ctrl.unmapped).then(data => {
                ctrl.loading = false;
                ctrl.unmappedEffected = data.CountUpdated;
            });
        }

        ctrl.update = () => {
            if (ctrl.mappingOption === 'mapped') {
                return repositories.mappingProject.systemItemMaps.updateMappings(project.MappingProjectId, ctrl.mapped).then(data => {
                    $uibModalInstance.close(data);
                });
            } else {
                return repositories.mappingProject.systemItemMaps.addMappings(project.MappingProjectId, ctrl.unmapped).then(data => {
                    $uibModalInstance.close(data);
                });
            }
        }

        ctrl.cancel = (tab) => {
            $uibModalInstance.dismiss();
        }

    }
])