// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.manage.system-constants.constant-display
//

var m = angular.module('app.modules.manage.system-constants.constant-display', [
    'app.modules.manage.system-constants.constant-text',
    'app.modules.manage.system-constants.constant-textarea',
    'app.modules.manage.system-constants.constant-complex',
    'app.modules.manage.system-constants.constant-boolean'

]);


// ****************************************************************************
// Directive app.modules.manage.system-constants
//

m.directive('constantDisplay', ['$compile', ($compile) => {
    return {
        restrict: 'A',
        scope: {
            constant: '=',
            index: '='
        },
        link: (scope, element, attrs) => {
            console.log(scope.index);
            var template = '<tr constant-'+ scope.constant.TypeName + ' constant="constant" index="index"></tr>';
            var templateElement = angular.element(template);
            element.replaceWith(templateElement);
            $compile(templateElement)(scope);
        }
    }
}])