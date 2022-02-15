// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.data-standard.modal.upload-swagger-api
//

var m = angular.module('app.modules.data-standard.modal.upload-swagger-api', []);


// ****************************************************************************
// Controller app.modules.data-standard.modal.upload-swagger-api
//

m.controller('app.modules.data-standard.modal.upload-swagger-api', ['$', '$scope', '$uibModalInstance', 'repositories', 'services','standard',
    ($, $scope, $uibModalInstance: angular.ui.bootstrap.IModalServiceInstance, repositories: IRepositories, services: IServices, standard) => {

        services.logger.debug('Loaded controller app.modules.data-standard.modal.upload-swagger-api');

        $scope.model = {
            ImportAll: false
        };

        repositories.systemConstant.find('Terms of Use').then((data) => {
            $scope.termsOfUse = data.Value;
            services.timeout(() => {
                $('#termsOfUse').perfectScrollbar();
            }, 100);
        });

        $scope.upload = () => {
            return repositories.dataStandard.import.odsApi(standard.DataStandardId, $scope.model).then(data => {
                services.logger.success('Uploaded Api.');
                $uibModalInstance.dismiss();
            }, error => {
                services.logger.error('Error uploading Api', error);
            });
        }

        $scope.close = () => {
            $uibModalInstance.dismiss();
        }
    }
]);
