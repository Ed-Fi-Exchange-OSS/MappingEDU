// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appElementDetail').controller('suggestController', [
    '_', '$timeout', '$scope', '$state', '$stateParams', 'suggestService', 'loggingService', 'handleErrorService', 'sessionService',
    function (_, $timeout, $scope, $state, $stateParams, suggestService, loggingService, handleErrorService, sessionService) {

        var suggestViewModel = this;
        suggestViewModel.allElementElementGroups = true;
        suggestViewModel.allMappingElementGroups = true;
        suggestViewModel.isTargetSelected = false;
        $scope.elementElementGroups = {};
        $scope.mappingElementGroups = {};
        $scope.elementGlobalSearch = '';
        $scope.mappingGlobalSearch = '';

        $scope.$on('element-fetched', function (event, data) {
            suggestViewModel.element = data;
            suggestViewModel.clearFilterFromSession();
            suggestViewModel.performSuggest();
        });

        suggestViewModel.loadFromSession = function () {
            var currentElement = sessionService.cloneFromSession('elementDetail', $scope.$parent.elementDetailMappingViewModel.id);
            if (!_.isUndefined(currentElement)) {
                suggestViewModel.element = currentElement;
                suggestViewModel.performSuggest();
            }
        };

        suggestViewModel.performSuggest = function () {
            if (null == $scope.$parent.$parent) {
                setupStuff();
                return;
            }

            suggestViewModel.loading = true;
            suggestService.get(
                    $scope.$parent.elementDetailMappingViewModel.element.ElementDetails.SystemItemId,
                    $scope.$parent.$parent.elementDetailViewModel.mappingProject.TargetDataStandardId
                )
                .then(function (data) {
                    suggestViewModel.suggestResults = data;
                    buildTables();
                })
                .catch(function (error) {
                    handleErrorService.handleErrors(error, suggestViewModel);
                })
                .finally(function () {
                    suggestViewModel.loading = false;
                });
        };

        function buildTables() {
            suggestViewModel.suggestMappings = _.filter(suggestViewModel.suggestResults, function (item) {
                return item.PreviousBusinessLogic || item.PreviousMappingMethod;
            });

            loggingService.add({ Source: 'suggestController', Message: 'Matchmaker returned ' + suggestViewModel.suggestMappings.length + ' suggested mappings.' });

            suggestViewModel.mappingElementGroups = _.map(
                _.uniq(suggestViewModel.suggestMappings, function (item) {
                    return item.DomainName;
                }), function (item) {
                    return { Id: item.DomainId, DisplayText: item.DomainName };
                });

            suggestViewModel.suggestElements = _.filter(suggestViewModel.suggestResults, function (item) {
                return !_.contains(suggestViewModel.suggestMappings, item);
            });

            loggingService.add({ Source: 'suggestController', Message: 'Matchmaker returned ' + suggestViewModel.suggestElements.length + ' suggested elements.' });

            suggestViewModel.elementElementGroups = _.map(
                _.uniq(suggestViewModel.suggestElements, function (item) {
                    return item.DomainName;
                }), function (item) {
                    return { Id: item.DomainId, DisplayText: item.DomainName };
                });

            if ($.fn.dataTable.isDataTable('#matchmakerSuggestMappingTable')) {
                var table = $('#matchmakerSuggestMappingTable').DataTable();
                table.clear().rows.add(suggestViewModel.suggestMappings).draw();
            } else {
                $('#matchmakerSuggestMappingTable').DataTable({
                    data: suggestViewModel.suggestMappings,
                    language: {
                        search: "Filter:",
                        searchPlaceholder: "Filter Results"
                    },
                    columns: [
                        {
                            data: 'DomainId',
                            visible: false
                        },
                        {
                            data: null
                        },
                        {
                            data: 'SuggestReason'
                        },
                        {
                            data: 'SuggestRank'
                        }
                    ],
                    columnDefs: [
                        {
                            targets: 1,
                            render: function (element, type, row) {
                                return '<div style="width: 500px; overflow-wrap: break-word;" data-toggle="popover" data-trigger="hover click" data-content="' +
                                    element.SpacedItemPath + '"><b>' +
                                    element.ShortItemPath + '</b>' +
                                    (element.Definition ? '<br \>' + element.Definition : '') + '</div>';
                            }
                        }
                    ]
                });
            }

            if ($.fn.dataTable.isDataTable('#matchmakerSuggestElementTable')) {
                var table = $('#matchmakerSuggestElementTable').DataTable();
                table.clear().rows.add(suggestViewModel.suggestElements).draw();
            } else {
                $('#matchmakerSuggestElementTable').DataTable({
                    data: suggestViewModel.suggestElements,
                    language: {
                        search: "Filter:",
                        searchPlaceholder: "Filter Results"
                    },
                    columns: [
                        {
                            data: 'DomainId',
                            visible: false
                        },
                        {
                            data: null
                        },
                        {
                            data: 'SuggestReason'
                        },
                        {
                            data: 'SuggestRank'
                        }
                    ],
                    columnDefs: [
                        {
                            targets: 1,
                            render: function (element, type, row) {
                                return '<div style="width: 500px; overflow-wrap: break-word;" data-toggle="popover" data-trigger="hover click" data-content="' +
                                    element.SpacedItemPath + '"><b>' +
                                    element.ShortItemPath + '</b>' +
                                    (element.Definition ? '<br \>' + element.Definition : '') + '</div>';
                            }
                        }
                    ]
                });
            }

            if ($('#searchHelpMappingTable').length == 0) {
                var input = $('#matchmakerSuggestMappingTable_filter > label > input[type=search]');
                input.closest('div.col-sm-6').removeClass('col-sm-6').addClass('col-sm-9').siblings('div.col-sm-6').removeClass('col-sm-6').addClass('col-sm-3');
                input.after('<br><span id="searchHelpMappingTable" class="small bold" style="margin-left: 65px;"><em>(Tip: Enter multiple filter terms separated by spaces.)</em></span>');
            }

            if ($('#searchHelpElementTable').length == 0) {
                var input2 = $('#matchmakerSuggestElementTable_filter > label > input[type=search]');
                input2.closest('div.col-sm-6').removeClass('col-sm-6').addClass('col-sm-9').siblings('div.col-sm-6').removeClass('col-sm-6').addClass('col-sm-3');
                input2.after('<br><span id="searchHelpElementTable" class="small bold" style="margin-left: 65px;"><em>(Tip: Enter multiple filter terms separated by spaces.)</em></span>');
            }

            setupStuff();

            var eventTableMapping = $('#matchmakerSuggestMappingTable');
            var dataTableMapping = eventTableMapping.DataTable();
            eventTableMapping.off('click');
            eventTableMapping.on('click', 'tr', function () {
                $('#matchmakerSuggestMappingTable > tbody > tr').removeClass('active');
                $(this).addClass('active');

                var selectedData = dataTableMapping.rows('.active').data();
                suggestViewModel.hasMapping = !_.isUndefined(_.find(selectedData, function (item) {
                    return item.PreviousMappingMethod != null;
                }));
                $scope.$apply(function () {
                    suggestViewModel.isMappingTargetSelected = selectedData.length > 0;
                });
            });
            eventTableMapping.off('draw.dt');
            eventTableMapping.on('draw.dt', function () {
                $('input[type=search]').unbind('keyup').keyup(function () {
                    $scope.globalSearch = $(this).val();
                    suggestViewModel.saveFilterToSession();
                });

                setupStuff();
                clearSelection(dataTableMapping);
            });
            eventTableMapping.off('order.dt');
            eventTableMapping.on('order.dt', function (e, settings) {
                var table = $('#matchmakerSuggestMappingTable').DataTable();
                var order = table.order();
                var column = settings.aoColumns[order[0][0]];
                if (column.sTitle.length > 0)
                    loggingService.add({ Source: 'suggestController', Message: 'Suggested Mappings results ordered by ' + column.sTitle + ' ' + order[0][1] });
            });
            eventTableMapping.off('page.dt');
            eventTableMapping.on('page.dt', function () {
                var table = $('#matchmakerSuggestMappingTable').DataTable();
                var info = table.page.info();
                loggingService.add({ Source: 'suggestController', Message: 'Suggested Mappings results current page changed to page ' + (++info.page) + ' of ' + info.pages });
            });

            var eventTableElement = $('#matchmakerSuggestElementTable');
            var dataTableElement = eventTableElement.DataTable();
            eventTableElement.off('click');
            eventTableElement.on('click', 'tr', function () {
                if ($(this).hasClass('active')) {
                    $(this).removeClass('active');
                } else {
                    $(this).addClass('active');
                }
                var selectedData = dataTableElement.rows('.active').data();
                suggestViewModel.hasMapping = false;
                $scope.$apply(function () {
                    suggestViewModel.isElementTargetSelected = selectedData.length > 0;
                });
            });
            eventTableElement.off('draw.dt');
            eventTableElement.on('draw.dt', function () {
                $('input[type=search]').unbind('keyup').keyup(function () {
                    $scope.globalSearch = $(this).val();
                    suggestViewModel.saveFilterToSession();
                });

                setupStuff();
                clearSelection(dataTableElement);
            });
            eventTableElement.off('order.dt');
            eventTableElement.on('order.dt', function (e, settings) {
                var table = $('#matchmakerSuggestElementTable').DataTable();
                var order = table.order();
                var column = settings.aoColumns[order[0][0]];
                if (column.sTitle.length > 0)
                    loggingService.add({ Source: 'suggestController', Message: 'Suggested Elements results ordered by ' + column.sTitle + ' ' + order[0][1] });
            });
            eventTableElement.off('page.dt');
            eventTableElement.on('page.dt', function () {
                var table = $('#matchmakerSuggestElementTable').DataTable();
                var info = table.page.info();
                loggingService.add({ Source: 'suggestController', Message: 'Suggested Elements results current page changed to page ' + (++info.page) + ' of ' + info.pages });
            });
        }

        function setupStuff() {
            $('[data-toggle="popover"]').popover();
            if (suggestViewModel.needToFilterFromSession) {
                suggestViewModel.needToFilterFromSession = false;
                suggestViewModel.applyMappingFilter();
                suggestViewModel.applyElementFilter();
            }
        }

        suggestViewModel.close = function() {
            logSearchResults($('#matchmakerSuggestMappingTable').DataTable())
                .then(function() {
                    logSearchResults($('#matchmakerSuggestElementTable').DataTable())
                        .then(function() {
                            loggingService.add({ Source: 'suggestController', Message: 'User cancelled the matchmaker from suggest tab' });
                        });
                });
            $('#matchmakerModal').modal('hide');
        }

        suggestViewModel.selectElement = function () {
            doSelect($('#matchmakerSuggestElementTable').DataTable(), true);
        };

        suggestViewModel.selectMapping = function () {
            doSelect($('#matchmakerSuggestMappingTable').DataTable(), false);
        };

        function doSelect(table, isPath) {
            var selectedData = table.rows('.active').data();
            var selectedRows = table.rows('.active').indexes();

            _.each(selectedRows, function (selectedRow, index) {
                selectedRows[index] = ++selectedRow;
                loggingService.add({ Source: 'suggestControoler', Message: 'Total records suggested: ' + table.page.info().recordsTotal + '. Position of record selected: ' + selectedRow + ', suggest reason: ' + selectedData[index].SuggestReason });
            });
            var isTargetSelected = selectedData.length > 0;

            if (!isTargetSelected)
                return;

            if (!isPath) {
                var hasBusinessLogic = false;
                var hasMappingMethod = false;
                for (var i = 0; i < selectedData.length; i++) {
                    if (null !== selectedData[i].PreviousBusinessLogic) {
                        hasBusinessLogic = true;
                        break;
                    }
                    if (null != selectedData[i].PreviousMappingMethod) {
                        hasMappingMethod = true;
                        break;
                    }
                }

                if (!hasBusinessLogic && !hasMappingMethod) return;
            }

            for (var j = 0; j < selectedData.length; j++) {
                updateMapping(selectedData[j], isPath);
            }

            table.$('tr.active').removeClass('active');
            suggestViewModel.isTargetSelected = false;
            logSearchResults(table).then(function() {
                loggingService.add({ Source: 'suggestController', Message: 'User selected the following element(s): ' + _.pluck(selectedData, 'DomainItemPath') });
                loggingService.add({ Source: 'suggestController', Message: 'User selected the row(s) at the following position(s): ' + selectedRows.join(',') });
            });
            $('#matchmakerModal').modal('hide');
        }

        function logSearchResults(table) {
            var searchText = table.search();
            if (searchText.length > 0) {
                return loggingService.add({ Source: 'suggestController', Message: 'User searched for: ' + searchText + ' in ' + table.settings()[0].sTableId }).then(function () {
                    var info = table.page.info();
                    var rowCount = info.recordsDisplay;
                    var recordCount = info.recordsTotal;
                    loggingService.add({ Source: 'suggestController', Message: 'Search filtered results to ' + rowCount + ' rows from ' + recordCount + ' total.' });
                });
            }
            return loggingService.add({ Source: 'suggestController', Message: 'No search filter was supplied for ' + table.settings()[0].sTableId });
        }

        function updateMapping(data, isPath) {
            var parentViewModel = $scope.$parent.elementDetailMappingViewModel;
            var blTextArea = angular.element('#businessLogic');

            if (_.isUndefined(parentViewModel.mapping)) {
                parentViewModel.mapping = {};
            }

            var businessLogic = parentViewModel.mapping.BusinessLogic || '';

            if (!isPath && data.PreviousMappingMethod) {
                parentViewModel.mapping.MappingMethodTypeId = data.PreviousMappingMethod.Id;
            }

            if (!isPath && data.PreviousMappingMethod.DisplayText === "Mark for Omission") {
                parentViewModel.mapping.OmissionReason = data.PreviousOmissionReason;
                parentViewModel.businessLogicChange();
                return;
            }

            if (!_.isUndefined(blTextArea[0])) {
                var selEndPosition = angularApp.Utils.getCaretPosition(blTextArea[0]);
                if (selEndPosition >= 0) {
                    parentViewModel.mapping.BusinessLogic =
                        isPath ?
                        angularApp.Utils.insertPathIntoBusinessLogic(businessLogic, data.DomainItemPath, selEndPosition) :
                        data.PreviousBusinessLogic;
                }
            } else {
                parentViewModel.mapping.BusinessLogic =
                    isPath ?
                    angularApp.Utils.appendPathToEndOfBusinessLogic(businessLogic, data.DomainItemPath) :
                    data.PreviousBusinessLogic;
            }
        }

        function clearSelection(table) {
            table.$('tr.active').removeClass('active');
            suggestViewModel.isTargetSelected = false;
        }

        suggestViewModel.saveFilterToSession = function () {
            sessionService.cloneToSession('suggestMappingFilters', suggestViewModel.id, {
                elementGroups: $scope.mappingElementGroups,
                globalSearch: $scope.mappingGlobalSearch,
            });
            sessionService.cloneToSession('suggestElementFilters', suggestViewModel.id, {
                elementGroups: $scope.elementElementGroups,
                globalSearch: $scope.elementGlobalSearch,
            });
        };

        suggestViewModel.clearFilterFromSession = function () {
            $scope.mappingElementGroups = {};
            $scope.elementElementGroups = {};
            $scope.mappingGlobalSearch = '';
            $scope.elementGlobalSearch = '';
            sessionService.clearSection('searchFilters');
            sessionService.clearSection('suggestMappingFilters');
            sessionService.clearSection('suggestElementFilters');
            suggestViewModel.needToFilterFromSession = true;
        };

        suggestViewModel.checkForFiltering = function () {
            suggestViewModel.needToFilterFromSession = false;

            var suggestMappingFilters = sessionService.cloneFromSession('suggestMappingFilters', suggestViewModel.id);
            var suggestElementFilters = sessionService.cloneFromSession('suggestElementFilters', suggestViewModel.id);

            if (suggestMappingFilters) {
                $scope.mappingElementGroups = suggestMappingFilters.elementGroups;
                $scope.mappingGlobalSearch = suggestMappingFilters.globalSearch;
                suggestViewModel.needToFilterFromSession = true;
            }

            if (suggestElementFilters) {
                $scope.elementElementGroups = suggestElementFilters.elementGroups;
                $scope.elementGlobalSearch = suggestElementFilters.globalSearch;
                suggestViewModel.needToFilterFromSession = true;
            }
        };

        suggestViewModel.applyMappingFilter = function () {
            suggestViewModel.saveFilterToSession();
            var table = $('#matchmakerSuggestMappingTable').DataTable();
            var elementGroupSuggest = '';
            if ($scope.mappingElementGroups) {
                for (var key in $scope.mappingElementGroups) {
                    if ($scope.mappingElementGroups[key]) {
                        elementGroupSuggest += (elementGroupSuggest.length > 0 ? '|' : '') + key;
                    }
                }
            }

            table.columns(0).search(elementGroupSuggest, (elementGroupSuggest.indexOf('|') > -1), false);
            suggestViewModel.allMappingElementGroups = elementGroupSuggest.length === 0;
            table.search($scope.mappingGlobalSearch);
            table.draw();
        };

        suggestViewModel.applyElementFilter = function () {
            suggestViewModel.saveFilterToSession();
            var table = $('#matchmakerSuggestElementTable').DataTable();
            var elementGroupSuggest = '';
            if ($scope.elementElementGroups) {
                for (var key in $scope.elementElementGroups) {
                    if ($scope.elementElementGroups[key]) {
                        elementGroupSuggest += (elementGroupSuggest.length > 0 ? '|' : '') + key;
                    }
                }
            }

            table.columns(0).search(elementGroupSuggest, (elementGroupSuggest.indexOf('|') > -1), false);
            suggestViewModel.allElementElementGroups = elementGroupSuggest.length === 0;
            table.search($scope.elementGlobalSearch);
            table.draw();
        };

        suggestViewModel.clearMappingElementGroups = function () {
            if (suggestViewModel.allMappingElementGroups) {
                $scope.mappingElementGroups = {};
            }
        };
        suggestViewModel.clearElementElementGroups = function () {
            if (suggestViewModel.allElementElementGroups) {
                $scope.elementElementGroups = {};
            }
        };

        suggestViewModel.checkForFiltering();
        suggestViewModel.loadFromSession();

        suggestViewModel.showSuggestedMappings = true;
        suggestViewModel.toggleSuggestedMappingsCaret = function () {
            suggestViewModel.showSuggestedMappings = !suggestViewModel.showSuggestedMappings;
        };

        suggestViewModel.showSuggestedElements = true;
        suggestViewModel.toggleSuggestedElementsCaret = function () {
            suggestViewModel.showSuggestedElements = !suggestViewModel.showSuggestedElements;
        };
    }
]);
