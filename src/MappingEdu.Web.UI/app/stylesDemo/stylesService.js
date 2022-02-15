// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appStylesDemo').service('stylesService', ['$http', function($http) {
        // Services should return the promise rather than the data. This is the asynchronous way.
        return {
            greeting: function() {
                return "Hello Angular!";
            },
        }
    }
]);