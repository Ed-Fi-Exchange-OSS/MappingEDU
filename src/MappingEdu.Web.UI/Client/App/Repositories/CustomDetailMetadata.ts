// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.repositories.custom-detail-metadata
//

var m = angular.module('app.repositories.custom-detail-metadata', ['restangular']);


// ****************************************************************************
// Interface CustomDetailMetadata
//
 
interface ICustomDetailMetadata extends restangular.IElement {
    DisplayName: string,
    IsBoolean: boolean,
    IsCore: boolean,
    CustomDetailMetadataId: string,
    DataStandardId: string
}


// ****************************************************************************
// Interface ICustomDetailMetadataRepository
//

interface ICustomDetailMetadataRepository {
    getAllByDataStandard(dataStandardId: string): angular.IPromise<Array<ICustomDetailMetadata>>;
    create(dataStandardId: string, customDetail: any): angular.IPromise<ICustomDetailMetadata>;
    save(dataStandardId: string, customDetail: any): angular.IPromise<ICustomDetailMetadata>;
    remove(dataStandardId: string, customDetailMetadataId: string): angular.IPromise<void>;
}

// ****************************************************************************
// SystemConstants repository
//

m.factory('app.repositories.custom-detail-metadata', ['Restangular', (restangular: restangular.IService) => {

    // extend 

    restangular.extendModel('CustomDetailMetadata', (model: ICustomDetailMetadata) => {
        return model;
    });

    // methods

    function getAllByDataStandard(dataStandardId: string) {
        return restangular.one('CustomDetailMetadata', dataStandardId).get();
    }

    function create(dataStandardId: string, customDetail: any) {
        return restangular.one('CustomDetailMetadata', dataStandardId).customPOST(customDetail);
    }

    function save(dataStandardId: string, customDetail: any) {
        return restangular.one('CustomDetailMetadata', customDetail.CustomDetailMetadataId).one('DataStandard', dataStandardId).customPUT(customDetail);
    }

    function remove(dataStandardId: string, customDetailMetadataId: string) {
        return restangular.one('CustomDetailMetadata', customDetailMetadataId).one('DataStandard', dataStandardId).remove();
    }

    var repository: ICustomDetailMetadataRepository = {
        getAllByDataStandard: getAllByDataStandard,
        create: create,
        save: save, 
        remove: remove
    };

    return repository;
}]); 