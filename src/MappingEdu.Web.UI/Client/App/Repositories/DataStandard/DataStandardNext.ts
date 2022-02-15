// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.data-standard.next
//

var m = angular.module('app.repositories.data-standard.next', ['restangular']);


// ****************************************************************************
// Interface IDataStandardTargetRepository
//

interface IDataStandardNextRepository {
    get(id: string): angular.IPromise<any>;
}

// ****************************************************************************
// Repository Next
//

m.factory('app.repositories.data-standard.next', ['Restangular', (restangular: restangular.IService) => {

    // methods

    function get(id: string) {
        return restangular.one('NextDataStandard', id).get();
    }

    var repository: IDataStandardNextRepository = {
        get: get
    };

    return repository;
}]); 