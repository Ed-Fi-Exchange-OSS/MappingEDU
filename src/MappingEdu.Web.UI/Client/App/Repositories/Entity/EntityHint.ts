// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.entity.hint
//

var m = angular.module('app.repositories.entity.hint', ['restangular']);


// ****************************************************************************
// Interface Entity Hint
//
 
interface IEntityHint extends restangular.IElement {
    //TODO
}

// ****************************************************************************
// Interface IEntityHintRepository
//

interface IEntityHintRepository {
    getAll(mappingProjectId: string): angular.IPromise<Array<any>>;
    create(mappingProjectId: string, data: any): angular.IPromise<any>;
    save(mappingProjectId: string, data: any): angular.IPromise<any>;
    remove(mappingProjectId: string, entityHintId: any): angular.IPromise<any>;
    filter(mappingProjectId: string, systemItemId: any): angular.IPromise<any>;
}

// ****************************************************************************
// Repository Entity Hint
//

m.factory('app.repositories.entity.hint', ['Restangular', (restangular: restangular.IService) => {

    // methods

    function getAll(mappingProjectId: string) {
        return restangular.one('EntityHint', mappingProjectId).get();
    }

    function create(mappingProjectId: string, data: any) {
        return restangular.one(`EntityHint/${mappingProjectId}`).customPOST(data);
    }

    function save(mappingProjectId: string, data: any) {
        return restangular.one(`EntityHint/${mappingProjectId}`, data.EntityHintId).customPUT(data);
    }

    function filter(mappingProjectId: string, systemItemId: string) {
        return restangular.one(`EntityHint/${mappingProjectId}/filter/${systemItemId}`).get();
    }

    function remove(mappingProjectId: string, entityHintId: string) {
        return restangular.one(`EntityHint/${mappingProjectId}`, entityHintId).remove();
    }

    var repository: IEntityHintRepository = {
        getAll: getAll,
        create: create,
        filter: filter,
        save: save,
        remove: remove
    };

    return repository;
}]); 
