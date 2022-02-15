// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.element.matchmaker
//

var m = angular.module('app.modules.element.matchmaker', [
    'app.modules.element.matchmaker.search',
    'app.modules.element.matchmaker.suggest'
]);

m.controller('app.modules.element.matchmaker', ['$scope', 'repositories', 'services', 'project', 'element', 'hint', '$uibModalInstance',
    function ($scope, repositories: IRepositories, services: IServices, project, element, hint, $uibModalInstance: angular.ui.bootstrap.IModalServiceInstance) {

        services.logger.debug('Loaded controller app.modals.matchmaker');

        var vm = this;

        services.profile.me().then(data => {
            vm.me = data;
        });

        $scope.elementGroups = [];
        repositories.elementGroup.getAll(project.TargetDataStandardId).then((data) => {
            $scope.elementGroups = data;
        });

        if (element.ItemTypeId === 4) {
            $scope.entities = [];
            repositories.entity.getFirstLevelEntities(project.TargetDataStandardId).then((data) => {
                $scope.entities = data;
            });
        }

        $scope.project = project;
        $scope.element = element;

        if (hint && hint.TargetEntity.DomainItemPath) {
            services.session.cloneToSession('BrowseElement', project.MappingProjectId, hint.TargetEntity);
            var searchFilter = services.session.cloneFromSession('MatchmakerSearchFilter', project.MappingProjectId);
            if (!searchFilter || searchFilter.searchText !== hint.TargetEntity.DomainItemPath) {
                searchFilter = {
                    ElementGroups: {},
                    Entities: {},
                    ItemDataTypes: {},
                    SearchText: hint.TargetEntity.DomainItemPath + '.',
                    Length: (searchFilter && searchFilter.Length) ? searchFilter.Length : 10,
                    Start: 0,
                    Order: (searchFilter && searchFilter.Order) ? searchFilter.Order : { column: 0, dir: 'asc' },
                    IsExtended: null
                }

                services.session.cloneToSession('MatchmakerSearchFilter', project.MappingProjectId, searchFilter);
            }
        }

        $scope.browseElement = services.session.cloneFromSession('BrowseElement', project.MappingProjectId);

        // For Perfect Scrollbar. Browse needs to have been visible at least once.
        var visitedBrowse = false;

        services.timeout(() => {
            var tab = services.session.cloneFromSession('MappingTab', project.MappingProjectId);
            $scope.active = { tab: tab ? tab : 'search' };
            if (tab === 'browse') visitedBrowse = true;
        }, 10);

        $scope.$watch('browseElement', (newVal, oldVal) => {
            if (newVal) {
               services.session.cloneToSession('BrowseElement', project.MappingProjectId, newVal);
               if ($scope.active) $scope.active.tab = 'browse';
            }
        });

        $scope.$watch('selectedBrowseItem', (newVal, oldVal) => {
            if (newVal) services.session.cloneToSession('BrowseElement', project.MappingProjectId, newVal);
        });

        $scope.isDisabled = (tab, isMapping) => {
            if (tab === 'browse') {
                if ($scope.selectedBrowseItem && $scope.selectedBrowseItem.ItemTypeId === element.ItemTypeId) return false;
                else return true;
            } else if (tab === 'search') {
                if ($scope.selectedSearchItems && $scope.selectedSearchItems.length > 0) return false;
                else return true;
            } else if (tab === 'suggest' && !isMapping) {
                if ($scope.selectedSuggestItems && $scope.selectedSuggestItems.length > 0) return false;
                else return true;
            } else if (tab === 'suggest' && isMapping) {
                if ($scope.selectedSuggestMapping) return false;
                else return true;
            }
        }

        $scope.setTab = (tab) => {
            $scope.active.tab = tab;
            if (tab === 'browse' && !visitedBrowse) {
                $scope.browseElement = services.session.cloneFromSession('BrowseElement', project.MappingProjectId);
                visitedBrowse = true;
            }
            services.session.cloneToSession('MappingTab', project.MappingProjectId, tab);
        }

        $scope.select = (tab, isMapping) => {

            if (tab === 'browse') {
                services.logging.add({
                    Source: 'browseController',
                    Message: `${project.ProjectName} ( ${project.MappingProjectId} ) - ${vm.me.FirstName} ${vm.me.LastName} selected the following element: ${$scope.selectedBrowseItem.DomainItemPath}`
                });
                $uibModalInstance.close([$scope.selectedBrowseItem]);
            } else if (tab === 'search') {
                var searchFilter = services.session.cloneFromSession('MatchmakerSearchFilter', project.MappingProjectId);

                if (searchFilter && searchFilter.SearchText && searchFilter.SearchText !== '') {
                    services.logging.add({
                        Source: 'searchController',
                        Message: `${project.ProjectName} ( ${project.MappingProjectId} ) - ${vm.me.FirstName} ${vm.me.LastName} searched for: ${searchFilter.SearchText}`
                    });
                } else {
                    services.logging.add({
                        Source: 'searchController',
                        Message: `${project.ProjectName} ( ${project.MappingProjectId} ) - ${vm.me.FirstName} ${vm.me.LastName} didn't supply search filter`
                    });
                }

                services.logging.add({
                    Source: 'searchController',
                    Message: `${project.ProjectName} ( ${project.MappingProjectId} ) - ${vm.me.FirstName} ${vm.me.LastName} selected the following element(s): ${services.underscore.pluck($scope.selectedSearchItems, 'DomainItemPath')}`
                });
                $uibModalInstance.close($scope.selectedSearchItems);
            } else if (tab === 'suggest' && !isMapping) {

                services.logging.add({
                    Source: 'suggestController',
                    Message: `${project.ProjectName} ( ${project.MappingProjectId} ) - ${vm.me.FirstName} ${vm.me.LastName} selected the following element(s): ${services.underscore.pluck($scope.selectedSuggestItems, 'DomainItemPath')}`
                });
                $uibModalInstance.close($scope.selectedSuggestItems);
            } else if (tab === 'suggest' && isMapping) {
                $scope.selectedSuggestMapping.IsMapping = true;
                $uibModalInstance.close([$scope.selectedSuggestMapping]);
            }
        }

        $scope.selected = (tab) => {
            if (tab === 'search') {
                if ($scope.selectedSearchItems && $scope.selectedSearchItems.length >= 2) return `s (${$scope.selectedSearchItems.length})`;
            } else if (tab === 'suggest') {
                if ($scope.selectedSuggestItems && $scope.selectedSuggestItems.length >= 2) return `s (${$scope.selectedSuggestItems.length})`;
            }
        }

        $scope.cancel = (tab) => {

            services.logging.add({
                Source: `${tab}Controller`,
                Message: `${project.ProjectName} ( ${project.MappingProjectId} ) - ${vm.me.FirstName} ${vm.me.LastName} cancelled the matchmaker from ${tab} tab`
            });

            $uibModalInstance.dismiss();
        }

    }
])