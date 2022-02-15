// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appElementDetail').directive('maWorkflowStatus', ['$compile', 'workflowStatusService', function($compile, workflowStatusService) {
        return {
            restrict: 'E',
            scope: {
                mapping: '=',
                saveSuccess: '=',
                readOnly: '=',
                control: '=',
                loading: '='
            },
            link: function(scope, element, attr) {
                var mode = attr.mode || 'standard';
                mode = mode.toLowerCase();
                scope.updateStatus = updateStatus;
                scope.updateFlagged = updateFlagged;
                scope.updateNote = updateNote;
                scope.saveSuccess = scope.saveSuccess || function() {};
                scope.readOnly = scope.readOnly || false;
                scope.loading = scope.loading || false;
                scope.statusNoteUpdate = scope.mapping ? scope.mapping.StatusNote : '';
                scope.oldMapping = angular.copy(scope.mapping);
                scope.reloadMapping = function() {
                    element.empty();
                    if (scope.oldMapping && scope.mapping && (scope.oldMapping.SystemItemMapId != scope.mapping.SystemItemMapId)) {
                        scope.statusNoteUpdate = scope.mapping.StatusNote;
                        scope.oldMapping = angular.copy(scope.mapping);
                    }
                    var el = angular.element(getHtml(scope.mapping, mode, scope.readOnly));
                    var compiled = $compile(el);
                    element.append(el);
                    compiled(scope);
                }
                scope.reloadMapping();

                scope.control = scope.control || {};
                scope.control.reloadMapping = function(mapping) {
                    scope.mapping = mapping;
                    scope.reloadMapping();
                }

                function updateStatus(mapping, workflowStatusTypeId) {
                    var originalWorkflowStatusTypeId = mapping.WorkflowStatusTypeId;
                    mapping.WorkflowStatusTypeId = workflowStatusTypeId;
                    scope.reloadMapping();
                    save(mapping)
                        .catch(function(error) {
                            mapping.WorkflowStatusTypeId = originalWorkflowStatusTypeId;
                            scope.reloadMapping();
                            scope.$emit('mapping-error', error);
                        });
                }

                function updateFlagged(mapping, flagged) {
                    var originalFlagged = mapping.Flagged;
                    mapping.Flagged = flagged;
                    scope.reloadMapping();
                    save(mapping)
                        .catch(function(error) {
                            mapping.Flagged = originalFlagged;
                            scope.reloadMapping();
                            scope.$emit('mapping-error', error);
                        });
                }

                function updateNote(mapping, statusNote) {
                    mapping.StatusNote = statusNote;
                    save(mapping)
                        .catch(function(error) {
                            scope.$emit('mapping-error', error);
                        });
                }

                function save(mapping) {
                    return workflowStatusService.update(mapping.SourceSystemItemId, mapping.SystemItemMapId, mapping)
                        .then(function() {
                            scope.saveSuccess();
                            scope.reloadMapping();
                            scope.$emit('mapping-saved');
                        });
                }
            }
        }

        function getHtml(mapping, mode, readOnly) {
            if (mapping == null)
                return "<span>N/A</span>";
            var standardMode = mode == 'standard';
            var listMode = mode == 'list';
            var html = '<div';
            html += listMode && mapping != null && mapping.StatusNote ? ' title="' + mapping.StatusNote + '"' : '';
            html += '>';
            if (standardMode) {
                html += '<div class="workflow-header">';
                html += '<div class="mapping">Mapping</div><div class="review">Review</div><div class="approval">Approval</div><div class="flag">Flag</div></div>';
                html += '<label class="workflow-label">Workflow Status:</label>';
            }
            html += '<div class="workflow-jellybean ';
            html += mode;
            html += '">';
            html += '<a role="button" title="Completed"';
            html += !readOnly ? ' ng-click="updateStatus(mapping, ' : '';
            html += !readOnly ? (mapping != null && mapping.WorkflowStatusTypeId > 1 ? 1 : 2) : '';
            html += !readOnly ? ');"' : '';
            html += ' class="complete-status';
            html += (mapping != null && mapping.WorkflowStatusTypeId > 1 ? ' workflow-checked' : '');
            html += '" data-status-id="2">';
            if (mapping == null || mapping.WorkflowStatusTypeId <= 1)
                html += standardMode ? '<span></span>' : '<span>C</span>';
            else
                html += '<i class="fa fa-check"></i>' + (standardMode ? ' Completed' : '');
            html += '</a>';
            html += '<a role="button" title="Reviewed"';
            html += !readOnly ? ' ng-click="updateStatus(mapping, ' : '';
            html += !readOnly ? (mapping != null && mapping.WorkflowStatusTypeId > 2 ? 2 : 3) : '';
            html += !readOnly ? ');"' : '';
            html += ' class="reviewed-status';
            html += (mapping != null && mapping.WorkflowStatusTypeId > 2 ? ' workflow-checked' : '');
            html += '" data-status-id="3">';
            if (mapping == null || mapping.WorkflowStatusTypeId <= 2)
                html += standardMode ? '<span></span>' : '<span>R</span>';
            else
                html += '<i class="fa fa-check"></i>' + (standardMode ? ' Reviewed' : '');
            html += '</a>';
            html += '<a role="button" title="Approved"';
            html += !readOnly ? ' ng-click="updateStatus(mapping, ' : '';
            html += !readOnly ? (mapping != null && mapping.WorkflowStatusTypeId > 3 ? 3 : 4) : '';
            html += !readOnly ? ');"' : '';
            html += ' class="approved-status';
            html += (mapping != null && mapping.WorkflowStatusTypeId > 3 ? ' workflow-checked' : '');
            html += '" data-status-id="4">';
            if (mapping == null || mapping.WorkflowStatusTypeId <= 3)
                html += standardMode ? '<span></span>' : '<span>A</span>';
            else
                html += '<i class="fa fa-check"></i>' + (standardMode ? ' Approved' : '');
            html += '</a>';
            html += '</div>';
            html += '<div class="flagged-jellybean ';
            html += mode;
            html += '">';
            html += '<a role="button" title="Flagged"';
            html += !readOnly ? ' ng-click="updateFlagged(mapping, ' : '';
            html += !readOnly ? (mapping != null && !mapping.Flagged ? 'true' : 'false') : '';
            html += !readOnly ? ');"' : '';
            html += ' class="flagged-status';
            if (mapping != null && mapping.Flagged) {
                html += ' on"><i class="fa fa-flag"></i>' + (standardMode ? ' Flagged' : '');
            } else {
                html += '">';
            }
            html += '</a>';
            html += '</div>';
            if (standardMode) {
                html += '<div class="status-note"><label class="workflow-label">Status Note:</label>';
                html += '<textarea ';
                html += readOnly ? 'readonly' : '';
                html += ' class="form-control" ng-model="statusNoteUpdate"></textarea>';
                if (!readOnly) {
                    html += '<div class="pull-right">';
                    html += '<button type="button" class="btn btn-cancel" ng-disabled="loading" ng-click="statusNoteUpdate = mapping.StatusNote"><i class="fa"></i>Cancel</button>';
                    html += '<button type="button" class="btn btn-happy btn-save" ng-disabled="loading" ng-click="updateNote(mapping, statusNoteUpdate);"><i class="fa"></i>Save</button>';
                    html += '</div>';
                }
                html += '</div>';
            }
            html += '</div>';
            return html;
        }
    }
]);