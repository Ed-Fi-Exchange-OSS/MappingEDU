// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.repositories.system-constant
//

var m = angular.module('app.repositories.system-constant', ['restangular']);


// ****************************************************************************
// Interface SystemConstant
//
 
interface ISystemConstant extends restangular.IElement {
    Id: string,
    Name: string,
    Value: string
}


// ****************************************************************************
// Interface ISystemConstantRepository
//

interface ISystemConstantRepository {
    find(name: string): angular.IPromise<ISystemConstant>;
    getAll(): angular.IPromise<Array<ISystemConstant>>;
    save(constant: ISystemConstant): angular.IPromise<ISystemConstant>;
}

// ****************************************************************************
// SystemConstants repository
//

m.factory('app.repositories.system-constant', ['Restangular', (restangular: restangular.IService) => {

    // extend 

    restangular.extendModel('SystemConstant', (model: ISystemConstant) => {
        return model;
    });

    // methods

    function find(name: string) {
        return restangular.one('SystemConstant', name).get();
    }

    function getAll() {
        return restangular.all('SystemConstant').getList();
    }

    function save(constant: ISystemConstant) {
        return restangular.copy(constant).put();
    }

    var repository: ISystemConstantRepository = {
        find: find,
        getAll: getAll,
        save: save
    };

    return repository;
}]); 