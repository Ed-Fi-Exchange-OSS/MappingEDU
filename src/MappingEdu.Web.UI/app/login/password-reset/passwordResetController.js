// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module("appLogin").controller('passwordResetController', ['$scope', '$timeout', function ($scope, $timeout) {
    
    var passwordViewModel = this;

    $scope.resetPassword = function () {

        $scope.success = true;
        $timeout(function () { $scope.success = false; }, 5000);

        $scope.error = true;
        $timeout(function () { $scope.error = false; }, 5000);

    }

}]);
