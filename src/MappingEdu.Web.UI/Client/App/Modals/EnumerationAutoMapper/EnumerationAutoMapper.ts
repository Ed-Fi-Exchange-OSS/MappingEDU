// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modals.enumeration-auto-mapper
//

var m = angular.module('app.modals.enumeration-auto-mapper', []);

// ****************************************************************************
// Service app.modals.enumeration-auto-mapper
//

m.factory('app.modals.enumeration-auto-mapper', ['settings', 'services', (settings, services) => {
    return (sourceEnumerationItems, targetEnumerationItems) => {

        var modal: ng.ui.bootstrap.IModalSettings = {
            backdrop: 'static',
            keyboard: false,
            templateUrl: `${settings.modalBaseUri}/EnumerationAutoMapper/EnumerationAutoMapper.tpl.html`,
            controller: 'app.modals.enumeration-auto-mapper',
            size: 'lg',
            resolve: {
                sourceEnumerationItems: () => { return sourceEnumerationItems; },
                targetEnumerationItems: () => { return targetEnumerationItems; }
            }
        }
        return services.modal.open(modal);
    }
}]);


// ****************************************************************************
// Controller app.modals.enumeration-auto-mapper
//

m.controller('app.modals.enumeration-auto-mapper', ['$scope', '$uibModalInstance', 'repositories', 'services', 'sourceEnumerationItems', 'targetEnumerationItems',
    ($scope, $uibModalInstance: angular.ui.bootstrap.IModalServiceInstance, repositories: IRepositories, services: IServices, sourceEnumerationItems, targetEnumerationItems ) => {

        services.logger.debug('Loaded controller app.modals.enumeration-auto-mapper');

        var unmappedEnumerationItems = services.underscore.filter(<Array<any>>angular.copy(sourceEnumerationItems), item => !item.Mapping.TargetSystemEnumerationItemId);

        $scope.enumerations = [];
        angular.forEach(unmappedEnumerationItems, sourceItem => {

            var target = null;
            var properties = ['CodeValue', 'ShortDescription', 'Description'];

            for (var index1 in properties) {
                var prop = properties[index1];
                for (var index2 in properties) {
                    var prop2 = properties[index2];
                    if (sourceItem[prop2] && !target) {
                        target = services.underscore.find(<Array<any>>angular.copy(targetEnumerationItems), item =>
                            ((item[prop]) ? item[prop].toLowerCase().replace(' ', '') : null) === sourceItem[prop2].toLowerCase().replace(' ', ''));
                    }
                }
            }

            if (target) {
                sourceItem.ApproveMapping = true;
                sourceItem.TargetSystemEnumerationItem = target;
                sourceItem.Mapping = {
                    DeferredMapping: false,
                    SourceCodeValue: sourceItem.Mapping.SourceCodeValue,
                    SourceSystemEnumerationItemId: sourceItem.Mapping.SourceSystemEnumerationItemId,
                    TargetCodeValue: target.CodeValue,
                    TargetSystemEnumerationItemId: target.SystemEnumerationItemId,
                    TargetSystemItem: target.SystemItem
                }
                $scope.enumerations.push(sourceItem);
            } 
        });

        $scope.save = () =>
        {
            var accepted = services.underscore.filter(<Array<any>>$scope.enumerations, enumeration => enumeration.ApproveMapping);
            $uibModalInstance.close(accepted);
        }

        $scope.close = () => {
            $uibModalInstance.dismiss();
        }

        $scope.getTargetPath = targetSystemItem => {
            if (!targetSystemItem)
                return '';

            if (targetSystemItem.ParentSystemItem)
                return $scope.getTargetPath(targetSystemItem.ParentSystemItem) + '.' + targetSystemItem.ItemName;

            return targetSystemItem.ItemName;
        };
    }
]);
