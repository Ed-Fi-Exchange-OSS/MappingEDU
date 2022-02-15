// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.data-standard.import
//

var m = angular.module('app.repositories.data-standard.import', ['restangular']);


// ****************************************************************************
// Interface IDataStandardImportRepository
//

interface IDataStandardImportRepository {
    odsApi(id: string, model: any): angular.IPromise<any>;
}

// ****************************************************************************
// Repository Import
//

m.factory('app.repositories.data-standard.import', ['Restangular', (restangular: restangular.IService) => {

    // methods

    function odsApi(id: string, model: any) {
        return restangular.one('Import', id).customPOST(model, 'ODSApi');
    }

    var repository: IDataStandardImportRepository = {
        odsApi: odsApi
    };

    return repository;
}]); 