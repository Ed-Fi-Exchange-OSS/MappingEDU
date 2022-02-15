// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Interface IAuthenticationToken
//

interface IAuthenticationToken {
    access_token: string;
    token_type: string;
    expires_in: number;
    roles: string[];
    name: string;
}


// ****************************************************************************
// Interface IAuthenticationRepository
//

interface IAuthenticationRepository {
    authenticate(authId: string, password: string, namespace?: string): angular.IPromise<IAuthenticationToken>
}


// ****************************************************************************
// Module app.repositories.authentication
//

var m = angular.module('app.repositories.authentication', ['restangular']);


// ****************************************************************************
// Authentication service
//

m.factory('authentication', ['Restangular', (restangular: restangular.IService) => {

    function authenticate(username: string, password: string) {
        var data = { grant_type: 'password', username: username, password: password };

        return <angular.IPromise<IAuthenticationToken>>restangular.one('accesstoken').customPOST(
            $.param(data), '', {}, { 'Content-Type': 'application/x-www-form-urlencoded' });
    }

    var repository: IAuthenticationRepository = {
        authenticate: authenticate
    };
    return repository;
}]); 