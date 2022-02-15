// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.mapping-project.summary
//

var m = angular.module('app.repositories.mapping-project.summary', ['restangular']);


// ****************************************************************************
// Interface IMappingProjectSummary 
//
 
interface IMappingProjectSummary extends restangular.IElement {
    //TODO
}


// ****************************************************************************
// Interface IMappingProjectSummaryRepository
//

interface IMappingProjectSummaryRepository {
    get(id: string, itemTypeId?: number, parentSystemItemId?: string): angular.IPromise<Array<any>>;
    getDetail(id: string, systemItemId: string, itemTypeId?: number): angular.IPromise<Array<any>>;
}

// ****************************************************************************
// Repository Approve All System Item Maps
//

m.factory('app.repositories.mapping-project.summary', ['Restangular', (restangular: restangular.IService) => {

    restangular.extendModel('MappingProjectSummary', (model: IMappingProjectSummary) => {
        return model;
    });

    // methods

    function get(id: string, itemTypeId?: number, parentSystemItemId?: string) {
        return restangular.one('MappingProjectSummary', id).get({ ItemTypeId: itemTypeId, ParentSystemItemId: parentSystemItemId });
    }

    function getDetail(id: string, systemItemId: string, itemTypeId?: number) {
        return restangular.one('MappingProjectSummary', id).customGET('detail', { ItemTypeId: itemTypeId, SystemItemId: systemItemId });
    }

    var repository: IMappingProjectSummaryRepository = {
        get: get,
        getDetail: getDetail
    };

    return repository;
}]); 