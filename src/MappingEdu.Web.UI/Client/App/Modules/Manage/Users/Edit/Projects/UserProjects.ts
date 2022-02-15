// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.manage.users.projects
//

var m = angular.module('app.modules.manage.users.edit.projects', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $stateProvider
        .state('app.manage.users.edit.projects', {
            url: '/projects',
            data: {
                title: 'User Projects'
            },
            views: {
                'user-info': {
                    templateUrl: `${settings.moduleBaseUri}/Manage/Users/Edit/Projects/UserProjects.tpl.html`,
                    controller: 'app.modules.manage.users.edit.user-projects',
                    controllerAs: 'userProjectsViewModel'
                },
                'guest-info': {
                    templateUrl: `${settings.moduleBaseUri}/Manage/Users/Edit/Projects/PublicUserProjects.tpl.html`,
                    controller: 'app.modules.manage.users.edit.guest-projects',
                    controllerAs: 'userProjectsViewModel'
                }
            },
            resolve: {
                projects: ['repositories', (repositories: IRepositories) => {
                    return repositories.mappingProject.getAll();
                }],
                userProjects: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.users.getProjects($stateParams.id);
                }]
            }
        });
}]);

// ****************************************************************************
// Controller app.modules.manage.users.edit.user-projects
//

m.controller('app.modules.manage.users.edit.user-projects', ['$scope', '$stateParams', 'projects', 'userProjects', 'repositories', 'services',
    function ($scope, $stateParams, projects, userProjects, repositories: IRepositories, services: IServices) {

        services.logger.debug('Loading app.modules.manage.users.edit.projects');

        $scope.project = {};

        var vm = this;
        vm.userProjects = userProjects;
        vm.projects = projects;

        vm.permissonOptions = [
            {
                value: 1,
                label: 'Can View'
            },
            {
                value: 2,
                label: 'Can Edit'
            },
            {
                value: 99,
                label: 'Owner'
            }
        ];

        vm.dtOptions = services.datatables.optionsBuilder.newOptions()
            .withOption('language', {
                emptyTable: 'This user has not been shared on any projects'
            })
            .withOption('aoColumns', [
                null, null, { "sSortDataType": 'dom-select' }, null
            ]);

        vm.getUserProjects = () => {
            repositories.users.getProjects($stateParams.id).then((projects: Array<IUserMappingProject>) => {
                vm.userProjects = angular.copy(projects);
            }, error => {
                services.logger.error('Error loading projects shared with the user.', error.data);
            });
        }

        vm.addUserToProject = (email, project) => {
            var user: IShareProjectToUser = { Email: email, Role: project.Role };
            return repositories.mappingProject.addUser(project.Id, user).then(() => {
                services.logger.success('Added user to project.');
                vm.getUserProjects();
                project.Id = null;
                project.Role = null;
            }, error => {
                services.logger.error('Error updating access level.', error.data);
            });
        }

        vm.updateProjectRole = (email, project) => {
            var user: IShareProjectToUser = { Email: email, Role: project.Role };
            repositories.mappingProject.addUser(project.Id, user).then(() => {
                services.logger.success('Updated access level.');
            }, error => {
                services.logger.error('Error updating access level.', error.data);
            });
        }

        vm.removeProjectRole = (project, user, index) => {
            repositories.mappingProject.removeUser(project.Id, user.Id).then(() => {
                vm.userProjects.splice(index, 1);
                services.logger.success('Removed access level.');
            }, error => {
                services.logger.error('Error removing access level.', error.data);
            });
        }
    }
]);

// ****************************************************************************
// Controller app.modules.manage.users.edit.guest-projects
//

m.controller('app.modules.manage.users.edit.guest-projects', ['$scope', '$stateParams', 'projects', 'repositories', 'services',
    function ($scope, $stateParams, projects, repositories: IRepositories, services: IServices) {

        services.logger.debug('Loading app.modules.manage.users.edit.guest-projects');

        $scope.selected = {};

        var vm = this;
        vm.projects = projects;
        vm.instance = {};

        vm.dtOptions = services.datatables.optionsBuilder.newOptions()
            .withOption('language', {
                emptyTable: 'This user has not been shared on any projects'
            });

        vm.togglePublic = (project) => {
            repositories.mappingProject.togglePublic(project.MappingProjectId).then(() => {
                project.IsPublic = !project.IsPublic;
                $scope.selected = {};
                vm.instance.rerender();
                services.logger.success('Changed public status.');
            }, error => {
                services.logger.error('Error changing public status.', error);
            });
        }
    }
]);