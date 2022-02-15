// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.manage.logs.view
//

var m = angular.module('app.modules.manage.logs.view', []);


// ****************************************************************************
// Controller app.modules.manage.logs.view
//

m.controller('app.modules.manage.logs.view', ['$scope', '$uibModalInstance', 'repositories', 'services', 'log',
    ($scope, $uibModalInstance: angular.ui.bootstrap.IModalServiceInstance, repositories: IRepositories, services: IServices, log) => {

        services.logger.debug('Loaded controller app.modules.manage.logs.view');

        $scope.log = log;

        $scope.close = () => {
            $uibModalInstance.dismiss();
        }
    }
]);
