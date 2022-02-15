// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.data-standard.edit.extensions.online-report
//

var m = angular.module('app.modules.data-standard.edit.extensions.report', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.data-standard.edit.extensions.report', {
            url: '/report?parentId',
            data: {
                title: 'Extensions',
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Owner')].Id,
                extensionsPublicAccess: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Guest')].Id
            },
            templateUrl: `${settings.moduleBaseUri}/DataStandard/Edit/Extensions/Report/OnlineExtensionsReportView.tpl.html`,
            controller: 'app.data-standard.edit.extensions.report',
            controllerAs: 'onlineExtensionsReportViewModel',
            resolve: {
                report: ['repositories', '$stateParams', (repositories: IRepositories, $stateParams) => {
                    return repositories.dataStandard.extensions.getReport($stateParams.dataStandardId, $stateParams.parentId);
                }],
                parent: ['repositories', '$stateParams', (repositories: IRepositories, $stateParams) => {
                    if (!$stateParams.parentId) return null;
                    else {
                        return repositories.systemItem.find($stateParams.parentId);
                    }
                }]
            }
        });
}]);


// ****************************************************************************
// Controller app.data-standard.edit.extensions
//

m.controller('app.data-standard.edit.extensions.report', ['$', '$stateParams', 'repositories', 'services', 'extensions', 'report', 'parent', 'standard', 'access',
    function ($, $stateParams, repositories: IRepositories, services: IServices, extensions, report, parent, standard, access) {

        services.logger.debug('Loaded controller app.data-standard.edit.extensions.report');

        var vm = this;
        vm.extensions = services.filter('orderBy')(angular.copy(extensions), 'ShortName');
        vm.standard = standard;
        vm.report = angular.copy(report);
        vm.parent = parent;
        vm.access = access;

        services.profile.dataStandardAccess($stateParams.dataStandardId).then(access => {
            console.log(access);
        });

        vm.total = {
            ItemName: vm.parent ? vm.parent.ItemName : `${vm.standard.SystemName} (${vm.standard.SystemVersion})`,
            Total: 0,
            SystemItemId: vm.parent ? vm.parent.SystemItemId : null
        };

        vm.containsAssociations = false;
        vm.containsDescriptors = false;

        angular.forEach(vm.report, row => {
            vm.total.Total += row.Total;
            angular.forEach(vm.extensions, extension => {
                if (vm.total[extension.ShortName]) vm.total[extension.ShortName] += row[extension.ShortName];
                else vm.total[extension.ShortName] = row[extension.ShortName];
            });

            if (row.Total === 0) row.Type = 'notExtended';
            else if (row.Total > 0 && !row.ShortName) row.Type = 'extended';
            else row.Type = 'new';

            if (!parent || parent.PathSegments.length < 2) {
                if (row.ItemName.toLowerCase().endsWith('association') || row.ItemName.toLowerCase().endsWith('associations')) vm.containsAssociations = true;
                if (row.ItemName.toLowerCase().endsWith('descriptors') || row.ItemName.toLowerCase().endsWith('descriptor')) vm.containsDescriptors = true;   
            }

        });

        vm.filteredReport = angular.copy(vm.report);

        vm.colors = [
            { Header: '#38B5E6', Cell: '#bbe6f7', Hover: '#a4ddf4', Selected: '#60c4eb' }, // Column1
            { Header: '#F8972A', Cell: '#fddbb5', Hover: '#fccf9c', Selected: '#faab52' }, // Column2
            { Header: '#63BAA9', Cell: '#cae8e2', Hover: '#b8e0d8', Selected: '#83c9bb' }, // Column3
            { Header: '#E41564', Cell: '#f8b9d1', Hover: '#f6a2c2', Selected: '#f05c94' }, // Column4
            { Header: '#0E76BD', Cell: '#b8dffa', Hover: '#a0d5f8', Selected: '#59b5f3' }, // Column5
            { Header: '#FFC528', Cell: '#ffebb3', Hover: '#ffe499', Selected: '#ffcf4d' }, // Column6
            { Header: '#AC238D', Cell: '#f2c0e6', Hover: '#eeaade', Selected: '#e16bc5' }  // Column7
        ];

        vm.table = {
             Colors: [
                {
                     strokeColor: [],
                     fillColor: [],
                     highlightFill: [],
                     highlightStroke: []
                }],
             Labels: [],
             Data: [[]],
             Series: ['Total Extensions']
        }

        vm.allExtensions = true;
        vm.allEntityTypes = true;
        vm.allExtensionStatuses = false;
        vm.entityType = {};
        vm.extensionStatus = {
            'new': true,
            'extended': true
        }
        vm.showSidebar = true;

        angular.forEach(vm.extensions, (extension, index) => {
            extension.Color = vm.colors[index % vm.colors.length];
            extension.Selected = false;
        });

        var model = services.session.cloneFromSession('extension-report', vm.standard.DataStandardId);
        if (model) {
            if (model && model.extension && Object.keys(model.extension).length > 0) {
                vm.allExtensions = true;
                angular.forEach(vm.extensions, (extension, index) => {
                    if (model.extension[extension.ShortName]) vm.allExtensions = false;
                    extension.Selected = model.extension[extension.ShortName];
                });
            }

            if (model && model.entityType && Object.keys(model.entityType).length > 0) {
                vm.allEntityTypes = true;
                angular.forEach(model.entityType, type => {
                    if (type) vm.allEntityTypes = false;
                });
                vm.entityType = model.entityType;
            }

            if (model && model.extensionStatus && Object.keys(model.extensionStatus).length > 0) {
                vm.allExtensionStatuses = true;
                angular.forEach(model.extensionStatus, status => {
                    if (status) vm.allExtensionStatuses = false;
                });
                vm.extensionStatus = model.extensionStatus;
            }

            if (model.showSidebar !== undefined)
                vm.showSidebar = model.showSidebar;
        }

        vm.selectCell = (extension, row, index) => {
            if (!extension) {
                if (vm.selected && !vm.selected.Extension && vm.selected.Index === index) {
                    vm.selected = null;
                    vm.extensionDetails = null;
                    vm.createTable(vm.total);
                } else {
                    var selectedExtension;
                    angular.forEach(vm.extensions, ext => {
                        if (row[ext.ShortName] && !selectedExtension) selectedExtension = ext;
                    });
                    if (selectedExtension) {
                        vm.selected = { Row: row, Index: index, Tab: selectedExtension.ShortName }
                        vm.getExtensionDetail(row.SystemItemId, selectedExtension);
                    } else {
                        vm.selected = { Row: row, Index: index }
                    }
                }
            } else if (!row[extension.ShortName] && index === -1) {
                vm.extensionDetails = null;
                vm.selected = { Row: row, Extension: extension, Index: index }
            } else {
                if (!row[extension.ShortName] || (vm.selected && vm.selected.Extension && vm.selected.Extension.ShortName === extension.ShortName && vm.selected.Index === index)) {
                    vm.selected = null;
                    vm.extensionDetails = null;
                    vm.createTable(vm.total, -1);
                }
                else {
                    vm.selected = { Row: row, Extension: extension, Index: index }
                    vm.getExtensionDetail(row.SystemItemId, extension);
                }
            }
        }

        vm.onSearch = () => {
            $('#extensions').scrollTop(0);
        }

        vm.selectTab = (row, extension) => {
            vm.selected.Tab = extension.ShortName;
            vm.getExtensionDetail(row.SystemItemId, extension);
        }

        vm.getExtensionDetail = (systemItemId, extension) => {
            vm.search = '';
            repositories.dataStandard.extensions.getReportDetail($stateParams.dataStandardId, { SystemItemId: systemItemId, MappedSystemExtensionId: extension.MappedSystemExtensionId }).then(data => {
                angular.forEach(data, detail => {
                    detail.ElementId = detail.SystemItemId;
                });
                vm.extensionDetails = data;
                services.session.cloneToSession('elementLists', vm.standard.DataStandardId, vm.extensionDetails);

                var oldFilter = services.session.cloneFromSession('elementExtensionListFilters', $stateParams.dataStandardId);

                var filter = {
                    elementGroups: {},
                    extensions: {},
                    itemTypes: {},
                    items: {},
                    globalSearch: '',
                    orderBy: { dir: 'asc', column: 0 },
                    pageNo: (oldFilter) ? oldFilter.pageNo : 0,
                    pageSize: (oldFilter) ? oldFilter.pageSize : 10
                }

                if (vm.parent) {
                    angular.forEach(vm.parent.PathSegments, segment => {
                        filter.globalSearch += segment.ItemName + '.';
                    });
                }

                filter.extensions[extension.MappedSystemExtensionId] = true;

                vm.detailedListHref = services.state.href('app.data-standard.edit.extension-elements', {
                    dataStandardId: $stateParams.dataStandardId,
                    filter: JSON.stringify(filter)
                });

                $('#extensions').scrollTop(0);
                $('#extensions').perfectScrollbar('destroy');
                $('#extensions').perfectScrollbar();

                vm.toggleSidebar(true);
            });
        }
        
        vm.changeEntityType = (allEntityTypes) => {
            if (allEntityTypes) {
                vm.entityType['entity'] = false;
                vm.entityType['association'] = false;
                vm.entityType['descriptor'] = false;
            }

            if (!vm.entityType['entity'] && !vm.entityType['association'] && !vm.entityType['descriptor'])
                vm.allEntityTypes = true;

            vm.reloadTable();
        }

        vm.changeExtensionStatus = (allExtensionStatuses) => {
            if (allExtensionStatuses) {
                vm.extensionStatus['new'] = false;
                vm.extensionStatus['extended'] = false;
                vm.extensionStatus['notExtended'] = false;
            }

            if (!vm.extensionStatus['new'] && !vm.extensionStatus['extended'] && !vm.extensionStatus['notExtended'])
                vm.allExtensionStatuses = true;

            vm.reloadTable();
        }

        vm.toggleSidebar = (show) => {

            if (show !== vm.showSidebar) {
                vm.reload();
                vm.showSidebar = show;

                var selectedExtensions = {};
                angular.forEach(vm.extensions, extension => {
                    if (extension.Selected) selectedExtensions[extension.ShortName] = true;
                });

                services.session.cloneToSession('extension-report', vm.standard.DataStandardId, {
                    entityType: vm.entityType,
                    extensionStatus: vm.extensionStatus,
                    extension: selectedExtensions,
                    showSidebar: vm.showSidebar
                });

                if (show && vm.selected && vm.selected.Extension) {
                    var index = 0;
                    angular.forEach(vm.extensions, extension => {
                        if ((vm.allExtensions || extension.Selected) && vm.selected.Extension.ShortName >= extension.ShortName) index++;
                    });

                    vm.tableScroll((index - 1) * 86);
                }
            }
        }

        vm.createTable = (row, index, toggleSidebar) => {

            if (vm.table.Index != null && vm.table.Index === index) {
                vm.createTable(vm.total);
            } else {
                vm.selected = null;
                vm.extensionDetails = null;

                vm.table.Data = [[]];
                vm.table.Labels = [];
                vm.table.Colors = [
                    {
                        strokeColor: [],
                        fillColor: [],
                        highlightFill: [],
                        highlightStroke: []
                    }],

                    vm.table.Title = row.ItemName;
                vm.table.Index = index;

                angular.forEach(vm.extensions, (extension) => {
                    if (extension.Selected || vm.allExtensions) {

                        vm.table.Labels.push(extension.ShortName);
                        vm.table.Data[0].push(row[extension.ShortName]);

                        vm.table.Colors[0].strokeColor.push(extension.Color.Header);
                        vm.table.Colors[0].fillColor.push(extension.Color.Cell);
                        vm.table.Colors[0].highlightFill.push(extension.Color.Hover);
                        vm.table.Colors[0].highlightStroke.push(extension.Color.Selected);
                    }
                });
            }

            vm.toggleSidebar(toggleSidebar === undefined ? vm.showSidebar : toggleSidebar);
        }

        vm.reloadTable = (reloadTable) => {
            if (reloadTable === undefined) reloadTable = true;

            var selectedExtensions = {};
            var setAllExtensions = true;
            angular.forEach(vm.extensions, extension => {
                if (vm.allExtensions) extension.Selected = false;

                if (extension.Selected) {
                    setAllExtensions = false;
                    selectedExtensions[extension.ShortName] = true;
                }

                vm.total[extension.ShortName] = 0;
            });

            vm.allExtensions = setAllExtensions;

            services.session.cloneToSession('extension-report', vm.standard.DataStandardId, {
                entityType: vm.entityType,
                extensionStatus: vm.extensionStatus,
                extension: selectedExtensions,
                showSidebar: vm.showSidebar
            });

            vm.filteredReport = [];
            vm.total.Total = 0;
            angular.forEach(vm.report, row => {

                var showRow = false;
                row.Total = 0;

                if (vm.parent) {
                    if (vm.containsAssociations || vm.containsDescriptors) {
                        if (vm.entityType['entity']) {
                            if (!(row.ItemName.toLowerCase().endsWith('association') ||
                                row.ItemName.toLowerCase().endsWith('associations') ||
                                row.ItemName.toLowerCase().endsWith('descriptors') ||
                                row.ItemName.toLowerCase().endsWith('descriptor'))) showRow = true;
                        }

                        if (vm.entityType['descriptor']) {
                            if (row.ItemName.toLowerCase().endsWith('descriptors') ||
                                row.ItemName.toLowerCase().endsWith('descriptor')) showRow = true;
                        }

                        if (vm.entityType['association']) {
                            if (row.ItemName.toLowerCase().endsWith('associations') ||
                                row.ItemName.toLowerCase().endsWith('association')) showRow = true;
                        }
                    } else {
                        showRow = true;
                    }

                    if (vm.allEntityTypes) showRow = true;

                    if (showRow) {
                        showRow = false;
                        if (vm.extensionStatus[row.Type]) showRow = true;
                        else if (vm.allExtensionStatuses) showRow = true;
                    }
                } else {
                    showRow = true;
                }

                if (showRow) {
                    showRow = false;
                    angular.forEach(vm.extensions,
                        extension => {
                            if (vm.allExtensions) extension.Selected = false;

                            if (vm.allExtensions || (extension.Selected && row[extension.ShortName])) {
                                showRow = true;
                                row.Total += row[extension.ShortName];
                                vm.total[extension.ShortName] += row[extension.ShortName];
                            } else if (vm.extensionStatus['notExtended'] && !row.ShortName) showRow = true;
                        });
                }

                if (showRow) {
                    vm.total.Total += row.Total;
                    vm.filteredReport.push(row);
                }
            });

            vm.createTable(vm.total);

            if(reloadTable) vm.reload(true);
        }
        vm.reloadTable(false);

        vm.downloadReport = () => {
            return repositories.dataStandard.extensions.downloadableReport($stateParams.dataStandardId).then(() => {

            }, error => {
                services.logger.error("Error loading report", error);
            });
        }
    }]);
