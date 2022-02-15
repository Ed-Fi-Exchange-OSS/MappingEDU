// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Interfaces common.security
//

interface ISecurity {
    authorization: IAuthorization;
    principal: IPrincipal;
}


// ****************************************************************************
// Module common.security
//

var m = angular.module('app.security', [
    'app.security.authorization',
    'app.security.principal'
]);


// ****************************************************************************
// Service 'security'
//

m.factory('security', ['authorization', 'principal', (authorization: IAuthorization, principal: IPrincipal) => {

        var security: ISecurity = {
            authorization: authorization,
            principal: principal
        };
        return security;
    }
]); 