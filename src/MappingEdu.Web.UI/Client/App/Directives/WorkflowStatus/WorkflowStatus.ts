// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module 'app.directives.workflow-status'
//

var m = angular.module('app.directives.workflow-status', []);


// ****************************************************************************
// Directive ma-workflow-status
//

m.directive('maWorkflowStatus', ['enumerations', 'repositories', 'services', 'settings', (enumerations: IEnumerations, repositories: IRepositories, services: IServices, settings: ISystemSettings) => {
        return {
            restrict: 'E',
            scope: {
                mapping: '=',
                listMode: '=',
                mappingProjectId: '=',
                readOnly: '='
            },
            templateUrl: `${settings.directiveBaseUri}/WorkflowStatus/WorkflowStatus.tpl.html`,
            link(scope, el, attr) {

                var statuses = enumerations.WorkflowStatusType;
                
                scope.updateStatus = (mapping, workflowStatusTypeId) => {
                    if (scope.readOnly) return;
                    var origStatus = mapping.WorkflowStatusTypeId;
                    if (mapping.WorkflowStatusTypeId === workflowStatusTypeId) mapping.WorkflowStatusTypeId--;
                    else mapping.WorkflowStatusTypeId = workflowStatusTypeId;

                    repositories.element.workflowStatus.save(mapping.SourceSystemItemId, mapping.SystemItemMapId, mapping).then(() => {
                        services.logger.success('Saved workflow status.');

                        var status = services.underscore.find(statuses, (status) => { return mapping.WorkflowStatusTypeId === status.Id });
                        mapping.WorkflowStatusType.Name = status.DisplayText;

                    }, error => {
                        services.logger.error('Error saving workflow status.', error.data);
                        mapping.WorkflowStatusTypeId = origStatus;
                    });
                }

                scope.updateFlagged = (mapping) => {
                    if (scope.readOnly) return;
                    mapping.Flagged = !mapping.Flagged;
                    repositories.element.workflowStatus.save(mapping.SourceSystemItemId, mapping.SystemItemMapId, mapping).then(() => {
                        services.logger.success(`${mapping.Flagged ? 'Flagged' : 'Unflagged'} mapping.`);
                    }, error => {
                        services.logger.error(`Error ${mapping.Flagged ? 'flagging' : 'unflagging'} mapping.`, error.data);
                        mapping.Flagged = !mapping.Flagged;
                    });
                }
            }
        }
    }
]);