// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.manage.users
//

var m = angular.module('app.modules.manage.users', [
    'app.modules.manage.users.create',
    'app.modules.manage.users.edit',
    'app.modules.manage.users.directives.user-form',
]);


// ****************************************************************************
// Configure 
//

m.config(['$urlRouterProvider', '$stateProvider', 'settings', ($urlRouterProvider: ng.ui.IUrlRouterProvider, $stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $stateProvider
        .state('app.manage.users', {
            url: '/users',
            data: {
                title: 'Manage Users'
            },
            templateUrl: `${settings.moduleBaseUri}/Manage/Users/UsersManageView.tpl.html`,
            controller: 'app.modules.manage.users',
            controllerAs: 'manageUsersViewModel',
            resolve: {
                me: ['services', (services: IServices) => {
                    return services.profile.me();
                }]
            }
        });
}]);


// ****************************************************************************
// Controler app.modules.manage.users
//

m.controller('app.modules.manage.users', ['$scope', 'repositories', 'services', 'me',
    function($scope: any, repositories: IRepositories, services: IServices, me) {

        services.logger.debug('Loaded controller app.modules.manage.users');

        var vm = this;
        vm.me = me;

        vm.loadTable = () => {
            var datatableElement = <any>$('#users-table');
            vm.table = datatableElement.DataTable({
                serverSide: true,
                pagging: true,
                ajax: {
                    url: 'api/users/paging',
                    type: 'POST'
                },
                createdRow(row) {
                    services.compile(angular.element(row).contents())($scope);
                },
                columns: [
                    {
                        data: ''
                    },
                    {
                        data: 'Email'
                    },
                    {
                        data: ''
                    },
                    {
                        data: ''
                    },
                    {
                        data: '',
                        sortable: false
                    }
                ],
                columnDefs: [
                    {
                        targets: 0,
                        render(model, type, user, meta) {
                            return `<a href="${services.state.href('app.manage.users.edit', { id: user.Id })}">${user.FirstName} ${user.LastName}</a>`;
                        }
                    },
                    {
                        targets: 1,
                        render(email, type, user, meta) {
                            return email;
                        }
                    },
                    {
                        targets: 2,
                        render(model, type, user, meta) {
                            var html = '';
                            if (user.Organizations) {
                                angular.forEach(user.Organizations, (organization, index) => {
                                    if (index > 0) html += ', ';
                                    html += organization.Name;
                                });   
                            }
                            return html;
                        }
                    },
                    {
                        targets: 3,
                        render(model, type, user, meta) {
                            if (user.IsAdministrator) return 'Admin';
                            else if (user.Roles[0] === 'user') return 'User';
                            else if (user.Roles[0] === 'guest') return 'Guest';
                            else return 'User';
                        }
                    },
                    {
                        targets: 4,
                        render(model, type, user, meta) {
                            var html = '<div class="text-center" ';

                            if (user.Roles[0] === 'guest') html += 'uib-tooltip="Cannot delete Guest Account" tooltip-append-to-body="true">';
                            else if (vm.me.Id === user.Id) html += 'uib-tooltip="Cannot delete yourself." tooltip-append-to-body="true">';
                            else html += '>';

                            html += `<button type="button" class="btn btn-delete" ma-confirm-action="manageUsersViewModel.delete('${user.Id}')" `;
                            if (user.Roles[0] === 'guest' ||vm.me.Id === user.Id) html += 'disabled>';
                            else html += '>';

                            html += '<i class="fa"></i>';
                            html += '</button>';
                            html += '</div>';
                            return html;
                        }
                    },
                ]
            });
        }

        vm.loadTable();

        vm.userHref = (user: IUser) => {
            return services.state.href('app.manage.users.edit', { id: user.Id });
        }

        vm.delete = (userId) => {
            return repositories.users.remove(userId).then((data) => {
                vm.table.ajax.reload();
                services.logger.success('Deleted user.');
            }, (error) => {
                services.logger.error(error.data.ExceptionMessage, error.data);
            });
        }

    }
]);