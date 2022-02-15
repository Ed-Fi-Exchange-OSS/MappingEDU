// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.detail.settings.templates
//

var m = angular.module('app.modules.mapping-project.detail.settings.templates', []);

m.controller('app.modules.mapping-project.detail.settings.templates', ['$scope', 'repositories', 'services', 'project', 'templates', '$uibModalInstance', 'settings',
    function ($scope, repositories: IRepositories, services: IServices, project, templates, $uibModalInstance: angular.ui.bootstrap.IModalServiceInstance, settings: ISystemSettings) {

        services.logger.debug('Loaded controller app.modules.mapping-project.detail.settings.templates');

        var vm = this;

        //services.profile.me().then(data => { vm.me = data; });

        $scope.templates = templates;

        $scope.dtOptions = services.datatables.optionsBuilder.newOptions();

        $scope.dtColumnDefs = [
            services.datatables.columnDefBuilder.newColumnDef(2).notSortable(),
            services.datatables.columnDefBuilder.newColumnDef(3).notSortable()
        ];


        $scope.delete = (template, index) => {
            repositories.mappingProject.template.remove(project.MappingProjectId, template.MappingProjectTemplateId).then(() => {
                services.logger.success('Deleted mapping project template.');
                $scope.templates.splice(index, 1);
            }, error => {
                services.logger.error('Error deleting mapping project template.', error);
            });
        }


        $scope.templateModal = (template, index) => {
            var modal = {
                backdrop: 'static',
                keyboard: false,
                animation: false,
                templateUrl: `${settings.moduleBaseUri}/mappingProject/detail/settings/template/templateFormView.tpl.html`,
                controller: 'app.modules.mapping-project.detail.settings.template-form',
                size: 'lg',
                resolve: {
                    project: () => { return project; },
                    template: () => { return template; },
                    templates: () => { return templates; }
                }
            }

            $uibModalInstance.dismiss();
            services.modal.open(modal);
        }

        $scope.cancel = (tab) => {
            $uibModalInstance.dismiss();
        }
    }
])