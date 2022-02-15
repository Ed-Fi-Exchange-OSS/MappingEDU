// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appElementDetail')
    .service('searchService', [
        '$http', function searchService($http) {
            var resourceUrl = Application.Urls.Api.ElementSearch;

            return {
                get: function(searchText, itemTypeId, id) {
                    return $http.get(
                        resourceUrl + id + '/?itemTypeId=' + itemTypeId + '&searchText=' + searchText).then(function(response) { return response.data; });
                }
            };
        }
    ]);