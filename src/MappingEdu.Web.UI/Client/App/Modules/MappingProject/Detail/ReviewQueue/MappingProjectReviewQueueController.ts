// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.detail.review-queue
//

var m = angular.module('app.modules.mapping-project.detail.review-queue', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.mapping-project.detail.review-queue', {
            url: '/queue?filter&filterId',
            data: {
                title: 'Mapping Project Queue',
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Guest')].Id
            },
            templateUrl: `${settings.moduleBaseUri}/mappingProject/detail/reviewQueue/mappingProjectReviewQueueView.tpl.html`,
            controller: 'app.modules.mapping-project.detail.review-queue',
            controllerAs: 'mappingProjectReviewQueueViewModel',
            resolve: {
                initObject: ['repositories', 'mappingProject', (repositories: IRepositories, mappingProject) => {
                    return repositories.elementGroup.getAll(mappingProject.SourceDataStandardId).then(domains => {
                        return repositories.dataStandard.isExtended(mappingProject.SourceDataStandardId).then(isExtended => {
                            return { project: mappingProject, domains: domains, isExtended: isExtended }
                        });
                    });
                }],
                filters: ['repositories', '$stateParams', (repositories: IRepositories, $stateParams) => {
                    return repositories.mappingProject.reviewQueue.filter.getAll($stateParams.id);
                }],
                createBys: ['repositories', '$stateParams', (repositories: IRepositories, $stateParams) => {
                    return repositories.element.mapping.uniqueCreateBy($stateParams.id);
                }],
                updateBys: ['repositories', '$stateParams', (repositories: IRepositories, $stateParams) => {
                    return repositories.element.mapping.uniqueUpdateBy($stateParams.id);
                }]
            }
        });
}]);

m.run(['$rootScope', '$state', 'services', ($rootScope, $state, services: IServices) => {
    $rootScope.$on('$stateChangeStart', (event, toState, toParams, fromState, fromParams) => {
        if (toState.name === 'app.mapping-project.detail.review-queue') {

            if (toParams.filter) {
                event.preventDefault();

                var filter = JSON.parse(toParams.filter);
                services.session.cloneToSession('queueFilters', toParams.id, filter);

                services.state.go('app.mapping-project.detail.review-queue', {
                    id: toParams.id
                }, { location: 'replace' });
            }
            else if (toParams.filterId) {
                event.preventDefault();

                services.session.cloneToSession('queueFilterId', toParams.id, toParams.filterId);

                services.state.go('app.mapping-project.detail.review-queue', {
                    id: toParams.id
                }, { location: 'replace' });
            }
        }
    });
}]);


// ****************************************************************************
// Controller app.modules.mapping-project.detail.review-queue
//

m.controller('app.modules.mapping-project.detail.review-queue', ['$', '$scope', '$stateParams', 'enumerations', 'modals', 'repositories', 'services', 'initObject', 'filters', 'createBys', 'updateBys',
    function ($, $scope, $stateParams, enumerations: IEnumerations, modals: IModals, repositories: IRepositories, services: IServices, initObject, filters, createBys, updateBys) {

        services.logger.debug('Loaded controller app.modules.mapping-project.detail.review-queue');

        var vm = this;

        vm.loading = false;
        vm.id = $stateParams.id;
        vm.table;
        vm.total = 0;
        vm.filters = filters;
        vm.filters.unshift({
            MappingProjectQueueFilterId: 0,
            Name: 'Clear all Filters',
            OrderColumn: 0,
            OrderDirection: 'asc',
            CreatedByColumn: false,
            CreationDateColumn: false,
            UpdatedByColumn: false,
            UpdateDateColumn: false,
            MappedByColumn: false,
            Length: 10
        });

        var project = initObject.project;
        vm.mappingProject = initObject.project;
        vm.isExtended = initObject.isExtended;

        vm.elementGroups = [];
        angular.forEach(initObject.domains, domain => {
            vm.elementGroups.push({ Id: domain.SystemItemId.toLowerCase(), DisplayText: domain.ItemName });
        });

        vm.createBys = createBys;
        vm.updateBys = updateBys;

        $scope.$parent.mappingProjectDetailViewModel.setTitle('QUEUE');

        $scope.allMethods = true;
        $scope.allStatuses = true;
        $scope.allElementGroups = true;
        $scope.elementGroups = {};
        $scope.allItemTypes = true;
        $scope.itemTypes = {};
        $scope.allItems = true;
        $scope.items = {};
        $scope.methods = {};
        $scope.columns = {
            CreateBy: false,
            UpdateBy: false,
            CreateDate: false,
            UpdateDate: false,
            AutoMapper: false
        };
        $scope.createBys = {};
        $scope.updateBys = {};
        $scope.statuses = {};
        $scope.flagged = false;
        $scope.unflagged = false;
        $scope.globalSearch = '';
        $scope.pageSize = 10;
        $scope.pageNo = 0;
        $scope.orderBy = { column: 0, dir: 'asc' };

        services.profile.mappingProjectAccess(vm.id).then((data) => {
            vm.isGuest = data.Role === 0;
            services.profile.me().then(me => {
                vm.me = me;
                $scope.statuses = (vm.isGuest) ? {} : $scope.statuses;
                vm.readOnly = data.Role < 2 && !me.IsAdministrator;
            });
        });

        vm.saveFilterToSession = () => {
            services.session.cloneToSession('queueFilters', vm.id, {
                elementGroups: $scope.elementGroups,
                itemTypes: $scope.itemTypes,
                methods: $scope.methods,
                statuses: ((vm.isGuest) ? {} : $scope.statuses),
                flagged: $scope.flagged,
                unflagged: $scope.unflagged,
                globalSearch: $scope.globalSearch,
                pageSize: $scope.pageSize,
                pageNo: $scope.pageNo,
                orderBy: $scope.orderBy,
                createBys: $scope.createBys,
                updateBys: $scope.updateBys,
                extended: $scope.extended,
                base: $scope.base,
                autoMapped: $scope.autoMapped,
                userMapped: $scope.userMappded,
                columns: $scope.columns
            });
        };

        vm.checkForFiltering = () => {
            vm.needToFilterFromSession = false;
            vm.currentFilterId = services.session.cloneFromSession('queueFilterId', vm.id);

            if (vm.currentFilterId) {
                services.session.clearSection('queueFilterId');
                var currentFilter = services.underscore.find(<Array<any>>vm.filters, (filter) => filter.MappingProjectQueueFilterId === vm.currentFilterId);

                var elementGroups = {};
                angular.forEach(currentFilter.ElementGroups, elementGroupId => {
                    elementGroups[elementGroupId] = true;
                });

                var itemTypes = {};
                angular.forEach(currentFilter.ItemTypes, itemTypeId => {
                    if (itemTypeId === 4) itemTypes['element'] = true;
                    if (itemTypeId === 5) itemTypes['enumeration'] = true;

                });

                var methods = {};
                angular.forEach(currentFilter.MappingMethods, methodId => {
                    methods[methodId] = true;
                });

                var statuses = {};
                angular.forEach(currentFilter.WorkflowStatuses, statusId => {
                    statuses[statusId] = true;
                });

                var createBys = {};
                angular.forEach(currentFilter.CreatedByUserIds, userId => {
                    createBys[userId] = true;
                });

                var updateBys = {};
                angular.forEach(currentFilter.UpdatedByUserIds, userId => {
                    updateBys[userId] = true;
                });

                $scope.elementGroups = elementGroups;
                $scope.itemTypes = itemTypes;
                $scope.methods = methods;
                $scope.statuses = statuses;
                $scope.flagged = currentFilter.Flagged;
                $scope.unflagged = currentFilter.Unflagged;
                $scope.globalSearch = currentFilter.Search;
                $scope.pageSize = currentFilter.Length > 0 ? currentFilter.Length : 10;
                $scope.pageNo = 0;
                $scope.extended = currentFilter.Extended;
                $scope.base = currentFilter.Base;
                $scope.autoMapped = currentFilter.AutoMapped;
                $scope.userMapped = currentFilter.UserMapped;
                $scope.orderBy = { column: currentFilter.OrderColumn, dir: currentFilter.OrderDirection }
                $scope.createBys = createBys;
                $scope.updateBys = updateBys;
                $scope.columns = {
                    CreateBy: currentFilter.CreatedByColumn,
                    UpdateBy: currentFilter.UpdatedByColumn,
                    CreateDate: currentFilter.CreationDateColumn,
                    UpdateDate: currentFilter.UpdateDateColumn,
                    AutoMapper: currentFilter.MappedByColumn
                };

            } else {
                var queueFilters = services.session.cloneFromSession('queueFilters', vm.id);
                if (queueFilters) {
                    $scope.elementGroups = queueFilters.elementGroups;
                    $scope.itemTypes = queueFilters.itemTypes;
                    $scope.methods = queueFilters.methods;
                    $scope.statuses = (vm.isGuest) ? {} : queueFilters.statuses;
                    $scope.flagged = queueFilters.flagged;
                    $scope.unflagged = queueFilters.unflagged;
                    $scope.globalSearch = queueFilters.globalSearch;
                    $scope.pageSize = queueFilters.pageSize;
                    $scope.pageNo = queueFilters.pageNo;
                    $scope.orderBy = (queueFilters.orderBy && queueFilters.orderBy.column) ? queueFilters.orderBy : { column: 0, dir: 'asc' }
                    $scope.createBys = queueFilters.createBys || {};
                    $scope.updateBys = queueFilters.updateBys || {};
                    $scope.columns = queueFilters.columns || {
                        CreateBy: false,
                        UpdateBy: false,
                        CreateDate: false,
                        UpdateDate: false,
                        AutoMapper: false
                    };
                    $scope.extended = queueFilters.extended;
                    $scope.base = queueFilters.base;
                    $scope.autoMapped = queueFilters.autoMapped;
                    $scope.userMapped = queueFilters.userMapped;

                    for (var key in $scope.columns) {
                        if ($scope.columns[key]) $scope.columnsFiltered = true;
                    }

                    vm.needToFilterFromSession = true;
                }   
            }

            services.timeout(() => {vm.getReviewQueue()}, 1);
        };

        vm.createQuery = (data) => {
            data.ElementGroups = [];
            if ($scope.elementGroups) {
                for (var groupKey in $scope.elementGroups) {
                    if ($scope.elementGroups[groupKey])
                        data.ElementGroups.push(groupKey);
                }
            }
            $scope.allElementGroups = data.ElementGroups.length === 0;

            data.CreatedByUserIds = [];
            if ($scope.createBys) {
                for (var userKey in $scope.createBys) {
                    if ($scope.createBys[userKey])
                        data.CreatedByUserIds.push(userKey);
                }
            }
            $scope.allCreateBys = data.CreatedByUserIds.length === 0;

            data.UpdatedByUserIds = [];
            if ($scope.updateBys) {
                for (var userKey in $scope.updateBys) {
                    if ($scope.updateBys[userKey])
                        data.UpdatedByUserIds.push(userKey);
                }
            }
            $scope.allUpdateBys = data.UpdatedByUserIds.length === 0;

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

            data.MappingMethods = [];
            if ($scope.methods) {
                for (var key in $scope.methods) {
                    if ($scope.methods[key])
                        data.MappingMethods.push(key);
                }
            }

            $scope.allMethods = data.MappingMethods.length === 0;

            data.WorkflowStatuses = [];
            if ($scope.statuses) {
                for (var key in $scope.statuses) {
                    if ($scope.statuses[key]) {
                        data.WorkflowStatuses.push(key);
                    }
                }
            }
            $scope.allStatuses = data.WorkflowStatuses.length === 0;

            data.Total = vm.total;
            data.Flagged = $scope.flagged;
            data.Unflagged = $scope.unflagged;

            $scope.allItemsAutoMapped = !($scope.autoMapped || $scope.userMapped);
            data.AutoMapped = $scope.autoMapped;
            data.UserMapped = $scope.userMapped;

            $scope.allItems = !($scope.extended || $scope.base);
            data.Extended = $scope.extended;
            data.Base = $scope.base;

            return data;
        }

        vm.getReviewQueue = () => {

            vm.table = $('#reviewQueueTable').DataTable(
                {
                    serverSide: true,
                    processing: true,
                    ajax: {
                        url: `api/MappingProjectReviewQueue/${vm.id}/paging`,
                        type: 'POST',
                        data: (data) => {

                            data = vm.createQuery(data);
                            $scope.orderBy = data.order[0];
                            $scope.globalSearch = data.search.value;
                            $scope.pageNo = Math.floor((data.start + 1) / data.length);
                            $scope.pageSize = data.length;
                            vm.saveFilterToSession();
                            vm.currentFilter = angular.copy(data);

                            return data;
                        },
                        dataSrc: (data) => {
                            angular.forEach(data.data, (item, index) => {
                                item.row = index;
                            });
                            vm.reviewQueue = { ReviewItems: data.data }
                            vm.total = data.recordsTotal;
                            return data.data;
                        }
                    },
                    createdRow(row) {
                        services.compile(angular.element(row).contents())($scope);
                    },
                    columns: [
                        {
                            data: 'Element',
                            dataSort: 0
                        },
                        {
                            data: 'Element.ItemTypeName'
                        },
                        {
                            data: 'Mapping'
                        },
                        {
                            data: 'Mapping',
                            visible: !vm.isGuest 
                        },
                        {
                            data: 'Mapping',
                            visible: !vm.readOnly && $scope.columns.CreateBy
                        },
                        {
                            data: 'Mapping',
                            visible: !vm.readOnly && $scope.columns.CreateDate
                        },
                        {
                            data: 'Mapping',
                            visible: !vm.readOnly && $scope.columns.UpdateBy
                        },
                        {
                            data: 'Mapping',
                            visible: !vm.readOnly && $scope.columns.UpdateDate
                        },
                        {
                            data: 'Mapping',
                            visible: !vm.readOnly && $scope.columns.AutoMapper
                        },
                        {
                            visible: !vm.readOnly,
                            searchable: false,
                            sortable: false
                        }
                    ],
                    columnDefs: [
                        {
                            targets: 0,
                            render(element, type, row) {
                                var html = '<div class="hidden">';
                                for (var i = 0; i < row.PathSegments.length; i++) {
                                    html += row.PathSegments[i].Name;
                                    if (i < (row.PathSegments.length - 1))
                                        html += '.';
                                }
                                if (element)
                                    html += '.' + element.Name;
                                html += '</div>';
                                html += '<div class="expand-container">';
                                html += '<ma-element-path context-id="mappingProjectReviewQueueViewModel.id"';
                                html += ' segments="mappingProjectReviewQueueViewModel.reviewQueue.ReviewItems[';
                                html += row.row;
                                html += '].PathSegments" ng-click="mappingProjectReviewQueueViewModel.logSearchResults(';
                                html += 'mappingProjectReviewQueueViewModel.reviewQueue.ReviewItems[';
                                html += row.row;
                                html += '])"></ma-element-path>';
                                html += '<i class="fa fa-caret-right separator"></i>';
                                html += '</span>';
                                html += '<div style="display: inline-block"><a href="';
                                html += vm.elementSref(element);
                                html += '"';
                                html += ' data-toggle="popover"';
                                html += ' data-trigger="hover" data-content="';
                                html += (element.Definition == null ? '' : element.Definition);
                                html += '" class="';
                                html += (element.IsExtended) ? 'standard-c">' : 'standard-a">';
                                if (element.IsExtended) html += '<i class="fa fa-extended"></i> ';
                                html += element.Name;
                                html += '</a></div><br />';
                                html += '<div class="expandable-div collapsed" data-ellipsis-id="ellipsis_';
                                html += row.row;
                                html += '">';
                                html += element.Definition == null ? '' : element.Definition;
                                html += '</div>';
                                html += '</div>';
                                return html;
                            }
                        },
                        {
                            targets: 1,
                            render(typeName, type, row) {
                                var html = typeName;

                                return html;
                            }
                        },
                        {
                            targets: 2,
                            render(mapping, type, row) {
                                var html = '<div class="expand-container">';
                                html += '<div class="expandable-div collapsed" data-ellipsis-id="ellipsis_';
                                html += row.row;
                                html += '">';
                                html += '<ma-mapping-detail mapping="mappingProjectReviewQueueViewModel.reviewQueue.ReviewItems[';
                                html += row.row;
                                html += '].Mapping"></ma-mapping-detail>';
                                html += '</div>';
                                html += '<div><a role="button"><i class="fa fa-ellipsis-h hidden pull-right" id="ellipsis_';
                                html += row.row;
                                html += '" title="Expand"></i></a></div></div>';
                                return html;
                            }
                        },
                        {
                            targets: 3,
                            render(mapping, type, row) {
                                var html = '<ma-workflow-status ng-click="mappingProjectReviewQueueViewModel.workflowClick(';
                                html += row.row;
                                html += ')" read-only="mappingProjectReviewQueueViewModel.readOnly" mapping="mappingProjectReviewQueueViewModel.reviewQueue.ReviewItems[';
                                html += row.row;
                                html += '].Mapping" list-mode="true"></ma-workflow-status>';

                                if (row.Element.ItemTypeName === 'Enumeration' && row.Mapping != null && row.Mapping.MappingMethodTypeId === 1) {
                                    var percent = parseFloat(((row.MappedEnumerations / row.TotalEnumerations) * 100).toFixed(2));
                                    if (row.TotalEnumerations === 0)
                                        percent = 100;

                                    html += '<div ';
                                    html += `><a uib-tooltip="${row.MappedEnumerations} of ${row.TotalEnumerations} Enumeration Items Mapped (${percent}%)" tooltip-placement="left"`;
                                    html += ' href="';
                                    html += vm.elementsEnumerationsAnchorSref(row.Element);
                                    html += '" >';
                                    html += '<br/><uib-progress style="margin: 0px;width: 122px;">';
                                    html += `<uib-bar value="${percent}" type= "success"><span ng-hide="${percent < 30}">${percent}%</span></uib-bar>`;
                                    html += `<uib-bar value="${100 - percent}" style="background-color: transparent"><span style="color: black" ng-hide="${percent - 30 >= 0}"> ${percent}%</span></uib-bar>`;
                                    html += '</uib-progress></a></div>';
                                } else if(mapping && mapping.IsAutoMapped && !vm.readOnly) html += '<br/><div class="text-center"><i class="fa fa-magic"></i></div>';
                                return html;
                            }
                        },
                        {
                            targets: 4,
                            render(mapping, type, row) {
                                if (mapping != null) return mapping.CreateBy;
                                else return 'N/A';
                            }
                        },
                        {
                            targets: 5,
                            render(mapping, type, row) {
                                if (mapping != null) return services.filter('date')(mapping.CreateDate, 'dd/MMM/yy');
                                else return 'N/A';
                            }
                        },
                        {
                            targets: 6,
                            render(mapping, type, row) {
                                if (mapping != null) return mapping.UpdateBy;
                                else return 'N/A';
                            }
                        },
                        {
                            targets: 7,
                            render(mapping, type, row) {
                                if (mapping != null) return services.filter('date')(mapping.UpdateDate, 'dd/MMM/yy');
                                else return 'N/A';
                            }
                        },
                        {
                            targets: 8,
                            render(mapping, type, row) {
                                var html = '<div style="width: 100%" class="text-center">';
                                if (mapping) {
                                    if (mapping.IsAutoMapped) html +=  'Auto Mapped<br/><i class="fa fa-magic"></i>';
                                    else html += mapping.CreateBy + '<br/><i class="fa fa-user"></i>';
                                } else html += 'N/A';
                                html += '</div>';
                                return html;
                            }
                        },
                        {
                            targets: 9,
                            searchable: false,
                            render(flagged, type, row) {
                                var html = '<a title="Edit" class="btn btn-edit" href="';
                                html += vm.elementSref(row.Element);
                                html += '"><i class="fa"></i></a>';
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

        vm.addFilter = () => {
            var filter = vm.createQuery({});
            filter.Search = $scope.globalSearch;
            filter.Length = $scope.pageSize;
            filter.OrderColumn = Array.isArray($scope.orderBy.column) ? $scope.orderBy.column[0] : $scope.orderBy.column;
            filter.OrderDirection = $scope.orderBy.dir;
            filter.CreatedByColumn = $scope.columns.CreateBy;
            filter.CreationDateColumn = $scope.columns.CreateDate;
            filter.MappedByColumn = $scope.columns.AutoMapper;
            filter.UpdatedByColumn = $scope.columns.UpdateBy;
            filter.UpdateDateColumn = $scope.columns.UpdateDate;

            var instance = modals.mappingProjectQueueFilterForm(vm.id, filter, initObject.domains, vm.createBys, vm.updateBys);
            return instance.result.then((data) => {
                vm.filters.push(data);
                vm.currentFilterId = data.MappingProjectQueueFilterId;
            });
        }

        vm.updateFilter = () => {
            var filter = vm.createQuery({});
            filter.Search = $scope.globalSearch;
            filter.Length = $scope.pageSize;
            filter.OrderColumn = Array.isArray($scope.orderBy.column) ? $scope.orderBy.column[0] : $scope.orderBy.column;
            filter.OrderDirection = $scope.orderBy.dir;
            filter.CreatedByColumn = $scope.columns.CreateBy;
            filter.CreationDateColumn = $scope.columns.CreateDate;
            filter.MappedByColumn = $scope.columns.AutoMapper;
            filter.UpdatedByColumn = $scope.columns.UpdateBy;
            filter.UpdateDateColumn = $scope.columns.UpdateDate;

            var currentFilter = services.underscore.find(<Array<any>>vm.filters, (filter) => filter.MappingProjectQueueFilterId === vm.currentFilterId);
            filter.MappingProjectQueueFilterId = vm.currentFilterId;
            filter.ShowInDashboard = currentFilter.ShowInDashboard;
            filter.Name = currentFilter.Name;

            var instance = modals.mappingProjectQueueFilterForm(vm.id, filter, initObject.domains, vm.createBys, vm.updateBys);
            return instance.result.then((data) => {
                angular.copy(data, currentFilter);
            });
        }

        vm.onFilterChange = () => {
            var currentFilter = services.underscore.find(<Array<any>>vm.filters, (filter) => filter.MappingProjectQueueFilterId === vm.currentFilterId);

            if (vm.currentFilterId === 0) vm.currentFilterId = null;

            var elementGroups = {};
            angular.forEach(currentFilter.ElementGroups, elementGroupId => {
                elementGroups[elementGroupId] = true;
            });

            var itemTypes = {};
            angular.forEach(currentFilter.ItemTypes, itemTypeId => {
                if (itemTypeId === 4) itemTypes['element'] = true;
                if (itemTypeId === 5) itemTypes['enumeration'] = true;
            });

            var methods = {};
            angular.forEach(currentFilter.MappingMethods, methodId => {
                methods[methodId] = true;
            });

            var statuses = {};
            angular.forEach(currentFilter.WorkflowStatuses, statusId => {
                statuses[statusId] = true;
            });

            var createBys = {};
            angular.forEach(currentFilter.CreatedByUserIds, userId => {
                createBys[userId] = true;
            });

            var updateBys = {};
            angular.forEach(currentFilter.UpdatedByUserIds, userId => {
                updateBys[userId] = true;
            });

            $scope.elementGroups = elementGroups;
            $scope.itemTypes = itemTypes;
            $scope.methods = methods;
            $scope.statuses = statuses;
            $scope.flagged = currentFilter.Flagged;
            $scope.unflagged = currentFilter.Unflagged;
            $scope.globalSearch = currentFilter.Search;
            $scope.pageSize = currentFilter.Length > 0 ? currentFilter.Length : 10;
            $scope.pageNo = 0;
            $scope.extended = currentFilter.Extended;
            $scope.base = currentFilter.Base;
            $scope.autoMapped = currentFilter.AutoMapped;
            $scope.userMapped = currentFilter.UserMapped;
            $scope.orderBy = { column: currentFilter.OrderColumn, dir: currentFilter.OrderDirection }
            $scope.createBys = createBys;
            $scope.updateBys = updateBys;
            $scope.columns = {
                CreateBy: currentFilter.CreatedByColumn,
                UpdateBy: currentFilter.UpdatedByColumn,
                CreateDate: currentFilter.CreationDateColumn,
                UpdateDate: currentFilter.UpdateDateColumn,
                AutoMapper: currentFilter.MappedByColumn
            };

            vm.table.column(4).visible(currentFilter.CreatedByColumn);
            vm.table.column(5).visible(currentFilter.CreationDateColumn);
            vm.table.column(6).visible(currentFilter.UpdatedByColumn);
            vm.table.column(7).visible(currentFilter.UpdateDateColumn);
            vm.table.column(8).visible(currentFilter.MappedByColumn);
            vm.table.search(currentFilter.Search ? currentFilter.Search : '');
            vm.table.order([[$scope.orderBy.column, $scope.orderBy.dir]]);
            vm.table.page.len($scope.pageSize);

            vm.applyFilter();
        }

        vm.elementSref = element => {
            return services.state.href('app.element.detail.mapping', {
                mappingProjectId: vm.id,
                elementListFilter: JSON.stringify(vm.currentFilter),
                elementId: element.SystemItemId
            });
        }

        vm.workflowClick = (row) => {
            //Used to stop toggling if workflow was clicked
            vm.workflowClicked = true;
            services.timeout(() => { vm.workflowClicked = false });

            var mapping = vm.reviewQueue.ReviewItems[row].Mapping;

            mapping.UpdateBy = vm.me.FirstName + ' ' + vm.me.LastName;
            mapping.UpdateDate = services.filter('date')(new Date(), 'MM-dd-yyyy');

            var cell = vm.table.cell({ row: row, column: 6 });
            cell.data(mapping);

            cell = vm.table.cell({ row: row, column: 7 });
            cell.data(mapping);
        }


        vm.elementsEnumerationsAnchorSref  = element => {
            return services.state.href('app.element.detail.mapping', {
                mappingProjectId: vm.id,
                elementListFilter: JSON.stringify(vm.currentFilter),
                elementId: element.SystemItemId,
                '#': 'enumerations'
            });
        }

        vm.logSearchResults = () => {
            if (vm.table) {
                var searchText = vm.table.search();
                if (searchText.length > 0) {
                    services.logging.add({
                        Source: 'reviewQueue',
                        Message: `${project.ProjectName} ( ${project.MappingProjectId} ) - searched for: ${searchText}`
                    }).then(function () {
                        var info = vm.table.page.info();
                        var rowCount = info.recordsDisplay;
                        var recordCount = info.recordsTotal;
                        services.logging.add({
                            Source: 'reviewQueue',
                            Message: `${project.ProjectName} ( ${project.MappingProjectId} ) - Search filtered results to ${rowCount} rows from ${recordCount} total.`
                        });
                    });
                } else {
                    services.logging.add({
                        Source: 'reviewQueue',
                        Message: `${project.ProjectName} ( ${project.MappingProjectId} ) - No search filter was supplied`
                    });
                }
            }
        };

        vm.workflowEnum = enumerations.WorkflowStatusType;
        vm.mappingMethodEnum = enumerations.MappingMethodTypeInQueue;
        vm.checkForFiltering();

        var sortCalls = 0; //Calls function below 3 times on start avoids over logging

        $('#reviewQueueTable').on('order.dt', (e, settings) => {
            if (vm.table) {
                sortCalls++;
                var order = vm.table.order();
                var column = settings.aoColumns[order[0][0]];
                if (column.sTitle.length > 0 && project && sortCalls > 2)
                    services.logging.add({
                        Source: 'reviewQueue',
                        Message: `${project.ProjectName} ( ${project.MappingProjectId} ) - Search results ordered by ${column.sTitle} ${order[0][1]}`
                    });
            }
        });

        $('#reviewQueueTable').on('length.dt', (e, settings, len) => {
            $scope.pageSize = len;
            vm.saveFilterToSession();
            if (vm.queueDirty)
                vm.refreshQueue();
        });

        $('#reviewQueueTable').on('page.dt', () => {

            var info = vm.table.page.info();
            services.logging.add({
                Source: 'reviewQueue',
                Message: `${project.ProjectName} ( ${project.MappingProjectId} ) - Search results current page changed to page ${++info.page} of ${info.pages}`
            });

            if (!this.needToFilterFromSession && !vm.loading) {
                $scope.pageNo = vm.table.page.info().page;
                this.saveFilterToSession();
            }
        });

        $('#reviewQueueTable').on('draw.dt', () => {

            if ($('.dataTables_empty').length > 0)
                return;
            $('[data-toggle="popover"]').popover();
            var source = null;
            var target = null;

            $('tr').find('td').each((index, element) => {
                $(element).css('vertical-align', 'top');
            });

            $('tr').each((index, tr) => {
                $(tr).find('div.expandable-div').each((index, element) => {
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
                        $(tr).css('cursor', 'pointer');
                        $(tr).click(() => {
                            //Don't want collapse to toggle if workflow flow status was clicked
                            if (!vm.workflowClicked) {
                                if (ellipsis.css('display') != 'none') {
                                    ellipsis.toggle();
                                    expandableDivs.toggleClass('collapsed', 300);
                                } else {
                                    ellipsis.toggle();
                                    expandableDivs.toggleClass('collapsed', 300);
                                }
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
            });
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

        vm.clearUpdateBys = () => {
            if ($scope.allUpdateBys) {
                for (var key in $scope.updateBys)
                    $scope.updateBys[key] = false;
            }
        };

        vm.clearCreateBys = () => {
            if ($scope.allCreateBys) {
                for (var key in $scope.createBys)
                    $scope.createBys[key] = false;
            }
        };

        vm.clearItemTypes = () => {
            if ($scope.itemTypes) {
                for (var key in $scope.itemTypes)
                    $scope.itemTypes[key] = false;
            }
        };

        vm.clearMethods = () => {
            if ($scope.allMethods) {
                for (var key in $scope.methods)
                    $scope.methods[key] = false;
            }
        };

        vm.clearStatuses = () => {
            if ($scope.allStatuses) {
                for (var key in $scope.statuses)
                    $scope.statuses[key] = false;
            }
        };

        vm.clearItems = () => {
            if ($scope.allItems) {
                $scope.extended = false;
                $scope.base = false;
            }
        };

        vm.clearAutoMapped = () => {
            if ($scope.allItemsAutoMapped) {
                $scope.autoMapped = false;
                $scope.userMapped = false;
            }
        };

        $scope.$on('mapping-error', (event, data) => {
            services.errors.handleErrors(data, vm);
        });

        vm.refreshQueue = () => {
            vm.checkForFiltering();
            vm.getReviewQueue();
            vm.queueDirty = false;
        };

        vm.saveSuccess = () => {
            vm.queueDirty = true;
            $scope.$emit('workflow-status-updated');
        }

        vm.toggleColumn = (column, toggle) => {
            vm.table.column(column).visible(toggle);

            $scope.columnsFiltered = false;
            for (var key in $scope.columns) {
                if ($scope.columns[key]) $scope.columnsFiltered = true;
            }

            vm.saveFilterToSession();
        }
    }
]);