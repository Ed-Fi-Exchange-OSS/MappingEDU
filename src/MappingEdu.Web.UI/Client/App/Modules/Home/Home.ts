// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.home
//

var m = angular.module('app.modules.home', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $stateProvider
        .state('app.home', {
            url: '/',
            data: {
                roles: ['user', 'guest'],
                title: 'MappingEDU'
            },
            views: {
                'content@': {
                    templateUrl: `${settings.moduleBaseUri}/home/home.tpl.html`,
                    controller: 'app.modules.home',
                    controllerAs: 'homeViewModel'
                }
            },
            resolve: {
                home: ['repositories', (repositories: IRepositories) => {
                    return repositories.home.getHomePage();
                }],
                me: ['services', (services: IServices) => {
                    return services.profile.me();
                }]
            }
        });
    }
]);


// ****************************************************************************
// Controller app.modules.home
//

m.controller('app.modules.home', ['filterFilter', 'modals', 'repositories', 'services', 'home', 'me',
    function (filterFilter, modals: IModals, repositories: IRepositories, services: IServices, home, me) {

        services.logger.debug('Loaded controller app.modules.home');

        var vm = this;
        if (me.FirstName === null || me.FirstName === '') modals.welcome();

        vm.me = me;
        vm.dataStandards = home.DataStandardList;
        vm.mappingProjects = home.MappingProjectList;

        vm.mappingProjects = services.underscore.sortBy(vm.mappingProjects, 'UserUpdateDate');
        vm.mappingProjects = services.underscore.sortBy(vm.mappingProjects, 'Flagged').reverse();

        vm.activeProjects = filterFilter(vm.mappingProjects, { ProjectStatusTypeName: 'Active' });

        vm.dataStandards = services.underscore.sortBy(vm.dataStandards, 'UserUpdateDate');
        vm.dataStandards = services.underscore.sortBy(vm.dataStandards, 'Flagged').reverse();

        vm.emptyDate = '0001-01-01T00:00:00';

        vm.listVisibleLength = 5;
        vm.mappingProjectListLength = vm.listVisibleLength;
        vm.allDataStandardsVisible = false;
        vm.allMappingProjectsVisible = false;
        vm.allActiveMappingProjectsVisible = false;

        vm.standardHref = (dataStandard: any) => {
            return services.state.href('app.data-standard.edit.groups', { dataStandardId: dataStandard.DataStandardId });
        }

        vm.resumeSref = (mappingProject: any) => {
            if (vm.me.IsGuest)
                return `app.mapping-project.detail.review-queue({ id: '${mappingProject.MappingProjectId}'})`;
            else
                return `app.mapping-project.detail.dashboard({ id: '${mappingProject.MappingProjectId}'})`;
        }

        vm.notificationsSref = (mappingProject: any) => {
            return `app.mapping-project.detail.notifications({ id: '${mappingProject.MappingProjectId}'})`;
        }

        vm.viewMappingProject = (mappingProject: any) => {
            if(vm.me.IsGuest)
                services.state.go('app.mapping-project.detail.review-queue', { id: mappingProject.MappingProjectId });
            else
                services.state.go('app.mapping-project.detail.dashboard',  { id: mappingProject.MappingProjectId });
        }

        vm.viewMappingProjectNotifications = (mappingProject: any) => {
            services.state.go('app.mapping-project.detail.notifications', { id: mappingProject.MappingProjectId });
        }        
    }
]);
