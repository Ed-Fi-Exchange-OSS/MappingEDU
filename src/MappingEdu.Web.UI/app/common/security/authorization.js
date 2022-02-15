// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appCommon').factory('authorization', ['$rootScope', '$http', '$state', '$timeout', 'principal',
    function ($rootScope, $http, $state, $timeout, principal) {

        var authorization = this;

        authorization.authorize = function () {

            return principal.identity().then(function () {

                var isAuthenticated = principal.isAuthenticated();

                if ($rootScope.toState.data && $rootScope.toState.data.roles && $rootScope.toState.data.roles.length > 0 && !principal.isInAnyRole($rootScope.toState.data.roles)) {

                    if (isAuthenticated) {
                        console.log("Access Denied");
                        $timeout(function () { $state.go('home'); }, 1);
                    } else {
                        $rootScope.returnToState = $rootScope.toState;
                        $rootScope.returnToStateParams = $rootScope.toStateParams;
                        if ($rootScope.toState.name != 'forgot')
                            $timeout(function() { $state.go('login'); }, 1);
                    }
                }
            });
        }

        return authorization;
    }
]) 