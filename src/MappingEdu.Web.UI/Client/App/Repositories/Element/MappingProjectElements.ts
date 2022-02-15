// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.element.mapping-project-elements
//

var m = angular.module('app.repositories.element.mapping-project-elements', ['restangular']);


// ****************************************************************************
// Interface MappingProjectElements
//
 
interface IMappingProjectElements extends restangular.IElement {
    //TODO
}

// ****************************************************************************
// Interface IMappingProjectElementsRepository
//

interface IMappingProjectElementsRepository {
    find(id: string, filter: string): angular.IPromise<any>;
}

// ****************************************************************************
// Mapping Project Elements repository
//

m.factory('app.repositories.element.mapping-project-elements', ['Restangular', (restangular: restangular.IService) => {

    restangular.extendModel('MappingProjectElements', (model: IMappingProjectElements) => {
        return model;
    });

    // methods

    function find(id: string, filter: string) {
        return restangular.one('MappingProjectElements/' + id + '?filter=' + filter).get();
    }

    var repository: IMappingProjectElementsRepository = {
        find: find
    };

    return repository;
}]); 