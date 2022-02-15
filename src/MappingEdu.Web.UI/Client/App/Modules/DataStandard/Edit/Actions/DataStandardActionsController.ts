// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.data-standard.edit.actions
//

var m = angular.module('app.modules.data-standard.edit.actions', [
    'app.modules.data-standard.edit.actions.clone'
   ]);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.data-standard.edit.actions', {
            url: '/actions',
            data: {
                title: 'Data Standard Actions',
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Guest')].Id
            },
            templateUrl: `${settings.moduleBaseUri}/DataStandard/Edit/Actions/DataStandardActionsView.tpl.html`,
            controller: 'app.modules.data-standard.edit.actions',
            controllerAs: 'dataStandardActionsViewModel'
        });
}]);


// ****************************************************************************
// Controller app.modules.data-standard.edit.actions
//

m.controller('app.modules.data-standard.edit.actions', ['$scope', '$location', '$stateParams', 'repositories', 'services', 'settings',
    function ($scope, $stateParams, $location: ng.ILocationService, repositories: IRepositories, services: IServices, settings: ISystemSettings) {

        var vm = this;
        vm.dataStandard = $scope.$parent.dataStandardDetailViewModel.dataStandard;
        vm.showExtension = true;

        services.profile.me().then((data) => {
            vm.me = data;
        });

        $scope.$parent.dataStandardDetailViewModel.setTitle('ACTIONS');

        vm.delete = standard => {
            repositories.dataStandard.remove(standard.DataStandardId)
                .then(() => {
                    services.logger.success('Deleted data standard.');
                    services.timeout(() => {
                        services.state.go('app.home');
                    }, 500);
                }, error => {
                    services.logger.error('Deleting data standard.', error.data);
                })
                .catch(error => {
                    services.errors.handleErrors(error, vm);
                });
        };

        vm.clone = () => {
            var modal: ng.ui.bootstrap.IModalSettings = {
                templateUrl: `${settings.moduleBaseUri}/DataStandard/Edit/Actions/DataStandardClone.tpl.html`,
                controller: 'app.modules.data-standard.edit.actions.clone',
                size: 'lg',
                resolve: {
                    standard: () => { return vm.dataStandard },
                    standards: () => { return repositories.dataStandard.getAllWithoutNextVersions(); },
                    hasExtensions: () => { return repositories.dataStandard.extensions.hasExtensions(vm.dataStandard.DataStandardId); }
                }
            }

            var instance = services.modal.open(modal);
            instance.result.then((standard) => {
                services.profile.clearProfile();
                services.state.go('app.data-standard.edit', { dataStandardId: standard.DataStandardId });
                services.logger.success('Cloned data standard.');
            });
        }

        vm.uploadInterchange = () => {
            var modal: ng.ui.bootstrap.IModalSettings = {
                templateUrl: `${settings.moduleBaseUri}/DataStandard/Modals/UploadInterchange/UploadInterchange.tpl.html`,
                controller: 'app.modules.data-standard.modal.upload-interchange',
                size: 'lg',
                resolve: {
                    standard: () => { return vm.dataStandard },
                    standards: () => { return repositories.dataStandard.getAllWithoutNextVersions(); }
                }
            }

            var instance = services.modal.open(modal);
        }

        vm.dataStandardExportUrl = `${settings.apiBaseUri}/DataStandard/${vm.dataStandard.DataStandardId}/export`;

        vm.export = () => {

            return repositories.dataStandard.getExportToken(vm.dataStandard.DataStandardId).then((data) => {
                var link = document.createElement('a');
                link.href = `${settings.apiBaseUri}/DataStandard/${vm.dataStandard.DataStandardId}/export/${data}`;
                document.body.appendChild(link);
                link.click();
            }, error => {
                services.logger.error('Error exporting data standard.', error.data);
            });

        }

        vm.upload = () => {
            var modal: ng.ui.bootstrap.IModalSettings = {
                backdrop: 'static',
                keyboard: false,
                templateUrl: `${settings.moduleBaseUri}/DataStandard/Modals/UploadDefinition/UploadDefinition.tpl.html`,
                controller: 'app.modules.data-standard.modal.upload-definition',
                size: 'lg',
                resolve: {
                    standard: () => { return vm.dataStandard }
                }
            }

            var instance = services.modal.open(modal);
            instance.result.then((standard) => {
                services.state.go('app.data-standard.edit', { dataStandardId: standard.DataStandardId });
                services.logger.success('Cloned data standard.');
            });
        }

        vm.uploadExtension = () => {

            var modal = {
                backdrop: 'static',
                keyboard: false,
                templateUrl: `${settings.moduleBaseUri}/dataStandard/modals/UploadExtension/uploadExtension.tpl.html`,
                controller: 'app.modules.data-standard.modal.upload-extension',
                size: 'lg',
                resolve: {
                    standard: () => { return vm.dataStandard }
                }
            }
            var instance = services.modal.open(modal);

            //On Return
            instance.result.then(() => {

            });
        }

        vm.uploadJson = () => {
            var modal = {
                backdrop: 'static',
                keyboard: false,
                templateUrl: `${settings.moduleBaseUri}//dataStandard/modals/UploadSwagger/uploadSwaggerApi.tpl.html`,
                controller: 'app.modules.data-standard.modal.upload-swagger-api',
                size: 'lg',
                resolve: {
                    standard: () => { return vm.dataStandard }
                }
            }
            var instance = services.modal.open(modal);

            //On Return
            instance.result.then(() => {

            });
        }
    }
]);
