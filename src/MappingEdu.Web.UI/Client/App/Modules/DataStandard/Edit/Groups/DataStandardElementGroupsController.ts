// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.data-standard.edit.groups
//

var m = angular.module('app.modules.data-standard.edit.groups', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.data-standard.edit.groups', {
            url: '/groups',
            data: {
                title: 'Element Group',
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Guest')].Id
            },
            templateUrl: `${settings.moduleBaseUri}/DataStandard/Edit/Groups/DataStandardElementGroupsView.tpl.html`,
            controller: 'app.modules.data-standard.edit.groups',
            controllerAs: 'dataStandardElementGroupsViewModel',
            resolve: {
                elementGroups: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.elementGroup.getAll($stateParams.dataStandardId);
                }],
            }
        });
}]);

// ****************************************************************************
// Controller app.modules.data-standard.edit.groups
//

m.controller('app.modules.data-standard.edit.groups', ['$scope', '$stateParams', 'standard', 'elementGroups', 'modals', 'repositories', 'services', 
    function ($scope, $stateParams, standard, elementGroups, modals: IModals, repositories: IRepositories, services: IServices) {

        services.logger.debug('Loaded app.modules.data-standard.edit.groups controller');
        $scope.$parent.dataStandardDetailViewModel.setTitle('DETAIL');

        var vm = this;

        vm.dataStandardId = $stateParams.dataStandardId;
        vm.editSetFocus = false;
        vm.dataStandard = standard;
       
        vm.setElementGroups = (data) => {
            var elementListState = 'app.data-standard.edit.elements';
            var elementGroupState = 'app.element-group.detail';
            var elementBrowseState = 'app.data-standard.edit.browse';
            vm.elementGroups = data;

            var allElementsGroup = {
                SystemItemId: 0,
                ItemName: 'All Element Groups',
                Definition: 'The collection of all Element Groups in this Data Standard.'
            };

            vm.elementGroups.unshift(allElementsGroup);

            services.underscore.each(<Array<any>>vm.elementGroups, elementGroup => {

                elementGroup.DataStandardId = vm.dataStandard.DataStandardId;
                elementGroup.itemNameSref = elementListState
                    + '({ id:\'' + elementGroup.DataStandardId + '\', elementGroup: \'' + elementGroup.SystemItemId
                    + '\' })';

                elementGroup.elementGroupDetailSref = elementGroupState
                    + '({ dataStandardId:\'' + elementGroup.DataStandardId
                    + '\', id: \'' + elementGroup.SystemItemId + '\' })';
                elementGroup.listSref = elementGroup.itemNameSref;
                elementGroup.browseSref = elementBrowseState + '({ dataStandardId:\'' + elementGroup.DataStandardId + '\'';
                if (elementGroup.SystemItemId != 0) elementGroup.browseSref += ', domainId: \'' + elementGroup.SystemItemId + '\'';
                elementGroup.browseSref += ' })';
            });
        }

        vm.setElementGroups(angular.copy(elementGroups));

        vm.create = () => {
            var modal = modals.systemItemForm(vm.dataStandardId, {}, 1);
            modal.result.then((group) => {
                elementGroups.push(group);
                vm.setElementGroups(angular.copy(elementGroups));
            });
        };

        vm.edit = group => {
            var modal = modals.systemItemForm(vm.dataStandardId, group, 1);
            modal.result.then((data) => {
                group.ItemName = data.ItemName;
                group.Definition = data.Definition;
                group.IsExtended = data.IsExtended;
            });
        };

        vm.delete = (elementGroup, index) => {
            repositories.elementGroup.remove(vm.dataStandard.DataStandardId, elementGroup.SystemItemId)
                .then(() => {
                    services.logger.success('Removed element group.');
                    elementGroups.splice(index - 1, 1);
                    vm.setElementGroups(angular.copy(elementGroups));
                }, error => {
                    services.logger.error('Error removing element group.', error.data);
                });
        };
    }
]);

