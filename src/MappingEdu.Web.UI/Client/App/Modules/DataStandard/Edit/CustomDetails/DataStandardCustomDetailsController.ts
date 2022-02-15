// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.data-standard.edit.custom-details
//

var m = angular.module('app.modules.data-standard.edit.custom-details', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.data-standard.edit.custom-details', {
            url: '/custom',
            data: {
                title: 'Element Group',
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Guest')].Id
            },
            templateUrl: `${settings.moduleBaseUri}/DataStandard/Edit/CustomDetails/DataStandardCustomDetailsView.tpl.html`,
            controller: 'app.modules.data-standard.edit.custom-details',
            controllerAs: 'dataStandardCustomDetailsViewModel',
            resolve: {
                customDetails: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.customDetailMetadata.getAllByDataStandard($stateParams.dataStandardId);
                }],
            }
        });
}]);

// ****************************************************************************
// Controller app.modules.data-standard.edit.groups
//

m.controller('app.modules.data-standard.edit.custom-details', ['$scope', '$stateParams', 'standard', 'customDetails', 'modals', 'repositories', 'services', 
    function ($scope, $stateParams, standard, customDetails, modals: IModals, repositories: IRepositories, services: IServices) {

        services.logger.debug('Loaded app.modules.data-standard.edit.custom-details controller');
        $scope.$parent.dataStandardDetailViewModel.setTitle('CUSTOM DETAILS');

        var vm = this;

        vm.dataStandardId = $stateParams.dataStandardId;
        vm.editSetFocus = false;
        vm.dataStandard = standard;
        vm.customDetails = customDetails;

        vm.create = () => {
            var modal = modals.customDetailForm(vm.dataStandardId, {});
            modal.result.then((customDetail) => {
                vm.customDetails.push(customDetail);
            });
        };

        vm.edit = customDetail => {
            var modal = modals.customDetailForm(vm.dataStandardId, customDetail);
            modal.result.then((data) => {
                customDetail.DisplayName = data.DisplayName;
                customDetail.IsBoolean = data.IsBoolean;
                customDetail.IsCoreDetail = data.IsCoreDetail;
            });
        };

        vm.delete = (customDetail, index) => {
            repositories.customDetailMetadata.remove(vm.dataStandardId, customDetail.CustomDetailMetadataId).then(() => {
                services.logger.success('Removed custom detail.');
                vm.customDetails.splice(index, 1);
            }, error => {
                services.logger.error('Error removing custom detail.', error.data);
            });
        };
    }
]);

