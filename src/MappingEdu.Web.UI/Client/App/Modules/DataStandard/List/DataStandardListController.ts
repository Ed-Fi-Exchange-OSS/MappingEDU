// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

//
// Module app.modules.data-standard.list
//

var m = angular.module('app.modules.data-standard.list', []);

// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.data-standard.list', {
            url: '/list',
            data: {
                title: 'Data Standard List'
            },
            views: {
                'content@': {
                    templateUrl: `${settings.moduleBaseUri}/dataStandard/list/dataStandardListView.tpl.html`,
                    controller: 'app.modules.data-standard.list',
                    controllerAs: 'dataStandardListViewModel'
                }
            },
            resolve: {
                standards: ['repositories', (repositories: IRepositories) => {
                    return repositories.dataStandard.getAll();
                }],
                me: ['services', (services) => {
                    return services.profile.me();
                }]
            }
        });
}]);


// ****************************************************************************
// Controller app.modules.data-standard.list
//

m.controller('app.modules.data-standard.list', ['services', 'repositories', 'enumerations', 'standards', 'me',
    function (services: IServices, repositories: IRepositories, enumerations: IEnumerations, standards, me) {

    services.logger.debug('Loaded controller app.modules.data-standard.list');

    var vm = this;
    vm.standards = services.underscore.sortBy(standards, 'UserUpdateDate').reverse();
    vm.me = me;
    vm.dtOptions = services.datatables.optionsBuilder.newOptions()
        .withOption('aaSorting', [[0, vm.me.Roles[0] === 'guest' ? 'asc' : 'desc']]);

    vm.editStandard = (standard) => {
        services.state.go('app.data-standard.edit.groups', { dataStandardId: standard.DataStandardId });
    }

    vm.getAccess = (standard) => {
        var html = '';
        if (standard.Role > 0) {
            html += enumerations.UserAccess[enumerations.UserAccess.map(x => x.Id).indexOf(standard.Role)].DisplayText;
            if (standard.IsPublic) html += ', ';
        }
        if (standard.IsPublic) html += 'Public';
        return html;
    }
}]);
