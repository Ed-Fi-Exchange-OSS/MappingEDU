// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.errors
//

var m = angular.module('app.modules.errors', [
    'app.modules.errors.access-denied',
    'app.modules.errors.unknown'
]);


// ****************************************************************************
// Configuration
//

m.config(['$stateProvider', 'settings', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $stateProvider
        .state('app.errors', {
            'abstract': true,
            url: '/errors',
            data: {
                roles: []
            }
        });
}]);

m.run(['$rootScope', 'services', ($rootScope, services: IServices) => {

    $rootScope.$on('$stateChangeError', (event, to, toParams, from, fromParams, error) => {
        event.preventDefault();
        $rootScope.loading = false;

        error.from = {
            state: from,
            params: fromParams
        };

        error.to = {
            state: to,
            params: toParams
        };

        var dataStandardId, mappingProjectId;
        if (to.name.indexOf('data-standard') > -1 || (to.name.indexOf('element') > -1 && to.dataStandardId) ||
            (to.name.indexOf('entity') > -1 && to.dataStandardId) || (to.name.indexOf('enumeration') > -1 && to.dataStandardId)) {
            dataStandardId = toParams.dataStandardId;
        } else if (to.name.indexOf('element') > -1 || to.name.indexOf('entity') > -1 || to.name.indexOf('enumeration') > -1) {
            mappingProjectId = toParams.mappingProjectId;
        } else if (to.name.indexOf('mapping-project') > -1) {
            mappingProjectId = toParams.id;
        }

        if (error.status === 403) {
            services.session.cloneToSession('navigation', 'error', {
                description: error.data,
                to: error.to,
                from: error.from,
                dataStandardId: dataStandardId,
                mappingProjectId: mappingProjectId
            });
            services.state.go('app.errors.access-denied', {}, { location: 'replace' });
        } else {
            services.session.cloneToSession('navigation', 'error', error);
            services.state.go('app.errors.unknown', {}, { location: 'replace' });
        }

    });

}]);
