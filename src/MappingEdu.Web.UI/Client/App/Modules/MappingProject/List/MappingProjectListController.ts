// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// list
// Module app.modules.mapping-project.create
//

var m = angular.module('app.modules.mapping-project.list', []);

// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.mapping-project.list', {
            url: '/list',
            data: {
                title: 'Mapping Project List'
            },
            views: {
                'content@': {
                    templateUrl: `${settings.moduleBaseUri}/mappingProject/list/mappingProjectListView.tpl.html`,
                    controller: 'app.modules.mapping-project.list',
                    controllerAs: 'mappingProjectListViewModel'
                }
            },
            resolve: {
                projects: ['repositories', (repositories: IRepositories) => {
                    return repositories.mappingProject.getAll();
                }],
                me: ['services', (services) => {
                    return services.profile.me();
                }]
            }
        });
}]);


// ****************************************************************************
// Controller app.modules.mapping-project.list
//

m.controller('app.modules.mapping-project.list', ['services', 'repositories', 'enumerations', 'projects', 'me',
    function (services: IServices, repositories: IRepositories, enumerations: IEnumerations, projects, me) {

    services.logger.debug('Loaded controller app.modules.mapping-project.list');

    var vm = this;
    vm.projects = services.underscore.sortBy(projects, 'UserUpdateDate').reverse();
    vm.me = me;

    vm.dtOptions = services.datatables.optionsBuilder.newOptions()
        .withOption('aaSorting', [[0, vm.me.Roles[0] === 'guest' ? 'asc' : 'desc']]);


    vm.editProject = (project) => {
        if (vm.me.IsGuest)
            services.state.go('app.mapping-project.detail.review-queue', { id: project.MappingProjectId });
        else
            services.state.go('app.mapping-project.detail.dashboard', { id: project.MappingProjectId });
    }

    vm.editStandard = (standard) => {
        services.state.go('app.data-standard.edit', { dataStandardId: standard.DataStandardId });
    }

    vm.getAccess = (project) => {
        var html = '';
        if (project.Role > 0) {
            html += enumerations.UserAccess[enumerations.UserAccess.map(x => x.Id).indexOf(project.Role)].DisplayText;
            if (project.IsPublic) html += ', ';
        }
        if (project.IsPublic) html += 'Public';
        return html;
    }

}]);
