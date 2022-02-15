// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.data-standard.edit.extensions.list.delete
//

var m = angular.module('app.modules.data-standard.edit.extensions.list.delete', []);


// ****************************************************************************
// Controller app.modules.data-standard.edit.extensions.list.delete
//

m.controller('app.modules.data-standard.edit.extensions.list.delete', ['$uibModalInstance', '$scope', 'repositories', 'services', 'dataStandardId', 'extensionId',
    ($uibModalInstance, $scope, repositories: IRepositories, services: IServices, dataStandardId, extensionId) => {

        services.logger.debug('Loaded controller app.modules.data-standard.edit.extensions.list.delete');

        $scope.delete = () => {
            return repositories.dataStandard.extensions.remove(dataStandardId, extensionId).then(() => {
                services.logger.success('Successfully deleted extension');
                $uibModalInstance.close();
            }, error => {
                services.logger.error('Error deleting extension', error);
            });
        }

        $scope.cancel = () => {
            $uibModalInstance.dismiss();
        }
    }
]);
