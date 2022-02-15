// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.mapping-projects-elements
//

var m = angular.module('app.repositories.element.detail', ['restangular']);


// ****************************************************************************
// Interface IDataStandardElementsRepository
//

interface IElementDetailRepository {
    create(detail: any): angular.IPromise<any>;
    save(detail: any): angular.IPromise<any>;
}

// ****************************************************************************
// Repository Elements
//

m.factory('app.repositories.element.detail', ['Restangular', (restangular: restangular.IService) => {

    // methods

    function create(detail) {
        return restangular.one('ElementDetails').customPOST(detail);
    }

    function save(detail) {
        return restangular.one('ElementDetails', detail.SystemItemId).customPUT(detail);
    }

    var repository: IElementDetailRepository = {
        create: create,
        save: save
    };

    return repository;
}]); 