// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.data-standard.edit.extensions.list.link
//

var m = angular.module('app.modules.data-standard.edit.extensions.list.link', []);


// ****************************************************************************
// Controller app.modules.data-standard.edit.extensions.list.link
//

m.controller('app.modules.data-standard.edit.extensions.list.link', ['$uibModalInstance', '$scope', 'repositories', 'services', 'settings', 'dataStandardId', 'standards', 'extension',
    ($uibModalInstance, $scope, repositories: IRepositories, services: IServices, settings: ISystemSettings, dataStandardId, standards, extension) => {

        services.logger.debug('Loaded controller app.modules.data-standard.edit.extensions.list.link');

        $scope.standards = standards;
        $scope.extension = angular.copy(extension);
        $scope.dataStandardId = dataStandardId;
        $scope.loading = 0;
        $scope.downloading = 0;

        $scope.downloadExtensionDifferences = (downloadType) => {
            var model = {
                ExtensionMappedSystemId: $scope.extension.ExtensionMappedSystemId,
                MappedSystemExtensionId: $scope.extension.MappedSystemExtensionId,
                IncludeMarkedExtended: (downloadType === 'all' || downloadType === 'new' || downloadType === 'extended'),
                IncludeNotMarkedExtended: (downloadType === 'all' || downloadType === 'new'  || downloadType === 'notExtended'),
                IncludeUpdated: (downloadType === 'all' || downloadType === 'changes' || downloadType === 'updated'),
                IncludeRemoved: (downloadType === 'all' || downloadType === 'changes' || downloadType === 'removed')
            }

            $scope.downloading++;
            return repositories.dataStandard.extensions.downloadExtensionDifferences(dataStandardId, model).then(data => {
                var link = document.createElement('a');
                link.href = `${settings.apiBaseUri}/MappedSystemExtensionReport/${data}`;
                document.body.appendChild(link);
                link.click();

            }, error => {
                services.logger.error('Error editing extensions', error);
            }).finally(() => {
                $scope.downloading--;
            });
        }

        $scope.getExtensionDetail = () => {
            $scope.showDetails = true;
            $scope.loading++;
            var loading = angular.copy($scope.loading);
            repositories.dataStandard.extensions.getLinkingDetail(dataStandardId, $scope.extension).then(data => {

                if (loading === $scope.loading)
                    $scope.extensionDetailModel = data;

            }, error => {
                services.logger.error('Error editing extensions', error);
            }).finally(() => {
                $scope.loading--;
            });  
        }

        if (extension.MappedSystemExtensionId)
            $scope.getExtensionDetail();

        $scope.link = () => {
            if (extension.MappedSystemExtensionId) {
                return repositories.dataStandard.extensions.put(dataStandardId, $scope.extension.MappedSystemExtensionId, $scope.extension).then(data => {
                    services.logger.success('Successfully edited extension');
                    $uibModalInstance.close(data);
                }, error => {
                    services.logger.error('Error editing extensions', error);
                });  
            } else {
                return repositories.dataStandard.extensions.post(dataStandardId, $scope.extension).then(data => {
                    services.logger.success('Successfully added extension');
                    $uibModalInstance.close(data);
                }, error => {
                    services.logger.error('Error creating extensions', error);
                });   
            }
        }

        $scope.cancel = () => {
            $uibModalInstance.dismiss();
        }
    }
]);
