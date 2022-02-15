// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.detail.admin
//

var m = angular.module('app.modules.mapping-project.detail.admin', []);


// ****************************************************************************
// Controller app.modules.mapping-project.detail.admin
//

m.controller('app.modules.mapping-project.detail.admin', [
    '$scope', '$stateParams', 'enumerations', 'repositories', 'services', 'modals',
    function ($scope, $stateParams, enumerations: IEnumerations, repositories: IRepositories, services: IServices, modals: IModals) {

        services.logger.debug('Loaded controller app.modules.mapping-project.detail.admin');

        var vm = this;

        vm.id = $stateParams.id;

        vm.isNew = (undefined === vm.id);
        vm.canChangeDataStandards = vm.isNew;

        vm.getDataStandards = () => {
            repositories.dataStandard.getAll()
                .then(result => {
                    vm.srcDataStandards = result;
                    if (vm.srcDataStandards.length > 0) {
                        vm.targetDataStandards = vm.srcDataStandards.slice(0);
                    }
                }, error => {
                    services.logger.error('Error loading data standards.', error.data);
                })
                .catch(error => {
                    services.errors.handleErrors(error, vm);
                });
        };

        vm.getDataStandards();

        vm.save = mappingProject => {
            $scope.$broadcast('show-errors-check-valid');
            if (services.underscore.isUndefined(mappingProject) || $scope.mappingProjectForm.$invalid)
                return;

            if (mappingProject.SourceDataStandardId
                === mappingProject.TargetDataStandardId) {
                services.errors.addError('Source Data Standard cannot be the same as Target Data Standard.', vm);
            }

            if (!services.underscore.isUndefined(vm.errorData) && vm.errorData.hasError) {
                return;
            }

            if (!services.underscore.isUndefined(mappingProject.MappingProjectId)) {
                repositories.mappingProject.save(mappingProject.MappingProjectId, mappingProject).then(
                    () => {
                        services.logger.success('Saved mapping project.');
                        vm.success = true;
                        var modalElement = <any>angular.element(document.querySelector('#editProjectModal'));
                        modalElement.modal('hide');
                    }, error => {
                        services.logger.error('Error saving mapping project.', error.data);
                    })
                    .catch(
                    error => {
                        services.errors.handleErrors(error, vm);
                    });
            } else {
                mappingProject.ProjectStatusTypeId = services.underscore.find(enumerations.ProjectStatusType, status => (status.DisplayText === 'Active')).Id;
                mappingProject.SourceDataStandard = services.underscore.find(<Array<any>>vm.targetDataStandards, standard => (standard.DataStandardId === mappingProject.SourceDataStandardId));
                mappingProject.TargetDataStandard = services.underscore.find(<Array<any>>vm.targetDataStandards, standard => (standard.DataStandardId === mappingProject.TargetDataStandardId));

                var instance = modals.autoMapper(mappingProject);

                //On Return
                instance.result.then((mappingProjectId) => {
                    services.profile.clearProfile();
                    goToMappingDashboardPage(mappingProjectId);
                });
            }
        };

        vm.cloneMappingProject = mappingProject => {
            $scope.$broadcast('show-errors-check-valid');
            if (services.underscore.isUndefined(mappingProject) || $scope.mappingProjectForm.$invalid)
                return;

            var postData = {
                MappingProjectId: mappingProject.MappingProjectId,
                CloneProjectName: mappingProject.newCloneName
            }

            vm.loading = true;
            repositories.mappingProject.clone(mappingProject.MappingProjectd, postData)
                .then(result => {
                    services.logger.success('Cloned mapping project.');
                    vm.loading = false;
                    var modalElement = <any>angular.element(document.querySelector('#cloneProjectModal'));
                    modalElement.modal('hide');
                    $scope.cloneCallback(result);
                }, error => {
                    services.logger.error('Error cloning mapping project.', error.data);
                })
                .catch(error => {
                    vm.loading = false;
                    services.errors.handleErrors(error, vm);
                });
        };

        vm.cancelClone = () => {
            $scope.$broadcast('show-errors-reset');
            var modalElement = <any>angular.element(document.querySelector('#cloneProjectModal'));
            modalElement.modal('hide');
        };

        function goToMappingDashboardPage(mappingProjectId: any);
        function goToMappingDashboardPage(mappingProjectId) {
            services.state.go('app.mapping-project.detail.dashboard', { id: mappingProjectId });
        };

        vm.cancel = mappingProject => {
            $scope.$broadcast('show-errors-reset');
            if (!services.underscore.isUndefined(mappingProject) && !services.underscore.isUndefined(mappingProject.MappingProjectId)) {
                var modalElement = <any>angular.element(document.querySelector('#editProjectModal'));
                modalElement.modal('hide');
            } else
                goToHomePage();
        };

        function goToHomePage() {
            services.state.go('app.home');
        }
    }
]);
