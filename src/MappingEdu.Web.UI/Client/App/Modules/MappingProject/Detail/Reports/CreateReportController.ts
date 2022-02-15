// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.detail.reports.create

var m = angular.module('app.modules.mapping-project.detail.reports.create', []);


// ****************************************************************************
// Controller app.modals.share
//

m.controller('app.modules.mapping-project.detail.reports.create', ['$scope', '$uibModalInstance', 'repositories', 'services', 'enumerations', 'mappingProjectId', 'isTargetReport', 'customDetailMetadata', 'viewOnly',
    ($scope, $uibModalInstance: angular.ui.bootstrap.IModalServiceInstance, repositories: IRepositories, services: IServices, enumerations: IEnumerations, mappingProjectId, isTargetReport, customDetailMetadata, viewOnly) => {

        services.logger.debug('Loaded controller app.modules.mapping-project.detail.reports.create');

        $scope.isTargetReport = isTargetReport;

        $scope.customDetailMetadata = [];

        $scope.elementListColumns = enumerations.ElementListColumns;
        if (viewOnly) {
            $scope.availableElementMappingColumns = (isTargetReport) ? angular.copy(enumerations.GapElementMappingColumns).splice(0, 9) : angular.copy(enumerations.ElementMappingColumns).slice(0, 13);
            $scope.availableEnumerationMappingColumns = (isTargetReport) ? angular.copy(enumerations.GapEnumerationMappingColumns).slice(0, 11) : angular.copy(enumerations.EnumerationMappingColumns).slice(0, 13);
        } else {
            $scope.availableElementMappingColumns = (isTargetReport) ? angular.copy(enumerations.GapElementMappingColumns) : angular.copy(enumerations.ElementMappingColumns);
            $scope.availableEnumerationMappingColumns = (isTargetReport) ? angular.copy(enumerations.GapEnumerationMappingColumns) : angular.copy(enumerations.EnumerationMappingColumns);
        }
        $scope.enumerationListColumns = angular.copy(enumerations.EnumerationListColumns);

        var i = 4;
        $scope.elementMappingColumns = angular.copy($scope.availableElementMappingColumns);
        $scope.enumerationMappingColumns = angular.copy($scope.availableEnumerationMappingColumns);

        angular.forEach(customDetailMetadata, detail => {
            $scope.elementMappingColumns.splice(i, 0, { Id: detail.CustomDetailMetadataId, DisplayText: detail.DisplayName, IsCustomDetail: true });
            $scope.enumerationMappingColumns.splice(i - 1, 0, { Id: detail.CustomDetailMetadataId, DisplayText: detail.DisplayName, IsCustomDetail: true });
            i++;
        });

        $scope.enumerationMappingStatusTypes = angular.copy(enumerations.EnumerationMappingStatusType);
        $scope.enumerationMappingStatusReasonTypes = angular.copy(enumerations.EnumerationMappingStatusReasonType);

        $scope.mappingMethods = angular.copy(enumerations.MappingMethodTypeInQueue);
        $scope.workflowStatuses = angular.copy(enumerations.WorkflowStatusType);

        $scope.mappingMethods.splice(3, 1);

        if (isTargetReport) $scope.mappingMethods = [{ DisplayText: 'Unmapped', Id: 0 }, { DisplayText: 'Mapped', Id: 1 }];

        $scope.enumerationMappingStatusTypes = $scope.enumerationMappingStatusTypes.concat([{ DisplayText: 'Unmapped', Id: 0 }]);

        //remove null id/displaytext
        $scope.enumerationMappingStatusTypes.splice(0, 1);
        $scope.enumerationMappingStatusReasonTypes.splice(0, 1);

        $scope.elementList = {
            Show: true,
            MappingMethods: [],
            WorkflowStatuses: [],
            Columns: enumerations.ElementListColumns
        };
        $scope.elementMappingList = {
            Show: true,
            Columns: $scope.availableElementMappingColumns
        };
        $scope.enumerationList = {
            Show: true,
            Columns: enumerations.EnumerationListColumns
        };
        $scope.enumerationMappingList = {
            Show: true,
            EnumerationMappingStatusTypes: [],
            EnumerationMappingStatusReasonTypes: [],
            Columns: $scope.availableEnumerationMappingColumns
        };

        $scope.close = () => {
            $uibModalInstance.dismiss();
        }

        $scope.download = () => {

            var elementMappingList = angular.copy($scope.elementMappingList);
            elementMappingList.MappingMethods = services.underscore.pluck(elementMappingList.MappingMethods, 'Id');
            elementMappingList.WorkflowStatuses = services.underscore.pluck(elementMappingList.WorkflowStatuses, 'Id');

            var enumerationMappingList = angular.copy($scope.enumerationMappingList);
            enumerationMappingList.EnumerationMappingStatusTypes = services.underscore.pluck(enumerationMappingList.EnumerationMappingStatusTypes, 'Id');
            enumerationMappingList.EnumerationMappingStatusReasonTypes = services.underscore.pluck(enumerationMappingList.EnumerationMappingStatusReasonTypes, 'Id');

            var createReport = {
                ElementList: $scope.elementList,
                ElementMappingList: elementMappingList,
                EnumerationList: $scope.enumerationList,
                EnumrationMappingList: enumerationMappingList
            };

            if (!isTargetReport) {
                return repositories.mappingProject.reports.getToken(mappingProjectId, createReport).then((data) => {
                    var link = document.createElement('a');
                    link.href = `api/MappingProjectReports/${mappingProjectId}/report/${data}`;
                    document.body.appendChild(link);
                    link.click();
                });
            } else {
                return repositories.mappingProject.reports.getTargetToken(mappingProjectId, createReport).then((data) => {
                    var link = document.createElement('a');
                    link.href = `api/MappingProjectReports/${mappingProjectId}/report/${data}`;
                    document.body.appendChild(link);
                    link.click();
                });
            }
        }
    }
]);
