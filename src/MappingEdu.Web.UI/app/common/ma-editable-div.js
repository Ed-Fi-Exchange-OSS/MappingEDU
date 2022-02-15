// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

(function () {
    angular.module('appCommon')
        .directive('maEditableDiv', ['$timeout', function($timeout) {
                return {
                    restrict: 'A',
                    require: 'ngModel',
                    link: function(scope, element, attrs, ngModel) {
                        function read() {
                            ngModel.$setViewValue(element.html()
                                .replace(/(<p>)|(<div>)|(<span class=\"standard-b\">)|(<\/span>)|(&nbsp;)/g, '')
                                .replace(/(<br>)|(<br \/>)|(<\/p>)|(<\/div>)/g, '\r\n')
                                .replace(/&lt;/g, '<')
                                .replace(/&gt;/g, '>')
                                .replace(/&amp;/g, '&'));
                        }

                        function render() {
                            var newValue = (ngModel.$viewValue == undefined ||
                                    ngModel.$viewValue == null) ? '' :
                                ngModel.$viewValue
                                .replace(/\r\n/g, '<br>')
                                .replace(/\[/g, '<span class="standard-b">[')
                                .replace(/\]/g, ']</span>&nbsp;');
                            element.html(newValue);
                        };

                        function getCaretPosition() {
                            var sel = window.getSelection();
                            var range = sel.getRangeAt(0);
                            var caretPos = range.endOffset;
                            return caretPos;
                        }

                        function setCaretPastSpan() {
                            var sel = window.getSelection();
                            if (sel.focusNode.parentNode.localName === 'span') {
                                sel.focusNode.parentNode.outerHTML += ' ';
                                setCaret(sel.focusNode.parentNode);
                            }
                        }

                        function setCaretToEnd() {
                            var textNode = element[0].lastChild;
                            setCaret(textNode);
                        }

                        function setCaret(textNode) {
                            var caret = textNode.length;
                            var range = document.createRange();
                            range.setStart(textNode, caret);
                            range.collapse(true);
                            var sel = window.getSelection();
                            sel.removeAllRanges();
                            sel.addRange(range);
                        }

                        element.bind('blur keyup change', function(data) {
                            scope.$apply(read);
                            if (data.keyCode === 221) {
                                render();
                                setCaretToEnd();
                            }

                            if (data.keyCode === 32) {
                                setCaretPastSpan();
                            }
                        });

                        ngModel.$render = render;
                    }
                };
            }
        ]);
}());