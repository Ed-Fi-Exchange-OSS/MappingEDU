// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.data-standard.elements
//

var m = angular.module('app.repositories.data-standard.elements', ['restangular']);


// ****************************************************************************
// Interface IDataStandardElementsRepository
//

interface IDataStandardElementsRepository {
    get(id: string): angular.IPromise<any>;
    getElements(id: string, data: any): angular.IPromise<any>;
}

// ****************************************************************************
// Repository Elements
//

m.factory('app.repositories.data-standard.elements', ['Restangular', (restangular: restangular.IService) => {

    // methods

    function get(id: string) {
        return restangular.one('ElementList', id).get();
    }

    function getElements(id: string, data: any) {
        return restangular.all(`ElementList/${id}/elements`).post(data, null, {
            "Content-Type": 'application/x-www-form-urlencoded; charset=UTF-8'
        });
    }

    var repository: IDataStandardElementsRepository = {
        get: get,
        getElements: getElements
    };

    return repository;
}]); 