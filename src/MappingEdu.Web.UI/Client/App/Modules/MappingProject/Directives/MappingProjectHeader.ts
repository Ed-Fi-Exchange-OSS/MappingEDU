// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.mapping-project-header
//

var m = angular.module('app.directives.mapping-project-header', []);


// ****************************************************************************
// Directive ma-mapping-project-header
//

m.directive('maMappingProjectHeader', ['settings', (settings: ISystemSettings) => ({
    restrict: 'E',
    scope: {
        mappingProject: '=',
        editable: '=',
        summary: '='
    },
    templateUrl: `${settings.moduleBaseUri}/MappingProject/Directives/MappingProjectHeader.tpl.html`,
    controller: 'maMappingProjectHeader'
})]);


m.controller('maMappingProjectHeader', ['$scope', 'repositories', 'services', 'enumerations', function ($scope, repositories: IRepositories, services: IServices, enumerations: IEnumerations) {

    var vm = this;

    services.profile.me().then((data) => {
        vm.me = data;

        $scope.dashboardHref = (mappingProject) => {
            if(vm.me.IsGuest)
                return services.state.href('app.mapping-project.detail.review-queue', { id: mappingProject.MappingProjectId });
            else
                return services.state.href('app.mapping-project.detail.dashboard', { id: mappingProject.MappingProjectId });
        }
    });

    $scope.standardHref = (standard) => {
        return services.state.href('app.data-standard.edit.groups', { dataStandardId: standard.DataStandardId });
    }

    $scope.getAccess = (project) => {
        if ($scope.summary) return '';

        var html = '';
        if (project.Role > 0) {
            html += enumerations.UserAccess[enumerations.UserAccess.map(x => x.Id).indexOf(project.Role)].DisplayText;
        }
        if (vm.me && vm.me.IsAdministrator && project.Role > 0) html += ', ';
        if (vm.me && vm.me.IsAdministrator) html += 'Admin';
        if (project.IsPublic && (project.Role > 0 || vm.me.IsAdministrator)) html += ', ';
        if (project.IsPublic) html += 'Public';
        return html;
    }

}]);