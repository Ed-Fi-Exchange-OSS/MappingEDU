// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.element.enumeration
//

var m = angular.module('app.repositories.element.enumeration', ['restangular']);


// ****************************************************************************
// Interface Enumeration
//
 
interface IEnumeration extends restangular.IElement {
    //TODO
}

// ****************************************************************************
// Interface IEnumerationItemRepository
//

interface IEnumerationRepository {
    find(id: string): angular.IPromise<any>;
    remove(id: string): angular.IPromise<any>;
}

// ****************************************************************************
// Enumeration repository
//

m.factory('app.repositories.element.enumeration', ['Restangular', (restangular: restangular.IService) => {

    restangular.extendModel('Enumeration', (model: IEnumeration) => {
        return model;
    });

    // methods

    function find(id: string) {
        return restangular.one('Enumeration',  id).get();
    }

    function remove(id: string) {
        return restangular.one('Enumeration', id).remove();
    }

    var repository: IEnumerationRepository = {
        find: find,
        remove: remove
    };

    return repository;
}]); 



