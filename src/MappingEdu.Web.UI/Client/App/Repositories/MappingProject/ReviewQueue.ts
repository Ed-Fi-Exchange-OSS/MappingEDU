// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.mapping-project.review-queue
//

var m = angular.module('app.repositories.mapping-project.review-queue', ['restangular']);


// ****************************************************************************
// Interface IMappingProjectReviewQueue 
//
 
interface IMappingProjectReviewQueue extends restangular.IElement {
    //TODO
}


// ****************************************************************************
// Interface IMappingProjectReviewQueueRepository
//

interface IMappingProjectReviewQueueRepository {
    get(id: string): angular.IPromise<any>;
    getElements(id: string, data: any): angular.IPromise<any>;
    filter: IMappingProjectReviewQueueFilterRepository;
}

// ****************************************************************************
// Interface IMappingProjectReviewQueueFilterRepository
//


interface IMappingProjectReviewQueueFilterRepository {
    getAll(mappingProjectId?: string): angular.IPromise<any>;
    create(mappingProjectId: string, filter: any): angular.IPromise<any>;
    dashboard(mappingProjectId: string): angular.IPromise<any>;
    save(mappingProjectQueueFilterId: string, mappingProjectId: string, filter: any): angular.IPromise<any>;
    remove(mappingProjectQueueFilterId: string): angular.IPromise<any>;
}

// ****************************************************************************
// Repository Approve All System Item Maps
//

m.factory('app.repositories.mapping-project.review-queue', ['Restangular', '$q', (restangular: restangular.IService, $q: ng.IQService) => {

    restangular.extendModel('MappingProjectReviewQueue', (model: IMappingProjectReviewQueue) => {
        return model;
    });

    // methods

    function getAllFilter(mappingProjectId?: string) {
        return restangular.one('MappingProjectQueueFilter').get({mappingProjectId: mappingProjectId});
    }

    function dashboardFilter(mappingProjectId: string) {
        return restangular.one('MappingProjectQueueFilter', mappingProjectId).customGET('dashboard');
    }


    function createFilter(mappingProjectId: string, model: any) {
        return restangular.one('MappingProjectQueueFilter', mappingProjectId).customPOST(model);
    }

    function saveFilter(mappingProjectQueueFilterId: string, mappingProjectId: string, model: any) {
        return restangular.one('MappingProjectQueueFilter', mappingProjectQueueFilterId).one('MappingProject', mappingProjectId).customPUT(model);
    }

    function removeFilter(mappingProjectQueueFilterId: string) {
        return restangular.one('MappingProjectQueueFilter', mappingProjectQueueFilterId).remove();
    }

    var filterRepository: IMappingProjectReviewQueueFilterRepository = {
        getAll: getAllFilter,
        create: createFilter,
        dashboard: dashboardFilter,
        save: saveFilter,
        remove: removeFilter
    };

    function get(id: string) {
        return restangular.one('MappingProjectReviewQueue', id).get();
    }

    function getElements(id: string, data: any) {
        return restangular.all(`MappingProjectReviewQueue/${id}/elements`).post(data, null, {
            "Content-Type": 'application/x-www-form-urlencoded; charset=UTF-8'
        });
    }

    var repository: IMappingProjectReviewQueueRepository = {
        get: get,
        getElements: getElements,
        filter: filterRepository
    };

    return repository;
}]); 