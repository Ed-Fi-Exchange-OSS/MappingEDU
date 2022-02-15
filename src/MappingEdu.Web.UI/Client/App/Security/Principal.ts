// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Interfaces app.security.principal
//

interface IIdentity {
    name: string;
    token: string;
    roles: string[];
}

interface IPrincipal {
    authenticate(identity: IIdentity): void;
    configure(config: IPrincipalConfiguration): void;
    isAuthenticated(): boolean;
    identity(force?: boolean): angular.IPromise<IIdentity>;
    isIdentityResolved(): boolean;
    isInAnyRole(roles: string[]): boolean;
    isInRole(role: string): boolean;
    logout(): void;
}

enum PrincipalStorageMode { Local, Session }

interface IPrincipalConfiguration {
    requestTokenName: string;
    storageKey: string;
    storageMode: PrincipalStorageMode;
}


// ****************************************************************************
// Module app.security.principal
//

var m = angular.module('app.security.principal', ['app.services.underscore']);


// ****************************************************************************
// Module Constants
//

m.value('$', $);


// ****************************************************************************
// Factory app.security.principal 
//

m.factory('principal', ['$', '$http', '$q', '$timeout', '_', ($, $http: ng.IHttpService, $q, $timeout, underscore: UnderscoreStatic) => {

    var _identity: IIdentity = undefined;
    var _authenticated = false;
    var _loginState = '';
    var _storageKey = 'principal.identity';
    var _storageMode: any = PrincipalStorageMode.Session;
    var _requestTokenName = 'Authorization';
    var storage = (_storageMode === PrincipalStorageMode.Local) ? localStorage : sessionStorage;

    function configure(config: IPrincipalConfiguration) {
        _requestTokenName = config.requestTokenName;
        _storageKey = config.storageKey;
        _storageMode = config.storageMode;
    }

    function authenticate(identity: IIdentity) {

        if (_identity) this.logout();

        _identity = identity;
        _authenticated = identity != null;
        if (identity) {
            storage.setItem(_storageKey, angular.toJson(_identity));
            $http.defaults.headers.common[_requestTokenName] = `bearer ${identity.token}`;
            var ajaxHeaders = {};
            ajaxHeaders[_requestTokenName] = `bearer ${identity.token}`;
            $.ajaxSetup({ headers: ajaxHeaders});
        } else {
            //storage.removeItem(_storageKey);
            $http.defaults.headers.common[_requestTokenName] = undefined;
        }
    }

    function identity(force?: boolean) {
        var deferred = $q.defer();

        if (force) _identity = undefined;

        // check and see if we have retrieved the identity data from the server. if we have, reuse it by immediately resolving
        if (_identity) {
            deferred.resolve(_identity);
            return deferred.promise;
        }

        // retrieve identity from storage 
        
        switch (_storageMode) {
            case PrincipalStorageMode.Local:
                _identity = angular.fromJson(localStorage.getItem(_storageKey));
                break;
            case PrincipalStorageMode.Session:
                _identity = angular.fromJson(sessionStorage.getItem(_storageKey));
                break;
        }

        if (!_identity) {
            var request = { grant_type: 'password', username: 'guest@example.com', password: 'guest9999' };
            $http({
                method: 'POST',
                url: 'api/accesstoken',
                data: $.param(request),
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then((response) => {
                return response.data;
            }).then((data: IAuthenticationToken) => {
                this.authenticate({ name: data.name, token: data.access_token, roles: data.roles }); // base role
                deferred.resolve({ name: data.name, token: data.access_token, roles: data.roles });
            });
        } else {
            this.authenticate(_identity);
            deferred.resolve(_identity);
        }

        return deferred.promise;
    }

    function isAuthenticated() {
        return _authenticated;
    }

    function isIdentityResolved() {
        if (_identity) return true;
        else return false;
    }

    function isInRole(role: string) {
        if (!_authenticated || !_identity.roles) return false;

        return _identity.roles.indexOf(role) !== -1;
    }

    function isInAnyRole(roles: string[]) {
        if (!_authenticated || !_identity.roles) return false;
        for (var i = 0; i < roles.length; i++) {
            if (this.isInRole(roles[i])) return true;
        }
        return false;
    }

    function logout() {

        _identity = undefined;
        _authenticated = false;
        storage.removeItem(_storageKey);
        $http.defaults.headers.common[_requestTokenName] = undefined;
    }

    var principal: IPrincipal = {
        authenticate: authenticate,
        configure: configure,
        identity: identity,
        isAuthenticated: isAuthenticated,
        isIdentityResolved: isIdentityResolved,
        isInAnyRole: isInAnyRole,
        isInRole: isInRole,
        logout: logout
    };
    return principal;
}]);
