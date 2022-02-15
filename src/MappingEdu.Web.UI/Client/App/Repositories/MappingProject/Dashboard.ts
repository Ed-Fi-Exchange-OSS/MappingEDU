// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.mapping-project.dashboard
//

var m = angular.module('app.repositories.mapping-project.dashboard', ['restangular']);


// ****************************************************************************
// Interface IMappingProjectDashboard 
//
 
interface IMappingProjectDashboard extends restangular.IElement {
    //TODO
}


// ****************************************************************************
// Interface IMappingProjectDashboardRepository
//

interface IMappingProjectDashboardRepository {
    get(id: string): angular.IPromise<any>;
}

// ****************************************************************************
// Repository Approve All System Item Maps
//

m.factory('app.repositories.mapping-project.dashboard', ['Restangular', (restangular: restangular.IService) => {

    restangular.extendModel('MappingProjectDashboard', (model: IMappingProjectDashboard) => {
        return model;
    });

    // methods

    function get(id: string) {
        return restangular.one('MappingProjectDashboard', id).get();
    }

    var repository: IMappingProjectDashboardRepository = {
        get: get
    };

    return repository;
}]); 