// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.element.map-note
//

var m = angular.module('app.repositories.element.map-note', ['restangular']);


// ****************************************************************************
// Interface Map Note
//
 
interface IMapNote extends restangular.IElement {
    //TODO
}

// ****************************************************************************
// Interface IMapNoteRepository
//

interface IMapNoteRepository {
    find(id: string): angular.IPromise<any>;
    create(id: string, data: any): angular.IPromise<any>;
    save(id: string, id2: string, data: any): angular.IPromise<any>;
    remove(elementId: string, id: string): angular.IPromise<any>;
}

// ****************************************************************************
// Repository MapNote
//

m.factory('app.repositories.element.map-note', ['Restangular', (restangular: restangular.IService) => {

    // methods

    function find(id: string) {
        return restangular.one('MapNotes', id).get();
    }

    function create(id: string, data: any) {
        return restangular.one('MapNotes/' + id).customPOST(data);
    }

    function save(id: string, id2: string, data: any) {
        return restangular.one('MapNotes/' + id, id2).customPUT(data);
    }

    function remove(elementId: string, id: string) {
        return restangular.one('MapNotes/' + elementId, id).remove();
    }

    var repository: IMapNoteRepository = {
        find: find,
        create: create,
        save: save,
        remove: remove
    };

    return repository;
}]); 
