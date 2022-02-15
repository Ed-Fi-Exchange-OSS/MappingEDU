// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.directives.clone-project
//

var m = angular.module('app.modules.mapping-project.directives.clone-project', []);


// ****************************************************************************
// Directive ma-clone-mapping-project
//

m.directive('maCloneMappingProject', ['settings', (settings: ISystemSettings) => ({
    restrict: 'E',
    templateUrl: `${settings.moduleBaseUri}/mappingProject/directives/CloneMappingProject.tpl.html`, 
    scope: {
        mappingProject: '=',
        modal: '=',
        cloneCallback: '='
    },
    controller: 'maCloneMappingProject'
})]);

m.controller('maCloneMappingProject', ['$scope', 'repositories', 'services', function($scope, repositories: IRepositories, services: IServices) {

    $scope.clone = { MappingProjectId: $scope.mappingProject.MappingProjectId };

    $scope.cloneProject = (model) => {
        return repositories.mappingProject.clone($scope.mappingProject.MappingProjectId, model).then(data => {
            var modalElement = <any>angular.element(document.querySelector('#cloneProjectModal'));
            modalElement.modal('hide');
            services.timeout(() => {
                services.state.go('app.mapping-project.detail.dashboard', { id: data.MappingProjectId });
                services.logger.success('Cloned mapping project.');
            }, 100);
        }, error => {
            services.logger.error('Error cloning mapping project.', error);
        });
    };

    $scope.cancelClone = () => {
        $scope.clone.CloneProjectNmae = null; 
        $scope.$broadcast('show-errors-reset');
        var modalElement = <any>angular.element(document.querySelector('#cloneProjectModal'));
        modalElement.modal('hide');
    };
}]);