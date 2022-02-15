﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appEntityDetail').service('entityService', ['$http', function($http) {
        var resourceUrl = Application.Urls.Api.Entity;

        return {
            get: function(mappingProjectId, id) {
                return $http.get(
                    resourceUrl + mappingProjectId + '/' + id).then(function(response) { return response.data; });
            },
            delete: function(id) {
                return $http.delete(resourceUrl + id + '/').then(function(response) { return response.data; });
            },
        }
    }
]);