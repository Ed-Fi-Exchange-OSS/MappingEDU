// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

(function() {
    angular.module('appCommon').
        directive('maMappingDetail', ['$compile', function($compile) {
                return {
                    restrict: 'E',
                    scope: {
                        mapping: '='
                    },
                    link: function(scope, element) {
                        var el = angular.element(getHtml(scope.mapping));
                        var compiled = $compile(el);
                        element.append(el);
                        compiled(scope);
                    }
                }

                function getHtml(mapping) {

                    if (mapping == null) {
                        return '<span>Not yet mapped</span>';
                    }
                    var display = _.find(Application.Enumerations.MappingMethodType, function(x) {
                        return x.Id == mapping.MappingMethodTypeId;
                    }).DisplayText;
                    var html = '<div>';
                    if (mapping.MappingMethodTypeId == 1) {
                        var businessLogic = mapping.BusinessLogic;
                        if (businessLogic != null) {
                            if (businessLogic.indexOf('[') > -1 && businessLogic.indexOf(']') > -1) {
                                businessLogic = businessLogic.split("[").join("<span class=\"standard-b\">");
                                businessLogic = businessLogic.split("]").join("</span>");
                            }
                            html += businessLogic;
                        } else {
                            html += 'Not yet mapped';
                        }
                    } else {
                        html += '<div>';
                        html += display;
                        html += '</div>';
                    }

                    if (html.indexOf('\r\n') > -1)
                        html = html.split("\r\n").join("<br/>");
                    html += '</div>';
                    return html;
                }
            }
        ]);
}());