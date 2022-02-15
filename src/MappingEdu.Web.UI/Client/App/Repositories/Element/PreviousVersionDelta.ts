// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.element.previous-version-delta
//

var m = angular.module('app.repositories.element.previous-version-delta', ['restangular']);


// ****************************************************************************
// Interface IPreviousVersionDelta
//
 
interface IPreviousVersionDelta extends restangular.IElement {
    //TODO
}

// ****************************************************************************
// Interface IPreviousVersionDeltaRepository
//

interface IPreviousVersionDeltaRepository {
    create(id: string, data: any): angular.IPromise<any>;
    save(id: string, id2: string, data: any): angular.IPromise<any>;
    remove(id: string, id2: string): angular.IPromise<any>;
}

// ****************************************************************************
// Previous Version Delta repository
//

m.factory('app.repositories.element.previous-version-delta', ['Restangular', (restangular: restangular.IService) => {

    restangular.extendModel('PreviousVersionDelta', (model: IPreviousVersionDelta) => {
        return model;
    });

    // methods

    function create(id: string, data: any) {
        return restangular.one('PreviousVersionDelta/' + id).customPOST(data);
    }

    function save(id: string, id2: string, data: any) {
        return restangular.one(`PreviousVersionDelta/${id}/`, id2).customPUT(data);
    }

    function remove(id: string, id2: string) {
        return restangular.one('PreviousVersionDelta/' + id, id2).remove();
    }

    var repository: IPreviousVersionDeltaRepository = {
        create: create,
        save: save,
        remove: remove
    };

    return repository;
}]); 


