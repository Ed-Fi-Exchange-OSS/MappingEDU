// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appElementDetail').controller('elementDetailMappingController', [
    '_', '$scope', '$timeout', '$state', '$stateParams', '$templateRequest', 'elementService', 'elementMappingService',
    'mapNoteService', 'handleErrorService', 'breadcrumbService', 'enumerationItemMappingService', 'loggingService',
    function (_, $scope, $timeout, $state, $stateParams, $templateRequest, elementService, elementMappingService,
        mapNoteService, handleErrorService, breadcrumbService, enumerationItemMappingService, loggingService) {
        var elementDetailMappingViewModel = this;

        $scope.form = {};
        elementDetailMappingViewModel.emptyGuid = '{00000000-0000-0000-0000-000000000000}';
        elementDetailMappingViewModel.controlWorkflowStatus = {};

        elementDetailMappingViewModel.mappingProjectId = $stateParams.mappingProjectId || elementDetailMappingViewModel.emptyGuid;
        elementDetailMappingViewModel.dataStandardId = $stateParams.dataStandardId || elementDetailMappingViewModel.emptyGuid;
        elementDetailMappingViewModel.id = elementDetailMappingViewModel.mappingProjectId != elementDetailMappingViewModel.emptyGuid
            ? elementDetailMappingViewModel.mappingProjectId
            : elementDetailMappingViewModel.dataStandardId;
        elementDetailMappingViewModel.current = $stateParams.current;
        elementDetailMappingViewModel.showBusinessLogicEdit = false;

        if (!_.isUndefined($scope.$parent.elementDetailViewModel.mappingProject))
            elementDetailMappingViewModel.mappingProject = $scope.$parent.elementDetailViewModel.mappingProject;

        elementDetailMappingViewModel.businessLogicChange = function() {
            if (_.isUndefined(elementDetailMappingViewModel.mapping)) {
                elementDetailMappingViewModel.businessLogicLabel = '';
                return;
            }

            if (elementDetailMappingViewModel.mapping.MappingMethodTypeId == 4) {
                elementDetailMappingViewModel.showOmissionReasonSection = true;
                elementDetailMappingViewModel.showBusinessLogicSection = false;
                return;
            }

            elementDetailMappingViewModel.showOmissionReasonSection = false;
            elementDetailMappingViewModel.showBusinessLogicSection = true;

            if (elementDetailMappingViewModel.mapping.MappingMethodTypeId == 1)
                elementDetailMappingViewModel.businessLogicLabel = '';
            else if (elementDetailMappingViewModel.mapping.MappingMethodTypeId == 2)
                elementDetailMappingViewModel.businessLogicLabel = 'Inclusion Detail';
            else if (elementDetailMappingViewModel.mapping.MappingMethodTypeId == 3)
                elementDetailMappingViewModel.businessLogicLabel = 'Extension Detail';
        };

        elementDetailMappingViewModel.initializeMappingMethod = function(mapping) {
            mapping.MappingMethodTypeId = mapping.MappingMethodTypeId ||
                _.find(Application.Enumerations.MappingMethodType, function(item) {
                    return item.DisplayText == 'Enter Mapping Business Logic';
                }).Id;
            elementDetailMappingViewModel.businessLogicChange();
        };

        elementDetailMappingViewModel.loadElement = function(element) {
            elementDetailMappingViewModel.element = element;
            if (element) {
                elementDetailMappingViewModel.elementId = element.ElementDetails.SystemItemId;
                elementDetailMappingViewModel.mapping = element.SystemItemMappings[0] || {};

                elementDetailMappingViewModel.initializeMappingMethod(elementDetailMappingViewModel.mapping);
                if (typeof elementDetailMappingViewModel.controlWorkflowStatus.reloadMapping === "function")
                    elementDetailMappingViewModel.controlWorkflowStatus.reloadMapping(elementDetailMappingViewModel.mapping);
            }
            elementDetailMappingViewModel.oldElement = angular.copy(elementDetailMappingViewModel.element);
        }

        elementDetailMappingViewModel.loadElement($scope.$parent.elementDetailViewModel.element);

        elementDetailMappingViewModel.loadEnumeration = function(enumeration) {
            elementDetailMappingViewModel.enumeration = enumeration;

            loadMissingEnumerationItemMaps();
        };

        function loadMissingEnumerationItemMaps() {
            if (elementDetailMappingViewModel.element && elementDetailMappingViewModel.mapping.SystemItemMapId) {
                var missingEnumItems = _.filter(elementDetailMappingViewModel.enumeration.EnumerationItems, function(enumItem) {
                    return !_.find(elementDetailMappingViewModel.mapping.EnumerationItemMappings, function(enumMap) {
                        return enumItem.SystemEnumerationItemId == enumMap.SourceSystemEnumerationItemId;
                    });
                });
                elementDetailMappingViewModel.mapping.MissingEnumerationItemMappings = _.map(missingEnumItems, function(item) {
                    return { SourceCodeValue: item.CodeValue, SourceSystemEnumerationItemId: item.SystemEnumerationItemId }
                });
            }
        }

        elementDetailMappingViewModel.getTargetPath = function(targetSystemItem) {
            if (!targetSystemItem)
                return '';

            if (targetSystemItem.ParentSystemItem)
                return elementDetailMappingViewModel.getTargetPath(targetSystemItem.ParentSystemItem) + '.' + targetSystemItem.ItemName;

            return targetSystemItem.ItemName;
        };

        elementDetailMappingViewModel.workflowStatusTypes = Application.Enumerations.WorkflowStatusType;

        elementDetailMappingViewModel.enumerationMappingStatusTypes = _.filter(
            Application.Enumerations.EnumerationMappingStatusType, function(item) {
                return item.Id;
            });

        elementDetailMappingViewModel.filterMappingStatusType = function() {
            return function(item) {
                // Remove the following values as selections for Enumeration Item Mappings - (reason: all values indicate an extension to a type which is not allowed)
                // Approved Core Type/Descriptor MapExtension = 2
                // Approved Enumeration = 3
                // Proposed CoreType/Descriptor MapExtension = 10
                // Proposed Enumeration = 11
                if (item.Id == 2 || item.Id == 3 || item.Id == 10 || item.Id == 11)
                    return false;
                return true;
            }
        }

        elementDetailMappingViewModel.enumerationMappingStatusReasonTypes = _.filter(
            Application.Enumerations.EnumerationMappingStatusReasonType, function(item) {
                return item.Id;
            });

        $scope.$on('element-fetched', function(event, data) {
            handleErrorService.clearErrors(elementDetailMappingViewModel);
            elementDetailMappingViewModel.loadElement(data);
        });

        $scope.$on('enumeration-fetched', function(event, data) {
            elementDetailMappingViewModel.loadEnumeration(data);
        });

        elementDetailMappingViewModel.emitElementChangedEvent = function() {
            $scope.$emit('element-changed');
        };

        $scope.$on('mapping-saved', function() {
            handleErrorService.clearErrors(elementDetailMappingViewModel);
            elementDetailMappingViewModel.emitElementChangedEvent();
        });

        $scope.$on('mapping-error', function(event, data) {
            handleErrorService.handleErrors(data, elementDetailMappingViewModel);
        });

        $scope.$on('mapping-project-changed', function(event, data) {
            elementDetailMappingViewModel.mappingProject = data;
        });

        elementDetailMappingViewModel.save = function(mapping) {
            $scope.$broadcast('show-errors-check-valid');
            if ($scope.form.elementDetailMappingForm.$invalid)
                return;

            elementDetailMappingViewModel.loading = true;
            if (_.isUndefined(mapping) || _.isUndefined(mapping.SystemItemMapId)) {
                mapping = mapping || {};
                mapping.MappingProjectId = elementDetailMappingViewModel.id;
                elementMappingService.add(elementDetailMappingViewModel.elementId, mapping)
                    .then(function() {
                        elementDetailMappingViewModel.saveSuccess();
                        elementDetailMappingViewModel.emitElementChangedEvent();
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, elementDetailMappingViewModel);
                    })
                    .finally(function() {
                        elementDetailMappingViewModel.loading = false;
                    });
            } else {
                elementMappingService.update(elementDetailMappingViewModel.elementId, mapping.SystemItemMapId, mapping)
                    .then(function() {
                        elementDetailMappingViewModel.saveSuccess();
                        elementDetailMappingViewModel.emitElementChangedEvent();
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, elementDetailMappingViewModel);
                    })
                    .finally(function() {
                        elementDetailMappingViewModel.loading = false;
                    });
            }
        };

        elementDetailMappingViewModel.cancel = function() {
            $scope.$broadcast('show-errors-reset');
            elementDetailMappingViewModel.loadElement(angular.copy(elementDetailMappingViewModel.oldElement));
            elementDetailMappingViewModel.showBusinessLogicEdit = false;
        };

        elementDetailMappingViewModel.saveSuccess = function() {
            handleErrorService.clearErrors(elementDetailMappingViewModel);
            elementDetailMappingViewModel.successData = {};
            elementDetailMappingViewModel.successData.success = true;
            elementDetailMappingViewModel.successData.message = 'Mapping data has been saved.';
            elementDetailMappingViewModel.showBusinessLogicEdit = false;
            $scope.$emit('workflow-status-updated');
            $timeout(function() { elementDetailMappingViewModel.successData.success = false; }, 2000);
        };

        elementDetailMappingViewModel.editNote = function(note) {
            if (!note.editNote)
                note.editNote = angular.copy(note);
        };

        elementDetailMappingViewModel.saveNote = function(note) {
            elementDetailMappingViewModel.loading = true;
            mapNoteService.update(elementDetailMappingViewModel.mapping.SystemItemMapId, note.MapNoteId, note.editNote)
                .then(function(savedNote) {
                    angular.extend(note, savedNote);
                    note.editNote = undefined;
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, elementDetailMappingViewModel);
                })
                .finally(function() {
                    elementDetailMappingViewModel.loading = false;
                });
        };

        elementDetailMappingViewModel.cancelEditNote = function(note) {
            $scope.$broadcast('show-errors-reset');
            note.editNote = undefined;
        };

        elementDetailMappingViewModel.deleteNote = function(note) {
            elementDetailMappingViewModel.loading = true;
            mapNoteService.delete(elementDetailMappingViewModel.mapping.SystemItemMapId, note.MapNoteId)
                .then(function() {
                    elementDetailMappingViewModel.mapping.MapNotes.pop(note);
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, elementDetailMappingViewModel);
                })
                .finally(function() {
                    elementDetailMappingViewModel.loading = false;
                });
        };

        elementDetailMappingViewModel.addNote = function() {
            elementDetailMappingViewModel.noteToAdd = { Title: '', Notes: '' };
        };

        elementDetailMappingViewModel.saveAddedNote = function() {
            elementDetailMappingViewModel.loading = true;
            mapNoteService.add(elementDetailMappingViewModel.mapping.SystemItemMapId, elementDetailMappingViewModel.noteToAdd)
                .then(function(addedNote) {
                    elementDetailMappingViewModel.mapping.MapNotes.push(addedNote);
                    elementDetailMappingViewModel.noteToAdd = undefined;
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, elementDetailMappingViewModel);
                })
                .finally(function() {
                    elementDetailMappingViewModel.loading = false;
                });
        };

        elementDetailMappingViewModel.cancelAddNote = function() {
            $scope.$broadcast('show-errors-reset');
            elementDetailMappingViewModel.noteToAdd = undefined;
        }

        elementDetailMappingViewModel.mappingMethodTypes = _.filter(Application.Enumerations.MappingMethodType, function(item) {
            return item.Id != 0;
        });

        elementDetailMappingViewModel.deleteEnumMapping = function(enumerationMapping) {
            enumerationItemMappingService.delete(enumerationMapping.SystemItemMapId, enumerationMapping.SystemEnumerationItemMapId)
                .then(function() {
                    elementDetailMappingViewModel.emitElementChangedEvent();
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, elementDetailMappingViewModel);
                })
                .finally(function() {
                    elementDetailMappingViewModel.loading = false;
                });
        }

        elementDetailMappingViewModel.addEnumerationMapping = function() {
            elementDetailMappingViewModel.editedEnumerationMapping = {};
            elementDetailMappingViewModel.editedEnumerationMapping.SourceSystemEnumerationItemId =
                elementDetailMappingViewModel.enumeration.EnumerationItems[0].SystemEnumerationItemId;
        }

        elementDetailMappingViewModel.editEnumerationMapping = function(enumerationMapping) {
            elementDetailMappingViewModel.editedEnumerationMapping = enumerationMapping;
        }

        elementDetailMappingViewModel.saveEnumerationMapping = function() {
            if (elementDetailMappingViewModel.editedEnumerationMapping.SystemEnumerationItemMapId) {
                enumerationItemMappingService.update(elementDetailMappingViewModel.mapping.SystemItemMapId,
                        elementDetailMappingViewModel.editedEnumerationMapping.SystemEnumerationItemMapId,
                        elementDetailMappingViewModel.editedEnumerationMapping)
                    .then(function() {
                        elementDetailMappingViewModel.emitElementChangedEvent();
                        $scope.$broadcast('show-errors-reset');
                        angular.element("#enumerationMappingModal").modal('hide');
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, elementDetailMappingViewModel);
                    })
                    .finally(function() {
                        elementDetailMappingViewModel.loading = false;
                    });
            } else {
                enumerationItemMappingService.add(elementDetailMappingViewModel.mapping.SystemItemMapId,
                        elementDetailMappingViewModel.editedEnumerationMapping)
                    .then(function() {
                        elementDetailMappingViewModel.emitElementChangedEvent();
                        $scope.$broadcast('show-errors-reset');
                        angular.element("#enumerationMappingModal").modal('hide');
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, elementDetailMappingViewModel);
                    })
                    .finally(function() {
                        elementDetailMappingViewModel.loading = false;
                    });
            }
        }

        elementDetailMappingViewModel.cancelEnumerationMapping = function() {
            $scope.$broadcast('show-errors-reset');
            angular.element("#enumerationMappingModal").modal('hide');
        };

        elementDetailMappingViewModel.addTemplate = function(template) {
            elementDetailMappingViewModel.showBusinessLogicEdit = true;

            $templateRequest('app/elementDetail/mappingTemplates/' + template + '.html').then(function(templateText) {
                $timeout(function() {
                    insertTemplate(templateText);
                }, 100);
            });
        }

        elementDetailMappingViewModel.toggleTargetCaret = function(index) {
            elementDetailMappingViewModel.showTarget[index] = !elementDetailMappingViewModel.showTarget[index];
        };

        elementDetailMappingViewModel.showMappingStatus = true;
        elementDetailMappingViewModel.toggleMappingStatusCaret = function() {
            elementDetailMappingViewModel.showMappingStatus = !elementDetailMappingViewModel.showMappingStatus;
        };

        elementDetailMappingViewModel.showMappingBusinessLogic = true;
        elementDetailMappingViewModel.toggleMappingBusinessLogicCaret = function() {
            elementDetailMappingViewModel.showMappingBusinessLogic = !elementDetailMappingViewModel.showMappingBusinessLogic;
        };

        elementDetailMappingViewModel.showEnumerationMappings = false;
        elementDetailMappingViewModel.toggleEnumerationMappingsCaret = function() {
            elementDetailMappingViewModel.showEnumerationMappings = !elementDetailMappingViewModel.showEnumerationMappings;
        }

        elementDetailMappingViewModel.showNotes = false;
        elementDetailMappingViewModel.toggleNotesCaret = function() {
            elementDetailMappingViewModel.showNotes = !elementDetailMappingViewModel.showNotes;
        };

        function insertTemplate(data) {

            var blTextArea = angular.element('#businessLogic');
            blTextArea.focus();
            var selEndPosition = angularApp.Utils.getCaretPosition(blTextArea[0]);
            if (selEndPosition >= 0) {
                blTextArea[0].value =
                    angularApp.Utils.insertIntoBusinessLogic(blTextArea[0].value, data, selEndPosition);
                setCursorPositionAndSelection(blTextArea[0]);
                elementDetailMappingViewModel.mapping.BusinessLogic = blTextArea[0].value;
                return;
            }
        }

        function setCursorPositionAndSelection(control) {
            if (_.isUndefined(control))
                return;
            var startPos = control.value.indexOf("{{cursor}}");
            if (startPos > -1)
                control.value = control.value.replace('{{cursor}}', '');
            var endPos = control.value.indexOf('{{endSelect}}');
            if (endPos == -1)
                endPos = startPos;
            else
                control.value = control.value.replace('{{endSelect}}', '');

            if (startPos == -1)
                return;

            $timeout(function() { angularApp.Utils.setCaretPosition(control, startPos, endPos); }, 0);
        }

        elementDetailMappingViewModel.showMatchmakerModal = function() {
            $("#matchmakerModal").modal("show");
            $('#matchmakerModal').off('hidden.bs.modal');
            $("#matchmakerModal").on('hidden.bs.modal', function () {
                loggingService.add({ Source: 'elementDetailMappingController', Message: 'Matchhmaker dialog closed.' });
            });
        }

        breadcrumbService.withCurrent();
    }
]);
