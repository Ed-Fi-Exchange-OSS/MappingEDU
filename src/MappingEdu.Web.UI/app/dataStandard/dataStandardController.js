// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appDataStandard')
    .controller('dataStandardController', [
        '_', '$scope', '$timeout', '$state', 'dataStandardService', 'breadcrumbService', 'handleErrorService',
        function(_, $scope, $timeout, $state, dataStandardService, breadcrumbService, handleErrorService) {
            var dataStandardViewModel = this;
            dataStandardViewModel.name = "dataStandardController";

            if ($state.current.name == "createDataStandard")
                breadcrumbService.withHome().withCurrent();

            dataStandardViewModel.getDataStandards = function() {
                dataStandardViewModel.loading = true;
                dataStandardService.getAll()
                    .then(function(data) {
                        dataStandardViewModel.dataStandards = data;
                        dataStandardViewModel.loading = false;
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, dataStandardViewModel);
                    });
            };

            dataStandardViewModel.getDataStandards();

            dataStandardViewModel.cancel = function(dataStandard) {
                $scope.$broadcast('show-errors-reset');
                if (!_.isUndefined(dataStandard) && !_.isUndefined(dataStandard.DataStandardId)) {
                    angular.element(document.querySelector("#editDataStandardModal")).modal('hide');
                } else
                    $state.go('home');
            };

            dataStandardViewModel.save = function(dataStandard) {
                $scope.$broadcast('show-errors-check-valid');
                if (_.isUndefined(dataStandard) || $scope.dataStandardForm.$invalid)
                    return;

                var existing = _.find(dataStandardViewModel.dataStandards, function(standard) {
                    return angular.equals(standard.SystemName.toLowerCase(), dataStandard.SystemName.toLowerCase()) &&
                        angular.equals(standard.SystemVersion.toLowerCase(), dataStandard.SystemVersion.toLowerCase()) &&
                        (_.isUndefined(dataStandard.DataStandardId) || !angular.equals(standard.DataStandardId, dataStandard.DataStandardId));
                });

                if (existing) {
                    handleErrorService.addError('Data Standard Name and Version must be unique.', dataStandardViewModel);
                    return;
                }

                if (!_.isUndefined(dataStandard.DataStandardId)) {
                    dataStandardService.update(dataStandard.DataStandardId, dataStandard)
                        .then(function() {
                            dataStandardViewModel.success = true;
                            angular.element(document.querySelector('#editDataStandardModal')).modal('hide');
                        })
                        .catch(function(error) {
                                handleErrorService.handleErrors(error, dataStandardViewModel);
                            }
                        );
                } else {
                    dataStandardService.add(dataStandard)
                        .then(function(data) {
                            dataStandardViewModel.success = true;
                            $state.go('dataStandard.elementGroups', { id: data.DataStandardId });
                        })
                        .catch(function(error) {
                                handleErrorService.handleErrors(error, dataStandardViewModel);
                            }
                        );
                }
            };

            dataStandardViewModel.delete = function(dataStandard) {
                dataStandardService.delete(dataStandard.DataStandardId)
                    .then(function() {
                        dataStandardViewModel.success = true;
                        $timeout(function() { dataStandardViewModel.success = false; }, 2000);
                    })
                    .catch(function(error) {
                            handleErrorService.handleErrors(error, dataStandardViewModel);
                        }
                    );
            };
        }
    ]);