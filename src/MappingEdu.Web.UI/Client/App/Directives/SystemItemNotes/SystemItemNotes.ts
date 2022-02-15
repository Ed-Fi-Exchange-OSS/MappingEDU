// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.system-item-notes

var m = angular.module('app.directives.system-item-notes', []);

m.directive('maSystemItemNotes', ['settings', (settings: ISystemSettings) => {
    return {
        restrict: 'E',
        templateUrl: `${settings.directiveBaseUri}/SystemItemNotes/SystemItemNotes.tpl.html`,
        controller: 'app.directives.system-item-notes',
        scope: {
            systemItemId: '=',
            systemItemMapId: '=?',
            mappingProjectId: '=',
            standardId: '=',
            notes: '=',
            readOnly: '=?'
        }
    }
}]);

m.controller('app.directives.system-item-notes', ['$scope', 'services', 'repositories', ($scope, services: IServices, repositories: IRepositories) => {

    //Get Current User
    services.profile.me().then(me => {
        $scope.me = me;

        if (!$scope.readOnly) {
            //Get Taggable Users
            if ($scope.mappingProjectId) {
                services.profile.mappingProjectAccess($scope.mappingProjectId).then((data) => {
                    $scope.readOnly = (data.Role < 2 && !me.IsAdministrator);

                    if (!$scope.readOnly) {
                        repositories.mappingProject.getTaggableUsers($scope.mappingProjectId).then(users => {
                            angular.forEach(users, user => {
                                user.label = user.FirstName + ' ' + user.LastName;
                            });
                            $scope.users = users;
                        });
                    } 
                }); 
            } else {
                services.profile.dataStandardAccess($scope.standardId).then((data) => {
                    $scope.readOnly = (data.Role < 2 && !me.IsAdministrator);

                    if (!$scope.readOnly) {
                        repositories.dataStandard.getTaggableUsers($scope.standardId).then((users: any) => {
                            angular.forEach(users, user => {
                                user.label = user.FirstName + ' ' + user.LastName;
                            });
                            $scope.users = users;
                        });
                    }
                }); 
            }
        }

    });

    $scope.userSelect = (user) => {
        return `[~${user.label}]`;
    }

    $scope.save = (note) => {

        if (note.Deleted) return;
        if (!$scope.adding && note.Index !== $scope.editIndex) return;

        $scope.editIndex = null;
        $scope.adding = false;

        if (note.EditNotes === note.Notes) {
            services.timeout(() => $scope.$apply());
            return;
        }

        note.Notes = note.EditNotes;

        if ($scope.mappingProjectId) {
            if (note.MapNoteId) { saveMapNote(note);
            } else addMapNote(note);
        } else {
            if (note.NoteId) saveNote(note);
            else addNote(note);
        }
    }

    $scope.cancel = (note) => {
        $scope.editIndex = null;
        $scope.adding = false;
        note.EditNotes = null;
    }

    $scope.add = () => {
        $scope.adding = true;
        $scope.editIndex = null;

        if ($scope.systemItemMapId) $scope.addNote = { SystemItemMapId: $scope.systemItemMapId };
        else  $scope.addNote = { SystemItemId: $scope.systemItemId };

        $scope.addNote.CreateDate = new Date();
        $scope.addNote.CreateBy = $scope.me.FirstName + ' ' + $scope.me.LastName;
        $scope.addNote.CreateById = $scope.me.Id;
    }

    $scope.edit = (note, index, event) => {
        if (event.target.tagName === 'A') return; //Don't Open Edit if a link is clicked
        if (note.CreateById !== $scope.me.Id) return;

        if ($scope.readOnly) return;
        if ($scope.editIndex != null && $scope.editIndex != undefined) $scope.save($scope.notes[$scope.editIndex]);

        note.Index = index;
        note.EditNotes = angular.copy(note.Notes);
        $scope.editIndex = index;
    }

    $scope.delete = (note, index) => {
        $scope.editIndex = null;
        note.Deleted = true;

        if ($scope.systemItemMapId) deleteMapNote(note, index);
        else deleteNote(note, index);
    }

    $scope.viewNotes = (note) => {
        if (!note.Notes)
            return '';

        var html = angular.copy(note.Notes);
        html = html.split('[~').join('<b>@');
        html = html.split(']').join('</b>');
        html = html.split('\n').join('<br/>');

        return html;
    }
    
    function addMapNote(note: any) {

        if (!note.Notes || note.Notes === '') {
            $scope.addNote = null;
            $scope.adding = false;
            return;
        }

        services.loading.start(`saving-note-add`);
        repositories.element.mapNote.create($scope.systemItemMapId, note).then(data => {
            services.logger.success('Sucessfully created map note');
            note.MapNoteId = data.MapNoteId;
            $scope.notes.push(angular.copy(note));
            $scope.addNote = null;
            $scope.adding = false;
        }, error => {
            services.logger.error('Error created map note', error);
        }).finally(() => {
            services.timeout(() => { services.loading.finish(`saving-note-add`) }, 200);
        });
    }

    function saveMapNote(note: any) {
        services.loading.start(`saving-note${note.Index}`);
        repositories.element.mapNote.save($scope.systemItemMapId, note.MapNoteId, note).then(data => {
            note.IsEdited = true;
            services.logger.success('Sucessfully saved map note');
        }, error => {
            services.logger.error('Error saving map note', error);
            $scope.editIndex = note.Index;
        }).finally(() => {
            services.timeout(() => { services.loading.finish(`saving-note${note.Index}`) }, 200);
        });
    }

    function deleteMapNote(note: any, index) {
        repositories.element.mapNote.remove($scope.systemItemMapId, note.MapNoteId)
            .then(() => {
                services.logger.success('Removed map note.');
                note.Deleted = true;
                $scope.notes.splice(index, 1);
                $scope.editIndex = null;
            }, error => {
                services.logger.error('Error remcoving map note.', error.data);
            });
    }

    function addNote(note: any) {

        if (!note.Notes || note.Notes === '') {
            $scope.addNote = null;
            $scope.adding = false;
            return;
        }

        services.loading.start(`saving-note-add`);
        repositories.element.note.create($scope.systemItemId, note).then(data => {
            services.logger.success('Sucessfully created note');
            note.NoteId = data.NoteId;
            $scope.notes.push(angular.copy(note));
            $scope.addNote = null;
            $scope.adding = false;
        }, error => {
            services.logger.error('Error created note', error);
            $scope.editIndex = null;
        }).finally(() => {
            services.timeout(() => { services.loading.finish(`saving-note-add`) }, 200);
        });
    }

    function saveNote(note: any) {
        repositories.element.note.save($scope.systemItemId, note.NoteId, note).then(data => {
            note.IsEdited = true;
            services.logger.success('Sucessfully saved note');
        }, error => {
            services.logger.error('Error saving note', error);
            $scope.editIndex = note.Index;
        }).finally(() => {
            services.timeout(() => { services.loading.finish(`saving-note${note.Index}`) }, 200);
        });
    }

    function deleteNote(note: any, index) {
        repositories.element.note.remove($scope.systemItemId, note.NoteId)
            .then(() => {
                note.Deleted = true;
                services.logger.success('Removed note.');
                $scope.notes.splice(index, 1);
                $scope.editIndex = null;
            }, error => {
                services.logger.error('Error removing note.', error.data);
            });
    }
}]);
