// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.detail.settings.synonym-form
//

var m = angular.module('app.modules.mapping-project.detail.settings.synonym-form', []);


// ****************************************************************************
// Controller app.modules.mapping-project.detail.settings.synonym-form
//

m.controller('app.modules.mapping-project.detail.settings.synonym-form', ['$scope', '$uibModalInstance', 'repositories', 'services', 'settings', 'synonym', 'synonyms', 'project',
    ($scope, $uibModalInstance, repositories: IRepositories, services: IServices, settings: ISystemSettings, synonym, synonyms, project) => {

    if (synonym) $scope.synonym = angular.copy(synonym);
    else $scope.synonym = {};

    var modal = {
        backdrop: 'static',
        keyboard: false,
        animation: false,
        templateUrl: `${settings.moduleBaseUri}/mappingProject/detail/settings/synonym/synonyms.tpl.html`,
        controller: 'app.modules.mapping-project.detail.settings.synonyms',
        size: 'lg',
        resolve: {
            project: () => { return project; },
            synonyms: () => { return synonyms; }
        }
    }

    $scope.save = () => {
        if ($scope.synonym.MappingProjectSynonymId) {
            repositories.mappingProject.synonym.save(project.MappingProjectId, $scope.synonym).then((data) => {
                services.logger.success('Updated mapping project synonym.');

                synonym.SourceWord = data.SourceWord;
                synonym.TargetWord = data.TargetWord;

                $uibModalInstance.dismiss();
                services.modal.open(modal);

            }, error => {
                services.logger.error('Error updating mapping project synonym.', error);
            });
        } else {
            repositories.mappingProject.synonym.create(project.MappingProjectId, $scope.synonym).then((data) => {
                services.logger.success('Created mapping project synonym.');
                synonyms.push(data);

                $uibModalInstance.dismiss();
                services.modal.open(modal);
            }, error => {
                services.logger.error('Error creating mapping project synonym.', error);
            });
        }
    }

    $scope.close = () => {
        $uibModalInstance.dismiss();
        services.modal.open(modal);
    }

}]);
