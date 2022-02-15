// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.manage.organizations
//

var m = angular.module('app.modules.manage.projects', []);


// ****************************************************************************
// Configure 
//

m.config(['$urlRouterProvider', '$stateProvider', 'settings', ($urlRouterProvider: ng.ui.IUrlRouterProvider, $stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $stateProvider
        .state('app.manage.projects', {
            url: '/projects',
            data: {
                title: 'Manage Projects'
            },
            templateUrl: `${settings.moduleBaseUri}/Manage/Projects/ProjectsManageView.tpl.html`,
            controller: 'app.modules.manage.projects',
            controllerAs: 'manageProjectsViewModel',
            resolve: {
                projects: ['repositories', (repositories: IRepositories) => {
                    return repositories.mappingProject.getAll();
                }]
            }
        });
}]);


// ****************************************************************************
// Controler app.modules.manage.projects
//

m.controller('app.modules.manage.projects', ['projects', 'repositories', 'services',
    function (projects, repositories: IRepositories, services: IServices) {

        services.logger.debug('Loaded controller app.modules.manage.projects');
    
        var vm = this;
        vm.projects = projects;
        vm.filter = 'All';

        vm.projectHref = (project) => {
            return services.state.href('app.mapping-project.detail.dashboard', { id: project.MappingProjectId });
        }

        vm.standardHref = (standard) => {
            return services.state.href('app.data-standard.edit.groups', { dataStandardId: standard.DataStandardId });
        }

        vm.delete = (project, index) => {
            repositories.mappingProject.remove(project.MappingProjectId).then(() => {
                services.logger.success('Removed mapping project.');
                vm.projects.splice(index, 1);
            }, error => {
                services.logger.error('Error removing mapping project.', error.data);
            });
        }

        vm.filterBy = (filter) => {
            vm.loading = true;
            if (filter === 'Orphaned') {
                repositories.mappingProject.getAllOrphaned().then(data => {
                    vm.projects = data;
                    vm.filter = 'Orphaned';
                }).finally(() => vm.loading = false);
            } else if (filter === 'Public') {
                repositories.mappingProject.getAllPublic().then(data => {
                    vm.projects = data;
                    vm.filter = 'Public';
                }).finally(() => vm.loading = false);
            } else {
                repositories.mappingProject.getAll().then(data => {
                    vm.projects = data;
                    vm.filter = 'All';
                }).finally(() => vm.loading = false);
            }
        }

    }
]);
