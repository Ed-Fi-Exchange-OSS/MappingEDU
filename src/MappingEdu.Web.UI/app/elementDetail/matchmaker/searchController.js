// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appElementDetail').controller('searchController', [
    '_', '$timeout', '$scope', '$state', '$stateParams', '$q', 'searchService', 'handleErrorService', 'sessionService', 'loggingService',
    function(_, $timeout, $scope, $state, $stateParams, $q, searchService, handleErrorService, sessionService, loggingService) {
        var searchViewModel = this;
        searchViewModel.searchText = '';
        searchViewModel.allElementGroups = true;
        searchViewModel.isTargetSelected = false;
        searchViewModel.id = $scope.$parent.elementDetailMappingViewModel.id;
        $scope.elementGroups = {};
        $scope.globalSearch = '';

        $scope.$on('element-fetched', function(event, data) {
            searchViewModel.element = data;
            searchViewModel.clearFilterFromSession();
            searchViewModel.performSearch();
        });

        $scope.$watch('searchViewModel.searchText', function() {
            searchViewModel.performSearch();
        });

        searchViewModel.loadFromSession = function() {
            var currentElement = sessionService.cloneFromSession('elementDetail', searchViewModel.id);
            if (currentElement) {
                searchViewModel.element = currentElement;
            }

            // Using promises here because we need to give up the thread long enough for the browser to start 
            //  rendering the page before we call buildTable
            searchViewModel.loading = true;
            getCurrentTargetElements()
                .then(function(data) {
                    if (data) {
                        searchViewModel.searchResults = data;
                        buildTable();
                    } else {
                        searchViewModel.performSearch();
                    }
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, searchViewModel);
                })
                .finally(function() {
                    searchViewModel.loading = false;
                });
        };

        function getCurrentTargetElements() {
            var deferred = $q.defer();

            $timeout(function() {
                var parentViewModel = $scope.$parent.elementDetailMappingViewModel;
                parentViewModel.targetElements = parentViewModel.targetElements || [];

                if (!searchViewModel.element) {
                    deferred.resolve(undefined);
                    return;
                }

                var itemTypeId = searchViewModel.element.ElementDetails.ItemType.Id;
                var currentTargetElements = parentViewModel.targetElements[itemTypeId];
                deferred.resolve(currentTargetElements);
            }, 100);

            return deferred.promise;
        }

        searchViewModel.performSearch = _.debounce(function() {
            if (!searchViewModel.element || !$scope.$parent.$parent || searchViewModel.searchResults) {
                setupStuff();
                return;
            }

            searchViewModel.isTargetSelected = false;
            searchViewModel.loading = true;
            var itemTypeId = searchViewModel.element.ElementDetails.ItemType.Id;
            var targetDataStandardId = $scope.$parent.$parent.elementDetailViewModel.mappingProject.TargetDataStandardId;
            searchService.get(searchViewModel.searchText, itemTypeId, targetDataStandardId)
                .then(function(data) {
                    searchViewModel.searchResults = data;
                    $scope.$parent.elementDetailMappingViewModel.targetElements[itemTypeId] = data;
                    buildTable();
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, searchViewModel);
                })
                .finally(function() {
                    searchViewModel.loading = false;
                });
        }, 100);

        function buildTable() {
            searchViewModel.elementGroups = _.map(
                _.uniq(searchViewModel.searchResults, function(item) {
                    return item.DomainName;
                }), function(item) {
                    return { Id: item.DomainId, DisplayText: item.DomainName };
                });

            if ($.fn.dataTable.isDataTable('#matchmakerSearchTable')) {
                var table = $('#matchmakerSearchTable').DataTable();
                table.clear().rows.add(searchViewModel.searchResults).draw();
            } else {
                $('#matchmakerSearchTable').DataTable({
                    data: searchViewModel.searchResults,
                    deferRender: true,
                    columns: [
                        {
                            data: 'DomainId',
                            visible: false
                        },
                        {
                            data: null,
                        },
                        {
                            data: 'ItemType.Name',
                        },
                        {
                            data: 'ItemDataType.Name',
                        },
                        {
                            data: 'FieldLength',
                        }
                    ],
                    columnDefs: [
                        {
                            targets: 1,
                            render: function(element, type, row) {
                                return '<div style="width: 500px; overflow-wrap: break-word;" data-toggle="popover" data-trigger="hover click" data-content="' +
                                    element.SpacedItemPath + '"><b>' +
                                    element.ShortItemPath + '</b>' +
                                    (element.Definition ? '<br \>' + element.Definition : '') + '</div>';
                            }
                        }
                    ]
                });
            }

            var input = $('#matchmakerSearchTable_filter > label > input[type=search]');
            input.closest('div.col-sm-6').removeClass('col-sm-6').addClass('col-sm-9').siblings('div.col-sm-6').removeClass('col-sm-6').addClass('col-sm-3');
            input.after('<br><span class="small bold" style="margin-left: 65px;"><em>(Tip: Enter multiple search terms separated by spaces.)</em></span>');

            setupStuff();

            var eventTable = $('#matchmakerSearchTable');
            var dataTable = eventTable.DataTable();
            eventTable.off('click');
            eventTable.on('click', 'tr', function() {
                if ($(this).hasClass('active')) {
                    $(this).removeClass('active');
                } else {
                    $(this).addClass('active');
                }
                var selectedData = dataTable.rows('.active').data();
                $scope.$apply(function() {
                    searchViewModel.isTargetSelected = selectedData.length > 0;
                });
            });
            eventTable.off('draw.dt');
            eventTable.on('draw.dt', function() {
                $('input[type=search]').unbind('keyup').keyup(function() {
                    $scope.globalSearch = $(this).val();
                    searchViewModel.saveFilterToSession();
                });

                setupStuff();
                clearSelection(dataTable);
            });
            eventTable.off('order.dt');
            eventTable.on('order.dt', function (e, settings) {
                var table = $('#matchmakerSearchTable').DataTable();
                var order = table.order();
                var column = settings.aoColumns[order[0][0]];
                if(column.sTitle.length > 0)
                    loggingService.add({ Source: 'searchController', Message: 'Search results ordered by ' + column.sTitle + ' ' + order[0][1] });
            });
            eventTable.off('page.dt');
            eventTable.on('page.dt', function () {
                var table = $('#matchmakerSearchTable').DataTable();
                var info = table.page.info();
                loggingService.add({ Source: 'searchController', Message: 'Search results current page changed to page ' + (++info.page) + ' of ' + info.pages  });
            });
        }

        function setupStuff() {
            $('[data-toggle="popover"]').popover();
            if (searchViewModel.needToFilterFromSession) {
                searchViewModel.needToFilterFromSession = false;
                searchViewModel.applyFilter();
            }
        }

        function logSearchResults(table) {
            var searchText = table.search();
            if (searchText.length > 0) {
                return loggingService.add({ Source: 'searchController', Message: 'User searched for: ' + searchText }).then(function () {
                    var info = table.page.info();
                    var rowCount = info.recordsDisplay;
                    var recordCount = info.recordsTotal;
                    loggingService.add({ Source: 'searchController', Message: 'Search filtered results to ' + rowCount + ' rows from ' + recordCount + ' total.' });
                });
            }
            return loggingService.add({ Source: 'searchController', Message: 'No search filter was supplied' });
        }

        searchViewModel.close = function () {
            var table = $('#matchmakerSearchTable').DataTable();
            logSearchResults(table).then(function () {
                loggingService.add({ Source: 'searchController', Message: 'User cancelled the matchmaker from search tab' });
            });
            $('#matchmakerModal').modal('hide');
        };

        searchViewModel.select = function() {
            var table = $('#matchmakerSearchTable').DataTable();
            var selectedData = table.rows('.active').data();
            var selectedRows = table.rows('.active').indexes();
            _.each(selectedRows, function(selectedRow, index) {
                selectedRows[index] = ++selectedRow;
            });
            var isTargetSelected = selectedData.length > 0;

            if (!isTargetSelected)
                return;

            for (var j = 0; j < selectedData.length; j++) {
                updateBusinessLogic(selectedData[j]);
            }
            logSearchResults(table).then(function () {
                loggingService.add({ Source: 'searchController', Message: 'User selected the following element(s): ' + _.pluck(selectedData, 'DomainItemPath') });
                loggingService.add({ Source: 'searchController', Message: 'User selected the row(s) at the following position(s): ' + selectedRows.join(',') });
            });
            clearSelection(table);
            $('#matchmakerModal').modal('hide');
        };

        function updateBusinessLogic(data) {
            var parentViewModel = $scope.$parent.elementDetailMappingViewModel;
            if (_.isUndefined(parentViewModel.mapping)) {
                parentViewModel.mapping = {};
                parentViewModel.mapping.BusinessLogic =
                    angularApp.Utils.appendPathToEndOfBusinessLogic('', data.DomainItemPath);
                return;
            }

            var blTextArea = angular.element('#businessLogic');
            var businessLogic = parentViewModel.mapping.BusinessLogic;
            if (!_.isUndefined(blTextArea[0])) {
                var selEndPosition = angularApp.Utils.getCaretPosition(blTextArea[0]);
                if (selEndPosition >= 0) {
                    parentViewModel.mapping.BusinessLogic =
                        angularApp.Utils.insertPathIntoBusinessLogic(businessLogic, data.DomainItemPath, selEndPosition);
                    return;
                }
            }

            parentViewModel.mapping.BusinessLogic =
                angularApp.Utils.appendPathToEndOfBusinessLogic(businessLogic, data.DomainItemPath);
        }

        function clearSelection(table) {
            table.$('tr.active').removeClass('active');
            searchViewModel.isTargetSelected = false;
        }

        searchViewModel.saveFilterToSession = function() {
            sessionService.cloneToSession('searchFilters', searchViewModel.id, {
                elementGroups: $scope.elementGroups,
                globalSearch: $scope.globalSearch,
            });
        };

        searchViewModel.clearFilterFromSession = function() {
            $scope.elementGroups = {};
            $scope.globalSearch = '';
            sessionService.clearSection('searchFilters');
            sessionService.clearSection('suggestFilters');
            searchViewModel.needToFilterFromSession = true;
        };

        searchViewModel.checkForFiltering = function() {
            searchViewModel.needToFilterFromSession = false;

            var searchFilters = sessionService.cloneFromSession('searchFilters', searchViewModel.id);

            if (searchFilters) {
                $scope.elementGroups = searchFilters.elementGroups;
                $scope.globalSearch = searchFilters.globalSearch;
                searchViewModel.needToFilterFromSession = true;
            }
        };

        searchViewModel.applyFilter = function() {
            searchViewModel.saveFilterToSession();
            var table = $('#matchmakerSearchTable').DataTable();
            var elementGroupSearch = '';
            if ($scope.elementGroups) {
                for (var key in $scope.elementGroups) {
                    if ($scope.elementGroups[key]) {
                        elementGroupSearch += (elementGroupSearch.length > 0 ? '|' : '') + key;
                    }
                }
            }

            table.columns(0).search(elementGroupSearch, (elementGroupSearch.indexOf('|') > -1), false);
            searchViewModel.allElementGroups = elementGroupSearch.length === 0;
            table.search($scope.globalSearch);
            table.draw();
        };

        searchViewModel.clearElementGroups = function() {
            if (searchViewModel.allElementGroups) {
                $scope.elementGroups = {};
            }
        };

        searchViewModel.checkForFiltering();
        searchViewModel.loadFromSession();
    }
]);
