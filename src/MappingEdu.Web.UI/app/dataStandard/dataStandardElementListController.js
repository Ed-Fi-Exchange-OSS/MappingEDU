// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appDataStandard').controller('dataStandardElementListController', [
    '_', '$location', '$compile', '$scope', '$state', '$stateParams', 'elementListService', 'sessionService', 'breadcrumbService', 'handleErrorService',
    function(_, $location, $compile, $scope, $state, $stateParams, elementListService, sessionService, breadcrumbService, handleErrorService) {
        var dataStandardElementListViewModel = this;
        dataStandardElementListViewModel.id = $stateParams.id;
        dataStandardElementListViewModel.filter = $stateParams.filter;
        $scope.$parent.pageTitle = 'Element List';

        $scope.allElementGroups = true;
        $scope.elementGroups = {};
        $scope.allItemTypes = true;
        $scope.itemTypes = {};
        $scope.globalSearch = '';
        $scope.pageSize = 10;
        $scope.pageNo = 0;
        $scope.orderBy = [];

        dataStandardElementListViewModel.saveFilterToSession = function() {
            var elementListFilters = {
                elementGroups: $scope.elementGroups,
                itemTypes: $scope.itemTypes,
                globalSearch: $scope.globalSearch,
                pageSize: $scope.pageSize,
                pageNo: $scope.pageNo,
                orderBy: $scope.orderBy
            };

            sessionService.cloneToSession('elementListFilters', dataStandardElementListViewModel.id, elementListFilters);
        };

        if (dataStandardElementListViewModel.filter != undefined) {
            if (dataStandardElementListViewModel.filter.match(/^[{]?[0-9a-fA-F]{8}[-]?([0-9a-fA-F]{4}[-]?){3}[0-9a-fA-F]{12}[}]?$/)) {
                $scope.allElementGroups = false;
                $scope.elementGroups[dataStandardElementListViewModel.filter] = true;
            }
            dataStandardElementListViewModel.saveFilterToSession();
            $location.search('filter', null);
            return;
        }

        dataStandardElementListViewModel.checkForFiltering = function() {
            dataStandardElementListViewModel.needToFilterFromSession = false;

            var elementListFilters = sessionService.cloneFromSession('elementListFilters', dataStandardElementListViewModel.id);
            if (elementListFilters) {
                $scope.elementGroups = elementListFilters.elementGroups;
                $scope.itemTypes = elementListFilters.itemTypes;
                $scope.globalSearch = elementListFilters.globalSearch;
                $scope.pageSize = elementListFilters.pageSize;
                $scope.pageNo = elementListFilters.pageNo;
                $scope.orderBy = elementListFilters.orderBy;
                dataStandardElementListViewModel.needToFilterFromSession = true;
            }
        };

        dataStandardElementListViewModel.checkForFiltering();

        dataStandardElementListViewModel.getList = function() {
            dataStandardElementListViewModel.loading = true;
            elementListService.get(dataStandardElementListViewModel.id).then(function(data) {
                    dataStandardElementListViewModel.loading = false;
                    dataStandardElementListViewModel.elements = data.Elements;
                    dataStandardElementListViewModel.elementGroups = [];

                    for (var i = 0; i < dataStandardElementListViewModel.elements.length; i++) {
                        var eg = dataStandardElementListViewModel.elements[i];
                        if (!eg.PathSegments)
                            continue;
                        var systemItemId = eg.PathSegments[0].SystemItemId;
                        var ids = dataStandardElementListViewModel.elementGroups.filter(function(val) {
                            return val.Id === systemItemId;
                        });
                        if (ids.length == 0) {
                            dataStandardElementListViewModel.elementGroups.push({ Id: systemItemId, DisplayText: eg.PathSegments[0].Name });
                        }
                    }

                    $('#elementListTable').DataTable(
                    {
                        data: dataStandardElementListViewModel.elements,
                        order: [[2, "asc"]],
                        deferRender: true,
                        createdRow: function(row) {
                            var compiled = $compile(row);
                            var element = compiled($scope);
                            angular.element(row).replaceWith(element);
                        },
                        columns: [
                            {
                                data: 'PathSegments',
                                visible: false,
                                searchable: true
                            },
                            {
                                data: 'PathSegments',
                                visible: false,
                                searchable: true
                            },
                            {
                                data: 'PathSegments',
                                dataSort: [1]
                            },
                            {
                                data: 'Element'
                            },
                            {
                                data: 'Element',
                                visible: false,
                                searchable: true
                            },
                            {
                                data: 'Element.TypeName'
                            },
                            {
                                data: 'Element.Length',
                            }
                        ],
                        columnDefs: [
                            {
                                targets: 0,
                                render: function(pathSegments, type, row) {
                                    return pathSegments ? pathSegments[0].SystemItemId : row.Element.SystemItemId;
                                }
                            },
                            {
                                targets: 1,
                                render: function(pathSegments, type, row) {
                                    var html = '<div class="hidden">';
                                    var namePath = '';
                                    for (i = 0; i < pathSegments.length; i++) {
                                        namePath += namePath.length > 0 ? '.' : '';
                                        namePath += pathSegments[i].Name;
                                    }
                                    html += namePath;
                                    html += "</div>";
                                    return html;
                                }
                            },
                            {
                                targets: 2,
                                render: function(pathSegments, type, row, meta) {
                                    var html = '<div class="hidden">';
                                    var namePath = '';
                                    for (i = 0; i < pathSegments.length; i++) {
                                        namePath += namePath.length > 0 ? '.' : '';
                                        namePath += pathSegments[i].Name;
                                    }
                                    if (row.Element) {
                                        namePath += '.' + row.Element.Name;
                                    }
                                    html += namePath;
                                    html += "</div>";
                                    html += '<ma-element-path context-id="dataStandardElementListViewModel.id"';
                                    html += ' segments="dataStandardElementListViewModel.elements[';
                                    html += meta.row;
                                    html += '].PathSegments" context="dataStandard" depth="4"></ma-element-path>';
                                    html += '<br/>';
                                    html += pathSegments[pathSegments.length - 1].Definition || '';
                                    return html;
                                }
                            },
                            {
                                targets: 3,
                                render: function(element) {
                                    var html = '<a class="standard-a" ui-sref="';
                                    html += 'elementDetail.info({ dataStandardId: \'';
                                    html += dataStandardElementListViewModel.id;
                                    html += '\', current: \'';
                                    html += element.SystemItemId;
                                    html += '\'})"';
                                    html += '">';
                                    html += element.Name;
                                    html += '</a><br />';
                                    html += element.Definition || '';
                                    return html;
                                }
                            },
                            {
                                targets: 4,
                                render: function(element) {
                                    return element.ItemTypeName;
                                }
                            }
                        ]
                    });

                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, dataStandardElementListViewModel);
                });
        }

        $('#elementListTable').on('order.dt', function() {
            if (!dataStandardElementListViewModel.needToFilterFromSession) {
                var table = $(this).DataTable();
                $scope.orderBy = table.order();
                dataStandardElementListViewModel.saveFilterToSession();
            }
        });

        $('#elementListTable').on('length.dt', function(e, settings, len) {
            $scope.pageSize = len;
            dataStandardElementListViewModel.saveFilterToSession();
        });

        $('#elementListTable').on('page.dt', function() {
            if (!dataStandardElementListViewModel.needToFilterFromSession) {
                var table = $(this).DataTable();
                $scope.pageNo = table.page.info().page;
                dataStandardElementListViewModel.saveFilterToSession();
            }
        });

        $('#elementListTable').on('draw.dt', function() {
            angular.element(document.querySelector('input[type=search]')).unbind('keyup').keyup(function() {
                $scope.globalSearch = angular.element(this).val();
                dataStandardElementListViewModel.saveFilterToSession();
            });

            $('[data-toggle="popover"]').popover();

            var table = $('#elementListTable').DataTable();

            if (dataStandardElementListViewModel.needToFilterFromSession) {
                dataStandardElementListViewModel.needToFilterFromSession = false;
                angular.element(document.querySelector('input[type=search]')).val($scope.globalSearch);
                table.page.len($scope.pageSize);
                if ($scope.orderBy.length > 0)
                    table.order($scope.orderBy);
                dataStandardElementListViewModel.applyFilter();
                if (table.page($scope.pageNo))
                    table.page($scope.pageNo).draw(false);
            }

            var ids = [];
            table.rows({ search: 'applied' }).data().each(function(item) {
                ids.push({ ElementId: item.Element.SystemItemId });
            });
            sessionService.cloneToSession('elementLists', dataStandardElementListViewModel.id, ids);
        });

        dataStandardElementListViewModel.applyFilter = function() {

            var table = $('#elementListTable').DataTable();
            var elementGroupSearch = '';
            if ($scope.elementGroups)
                for (var key in $scope.elementGroups) {
                    if ($scope.elementGroups[key]) {
                        $scope.allElementGroups = false;
                        elementGroupSearch += (elementGroupSearch.length > 0 ? '|' : '') + key;
                    }
                }
            table.columns(0).search(elementGroupSearch, (elementGroupSearch.indexOf('|') > -1), false);

            if (elementGroupSearch.length == 0)
                $scope.allElementGroups = true;

            table.search($scope.globalSearch);

            var itemTypeSearch = '';
            if ($scope.itemTypes)
                for (var key in $scope.itemTypes) {
                    if ($scope.itemTypes[key]) {
                        $scope.allItemTypes = false;
                        itemTypeSearch += (itemTypeSearch.length > 0 ? '|' : '') + key;
                    }
                }
            table.columns(4).search(itemTypeSearch, (itemTypeSearch.indexOf('|') > -1), false);

            if (itemTypeSearch.length == 0)
                $scope.allItemTypes = true;

            table.draw();
            dataStandardElementListViewModel.saveFilterToSession();
        };

        dataStandardElementListViewModel.clearElementGroups = function() {
            if ($scope.allElementGroups) {
                for (var key in $scope.elementGroups)
                    $scope.elementGroups[key] = false;
            }
        };

        dataStandardElementListViewModel.clearItemTypes = function() {
            if ($scope.itemTypes) {
                for (var key in $scope.itemTypes)
                    $scope.itemTypes[key] = false;
            }
        };

        dataStandardElementListViewModel.getList();

        breadcrumbService.withCurrent();
    }
]);