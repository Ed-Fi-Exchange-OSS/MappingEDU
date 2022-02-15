// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

(function() {
    angular.module('appCommon').
        directive('maElementPath', ['$state', '$compile', function($state, $compile) {
                return {
                    restrict: 'E',
                    scope: {
                        contextId: '=',
                        segments: '=',
                        element: '=',
                        depth: '=',
                        linkPath: '=',
                        lastSegmentIsElement: '=',
                        control: '='
                    },
                    link: function(scope, element, attrs) {
                        var context = attrs.context;
                        scope.reloadPath = function(segments, elmnt) {
                            element.empty();
                            var el = angular.element(getHtml(scope.contextId, segments, elmnt, context, scope.depth, scope.linkPath, scope.lastSegmentIsElement));
                            var compiled = $compile(el);
                            element.append(el);
                            compiled(scope);
                        }
                        scope.reloadPath(scope.segments, scope.element);

                        scope.control = scope.control || {};
                        scope.control.reloadPath = function(segments, elmnt) {
                            scope.reloadPath(segments, elmnt);
                        }
                    }
                }

                function getHtml(contextId, segments, element, context, depth, linkPath, lastSegmentIsElement) {
                    var html = '';
                    if (!segments)
                        return html;
                    depth = depth || 3;
                    depth = segments.length == depth + 1 ? segments.length : depth;
                    context = context || 'mappingProject';
                    if (typeof linkPath == 'undefined')
                        linkPath = true;

                    var srefElementGroupTemplate = 'elementGroupDetail.info({' + context + 'Id: \'' + contextId + '\', id: \'SystemItemId\'})';

                    var srefEntityTemplate = 'entityDetail.info({' + context + 'Id: \'' + contextId + '\', id: \'SystemItemId\'})';

                    var srefElementTemplate = 'elementDetail.info({' + context + 'Id: \'' + contextId + '\', current: \'SystemItemId\'})';

                    for (index = 0; index < depth; index++) {
                        if (index <= segments.length - 1) {
                            html += '<span><a class="path-segment"';
                            if (linkPath) {
                                html += ' ui-sref="';
                                if (index == 0)
                                    html += srefElementGroupTemplate.replace('SystemItemId', segments[index].SystemItemId);
                                else if (segments.length - 1 == index && lastSegmentIsElement)
                                    html += srefElementTemplate.replace('SystemItemId', segments[index].SystemItemId);
                                else
                                    html += srefEntityTemplate.replace('SystemItemId', segments[index].SystemItemId);
                                html += '"';
                            }
                            if (segments[index].Definition != null) {
                                html += ' data-toggle="popover" data-trigger="hover" data-content="';
                                html += segments[index].Definition;
                                html += '"';
                            }
                            html += '>';
                            html += segments[index].Name;
                            html += '</a>';
                            html += (index < (depth - 1) && index < (segments.length - 1)) ? '<i class="fa fa-caret-right separator"></i>' : '';
                            html += '</span>';
                        }
                    };

                    if (segments.length > depth) {
                        html += '<i class="fa fa-caret-right separator"></i>';
                        html += '<span>';
                        html += '<div class="dropdown data-standard-dropdown">';
                        html += '<a class="dropdown-toggle" data-toggle="dropdown" role="button" ';
                        html += 'aria-haspopup="true" aria-expanded="false">';
                        html += '<i class="fa fa-ellipsis-h"></i></a>';
                        html += '<ul class="dropdown-menu data-standard-path">';
                        for (i = 0; i < segments.length; i++) {
                            var value = segments[i];
                            html += '<li>';
                            html += '<a';
                            if (linkPath) {
                                html += ' ui-sref="';
                                if (i == 0)
                                    html += srefElementGroupTemplate.replace('SystemItemId', value.SystemItemId);

                                else if (segments.length - 1 == i && lastSegmentIsElement)
                                    html += srefElementTemplate.replace('SystemItemId', value.SystemItemId);
                                else
                                    html += srefEntityTemplate.replace('SystemItemId', value.SystemItemId);
                                html += '"';
                            }
                            if (value.Definition != null) {
                                html += ' data-toggle="popover" data-placement="right" data-trigger="hover" data-content="';
                                html += value.Definition;
                                html += '"';
                            }
                            html += ' href="#/">';
                            html += value.Name;
                            html += '</a>';
                            html += i < segments.length - 1 ? '<i class="fa fa-caret-down"></i></li>' : '';
                        };
                        if (element) {
                            html += '<i class="fa fa-caret-down"></i></li>';
                            html += '<li>';
                            html += '<a';
                            if (linkPath) {
                                html += ' ui-sref="';
                                html += srefElementTemplate.replace('SystemItemId', element.SystemItemId);
                                html += '"';
                            }
                            if (element.Definition != null) {
                                html += ' data-toggle="popover" data-placement="right"';
                                html += ' data-trigger="hover" data-content="';
                                html += element.Definition;
                                html += '"';
                            }
                            html += '  href="#/">';
                            html += element.Name;
                            html += '</a></li>';
                        } else {
                            html += '</li>';
                        }
                        html += '</ul>';
                        html += '</div>';
                    }
                    if (element) {
                        html += '<i class="fa fa-caret-right separator"></i>';
                        html += '</span>';
                        html += '<a';
                        if (linkPath) {
                            html += ' ui-sref="';
                            html += srefElementTemplate.replace('SystemItemId', element.SystemItemId);
                            html += '"';
                        }
                        html += ' data-toggle="popover"';
                        html += ' data-trigger="hover" data-content="';
                        html += (element.Definition == null ? '' : element.Definition);
                        html += '" class="standard-a">';
                        html += element.Name;
                        html += '</a><br />';
                    } else if (segments.length > depth) {
                        html += '<i class="fa fa-caret-right separator"></i>';
                        html += '</span>';
                        html += '<a';
                        if (linkPath) {
                            html += ' ui-sref="';
                            if (lastSegmentIsElement)
                                html += srefElementTemplate.replace('SystemItemId', segments[segments.length - 1].SystemItemId);
                            else
                                html += srefEntityTemplate.replace('SystemItemId', segments[segments.length - 1].SystemItemId);
                            html += '"';
                        }
                        html += ' data-toggle="popover"';
                        html += ' data-trigger="hover" data-content="';
                        html += (segments[segments.length - 1].Definition == null ? '' : segments[segments.length - 1].Definition);
                        html += '" class="path-segment">';
                        html += segments[segments.length - 1].Name;
                        html += '</a><br />';
                    }

                    return html;
                }
            }
        ]);
}());