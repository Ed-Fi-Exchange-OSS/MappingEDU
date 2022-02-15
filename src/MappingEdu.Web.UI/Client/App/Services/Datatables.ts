// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.services.breadcrumbs
//

var m = angular.module('app.services.datatables', []);

// ****************************************************************************
// Interface IDatatablesService
//

interface IDatatablesService {
    defaultOptions: any;
    optionsBuilder: any;
    columnBuilder: any;
    columnDefBuilder: any;
}

// ****************************************************************************
// Service breadcrumbService
//

m.factory('app.services.datatables', ['DTDefaultOptions', 'DTOptionsBuilder', 'DTColumnBuilder', 'DTColumnDefBuilder',
    (DTDefaultOptions, DTOptionsBuilder, DTColumnBuilder, DTColumnDefBuilder) => {

        var service: IDatatablesService = {
            defaultOptions: DTDefaultOptions,
            optionsBuilder: DTOptionsBuilder,
            columnBuilder: DTColumnBuilder,
            columnDefBuilder: DTColumnDefBuilder
        }

        return service;

}]);