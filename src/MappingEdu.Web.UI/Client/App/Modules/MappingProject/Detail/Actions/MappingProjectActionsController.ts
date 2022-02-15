// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.detail.actions
//

var m = angular.module('app.modules.mapping-project.detail.actions', [
    'app.modules.mapping-project.detail.actions.bulk-update'
]);

// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.mapping-project.detail.actions', { //mappingProject.actions
            url: '/actions',
            templateUrl: `${settings.moduleBaseUri}/mappingProject/detail/actions/mappingProjectActionsView.tpl.html`,
            controller: 'app.modules.mapping-project.detail.actions',
            controllerAs: 'mappingProjectActionsViewModel',
            data: {
                title: 'Mapping Project Functions',
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('View')].Id
            }
        });
}]);


// ****************************************************************************
// Controller app.modules.mapping-project.detail.actions
//

m.controller('app.modules.mapping-project.detail.actions', [
    '$scope', '$stateParams', 'enumerations', 'modals', 'repositories', 'services', 'settings',
    function($scope, $stateParams, enumerations: IEnumerations, modals: IModals, repositories: IRepositories, services: IServices, settings: ISystemSettings) {

        services.logger.debug('Loaded controller app.modules.mapping-project.detail.actions');
        $scope.$parent.mappingProjectDetailViewModel.setTitle('ACTIONS');

        var vm = this;

        vm.showApproveMsg = false;

        vm.deleteMappingProject = mappingProject => {
            repositories.mappingProject.remove(mappingProject.MappingProjectId)
                .then(() => {
                    services.logger.success('Removed mapping project.');
                    services.timeout(() => { services.state.go('app.home'); }, 500);
                }, error => {
                    services.logger.error('Error removing mapping project.', error.data);
                })
                .catch(error => {
                    services.errors.handleErrors(error, vm);
                });
        };

        vm.approveReadyForReview = mappingProject => {
            repositories.mappingProject.systemItemMaps.approveReviewed(mappingProject.MappingProjectId)
                .then(data => {
                    services.logger.success('Approved system items.');
                    vm.approveAllMsg = data.CountUpdated > 0 ? data.CountUpdated + ' mappings were approved successfully.' : 'There were no mappings ready for approval';
                    vm.showApproveMsg = true;
                    $scope.$emit('workflow-status-updated');
                    services.timeout(() => { vm.showApproveMsg = false; }, 5000);
                }, error => {
                    services.logger.error('Error approving system items.', error.data);
                })
                .catch(error => {
                    services.errors.handleErrors(error, vm);
                });
        }

        vm.closeProject = mappingProject => {
            mappingProject.ProjectStatusTypeId = services.underscore.find(enumerations.ProjectStatusType, status => (status.DisplayText === 'Closed')).Id;

            repositories.mappingProject.save(mappingProject.MappingProjectId, mappingProject)
                .then(() => {
                    services.logger.success('Saved mapping project.');
                    $scope.mappingProjectDetailViewModel.load();
                    vm.closedMsg = mappingProject.ProjectName + ' has been closed.';
                    vm.showClosedMsg = true;
                    services.timeout(() => { vm.showClosedMsg = false; }, 5000);
                }, error => {
                    services.logger.error('Error saving maping project.');
                })
                .catch(error => {
                    services.errors.handleErrors(error, vm);
                });
        };

        vm.reopenProject = mappingProject => {
            mappingProject.ProjectStatusTypeId = services.underscore.find(enumerations.ProjectStatusType, status => (status.DisplayText === 'Active')).Id;

            repositories.mappingProject.save(mappingProject.MappingProjectId, mappingProject)
                .then(() => {
                    services.logger.success('Saved mapping project.');
                    $scope.mappingProjectDetailViewModel.load();
                    vm.openedMsg = mappingProject.ProjectName + ' has been re-opened.';
                    vm.showOpenedMsg = true;
                    services.timeout(() => { vm.showOpenedMsg = false; }, 5000);
                }, error => {
                    services.logger.error('Error saving mapping project.', error.data);
                })
                .catch(error => {
                    services.errors.handleErrors(error, vm);
                });
        };

        vm.getMappingProject = () => {
            repositories.mappingProject.find($stateParams.id)
                .then(result => {
                    vm.currentMappingProject = result;
                }, error => {
                    services.logger.error('Error loading mapping project.', error.data);
                })
                .catch(error => {
                    services.errors.handleErrors(error, vm);
                });
        };

        vm.cloneCallback = result => {
            if (result.statusText == 'OK')
                vm.cloneProjectMessage = 'Mapping project cloned successfully.';
            else
                vm.cloneProjectMessage = `There was an error when cloning the mapping project: ${result.statusText}`;
            vm.showCloneMessage = true;
            services.timeout(() => { vm.showCloneMessage = false; }, 5000);
        };

        vm.bulkUpdate = mappingProject => {
            var model = {
                backdrop: 'static',
                keyboard: false,
                templateUrl: `${settings.moduleBaseUri}/mappingProject/detail/actions/bulkUpdateView.tpl.html`,
                controller: 'app.modules.mapping-project.detail.actions.bulk-update',
                controllerAs: 'ctrl',
                windowClass: 'modal-xl',
                resolve: {
                    project: () => { return mappingProject; }
                }
            }
            var instance = services.modal.open(model);

            //On Return
            instance.result.then((data) => {
                vm.showBulkResultsMsg = true;
                vm.bulkResultsCount = data.CountUpdated;
            });
        }

        vm.runAutoMapper = mappingProject => {

            var instance = modals.autoMapper(mappingProject);

            //On Return
            instance.result.then(() => {
                services.state.go('app.mapping-project.detail.dashboard', { id: mappingProject.MappingProjectId });
            });
        }


        vm.getMappingProject();
    }
]);
