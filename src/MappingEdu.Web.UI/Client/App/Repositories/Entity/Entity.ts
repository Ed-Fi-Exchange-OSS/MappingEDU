// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.entity
//

var m = angular.module('app.repositories.entity', [
    'restangular',
     'app.repositories.entity.hint']);


// ****************************************************************************
// Interface Entity
//
 
interface IEntity extends restangular.IElement {
    //TODO
}

// ****************************************************************************
// Interface IEntityRepository
//

interface IEntityRepository {
    find(mappingProjectId: string, id: string): angular.IPromise<any>;
    getFirstLevelEntities(mappedSystemId: string): angular.IPromise<any>;
    remove(id: string): angular.IPromise<any>;
    hint: IEntityHintRepository;
}

// ****************************************************************************
// Entity repository
//

m.factory('app.repositories.entity', ['Restangular', 'app.repositories.entity.hint',  (restangular: restangular.IService, hint: IEntityHintRepository) => {

    restangular.extendModel('Entity', (model: IEntity) => {
        return model;
    });

    // methods

    function find(mappingProjectId: string, id: string) {
        return restangular.one('Entity/' + mappingProjectId, id).get();
    }

    function getFirstLevelEntities(mappedSystemId) {
        return restangular.one(`Entity/${mappedSystemId}/first-level`).get();
    }

    function remove(id: string) {
        return restangular.one('Entity', id).remove();
    }

    var repository: IEntityRepository = {
        find: find,
        getFirstLevelEntities: getFirstLevelEntities,
        remove: remove,
        hint: hint
    };

    return repository;
}]); 



