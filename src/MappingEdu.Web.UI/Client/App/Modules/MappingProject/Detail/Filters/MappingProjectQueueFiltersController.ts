// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.detail.filters
//

var m = angular.module('app.modules.mapping-project.detail.filters', []);

// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.mapping-project.detail.filters', { //mappingProject.filters
            url: '/filters',
            data: {
                title: 'Mapping Project Queue Filters',
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('View')].Id
            },
            templateUrl: `${settings.moduleBaseUri}/mappingProject/detail/filters/mappingProjectQueueFiltersView.tpl.html`,
            controller: 'app.modules.mapping-project.detail.filters',
            controllerAs: 'mappingProjectQueueFiltersViewModel',
            resolve: {
                filters: ['repositories', '$stateParams', (repositories: IRepositories, $stateParams) => {
                    return repositories.mappingProject.reviewQueue.filter.getAll($stateParams.id);
                }],
                groups: ['repositories', 'mappingProject', (repositories: IRepositories, mappingProject) => {
                    return repositories.elementGroup.getAll(mappingProject.SourceDataStandardId);
                }],
                createBys: ['repositories', '$stateParams', (repositories: IRepositories, $stateParams) => {
                    return repositories.element.mapping.uniqueCreateBy($stateParams.id);
                }],
                updateBys: ['repositories', '$stateParams', (repositories: IRepositories, $stateParams) => {
                    return repositories.element.mapping.uniqueUpdateBy($stateParams.id);
                }]
            }
        });
}]);


// ****************************************************************************
// Controller app.modules.mapping-project.detail.filters
//

m.controller('app.modules.mapping-project.detail.filters', ['$scope', '$stateParams', 'modals', 'repositories', 'services', 'filters', 'groups', 'createBys', 'updateBys',
    function ($scope, $stateParams, modals: IModals, repositories: IRepositories, services: IServices, filters, groups, createBys, updateBys) {

        services.logger.debug('Loaded contoller app.modules.mapping-project.detail.filters');
        $scope.$parent.mappingProjectDetailViewModel.setTitle('FILTERS');

        var vm = this;
        vm.filters = filters;
        vm.mappingProjectId = $stateParams.id;

        vm.edit = (filter) => {
            var instance = modals.mappingProjectQueueFilterForm(vm.mappingProjectId, filter, groups, createBys, updateBys);
            return instance.result.then((data) => {
                angular.copy(data, filter);
            });
        }

        vm.delete = (filter, index) => {
            repositories.mappingProject.reviewQueue.filter.remove(filter.MappingProjectQueueFilterId).then(() => {
                services.logger.success('Deleted filter');
                vm.filters.splice(index, 1);
            }, error => {
                services.logger.error('Error deleting filter', error);
            });
        }
    }
]);
