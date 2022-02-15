// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.mapping=project.synonym
//

var m = angular.module('app.repositories.mapping-project.synonym', ['restangular']);


// ****************************************************************************
// Interface Mapping Project Synonym
 
interface IMappingProjectSynonym extends restangular.IElement {
    //TODO
}

// ****************************************************************************
// Interface IMappingProjectSynonymRepository
//

interface IMappingProjectSynonymRepository {
    getAll(mappingProjectId: string): angular.IPromise<Array<any>>;
    create(mappingProjectId: string, data: any): angular.IPromise<any>;
    save(mappingProjectId: string, data: any): angular.IPromise<any>;
    remove(mappingProjectId: string, entityHintId: any): angular.IPromise<any>;
}

// ****************************************************************************
// Repository Mapping Project Synonym
//

m.factory('app.repositories.mapping-project.synonym', ['Restangular', (restangular: restangular.IService) => {

    // methods

    function getAll(mappingProjectId: string) {
        return restangular.one('MappingProjectSynonym', mappingProjectId).get();
    }

    function create(mappingProjectId: string, data: any) {
        return restangular.one(`MappingProjectSynonym/${mappingProjectId}`).customPOST(data);
    }

    function save(mappingProjectId: string, data: any) {
        return restangular.one(`MappingProjectSynonym/${mappingProjectId}`, data.MappingProjectSynonymId).customPUT(data);
    }

    function remove(mappingProjectId: string, entityHintId: string) {
        return restangular.one(`MappingProjectSynonym/${mappingProjectId}`, entityHintId).remove();
    }

    var repository: IMappingProjectSynonymRepository = {
        getAll: getAll,
        create: create,
        save: save,
        remove: remove
    };

    return repository;
}]); 
