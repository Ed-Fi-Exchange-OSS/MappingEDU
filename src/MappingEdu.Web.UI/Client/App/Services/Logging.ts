// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.services.logging
//

var m = angular.module('app.services.logging', []);


// ****************************************************************************
// Interfaces ILoggingService
//

interface ILoggingService {
    add(data: any);
}


// ****************************************************************************
// Service loggingService 
//

m.factory('loggingService', ['$http', 'principal',  ($http: ng.IHttpService, principal: IPrincipal) => {

    function add(data: any) {
        if (!data.Level) data.Level = 'DEBUG';

        // Force retrieval from session
        return principal.identity(true).then(() => {
            return $http.post('api/logging', data);
        });
    }

    var service: ILoggingService = {
        add: add
    };

    return service;
}]);
