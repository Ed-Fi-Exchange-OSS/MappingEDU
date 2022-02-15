// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.data-standard.modal.upload-extension
//

var m = angular.module('app.modules.data-standard.modal.upload-extension', []);


// ****************************************************************************
// Controller app.modules.data-standard.modal.upload-extension
//

m.controller('app.modules.data-standard.modal.upload-extension', ['$', '$scope', '$uibModalInstance', 'repositories', 'services', 'Upload', 'standard',
    ($, $scope, $uibModalInstance: angular.ui.bootstrap.IModalServiceInstance, repositories: IRepositories, services: IServices, upload, standard) => {

        services.logger.debug('Loaded controller app.modules.data-standard.modal.upload-extension');

        repositories.systemConstant.find('Terms of Use').then((data) => {
            $scope.termsOfUse = data.Value;
            services.timeout(() => {
                $('#termsOfUse').perfectScrollbar();
            }, 100);
        });

        $scope.log = '';
        $scope.uploadSuccessful = false;
        $scope.percent = 0;
        $scope.stylepercent = '0%';
     
        $scope.upload = (xsd) => {
            if(xsd) {
                const file = xsd.length ? xsd[0] : xsd;

                $scope.loading = true;
                return upload.upload({
                    url: `api/Import/${standard.DataStandardId}/Extension`,
                    method: 'POST',
                    file: file

                }).success((data, status, headers, config) => {
                    $scope.loading = false;
                    $scope.log = `Successful Upload: ${config.file.name}`;

                    if (!data.Logs) {
                        angular.element(document.querySelector('#logMessage')).removeClass('bg-warning').addClass('bg-success');
                    }

                    $scope.warnings = data.Logs;
                    $scope.uploadSuccessful = true;

                    $scope.fileToken = data.FileToken;
                    $scope.totalLogs = data.TotalLogs;

                }).progress((evt) => {
                    $scope.percent = (100.0 * evt.loaded / evt.total);
                    $scope.stylepercent = $scope.percent + '%';

                }).error((error, status, headers, config) => {
                    $scope.loading = false;
                    var errors = <Array<any>>error.ExceptionMessage.split('; ');

                    $scope.warnings = services.underscore.filter(errors, item => (item.indexOf('Warning:') === 0));
                    angular.forEach($scope.warnings, (value, key) => {
                        $scope.warnings[key] = value.replace('Warning: ', '');
                    });
                    $scope.infos = services.underscore.filter(errors, item => (item.indexOf('Info:') === 0));
                    angular.forEach($scope.infos, (value, key) => {
                        $scope.infos[key] = value.replace('Info: ', '');
                    });
                    $scope.errors = services.underscore.filter(errors, item => (item.indexOf('ERROR:') === 0));
                    angular.forEach($scope.errors, (value, key) => {
                        $scope.errors[key] = value.replace('ERROR: ', '');
                    });
                    $scope.fatals = services.underscore.filter(errors, item => (item.indexOf('FATAL:') === 0));
                    angular.forEach($scope.fatals, (value, key) => {
                        $scope.fatals[key] = value.replace('FATAL: ', '');
                    });

                    if ($scope.fatals.length > 0)
                        services.logger.error('Error Uploading Extension.', error);
                    else {
                        $scope.log = `Successful Upload: ${config.file.name}`;
                        $scope.log += '\r\n';
                        $scope.log += $scope.errors.length + ' error(s), ';
                        $scope.log += $scope.warnings.length + ' warning(s), and ';
                        $scope.log += $scope.infos.length + ' informational message(s).';
                        $scope.uploadSuccessful = true;
                        angular.element(document.querySelector('#logMessage')).removeClass('bg-success').addClass('bg-warning');
                    }
                });
            }
        };

        $scope.close = () => {
            $uibModalInstance.dismiss();
        }
    }
]);
