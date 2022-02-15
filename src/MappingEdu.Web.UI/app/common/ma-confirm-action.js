// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

(function () {
    angular.module('appMappingEdu').
        directive('maConfirmAction', ['$document', function($document) {
                return {
                    restrict: 'A',
                    scope: {
                        confirmAction: '&maConfirmAction',
                        placementCallback: '&'
                    },
                    link: function(scope, element, attrs) {
                        var buttonId = Math.floor(Math.random() * 10000000000),
                            message = attrs.message || "Are you sure?",
                            yep = attrs.yes || "Yes",
                            nope = attrs.no || "No",
                            title = attrs.title || "Confirm delete?",
                            classes = attrs.classes || "text-center, popover-delete",
                            placement = attrs.placement || "bottom",
                            modal = attrs.modal && attrs.modal.toLowerCase() === 'true' ? true : false;

                        attrs.buttonId = buttonId;

                        var html = "<div id=\"button-" + buttonId + "\" class=\"" + classes + "\">" +
                            "<button style=\"margin-left:10px;\" type=\"button\" class=\"confirmbutton-yes btn btn-sm btn-danger\">" + yep + "</button> " +
                            "<button type=\"button\" class=\"confirmbutton-no btn btn-sm\">" + nope + "</button>" +
                            "</div>";

                        if (modal) {
                            var modalId = "modal" + buttonId;
                            html = '<div class=\'modal fade\' id=\'' + modalId + '\'>' +
                                '<div class=\'modal-dialog\'>' +
                                '<div class=\'modal-content\'>' +
                                '<div class=\'modal-header\'>' +
                                title +
                                '</div>' +
                                '<div class=\'modal-body\'>' +
                                html +
                                '</div>' +
                                '</div>' +
                                '</div>' +
                                '</div>';
                            $(html).insertAfter(element);
                            element.off('click');
                            element.on('click', function(e) {
                                var dontBubble = true;
                                e.stopPropagation();
                                $('#' + modalId).modal({ backdrop: 'static', keyboard: 'false' });
                                var button = $("#button-" + buttonId);

                                button.closest(".modal").off('click');
                                button.closest(".modal").on('click', function(e) {
                                    if (dontBubble) {
                                        e.stopPropagation();
                                    }
                                });
                                button.find('.confirmbutton-yes').off('click');
                                button.find('.confirmbutton-yes').on('click', function(e) {
                                    dontBubble = false;
                                    scope.$apply(scope.confirmAction);
                                    $('#' + modalId).modal('hide');
                                });

                                button.find('.confirmbutton-no').off('click');
                                button.find('.confirmbutton-no').on('click', function(e) {
                                    dontBubble = false;
                                    $('#' + modalId).modal('hide');
                                });

                            });
                        } else {
                            element.popover({
                                content: html,
                                html: true,
                                trigger: "manual",
                                title: title,
                                placement: (angular.isDefined(attrs.placementCallback) ? scope.placementCallback() : placement)
                            });

                            element.off('click');
                            element.bind('click', function(e) {
                                var dontBubble = true;
                                e.stopPropagation();
                                element.popover('show');
                                var pop = $("#button-" + buttonId);

                                pop.closest(".popover").off('click');
                                pop.closest(".popover").on('click', function(e) {
                                    if (dontBubble) {
                                        e.stopPropagation();
                                    }
                                });

                                pop.find('.confirmbutton-yes').off('click');
                                pop.find('.confirmbutton-yes').on('click', function(e) {
                                    dontBubble = false;
                                    scope.$apply(scope.confirmAction);
                                    element.popover('hide');
                                });

                                pop.find('.confirmbutton-no').off('click');
                                pop.find('.confirmbutton-no').on('click', function(e) {
                                    dontBubble = false;
                                    element.popover('hide');
                                });
                            });
                        }
                    }
                };
            }
        ]);
}());