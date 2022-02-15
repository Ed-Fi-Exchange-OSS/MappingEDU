// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appElementDetail').service('noteService', ['$http', function($http) {

        var resourceUrl = Application.Urls.Api.Note;

        return {
            get: function(id) {
                return $http.get(resourceUrl + id + '/').then(function(response) { return response.data; });
            },
            add: function(id, data) {
                return $http.post(resourceUrl + id + '/', data).then(function(response) { return response.data; });
            },
            update: function(id, id2, data) {
                return $http.put(resourceUrl + id + '/' + id2 + '/', data).then(function(response) { return response.data; });
            },
            delete: function(elementId, id) {
                return $http.delete(resourceUrl + elementId + '/' + id + '/').then(function(response) { return response.data; });
            }
        }
    }
]);
