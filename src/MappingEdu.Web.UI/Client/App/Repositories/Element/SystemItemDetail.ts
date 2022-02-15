// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.element.system-item-detail
//

var m = angular.module('app.repositories.element.system-item-detail', ['restangular']);


// ****************************************************************************
// Interface SystemItemDetail
//
 
interface ISystemItemDetail extends restangular.IElement {
    //TODO
}

// ****************************************************************************
// Interface ISystemItemDetailRepository
//

interface ISystemItemDetailRepository {
    getCustomDetails(systemItemId: any): angular.IPromise<Array<any>>;
    save(data: any): angular.IPromise<any>;
}

// ****************************************************************************
// System Item Detail repository
//

m.factory('app.repositories.element.system-item-detail', ['Restangular', (restangular: restangular.IService) => {

    restangular.extendModel('SystemItemDetail', (model: ISystemItemDetail) => {
        return model;
    });

    // methods

    function getCustomDetails(systemItemId: any) {
        return restangular.one('SystemItemDetail', systemItemId).get();
    }

    function save(data: any) {
        return restangular.one('SystemItemDetail').customPUT(data);
    }

    var repository: ISystemItemDetailRepository = {
        getCustomDetails: getCustomDetails,
        save: save
    };

    return repository;
}]); 