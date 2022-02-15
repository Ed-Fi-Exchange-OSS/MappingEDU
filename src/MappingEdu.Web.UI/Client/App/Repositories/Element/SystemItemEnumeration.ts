// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.element.system-item-enumeration
//

var m = angular.module('app.repositories.element.system-item-enumeration', ['restangular']);


// ****************************************************************************
// Interface SystemItemEnumeration
//
 
interface ISystemItemEnumeration extends restangular.IElement {
    //TODO
}

// ****************************************************************************
// Interface ISystemItemEnumerationRepository
//

interface ISystemItemEnumerationRepository {
    find(id: string): angular.IPromise<any>;
}

// ****************************************************************************
// System Item Enumeration repository
//

m.factory('app.repositories.element.system-item-enumeration', ['Restangular', (restangular: restangular.IService) => {

    restangular.extendModel('SystemItemEnumeration', (model: ISystemItemEnumeration) => {
        return model;
    });

    // methods

    function find(id: string) {
        return restangular.one('SystemItemEnumeration', id).get();
    }

    var repository: ISystemItemEnumerationRepository = {
        find: find
    };

    return repository;
}]); 