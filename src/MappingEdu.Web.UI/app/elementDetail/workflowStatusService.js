// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appElementDetail').service('workflowStatusService', ['$http', function($http) {
        var resourceUrl = Application.Urls.Api.WorkflowStatus;
        return {
            update: function(id, id2, data) {
                return $http.put(resourceUrl + id + '/' + id2 + '/', data).then(function(response) { return response.data; });
            }
        }
    }
]);
