// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.detail.settings.template-form
//

var m = angular.module('app.modules.mapping-project.detail.settings.template-form', []);


// ****************************************************************************
// Controller app.modules.mapping-project.detail.settings.template-form
//

m.controller('app.modules.mapping-project.detail.settings.template-form', ['$scope', '$uibModalInstance', 'repositories', 'services', 'settings', 'template', 'templates', 'project',
    ($scope, $uibModalInstance, repositories: IRepositories, services: IServices, settings: ISystemSettings, template, templates, project) => {

    if (template) $scope.template = angular.copy(template);
    else $scope.template = {};

    var modal = {
        backdrop: 'static',
        keyboard: false,
        animation: false,
        templateUrl: `${settings.moduleBaseUri}/mappingProject/detail/settings/template/templates.tpl.html`,
        controller: 'app.modules.mapping-project.detail.settings.templates',
        size: 'lg',
        resolve: {
            project: () => { return project; },
            templates: () => { return templates; }
        }
    }

    $scope.save = () => {
        window.setTimeout(1000);
        if ($scope.template.MappingProjectTemplateId) {
            return repositories.mappingProject.template.save(project.MappingProjectId, $scope.template).then((data) => {
                services.logger.success('Updated mapping project template.');

                template.Title = data.Title;
                template.Template = data.Template;

                $uibModalInstance.dismiss();
                services.modal.open(modal);

            }, error => {
                services.logger.error('Error updating mapping project template.', error);
            });
        } else {
            return repositories.mappingProject.template.create(project.MappingProjectId, $scope.template).then((data) => {
                services.logger.success('Created mapping project template.');
                templates.push(data);

                $uibModalInstance.dismiss();
                services.modal.open(modal);
            }, error => {
                services.logger.error('Error creating mapping project template.', error);
            });
        }
    }

    $scope.getTypedSystemItems = (searchText) => {
        if (searchText.indexOf('.') > -1) return;

        var blTextArea = angular.element('#businessLogic');
        var domainItemPath = null;
        if (!services.underscore.isUndefined(blTextArea[0])) {
            var lastPosition = services.utils.getCaretPosition(blTextArea[0]);
            var firstPosition = null;

            for (var i = lastPosition; i >= 0; i--) {

                if ($scope.template.Template.charAt(i) === ']') break;

                if ($scope.template.Template.charAt(i) === '[') {
                    firstPosition = i + 1;
                    break;
                }
            }

            if (firstPosition == null) return;

            domainItemPath = $scope.template.Template.substr(firstPosition, lastPosition);
        }

        return repositories.element.search(project.TargetDataStandardId, null, searchText, domainItemPath).then(data => {
            $scope.typedSystemItems = [];
            angular.forEach(data, item => {
                item.label = item.ItemName;
                if (item.ItemTypeId == 5) item.label += ' [ENUM]';
                $scope.typedSystemItems.push(item);
            });
            return services.q.when($scope.typedSystemItems);
        });
    }

    $scope.selectTypedItem = (item) => {
        if (item.ItemTypeId === 1) return '[' + item.ItemName + '.';
        else if (item.ItemTypeId < 4) return '.' + item.ItemName + '.';
        else return '.' + item.ItemName + '] ';
    }

    $scope.close = () => {
        $uibModalInstance.dismiss();
        services.modal.open(modal);
    }

}]);
