// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.data-standard.source
//

var m = angular.module('app.repositories.data-standard.source', ['restangular']);


// ****************************************************************************
// Interface Data Standard Source Repository
//

interface IDataStandardSourceRepository {
    get(id: string): angular.IPromise<any>;
}

// ****************************************************************************
// Data Standard Source repository
//

m.factory('app.repositories.data-standard.source', ['Restangular', (restangular: restangular.IService) => {

    // methods

    function get(id: string) {
        return restangular.one('DataStandardSourceMappingProjects', id).get();
    }

    var repository: IDataStandardSourceRepository = {
        get: get
    };

    return repository;
}]); 
