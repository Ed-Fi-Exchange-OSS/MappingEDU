// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.repositories.element-group
//

var m = angular.module('app.repositories.element-group',
    ['restangular',
     'app.repositories.element-group.new-system-item']);


// ****************************************************************************
// Interface ElementGroup
//
 
interface IElementGroup extends restangular.IElement {
    //TODO
}

// ****************************************************************************
// Interface IElementGroupRepository
//

interface IElementGroupRepository {
    newSystemItem: INewSystemItemRepository,
    getAll(parentId: string): angular.IPromise<any>;
    find(parentId: string, id: string): angular.IPromise<any>;
    create(elementGroup: any): angular.IPromise<any>;
    save(id: string, elementGroup: any): angular.IPromise<any>;
    remove(parentId: string, id: string): angular.IPromise<any>;
}

// ****************************************************************************
// Element Group repository
//

m.factory('app.repositories.element-group', [
    'Restangular',
    'app.repositories.element-group.new-system-item',
    (restangular: restangular.IService,
     newSystemItem: INewSystemItemRepository) => {

        restangular.extendModel('Domain', (model: IElementGroup) => {
            return model;
        });

        // methods

        function getAll(parentId: string) {
            return restangular.all('Domain/' + parentId).getList();
        }

        function find(parentId: string, id: string) {
            return restangular.one('Domain/' + parentId, id).get();
        }
        
        function create(elementGroup: any) {
            return restangular.one('Domain').customPOST(elementGroup);
        }

        function save(id: string, elementGroup: any) {
            return restangular.one('Domain', id).customPUT(elementGroup);
        }

        function remove(parentId: string, id: string) {
            return restangular.one('Domain/' + parentId, id).remove();
        }

        var repository: IElementGroupRepository = {
            newSystemItem: newSystemItem,
            getAll: getAll,
            find: find,
            create: create,
            save: save,
            remove: remove
        };

        return repository;
    }]); 