// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appMappingEdu', [
    'ui.router',
    'datatables',
    'appCommon',
    'appHome',
    'appLogin',
    'appDataStandard',
    'appStylesDemo',
    'appMappingProject',
    'appElementDetail',
    'appElementGroup',
    'appEntityDetail',
    'appSystemItemMap',
    'ngMessages',
    'ngFileUpload',
    'appAnalytics']);

/* Common */
angular.module('appCommon', []);

/* Home */
angular.module('appHome', []);

/* Login */
angular.module('appLogin', []);

/* Mapped Systems */
angular.module('appDataStandard', []);

/* Mapping Project */
angular.module('appMappingProject', []);

/* Element */
angular.module('appElementDetail', []);

/* Element Groups */
angular.module('appElementGroup', []);

/* Entity */
angular.module('appEntityDetail', []);

/* System Items */
angular.module('appSystemItemMap', []);

/* Analytics (Logging) */
angular.module('appAnalytics', ['angulartics']);

/* Styles */
angular.module('appStylesDemo', []);

angular.module('appMappingEdu').value('$', $);
angular.module('appMappingEdu').value('_', _);


angular.module('appMappingEdu').run(['_', '$rootScope', '$state', '$timeout', 'sessionService', 'security', function(_, $rootScope, $state, $timeout, sessionService, security) {

        security.principal.configure({
            requestTokenName: 'Authorization',
            storageKey: 'skeleton.principal.identity',
            storageMode: "Session"
        });
      
        $rootScope.$on('$stateChangeStart', function (event, toState, toStateParams) {
            $rootScope.toState = toState;
            $rootScope.toStateParams = toStateParams;
            angular.element(document.querySelectorAll('.container-fluid')).removeClass('container-fluid').addClass('container');
            if (security.principal.isIdentityResolved()) {
                security.authorization.authorize();
            }
        });

        $rootScope.$on('$stateChangeSuccess', function(event, to, toParams, from, fromParams) {
            if (!_.isUndefined($state.current.title))
            // Title was getting changed before the browser could add the previous one to the history, 
            //  so had to add a timeout here
                $timeout(function() {
                    document.title = $state.current.title + ($state.current.title != 'MappingEDU' ? ' - MappingEDU' : '');
                }, 100);

            var goBackToStates = ['entityDetail.info', 'dataStandard.elementList', 'mappingProject.reviewQueue', 'mappingProject.dashboard'];

            if (_.contains(goBackToStates, from.name))
                sessionService.cloneToSession("navigation", "goBack", { state: from, params: fromParams });
        });
    }
]);
