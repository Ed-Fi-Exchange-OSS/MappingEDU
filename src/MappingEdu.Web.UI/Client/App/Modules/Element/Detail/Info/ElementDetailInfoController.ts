// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.element.detail.info
//

var m = angular.module('app.modules.element.detail.info', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $stateProvider
        .state('app.element.detail.info', {
            url: '/info',
            data: {
                title: 'Element Detail Info'
            },
            templateUrl: `${settings.moduleBaseUri}/Element/Detail/Info/ElementDetailInfoView.tpl.html`,
            controller: 'app.modules.element.detail.info',
            controllerAs: 'elementDetailInfoViewModel',
            resolve: {
                elementDetail: ['repositories', '$stateParams', (repositories: IRepositories, $stateParams) => {
                    return repositories.systemItem.detail($stateParams.elementId, $stateParams.mappingProjectId);
                }]   
            }
        });
}]);


// ****************************************************************************
// Controller app.modules.element.detail.info
//

m.controller('app.modules.element.detail.info', ['$scope', '$stateParams', 'enumerations', 'modals', 'repositories', 'services', 'element', 'elementDetail', 'model',
    function ($scope, $stateParams, enumerations: IEnumerations, modals: IModals, repositories: IRepositories, services: IServices, element, elementDetail, model) {

        services.logger.debug('Loaded controller app.modules.element.detail.info');

        var vm = this;

        vm.mappingProjectId = $stateParams.mappingProjectId;
        vm.dataStandardId = $stateParams.dataStandardId;
        vm.element = element;

        vm.element.PreviousVersions = elementDetail.PreviousVersions;
        vm.element.NextVersions = elementDetail.NextVersions;
        vm.element.Notes = elementDetail.Notes;
        vm.element.EnumerationSystemItem = elementDetail.EnumerationSystemItem;

        if (vm.dataStandardId) vm.dataStandard = model;
        else vm.mappingProject = model;

        vm.edit = () => {
            var instance = modals.systemItemForm(vm.dataStandardId, vm.element, vm.element.ItemTypeId);
            instance.result.then((data) => {
                element.ItemName = data.ItemName;
                element.IsExtended = data.IsExtended;
                element.TechnicalName = data.TechnicalName;
                element.DataTypeSource = data.DataTypeSource;
                element.Definition = data.Definition;
                element.EnumerationSystemItemId = data.EnumerationSystemItemId;
                element.ItemUrl = data.ItemUrl;
                element.ItemDataTypeId = data.ItemDataTypeId;
                element.FieldLength = data.FieldLength;

                var length = element.PathSegments.length;
                element.PathSegments[length - 1].ItemName = data.ItemName;
                element.PathSegments[length - 1].IsExtended = data.IsExtended;
                element.reloadPath(element.PathSegments); //TODO: Find a better way to reload path
            });
        }

        vm.editDetails = () => {
            var instance = modals.systemItemCustomDetailForm(vm.element.SystemItemId, vm.element.SystemItemCustomDetails);
            instance.result.then((data) => {
                vm.element.SystemItemCustomDetails = data.SystemItemCustomDetails;
            });
        }
       
        vm.versionModal = (delta, isPrevious) => {
            if (!delta) delta = {};
            else {
                delta.Segments = isPrevious ? delta.OldSystemItemPathSegments : delta.NewSystemItemPathSegments;
                delta.ItemChangeTypeId = delta.ItemChangeType.Id;
            }

            var standard;
                
            if (isPrevious) {
                delta.NewSystemItemId = vm.element.SystemItemId;
                standard = vm.dataStandard.PreviousDataStandard;
            } else {
                delta.OldSystemItemId = vm.element.SystemItemId;
                standard = vm.dataStandard.NextDataStandard;
            }

            var instance = modals.deltaForm(isPrevious, delta, standard);

            //On Return
            instance.result.then((data) => {
                if (isPrevious) {
                    if (delta.PreviousVersionId) {
                        delta.OldSystemItemPathSegments = data.OldSystemItemPathSegments;
                        delta.OldSystemItemId = data.OldSystemItemId;
                        delta.Description = data.Description;
                        delta.ItemChangeType = data.ItemChangeType;
                    } else vm.element.PreviousVersions.push(data);
                } else {
                    if (delta.NextVersionId) {
                        delta.NewSystemItemPathSegments = data.NewSystemItemPathSegments;
                        delta.NewSystemItemId = data.NewSystemItemId;
                        delta.Description = data.Description;
                        delta.ItemChangeType = data.ItemChangeType;
                    } else vm.element.NextVersions.push(data);
                }
            });
        }


        vm.deletePreviousVersion = (version, index) => {
            repositories.element.previousVersionDelta.remove(vm.element.SystemItemId, version.PreviousVersionId).then(() => {
                services.logger.success('Removed previous version delta.');
                vm.element.PreviousVersions.splice(index, 1);
            }, error => {
                services.logger.error('Error removing previous version delta.', error.data);
            });
        }

        vm.deleteNextVersion = (version, index) => {
            repositories.element.nextVersionDelta.remove(vm.element.SystemItemId, version.NextVersionId).then(() => {
                services.logger.success('Removed next version delta.');
                vm.element.NextVersions.splice(index, 1);
            }, error => {
                services.logger.error('Error removing next version delta.', error.data);
            });
        }
    }
]);