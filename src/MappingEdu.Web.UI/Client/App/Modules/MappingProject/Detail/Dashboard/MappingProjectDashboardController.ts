// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.detail.dashboard
//

var m = angular.module('app.modules.mapping-project.detail.dashboard', []);

// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.mapping-project.detail.dashboard', { //mappingProject.dashboard
            url: '/dashboard',
            data: {
                title: 'Mapping Project Dashboard',
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('View')].Id
            },
            templateUrl: `${settings.moduleBaseUri}/mappingProject/detail/dashboard/mappingProjectDashboardView.tpl.html`,
            controller: 'app.modules.mapping-project.detail.dashboard',
            controllerAs: 'mappingProjectDashboardViewModel',
            resolve: {
                dashboard: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.mappingProject.dashboard.get($stateParams.id);
                }]
            }
        });
}]);


// ****************************************************************************
// Controller app.modules.mapping-project.detail.dashboard
//

m.controller('app.modules.mapping-project.detail.dashboard', [
    '$rootScope', '$scope', '$stateParams', 'repositories', 'services', 'dashboard',
    function ($rootScope, $scope, $stateParams, repositories: IRepositories, services: IServices, dashboard) {

        services.logger.debug('Loaded contoller app.modules.mapping-project.detail.dashboard');
        $scope.$parent.mappingProjectDetailViewModel.setTitle('DASHBOARD');
        var vm = this;

        vm.id = $stateParams.id;

        vm.getFilters = () => {
            services.timeout(() => services.loading.start('dashboard_filters'));
            repositories.mappingProject.reviewQueue.filter.dashboard($stateParams.id).then(filters => {
                vm.filters = filters;
                services.timeout(() => services.loading.finish('dashboard_filters'));
            });
        }

        vm.getFilters();

        vm.getMappingProjectDashboard = () => {
            vm.dashboard = dashboard;

            var maxLen = Math.max(vm.dashboard.ElementGroups.length,
                vm.dashboard.Statuses.length) * 1.5;

            vm.verticalRange = [];

            for (var i = 0; i < maxLen; i++) {
                vm.verticalRange.push(i);
            }
        }

        vm.reviewQueueSref = (item: any, isFilter) => {
            if (isFilter) {
                return services.state.href('app.mapping-project.detail.review-queue', {
                    mappingProjectId: vm.id,
                    filterId: item.MappingProjectQueueFilterId.toString()
                });   
            } else {
                var filter = item.Filter;
                var newFilter = {};
                var statuses = {};
                var elementGroups = {};
                var flagged = false;
                if (filter != undefined) {
                    if (filter.toLowerCase() === 'allincomplete') {
                        statuses['1'] = true;
                        statuses['Unmapped'] = true;
                    } else if (filter.toLowerCase() === 'unmapped') {
                        statuses['Unmapped'] = true;
                    } else if (filter.toLowerCase() === 'flagged')
                        flagged = true;

                    else if ($.isNumeric(filter)) {
                        statuses[filter] = true;
                    } else if (filter.match(/^[{]?[0-9a-fA-F]{8}[-]?([0-9a-fA-F]{4}[-]?){3}[0-9a-fA-F]{12}[}]?$/)) {
                        statuses['1'] = true;
                        statuses['Unmapped'] = true;
                        elementGroups[filter.toLowerCase()] = true;
                    }

                    var queueFilters = services.session.cloneFromSession('queueFilters', vm.id);

                    newFilter = {
                        elementGroups: elementGroups,
                        itemTypes: {},
                        methods: {},
                        statuses: statuses,
                        flagged: flagged,
                        globalSearch: '',
                        pageSize: (queueFilters) ? queueFilters.pageSize : 10,
                        pageNo: 0,
                        orderBy: (queueFilters) ? queueFilters.orderBy : []
                    };
                }

                return services.state.href('app.mapping-project.detail.review-queue', {
                    mappingProjectId: vm.id,
                    filter: JSON.stringify(newFilter)
                });   
            }
        }

        vm.elementSref = (item: any, resume) => {
            return services.state.href('app.element.detail.mapping', {
                mappingProjectId: vm.id,
                filter: item.Filter,
                resume: resume
            });
        }

        vm.getMappingProjectDashboard();
        
    }
]);
