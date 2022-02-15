// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appElementDetail').controller('elementDetailInfoController', [
    '_', '$compile', '$scope', '$timeout', '$state', '$stateParams', 'elementService', 'elementDetailService', 'noteService', 'elementListService', 'nextDataStandardService',
    'previousVersionDeltaService', 'nextVersionDeltaService', 'systemItemDetailService', 'systemItemEnumerationService', 'handleErrorService', 'breadcrumbService', 'sessionService',
    function(_, $compile, $scope, $timeout, $state, $stateParams, elementService, elementDetailService, noteService, elementListService, nextDataStandardService,
        previousVersionDeltaService, nextVersionDeltaService, systemItemDetailService, systemItemEnumerationService, handleErrorService, breadcrumbService, sessionService) {
        var elementDetailInfoViewModel = this;
        var emptyGuid = '{00000000-0000-0000-0000-000000000000}';

        elementDetailInfoViewModel.mappingProjectId = $stateParams.mappingProjectId || emptyGuid;
        elementDetailInfoViewModel.dataStandardId = $stateParams.dataStandardId || emptyGuid;
        elementDetailInfoViewModel.id = elementDetailInfoViewModel.mappingProjectId != emptyGuid
            ? elementDetailInfoViewModel.mappingProjectId
            : elementDetailInfoViewModel.dataStandardId;
        elementDetailInfoViewModel.showNextVersionItem = true;

        elementDetailInfoViewModel.elementListPreviousLoaded = false;
        elementDetailInfoViewModel.elementListNextLoaded = false;

        elementDetailInfoViewModel.loadElement = function(element) {
            elementDetailInfoViewModel.element = element;
            elementDetailInfoViewModel.oldElement = angular.copy(elementDetailInfoViewModel.element);
        }

        elementDetailInfoViewModel.loadElement($scope.$parent.elementDetailViewModel.element);

        $scope.$on('element-fetched', function(event, data) {
            handleErrorService.clearErrors(elementDetailInfoViewModel);
            elementDetailInfoViewModel.loadElement(data);
        });

        $scope.$on('mapping-project-changed', function(event, data) {
            elementDetailInfoViewModel.mappingProject = data;
            getSystemItemEnumerations(elementDetailInfoViewModel.mappingProject.SourceDataStandardId);
        });

        elementDetailInfoViewModel.emitElementChangedEvent = function() {
            $scope.$emit('element-changed');
        };

        elementDetailInfoViewModel.getListNext = function() {
            var timer = $timeout(showLoading, 100);
            elementListService.get(elementDetailInfoViewModel.nextDataStandard.DataStandardId)
                .then(function(data) {
                    elementDetailInfoViewModel.systemItemsNext = [];
                    for (var i = 0; i < data.Elements.length; i++) {
                        var eg = data.Elements[i];
                        if (!eg.PathSegments)
                            eg.PathSegments = [];
                        if (eg.PathSegments.length > 0) {
                            var systemItemId = eg.PathSegments[eg.PathSegments.length - 1].SystemItemId;
                            var ids = elementDetailInfoViewModel.systemItemsNext.filter(function(val) {
                                return val.Id === systemItemId;
                            });
                            if (ids.length == 0) {
                                elementDetailInfoViewModel.systemItemsNext.push({
                                    Id: systemItemId,
                                    Type: 'Entity',
                                    Segments: eg.PathSegments
                                });
                            } else {
                                console.log();
                            };
                        }
                        var elementSegments = angular.copy(eg.PathSegments);
                        elementSegments.push(eg.Element);
                        elementDetailInfoViewModel.systemItemsNext.push({ Id: eg.Element.SystemItemId, Type: eg.Element.ItemTypeName, Segments: elementSegments });
                    }

                    if ($.fn.dataTable.isDataTable('#elementListTableNext')) {
                        var table = $('#elementListTableNext').DataTable();
                        table.clear().rows.add(elementDetailInfoViewModel.systemItemsNext).draw();
                    } else {

                        $('#elementListTableNext').DataTable(
                        {
                            data: elementDetailInfoViewModel.systemItemsNext,
                            order: [[4, "asc"], [1, "asc"]],
                            deferRender: true,
                            createdRow: function(row) {
                                var compiled = $compile(row);
                                var element = compiled($scope);
                                angular.element(row).replaceWith(element);
                            },
                            columns: [
                                {
                                    data: 'Segments',
                                    visible: false,
                                    searchable: true
                                },
                                {
                                    data: 'Segments',
                                    visible: false,
                                    searchable: true
                                },
                                {
                                    data: 'Segments',
                                    sortable: false
                                },
                                {
                                    data: 'Type',
                                    sortable: false
                                },
                                {
                                    data: 'Type',
                                    visible: false
                                },
                                {
                                    data: 'Segments'
                                }
                            ],
                            columnDefs: [
                                {
                                    targets: 0,
                                    render: function(pathSegments) {
                                        return pathSegments[pathSegments.length - 1].SystemItemId;
                                    }
                                },
                                {
                                    targets: 1,
                                    render: function(pathSegments) {
                                        var html = '<div class="hidden">';
                                        var namePath = '';
                                        for (i = 0; i < pathSegments.length; i++) {
                                            namePath += namePath.length > 0 ? '.' : '';
                                            namePath += pathSegments[i].Name;
                                        }
                                        html += namePath;
                                        html += "</div>";
                                        return html;
                                    }
                                },
                                {
                                    targets: 2,
                                    render: function(pathSegments, type, row, meta) {
                                        var html = '<div class="hidden">';
                                        var namePath = '';
                                        for (i = 0; i < pathSegments.length; i++) {
                                            namePath += namePath.length > 0 ? '.' : '';
                                            namePath += pathSegments[i].Name;
                                        }
                                        if (row.Element) {
                                            namePath += '.' + row.Element.Name;
                                        }
                                        html += namePath;
                                        html += "</div>";
                                        html += '<ma-element-path link-path="false" context-id="elementDetailInfoViewModel.id"';
                                        html += ' segments="elementDetailInfoViewModel.systemItemsNext[';
                                        html += meta.row;
                                        html += '].Segments" context="dataStandard" depth="4"></ma-element-path>';
                                        return html;
                                    }
                                },
                                {
                                    targets: 4,
                                    render: function(type) {
                                        return ['Entity', 'Element', 'Enumeration'].indexOf(type).toString();
                                    }
                                },
                                {
                                    targets: 5,
                                    render: function(segments, type, row, meta) {
                                        return '<a role="button" class="btn btn-happy" ng-click="elementDetailInfoViewModel.selectItemNext(elementDetailInfoViewModel.systemItemsNext['
                                            + meta.row
                                            + '].Segments)" data-toggle="collapse" data-target="#elementListContainerNext"><i class="fa"></i></a>';
                                    }
                                }
                            ]
                        });
                    }
                    elementDetailInfoViewModel.elementListNextLoaded = true;
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, elementDetailInfoViewModel);
                })
                .finally(function() {
                    $timeout.cancel(timer);
                    elementDetailInfoViewModel.loading = false;
                });
        }

        elementDetailInfoViewModel.getListPrevious = function() {
            var timer = $timeout(showLoading, 100);
            elementListService.get($scope.dataStandard.PreviousDataStandardId)
                .then(function(data) {
                    elementDetailInfoViewModel.systemItemsPrevious = [];
                    for (var i = 0; i < data.Elements.length; i++) {
                        var eg = data.Elements[i];
                        if (!eg.PathSegments)
                            eg.PathSegments = [];
                        if (eg.PathSegments.length > 0) {
                            var systemItemId = eg.PathSegments[eg.PathSegments.length - 1].SystemItemId;
                            var ids = elementDetailInfoViewModel.systemItemsPrevious.filter(function(val) {
                                return val.Id === systemItemId;
                            });
                            if (ids.length == 0) {
                                elementDetailInfoViewModel.systemItemsPrevious.push({
                                    Id: systemItemId,
                                    Type: 'Entity',
                                    Segments: eg.PathSegments
                                });
                            } else {
                                console.log();
                            };
                        }
                        var elementSegments = angular.copy(eg.PathSegments);
                        elementSegments.push(eg.Element);
                        elementDetailInfoViewModel.systemItemsPrevious.push({ Id: eg.Element.SystemItemId, Type: eg.Element.ItemTypeName, Segments: elementSegments });
                    }

                    if ($.fn.dataTable.isDataTable('#elementListTablePrevious')) {
                        var table = $('#elementListTablePrevious').DataTable();
                        table.clear().rows.add(elementDetailInfoViewModel.systemItemsPrevious).draw();
                    } else {

                        $('#elementListTablePrevious').DataTable(
                        {
                            data: elementDetailInfoViewModel.systemItemsPrevious,
                            order: [[4, "asc"], [1, "asc"]],
                            deferRender: true,
                            createdRow: function(row) {
                                var compiled = $compile(row);
                                var element = compiled($scope);
                                angular.element(row).replaceWith(element);
                            },
                            columns: [
                                {
                                    data: 'Segments',
                                    visible: false,
                                    searchable: true
                                },
                                {
                                    data: 'Segments',
                                    visible: false,
                                    searchable: true
                                },
                                {
                                    data: 'Segments',
                                    sortable: false
                                },
                                {
                                    data: 'Type',
                                    sortable: false
                                },
                                {
                                    data: 'Type',
                                    visible: false
                                },
                                {
                                    data: 'Segments'
                                }
                            ],
                            columnDefs: [
                                {
                                    targets: 0,
                                    render: function(pathSegments) {
                                        return pathSegments[pathSegments.length - 1].SystemItemId;
                                    }
                                },
                                {
                                    targets: 1,
                                    render: function(pathSegments) {
                                        var html = '<div class="hidden">';
                                        var namePath = '';
                                        for (i = 0; i < pathSegments.length; i++) {
                                            namePath += namePath.length > 0 ? '.' : '';
                                            namePath += pathSegments[i].Name;
                                        }
                                        html += namePath;
                                        html += "</div>";
                                        return html;
                                    }
                                },
                                {
                                    targets: 2,
                                    render: function(pathSegments, type, row, meta) {
                                        var html = '<div class="hidden">';
                                        var namePath = '';
                                        for (i = 0; i < pathSegments.length; i++) {
                                            namePath += namePath.length > 0 ? '.' : '';
                                            namePath += pathSegments[i].Name;
                                        }
                                        if (row.Element) {
                                            namePath += '.' + row.Element.Name;
                                        }
                                        html += namePath;
                                        html += "</div>";
                                        html += '<ma-element-path link-path="false" context-id="elementDetailInfoViewModel.id"';
                                        html += ' segments="elementDetailInfoViewModel.systemItemsPrevious[';
                                        html += meta.row;
                                        html += '].Segments" context="dataStandard" depth="4"></ma-element-path>';
                                        return html;
                                    }
                                },
                                {
                                    targets: 4,
                                    render: function(type) {
                                        return ['Entity', 'Element', 'Enumeration'].indexOf(type).toString();
                                    }
                                },
                                {
                                    targets: 5,
                                    render: function(segments, type, row, meta) {
                                        return '<a role="button" class="btn btn-happy" ng-click="elementDetailInfoViewModel.selectItemPrevious(elementDetailInfoViewModel.systemItemsPrevious['
                                            + meta.row
                                            + '].Segments)" data-toggle="collapse" data-target="#elementListContainerPrevious"><i class="fa"></i></a>';
                                    }
                                }
                            ]
                        });
                    }
                    elementDetailInfoViewModel.elementListPreviousLoaded = true;
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, elementDetailInfoViewModel);
                })
                .finally(function() {
                    $timeout.cancel(timer);
                    elementDetailInfoViewModel.loading = false;
                });
        }

        $('#elementListTablePrevious').on('draw.dt', function() {
            $('[data-toggle="popover"]').popover();

            $('#elementListTablePrevious tbody').on('click', 'tr', function() {
                if ($(this).hasClass('selected')) {
                    $(this).removeClass('selected');
                } else {
                    $('#elementListTablePrevious').DataTable().$('tr.selected').removeClass('selected');
                    $(this).addClass('selected');
                }
            });
        });

        $('#elementListTableNext').on('draw.dt', function() {
            $('[data-toggle="popover"]').popover();

            $('#elementListTableNext tbody').on('click', 'tr', function() {
                if ($(this).hasClass('selected')) {
                    $(this).removeClass('selected');
                } else {
                    $('#elementListTableNext').DataTable().$('tr.selected').removeClass('selected');
                    $(this).addClass('selected');
                }
            });
        });

        function getSystemItemEnumerations(dataStandardId) {
            var timer = $timeout(showLoading, 100);
            systemItemEnumerationService.get(dataStandardId)
                .then(function(data) {
                    elementDetailInfoViewModel.systemItemEnumerations = data;
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, elementDetailInfoViewModel);
                })
                .finally(function() {
                    $timeout.cancel(timer);
                    elementDetailInfoViewModel.loading = false;
                });
        }

        if (elementDetailInfoViewModel.dataStandardId != emptyGuid) {
            elementDetailInfoViewModel.getNextDataStandard = function() {
                nextDataStandardService.get(elementDetailInfoViewModel.dataStandardId)
                    .then(function(data) {
                        if (data) {
                            elementDetailInfoViewModel.nextDataStandard = data;
                        }
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, elementDetailInfoViewModel);
                    });
            }
            elementDetailInfoViewModel.getNextDataStandard();

            getSystemItemEnumerations(elementDetailInfoViewModel.dataStandardId);
        }

        $('#previousVersionModal').on('shown.bs.modal', function(e) {
            if (!$.fn.dataTable.isDataTable('#elementListTablePrevious'))
                elementDetailInfoViewModel.getListPrevious();
        });

        $('#nextVersionModal').on('shown.bs.modal', function(e) {
            if (!$.fn.dataTable.isDataTable('#elementListTableNext'))
                elementDetailInfoViewModel.getListNext();
        });

        function showLoading() {
            elementDetailInfoViewModel.loading = true;
        }

        var sessionElement = sessionService.cloneFromSession('elementDetail', elementDetailInfoViewModel.id);
        if (sessionElement) {
            elementDetailInfoViewModel.element = sessionElement;
        }

        elementDetailInfoViewModel.showElementDetails = true;
        elementDetailInfoViewModel.toggleElementDetailsCaret = function() {
            elementDetailInfoViewModel.showElementDetails = !elementDetailInfoViewModel.showElementDetails;
        }

        elementDetailInfoViewModel.showElementSystemDetails = false;
        elementDetailInfoViewModel.toggleElementSystemDetailsCaret = function() {
            elementDetailInfoViewModel.showElementSystemDetails = !elementDetailInfoViewModel.showElementSystemDetails;
        }

        elementDetailInfoViewModel.showPreviousVersions = false;
        elementDetailInfoViewModel.togglePreviousVersionsCaret = function() {
            elementDetailInfoViewModel.showPreviousVersions = !elementDetailInfoViewModel.showPreviousVersions;
        }

        elementDetailInfoViewModel.showNextVersions = false;
        elementDetailInfoViewModel.toggleNextVersionsCaret = function() {
            elementDetailInfoViewModel.showNextVersions = !elementDetailInfoViewModel.showNextVersions;
        }

        elementDetailInfoViewModel.showNotes = false;
        elementDetailInfoViewModel.toggleNotesCaret = function() {
            elementDetailInfoViewModel.showNotes = !elementDetailInfoViewModel.showNotes;
        };

        elementDetailInfoViewModel.saveElement = function() {
            return elementDetailService.update(elementDetailInfoViewModel.element.ElementDetails.SystemItemId, elementDetailInfoViewModel.element.ElementDetails)
                .then(function(data) {
                    elementDetailInfoViewModel.element.ElementDetails = data;
                    sessionService.cloneToSession('elementDetail', elementDetailInfoViewModel.id, elementDetailInfoViewModel.element);
                    elementDetailInfoViewModel.oldElement = angular.copy(elementDetailInfoViewModel.element);
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, elementDetailInfoViewModel);
                });
        }

        elementDetailInfoViewModel.saveElementDetail = function() {
            $scope.$broadcast('show-errors-check-valid');
            if (elementDetailInfoViewModel.elementForm.$invalid)
                return;

            elementDetailInfoViewModel.saveElement()
                .then(function() {
                    angular.element("#editElementDetailModal").modal('hide');
                    elementDetailInfoViewModel.emitElementChangedEvent();
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, elementDetailInfoViewModel);
                });
        }

        elementDetailInfoViewModel.cancelElementDetail = function() {
            $scope.$broadcast('show-errors-reset');
            elementDetailInfoViewModel.element = angular.copy(elementDetailInfoViewModel.oldElement);
            $scope.$parent.elementDetailViewModel.element = elementDetailInfoViewModel.element;
            angular.element("#editElementDetailModal").modal('hide');
        }


        elementDetailInfoViewModel.editNote = function(note) {
            if (!note.editNote)
                note.editNote = angular.copy(note);
        };

        elementDetailInfoViewModel.saveNote = function(note) {
            noteService.update(elementDetailInfoViewModel.element.SystemItemId, note.NoteId, note.editNote)
                .then(function(savedNote) {
                    angular.extend(note, savedNote);
                    note.editNote = undefined;
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, elementDetailInfoViewModel);
                });
        };

        elementDetailInfoViewModel.cancelEditNote = function(note) {
            note.editNote = undefined;
        };

        elementDetailInfoViewModel.deleteNote = function(note) {
            noteService.delete(elementDetailInfoViewModel.element.SystemItemId, note.NoteId)
                .then(function() {
                    elementDetailInfoViewModel.element.Notes.pop(note);
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, elementDetailInfoViewModel);
                });
        };

        elementDetailInfoViewModel.addNote = function() {
            elementDetailInfoViewModel.noteToAdd = { Title: '', Notes: '' };
        };

        elementDetailInfoViewModel.saveAddedNote = function() {
            noteService.add(elementDetailInfoViewModel.element.SystemItemId, elementDetailInfoViewModel.noteToAdd)
                .then(function(addedNote) {
                    elementDetailInfoViewModel.element.Notes.push(addedNote);
                    elementDetailInfoViewModel.noteToAdd = undefined;
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, elementDetailInfoViewModel);
                });
        };

        elementDetailInfoViewModel.cancelAddNote = function() {
            elementDetailInfoViewModel.noteToAdd = undefined;
        }

        elementDetailInfoViewModel.saveSystemDetails = function() {
            $scope.$broadcast('show-errors-check-valid');
            _.each(elementDetailInfoViewModel.element.SystemItemCustomDetailsContainer.SystemItemCustomDetails, function(item) {
                if (item.CustomDetailMetadata.IsBoolean)
                    item.Value = item.boolValue ? '1' : '0';
            });

            systemItemDetailService.update(elementDetailInfoViewModel.element.SystemItemCustomDetailsContainer)
                .then(function(data) {
                    elementDetailInfoViewModel.element.SystemItemCustomDetailsContainer = data;
                    sessionService.cloneToSession('elementDetail', elementDetailInfoViewModel.id, elementDetailInfoViewModel.element);
                    elementDetailInfoViewModel.oldElement = angular.copy(elementDetailInfoViewModel.element);

                    angular.element("#editElementSystemDetailModal").modal('hide');
                });
        }

        elementDetailInfoViewModel.cancelSystemDetails = function() {
            $scope.$broadcast('show-errors-reset');
            elementDetailInfoViewModel.element = angular.copy(elementDetailInfoViewModel.oldElement);
            angular.element("#editElementSystemDetailModal").modal('hide');
        }

        elementDetailInfoViewModel.addPreviousVersion = function() {
            elementDetailInfoViewModel.editedPreviousVersion = {};
            elementDetailInfoViewModel.editedPreviousVersion.ItemChangeTypeId = 0;
            elementDetailInfoViewModel.determinePreviousVersionItemDisplay();
        }

        elementDetailInfoViewModel.editPreviousVersion = function(version) {
            elementDetailInfoViewModel.editedPreviousVersion = angular.copy(version);
            elementDetailInfoViewModel.editedPreviousVersion.ItemChangeTypeId = version.ItemChangeType.Id;
            elementDetailInfoViewModel.determinePreviousVersionItemDisplay();
        }

        elementDetailInfoViewModel.savePreviousVersion = function() {
            $scope.$broadcast('show-errors-reset');
            $scope.$broadcast('show-errors-check-valid');
            if (elementDetailInfoViewModel.editedPreviousVersion.PreviousVersionId) {
                previousVersionDeltaService.update(elementDetailInfoViewModel.element.ElementDetails.SystemItemId, elementDetailInfoViewModel.editedPreviousVersion.PreviousVersionId, elementDetailInfoViewModel.editedPreviousVersion)
                    .then(function(data) {
                        elementDetailInfoViewModel.emitElementChangedEvent();
                        angular.element("#previousVersionModal").modal('hide');
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, elementDetailInfoViewModel);
                    });
            } else {
                previousVersionDeltaService.add(elementDetailInfoViewModel.element.ElementDetails.SystemItemId, elementDetailInfoViewModel.editedPreviousVersion)
                    .then(function(data) {
                        elementDetailInfoViewModel.emitElementChangedEvent();
                        angular.element("#previousVersionModal").modal('hide');
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, elementDetailInfoViewModel);
                    });
            };
        }

        elementDetailInfoViewModel.cancelPreviousVersion = function() {
            $scope.$broadcast('show-errors-reset');
            elementDetailInfoViewModel.element = angular.copy(elementDetailInfoViewModel.oldElement);
            angular.element("#previousVersionModal").modal('hide');
        }

        elementDetailInfoViewModel.deletePreviousVersion = function(version) {
            previousVersionDeltaService.delete(elementDetailInfoViewModel.element.ElementDetails.SystemItemId, version.PreviousVersionId)
                .then(function() {
                    elementDetailInfoViewModel.emitElementChangedEvent();
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, elementDetailInfoViewModel);
                });
        }

        elementDetailInfoViewModel.selectItemPrevious = function(segments) {
            elementDetailInfoViewModel.editedPreviousVersion.OldSystemItemId = segments[segments.length - 1].SystemItemId;
            elementDetailInfoViewModel.editedPreviousVersion.PreviousVersionItems = segments;
        }

        elementDetailInfoViewModel.clearSelectedItemPrevious = function() {
            elementDetailInfoViewModel.editedPreviousVersion.OldSystemItemId = null;
            elementDetailInfoViewModel.editedPreviousVersion.PreviousVersionItems = [];
        }

        elementDetailInfoViewModel.determinePreviousVersionItemDisplay = function() {
            var changeTypeDisplayText = elementDetailInfoViewModel.editedPreviousVersion.ItemChangeTypeId ? _.find(Application.Enumerations.ItemChangeType, function(item) {
                    return item.Id == elementDetailInfoViewModel.editedPreviousVersion.ItemChangeTypeId;
                }).DisplayText
                : '';
            elementDetailInfoViewModel.showPreviousVersionItem = changeTypeDisplayText.indexOf('Added') != 0 && changeTypeDisplayText.indexOf('Deleted') != 0;
            if (!elementDetailInfoViewModel.showPreviousVersionItem) {
                if (changeTypeDisplayText.indexOf('Added') == 0) {
                    elementDetailInfoViewModel.editedPreviousVersion.OldSystemItemId = null;
                } else {
                    elementDetailInfoViewModel.editedPreviousVersion.NewSystemItemId = null;
                }
            }
        }

        elementDetailInfoViewModel.addNextVersion = function() {
            elementDetailInfoViewModel.editedNextVersion = {};
            elementDetailInfoViewModel.editedNextVersion.ItemChangeTypeId = 0;
            elementDetailInfoViewModel.determineNextVersionItemDisplay();
        }

        elementDetailInfoViewModel.editNextVersion = function(version) {
            elementDetailInfoViewModel.editedNextVersion = angular.copy(version);
            elementDetailInfoViewModel.editedNextVersion.ItemChangeTypeId = version.ItemChangeType.Id;
            elementDetailInfoViewModel.determineNextVersionItemDisplay();
        }

        elementDetailInfoViewModel.saveNextVersion = function() {
            $scope.$broadcast('show-errors-reset');
            $scope.$broadcast('show-errors-check-valid');
            if (elementDetailInfoViewModel.editedNextVersion.NextVersionId) {
                nextVersionDeltaService.update(elementDetailInfoViewModel.element.ElementDetails.SystemItemId, elementDetailInfoViewModel.editedNextVersion.NextVersionId, elementDetailInfoViewModel.editedNextVersion)
                    .then(function(data) {
                        elementDetailInfoViewModel.emitElementChangedEvent();
                        angular.element("#nextVersionModal").modal('hide');
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, elementDetailInfoViewModel);
                    });
            } else {
                nextVersionDeltaService.add(elementDetailInfoViewModel.element.ElementDetails.SystemItemId, elementDetailInfoViewModel.editedNextVersion)
                    .then(function(data) {
                        elementDetailInfoViewModel.emitElementChangedEvent();
                        angular.element("#nextVersionModal").modal('hide');
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, elementDetailInfoViewModel);
                    });
            };
        }

        elementDetailInfoViewModel.cancelNextVersion = function() {
            $scope.$broadcast('show-errors-reset');
            elementDetailInfoViewModel.element = angular.copy(elementDetailInfoViewModel.oldElement);
            angular.element("#nextVersionModal").modal('hide');
        }

        elementDetailInfoViewModel.deleteNextVersion = function(version) {
            nextVersionDeltaService.delete(elementDetailInfoViewModel.element.ElementDetails.SystemItemId, version.NextVersionId)
                .then(function() {
                    elementDetailInfoViewModel.emitElementChangedEvent();
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, elementDetailInfoViewModel);
                });
        }

        elementDetailInfoViewModel.selectItemNext = function(segments) {
            elementDetailInfoViewModel.editedNextVersion.NewSystemItemId = segments[segments.length - 1].SystemItemId;
            elementDetailInfoViewModel.editedNextVersion.NextVersionItems = segments;
        }

        elementDetailInfoViewModel.clearSelectedItemNext = function() {
            elementDetailInfoViewModel.editedNextVersion.NewSystemItemId = null;
            elementDetailInfoViewModel.editedNextVersion.NextVersionItems = [];
        }

        elementDetailInfoViewModel.determineNextVersionItemDisplay = function() {
            var changeTypeDisplayText = elementDetailInfoViewModel.editedNextVersion.ItemChangeTypeId ? _.find(Application.Enumerations.ItemChangeType, function(item) {
                    return item.Id == elementDetailInfoViewModel.editedNextVersion.ItemChangeTypeId;
                }).DisplayText
                : '';
            elementDetailInfoViewModel.showNextVersionItem = changeTypeDisplayText.indexOf('Added') != 0 && changeTypeDisplayText.indexOf('Deleted') != 0;
            if (!elementDetailInfoViewModel.showNextVersionItem) {
                if (changeTypeDisplayText.indexOf('Added') == 0) {
                    elementDetailInfoViewModel.editedNextVersion.OldSystemItemId = null;
                } else {
                    elementDetailInfoViewModel.editedNextVersion.NewSystemItemId = null;
                }
            }
        }

        elementDetailInfoViewModel.itemDataTypes = Application.Enumerations.ItemDataType;
        elementDetailInfoViewModel.itemChangeTypesPrevious = _.filter(Application.Enumerations.ItemChangeType, function(item) {
            return item.DisplayText.indexOf('Deleted') != 0 && item.DisplayText != '';
        });
        elementDetailInfoViewModel.itemChangeTypesNext = _.filter(Application.Enumerations.ItemChangeType, function(item) {
            return item.DisplayText.indexOf('Added') != 0 && item.DisplayText != '';;
        });

        breadcrumbService.withCurrent();
    }
]);