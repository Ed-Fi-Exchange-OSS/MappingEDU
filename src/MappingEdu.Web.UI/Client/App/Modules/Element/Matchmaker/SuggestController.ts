// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.element.matchmaker.suggest
//

var m = angular.module('app.modules.element.matchmaker.suggest', []);

m.directive('maMatchmakerSuggest', ['settings', (settings: ISystemSettings) => ({
    restrict: 'E',
    templateUrl: `${settings.moduleBaseUri}/element/matchmaker/suggestView.tpl.html`,
    scope: {
        standardId: '=',
        itemTypeId: '=',
        systemItemId: '=',
        selectedItems: '=',
        selectedMapping: '=',
        project: '=',
        elementGroups: '=',
        browseElement: '=',
        close: '&'
    },
    controller: 'app.modules.element.matchmaker.suggest',
    controllerAs: 'suggestViewModel'
})]);


// ****************************************************************************
// Controller app.modules.element.matchmaker.suggest
//

m.controller('app.modules.element.matchmaker.suggest', ['$stateParams', '$scope', 'repositories', 'services',
    function($stateParams, $scope, repositories: IRepositories, services: IServices) {

        services.logger.debug('Loaded controller app.modules.element.matchmaker.suggest.');

        var vm = this;
        vm.allElementGroups = true;
        vm.allItems = true;

        if (!$scope.selectedItems)
            $scope.selectedItems = [];

        repositories.dataStandard.isExtended($scope.standardId).then(data => {
            vm.isExtended = data;
        });

        vm.refresh = () => {
            services.session.cloneToSession('MatchmakerSuggestResults', $scope.project.MappingProjectId, null);
            vm.suggestions = {};
            vm.collectingResults = true;
            repositories.element.matchmaker.suggest($scope.project.MappingProjectId, $scope.systemItemId).then(data => {
                vm.suggestions = data;

                angular.forEach(vm.suggestions.ElementSuggestions, suggestion => {
                    suggestion.Selected = 'false';
                    suggestion.FilterExtension = suggestion.IsExtended ? 'true' : 'false';
                });

                services.session.cloneToSession('MatchmakerSuggestResults', $scope.project.MappingProjectId, {
                    SystemItemId: $scope.systemItemId,
                    Results: data
                });

                vm.elementTable.rows().invalidate().draw();

            }, error => {
                services.logger.error('Error collecting suggestions.', error);
            }).finally(() => {
                vm.collectingResults = false;
            });  
        }

        vm.collectResults = () => {
            var filter = services.session.cloneFromSession('MatchmakerSuggestResults', $scope.project.MappingProjectId);
            if (filter && filter.SystemItemId === $scope.systemItemId) {

                vm.suggestions = filter.Results;
                angular.forEach(vm.suggestions.ElementSuggestions, suggestion => {
                    suggestion.Selected = 'false';
                    suggestion.FilterExtension = suggestion.IsExtended ? 'true' : 'false';
                });

                angular.forEach(vm.suggestions.MappingSuggestions, (suggestion, index) => {
                    suggestion.Selected = 'false';
                    suggestion.Collapsed = true;
                    suggestion.row = index;
                    suggestion.ShowMore = true;
                });

                if (vm.suggestions.ElementSuggestions.length > 0) services.timeout(() => { vm.loadTables() }, 1);

            } else {
                vm.collectingResults = true;
                repositories.element.matchmaker.suggest($scope.project.MappingProjectId, $scope.systemItemId).then(data => {
                    vm.suggestions = data;

                    angular.forEach(vm.suggestions.ElementSuggestions, suggestion => {
                        suggestion.Selected = 'false';
                        suggestion.FilterExtension = suggestion.IsExtended ? 'true' : 'false';
                    });

                    angular.forEach(vm.suggestions.MappingSuggestions, (suggestion, index) => {
                        suggestion.Selected = 'false';
                        suggestion.Collapsed = true;
                        suggestion.row = index;
                        suggestion.ShowMore = true;
                    });

                    services.session.cloneToSession('MatchmakerSuggestResults', $scope.project.MappingProjectId, {
                        SystemItemId: $scope.systemItemId,
                        Results: data
                    });

                    if (vm.suggestions.ElementSuggestions.length > 0) services.timeout(() => { vm.loadTables() }, 1);
                }, error => {
                    services.logger.error('Error collecting suggestions.', error);
                }).finally(() => {
                    vm.collectingResults = false;
                });   
            }
        }

        vm.collectResults();

        vm.viewSuggestedElement = (element) => {
            var html = '<div style="width: 500px; overflow-wrap: break-word;" data-toggle="popover" data-trigger="hover click" data-content="';
            html += element.DomainItemPath.split('.').join(' ') + '"><b>';
            if (element.IsExtended) html += ' <i class="fa fa-extended"></i> ';
            if (element.DomainItemPath.split('.').length > 4) {
                var split = element.DomainItemPath.split('.');
                html += split[0] + '.' + split[1] + '..' + split[split.length - 2] + '.' + split[split.length - 1];
            } else html += element.DomainItemPath;
            html += '</b>';
            html += (element.Definition ? '<br \>' + element.Definition : '') + '</div>';
            return html;
        }

        var elementTable = <any>$('#elementSuggestionsTable');
        var mappingTable = <any>$('#mappingSuggestionsTable');
        vm.loadTables = () => {
            vm.elementTable = elementTable.DataTable({
                data: vm.suggestions.ElementSuggestions,
                deferRender: true,
                language: {
                    search: "Filter:",
                    searchPlaceholder: "Filter Results"
                },
                createdRow(row) {
                    services.compile(angular.element(row).contents())($scope);
                },
                columns: [
                    {
                        data: 'DomainId',
                        visible: false
                    },
                    {
                        data: 'Selected',
                        visible: false
                    },
                    {
                        data: 'FilterExtension',
                        visible: false
                    },
                    {
                        data: null
                    },
                    {
                        data: 'Reasons'
                    },
                    {
                        data: 'Percentage'
                    }
                ],
                order: [[5, 'desc']],
                columnDefs: [
                    {
                        targets: 3,
                        render(element, type, row) {
                            return vm.viewSuggestedElement(element);
                        }
                    },
                    {
                        targets: 4,
                        render(reasons, type, row) {
                            var html = '';
                            angular.forEach(reasons, (reason, index) => {

                                if (reason.indexOf('-') !== 0) html += `<b>${reason}</b>`;
                                else html += reason;

                                html += '<br/>';
                            });
                            return html;
                        }
                    },
                    {
                        targets: 5,
                        render(percentage, type, row) {
                            var html = '<div style="vertical-align: middle"><uib-progressbar animate="true" value="';
                            html += percentage;
                            html += '" type="';
                            html += (percentage > 75) ? 'success' : ((percentage > 50) ? 'info' : ((percentage > 25) ? 'warning' : 'danger'));
                            html += '" style="margin: 0px"><b>' + percentage + '%</b></uib-progressbar></div>';
                            return html;
                        }
                    }
                ]
            });
            $('#elementSuggestionsTable_filter input').css('width', '300px');

            vm.mappingTable = mappingTable.DataTable({
                data: vm.suggestions.MappingSuggestions,
                deferRender: true,
                language: {
                    search: "Filter:",
                    searchPlaceholder: "Filter Results"
                },
                createdRow(row) {
                    services.compile(angular.element(row).contents())($scope);
                },
                columns: [
                    {
                        data: 'TargetSuggestItems'
                    },
                    {
                        data: 'Reasons'
                    },
                    {
                       
                    }
                ],
                order: [[0, 'desc']],
                columnDefs: [
                    {
                        targets: 0,
                        render(targetElements, type, row, meta) {

                            var html = '<div ';
                            if (row.ShowMore) {
                                html += 'class="expandable" ';
                                if (!row.Collapsed) html += `style="max-height: ${row.MaxRowHeight}px; height: ${row.MaxRowHeight}px;" `;
                            }
                            html += '>';

                            if (row.MappingMethodTypeId === 3) html += 'Mark for Extension';
                            else if (row.MappingMethodTypeId === 4) html += 'Mark for Omission';
                            else if (targetElements.length) {
                                for (var i = 0; i < targetElements.length; i++) {
                                    html += vm.viewSuggestedElement(targetElements[i]);

                                    if (i < targetElements.length - 1) html += '<br/>';
                                }

                            }
                            else html += 'No Elements';

                            html += '</div>';

                            if (row.ShowMore) {
                                html += `<div class="see-more text-center" ng-click="suggestViewModel.toggleRow(suggestViewModel.suggestions.MappingSuggestions[${row.row}])" `;
                                html += `style="width: ${row.RowWidth}px;">SEE {{suggestViewModel.suggestions.MappingSuggestions[${row.row}].Collapsed ? 'MORE' : 'LESS'}}</div>`;   
                            }
                            return html;
                        }
                    },
                    {
                        targets: 1,
                        render(reasons, type, row) {
                            var html = '<div ';
                            if (row.ShowMore) {
                                html += 'class="expandable" ';
                                if (!row.Collapsed) html += `style="max-height: ${row.MaxRowHeight}px; height: ${row.MaxRowHeight}px;" `;
                            }

                            html += '>';
                            angular.forEach(reasons, (reason, index) => {

                                if (reason.indexOf('-') !== 0) html += `<b>${reason}</b>`;
                                else html += reason;

                                html += '<br/>';
                            });
                            html += '</div>';
                            return html;
                        }
                    },
                    {
                        targets: 2,
                        render(model, type, row) {
                            var html = '<div ';
                            if (row.ShowMore) {
                                html += 'class="expandable" ';
                                if (!row.Collapsed) html += `style="max-height: ${row.MaxRowHeight}px; height: ${row.MaxRowHeight}px;" `;
                            }
                            html += '>';

                            if (row.MappingMethodTypeId === 3) html += 'Mark for Extension';
                            else if (row.MappingMethodTypeId === 4) html += 'Mark for Omission';
                            else {

                                function styleStringLiterals(businessLogic, newValue) {
                                    // Matches string literals in double quotes
                                    var matches = businessLogic.match(/"(.*?)"/gm);
                                    services.underscore.each(services.underscore.uniq(matches), (item: string) => {
                                        newValue = newValue.replace(
                                            new RegExp(item, 'gm'),
                                            '<span class="business-logic-literal">' + item + '</span>');
                                    });

                                    return newValue;
                                }

                                function styleNumbers(businessLogic, newValue) {
                                    // Matches numbers
                                    var matches = businessLogic.match(/\d+/gm);
                                    services.underscore.each(services.underscore.uniq(matches), function (item) {
                                        newValue = newValue.replace(
                                            new RegExp('\\b' + item + '\\b', 'gm'),
                                            '<span class="business-logic-literal">' + item + '</span>');
                                    });

                                    return newValue;
                                }

                                function styleKeywords(businessLogic, newValue) {
                                    // Matches keyword true
                                    var matches = businessLogic.match(/(\btrue\b)|(\bfalse\b)|(\byes\b)|(\bno\b)|(\bexists\b)/gim);
                                    services.underscore.each(services.underscore.uniq(matches), function (item) {
                                        newValue = newValue.replace(
                                            new RegExp('\\b' + item + '\\b', 'gm'),
                                            '<span class="business-logic-keyword">' + item + '</span>');
                                    });

                                    return newValue;
                                }

                                if (row.BusinessLogic == null) row.BusinessLogic = '';

                                var businessLogic =
                                    row.BusinessLogic
                                        .replace(/\r\n/gm, '<br>')
                                        .replace(/\n/gm, '<br>')
                                        .replace(/\[/gm, '<span class="standard-b">[')
                                        .replace(/\]/gm, ']</span>')
                                        .replace(/\$\{/gm, '<span class="business-logic-replacement">${')
                                        .replace(/\}/gm, '}</span>');

                                businessLogic = styleStringLiterals(row.BusinessLogic, businessLogic);
                                businessLogic = styleKeywords(row.BusinessLogic, businessLogic);
                                businessLogic = styleNumbers(row.BusinessLogic, businessLogic);

                                html += businessLogic;
                            }

                            html += '</div>';
                            return html;
                        }
                    }
                ]
            });
            $('#mappingSuggestionsTable_filter input').css('width', '300px');

        }

        vm.toggleRow = suggestion => {

            suggestion.CollapseClick = true;

            var tr = suggestion.DOM;

            if (suggestion.Collapsed) {
                tr.find('.expandable').each((expandableIndex, expandableElement) => {
                    var expandable = $(expandableElement);
                    expandable.animate({ 'maxHeight': suggestion.MaxRowHeight, 'height': suggestion.MaxRowHeight  }, 400);
                });
            } else {
                tr.find('.expandable').each((expandableIndex, expandableElement) => {
                    var expandable = $(expandableElement);
                    expandable.animate({ 'maxHeight': 70 }, 400);
                });
            }

            suggestion.Collapsed = !suggestion.Collapsed;


        }

        var selectedRow;

        vm.toggleSelect = () => {
            var element = vm.elementTable.row(selectedRow).data();
            if (selectedRow.hasClass('active') || selectedRow.hasClass('active-extended')) {
                selectedRow.removeClass('active');
                selectedRow.removeClass('active-extended');
                var index = $scope.selectedItems.map(x => x.SystemItemId).indexOf(element.SystemItemId);
                $scope.selectedItems.splice(index, 1);
            } else {
                if (element.IsExtended) selectedRow.addClass('active-extended');
                else selectedRow.addClass('active');
                $scope.selectedItems.push(element);
            }
        };

        vm.viewInBrowse = () => {
            $scope.browseElement = null;
            services.timeout(() => {
                var element = vm.elementTable.row(selectedRow).data();
                element.ItemTypeId = $scope.itemTypeId;
                $scope.browseElement = element;
            }, 20);
        }

        elementTable.on('contextmenu', 'tr', function (e) {
            var tr = <any>$(this);
            if (tr[0]._DT_RowIndex >= 0) {
                e.preventDefault();
                selectedRow = tr;
                var menu = $('#suggest-context-menu');
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

        elementTable.on('draw.dt', function () {
            $(this).find('tr').each(function () {
                var tr = <any>$(this);
                if (tr[0]._DT_RowIndex >= 0) {
                    var element = vm.suggestions.ElementSuggestions[tr[0]._DT_RowIndex];
                    var found = services.underscore.find(<Array<any>>$scope.selectedItems, item => item.SystemItemId === element.SystemItemId);
                    if (found) {
                        if (element.IsExtended) tr.addClass('active-extended');
                        else tr.addClass('active');
                    }
                }
            });
        });


        elementTable.on('click', 'tr', function (e) {
            var tr = <any>$(this);
            if (tr[0]._DT_RowIndex >= 0) {
                var element = vm.suggestions.ElementSuggestions[tr[0]._DT_RowIndex];
                if (tr.hasClass('active') || tr.hasClass('active-extended')) {
                    element.Selected = 'false';
                    tr.removeClass('active');
                    tr.removeClass('active-extended');
                    var index = $scope.selectedItems.map(x => x.SystemItemId).indexOf(element.SystemItemId);
                    $scope.selectedItems.splice(index, 1);
                } else {
                    element.Selected = 'true';
                    if (element.IsExtended) tr.addClass('active-extended');
                    else tr.addClass('active');
                    $scope.selectedItems.push(element);
                }
                vm.elementTable.row(tr).data(element);
                services.compile(angular.element(tr).contents())($scope);
            }
        });

        elementTable.on('order.dt', (e, settings) => {
            if (!vm.elementTable) return;
            var order = vm.elementTable.order();
            var column = settings.aoColumns[order[0][0]];
            if (column.sTitle.length > 0)
                services.logging.add({
                    Source: 'suggestController',
                    Message: `${$scope.ProjectName} ( ${$scope.MappingProjectId} ) - Suggestion results ordered by ${column.sTitle} ${order[0][1]}`
                });
        });

        elementTable.on('page.dt', function () {
            if (!vm.elementTable) return;
            var info = vm.elementTable.page.info();
            services.logging.add({
                Source: 'suggestController',
                Message: `${$scope.ProjectName} ( ${$scope.MappingProjectId} ) - Suggestion results current page changed to page ${++info.page} of ${info.pages}`
            });
        });

        $(document).on('click', () => {
            var menu = $('#suggest-context-menu');
            menu.hide();
        });

        elementTable.on('dblclick', 'tr', function () {
            var tr = <any>$(this);
            if (tr[0]._DT_RowIndex >= 0) {
                var element = vm.elementTable.row(tr).data();
                $scope.selectedItems.splice(0);
                $scope.selectedItems.push(element);
                $scope.close();
            }
        });

        mappingTable.on('draw.dt', () => {

            if ($('.dataTables_empty').length > 0)
                return;

            mappingTable.find('tbody').find('tr').each((index, element) => {

                var tr = <any>$(element);

                var rowIndex = tr[0]._DT_RowIndex;

                vm.suggestions.MappingSuggestions[rowIndex].RowWidth = tr.width();
                vm.suggestions.MappingSuggestions[rowIndex].Expandables = [];

                var maxHeight = 0;

                tr.find('.expandable').each((expandableIndex, expandableElement) => {
                    var expandable = $(expandableElement);
                    var height = expandable[0].scrollHeight;
                    if (maxHeight < height) maxHeight = height;
                });

                vm.suggestions.MappingSuggestions[rowIndex].MaxRowHeight = maxHeight + 10;

                if (maxHeight <= 70) {
                    tr.find('.see-more').first().hide();
                    vm.suggestions.MappingSuggestions[rowIndex].ShowMore = false;

                    tr.find('.expandable').each((expandableIndex, expandableElement) => {
                        $(expandableElement).removeClass('expandable');
                    });
                }

                vm.suggestions.MappingSuggestions[rowIndex].DOM = tr;


                tr.find('.see-more').css('width', vm.suggestions.MappingSuggestions[rowIndex].RowWidth);

            });
            
        });

        mappingTable.on('click', 'tr', function (e) {

            var tr = <any>$(this);
            if (tr[0]._DT_RowIndex >= 0) {
                var mapping = vm.suggestions.MappingSuggestions[tr[0]._DT_RowIndex];

                if (mapping.CollapseClick) {
                    mapping.CollapseClick = false;
                    return;
                }

                if (tr.hasClass('active')) {
                    mapping.Selected = 'false';
                    tr.removeClass('active');
                    $scope.selectedMapping = null;
                } else {

                    mappingTable.find('tr').each(function () {
                        var tr = $(this);
                        tr.removeClass('active');
                    });

                    mapping.Selected = 'true';
                    tr.addClass('active');
                    $scope.selectedMapping = mapping;
                }

                vm.mappingTable.row(tr).data(mapping);
                services.timeout(() => { services.compile(angular.element(tr).contents())($scope); });
            }
        });




        vm.clearElementGroups = () => {
            vm.selectedElementGroups = {};
            vm.allElementGroups = true;
            vm.redraw();
        }


        vm.clearItems = () => {
            vm.selectedItems = {};
            vm.alIltems = true;
            vm.redraw();
        }

        vm.toggleSelectedView = () => {
            vm.showSelected = !vm.showSelected;
            vm.redraw();
        }

        vm.redraw = () => {
            var elementGroupSuggest = '';
            if (vm.selectedElementGroups) {
                for (var key in vm.selectedElementGroups) {
                    if (vm.selectedElementGroups[key]) {
                        elementGroupSuggest += (elementGroupSuggest.length > 0 ? '|' : '') + key;
                    }
                }
            }

            var itemsSearch = '';
            if (vm.selectedItems) {
                for (var key in vm.selectedItems) {
                    if (vm.selectedItems[key]) {
                        if (key == 'base') itemsSearch += (itemsSearch.length > 0 ? '|' : '') + 'false';
                        else if (key == 'extended') itemsSearch += (itemsSearch.length > 0 ? '|' : '') + 'true';
                    }
                }
            }

            if (vm.showSelected) {
                vm.elementTable.columns(0).search('', (''.indexOf('|') > -1), false);
                vm.elementTable.columns(1).search('true', ('true'.indexOf('|') > -1), false);
                vm.elementTable.columns(2).search('', (''.indexOf('|') > -1), false);
            } else {
                vm.elementTable.columns(0).search(elementGroupSuggest, (elementGroupSuggest.indexOf('|') > -1), false);
                vm.elementTable.columns(1).search('', (''.indexOf('|') > -1), false);
                vm.elementTable.columns(2).search(itemsSearch, (itemsSearch.indexOf('|') > -1), false);
            }

            vm.allElementGroups = elementGroupSuggest.length === 0;
            vm.allItems = itemsSearch.length === 0;

            vm.elementTable.draw();
        }
    }
]);
