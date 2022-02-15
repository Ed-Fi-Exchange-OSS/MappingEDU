// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.element.detail.enumeration.usage
//

var m = angular.module('app.modules.element.detail.enumeration-usage', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $stateProvider
        .state('app.element.detail.enumeration-usage', {
            url: '/usage',
            data: {
                title: 'Element Detail Mapping'
            },
            templateUrl: `${settings.moduleBaseUri}/Element/Detail/Usage/EnumerationUsageView.tpl.html`,
            controller: 'app.modules.element.detail.enumeration-usage',
            controllerAs: 'enumerationUsageViewModel',
            resolve: {
                usage: ['$stateParams', 'repositories', ($stateParams, repositories: IRepositories) => {
                    return repositories.systemItem.usage($stateParams.elementId);
                }],
                reroute: ['$stateParams', 'services', 'element', ($stateParams, services: IServices, element) => {
                    if (element.ItemTypeId === 4) {
                        if ($stateParams.dataStandardId) services.state.go('app.element.detail.info', { elementId: element.SystemItemId });
                        else services.state.go('app.element.detail.mapping', { elementId: element.SystemItemId });
                    }
                }]
            }
        });
}]);


// ****************************************************************************
// Controller app.modules.element.detail.enumeration.usage
//

m.controller('app.modules.element.detail.enumeration-usage', ['$scope', '$stateParams', 'services', 'usage',
    function enumerationUsageController($scope, $stateParams, services: IServices, usage) {

        services.logger.debug('Loaded controller app.modules.element.detail.enumeration-usage');

        var vm = this;

        vm.mappingProjectId = $stateParams.mappingProjectId;
        vm.dataStandardId = $stateParams.dataStandardId;
        vm.usage = usage;
    }
]);
