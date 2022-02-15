// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module aapp.modules.elements.modals.mapping-project-queue-filter-form
//

var m = angular.module('app.modals.mapping-project-queue-filter-form', []);

// ****************************************************************************
// Service app.modals.mapping-project-queue-filter-form
//

m.factory('app.modals.mapping-project-queue-filter-form', ['settings', 'services', (settings, services) => {
    return (mappingProjectId, filter, elementGroups, creaters, updaters) => {

        var modal: ng.ui.bootstrap.IModalSettings = {
            backdrop: 'static',
            keyboard: false,
            controller: 'app.modals.mapping-project-queue-filter-form',
            templateUrl: `${settings.modalBaseUri}/MappingProjectQueueFilterForm/MappingProjectQueueFilterForm.tpl.html`,
            size: 'lg',
            resolve: {
                mappingProjectId: () => { return mappingProjectId },
                filter: () => { return filter },
                elementGroups: () => { return elementGroups },
                creaters: () => { return creaters },
                updaters: () => { return updaters }
            }
        };

        return services.modal.open(modal);
    }
}]);

// ****************************************************************************
// Controller app.modals.mapping-project-queue-filter-form
//

m.controller('app.modals.mapping-project-queue-filter-form', ['$scope', '$uibModalInstance', 'repositories', 'services', 'filter', 'mappingProjectId', 'elementGroups', 'creaters', 'updaters',
    ($scope, $uibModalInstance: angular.ui.bootstrap.IModalServiceInstance, repositories: IRepositories, services: IServices, filter, mappingProjectId, elementGroups, creaters, updaters) => {

        services.logger.debug('Loaded controller app.modals.mapping-project-queue-filter-form');

        $scope.filter = angular.copy(filter);

        $scope.displayElementGroup = (systemItemId) => {
            var group = services.underscore.find(<Array<any>>elementGroups, x => (x.SystemItemId === systemItemId));
            if (group) return group.ItemName;
            else return '';
        }

        $scope.displayCreatedBy = (userId) => {
            var user = services.underscore.find(<Array<any>>creaters, x => (x.Id === userId));
            if (user) return user.FirstName[0] + '. ' + user.LastName;
            else return '';
        }

        $scope.displayUpdatedBy = (userId) => {
            var user = services.underscore.find(<Array<any>>updaters, x => (x.Id === userId));
            if (user) return user.FirstName[0] + '. ' + user.LastName;
            else return '';
        }

        $scope.save = () => {
            if ($scope.filter.MappingProjectQueueFilterId) {
                repositories.mappingProject.reviewQueue.filter.save($scope.filter.MappingProjectQueueFilterId, mappingProjectId, $scope.filter).then((data) => {
                    services.logger.success('Updated mapping filter.');
                    $uibModalInstance.close(data);
                }, error => {
                    services.logger.error('Error updating mapping filter', error);
                });
            } else {
                repositories.mappingProject.reviewQueue.filter.create(mappingProjectId, $scope.filter).then((data) => {
                    services.logger.success('Created mapping filter.');
                    $uibModalInstance.close(data);
                }, error => {
                    services.logger.error('Error creating mapping filter', error);
                });
            }
        }

        $scope.cancel = () => {
            $uibModalInstance.dismiss();
        }

    }
]);
