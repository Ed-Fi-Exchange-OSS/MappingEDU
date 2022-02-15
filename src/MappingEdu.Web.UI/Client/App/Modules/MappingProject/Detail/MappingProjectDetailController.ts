// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.detail
//

var m = angular.module('app.modules.mapping-project.detail', [
    'app.modules.mapping-project.detail.actions',
    'app.modules.mapping-project.detail.admin',
    'app.modules.mapping-project.detail.dashboard',
    'app.modules.mapping-project.detail.filters',
    'app.modules.mapping-project.detail.info',
    'app.modules.mapping-project.detail.mapping-summary',
    'app.modules.mapping-project.detail.notifications',
    'app.modules.mapping-project.detail.reports',
    'app.modules.mapping-project.detail.review-queue',
    'app.modules.mapping-project.detail.settings',
    'app.modules.mapping-project.detail.share',
    'app.modules.mapping-project.detail.status']);

// ****************************************************************************
// Configure 
//

m.config(['$urlRouterProvider', '$stateProvider', 'settings', ($urlRouterProvider: ng.ui.IUrlRouterProvider, $stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $urlRouterProvider.when('/mapping-project/detail/:id', '/mapping-project/detail/:id/info');

    $stateProvider
        .state('app.mapping-project.detail', { //mappingProject
            url: '/detail/:id',
            data: {
                title: 'Standard and Element Group Detail'
            },
            views: {
                'content@': {
                    templateUrl: `${settings.moduleBaseUri}/mappingProject/detail/mappingProjectDetailView.tpl.html`,
                    controller: 'app.modules.mapping-project.detail',
                    controllerAs: 'mappingProjectDetailViewModel'
                }
            },
            resolve: {
                mappingProject: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.mappingProject.find($stateParams.id);
                }],
                unreadNotificationsCount: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.notifications.getUnread($stateParams.id);
                }]
            }
        });
}]);


// ****************************************************************************
// Controller app.modules.mapping-project.detail
//

m.controller('app.modules.mapping-project.detail', [ '$scope', '$stateParams', 'repositories', 'services', 'enumerations', 'mappingProject', 'unreadNotificationsCount',
    function ($scope, $stateParams, repositories: IRepositories, services: IServices, enumerations: IEnumerations, mappingProject, unreadNotificationsCount) {

        services.logger.debug('Loaded controller app.modules.mapping-project.detail');

        var vm = this;
        vm.unreadNotificationsCount = unreadNotificationsCount;

        vm.id = $stateParams.id;
        vm.unreadNotificationsCount = unreadNotificationsCount;
        vm.currentState = services.state.current.name;
        vm.mappingProject = mappingProject;

        vm.setTitle = (title) => { vm.pageTitle = `MAPPING PROJECT ${title}`;}

        vm.getMappingProjectDetails = () => {
            vm.loading = true;
            repositories.mappingProject.find(vm.id)
                .then(data => {
                    vm.mappingProject = data;
                    {}
                }, error => {
                    services.logger.error('Error loading mapping project.', error);
                })
                .catch(error => {
                    services.errors.handleErrors(error, vm);
                })
                .finally(() => {
                    vm.loading = false;
                });
        }

        vm.load = () => {
            vm.getMappingProjectDetails();
        }

        $scope.$on('workflow-status-updated', () => {
            vm.load();
        });

        vm.onPage = sref => (services.state.current.name === sref);

        services.profile.me().then((data) => {
            vm.me = data;
            var index = data.Projects.map(x => x.Id).indexOf(vm.id);
            if (index >= 0) {
                var project = data.Projects[index];
                if (project.Role > 1 || data.IsAdministrator) {
                    vm.tabs = [
                        { link: 'app.mapping-project.detail.info', label: 'Info' },
                        { link: 'app.mapping-project.detail.dashboard', label: 'Dashboard' },
                        { link: 'app.mapping-project.detail.mapping-summary', label: 'Mapping Summary' },
                        { link: 'app.mapping-project.detail.filters', label: 'Filters' },
                        { link: 'app.mapping-project.detail.reports', label: 'Reports' },
                        { link: 'app.mapping-project.detail.actions', label: 'Actions' },
                        { link: 'app.mapping-project.detail.notifications', label: 'Notifications'},
                        { link: 'app.mapping-project.detail.share', label: 'Share' },
                        { link: 'app.mapping-project.detail.settings', label: 'Settings' }
                    ];
                } else if (project.Role == 1) {
                    vm.tabs = [
                        { link: 'app.mapping-project.detail.info', label: 'Info' },
                        { link: 'app.mapping-project.detail.dashboard', label: 'Dashboard' },
                        { link: 'app.mapping-project.detail.mapping-summary', label: 'Mapping Summary' },
                        { link: 'app.mapping-project.detail.filters', label: 'Filters' },
                        { link: 'app.mapping-project.detail.reports', label: 'Reports' },
                        { link: 'app.mapping-project.detail.actions', label: 'Actions' }
                    ];
                }
            } else if (data.IsAdministrator) {
                vm.tabs = [
                    { link: 'app.mapping-project.detail.info', label: 'Info' },
                    { link: 'app.mapping-project.detail.dashboard', label: 'Dashboard' },
                    { link: 'app.mapping-project.detail.mapping-summary', label: 'Mapping Summary' },
                    { link: 'app.mapping-project.detail.filters', label: 'Filters' },
                    { link: 'app.mapping-project.detail.reports', label: 'Reports' },
                    { link: 'app.mapping-project.detail.actions', label: 'Actions' },
                    { link: 'app.mapping-project.detail.notifications', label: 'Notifications' },
                    { link: 'app.mapping-project.detail.share', label: 'Share' },
                    { link: 'app.mapping-project.detail.settings', label: 'Settings' }
                ];
            } else if (!data.IsGuest && vm.mappingProject.IsPublic) {
                vm.tabs = [
                    { link: 'app.mapping-project.detail.info', label: 'Info' },
                    { link: 'app.mapping-project.detail.dashboard', label: 'Dashboard' },
                    { link: 'app.mapping-project.detail.mapping-summary', label: 'Mapping Summary' },
                    { link: 'app.mapping-project.detail.filters', label: 'Filters' },
                    { link: 'app.mapping-project.detail.reports', label: 'Reports' },
                    { link: 'app.mapping-project.detail.actions', label: 'Actions' }
                ];
            } else if (data.IsGuest && vm.mappingProject.IsPublic) {
                vm.tabs = [
                    { link: 'app.mapping-project.detail.review-queue', label: 'Review Queue' },
                    { link: 'app.mapping-project.detail.reports', label: 'Reports' }
                ];
            }
            vm.showTabs = () => services.underscore.some(<Array<any>>vm.tabs, tab => services.state.is(tab.link));
        });

        angular.element(document.querySelector('#editProjectModal')).on('hidden.bs.modal', () => {
            vm.load();
        });

        vm.toggleStar = (mappingProject: any) => {
            mappingProject.disabled = true;
            repositories.mappingProject.toggleFlagged(mappingProject.MappingProjectId, vm.me.Id).then(() => {
            }, error => {
                mappingProject.Flagged = !mappingProject.Flagged;
                services.logger.error('Error updating favorites.');
            }).finally(() => {
                mappingProject.disabled = false;
            });
        }

        vm.goToDashboard = () => {
            services.state.go('app.mapping-project.detail.dashboard', { id: vm.mappingProject.MappingProjectId });
        }

        vm.goToStandard = (standard) => {
            services.state.go('app.data-standard.edit.groups', { dataStandardId: standard.DataStandardId });
        }

        vm.getAccess = (project) => {
            var html = '';
            if (project.Role > 0) {
                html += enumerations.UserAccess[enumerations.UserAccess.map(x => x.Id).indexOf(project.Role)].DisplayText;
            }
            if (vm.me && vm.me.IsAdministrator && project.Role > 0) html += ', ';
            if (vm.me && vm.me.IsAdministrator) html += 'Admin';
            if (project.IsPublic && (project.Role > 0 || vm.me.IsAdministrator)) html += ', ';
            if (project.IsPublic) html += 'Public';
            return html;
        }
    }
]);
