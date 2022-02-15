// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appCommon').factory('principal', [
    '$http', '$q', '$timeout', '_', function($http, $q, $timeout, _) {

        var principal = this;

        var _identity = undefined;
        var _authenicated = false;
        var _storageKey = 'principal.identity';
        var _storageMode = "Session";
        var _requestTokenName = 'Authorization';

        var storage = (_storageKey === "Local") ? localStorage : sessionStorage;

        principal.configure = function (config)
        {
            _requestTokenName = config.requestTokenName;
            _storageKey = config.storageKey;
            _storageMode = config.storageMode;
        }

        principal.authenticate = function(identity) {
            _identity = identity;
            _authenicated = identity != null;

            if (identity) {
                storage.setItem(_storageKey, angular.toJson(_identity));
                //$http.defaults.headers.common(_requestTokenName] = 'bearer ${identity.token}';
            } else {
                storage.removeItem(_storageKey);
                $http.defaults.headers.common[_requestTokenName] = undefined;
            }
        };

        principal.identity = function (force) {

            var deferred = $q.defer();

            if (force) _identity = undefined;

            // check and see if we have retrieved the identity data from the server. if we have, reuse it by immediately resolving
            if (angular.isDefined(_identity)) {
                deferred.resolve(_identity);
                return deferred.promise;
            }

            switch (_storageMode) {
                case "Local":
                    _identity = angular.fromJson(localStorage.getItem(_storageKey));
                    break;
                case "Session":
                    _identity = angular.fromJson(sessionStorage.getItem(_storageKey));
                    break;
            }

            return deferred.promise;
        };

        principal.isAuthenticated = function () {
            return _authenicated;
        };

        principal.isIdentityResolved = function () {
            return angular.isDefined(_identity);
        }

        principal.isInRole = function(role) {
            if (!_authenicated || !_identity.roles) return false;
            return _.indexOf(_identity.roles, role) !== -1;
        }

        principal.isInAnyRole = function(roles) {
            if (!_authenicated || !_identity.roles) return false;
            for (var i = 0; i < roles.length; i++) {
                if (principal.isInRole(roles[i])) return true;
            }

            return false;
        }

        return principal;
    }
]);