// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.element.matchmaker.search
//

var m = angular.module('app.modules.element.matchmaker.search', []);

m.directive('maMatchmakerSearch', ['settings', (settings: ISystemSettings) => ({
    restrict: 'E',
    templateUrl: `${settings.moduleBaseUri}/element/matchmaker/searchView.tpl.html`,
    scope: {
        standardId: '=',
        itemTypeId: '=',
        selectedItems: '=',
        project: '=',
        elementGroups: '=',
        close: '&',
        onlyOne: '=',
        browseElement: '=',
        entities: '=?'
    },
    controller: 'app.modules.element.matchmaker.search',
    controllerAs: 'searchViewModel'
})]);

// ****************************************************************************
// Controller app.modules.element.matchmaker.search
//

m.controller('app.modules.element.matchmaker.search', ['$', '$scope', 'repositories', 'services', 'enumerations',
    function ($, $scope, repositories: IRepositories, services: IServices, enumerations: IEnumerations) {

        services.logger.debug('Loaded controller app.modals.matchmaker.search');
        var vm = this;
        var project = $scope.project;
        var selectedRow;

        vm.random = Math.floor(Math.random() * 10000);

        repositories.dataStandard.isExtended($scope.standardId).then(data => {
            vm.isExtended = data;
        });

        vm.loadTable = () => {
            var table = <any>$(`#matchmakerModalSearchTable${vm.random}`); // TODO: Cast properly (cpt)
            vm.table = table.DataTable(
                {
                    serverSide: true,
                    processing: true,
                    ajax: {
                        url: `api/ElementSearch/${$scope.standardId}/paging`,
                        type: 'POST',
                        data: (data) => {

                            data.ElementGroups = [];
                            if (vm.selectedElementGroups) {
                                for (var groupKey in vm.selectedElementGroups) {
                                    if (vm.selectedElementGroups[groupKey]) data.ElementGroups.push(groupKey);
                                    else vm.selectedElementGroups[groupKey] = undefined;
                                }
                            }
                            vm.allElementGroups = data.ElementGroups.length === 0;

                            data.Entities = [];
                            if (vm.selectedEntities && $scope.itemTypeId === 4) {
                                for (var key in vm.selectedEntities) {
                                    if (vm.selectedEntities[key]) data.Entities.push(key);
                                    else vm.selectedEntities[key] = undefined;
                                }
                            }
                            vm.allEntities = data.Entities.length === 0;
                            vm.selectedEntitiesLength = data.Entities.length;

                            if ($scope.itemTypeId < 4) data.ItemTypes = [1, 2, 3];
                            else data.ItemTypes = [$scope.itemTypeId];

                            data.ItemDataTypes = [];
                            if (vm.selectedDataTypes) {
                                for (var key in vm.selectedDataTypes) {
                                    if (vm.selectedDataTypes[key]) data.ItemDataTypes.push(key);
                                    else vm.selectedDataTypes[key] = undefined;
                                }
                            }
                            vm.allDataTypes = data.ItemDataTypes.length === 0;

                            data.IsExtended = null;
                            if (vm.selectedItems) {
                                if (vm.selectedItems['base'] && vm.selectedItems['extended']) {
                                    vm.allItems = false;
                                    data.IsExtended = null;
                                }
                                else if (vm.selectedItems['base']) {
                                    data.IsExtended = false;
                                    vm.selectedItems['extended'] = undefined;
                                    vm.allItems = false;
                                }
                                else if (vm.selectedItems['extended'])
                                {
                                    data.IsExtended = true;
                                    vm.selectedItems['base'] = undefined;
                                    vm.allItems = false;
                                }
                                else {
                                    data.IsExtended = null;
                                    vm.selectedItems = {};
                                    vm.allItems = true;
                                }
                            } else {
                                vm.selectedItems = {};
                                vm.allItems = true;
                            }

                            if (data.search) vm.searchText = data.search.value;
                            if (data.order && data.order[0]) vm.orderColumn = data.order[0];
                            vm.pageLength = data.length;
                            vm.pageStart = data.start;


                            vm.setFilter();

                            return data;
                        },
                        dataFilter: (data) => {
                            var result = JSON.parse(data);
                            if (vm.showSelected) {
                                result.recordsFiltered = $scope.selectedItems.length;
                                result.data = $scope.selectedItems;
                            }
                            vm.elements = angular.copy(result.data);
                            return JSON.stringify(result);
                        }
                    },
                    createdRow(row) {
                        services.compile(angular.element(row).contents())($scope);
                    },
                    columns: [
                        {
                            data: null
                        },
                        {
                            data: 'ItemTypeId'
                        },
                        {
                            data: 'ItemDataTypeId',
                            visible: ($scope.itemTypeId >= 4)
                        },
                        {
                            data: 'FieldLength',
                            visible: ($scope.itemTypeId >= 4)
                        }
                    ],
                    columnDefs: [
                        {
                            targets: 0,
                            render(element, type, row) {
                                var shortDomainItemPath = "";
                                var splitPath = element.DomainItemPath.split('.');
                                if (splitPath.length < 5) shortDomainItemPath = element.DomainItemPath;
                                else {
                                    shortDomainItemPath = splitPath[0] + '.' + splitPath[1] + '..'
                                        + splitPath[splitPath.length - 2] + '.' + splitPath[splitPath.length - 1];
                                }

                                var html = '<div style="width: 700px; overflow-wrap: break-word">';
                                if (element.IsExtended) html += ' <i class="fa fa-extended"></i> ';
                                html += `<b uib-popover="${splitPath.join(' ')}" popover-trigger="'mouseenter'" popover-placement="right">${shortDomainItemPath}</b>`;
                                if (element.Definition) html += `<br\> ${element.Definition}`;
                                html += '</div>';
                                return html;
                            },
                        },
                        {
                            targets: 1,
                            render(itemTypeId, type, row) {
                                if (itemTypeId)
                                    return enumerations.ItemType[enumerations.ItemType.map(x => x.Id).indexOf(itemTypeId)].DisplayText;
                                else return '';
                            }
                        },
                        {
                            targets: 2,
                            render(itemDataTypeId, type, row) {
                                if (itemDataTypeId)
                                    return enumerations.ItemDataType[enumerations.ItemDataType.map(x => x.Id).indexOf(itemDataTypeId)].DisplayText;
                                else return '';
                            }
                        }
                    ],
                    displayStart: vm.pageStart,
                    order: [[vm.orderColumn.column, vm.orderColumn.dir]],
                    pageLength: vm.pageLength,
                    search: { search: vm.searchText }
                });

            $('.dataTables_processing').html('<div class="loading-inner"><img src= "Client/Content/Images/Loading.gif" alt= "Loading">Loading...</div>');
            $(`#matchmakerModalSearchTable${vm.random}_filter input`).css('width', '300px');


            table.on('click', 'tr', function (e) {
                var tr = $(this);
                if (tr[0]._DT_RowIndex >= 0) {
                    vm.toggleSelect(tr);
                }
            });

            table.on('contextmenu', 'tr', function (e) {
                var tr = $(this);
                if (tr[0]._DT_RowIndex >= 0) {
                    e.preventDefault();
                    selectedRow = tr;
                    var menu = $(`#search-context-menu${vm.random}`);
                    menu.show().css({
                        position: 'absolute',
                        'z-index': 1000,
                        top: tr[0].offsetTop + 120 + e.offsetY + 'px',
                        left: e.target.offsetLeft + e.offsetX + 20 + 'px'
                    });

                    if (tr.hasClass('active') || tr.hasClass('active-extended')) {
                        menu.find('#row-selected').show();
                        menu.find('#row-not-selected').hide();
                    } else {
                        menu.find('#row-selected').hide();
                        menu.find('#row-not-selected').show();
                    }
                }
            });

            $(document).on('click', () => {
                var menu = $(`#search-context-menu${vm.random}`);
                menu.hide();
            });

            table.on('dblclick', 'tr', function () {
                var tr = $(this);
                if (tr[0]._DT_RowIndex >= 0) {
                    var element = vm.elements[tr[0]._DT_RowIndex];
                    $scope.selectedItems.splice(0);
                    $scope.selectedItems.push(element);
                    $scope.close();
                }
            });

            table.on('draw.dt', function () {
                $(this).find('tr').each(function () {
                    var tr = $(this);
                    if (tr[0]._DT_RowIndex >= 0) {
                        var element = vm.elements[tr[0]._DT_RowIndex];
                        var found = services.underscore.find(<Array<any>>$scope.selectedItems, item => item.SystemItemId === element.SystemItemId);
                        if (found) {
                            if(element.IsExtended) tr.addClass('active-extended');
                            else tr.addClass('active');
                        }
                    }
                });
            });

            table.on('order.dt', (e, settings) => {
                var order = vm.table.order();
                var column = settings.aoColumns[order[0][0]];
                if (column.sTitle.length > 0 && project)
                    services.logging.add({
                        Source: 'searchController',
                        Message: `${project.ProjectName} ( ${project.MappingProjectId} ) - Search results ordered by ${column.sTitle} ${order[0][1]}`
                    });
            });

            table.on('page.dt', function () {
                var info = vm.table.page.info();
                if (project)
                    services.logging.add({
                        Source: 'searchController',
                        Message: `${project.ProjectName} ( ${project.MappingProjectId} ) - Search results current page changed to page ${++info.page} of ${info.pages}`
                    });
            });
        }

        vm.getFilter = () => {

            if (!$scope.selectedItems)
                $scope.selectedItems = [];

            if (!$scope.entities)
                $scope.entities = [];

            $scope.dataTypes = enumerations.ItemDataType;

            var filter = (project) ? services.session.cloneFromSession('MatchmakerSearchFilter', project.MappingProjectId) : null;

            vm.selectedElementGroups = (filter && filter.ElementGroups) ? filter.ElementGroups : {};
            vm.allElementGroups = (filter && filter.ElementGroups) ? Object.keys(filter.ElementGroups).length > 0 : true;

            vm.selectedEntities = (filter && filter.Entities) ? filter.Entities : {};
            vm.allEntities = (filter && filter.Entities) ? Object.keys(filter.Entities).length > 0 : true;

            vm.selectedDataTypes = (filter && filter.ItemDataTypes) ? filter.ItemDataTypes : {};
            vm.allDataTypes = (filter && filter.ItemDataTypes) ? Object.keys(filter.ItemDataTypes).length > 0 : true;

            vm.selectedItems = (filter && filter.selectedItems) ? filter.selectedItems : {};
            vm.allItems = (filter && filter.allItems) ? Object.keys(filter.selectedItems).length > 0 : true;

            vm.searchText = (filter && filter.SearchText) ? filter.SearchText : '';
            vm.pageLength = (filter && filter.Length) ? filter.Length : 10;
            vm.pageStart = (filter && filter.Start) ? filter.Start : 0;
            vm.orderColumn = (filter && filter.Order) ? filter.Order : {column: 0, dir: 'asc'};

            services.timeout(() => { vm.loadTable() }, 1);

        }

        vm.getFilter();

        vm.toggleSelect = (tr) => {

            if (!tr) tr = selectedRow;

            var element = vm.elements[tr[0]._DT_RowIndex];
            if (tr.hasClass('active') || tr.hasClass('active-extended')) {
                tr.removeClass('active');
                tr.removeClass('active-extended');
                var index = $scope.selectedItems.map(x => x.SystemItemId).indexOf(element.SystemItemId);
                $scope.selectedItems.splice(index, 1);
            } else {
                if ($scope.onlyOne) {
                    $(`#matchmakerModalSearchTable${vm.random}`).find('tr').each(function() {
                        var tr = $(this);
                        tr.removeClass('active');
                        tr.removeClass('active-extended');
                    });
                    $scope.selectedItems[0] = element;
                } else $scope.selectedItems.push(element);

                if (element.IsExtended) tr.addClass('active-extended');
                else tr.addClass('active');
            }

            services.timeout(() => { $scope.$apply(); });
        };

        vm.viewInBrowse = () => {
            $scope.browseElement = null;
            services.timeout(() => {
                $scope.browseElement = vm.elements[selectedRow[0]._DT_RowIndex];
            }, 20);
        }

        vm.clearElementGroups = () => {
            vm.selectedElementGroups = {};
            vm.allElementGroups = true;
            if ($scope.entities) vm.entitiesLength = $scope.entities.length;
            vm.redraw();
        }

        vm.clearEntities = () => {
            vm.selectedEntities = {};
            vm.allEntities = true;
            vm.selectedEntitiesLength = 0;
            vm.redraw();
        }

        vm.clearDataTypes = () => {
            vm.selectedDataTypes = {};
            vm.allDataTypes = true;
            vm.redraw();
        }

        vm.clearItems = () => {
            vm.selectedItems = {};
            vm.allItems = true;
            vm.redraw();
        }

        vm.setFilter = () => {
            var filter = {
                ElementGroups: vm.selectedElementGroups,
                Entities: vm.selectedEntities,
                ItemDataTypes: vm.selectedDataTypes,
                SearchText: vm.searchText,
                Order: vm.orderColumn,
                Length: vm.pageLength,
                Start: vm.pageStart,
                Items: vm.selectedItems
            }

            if(project) services.session.cloneToSession('MatchmakerSearchFilter', project.MappingProjectId, filter);
        }

        vm.redraw = () => { vm.table.draw(); }

        vm.displayEntity = (entityId) => {
            if (entityId && $scope.entities) {
                var entity = services.underscore.find(<Array<any>>$scope.entities, item => item.SystemItemId === entityId);
                if (entity) return entity.ItemName;
            }
        }

        vm.entitySearch = row => {
            var keys = [];
            if (vm.selectedElementGroups) {
                for (var groupKey in vm.selectedElementGroups) {
                    if (vm.selectedElementGroups[groupKey])
                        keys.push(groupKey);
                }
            }
             return (angular.lowercase(row.ItemName).indexOf(angular.lowercase(vm.entitySearchText)) !== -1 && (services.underscore.contains(keys, row.ParentSystemItemId) || vm.allElementGroups));
        }

        vm.clearEntitiesByGroup = () => {
            var groupKeys = [];
            var entityKeys = Object.keys(vm.selectedEntities);
            vm.entitySearchText = null;

            services.underscore.each(vm.selectedElementGroups, (val, key) => {
                if (val) groupKeys.push(key);
            });

            if (groupKeys.length === 0) vm.entitiesLength = $scope.entities.length;
            else {
                vm.entitiesLength = 0;
                angular.forEach($scope.entities, entity => {
                    if (vm.selectedElementGroups[entity.ParentSystemItemId]) vm.entitiesLength++;
                });

                angular.forEach(entityKeys, entityId => {
                    var entity = services.underscore.find(<Array<any>>$scope.entities, item => item.SystemItemId === entityId);
                    if (entity && !(services.underscore.contains(groupKeys, entity.ParentSystemItemId)))
                        vm.selectedEntities[entityId] = undefined;
                });
            }

            vm.redraw();
        }

        vm.entitiesSearchLength = () => {
            var length = 0;
            var groupKeys = [];

            if (vm.selectedElementGroups) {
                for (var groupKey in vm.selectedElementGroups) {
                    if (vm.selectedElementGroups[groupKey])
                        groupKeys.push(groupKey);
                }
            }

            angular.forEach($scope.entities, entity => {
                if (entity.ItemName.toLowerCase().indexOf(vm.entitySearchText.toLowerCase()) > -1 &&
                    (services.underscore.contains(groupKeys, entity.ParentSystemItemId) || vm.allElementGroups))
                    length++;
            });

            vm.entitiesSearchCount = length;

        }

        vm.toggleSelectedView = () => {
            vm.showSelected = !vm.showSelected;
            vm.redraw();
        }
    }
]);
