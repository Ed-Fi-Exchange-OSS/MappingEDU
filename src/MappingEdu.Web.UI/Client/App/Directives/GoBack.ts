// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.go-back
//

var m = angular.module('app.directives.go-back', []);


// ****************************************************************************
// Directive ma-go-back
//

m.directive('maGoBack', ['services', (services: IServices) => ({
    restrict: 'A',
    link(scope, element) {
        var value = services.session.cloneFromSession('navigation', 'goBack');
        if (value && value.state && value.state.name) {
            element.attr('href', services.state.href(value.state.name, value.params));

            element.html(`Back to ${value.state.title || (services.state.current.name.indexOf('app.element.detail') > -1) ? 'List' : 'Previous Page'}`);
        }
    }
})]);
