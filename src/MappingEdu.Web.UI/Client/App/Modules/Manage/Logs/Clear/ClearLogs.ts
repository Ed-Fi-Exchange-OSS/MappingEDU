// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.manage.logs.clear
//

var m = angular.module('app.modules.manage.logs.clear', []);


// ****************************************************************************
// Controller app.modules.manage.logs.clear
//

m.controller('app.modules.manage.logs.clear', ['$scope', '$uibModalInstance', 'repositories', 'services',
    ($scope, $uibModalInstance: angular.ui.bootstrap.IModalServiceInstance, repositories: IRepositories, services: IServices) => {

        services.logger.debug('Loaded controller app.modules.manage.logs.clear');

        $scope.clear = () => {
            return repositories.logging.clearLogs($scope.model).then(() => {
                services.logger.success('Successfully cleared old logs');
                $uibModalInstance.close();
            }, error => {
                services.logger.error('Error clearing logs', error);
            });
        }

        $scope.close = () => {
            $uibModalInstance.dismiss();
        }
    }
]);
