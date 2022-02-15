// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.mapping=project.template
//

var m = angular.module('app.repositories.mapping-project.template', ['restangular']);


// ****************************************************************************
// Interface Mapping Project Template
 
interface IMappingProjectTemplate extends restangular.IElement {
    //TODO
}

// ****************************************************************************
// Interface IMappingProjectTemplateRepository
//

interface IMappingProjectTemplateRepository {
    getAll(mappingProjectId: string): angular.IPromise<Array<any>>;
    create(mappingProjectId: string, data: any): angular.IPromise<any>;
    save(mappingProjectId: string, data: any): angular.IPromise<any>;
    remove(mappingProjectId: string, templateId: any): angular.IPromise<any>;
}

// ****************************************************************************
// Repository Mapping Project Template
//

m.factory('app.repositories.mapping-project.template', ['Restangular', (restangular: restangular.IService) => {

    // methods

    function getAll(mappingProjectId: string) {
        return restangular.one('MappingProjectTemplate', mappingProjectId).get();
    }

    function create(mappingProjectId: string, data: any) {
        return restangular.one(`MappingProjectTemplate/${mappingProjectId}`).customPOST(data);
    }

    function save(mappingProjectId: string, data: any) {
        return restangular.one(`MappingProjectTemplate/${mappingProjectId}`, data.MappingProjectTemplateId).customPUT(data);
    }

    function remove(mappingProjectId: string, templateId: string) {
        return restangular.one(`MappingProjectTemplate/${mappingProjectId}`, templateId).remove();
    }

    var repository: IMappingProjectTemplateRepository = {
        getAll: getAll,
        create: create,
        save: save,
        remove: remove
    };

    return repository;
}]); 
