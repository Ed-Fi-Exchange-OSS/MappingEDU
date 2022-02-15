// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.preview-business-logic
//

var m = angular.module('app.directives.preview-business-logic', []);


// ****************************************************************************
// Directive ma-preview-business-logic
//

((() => {
        m.directive('maPreviewBusinessLogic', ['_', '$', (_, $) => {
                return {
                    restrict: 'A',
                    scope: {
                        businessLogic: '=',
                        readOnly: '=?',
                        fullHeight: '=?'
                    },
                    link(scope, element, attrs) {
                        scope.$watch('businessLogic', () => {
                            var newValue;
                            if ((scope.businessLogic == undefined ||
                                scope.businessLogic == null)) {
                                newValue = '';
                            } else {
                                newValue =
                                    scope.businessLogic
                                    .replace(/\r\n/gm, '<br>')
                                    .replace(/\n/gm, '<br>')
                                    .replace(/\[/gm, '<span class="standard-b">[')
                                    .replace(/\]/gm, ']</span>')
                                    .replace(/\$\{/gm, '<span class="business-logic-replacement">${')
                                    .replace(/\}/gm, '}</span>');

                                newValue = styleStringLiterals(_, scope.businessLogic, newValue);
                                //newValue = styleBusinessLogicDestinationInfo(_, scope.businessLogic, newValue);
                                newValue = styleKeywords(_, scope.businessLogic, newValue);
                                newValue = styleNumbers(_, scope.businessLogic, newValue);
                            }
                        
                            element.html(newValue);

                            scope.readOnly = scope.readOnly || false;
                            if (!scope.readOnly && !scope.fullHeight) {
                                element.height(0);
                                var height = element[0].scrollHeight;
                                element.height(height);
                            }
                        });
                    }
                }
            }
        ]);

    function styleStringLiterals(_, businessLogic, newValue) {
        // Matches string literals in double quotes
        var matches = businessLogic.match(/"(.*?)"/gm);
        _.each(_.uniq(matches), function (item) {
            newValue = newValue.replace(
                new RegExp(item, 'gm'),
                '<span class="business-logic-literal">' + item + '</span>');
        });

        return newValue;
    }

    function styleNumbers(_, businessLogic, newValue) {
        // Matches numbers
        var matches = businessLogic.match(/\d+/gm);
        _.each(_.uniq(matches), function (item) {
            newValue = newValue.replace(
                new RegExp('\\b' + item + '\\b', 'gm'),
                '<span class="business-logic-literal">' + item + '</span>');
        });

        return newValue;
    }

    function styleKeywords(_, businessLogic, newValue) {
        // Matches keyword true
        var matches = businessLogic.match(/(\btrue\b)|(\bfalse\b)|(\byes\b)|(\bno\b)|(\bexists\b)/gim);
        _.each(_.uniq(matches), function (item) {
            newValue = newValue.replace(
                new RegExp('\\b' + item + '\\b', 'gm'),
                '<span class="business-logic-keyword">' + item + '</span>');
        });

        return newValue;
    }

    function styleBusinessLogicDestinationInfo(_, businessLogic, newValue) {
        // Matches items.with.periods
        var matches = businessLogic.match(/[A-Za-z0-9_-]+(\.[A-Za-z0-9_-]+)+/gm);
        _.each(_.uniq(matches), function (item) {
            var index = businessLogic.indexOf(item);
            if (businessLogic.charAt(index - 1) != '[')
                newValue = newValue.replace(
                    new RegExp(item, 'gm'),
                    '<span class="business-logic-dest-item">' + item + '</span>');
        });

        return newValue;
    }
}) ());