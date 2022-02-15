// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appMappingProject').controller('mappingProjectReviewQueueController', [
    '$timeout', '$', '_', '$scope', '$compile', '$state', '$stateParams', 'mappingProjectReviewQueueService', 'handleErrorService', 'sessionService', 'breadcrumbService', '$location',
    function($timeout, $, _, $scope, $compile, $state, $stateParams, mappingProjectReviewQueueService, handleErrorService, sessionService, breadcrumbService, $location) {
        var mappingProjectReviewQueueViewModel = this;
        mappingProjectReviewQueueViewModel.loading = false;
        mappingProjectReviewQueueViewModel.id = $stateParams.id;

        $scope.$parent.pageTitle = 'ELEMENT QUEUE';

        $scope.allMethods = true;
        $scope.allStatuses = true;
        $scope.allElementGroups = true;
        $scope.elementGroups = {};
        $scope.allItemTypes = true;
        $scope.itemTypes = {};
        $scope.methods = {};
        $scope.statuses = {};
        $scope.flagged = false;
        $scope.globalSearch = '';
        $scope.pageSize = 10;
        $scope.pageNo = 0;
        $scope.orderBy = [];

        mappingProjectReviewQueueViewModel.saveFilterToSession = function() {
            sessionService.cloneToSession('queueFilters', mappingProjectReviewQueueViewModel.id, {
                elementGroups: $scope.elementGroups,
                itemTypes: $scope.itemTypes,
                methods: $scope.methods,
                statuses: $scope.statuses,
                flagged: $scope.flagged,
                globalSearch: $scope.globalSearch,
                pageSize: $scope.pageSize,
                pageNo: $scope.pageNo,
                orderBy: $scope.orderBy
            });
        };

        mappingProjectReviewQueueViewModel.filter = $stateParams.filter;
        if (mappingProjectReviewQueueViewModel.filter != undefined) {
            if (mappingProjectReviewQueueViewModel.filter.toLowerCase() == 'allincomplete') {
                $scope.statuses["1"] = true;
                $scope.statuses["Unmapped"] = true;
            } else if (mappingProjectReviewQueueViewModel.filter.toLowerCase() == 'unmapped') {
                $scope.statuses["Unmapped"] = true;
            } else if (mappingProjectReviewQueueViewModel.filter.toLowerCase() == 'flagged')
                $scope.flagged = true;

            else if ($.isNumeric(mappingProjectReviewQueueViewModel.filter)) {
                $scope.allStatuses = false;
                $scope.statuses[mappingProjectReviewQueueViewModel.filter] = true;
            } else if (mappingProjectReviewQueueViewModel.filter.match(/^[{]?[0-9a-fA-F]{8}[-]?([0-9a-fA-F]{4}[-]?){3}[0-9a-fA-F]{12}[}]?$/)) {
                $scope.allElementGroups = false;
                $scope.allStatuses = false;
                $scope.statuses["1"] = true;
                $scope.statuses["Unmapped"] = true;
                $scope.elementGroups[mappingProjectReviewQueueViewModel.filter] = true;
            }
            mappingProjectReviewQueueViewModel.saveFilterToSession();
            $location.search('filter', null);
            return;

        }

        mappingProjectReviewQueueViewModel.checkForFiltering = function() {
            mappingProjectReviewQueueViewModel.needToFilterFromSession = false;

            var queueFilters = sessionService.cloneFromSession('queueFilters', mappingProjectReviewQueueViewModel.id);

            if (queueFilters) {
                $scope.elementGroups = queueFilters.elementGroups;
                $scope.itemTypes = queueFilters.itemTypes;
                $scope.methods = queueFilters.methods;
                $scope.statuses = queueFilters.statuses;
                $scope.flagged = queueFilters.flagged;
                $scope.globalSearch = queueFilters.globalSearch;
                $scope.pageSize = queueFilters.pageSize;
                $scope.pageNo = queueFilters.pageNo;
                $scope.orderBy = queueFilters.orderBy;
                mappingProjectReviewQueueViewModel.needToFilterFromSession = true;
            }
        };

        mappingProjectReviewQueueViewModel.checkForFiltering();

        mappingProjectReviewQueueViewModel.getReviewQueue = function() {
            mappingProjectReviewQueueViewModel.loading = true;
            mappingProjectReviewQueueService.get(mappingProjectReviewQueueViewModel.id)
                .then(function(data) {
                    mappingProjectReviewQueueViewModel.reviewQueue = data;
                    mappingProjectReviewQueueViewModel.elementGroups = [];

                    for (var i = 0; i < mappingProjectReviewQueueViewModel.reviewQueue.ReviewItems.length; i++) {
                        var eg = mappingProjectReviewQueueViewModel.reviewQueue.ReviewItems[i];
                        var systemItemId = eg.PathSegments[0].SystemItemId;
                        var ids = mappingProjectReviewQueueViewModel.elementGroups.filter(function(val) {
                            return val.Id === systemItemId;
                        });
                        if (ids.length == 0) {
                            mappingProjectReviewQueueViewModel.elementGroups.push({ Id: systemItemId, DisplayText: eg.PathSegments[0].Name });
                        }
                    }

                    if ($.fn.dataTable.isDataTable('#reviewQueueTable')) {
                        var table = $('#reviewQueueTable').DataTable();
                        table.clear().rows.add(mappingProjectReviewQueueViewModel.reviewQueue.ReviewItems).draw();
                    } else {
                        $('#reviewQueueTable').DataTable(
                        {
                            data: mappingProjectReviewQueueViewModel.reviewQueue.ReviewItems,
                            rowId: 'Element.SystemItemId',
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
                                    data: 'Element',
                                    dataSort: [0]
                                },
                                {
                                    data: 'Mapping'
                                },
                                {
                                    data: 'Mapping'
                                },
                                {
                                    data: 'Mapping.Flagged',
                                },
                                {
                                    data: 'Element',
                                    visible: false,
                                    searchable: true
                                }
                            ],
                            columnDefs: [
                                {
                                    targets: 0,
                                    render: function(pathSegments, type, row) {
                                        var html = '<div class="hidden">';
                                        for (i = 0; i < pathSegments.length; i++) {
                                            html += pathSegments[i].Name;
                                            if (i < (pathSegments.length - 1))
                                                html += '.';
                                        }
                                        html += "</div>";
                                        return html;
                                    }
                                },
                                {
                                    targets: 1,
                                    render: function(element, type, row, meta) {
                                        var html = '<div class="hidden">';
                                        for (i = 0; i < row.PathSegments.length; i++) {
                                            html += row.PathSegments[i].Name;
                                            if (i < (row.PathSegments.length - 1))
                                                html += '.';
                                        }
                                        if (element)
                                            html += '.' + element.Name;
                                        html += "</div>";
                                        html += '<div class="expand-container">';
                                        html += '<ma-element-path context-id="mappingProjectReviewQueueViewModel.id"';
                                        html += ' segments="mappingProjectReviewQueueViewModel.reviewQueue.ReviewItems[';
                                        html += meta.row;
                                        html += '].PathSegments" element="mappingProjectReviewQueueViewModel.reviewQueue.ReviewItems[';
                                        html += meta.row;
                                        html += '].Element"></ma-element-path>';
                                        html += '<div class="expandable-div collapsed" data-ellipsis-id="ellipsis_';
                                        html += meta.row;
                                        html += '">';
                                        html += element.Definition == null ? '' : element.Definition;
                                        html += '</div>';
                                        html += '<div class="hidden">';
                                        html += row.PathSegments[0].SystemItemId;
                                        html += '</div>';
                                        html += '</div>';
                                        return html;
                                    }
                                },
                                {
                                    targets: 2,
                                    render: function(mapping, type, row, meta) {
                                        var html = '<div class="expand-container">';
                                        html += '<div class="expandable-div collapsed" data-ellipsis-id="ellipsis_';
                                        html += meta.row;
                                        html += '">';
                                        var display = mapping != null ? _.find(Application.Enumerations.MappingMethodType, function(x) {
                                            return x.Id == mapping.MappingMethodTypeId;
                                        }).DisplayText : '';
                                        html += '<div class="hidden">';
                                        html += display;
                                        html += '</div>';
                                        html += '<ma-mapping-detail mapping="mappingProjectReviewQueueViewModel.reviewQueue.ReviewItems[';
                                        html += meta.row;
                                        html += '].Mapping"></ma-mapping-detail>';
                                        html += '</div>';
                                        html += '<div><a role="button"><i class="fa fa-ellipsis-h hidden pull-right" id="ellipsis_';
                                        html += meta.row;
                                        html += '" title="Expand"></i></a></div></div>';
                                        return html;
                                    }
                                },
                                {
                                    targets: 3,
                                    render: function(mapping, type, row, meta) {
                                        var html = '<ma-workflow-status mapping="mappingProjectReviewQueueViewModel.reviewQueue.ReviewItems[';
                                        html += meta.row;
                                        html += '].Mapping" save-success="mappingProjectReviewQueueViewModel.saveSuccess" mode="list"></ma-workflow-status>';
                                        html += '<div class="hidden">';
                                        html += mapping != null ? mapping.WorkflowStatusTypeId : 'Unmapped';
                                        html += '</div>';
                                        return html;
                                    }
                                },
                                {
                                    targets: 4,
                                    render: function(flagged, type, row) {
                                        var html = '<a title="Edit" class="btn btn-edit" href="';
                                        html += $state.href('elementDetail.mapping', {
                                            mappingProjectId: mappingProjectReviewQueueViewModel.id,
                                            current: row.Element.SystemItemId
                                        });
                                        html += '"><i class="fa"></i></a>';
                                        html += flagged != undefined && flagged ? '<div class="hidden">true</div>' : '';
                                        return html;
                                    }
                                },
                                {
                                    targets: 5,
                                    render: function(element) {
                                        return element.ItemTypeName;
                                    }
                                }
                            ]
                        });
                    }
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, mappingProjectReviewQueueViewModel);
                })
                .finally(function() {
                    mappingProjectReviewQueueViewModel.loading = false;
                });
        }

        mappingProjectReviewQueueViewModel.workflowEnum = Application.Enumerations.WorkflowStatusType;
        mappingProjectReviewQueueViewModel.mappingMethodEnum = Application.Enumerations.MappingMethodType;
        mappingProjectReviewQueueViewModel.getReviewQueue();

        $('#reviewQueueTable').on('order.dt', function() {
            if (!mappingProjectReviewQueueViewModel.needToFilterFromSession) {
                var table = $(this).DataTable();
                $scope.orderBy = table.order();
                mappingProjectReviewQueueViewModel.saveFilterToSession();
            }
            if (mappingProjectReviewQueueViewModel.queueDirty)
                mappingProjectReviewQueueViewModel.refreshQueue();
        });

        $('#reviewQueueTable').on('length.dt', function(e, settings, len) {
            $scope.pageSize = len;
            mappingProjectReviewQueueViewModel.saveFilterToSession();
            if (mappingProjectReviewQueueViewModel.queueDirty)
                mappingProjectReviewQueueViewModel.refreshQueue();
        });

        $('#reviewQueueTable').on('page.dt', function() {
            if (!mappingProjectReviewQueueViewModel.needToFilterFromSession) {
                var table = $(this).DataTable();
                $scope.pageNo = table.page.info().page;
                mappingProjectReviewQueueViewModel.saveFilterToSession();
            }
            if (mappingProjectReviewQueueViewModel.queueDirty)
                mappingProjectReviewQueueViewModel.refreshQueue();
        });

        $('#reviewQueueTable').on('draw.dt', function() {
            angular.element(document.querySelector('input[type=search]')).unbind('keyup').keyup(function() {
                $scope.globalSearch = angular.element(this).val();
                mappingProjectReviewQueueViewModel.saveFilterToSession();
            });

            angular.element(document.querySelector('input[type=search]')).unbind('focus').focus(function() {
                if (mappingProjectReviewQueueViewModel.queueDirty)
                    mappingProjectReviewQueueViewModel.refreshQueue();
            });

            if ($('.dataTables_empty').length > 0)
                return;
            $('[data-toggle="popover"]').popover();
            var source = null;
            var target = null;
            $('tr').find('div.expandable-div').each(function(index, element) {
                var ellipsis = $('#' + $(element).attr('data-ellipsis-id')).first();
                if (index % 2 == 0) {
                    source = element;
                } else {
                    target = element;
                }
                if ((element.parentElement.offsetHeight < element.scrollHeight)) {
                    ellipsis.removeClass('hidden');
                    var row = $(element).closest('tr');
                    var expandContainers = row.find('div.expand-container');
                    var expandableDivs = expandContainers.find('div.expandable-div');
                    expandableDivs.css("cursor", "pointer");
                    expandableDivs.add(ellipsis).unbind().click(function() {
                        if (ellipsis.css("display") != 'none') {
                            ellipsis.toggle();
                            expandableDivs.toggleClass('collapsed', 300);
                        } else {
                            $timeout(function() { ellipsis.toggle(); }, 350);
                            expandableDivs.toggleClass('collapsed', 300);
                        }
                    });
                }
                if (ellipsis.hasClass('hidden')) {
                    if (index % 2 == 1) {
                        angular.element(source).removeClass('collapsed').removeClass('expandable-div');
                        angular.element(target).removeClass('collapsed').removeClass('expandable-div');
                    }
                }
            });

            var table = $('#reviewQueueTable').DataTable();

            if (mappingProjectReviewQueueViewModel.needToFilterFromSession) {
                mappingProjectReviewQueueViewModel.needToFilterFromSession = false;
                angular.element(document.querySelector('input[type=search]')).val($scope.globalSearch);
                table.page.len($scope.pageSize);
                if ($scope.orderBy.length > 0)
                    table.order($scope.orderBy);
                mappingProjectReviewQueueViewModel.applyFilter();
                if (table.page($scope.pageNo))
                    table.page($scope.pageNo).draw(false);
            }

            var ids = [];
            table.rows({ search: 'applied' }).data().each(function(item) {
                ids.push({ ElementId: item.SystemItemId });
            });
            sessionService.cloneToSession('elementQueues', mappingProjectReviewQueueViewModel.id, ids);
        });

        mappingProjectReviewQueueViewModel.applyFilter = function() {
            var table = $('#reviewQueueTable').DataTable();
            var elementGroupSearch = '';
            if ($scope.elementGroups) {
                for (var groupKey in $scope.elementGroups) {
                    if ($scope.elementGroups[groupKey]) {
                        $scope.allElementGroups = false;
                        elementGroupSearch += (elementGroupSearch.length > 0 ? '|' : '') + groupKey;
                    }
                }
            }

            table.columns(1).search(elementGroupSearch, (elementGroupSearch.indexOf('|') > -1), false);

            if (elementGroupSearch.length == 0)
                $scope.allElementGroups = true;

            var methodSearch = '';
            if ($scope.methods != null) {
                for (var methodKey in $scope.methods) {
                    if ($scope.methods[methodKey]) {
                        $scope.allMethods = false;
                        methodSearch += (methodSearch.length > 0 ? '|' : '') + methodKey;
                    }
                }
            }

            table.columns(2).search(methodSearch, (methodSearch.indexOf('|') > -1), false);

            if (methodSearch.length == 0)
                $scope.allMethods = true;

            var statusSearch = '';
            if ($scope.statuses != null) {
                for (var statusKey in $scope.statuses) {
                    if ($scope.statuses[statusKey]) {
                        $scope.allStatuses = false;
                        statusSearch += (statusSearch.length > 0 ? '|' : '') + statusKey;
                    }
                }
            }

            table.columns(3).search(statusSearch, (statusSearch.indexOf('|') > -1), false);

            if (statusSearch.length == 0)
                $scope.allStatuses = true;

            if ($scope.flagged)
                table.columns(4).search('true');
            else
                table.columns(4).search('');

            table.search($scope.globalSearch);

            var itemTypeSearch = '';
            if ($scope.itemTypes) {
                for (var key in $scope.itemTypes) {
                    if ($scope.itemTypes[key]) {
                        $scope.allItemTypes = false;
                        itemTypeSearch += (itemTypeSearch.length > 0 ? '|' : '') + key;
                    }
                }
            }

            table.columns(5).search(itemTypeSearch, (itemTypeSearch.indexOf('|') > -1), false);

            if (itemTypeSearch.length == 0)
                $scope.allItemTypes = true;

            table.draw();
            mappingProjectReviewQueueViewModel.saveFilterToSession();
        };

        mappingProjectReviewQueueViewModel.clearElementGroups = function() {
            if ($scope.allElementGroups) {
                for (var key in $scope.elementGroups)
                    $scope.elementGroups[key] = false;
            }
        };


        mappingProjectReviewQueueViewModel.clearItemTypes = function() {
            if ($scope.itemTypes) {
                for (var key in $scope.itemTypes)
                    $scope.itemTypes[key] = false;
            }
        };

        mappingProjectReviewQueueViewModel.clearMethods = function() {
            if ($scope.allMethods) {
                for (var key in $scope.methods)
                    $scope.methods[key] = false;
            }
        };

        mappingProjectReviewQueueViewModel.clearStatuses = function() {
            if ($scope.allStatuses) {
                for (var key in $scope.statuses)
                    $scope.statuses[key] = false;
            }
        };

        $scope.$on('mapping-error', function(event, data) {
            handleErrorService.handleErrors(data, mappingProjectReviewQueueViewModel);
        });

        mappingProjectReviewQueueViewModel.refreshQueue = function() {
            mappingProjectReviewQueueViewModel.checkForFiltering();
            mappingProjectReviewQueueViewModel.getReviewQueue();
            mappingProjectReviewQueueViewModel.queueDirty = false;
        };

        mappingProjectReviewQueueViewModel.saveSuccess = function() {
            mappingProjectReviewQueueViewModel.queueDirty = true;
            $scope.$emit('workflow-status-updated');
        }

        breadcrumbService.withCurrent();
    }
]);