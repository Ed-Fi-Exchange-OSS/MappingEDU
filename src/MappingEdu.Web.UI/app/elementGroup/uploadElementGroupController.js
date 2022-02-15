// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appElementGroup').controller('uploadElementGroupController', [
    '$', '$timeout', 'Upload', '$scope', '_', '$state', '$stateParams', 'handleErrorService',
    function($, $timeout, Upload, $scope, _, $state, $stateParams, handleErrorService) {
        var uploadElementGroupViewModel = this;
        $scope.log = '';
        uploadElementGroupViewModel.uploadSuccessful = false;
        uploadElementGroupViewModel.percent = 0;
        uploadElementGroupViewModel.stylepercent = '0%';
        uploadElementGroupViewModel.upload = function(files, standard) {
            if (files && files.length) {
                var file = files[0];
                if (_.isUndefined(standard)) {
                    handleErrorService.addError('Data Standard is null', uploadElementGroupViewModel);
                    return;
                }
                Upload.upload({
                    url: Application.Urls.Api.ElementGroupUpload,
                    method: 'POST',
                    data: {
                        SystemName: standard.SystemName,
                        SystemVersion: standard.SystemVersion,
                        MappedSystemId: standard.DataStandardId,
                        FileName: file.name
                    },
                    file: file
                }).success(function(data, status, headers, config) {
                    $scope.log = 'Successful Upload: ' + config.file.name;
                    uploadElementGroupViewModel.uploadSuccessful = true;
                    angular.element(document.querySelector('#logMessage')).removeClass('bg-warning').addClass('bg-success');
                }).progress(function(evt) {
                    uploadElementGroupViewModel.percent = parseInt(100.0 * evt.loaded / evt.total);
                    uploadElementGroupViewModel.stylepercent = uploadElementGroupViewModel.percent + '%';
                }).error(function(error, status, headers, config) {
                    var errors = error.ExceptionMessage.split('; ');
                    uploadElementGroupViewModel.warnings = _.filter(errors, function(item) {
                        return item.indexOf('Warning:') == 0;
                    });
                    angular.forEach(uploadElementGroupViewModel.warnings, function(value, key) {
                        uploadElementGroupViewModel.warnings[key] = value.replace('Warning: ', '');
                    });
                    uploadElementGroupViewModel.infos = _.filter(errors, function(item) {
                        return item.indexOf('Info:') == 0;
                    });
                    angular.forEach(uploadElementGroupViewModel.infos, function(value, key) {
                        uploadElementGroupViewModel.infos[key] = value.replace('Info: ', '');
                    });
                    uploadElementGroupViewModel.errors = _.filter(errors, function(item) {
                        return item.indexOf('ERROR:') == 0;
                    });
                    angular.forEach(uploadElementGroupViewModel.errors, function(value, key) {
                        uploadElementGroupViewModel.errors[key] = value.replace('ERROR: ', '');
                    });
                    uploadElementGroupViewModel.fatals = _.filter(errors, function(item) {
                        return item.indexOf('FATAL:') == 0;
                    });
                    angular.forEach(uploadElementGroupViewModel.fatals, function(value, key) {
                        uploadElementGroupViewModel.fatals[key] = value.replace('FATAL: ', '');
                    });
                    if (uploadElementGroupViewModel.fatals.length > 0)
                        handleErrorService.handleErrors(uploadElementGroupViewModel.fatals, uploadElementGroupViewModel);
                    else {
                        $scope.log = 'Successful Upload: ' + config.file.name;
                        $scope.log += '\r\n';
                        $scope.log += uploadElementGroupViewModel.errors.length + ' error(s), ';
                        $scope.log += uploadElementGroupViewModel.warnings.length + ' warning(s), and ';
                        $scope.log += uploadElementGroupViewModel.infos.length + ' informational message(s).';
                        uploadElementGroupViewModel.uploadSuccessful = true;
                        angular.element(document.querySelector('#logMessage')).removeClass('bg-success').addClass('bg-warning');
                    }
                });
            }
        };

        uploadElementGroupViewModel.reset = function() {
            angular.element(document.querySelector("#uploadElementGroup")).modal('hide');
            $scope.files = '';
            $scope.log = '';
            uploadElementGroupViewModel.uploadSuccessful = false;
            uploadElementGroupViewModel.percent = 0;
            uploadElementGroupViewModel.stylepercent = '0%';
            $scope.$broadcast('show-errors-reset');
            $scope.uploadElementGroupForm.$setPristine();
            uploadElementGroupViewModel.resetFileInput();
            angular.element(document.querySelector('#submitFile')).addClass('disabled');
            angular.element(document.querySelector('#logMessage')).removeClass('bg-success');
        };

        uploadElementGroupViewModel.resetFileInput = function() {
            $('input[type=file]:first').val('');
            angular.element(document.querySelector("#uploadElementGroup")).modal('hide');
            $timeout(function() {
                $state.transitionTo($state.current, $stateParams, {
                    reload: true,
                    inherit: false,
                    notify: true
                });
            }, 250);
        };
    }
]);

