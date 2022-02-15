// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.mapping-project.status
//

var m = angular.module('app.repositories.mapping-project.status', ['restangular']);


// ****************************************************************************
// Interface IMappingProjectStatus 
//
 
interface IMappingProjectStatus extends restangular.IElement {
    //TODO
}


// ****************************************************************************
// Interface IMappingProjectStatusRepository
//

interface IMappingProjectStatusRepository {
    get(id: string): angular.IPromise<any>;
}

// ****************************************************************************
// Repository Approve All System Item Maps
//

m.factory('app.repositories.mapping-project.status', ['Restangular', (restangular: restangular.IService) => {

    restangular.extendModel('MappingProjectStatus', (model: IMappingProjectStatus) => {
        return model;
    });

    // methods

    function get(id: string) {
        return restangular.one('MappingProjectStatus', id).get();
    }

    var repository: IMappingProjectStatusRepository = {
        get: get
    };

    return repository;
}]); 