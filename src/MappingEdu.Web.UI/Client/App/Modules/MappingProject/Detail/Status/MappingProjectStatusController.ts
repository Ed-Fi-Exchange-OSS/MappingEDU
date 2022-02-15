// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.detail.status
//

var m = angular.module('app.modules.mapping-project.detail.status', []);


// ****************************************************************************
// Controller app.modules.mapping-project.detail.status
//

m.controller('app.modules.mapping-project.detail.status', ['$scope', 'enumerations', 'repositories', 'services',
    function($scope, enumerations: IEnumerations, repositories: IRepositories, services: IServices) {

        services.logger.debug('Loaded controller app.modules.mapping-project.detail.status');

        var vm = this;
        var readOnly = null;
        var loading = true;

        $scope.$watch('mappingProject', () => {
            if(!readOnly && !loading) vm.getMappingProjectStatus();
        });

        vm.getMappingProjectStatus = () => {
            if (!$scope.mappingProject) {
                vm.isReadyForClose = false;
                return;
            }

            var mappingProject = $scope.mappingProject;
            repositories.mappingProject.status.get(mappingProject.MappingProjectId)
                .then(data => {
                    vm.isReadyForClose = data.Approved && (mappingProject.ProjectStatusTypeName == 'Active');
                }, error => {
                    services.logger.error('Error getting mapping project status.', error.data);
                })
                .catch(error => {
                    services.errors.handleErrors(error, vm);
                });
        }

        services.profile.me().then(me => {

            var index = me.Projects.map(x => x.Id).indexOf(vm.mappingProjectId);
            if (index === -1 && !me.IsAdministrator) readOnly = true;
            else if (!me.IsAdministrator) {
                var model = me.Projects[index];
                if (model.Role < 2) readOnly = true;
            }
            if (!readOnly) vm.getMappingProjectStatus();
            loading = false;
        });


        vm.closeProject = () => {
            var mappingProject = $scope.mappingProject;
            mappingProject.ProjectStatusTypeId = services.underscore.find(enumerations.ProjectStatusType, status => (status.DisplayText === 'Closed')).Id;

            repositories.mappingProject.save(mappingProject.MappingProjectId, mappingProject)
                .then(data => {
                    services.logger.success('Closed project.');
                    $scope.mappingProject = data;
                    vm.isReadyForClose = false;
                    vm.closedMsg = mappingProject.ProjectName + ' has been closed.';
                    vm.showClosedMsg = true;
                    services.timeout(() => { vm.showClosedMsg = false; }, 5000);
                }, error => {
                    services.logger.error('Error saving mapping project.', error.data);
                })
                .catch(error => {
                    services.errors.handleErrors(error, vm);
                });
        };
    }
]);
