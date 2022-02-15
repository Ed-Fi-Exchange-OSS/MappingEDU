// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.repositories.system-item
//

var m = angular.module('app.repositories.system-item', ['restangular']);


// ****************************************************************************
// Interface ISystemItemRepository
//

interface ISystemItemRepository {
    find(systemItemId: string): angular.IPromise<any>;
    detail(systemItemId: string, mappingProjectId?: string, mappedSystemExtensionId?: string): angular.IPromise<any>;
    create(systemItem: any): angular.IPromise<any>;
    save(systemItem: any): angular.IPromise<any>;
    remove(systemItem: any): angular.IPromise<any>;
    usage(systemItem: any): angular.IPromise<any>;
}

// ****************************************************************************
// Home repository
//

m.factory('app.repositories.system-item', ['Restangular', (restangular: restangular.IService) => {

    // methods
    function find(systemItemId: string) {
        return restangular.one('SystemItem', systemItemId).get();
    }

    function detail(systemItemId: string, mappingProjectId?: string, mappedSystemExtensionId?: string) {
        return restangular.one('SystemItem', systemItemId).customGET('detail', { mappingProjectId: mappingProjectId, mappedSystemExtensionId: mappedSystemExtensionId});
    }

    function create(systemItem: any) {
        return restangular.one('SystemItem').customPOST(systemItem);
    }

    function save(systemItem: any) {
        return restangular.one('SystemItem', systemItem.SystemItemId).customPUT(systemItem);
    }

    function remove(systemItem: any) {
        return restangular.one('SystemItem', systemItem.SystemItemId).remove();
    }

    function usage(systemItemId: string) {
        return restangular.one('SystemItem', systemItemId).customGET('usage');
    }

    var repository: ISystemItemRepository = {
        find: find,
        detail: detail,
        create: create,
        save: save,
        remove: remove,
        usage
    };

    return repository;
}]); 