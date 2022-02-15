// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appDataStandard')
    .service('dataStandardService', ['$http', function($http) {
            var resourceUrl = Application.Urls.Api.DataStandard;

            return {
                getAll: function() {
                    return $http.get(resourceUrl).then(function(response) { return response.data; });
                },
                get: function(id) {
                    return $http.get(resourceUrl + id).then(function(response) { return response.data; });
                },
                add: function(dataStandard) {
                    return $http.post(resourceUrl, dataStandard).then(function(response) { return response.data; });
                },
                update: function(id, dataStandard) {
                    return $http.put(resourceUrl + id, dataStandard).then(function(response) { return response.data; });
                },
                delete: function(id) {
                    return $http.delete(resourceUrl + id).then(function(response) { return response; });
                }
            };
        }
    ]);