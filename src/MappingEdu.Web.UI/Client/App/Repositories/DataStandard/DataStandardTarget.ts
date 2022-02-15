// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.data-standard.target
//

var m = angular.module('app.repositories.data-standard.target', ['restangular']);


// ****************************************************************************
// Interface IDataStandardTargetRepository
//

interface IDataStandardTargetRepository {
    get(id: string): angular.IPromise<any>;
}

// ****************************************************************************
// Data Standard Target repository
//

m.factory('app.repositories.data-standard.target', ['Restangular', (restangular: restangular.IService) => {

    // methods

    function get(id: string) {
        return restangular.one('DataStandardTargetMappingProjects', id).get();
    }

    var repository: IDataStandardTargetRepository = {
        get: get
    };

    return repository;
}]); 