// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.element.enumeration-item
//

var m = angular.module('app.repositories.element.enumeration-item', ['restangular' ]);


// ****************************************************************************
// Interface EnumerationItem
//
 
interface IEnumerationItem extends restangular.IElement {
    //TODO
}

// ****************************************************************************
// Interface IEnumerationItemRepository
//

interface IEnumerationItemRepository {
    getAll(id: string): angular.IPromise<any>;
    find(id: string, id2: string): angular.IPromise<any>;
    create(id: string, data: any): angular.IPromise<any>;
    save(id: string, id2: string, data: any): angular.IPromise<any>;
    remove(id: string, id2: string): angular.IPromise<any>;
}

// ****************************************************************************
// Enumeration Item repository
//

m.factory('app.repositories.element.enumeration-item', [ 'Restangular', (restangular: restangular.IService) => {

    restangular.extendModel('EnumerationItem', (model: IEnumerationItem) => {
            return model;
        });

        // methods

        function getAll(id: string) {
            return restangular.all('EnumerationItem/' + id).getList();
        }

        function find(id: string, id2: string) {
            return restangular.one('EnumerationItem/' + id, id2).get();
        }

        function create(id: string, data: any) {
            return restangular.one('EnumerationItem/' + id).customPOST(data);
        }

        function save(id: string, id2: string, data: any) {
            return restangular.one(`EnumerationItem/${id}`, id2).customPUT(data);
        }

        function remove(id: string, id2: string) {
            return restangular.one('EnumerationItem/' + id, id2).remove();
        }

        var repository: IEnumerationItemRepository = {
            getAll: getAll,
            find: find,
            create: create,
            save: save,
            remove: remove
        };

        return repository;
    }]); 


