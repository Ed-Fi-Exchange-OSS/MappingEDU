// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module 'app.modules.element.detail.mapping.project.workflow-status'
//

var m = angular.module('app.modules.element.detail.mapping.project.workflow-status', []);


// ****************************************************************************
// Directive mapping-workflow-status
//

m.directive('mappingWorkflowStatus', ['settings', (settings: ISystemSettings) => {
        return {
            restrict: 'E',
            scope: {
                mapping: '=',
                listMode: '=',
                mappingProjectId: '=',
                readOnly: '='
            },
            templateUrl: `${settings.moduleBaseUri}/Element/Detail/Mapping/Project/WorkflowStatus/WorkflowStatus.tpl.html`,
            controller: 'app.modules.element.detail.mapping.project.workflow-status',
            controllerAs: 'vm'
        }
    }
]);

m.controller('app.modules.element.detail.mapping.project.workflow-status', ['$scope', 'repositories', 'services', function ($scope, repositories: IRepositories, services: IServices) {

    services.logger.debug('Loaded app.modules.element.detail.mapping.project.workflow-status');

    var vm = this;
    vm.editMode = false;
    vm.mapping = $scope.mapping;

    if (!$scope.readOnly && !$scope.listMode && $scope.mappingProjectId) {
        repositories.mappingProject.getTaggableUsers($scope.mappingProjectId).then(users => {
            angular.forEach(users, user => {
                user.label = user.FirstName + ' ' + user.LastName;
            });
            $scope.users = users;
        });

        $scope.userSelect = (user) => {
            return `[~${user.label}]`;
        }
    }

    vm.editNote = (event) => {
        if (!$scope.readOnly) {
            if (event.target.tagName === 'A') return; //Don't Open Edit if a link is clicked

            vm.editMode = true;
            vm.statusNote = angular.copy(vm.mapping.StatusNote);
        }
    }

    vm.cancelNote = () => {
        vm.editMode = false;
        vm.statusNote = null;
    }

    vm.updateNote = () => {
        if (vm.mapping.StatusNote === vm.statusNote) {
            vm.cancelNote();
            services.timeout(() => $scope.$apply());
        } else {
            var mapping = vm.mapping;
            var oldNote = angular.copy(mapping.StatusNote);
            mapping.StatusNote = vm.statusNote;

            repositories.element.workflowStatus.save(mapping.SourceSystemItemId, mapping.SystemItemMapId, mapping).then(() => {
                services.logger.success('Saved status note.');
                vm.cancelNote();
            }, error => {
                services.logger.error('Error saving status note.', error.data);
                mapping.StatusNote = oldNote;
            });
        }
    }

    vm.viewStatusNote = (note) => {
        if (!note) return '';

        var html = angular.copy(note);
        html = html.split('[~').join('<b>@');
        html = html.split(']').join('</b>');
        html = html.split('\n').join('<br/>');

        return html;
    }

}])