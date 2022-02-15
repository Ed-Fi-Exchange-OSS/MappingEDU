// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.styles-demo.service
//

var m = angular.module('app.modules.styles-demo.service', []);


// ****************************************************************************
// Service stylesService
//

m.service('stylesService', ['$http', $http => ({
        greeting() {
            return 'Hello Angular!';
        }
    })
]);
