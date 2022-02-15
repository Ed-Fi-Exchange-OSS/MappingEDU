// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.mapping-project.reports
//

var m = angular.module('app.repositories.mapping-project.reports', ['restangular']);


// ****************************************************************************
// Interface IMappingProjectReports
//
 
interface IMappingProjectReports extends restangular.IElement {
    //TODO
}


// ****************************************************************************
// Interface IMappingProjectReportsRepository
//

interface IMappingProjectReportsRepository {
    get(id: string): angular.IPromise<any>;
    getToken(id: string, data: any): angular.IPromise<any>;
    getTargetToken(id: string, data: any): angular.IPromise<any>;
    getCedsToken(id: string): angular.IPromise<any>;
}

// ****************************************************************************
// Repository Approve All System Item Maps
//

m.factory('app.repositories.mapping-project.reports', ['Restangular', (restangular: restangular.IService) => {

    restangular.extendModel('MappingProjectReports', (model: IMappingProjectReports) => {
        return model;
    });

    // methods

    function get(id: string) {
        return restangular.one('MappingProjectReports', id).get();
    }

    function getToken(id: string, data?) {
        return restangular.one('MappingProjectReports', id).customPOST(data, 'token');
    }

    function getTargetToken(id: string, data?) {
        return restangular.one('MappingProjectReports', id).customPOST(data, 'target-token');
    }

    function getCedsToken(id: string, data?) {
        return restangular.one('MappingProjectReports', id).customPOST(data, 'ceds-token');
    }

    var repository: IMappingProjectReportsRepository = {
        get: get,
        getToken: getToken,
        getTargetToken: getTargetToken,
        getCedsToken: getCedsToken
    };

    return repository;
}]); 