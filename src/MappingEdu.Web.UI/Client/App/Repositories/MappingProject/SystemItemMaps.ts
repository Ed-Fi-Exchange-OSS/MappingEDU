// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.mapping-project.system-item-maps
//

var m = angular.module('app.repositories.mapping-project.system-item-maps', ['restangular']);


// ****************************************************************************
// Interface SystemItemMaps 
//
 
interface IBulkActionResult extends restangular.IElement {
    MappingProjectId: string;
    CountUpdated: number;
}


// ****************************************************************************
// Interface ISystemItemMapsRepository
//

interface ISystemItemMapsRepository {
    approveReviewed(id: string): angular.IPromise<IBulkActionResult>;
    addMappings(mappingProjectId: string, model: any): angular.IPromise<IBulkActionResult>;
    updateMappings(mappingProjectId: string, model: any): angular.IPromise<IBulkActionResult>;
    getAddCount(mappingProjectId: string, model: any): angular.IPromise<IBulkActionResult>;
    getUpdateCount(mappingProjectId: string, model: any): angular.IPromise<IBulkActionResult>;
}

// ****************************************************************************
// Repository System Item Maps
//

m.factory('app.repositories.mapping-project.system-item-maps', ['Restangular', (restangular: restangular.IService) => {

    restangular.extendModel('SystemItemMaps', (model: IBulkActionResult) => {
        return model;
    });

    // methods

    function approveReviewed(id: string) {
        return restangular.one('SystemItemMaps', id).customPUT(null, 'ApproveReviewed');
    }

    function addMappings(mappingProjectId: string, data: any) {
        return restangular.one('SystemItemMaps', mappingProjectId).customPUT(data, 'Add');
    }

    function updateMappings(mappingProjectId: string, data: any) {
        return restangular.one('SystemItemMaps', mappingProjectId).customPUT(data, 'Update');
    }

    function getAddCount(mappingProjectId: string, data: any) {
        return restangular.one('SystemItemMaps', mappingProjectId).customPUT(data, 'AddCount');
    }

    function getUpdateCount(mappingProjectId: string, data: any) {
        return restangular.one('SystemItemMaps', mappingProjectId).customPUT(data, 'UpdateCount');
    }

    var repository: ISystemItemMapsRepository = {
        approveReviewed: approveReviewed,
        addMappings: addMappings,
        updateMappings: updateMappings,
        getAddCount: getAddCount,
        getUpdateCount: getUpdateCount
};

    return repository;
}]); 