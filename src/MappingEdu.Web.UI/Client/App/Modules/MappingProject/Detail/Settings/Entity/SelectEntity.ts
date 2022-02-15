// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.detail.settings.select-entity
//

var m = angular.module('app.modules.mapping-project.detail.settings.select-entity', []);

m.controller('app.modules.mapping-project.detail.settings.select-entity', ['$', '$scope', 'repositories', 'services', 'enumerations', 'project', 'hint', 'hints', '$uibModalInstance', 'settings', 
    function ($, $scope, repositories: IRepositories, services: IServices, enumerations: IEnumerations, project, hint, hints, $uibModalInstance: angular.ui.bootstrap.IModalServiceInstance, settings: ISystemSettings) {

        services.logger.debug('Loaded controller app.modules.mapping-project.detail.settings.select-entity');

        var vm = this;

        services.profile.me().then(data => {
            vm.me = data;
        });

        $scope.selectedSourceItems = [];
        $scope.selectedTargetItems = [];

        if (hint) {
            $scope.hint = angular.copy(hint);
            $scope.sourceBrowseElement = $scope.hint.SourceEntity;
            $scope.targetBrowseElement = $scope.hint.TargetEntity;
        } else {
            $scope.hint = {};
        }

        $scope.sourceElementGroups = [];
        repositories.elementGroup.getAll(project.SourceDataStandardId).then((data) => {
            $scope.sourceElementGroups = data;
        });

        $scope.targetElementGroups = [];
        repositories.elementGroup.getAll(project.TargetDataStandardId).then((data) => {
            $scope.targetElementGroups = data;
        });

        $scope.project = project;
        $scope.itemTypeId = 2;
        $scope.active = {
            sourceTab: 'browse',
            targetTab: 'browse'
        };

        $scope.$watch('sourceBrowseElement', (newVal) => {
            if (newVal) $scope.active.sourceTab = 'browse';
        });

        $scope.$watch('targetBrowseElement', (newVal) => {
            if (newVal) $scope.active.targetTab = 'browse';
        });

        $scope.toggleDropdown = isSource => {
            if (isSource || $scope.sourceOpen) {
                $scope.sourceOpen = !$scope.sourceOpen;
                $('#source-entity').toggle(800);
            }

            if (!isSource || $scope.targetOpen) {
                $scope.targetOpen = !$scope.targetOpen;
                $('#target-entity').toggle(800);
            }
        }

        $scope.isDisabled = () => {
            var disabled = false;
            var disabledMessage = 'Need to select ';
            if (!$scope.hint.SourceEntity) {
                disabled = true;
                disabledMessage += 'a source entity ';
            }
            if (!$scope.hint.TargetEntity) {
                if (disabled) disabledMessage += 'and ';
                else disabled = true;

                disabledMessage += 'a target entity';
            }

            $scope.disabledMessage = (disabled) ? disabledMessage : '';
            return disabled;
        }


        $scope.isSelectDisabled = (tab, isSource) => {
            var item;

            if (isSource) {
                if (tab === 'browse') item = $scope.selectedBrowseSourceItem;
                else if (tab === 'search' && $scope.selectedSourceItems) item = $scope.selectedSourceItems[0];
            } else {
                if (tab === 'browse') item = $scope.selectedBrowseTargetItem;
                else if (tab === 'search' && $scope.selectedTargetItems) item = $scope.selectedTargetItems[0];
            }

            return !item || item.ItemTypeId >= 4;
        }

        $scope.setTab = (tab, isSource) => {
            if (isSource) $scope.active.sourceTab = tab;
            else $scope.active.targetTab = tab;
        }

        $scope.selectBrowseElement = (tab, isSource) => {
            var item;

            if (isSource) {
                if (tab === 'browse') item = $scope.selectedBrowseSourceItem;
                else if (tab === 'search' && $scope.selectedSourceItems) item = $scope.selectedSourceItems[0];

                $scope.hint.SourceEntity = angular.copy(item);
            } else {
                if (tab === 'browse') item = $scope.selectedBrowseTargetItem;
                else if (tab === 'search' && $scope.selectedTargetItems) item = $scope.selectedTargetItems[0];

                $scope.hint.TargetEntity = angular.copy(item);
            }

            $scope.toggleDropdown();

        }

        $scope.selected = (tab) => {
            if (tab === 'search') {
                if ($scope.selectedSourceSearchItems && $scope.selectedSourceSearchItems.length >= 2) return `(${$scope.selectedSourceSearchItems.length})`;
            } else if (tab === 'suggest') {
                if ($scope.selectedSourceSearchItems && $scope.selectedSourceSearchItems.length >= 2) return `(${$scope.selectedSourceSearchItems.length})`;
            }
        }

        var modal = {
            backdrop: 'static',
            animation: false,
            keyboard: false,
            templateUrl: `${settings.moduleBaseUri}/mappingProject/detail/settings/entity/EntityHints.tpl.html`,
            controller: 'app.modules.mapping-project.detail.settings.entity-hints',
            windowClass: 'modal-xl',
            resolve: {
                project: () => { return project; },
                hints: () => { return hints }
            }
        }

        $scope.select = () => {
            $scope.hint.SourceEntityId = $scope.hint.SourceEntity.SystemItemId;
            $scope.hint.TargetEntityId = $scope.hint.TargetEntity.SystemItemId;


            if ($scope.hint.EntityHintId) {
                repositories.entity.hint.save(project.MappingProjectId, $scope.hint).then((data) => {
                    services.logger.success('Updated entity hint.');

                    hint.SourceEntityId = data.SourceEntityId;
                    hint.TargetEntityId = data.TargetEntityId;
                    hint.SourceEntity = data.SourceEntity;
                    hint.TargetEntity = data.TargetEntity;

                    $uibModalInstance.dismiss();
                    services.modal.open(modal);

                }, error => {
                    services.logger.error('Error updating entity hint.', error);
                });
            } else {
                repositories.entity.hint.create(project.MappingProjectId, $scope.hint).then((data) => {
                    services.logger.success('Created entity hint.');
                    hints.push(data);

                    $uibModalInstance.dismiss();
                    services.modal.open(modal);

                }, error => {
                    services.logger.error('Error creating entity hint.', error);
                });
            }
        }

        $scope.cancel = (tab) => {

            $uibModalInstance.dismiss();
            services.timeout(() => {
                services.modal.open(modal);
            }, 1);
        }
    }
])