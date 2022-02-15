// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.data-standard.edit.extensions
//

var m = angular.module('app.modules.data-standard.edit.extensions.list', [
    'app.modules.data-standard.edit.extensions.list.delete',
    'app.modules.data-standard.edit.extensions.list.link'
]);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', 'enumerations', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.data-standard.edit.extensions.list', {
            url: '',
            data: {
                title: 'Extensions',
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Owner')].Id
            },
            templateUrl: `${settings.moduleBaseUri}/DataStandard/Edit/Extensions/List/DataStandardExtensionsListView.tpl.html`,
            controller: 'app.data-standard.edit.extensions.list',
            controllerAs: 'dataStandardExtensionsViewModel',
            resolve: {
                me: ['services', (services: IServices) => {
                    return services.profile.me();
                }]
            }
        });
}]);


// ****************************************************************************
// Controller app.data-standard.edit.extensions.list
//

m.controller('app.data-standard.edit.extensions.list', ['$scope', '$stateParams', 'repositories', 'services', 'settings', 'extensions', 'me',
    function ($scope, $stateParams: any, repositories: IRepositories, services: IServices, settings: ISystemSettings, extensions, me) {

        services.logger.debug('Loaded controller app.data-standard.edit.extensions');
        $scope.$parent.dataStandardDetailViewModel.setTitle('EXTENSIONS');

        var vm = this;
        vm.dataStandard = $scope.$parent.dataStandardDetailViewModel.dataStandard;
        vm.extensions = angular.copy(extensions);
        vm.me = me;

        vm.dtOptions = services.datatables.optionsBuilder.newOptions();

        vm.columnDefs = [
            services.datatables.columnDefBuilder.newColumnDef(2).notSortable(),
            services.datatables.columnDefBuilder.newColumnDef(3).notSortable()
        ];

        vm.link = () => {
           return repositories.dataStandard.extensions.getLinkableStandards($stateParams.dataStandardId).then(standards => {
                var modal: ng.ui.bootstrap.IModalSettings = {
                    templateUrl: `${settings.moduleBaseUri}/DataStandard/Edit/Extensions/List/Link/LinkExtensionView.tpl.html`,
                    controller: 'app.modules.data-standard.edit.extensions.list.link',
                    windowClass: 'link-modal',
                    resolve: {
                        dataStandardId: () => { return $stateParams.dataStandardId },
                        standards: () => { return standards },
                        extension: () => { return {} }
                    }
                }

                var instance = services.modal.open(modal);
                instance.result.then((extension) => {
                    vm.extensions.push(extension);
                    angular.copy(vm.extensions, extensions);
                });
            });
        }

        vm.edit = (extension, index) => {
            var modal: ng.ui.bootstrap.IModalSettings = {
                templateUrl: `${settings.moduleBaseUri}/DataStandard/Edit/Extensions/List/Link/LinkExtensionView.tpl.html`,
                controller: 'app.modules.data-standard.edit.extensions.list.link',
                windowClass: 'link-modal',
                resolve: {
                    dataStandardId: () => { return $stateParams.dataStandardId },
                    standards: () => {
                        return [{
                            DataStandardId: extension.ExtensionMappedSystemId,
                            SystemName: extension.ExtensionMappedSystemName,
                            SystemVersion: extension.ExtensionMappedSystemVersion
                        }];
                    },
                    extension: () => { return extension }
                }
            }

            var instance = services.modal.open(modal);
            instance.result.then((result) => {
                extension.ShortName = result.ShortName;
                angular.copy(vm.extensions, extensions);
            });
        }

        vm.delete = (extension, index) => {
            var modal: ng.ui.bootstrap.IModalSettings = {
                templateUrl: `${settings.moduleBaseUri}/DataStandard/Edit/Extensions/List/Delete/DeleteExtensionView.tpl.html`,
                controller: 'app.modules.data-standard.edit.extensions.list.delete',
                resolve: {
                    dataStandardId: () => { return $stateParams.dataStandardId },
                    extensionId: () => { return extension.MappedSystemExtensionId }
                }
            }

            var instance = services.modal.open(modal);
            instance.result.then(() => {
                vm.extensions.splice(index, 1);
                angular.copy(vm.extensions, extensions);
            });
        }

        vm.downloadReport = () => {
            return repositories.dataStandard.extensions.downloadableReport($stateParams.dataStandardId).then(() => {

            }, error => {
                services.logger.error("Error loading report", error);
            });
        }

        vm.togglePublicExtensions = () => {
            repositories.dataStandard.togglePublicExtensions(vm.dataStandard.DataStandardId).then(() => {
                vm.dataStandard.AreExtensionsPublic = !vm.dataStandard.AreExtensionsPublic;
                services.logger.success('Changed public status.');
            }, error => {
                services.logger.error('Error changing public status.', error);
            });
        }

    }]);
