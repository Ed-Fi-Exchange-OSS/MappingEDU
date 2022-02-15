// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.repositories.auto-map
//

var m = angular.module('app.repositories.auto-map', ['restangular']);


// ****************************************************************************
// Interface IAutoMapRespository
//

interface IAutoMapRespository {
    addResults(mappingProject: any): angular.IPromise<any>;
    deltaCopy(standard: any): angular.IPromise<any>;
    mappingSuggestions(data: any, mappingProjectId: any): angular.IPromise<any>;
}

// ****************************************************************************
// Organizations repository
//

m.factory('app.repositories.auto-map', ['Restangular', (restangular: restangular.IService) => {

    // methods
    function mappingSuggestions(data: any, mappingProjectId: any) {
        return restangular.one(`AutoMapper/mapping-suggestions${(mappingProjectId) ? `?mappingProjectId=${mappingProjectId}` : ''}`).customPOST(data).then(data => {
            return data;
        });
    }

    function addResults(mappingProject: any) {
        return restangular.one(`AutoMapper/add-results/${mappingProject.MappingProjectId}`).customPOST(mappingProject).then(data => {
            return data;
        });
    }

    function deltaCopy(standard: any) {
        return restangular.one(`AutoMapper/delta-copy/${standard.DataStandardId}`).customPOST().then(data => {
            return data;
        });
    }

    var repository: IAutoMapRespository = {
        mappingSuggestions: mappingSuggestions,
        addResults: addResults,
        deltaCopy: deltaCopy
    };

    return repository;
}]); 