// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.manage.logs.clear
//

var m = angular.module('app.modules.manage.logs.export', []);


// ****************************************************************************
// Controller app.modules.manage.logs.export
//

m.controller('app.modules.manage.logs.export', ['$scope', '$uibModalInstance', 'repositories', 'settings', 'services', 'exportModel', 'levels',
    ($scope, $uibModalInstance: angular.ui.bootstrap.IModalServiceInstance, repositories: IRepositories, settings: ISystemSettings, services: IServices, exportModel, levels) => {

        services.logger.debug('Loaded controller app.modules.manage.logs.export');

        $scope.today = new Date();
        $scope.model = angular.copy(exportModel);
        $scope.levels = levels;
        $scope.loading = 0;

        $scope.columns = [
            { column: 0, display: 'Level' },
            { column: 1, display: 'User' },
            { column: 2, display: 'Message' },
            { column: 3, display: 'Date' }
        ];

        $scope.directions = [
            { direction: 'asc', display: 'Ascending' },
            { direction: 'desc', display: 'Decending' }
        ];

        $scope.getExportCount = () => {
            var model = angular.copy($scope.model);
            model.StartDate = services.filter('date')($scope.model.StartDate, 'MM/d/yyyy h:mm:ss a');

            //Adds 1 day to enddate
            var endDate = new Date($scope.model.EndDate);
            endDate.setDate(endDate.getDate() + 1);

            model.EndDate = services.filter('date')(endDate, 'MM/d/yyyy h:mm:ss a');

            $scope.loading++;
            return repositories.logging.getExportLogsCount(model).then((data) => {
                $scope.logCount = data;
            }, error => {
                services.logger.error('Error retreiving count', error);
            }).finally(() => {
                $scope.loading--;
            });
        }
        $scope.getExportCount();

        $scope.export = () => {
            var model = angular.copy($scope.model);
            model.StartDate = services.filter('date')($scope.model.StartDate, 'MM/d/yyyy h:mm:ss a');

            //Adds 1 day to enddate
            var endDate = new Date($scope.model.EndDate);
            endDate.setDate(endDate.getDate() + 1);

            model.EndDate = services.filter('date')(endDate, 'MM/d/yyyy h:mm:ss a');

            return repositories.logging.exportLogs(model).then((data) => {
                var link = document.createElement('a');
                link.href = `${settings.apiBaseUri}/Logging/export/${data}`;
                document.body.appendChild(link);
                link.click();
            }, error => {
                services.logger.error('Error retreiving logs', error);
            });
        }

        $scope.close = () => {
            $uibModalInstance.dismiss();
        }
    }
]);
