// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.manage.logs
//

var m = angular.module('app.modules.manage.logs', [
    'app.modules.manage.logs.clear',
    'app.modules.manage.logs.export',
    'app.modules.manage.logs.view'
]);


// ****************************************************************************
// Configure 
//

m.config(['$urlRouterProvider', '$stateProvider', 'settings', ($urlRouterProvider: ng.ui.IUrlRouterProvider, $stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $stateProvider
        .state('app.manage.logs', {
            url: '/logs',
            data: {
                title: 'Manage Standards'
            },
            templateUrl: `${settings.moduleBaseUri}/Manage/Logs/ManageLogsView.tpl.html`,
            controller: 'app.modules.manage.logs',
            controllerAs: 'manageLogsViewModel'
        });
}]);


// ****************************************************************************
// Controler app.modules.manage.logs
//

m.controller('app.modules.manage.logs', ['$', '$scope', '$filter', 'repositories', 'services', 'settings',
    function ($, $scope, $filter, repositories: IRepositories, services: IServices, settings: ISystemSettings) {

        var vm = this;

        vm.today = new Date();

        vm.levels = ['INFO', 'DEBUG', 'ERROR', 'WARN', 'FATAL'];

        vm.getLogs = () => {
            vm.endDate = new Date();
            vm.startDate = new Date();
            var m = vm.startDate.getMonth();
            vm.startDate.setMonth(vm.startDate.getMonth() - 1);

            // If still in same month, set date to last day of previous month
            if (vm.startDate.getMonth() === m) vm.startDate.setDate(0);

            vm.startDate.setHours(0, 0, 0);
            vm.endDate.setHours(0, 0, 0);

            vm.selectedLevels = {
                INFO: true,
                ERROR: true
            };

            vm.table = $('#logsTable').DataTable(
            {
                serverSide: true,
                processing: true,
                ajax: {
                    url: `${settings.apiBaseUri}/Logging/paging`,
                    type: 'POST',
                    data: (data) => {
                        data.Levels = [];
                        if (vm.selectedLevels) {
                            for (var key in vm.selectedLevels) {
                                if (vm.selectedLevels[key])
                                    data.Levels.push(key);
                            }
                        }

                        vm.allLevels = (data.Levels.length === 0);

                        data.StartDate = $filter('date')(vm.startDate, 'MM/d/yyyy h:mm:ss a');

                        //Adds 1 day to enddate
                        var endDate = new Date(vm.endDate);
                        endDate.setDate(endDate.getDate() + 1);

                        data.EndDate = $filter('date')(endDate, 'MM/d/yyyy h:mm:ss a');
                        if(data.search)
                            vm.search = data.search.value;

                        vm.model = data;
                        return data;
                    },
                    dataSrc: (data) => {
                        angular.forEach(data.data, (item, index) => {
                            item.row = index;
                        });
                        vm.logs = data.data;
                        return data.data;
                    }
                },
                createdRow(row) {
                    services.compile(angular.element(row).contents())($scope);
                },
                order: [3, 'desc'],
                columns: [
                    {
                        data: 'Level'
                    },
                    {
                        data: 'User'
                    },
                    {
                        data: 'Message'
                    },
                    {
                        data: 'Date'
                    }
                ],
                columnDefs: [
                    {
                        targets: 1,
                        render(user) {
                            return `<div style="max-width: 175px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;"">${user}</div>`;
                        }
                    },
                    {
                        targets: 2,
                        render(message) {
                            return `<div style="width: 600px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;">${message}</div>`;
                        }
                    },
                    {
                        targets: 3,
                        render(date) {
                            return $filter('date')(date, 'MMM dd, yyyy hh:mm a');
                        }
                    }
                ]
                });

            $('.dataTables_processing').html('<div class="loading-inner"><img src= "Client/Content/Images/Loading.gif" alt= "Loading">Loading...</div>');

            $('#logsTable tbody').on('click', 'tr', (event) => {
                var data = vm.table.row(event.currentTarget.rowIndex - 2).data();
                vm.currentRow = data;

                var model = {
                    backdrop: 'static',
                    templateUrl: `${settings.moduleBaseUri}/manage/logs/view/viewLog.tpl.html`,
                    controller: 'app.modules.manage.logs.view',
                    size: 'lg',
                    resolve: {
                        log: () => { return data; }
                    }
                }

                services.modal.open(model);
            });
        }

        vm.clearLogs = () => {
            var model = {
                backdrop: 'static',
                templateUrl: `${settings.moduleBaseUri}/manage/logs/clear/clearLogs.tpl.html`,
                controller: 'app.modules.manage.logs.clear'
            }

            var instance = services.modal.open(model);
            instance.result.then(() => {
                vm.redraw();
            });
        }

        vm.exportLogs = () => {
            var model = {
                backdrop: 'static',
                templateUrl: `${settings.moduleBaseUri}/manage/logs/export/exportLogs.tpl.html`,
                controller: 'app.modules.manage.logs.export',
                resolve: {
                    levels: () => vm.levels,
                    exportModel: () => {
                        vm.model.StartDate = new Date(vm.model.StartDate);
                        vm.model.EndDate = new Date(vm.endDate);
                        return vm.model;
                    }
                }
            }

            services.modal.open(model);
        }

        vm.redraw = () => {
            vm.table.draw();
        }

        vm.clearLevels = () => {
            vm.selectedLevels = {};
            vm.allLevels = true;
            vm.redraw();
        }
        services.timeout(() => { vm.getLogs() }, 1);
    }
]);
