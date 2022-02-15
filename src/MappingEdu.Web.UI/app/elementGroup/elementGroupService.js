// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appElementGroup').service('elementGroupService', ['$http', function($http) {
        var resourceUrl = Application.Urls.Api.Domain;

        return {
            getAll: function(parentId) {
                return $http.get(resourceUrl + parentId).then(function(response) { return response.data; });
            },
            get: function(parentId, id) {
                return $http.get(resourceUrl + parentId + '/' + id).then(function(response) { return response.data; });
            },
            add: function(elementGroup) {
                return $http.post(resourceUrl, elementGroup).then(function(response) { return response.data; });
            },
            update: function(id, elementGroup) {
                return $http.put(resourceUrl + id, elementGroup).then(function(response) { return response.data; });
            },
            delete: function(parentId, id) {
                return $http.delete(resourceUrl + parentId + '/' + id).then(function(response) { return response.data; });
            }
        }
    }
]);