// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.element.enumeration-item-mapping
//

var m = angular.module('app.repositories.element.enumeration-item-mapping', ['restangular']);


// ****************************************************************************
// Interface EnumerationItemMapping
//
 
interface IEnumerationItemMapping extends restangular.IElement {
    //TODO
}

// ****************************************************************************
// Interface IEnumerationItemMappingRepository
//

interface IEnumerationItemMappingRepository {
    find(id: string): angular.IPromise<any>;
    create(id: string, data: any): angular.IPromise<any>;
    save(id: string, id2: string, data: any): angular.IPromise<any>;
    remove(id: string, id2: string): angular.IPromise<any>;
}

// ****************************************************************************
// Repository EnumerationItemMapping
//

m.factory('app.repositories.element.enumeration-item-mapping', ['Restangular', (restangular: restangular.IService) => {

    // methods

    function find(id: string) {
        return restangular.one('EnumerationItemMapping', id).get();
    }

    function create(id: string, data: any) {
        return restangular.one('EnumerationItemMapping/' + id).customPOST(data);
    }

    function save(id: string, id2: string, data: any) {
        return restangular.one('EnumerationItemMapping/' + id, id2).customPUT(data);
    }

    function remove(id: string, id2: string) {
        return restangular.one('EnumerationItemMapping/' + id, id2).remove();
    }

    var repository: IEnumerationItemMappingRepository = {
        find: find,
        create: create,
        save: save,
        remove: remove
    };

    return repository;
}]); 

