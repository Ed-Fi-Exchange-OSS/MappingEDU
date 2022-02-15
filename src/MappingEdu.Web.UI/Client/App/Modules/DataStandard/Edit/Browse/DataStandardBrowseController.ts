// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.data-standard.edit.browse
//

var m = angular.module('app.modules.data-standard.edit.browse', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.data-standard.edit.browse', {
            url: '/browse?domainId',
            data: {
                title: 'Browse Standard',
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Guest')].Id
            },
            templateUrl: `${settings.moduleBaseUri}/DataStandard/Edit/Browse/DataStandardBrowseView.tpl.html`,
            controller: 'app.modules.data-standard.edit.browse',
            controllerAs: 'dataStandardBrowseModel'
        });
}]);


// ****************************************************************************
// Controller app.modules.data-standard.edit.target
//

m.controller('app.modules.data-standard.edit.browse', ['$', '$scope', '$stateParams', 'services', 'repositories',
    function ($, $scope, $stateParams, services: IServices) {

        services.logger.debug('Lodaed app.modules.data-standard.edit.browse controller');
        $scope.$parent.dataStandardDetailViewModel.setTitle('BROWSE');

        var vm = this;
        vm.dataStandardId = $stateParams.dataStandardId;
        vm.domainId = $stateParams.domainId;
    }
]);