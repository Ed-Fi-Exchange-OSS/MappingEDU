// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.mapping-detail
//

var m = angular.module('app.directives.mapping-detail', []);


// ****************************************************************************
// Directive ma-mapping-detail
//

m.directive('maMappingDetail', ['$compile', 'enumerations', ($compile, enumerations: IEnumerations) => {

    // functions

    function getBusinessLogicHtml(businessLogic) {
        var html = '';
        if (businessLogic != null) {
            if (businessLogic.indexOf('[') > -1 && businessLogic.indexOf(']') > -1) {
                businessLogic = businessLogic.split('[').join('<span class="standard-b">');
                businessLogic = businessLogic.split(']').join('</span>');
            }
            html += businessLogic;
        } else {
            html += 'Not yet mapped';
        }
        return html;
    }

    function getHtml(mapping) {
        if (mapping == null) {
            return '<span>Not yet mapped</span>';
        }

        var display = _.find(enumerations.MappingMethodType, x => (x.Id === mapping.MappingMethodTypeId)).DisplayText;

        var html = '<div>';
        if (mapping.MappingMethodTypeId === 1) {
            html += getBusinessLogicHtml(mapping.BusinessLogic);
        } else {
            html += '<div>';
            html += display;
            html += '</div>';
        }

        if (html.indexOf('\r\n') > -1)
            html = html.split('\r\n').join('<br/>');
        html += '</div>';
        return html;
    }

    // directive

    return {
        restrict: 'E',
        scope: {
            mapping: '=',
            businessLogic: '='
        },
        link(scope, element) {
            var html = '';
            if (scope.businessLogic) html = getBusinessLogicHtml(scope.businessLogic);
            else html = getHtml(scope.mapping);

            var el = angular.element(html);
            var compiled = $compile(el);
            element.append(el);
            compiled(scope);
        }
    }

}]);
