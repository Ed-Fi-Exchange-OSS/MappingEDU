// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appElementDetail').directive('maEditElementDetail', function () {
    return {
        restrict: 'E',
        templateUrl: 'app/elementDetail/ma-edit-element-detail.html',
        scope: {
            elementDetail: '=',
            editingAnElement: '=',
            addingAnElement: '=',
            save: '&',
            cancel: '&',
            errorData: '=',
            types: '=',
            enumerations: '=',
            elementForm: '=elementForm'
        }
    }
})