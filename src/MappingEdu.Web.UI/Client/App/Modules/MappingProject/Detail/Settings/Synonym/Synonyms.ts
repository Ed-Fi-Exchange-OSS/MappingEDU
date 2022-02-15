// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.detail.settings.synonyms
//

var m = angular.module('app.modules.mapping-project.detail.settings.synonyms', []);

m.controller('app.modules.mapping-project.detail.settings.synonyms', ['$scope', 'repositories', 'services', 'project', 'synonyms', '$uibModalInstance', 'settings',
    function ($scope, repositories: IRepositories, services: IServices, project, synonyms, $uibModalInstance: angular.ui.bootstrap.IModalServiceInstance, settings: ISystemSettings) {

        services.logger.debug('Loaded controller app.modules.mapping-project.detail.settings.synonyms');

        var vm = this;

        //services.profile.me().then(data => { vm.me = data; });

        $scope.synonyms = synonyms;

        $scope.dtOptions = services.datatables.optionsBuilder.newOptions();

        $scope.dtColumnDefs = [
            services.datatables.columnDefBuilder.newColumnDef(2).notSortable(),
            services.datatables.columnDefBuilder.newColumnDef(3).notSortable()
        ];


        $scope.delete = (synonym, index) => {
            repositories.mappingProject.synonym.remove(project.MappingProjectId, synonym.MappingProjectSynonymId).then(() => {
                services.logger.success('Deleted mapping project synonym.');
                $scope.synonyms.splice(index, 1);
            }, error => {
                services.logger.error('Error deleting mapping project synonym.', error);
            });
        }


        $scope.synonymModal = (synonym, index) => {
            var modal = {
                backdrop: 'static',
                keyboard: false,
                animation: false,
                templateUrl: `${settings.moduleBaseUri}/mappingProject/detail/settings/synonym/synonymFormView.tpl.html`,
                controller: 'app.modules.mapping-project.detail.settings.synonym-form',
                size: 'lg',
                resolve: {
                    project: () => { return project; },
                    synonym: () => { return synonym; },
                    synonyms: () => { return synonyms; }
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