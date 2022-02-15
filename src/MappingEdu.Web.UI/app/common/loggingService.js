﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appCommon').service('loggingService', loggingService);

loggingService.$inject = ['$http'];

function loggingService($http) {
    var resourceUrl = Application.Urls.Api.Logging;

    return {
        add: function (data) {
            return $http.post(resourceUrl, data).then(function (response) { return response.data; });
        }
    };
}