// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.data-standard.edit.actions.clone
//

var m = angular.module('app.modules.data-standard.edit.actions.clone', []);


// ****************************************************************************
// Controller app.modules.data-standard.edit.actions.clone
//

m.controller('app.modules.data-standard.edit.actions.clone', ['$uibModalInstance', '$scope', 'repositories', 'services', 'standard', 'standards', 'hasExtensions',
    ($uibModalInstance, $scope, repositories: IRepositories, services: IServices, standard, standards, hasExtensions) => {

        services.logger.debug('Loaded controller app.modules.data-standard.edit.clone');

        $scope.clonedFrom = angular.copy(standard);
        $scope.standard = {
            SystemName: $scope.clonedFrom.SystemName + ' (Copy)',
            SystemVersion: $scope.clonedFrom.SystemVersion
        };
        $scope.dataStandards = standards;
        $scope.hasExtensions = hasExtensions;

        $scope.clone = (standard) => {
            var q = services.q.defer();
            repositories.dataStandard.clone($scope.clonedFrom.DataStandardId, standard).then(data => {
                if (data.SimilarVersioning) {
                    q.resolve();
                    $scope.similarVersioning = true;
                    $scope.clonedStandard = data;
                    $scope.clonedStandard.PreviousDataStandard = services.underscore.find(<Array<any>>standards, standard => { return standard.DataStandardId === $scope.clonedStandard.PreviousDataStandardId })
                }
                else $uibModalInstance.close(data);
            }, error => {
                services.logger.error('Error cloning data standard.', error.data);
            });
            return q.promise;
        }

        $scope.deltaCopy = (standard) => {
            return repositories.autoMap.deltaCopy(standard).then(() => {
                $uibModalInstance.close(standard);
            });
        }

        $scope.cancel = () => {
            $uibModalInstance.dismiss();
        }
    }
]);
