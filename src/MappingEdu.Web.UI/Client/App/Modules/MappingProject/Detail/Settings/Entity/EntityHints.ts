// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.detail.settings.entity-hints
//

var m = angular.module('app.modules.mapping-project.detail.settings.entity-hints', []);

m.controller('app.modules.mapping-project.detail.settings.entity-hints', ['$', '$scope', 'repositories', 'services', 'enumerations', 'project', 'hints', '$uibModalInstance', 'settings',
    function ($, $scope, repositories: IRepositories, services: IServices, enumerations: IEnumerations, project, hints, $uibModalInstance: angular.ui.bootstrap.IModalServiceInstance, settings: ISystemSettings) {

        services.logger.debug('Loaded controller app.modules.mapping-project.detail.settings.select-entity');

        var vm = this;

        //services.profile.me().then(data => { vm.me = data; });

        $scope.hints = hints;

        $scope.dtOptions = services.datatables.optionsBuilder.newOptions();

        $scope.dtColumnDefs = [
            services.datatables.columnDefBuilder.newColumnDef(2).notSortable(),
            services.datatables.columnDefBuilder.newColumnDef(3).notSortable()
        ];


        $scope.delete = (mapping, index) => {
            repositories.entity.hint.remove(project.MappingProjectId, mapping.EntityHintId).then(() => {
                services.logger.success('Deleted entity hint.');
                hints.splice(index, 1);
            }, error => {
                services.logger.error('Error deleting entity hint.', error);
            });
        }


        $scope.entityHintModal = (hint, index) => {
            var modal = {
                backdrop: 'static',
                keyboard: false,
                animation: false,
                templateUrl: `${settings.moduleBaseUri}/mappingProject/detail/settings/entity/selectEntity.tpl.html`,
                controller: 'app.modules.mapping-project.detail.settings.select-entity',
                windowClass: 'modal-xl',
                resolve: {
                    project: () => { return project; },
                    hint: () => { return hint; },
                    hints: () => { return hints; }
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