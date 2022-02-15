// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.element.mapping
//

var m = angular.module('app.repositories.element.mapping', ['restangular']);


// ****************************************************************************
// Interface Element Mapping
//
 
interface IElementMapping extends restangular.IElement {
    //TODO
}

// ****************************************************************************
// Interface IElementMappingRepository
//

interface IElementMappingRepository {
    find(id: string): angular.IPromise<any>;
    findByProject(systemItemId: string, mappingProjectId: string): angular.IPromise<any>;
    uniqueCreateBy(id: string): angular.IPromise<any>;
    uniqueUpdateBy(id: string): angular.IPromise<any>;
    create(id: string, data: any): angular.IPromise<any>;
    save(id: string, id2: string, data: any): angular.IPromise<any>;
    remove(elementId: string, id: string): angular.IPromise<any>;
}

// ****************************************************************************
// Repository Elements
//

m.factory('app.repositories.element.mapping', ['Restangular', (restangular: restangular.IService) => {

    // methods

    function find(id: string) {
        return restangular.one('SystemItemMapping', id).get();
    }

    function findByProject(systemItemId: string, mappingProjectId: string) {
        return restangular.one('SystemItemMapping', systemItemId).one('Project', mappingProjectId).get();
    }

    function uniqueCreateBy(id: string) {
        return restangular.one('SystemItemMapping', id).customGET('UniqueCreateBy');
    }

    function uniqueUpdateBy(id: string) {
        return restangular.one('SystemItemMapping', id).customGET('UniqueUpdateBy');
    }

    function create(id: string, data: any) {
        return restangular.one('SystemItemMapping/' + id).customPOST(data);
    }

    function save(id: string, id2: string, data: any) {
        return restangular.one('SystemItemMapping/' + id, id2).customPUT(data);
    }

    function remove(elementId: string, id: string) {
        return restangular.one('SystemItemMapping/' + elementId, id).remove();
    }

    var repository: IElementMappingRepository = {
        find: find,
        findByProject: findByProject,
        uniqueCreateBy: uniqueCreateBy,
        uniqueUpdateBy: uniqueUpdateBy,
        create: create,
        save: save,
        remove: remove
    };

    return repository;
}]); 
