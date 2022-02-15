// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appMappingProject').service('mappingProjectService', ['$http', function($http) {
        var resourceUrl = Application.Urls.Api.MappingProject;
        var cloneUrl = Application.Urls.Api.CloneMappingProject;
        return {
            getAll: function() {
                return $http.get(resourceUrl).then(function(response) { return response.data; });
            },
            get: function(id) {
                return $http.get(resourceUrl + id + '/').then(function(response) { return response.data; });
            },
            add: function(data) {
                return $http.post(resourceUrl, data).then(function(response) { return response.data; });
            },
            update: function(id, data) {
                return $http.put(resourceUrl + id + '/', data).then(function(response) { return response.data; });
            },
            delete: function(id) {
                return $http.delete(resourceUrl + id + '/').then(function(response) { return response.data; });
            },
            cloneMappingProject: function(data) {
                //TODO: add route
                return $http.post(cloneUrl, data).then(function(response) { return response; });
            }
        }
    }
]);
