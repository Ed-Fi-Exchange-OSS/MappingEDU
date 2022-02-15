// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.repositories.home
//

var m = angular.module('app.repositories.home', ['restangular']);


// ****************************************************************************
// Interface IHomeRepository
//

interface IHomeRepository {
    getHomePage(): angular.IPromise<any>;
}

// ****************************************************************************
// Home repository
//

m.factory('app.repositories.home', ['Restangular', (restangular: restangular.IService) => {

    // methods
    function getHomePage() {
        return restangular.one('Home').get().then(data => {
            return data;
        });
    }

    var repository: IHomeRepository = {
        getHomePage: getHomePage
    };

    return repository;
}]); 