// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appElementDetail').controller('browseController', [
    '_', '$timeout', '$scope', '$state', '$stateParams', 'handleErrorService', 'sessionService',
    function(_, $timeout, $scope, $state, $stateParams, handleErrorService, sessionService) {
        var browseViewModel = this;
        browseViewModel.title = 'Browse Title';
    }
]);
