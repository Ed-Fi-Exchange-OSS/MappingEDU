// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appEntityDetail').controller('entityDetailInfoController', [
    '_', '$', '$compile', '$scope', '$timeout', '$state', '$stateParams', 'entityService', 'elementService', 'elementDetailService', 'elementListService', 'nextDataStandardService',
    'previousVersionDeltaService', 'nextVersionDeltaService', 'noteService', 'newSystemItemService', 'systemItemEnumerationService', 'handleErrorService', 'sessionService', 'breadcrumbService',
    function (_, $, $compile, $scope, $timeout, $state, $stateParams, entityService, elementService, elementDetailService, elementListService, nextDataStandardService,
        previousVersionDeltaService, nextVersionDeltaService, noteService, newSystemItemService, systemItemEnumerationService, handleErrorService, sessionService, breadcrumbService) {
        var entityDetailInfoViewModel = this;
        var emptyGuid = '{00000000-0000-0000-0000-000000000000}';

        entityDetailInfoViewModel.mappingProjectId = $stateParams.mappingProjectId || emptyGuid;
        entityDetailInfoViewModel.dataStandardId = $stateParams.dataStandardId || emptyGuid;
        entityDetailInfoViewModel.showMapping = entityDetailInfoViewModel.mappingProjectId != emptyGuid;
        entityDetailInfoViewModel.showNextVersionItem = true;

        entityDetailInfoViewModel.elementListPreviousLoaded = false;
        entityDetailInfoViewModel.elementListNextLoaded = false;

        entityDetailInfoViewModel.entity = $scope.$parent.entityDetailViewModel.entity;

        if (!entityDetailInfoViewModel.showMapping)
            $timeout(function() {
                document.title = 'Data Standard Entity Details - MappingEDU';
            }, 100);

        entityDetailInfoViewModel.id = $stateParams.id;

        $scope.$on('entity-fetched', function(event, data) {
            entityDetailInfoViewModel.entity = data;
        });

        entityDetailInfoViewModel.emitEntityChangedEvent = function() {
            $scope.$emit('entity-changed');
        };

        entityDetailInfoViewModel.getListNext = function() {
            entityDetailInfoViewModel.loading = true;
            elementListService.get(entityDetailInfoViewModel.nextDataStandard.DataStandardId)
                .then(function(data) {
                    entityDetailInfoViewModel.systemItemsNext = [];
                    for (var i = 0; i < data.Elements.length; i++) {
                        var eg = data.Elements[i];
                        if (!eg.PathSegments)
                            eg.PathSegments = [];
                        if (eg.PathSegments.length > 0) {
                            var systemItemId = eg.PathSegments[eg.PathSegments.length - 1].SystemItemId;
                            var ids = entityDetailInfoViewModel.systemItemsNext.filter(function(val) {
                                return val.Id === systemItemId;
                            });
                            if (ids.length == 0) {
                                entityDetailInfoViewModel.systemItemsNext.push({
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
                        entityDetailInfoViewModel.systemItemsNext.push({ Id: eg.Element.SystemItemId, Type: eg.Element.ItemTypeName, Segments: elementSegments });
                    }

                    if ($.fn.dataTable.isDataTable('#elementListTableNext')) {
                        var table = $('#elementListTableNext').DataTable();
                        table.clear().rows.add(entityDetailInfoViewModel.systemItemsNext).draw();
                    } else {

                        $('#elementListTableNext').DataTable(
                        {
                            data: entityDetailInfoViewModel.systemItemsNext,
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
                                        html += '<ma-element-path link-path="false" context-id="entityDetailInfoViewModel.id"';
                                        html += ' segments="entityDetailInfoViewModel.systemItemsNext[';
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
                                        return '<a role="button" class="btn btn-happy" ng-click="entityDetailInfoViewModel.selectItemNext(entityDetailInfoViewModel.systemItemsNext['
                                            + meta.row
                                            + '].Segments)" data-toggle="collapse" data-target="#elementListContainerNext"><i class="fa"></i></a>';
                                    }
                                }
                            ]
                        });
                    }
                    entityDetailInfoViewModel.elementListNextLoaded = true;
                })
                .catch(function(error) {
                    console.log(error);
                });
        }

        entityDetailInfoViewModel.getListPrevious = function() {
            entityDetailInfoViewModel.loading = true;
            elementListService.get($scope.dataStandard.PreviousDataStandardId)
                .then(function(data) {
                    entityDetailInfoViewModel.systemItemsPrevious = [];
                    for (var i = 0; i < data.Elements.length; i++) {
                        var eg = data.Elements[i];
                        if (!eg.PathSegments)
                            eg.PathSegments = [];
                        if (eg.PathSegments.length > 0) {
                            var systemItemId = eg.PathSegments[eg.PathSegments.length - 1].SystemItemId;
                            var ids = entityDetailInfoViewModel.systemItemsPrevious.filter(function(val) {
                                return val.Id === systemItemId;
                            });
                            if (ids.length == 0) {
                                entityDetailInfoViewModel.systemItemsPrevious.push({
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
                        entityDetailInfoViewModel.systemItemsPrevious.push({ Id: eg.Element.SystemItemId, Type: eg.Element.ItemTypeName, Segments: elementSegments });
                    }

                    if ($.fn.dataTable.isDataTable('#elementListTablePrevious')) {
                        var table = $('#elementListTablePrevious').DataTable();
                        table.clear().rows.add(entityDetailInfoViewModel.systemItemsPrevious).draw();
                    } else {

                        $('#elementListTablePrevious').DataTable(
                        {
                            data: entityDetailInfoViewModel.systemItemsPrevious,
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
                                        html += '<ma-element-path link-path="false" context-id="entityDetailInfoViewModel.id"';
                                        html += ' segments="entityDetailInfoViewModel.systemItemsPrevious[';
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
                                        return '<a role="button" class="btn btn-happy" ng-click="entityDetailInfoViewModel.selectItemPrevious(entityDetailInfoViewModel.systemItemsPrevious['
                                            + meta.row
                                            + '].Segments)" data-toggle="collapse" data-target="#elementListContainerPrevious"><i class="fa"></i></a>';
                                    }
                                }
                            ]
                        });
                    }
                    entityDetailInfoViewModel.elementListPreviousLoaded = true;
                })
                .catch(function(error) {
                    console.log(error);
                });
        }


        $('#previousVersionModal').on('shown.bs.modal', function(e) {
            if (!$.fn.dataTable.isDataTable('#elementListTablePrevious'))
                entityDetailInfoViewModel.getListPrevious();
        });

        $('#nextVersionModal').on('shown.bs.modal', function(e) {
            if (!$.fn.dataTable.isDataTable('#elementListTableNext'))
                entityDetailInfoViewModel.getListNext();
        });

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


        if (entityDetailInfoViewModel.dataStandardId != emptyGuid) {
            entityDetailInfoViewModel.getNextDataStandard = function() {
                nextDataStandardService.get(entityDetailInfoViewModel.dataStandardId)
                    .then(function(data) {
                        if (data) {
                            entityDetailInfoViewModel.nextDataStandard = data;
                        }
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, entityDetailInfoViewModel);
                    });
            }
            entityDetailInfoViewModel.getNextDataStandard();

            entityDetailInfoViewModel.getSystemItemEnumerations = function() {
                systemItemEnumerationService.get(entityDetailInfoViewModel.dataStandardId)
                    .then(function(data) {
                        entityDetailInfoViewModel.systemItemEnumerations = data;
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, entityDetailInfoViewModel);
                    });
            }
            entityDetailInfoViewModel.getSystemItemEnumerations();
        }

        entityDetailInfoViewModel.showElements = true;
        entityDetailInfoViewModel.toggleElementsCaret = function() {
            entityDetailInfoViewModel.showElements = !entityDetailInfoViewModel.showElements;
        }

        entityDetailInfoViewModel.showPreviousVersions = false;
        entityDetailInfoViewModel.togglePreviousVersionsCaret = function() {
            entityDetailInfoViewModel.showPreviousVersions = !entityDetailInfoViewModel.showPreviousVersions;
        }

        entityDetailInfoViewModel.showNextVersions = false;
        entityDetailInfoViewModel.toggleNextVersionsCaret = function() {
            entityDetailInfoViewModel.showNextVersions = !entityDetailInfoViewModel.showNextVersions;
        }

        entityDetailInfoViewModel.showNotes = false;
        entityDetailInfoViewModel.toggleNotesCaret = function() {
            entityDetailInfoViewModel.showNotes = !entityDetailInfoViewModel.showNotes;
        };

        $('#elementsTable').on('page.dt', function() {
            var table = $(this).DataTable();
            var info = table.page.info();
            sessionService.cloneToSession('entityPage', entityDetailInfoViewModel.id, info.page);
        });

        $('#elementsTable').on('init.dt', function() {
            var entityPage = sessionService.cloneFromSession('entityPage', entityDetailInfoViewModel.id);
            if (entityPage) {
                var table = $(this).DataTable();
                table.page(entityPage).draw(false);
            }
        });

        entityDetailInfoViewModel.edit = function(element) {
            entityDetailInfoViewModel.editElement = element;
            entityDetailInfoViewModel.editElement.ElementDetails.ItemDataTypeId = element.ElementDetails.ItemDataType.Id;
            entityDetailInfoViewModel.editingAnElement = (entityDetailInfoViewModel.editElement.ElementDetails.ItemType.Name == 'Element');
            entityDetailInfoViewModel.addingAnElement = false;
        }

        entityDetailInfoViewModel.saveElementDetail = function() {
            var element = entityDetailInfoViewModel.editElement;
            $scope.$broadcast('show-errors-check-valid');

            if (entityDetailInfoViewModel.elementForm.$invalid)
                return;

            if (element.ElementDetails.SystemItemId) {
                elementDetailService.update(element.ElementDetails.SystemItemId, element.ElementDetails)
                    .then(function(data) {
                        $("#elementModal").modal('hide');
                        entityDetailInfoViewModel.emitEntityChangedEvent();
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, entityDetailInfoViewModel);
                    });
            } else {
                element.ElementDetails.ParentSystemItemId = entityDetailInfoViewModel.id;
                newSystemItemService.add(element.ElementDetails)
                    .then(function(data) {
                        $("#elementModal").modal('hide');
                        entityDetailInfoViewModel.emitEntityChangedEvent();
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, entityDetailInfoViewModel);
                    });
            }
        };

        entityDetailInfoViewModel.cancelElementDetail = function() {
            $scope.$broadcast('show-errors-reset');
            $("#elementModal").modal('hide');
            entityDetailInfoViewModel.emitEntityChangedEvent();
            entityDetailInfoViewModel.editElement = {};
            entityDetailInfoViewModel.editElement.ElementDetails = {};
            entityDetailInfoViewModel.editElement.ElementDetails.ItemName = '';
            entityDetailInfoViewModel.editElement.ElementDetails.TechnicalName = '';
            entityDetailInfoViewModel.editElement.ElementDetails.ItemDataTypeId = null;
            entityDetailInfoViewModel.editElement.ElementDetails.FieldLength = '';
            entityDetailInfoViewModel.editElement.ElementDetails.DataTypeSource = '';
            entityDetailInfoViewModel.editElement.ElementDetails.ItemUrl = '';
        };

        entityDetailInfoViewModel.delete = function(element) {
            elementService.delete(element.ElementDetails.SystemItemId)
                .then(function() {
                    entityDetailInfoViewModel.emitEntityChangedEvent();
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, entityDetailInfoViewModel);
                });
        }

        entityDetailInfoViewModel.addElement = function() {
            entityDetailInfoViewModel.editElement = {};
            entityDetailInfoViewModel.editElement.ElementDetails = {};
            entityDetailInfoViewModel.editElement.ElementDetails.ItemTypeName = 'Element';
            entityDetailInfoViewModel.editElement.ElementDetails.ItemDataTypeId = null;
            entityDetailInfoViewModel.editingAnElement = false;
            entityDetailInfoViewModel.addingAnElement = true;
        }

        entityDetailInfoViewModel.editNote = function(note) {
            if (!note.editNote)
                note.editNote = angular.copy(note);
        };

        entityDetailInfoViewModel.saveNote = function(note) {
            noteService.update(entityDetailInfoViewModel.entity.SystemItemId, note.NoteId, note.editNote)
                .then(function(savedNote) {
                    angular.extend(note, savedNote);
                    note.editNote = undefined;
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, entityDetailInfoViewModel);
                });
        };

        entityDetailInfoViewModel.cancelEditNote = function(note) {
            note.editNote = undefined;
        };

        entityDetailInfoViewModel.deleteNote = function(note) {
            noteService.delete(entityDetailInfoViewModel.entity.SystemItemId, note.NoteId)
                .then(function() {
                    entityDetailInfoViewModel.entity.Notes.pop(note);
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, entityDetailInfoViewModel);
                });
        };

        entityDetailInfoViewModel.addNote = function() {
            entityDetailInfoViewModel.noteToAdd = { Title: '', Notes: '' };
        };

        entityDetailInfoViewModel.saveAddedNote = function() {
            noteService.add(entityDetailInfoViewModel.entity.SystemItemId, entityDetailInfoViewModel.noteToAdd)
                .then(function(addedNote) {
                    entityDetailInfoViewModel.entity.Notes.push(addedNote);
                    entityDetailInfoViewModel.noteToAdd = undefined;
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, entityDetailInfoViewModel);
                });
        };

        entityDetailInfoViewModel.cancelAddNote = function() {
            entityDetailInfoViewModel.noteToAdd = undefined;
        }

        entityDetailInfoViewModel.addPreviousVersion = function() {
            entityDetailInfoViewModel.editedPreviousVersion = {};
            entityDetailInfoViewModel.editedPreviousVersion.ItemChangeTypeId = 0;
            entityDetailInfoViewModel.determinePreviousVersionItemDisplay();
        }

        entityDetailInfoViewModel.editPreviousVersion = function(version) {
            entityDetailInfoViewModel.editedPreviousVersion = angular.copy(version);
            entityDetailInfoViewModel.editedPreviousVersion.ItemChangeTypeId = version.ItemChangeType.Id;
            entityDetailInfoViewModel.determinePreviousVersionItemDisplay();
        }

        entityDetailInfoViewModel.savePreviousVersion = function() {
            $scope.$broadcast('show-errors-reset');
            $scope.$broadcast('show-errors-check-valid');
            if (entityDetailInfoViewModel.editedPreviousVersion.PreviousVersionId) {
                previousVersionDeltaService.update(entityDetailInfoViewModel.id, entityDetailInfoViewModel.editedPreviousVersion.PreviousVersionId, entityDetailInfoViewModel.editedPreviousVersion)
                    .then(function(data) {
                        entityDetailInfoViewModel.emitEntityChangedEvent();
                        angular.element("#previousVersionModal").modal('hide');
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, entityDetailInfoViewModel);
                    });
            } else {
                previousVersionDeltaService.add(entityDetailInfoViewModel.id, entityDetailInfoViewModel.editedPreviousVersion)
                    .then(function(data) {
                        entityDetailInfoViewModel.emitEntityChangedEvent();
                        angular.element("#previousVersionModal").modal('hide');
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, entityDetailInfoViewModel);
                    });
            };
        }

        entityDetailInfoViewModel.cancelPreviousVersion = function() {
            $scope.$broadcast('show-errors-reset');
            entityDetailInfoViewModel.element = angular.copy(entityDetailInfoViewModel.oldElement);
            angular.element("#previousVersionModal").modal('hide');
        }

        entityDetailInfoViewModel.deletePreviousVersion = function(version) {
            previousVersionDeltaService.delete(entityDetailInfoViewModel.id, version.PreviousVersionId)
                .then(function() {
                    entityDetailInfoViewModel.emitEntityChangedEvent();
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, entityDetailInfoViewModel);
                });
        }

        entityDetailInfoViewModel.selectItemPrevious = function(segments) {
            entityDetailInfoViewModel.editedPreviousVersion.OldSystemItemId = segments[segments.length - 1].SystemItemId;
            entityDetailInfoViewModel.editedPreviousVersion.PreviousVersionItems = segments;
        }

        entityDetailInfoViewModel.clearSelectedItemPrevious = function() {
            entityDetailInfoViewModel.editedPreviousVersion.OldSystemItemId = null;
            entityDetailInfoViewModel.editedPreviousVersion.PreviousVersionItems = [];
        }

        entityDetailInfoViewModel.determinePreviousVersionItemDisplay = function() {
            var changeTypeDisplayText = entityDetailInfoViewModel.editedPreviousVersion.ItemChangeTypeId ? _.find(Application.Enumerations.ItemChangeType, function(item) {
                    return item.Id == entityDetailInfoViewModel.editedPreviousVersion.ItemChangeTypeId;
                }).DisplayText
                : '';
            entityDetailInfoViewModel.showPreviousVersionItem = changeTypeDisplayText.indexOf('Added') != 0 && changeTypeDisplayText.indexOf('Deleted') != 0;
            if (!entityDetailInfoViewModel.showPreviousVersionItem) {
                if (changeTypeDisplayText.indexOf('Added') == 0) {
                    entityDetailInfoViewModel.editedPreviousVersion.OldSystemItemId = null;
                } else {
                    entityDetailInfoViewModel.editedPreviousVersion.NewSystemItemId = null;
                }
            }
        }

        entityDetailInfoViewModel.addNextVersion = function() {
            entityDetailInfoViewModel.editedNextVersion = {};
            entityDetailInfoViewModel.editedNextVersion.ItemChangeTypeId = 0;
            entityDetailInfoViewModel.determineNextVersionItemDisplay();
        }

        entityDetailInfoViewModel.editNextVersion = function(version) {
            entityDetailInfoViewModel.editedNextVersion = angular.copy(version);
            entityDetailInfoViewModel.editedNextVersion.ItemChangeTypeId = version.ItemChangeType.Id;
            entityDetailInfoViewModel.determineNextVersionItemDisplay();
        }

        entityDetailInfoViewModel.saveNextVersion = function() {
            $scope.$broadcast('show-errors-reset');
            $scope.$broadcast('show-errors-check-valid');
            if (entityDetailInfoViewModel.editedNextVersion.NextVersionId) {
                nextVersionDeltaService.update(entityDetailInfoViewModel.id, entityDetailInfoViewModel.editedNextVersion.NextVersionId, entityDetailInfoViewModel.editedNextVersion)
                    .then(function(data) {
                        entityDetailInfoViewModel.emitEntityChangedEvent();
                        angular.element("#nextVersionModal").modal('hide');
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, entityDetailInfoViewModel);
                    });
            } else {
                nextVersionDeltaService.add(entityDetailInfoViewModel.id, entityDetailInfoViewModel.editedNextVersion)
                    .then(function(data) {
                        entityDetailInfoViewModel.emitEntityChangedEvent();
                        angular.element("#nextVersionModal").modal('hide');
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, entityDetailInfoViewModel);
                    });
            };
        }

        entityDetailInfoViewModel.cancelNextVersion = function() {
            $scope.$broadcast('show-errors-reset');
            entityDetailInfoViewModel.element = angular.copy(entityDetailInfoViewModel.oldElement);
            angular.element("#nextVersionModal").modal('hide');
        }

        entityDetailInfoViewModel.deleteNextVersion = function(version) {
            nextVersionDeltaService.delete(entityDetailInfoViewModel.id, version.NextVersionId)
                .then(function() {
                    entityDetailInfoViewModel.emitEntityChangedEvent();
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, entityDetailInfoViewModel);
                });
        }

        entityDetailInfoViewModel.selectItemNext = function(segments) {
            entityDetailInfoViewModel.editedNextVersion.NewSystemItemId = segments[segments.length - 1].SystemItemId;
            entityDetailInfoViewModel.editedNextVersion.NextVersionItems = segments;
        }

        entityDetailInfoViewModel.clearSelectedItemNext = function() {
            entityDetailInfoViewModel.editedNextVersion.NewSystemItemId = null;
            entityDetailInfoViewModel.editedNextVersion.NextVersionItems = [];
        }

        entityDetailInfoViewModel.determineNextVersionItemDisplay = function() {
            var changeTypeDisplayText = entityDetailInfoViewModel.editedNextVersion.ItemChangeTypeId ? _.find(Application.Enumerations.ItemChangeType, function(item) {
                    return item.Id == entityDetailInfoViewModel.editedNextVersion.ItemChangeTypeId;
                }).DisplayText
                : '';
            entityDetailInfoViewModel.showNextVersionItem = changeTypeDisplayText.indexOf('Added') != 0 && changeTypeDisplayText.indexOf('Deleted') != 0;
            if (!entityDetailInfoViewModel.showNextVersionItem) {
                if (changeTypeDisplayText.indexOf('Added') == 0) {
                    entityDetailInfoViewModel.editedNextVersion.OldSystemItemId = null;
                } else {
                    entityDetailInfoViewModel.editedNextVersion.NewSystemItemId = null;
                }
            }
        }

        entityDetailInfoViewModel.itemDataTypes = Application.Enumerations.ItemDataType;
        entityDetailInfoViewModel.itemChangeTypesPrevious = _.filter(Application.Enumerations.ItemChangeType, function(item) {
            return item.DisplayText.indexOf('Deleted') != 0 && item.DisplayText != '';
        });
        entityDetailInfoViewModel.itemChangeTypesNext = _.filter(Application.Enumerations.ItemChangeType, function(item) {
            return item.DisplayText.indexOf('Added') != 0 && item.DisplayText != '';;
        });

        breadcrumbService.withCurrent();
    }
]);