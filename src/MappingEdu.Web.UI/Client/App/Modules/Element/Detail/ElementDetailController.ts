// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.element.detail
//

var m = angular.module('app.modules.element.detail', [
    'app.modules.element.detail.actions',
    'app.modules.element.detail.enumeration-details',
    'app.modules.element.detail.info',
    'app.modules.element.detail.mapping',
    'app.modules.element.detail.enumeration-usage'
]);



// ****************************************************************************
// Configure 
//

m.config(['$urlRouterProvider', '$stateProvider', 'settings', ($urlRouterProvider: ng.ui.IUrlRouterProvider, $stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $urlRouterProvider.when('/element/:id?mappingProjectId&dataStandardId', '/element/:id/info?mappingProjectId&dataStandardId');

    $stateProvider
        .state('app.element.detail', {
            url: '/:elementId',
            data: {
                title: 'Standard and Element Group Detail'
            },
            views: {
                'content@': {
                    templateUrl: `${settings.moduleBaseUri}/Element/Detail/ElementDetailView.tpl.html`,
                    controller: 'app.modules.element.detail',
                    controllerAs: 'elementDetailViewModel'
                }
            },
            resolve: {
                element: ['repositories', '$stateParams', (repositories: IRepositories, $stateParams) => {
                    return repositories.systemItem.find($stateParams.elementId);
                }]
            }
        });
}]);

// ****************************************************************************
// Controller app.modules.element.detail
//

m.controller('app.modules.element.detail', ['$scope', '$stateParams', 'modals', 'repositories', 'services', 'model', 'element', 'readOnly',
    function ($scope, $stateParams, modals: IModals, repostiories: IRepositories, services: IServices, model, element, readOnly) {

        services.logger.debug('Loaded controller app.modules.element.detail');

        var vm = this;
        vm.pageTitle = 'Element Detail';
        vm.mappingProjectId = $stateParams.mappingProjectId;
        vm.dataStandardId = $stateParams.dataStandardId;
        vm.elementId = $stateParams.elementId;
        vm.element = element;
        vm.controlElementPath = {};

        if (vm.dataStandardId) {
            vm.dataStandard = model;
            vm.list = services.session.cloneFromSession('elementLists', vm.dataStandardId);

        } else {
            vm.mappingProject = model;
            vm.list = services.session.cloneFromSession('elementQueues', vm.mappingProjectId);
        }

        vm.current = services.underscore.findIndex(<Array<any>>vm.list, item => { return item.ElementId === element.SystemItemId; });
        vm.previous = () =>services.state.go(services.state.current.name, { elementId: vm.list[vm.current - 1].ElementId });
        vm.next = () => services.state.go(services.state.current.name, { elementId: vm.list[vm.current + 1].ElementId });

        vm.onPage = sref => (services.state.current.name === sref);

        vm.tabs = [
            { link: 'app.element.detail.info', label: 'Info' },
            { link: 'app.element.detail.mapping', label: 'Mapping' }
        ];

        if (element.ItemTypeId === 5) {
            vm.tabs.push({ link: 'app.element.detail.enumeration-details', label: 'Enumerations' });
            vm.tabs.push({ link: 'app.element.detail.enumeration-usage', label: 'Usage' });
        }

        if (!readOnly) vm.tabs.push({ link: 'app.element.detail.actions', label: 'Actions' });

        vm.showTabs = () => services.underscore.some(<Array<any>>vm.tabs, tab => services.state.is(tab.link));

        vm.edit = () => {
            var instance = modals.systemItemForm(vm.dataStandardId, vm.element, vm.element.ItemTypeId);
            instance.result.then((data) => {
                element.ItemName = data.ItemName;
                element.IsExtended = data.IsExtended;
                element.TechnicalName = data.TechnicalName;
                element.DataTypeSource = data.DataTypeSource;
                element.Definition = data.Definition;
                element.EnumerationSystemItemId = data.EnumerationSystemItemId;
                element.ItemUrl = data.ItemUrl;
                element.ItemDataTypeId = data.ItemDataTypeId;
                element.FieldLength = data.FieldLength;

                var length = element.PathSegments.length;
                element.PathSegments[length - 1].ItemName = data.ItemName;
                element.PathSegments[length - 1].IsExtended = data.IsExtended;
                element.reloadPath(element.PathSegments);
            });
        }

        //This is to get close to the height margin so the view jumps less on load
        var closeHeight = element.Definition ? element.Definition.length : 0;
        vm.tabStyle = { 'margin-top': (Math.ceil(closeHeight / 120) * 19) + 126 + 'px' };

        services.timeout(() => {
            var elementHeader = angular.element('#element-info');
            var contentPlacement = elementHeader.outerHeight(true) - 100;
            vm.tabStyle = { 'margin-top': contentPlacement + 'px' }
        });
    }
]);