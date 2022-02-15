// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.container-fluid
//

var m = angular.module('app.directives.container-fluid', []);


// ****************************************************************************
// Directive ma-container-fluid
//

m.directive('maContainerFluid', ($window) => ({
    restrict: 'E',
    link() {
        angular.element(document.querySelectorAll('.container')).removeClass('container').addClass('container-fluid');
        angular.element($window).on('resize', () => {
            angular.element(document.querySelectorAll('.table')).css({width: '100%'});
        });
    }
}));

m.directive('maTableFluid', ($window) => ({
    restrict: 'E',
    link() {
        angular.element($window).on('resize', () => {
            angular.element(document.querySelectorAll('.table')).css({ width: '100%' });
        });
    }
}));

