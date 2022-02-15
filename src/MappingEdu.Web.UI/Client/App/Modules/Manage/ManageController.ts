// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.manage
//

var m = angular.module('app.modules.manage', [
    'app.modules.manage.logs',
    'app.modules.manage.organizations',
    'app.modules.manage.projects',
    'app.modules.manage.standards',
    'app.modules.manage.system-constants',
    'app.modules.manage.users']);


// ****************************************************************************
// Configure 
//

m.config(['$urlRouterProvider', '$stateProvider', 'settings', ($urlRouterProvider: ng.ui.IUrlRouterProvider, $stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $urlRouterProvider.when('/manage', '/manage/users');

    $stateProvider
        .state('app.manage', {
            url: '/manage',
            data: {
                roles: ['admin'],
                title: 'Manage'
            },
            views: {
                'content@': {
                    templateUrl: `${settings.moduleBaseUri}/manage/manageView.tpl.html`,
                    controller: 'app.modules.manage',
                    controllerAs: 'manageViewModel'
                }
            }
        });

}]);


// ****************************************************************************
// Controller app.modules.manage
//

m.controller('app.modules.manage', ['services', function (services: IServices) {

    var vm = this;

    vm.tabs = [
        { link: 'app.manage.users', label: 'Users' },
        { link: 'app.manage.organizations', label: 'Organizations' },
        { link: 'app.manage.projects', label: 'Projects' },
        { link: 'app.manage.standards', label: 'Standards' },
        { link: 'app.manage.system-constants', label: 'System Config' },
        { link: 'app.manage.logs', label: 'Logs' }
    
    ];

    vm.onPage = sref => (services.state.current.name === sref);
    vm.showTabs = () => services.underscore.some(<Array<any>>vm.tabs, tab => services.state.is(tab.link));

}]);
