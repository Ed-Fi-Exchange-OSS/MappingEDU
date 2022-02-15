// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.detail.notifications
//

var m = angular.module('app.modules.mapping-project.detail.notifications', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.mapping-project.detail.notifications', { 
            url: '/notifications',
            data: {
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Edit')].Id,
                roles: ['user'],
                title: 'Mapping Project Notifications'
            },
            templateUrl: `${settings.moduleBaseUri}/mappingProject/detail/notifications/mappingProjectNotificationView.tpl.html`,
            controller: 'app.modules.mapping-project.detail.notifications',
            controllerAs: 'mappingProjectNotificationsViewModel',
            resolve: {
                
            }
        });
}]);


// ****************************************************************************
// Controller app.modules.mapping-project.detail.notifications
//

m.controller('app.modules.mapping-project.detail.notifications', ['$', '$scope', '$stateParams', 'repositories', 'services',
    function ($, $scope, $stateParams: any, repositories: IRepositories, services: IServices) {

        services.logger.debug('Loaded controller app.modules.mapping-project.detail.notifications');
        $scope.$parent.mappingProjectDetailViewModel.setTitle('NOTIFICATIONS');


        var vm = this;
        vm.id = $stateParams.id;

        vm.table = $('#notificationsTable').DataTable(
            {
                serverSide: true,
                processing: true,
                ajax: {
                    url: `api/Notifications/paging?mappingProjectId=` + vm.id,
                    type: 'POST',
                    data: (data) => {
                        vm.currentFilter = data;
                        return data;
                    },
                    dataSrc: (data) => {
                        angular.forEach(data.data, (item, index) => {
                            item.row = index;
                        });
                        vm.notifications = data.data;
                        return data.data;
                    }
                },
                createdRow(row, data) {
                    services.compile(angular.element(row).contents())($scope);
                    if (!data.HasSeen) {
                        angular.element(row).addClass('unread-notification');
                    }
                },
                columns: [
                    {
                        data: 'PathSegments'
                    },
                    {
                        data: 'Notification'
                    },
                    {
                        data: 'NotificationDate'
                    },
                    {
                        sortable: false
                    }
                ],
                columnDefs: [
                    {
                        targets: 0,
                        render(segments, type, row) {

                            var element = row.Element;

                            var html = '<div class="expand-container">';
                            html += '<ma-element-path context-id="mappingProjectNotificationsViewModel.id"';
                            html += ' segments="mappingProjectNotificationsViewModel.notifications[';
                            html += row.row;
                            html += '].PathSegments" ng-click="mappingProjectReviewQueueViewModel.logSearchResults(';
                            html += 'mappingProjectReviewQueueViewModel.reviewQueue.ReviewItems[';
                            html += row.row;
                            html += '])"></ma-element-path>';
                            html += '<i class="fa fa-caret-right separator"></i>';
                            html += '</span>';
                            html += '<div style="display: inline-block"><a href="';
                            html += vm.elementsHref(element);
                            html += '"';
                            html += ' data-toggle="popover"';
                            html += ' data-trigger="hover" data-content="';
                            html += (element.Definition == null ? '' : element.Definition);
                            html += '" class="';
                            html += (element.IsExtended) ? 'standard-c">' : 'standard-a">';
                            if (element.IsExtended) html += '<i class="fa fa-extended"></i> ';
                            html += element.Name;
                            html += '</a></div>';
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
                        render(notification, type, row) {
                            if (!notification)
                                return '';

                            var html = angular.copy(notification);
                            html = html.split('[~').join('<b>@');
                            html = html.split(']').join('</b>');
                            html = html.split('\n').join('<br/>');

                            return html;
                        }
                    },
                    {
                        targets: 2,
                        render(date, type, row) {
                            var html = services.filter('date')(date, 'M/d/yyyy h:mm a');

                            return html;
                        }
                    },
                    {
                        targets: 3,
                        render(date, type, row) {
                            var html = '<div class="text-center">';
                            html += '<button class="btn btn-delete" ng-click="mappingProjectNotificationsViewModel.dismiss(mappingProjectNotificationsViewModel.notifications[';
                            html += row.row;
                            html += '].UserNotificationIds)">';
                            html += '<i class="fa"></i>';
                            html += '</button>';
                            html += '</div>';
                            return html;
                        }
                    }
                ],
                order: [[2, 'desc']]
            });


        $('#notificationsTable').on('draw.dt', () => {

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


        vm.elementsHref = element => {
            vm.currentFilter.fromNotifications = true;
            return services.state.href('app.element.detail.mapping', {
                mappingProjectId: vm.id,
                elementId: element.SystemItemId,
                elementListFilter: JSON.stringify(vm.currentFilter)
            });
        }

        vm.dismiss = (ids) => {
            repositories.notifications.dismissMany(ids).then(() => {
                repositories.notifications.getUnread(vm.id).then(data => {
                    $scope.$parent.mappingProjectDetailViewModel.unreadNotificationsCount = data;
                });
                services.logger.success('Dismissed notification');
                vm.table.draw();
            }, error => {
                services.logger.error('Error dismissing notification', error);
            });
        }
    }
]);
