// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module("appLogin").controller('loginController', ['$scope', '$timeout', '$state', 'authentication', 'security', function ($scope, $timeout, $state, authentication, security) {
    
    var loginViewModel = this;

    $scope.auth = {};

    $scope.error = false;

    $scope.authenticate = function(form) {
        authentication.authenticate($scope.auth.username, $scope.auth.password).then(function (data) {

            security.principal.authenticate({
                name: $scope.auth.username,
                token: data.access_token,
                roles: ($scope.auth.username == "admin") ? ['user', 'admin'] : ['user']
        });

            $state.go('home');

        }, function (error) {
            $scope.error = true;
            $timeout(function () { $scope.error = false }, 5000);
            $scope.auth.password = "";
        });
    };
}]);
