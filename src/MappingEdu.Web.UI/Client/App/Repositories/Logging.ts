// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.repositories.logging
//

var m = angular.module('app.repositories.logging', ['restangular']);


// ****************************************************************************
// Interface ILoggingRepository
//

interface ILoggingRepository {
    add(model: any): angular.IPromise<any>;
    clearLogs(model: any): angular.IPromise<any>;
    exportLogs(model: any): angular.IPromise<any>;
    getExportLogsCount(model: any): angular.IPromise<any>;
}

// ****************************************************************************
// Logging repository
//

m.factory('app.repositories.logging', ['Restangular', (restangular: restangular.IService) => {

    // methods
    function add(model: any) {
        return restangular.one('Logging').customPOST(model);
    }

    function clearLogs(model: any) {
        return restangular.one('Logging/Clear').customPOST(model);
    }

    function exportLogs(model: any) {
        return restangular.one('Logging/Export').customPOST(model);
    }

    function getExportLogsCount(model: any) {
        return restangular.one('Logging/Export-Count').customPOST(model);
    }

    var repository: ILoggingRepository = {
        add: add,
        clearLogs: clearLogs,
        exportLogs: exportLogs,
        getExportLogsCount: getExportLogsCount
    };

    return repository;
}]); 