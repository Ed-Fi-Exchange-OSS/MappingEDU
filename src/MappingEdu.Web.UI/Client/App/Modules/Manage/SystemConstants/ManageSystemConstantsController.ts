// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.manage.system-constants
//

var m = angular.module('app.modules.manage.system-constants', ['app.modules.manage.system-constants.constant-display']);


// ****************************************************************************
// Configure 
//

m.config(['$urlRouterProvider', '$stateProvider', 'settings', ($urlRouterProvider: ng.ui.IUrlRouterProvider, $stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $stateProvider
        .state('app.manage.system-constants', {
            url: '/constants',
            data: {
                title: 'Manage System Constants'
            },
            templateUrl: `${settings.moduleBaseUri}/Manage/SystemConstants/ManageSystemConstantsView.tpl.html`,
            controller: 'app.modules.manage.system-constants',
            controllerAs: 'manageSystemConstantsView',
            resolve: {
                constants: ['repositories', (repositories: IRepositories) => {
                    return repositories.systemConstant.getAll();
                }]
            }
        });
}]);


// ****************************************************************************
// Controler app.modules.manage.system-constants
//

m.controller('app.modules.manage.system-constants', ['constants',
    function (constants) {

        var vm = this;
        vm.constants = constants;
    }
]);
