// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.data-standard.edit.elements
//

var m = angular.module('app.modules.data-standard.edit.elements', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.data-standard.edit.elements', {
            url: '/elements?filter&elementGroup',
            data: {
                title: 'Element List',
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Guest')].Id
            },
            templateUrl: `${settings.moduleBaseUri}/DataStandard/Edit/Elements/DataStandardElementsView.tpl.html`,
            controller: 'app.modules.data-standard.edit.elements',
            controllerAs: 'dataStandardElementListViewModel',
            resolve: {
                isExtended: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.dataStandard.isExtended($stateParams.dataStandardId);
                }],
                extensions: [() => {return null}]
            }
        })
        .state('app.data-standard.edit.extension-elements', {
            url: '/extension-elements?filter&elementGroup',
            data: {
                title: 'Extension Element List',
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Owner')].Id,
                extensionsPublicAccess: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Guest')].Id
            },
            templateUrl: `${settings.moduleBaseUri}/DataStandard/Edit/Elements/DataStandardExtensionElementsView.tpl.html`,
            controller: 'app.modules.data-standard.edit.elements',
            controllerAs: 'dataStandardElementListViewModel',
            resolve: {
                isExtended: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.dataStandard.isExtended($stateParams.dataStandardId);
                }],
                extensions: ['repositories', '$stateParams', (repostiories: IRepositories, $stateParams) => {
                    return repostiories.dataStandard.extensions.getAll($stateParams.dataStandardId);
                }]
            }
        });
}]);


m.run(['$rootScope', '$state', 'services', ($rootScope, $state, services: IServices) => {
    $rootScope.$on('$stateChangeStart', (event, toState, toParams, fromState, fromParams) => {
        if (toState.name === 'app.data-standard.edit.elements' || toState.name === 'app.data-standard.edit.extension-elements') {

            if (toParams.filter) {
                event.preventDefault();

                if (toState.name === 'app.data-standard.edit.extension-elements') {
                    services.session.cloneToSession('elementExtensionListFilters', toParams.dataStandardId, JSON.parse(toParams.filter));
                } else {
                    services.session.cloneToSession('elementListFilters', toParams.dataStandardId, JSON.parse(toParams.filter));
                }

                services.state.go(toState.name, {
                    dataStandardId: toParams.dataStandardId,
                    filter: null
                }, { location: 'replace' });

            } else if (toParams.elementGroup) {
                if (toParams.elementGroup.match(/^[{]?[0-9a-fA-F]{8}[-]?([0-9a-fA-F]{4}[-]?){3}[0-9a-fA-F]{12}[}]?$/)) {
                    event.preventDefault();
                    var oldFilter;

                    if (toState.name === 'app.data-standard.edit.extension-elements') {
                        oldFilter = services.session.cloneFromSession('elementExtensionListFilters', toParams.dataStandardId);
                    } else {
                        oldFilter = services.session.cloneFromSession('elementListFilters', toParams.dataStandardId);
                    }

                    var filter = {
                        elementGroups: {},
                        itemTypes: {},
                        items: {},
                        extensions: {},
                        globalSearch: '',
                        pageSize: oldFilter ? oldFilter.pageSize : 10,
                        pageNo: oldFilter ? oldFilter.pageNo : 0,
                        orderBy: oldFilter ? oldFilter.orderBy : { column: 1, dir: 'asc' }
                    }

                    filter.elementGroups[toParams.elementGroup] = true;

                    if (toState.name === 'app.data-standard.edit.extension-elements') {
                        services.session.cloneToSession('elementExtensionListFilters', toParams.dataStandardId, filter);
                    } else {
                        services.session.cloneToSession('elementListFilters', toParams.dataStandardId, filter);

                    }

                    services.state.go(toState.name, {
                        dataStandardId: toParams.dataStandardId,
                        elementGroup: null
                    });
                }
            }
        }
    });
}]);


// ****************************************************************************
// Controller app.modules.data-standard.edit.elements
//

m.controller('app.modules.data-standard.edit.elements', ['$scope', '$rootScope', '$stateParams', 'repositories', 'services', 'isExtended', 'extensions',
    function ($scope, $rootScope, $stateParams, repositories: IRepositories, services: IServices, isExtended, extensions) {

        services.logger.debug('Loaded app.modules.data-standard.edit.elements controller');
        $scope.$parent.dataStandardDetailViewModel.setTitle('ELEMENT LIST');

        var vm = this;
        vm.isExtensionTable = services.state.current.name === 'app.data-standard.edit.extension-elements';

        vm.id = $stateParams.dataStandardId;
        vm.isExtended = isExtended;
        vm.extensions = extensions;

        vm.elementGroups = [];
        repositories.elementGroup.getAll(vm.id).then(domains => {
            angular.forEach(domains, domain => {
                vm.elementGroups.push({ Id: domain.SystemItemId.toLowerCase(), DisplayText: domain.ItemName });

            });
        });

        $scope.$parent.pageTitle = 'Element List';
        $scope.allElementGroups = true;
        $scope.elementGroups = {};
        $scope.allItemTypes = true;
        $scope.itemTypes = {};
        $scope.allItems = true;
        $scope.items = {};
        $scope.extensions = {};
        $scope.globalSearch = '';
        $scope.pageSize = 10;
        $scope.pageNo = 0;
        $scope.orderBy = { column: 1, dir: 'asc' };

        vm.elementsHref = element => {
            return services.state.href('app.element.detail.info', {
                dataStandardId: vm.id,
                elementId: element.SystemItemId,
                elementListFilter: JSON.stringify(vm.currentFilter)
            });
        }

        vm.saveFilterToSession = () => {
            var elementListFilters = {
                elementGroups: $scope.elementGroups,
                extensions: $scope.extensions,
                itemTypes: $scope.itemTypes,
                items: $scope.items,
                globalSearch: $scope.globalSearch,
                pageSize: $scope.pageSize,
                pageNo: $scope.pageNo,
                orderBy: $scope.orderBy
            };

            if (vm.isExtensionTable) {
                services.session.cloneToSession('elementExtensionListFilters', vm.id, elementListFilters);
            } else {
                services.session.cloneToSession('elementListFilters', vm.id, elementListFilters);
            }
        };

        if ($stateParams.filter != undefined) {
            if (vm.isExtensionTable) {
                services.session.cloneToSession('elementExtensionListFilters', vm.id, JSON.parse($stateParams.filter));
            } else {
                services.session.cloneToSession('elementListFilters', vm.id, JSON.parse($stateParams.filter));
            }

            services.state.go(services.state.current.name, {
                dataStandardId: $stateParams.dataStandardId,
                filter: null
            }, { location: 'replace' });
        }

        if ($stateParams.elementGroup != undefined) {
            if ($stateParams.elementGroup.match(/^[{]?[0-9a-fA-F]{8}[-]?([0-9a-fA-F]{4}[-]?){3}[0-9a-fA-F]{12}[}]?$/)) {
                $scope.allElementGroups = false;
                $scope.elementGroups[$stateParams.elementGroup] = true;

                vm.saveFilterToSession();

                services.location.search('elementGroup', null);

            }
        }

        vm.checkForFiltering = () => {
            vm.needToFilterFromSession = false;

            var elementListFilters = (vm.isExtensionTable) ? services.session.cloneFromSession('elementExtensionListFilters', vm.id) : services.session.cloneFromSession('elementListFilters', vm.id);
            if (elementListFilters) {
                $scope.elementGroups = elementListFilters.elementGroups;
                $scope.extensions = elementListFilters.extensions ? elementListFilters.extensions : {};
                $scope.itemTypes = elementListFilters.itemTypes;
                $scope.items = elementListFilters.items;
                $scope.globalSearch = elementListFilters.globalSearch;
                $scope.pageSize = elementListFilters.pageSize;
                $scope.pageNo = elementListFilters.pageNo;
                $scope.orderBy = (elementListFilters.orderBy && elementListFilters.orderBy.column) ? elementListFilters.orderBy : { column: 0, dir: 'asc' }
                vm.needToFilterFromSession = true;
            }

            services.timeout(() => { vm.getList() }, 1);
        };

        vm.checkForFiltering();

        vm.getList = () => {
            var dataTableElement = <any>$('#elementListTable'); // TODO: Cast properly (cpt)
            vm.table = dataTableElement.DataTable(
                {
                    serverSide: true,
                    processing: true,
                    ajax: {
                        url: `api/ElementList/${vm.id}/paging`,
                        type: 'POST',
                        data: (data) => {

                            data.MappedSystemExtensions = [];
                            if ($scope.extensions) {
                                for (var extensionKey in $scope.extensions) {
                                    if ($scope.extensions[extensionKey])
                                        data.MappedSystemExtensions.push(extensionKey);
                                }
                            }

                            $scope.allExtensions = data.MappedSystemExtensions.length === 0;

                            data.ElementGroups = [];
                            if ($scope.elementGroups) {
                                for (var groupKey in $scope.elementGroups) {
                                    if ($scope.elementGroups[groupKey])
                                        data.ElementGroups.push(groupKey);
                                }
                            }
                            $scope.allElementGroups = data.ElementGroups.length === 0;

                            data.ItemTypes = [];
                            if ($scope.itemTypes) {
                                for (var key in $scope.itemTypes) {
                                    if ($scope.itemTypes[key]) {
                                        if (key === 'element') data.ItemTypes.push(4);
                                        if (key === 'enumeration') data.ItemTypes.push(5);
                                    }
                                }
                            }
                            $scope.allItemTypes = data.ItemTypes.length === 0;

                            data.IsExtended = null;
                            if ($scope.items) {
                                if ($scope.items['base'] && $scope.items['extended']) {
                                    $scope.allItems = false;
                                    data.IsExtended = null;
                                }
                                else if ($scope.items['base']) {
                                    data.IsExtended = false;
                                    $scope.items['extended'] = undefined;
                                    $scope.allItems = false;
                                }
                                else if ($scope.items['extended']) {
                                    data.IsExtended = true;
                                    $scope.items['base'] = undefined;
                                    $scope.allItems = false;
                                }
                                else {
                                    data.IsExtended = null;
                                    $scope.items = {};
                                    $scope.allItems = true;
                                }
                            } else {
                                $scope.items = {};
                                $scope.allItems = true;
                            }

                            $scope.orderBy = data.order[0];
                            $scope.globalSearch = data.search.value;
                            $scope.pageNo = Math.floor((data.start + 1) / data.length);
                            $scope.pageSize = data.length;

                            vm.saveFilterToSession();
                            vm.currentFilter = angular.copy(data);
                            return data;
                        },
                        dataSrc: (data) => {
                            vm.elements = angular.copy(data.data);
                            return data.data;
                        }
                    },
                    createdRow(row) {
                        services.compile(angular.element(row).contents())($scope);
                    },
                    columns: [
                        {
                            data: 'Element',
                            visible: services.state.current.name === 'app.data-standard.edit.extension-elements'
                        },
                        {
                            data: 'PathSegments'
                        },
                        {
                            data: 'Element'
                        },
                        {
                            data: 'Element.ItemTypeName'
                        },
                        {
                            data: 'Element.TypeName'
                        },
                        {
                            data: 'Element.Length'
                        }
                    ],
                    columnDefs: [
                        {
                            targets: 0,
                            render(element, type, row, meta) {
                                var html = '<div class="text-center">';
                                if(element.ExtensionShortName) html += element.ExtensionShortName;
                                else html += 'Core';
                                html += '</div>';
                                return html;
                            }
                        },
                        {
                            targets: 1,
                            render(pathSegments, type, row, meta) {
                                var html = '<div class="hidden">';
                                var namePath = '';
                                for (var i = 0; i < pathSegments.length; i++) {
                                    namePath += namePath.length > 0 ? '.' : '';
                                    namePath += pathSegments[i].Name;
                                }
                                if (row.Element) {
                                    namePath += '.' + row.Element.Name;
                                }
                                html += namePath;
                                html += '</div>';
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
                            targets: 2,
                            render(element, type, row, meta) {
                                var html = '<a class="';
                                html += (element.IsExtended) ? 'standard-c' : 'standard-a';
                                html += '" href="';
                                html += vm.elementsHref(element);
                                html += '"';
                                html += '\', elementId: \'';
                                html += element.SystemItemId;
                                html += '\'})"';
                                html += '">';
                                if (element.ExtensionShortName) html += `(${element.ExtensionShortName}) `;
                                else if (element.IsExtended) html += '<i class="fa fa-extended"></i> ';
                                html += element.Name;
                                html += '</a><br />';
                                html += element.Definition || '';
                                return html;
                            }
                        }
                    ],
                    displayStart: $scope.pageNo * $scope.pageSize,
                    order: [[$scope.orderBy.column, $scope.orderBy.dir]],
                    pageLength: $scope.pageSize,
                    search: { search: $scope.globalSearch }
                });
            $('.dataTables_processing').html('<div class="loading-inner"><img src= "Client/Content/Images/Loading.gif" alt= "Loading">Loading...</div>');

        }

        $('#elementListTable').on('order.dt', () => {
            if (!this.needToFilterFromSession && !vm.loading && vm.table) {
                services.timeout(() => {
                    $scope.orderBy = vm.table.order();
                    this.saveFilterToSession();
                }, 20000);
            }
        });

        $('#elementListTable').on('length.dt', (e, settings, len) => {
            $scope.pageSize = len;
            vm.saveFilterToSession();
        });

        $('#elementListTable').on('page.dt', () => {
            if (!this.needToFilterFromSession && !vm.loading && vm.table) {
                $scope.pageNo = vm.table.page.info().page;
                this.saveFilterToSession();
            }
        });

        $('#elementListTable').on('draw.dt', () => {
            angular.element(document.querySelector('input[type=search]')).unbind('keyup').keyup(() => {
                $scope.globalSearch = angular.element(vm).val();
                vm.saveFilterToSession();
            });

            var popoverElement = <any>$('[data-toggle="popover"]'); // TODO: Cast properly (cpt)
            popoverElement.popover();
        });

        vm.applyFilter = () => {
            vm.table.ajax.reload();
            vm.saveFilterToSession();
        };

        vm.clearElementGroups = () => {
            if ($scope.allElementGroups) {
                for (var key in $scope.elementGroups)
                    $scope.elementGroups[key] = false;
            }
        };

        vm.clearItemTypes = () => {
            if ($scope.itemTypes) {
                for (var key in $scope.itemTypes)
                    $scope.itemTypes[key] = false;
            }
        };

        vm.clearItems = () => {
            if ($scope.items) {
                for (var key in $scope.items)
                    $scope.items[key] = false;
            }
        };
    }
]);