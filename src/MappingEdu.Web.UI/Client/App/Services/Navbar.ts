// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.services.navbar
//

var m = angular.module('app.services.navbar', ['app.repositories.home']);


// ****************************************************************************
// Interface INavbarService
//

interface INavbarService {
    set(standardId, projectId): void;
    update(): void;
    toggleProjectFlag(projectId): void;
    toggleStandardFlag(standardId): void;
}


// ****************************************************************************
// Navbar
//

m.factory('navbar', ['$rootScope', 'app.repositories.home', '$q', '_', ($rootScope, home: IHomeRepository, $q, _: UnderscoreStatic) => {

    var navbar = null;
    var activeStandardId = null;
    var activeProjectId = null;

    function get(): angular.IPromise<any> {
        var deferred = $q.defer();
        if (!navbar) {
            home.getHomePage().then(data => {
                navbar = data;
                deferred.resolve(data);
            });
        } else deferred.resolve(navbar);
        return deferred.promise;  
    }

    function set(standardId, projectId): void {
        activeStandardId = standardId;
        activeProjectId = projectId;
        get().then((data) => {

            $rootScope.activeTab = '';
            $rootScope.activeProject = null;
            $rootScope.activeStandard = null;

            if (activeStandardId) {
                $rootScope.activeTab = 'standard';
                $rootScope.activeStandard = _.findWhere(data.DataStandardList, { DataStandardId: activeStandardId });
            } else if (activeProjectId) {
                $rootScope.activeTab = 'project';
                $rootScope.activeProject = _.findWhere(data.MappingProjectList, { MappingProjectId: activeProjectId });
            } else {
                if ($rootScope.currentState === 'app.data-standard.list') $rootScope.activeTab = 'standard';
                else if ($rootScope.currentState === 'app.mapping-project.list') $rootScope.activeTab = 'project';
                else $rootScope.activeTab = 'home';
            }

            $rootScope.projects = _.where(data.MappingProjectList, { Flagged: true });
            $rootScope.standards = _.where(data.DataStandardList, { Flagged: true });
        });
    }

    function update(): void {
        home.getHomePage().then(data => {
            navbar = angular.copy(data);
            set(activeStandardId, activeProjectId);
        });
    }

    function toggleProjectFlag(projectId): void {
        angular.forEach(navbar.MappingProjectList, project => {
            if (project.MappingProjectId === projectId) {
                project.Flagged = !project.Flagged;
                set(activeStandardId, activeProjectId);
                return;
            }
        });
    }

    function toggleStandardFlag(standardId): void {
        angular.forEach(navbar.DataStandardList, standard => {
            if (standard.DataStandardId === standardId) {
                standard.Flagged = !standard.Flagged;
                set(activeStandardId, activeProjectId);
                return;
            }
        });
    }

    var navbarService: INavbarService = {
        set: set,
        update: update,
        toggleProjectFlag: toggleProjectFlag,
        toggleStandardFlag: toggleStandardFlag
    };
    return navbarService;
}]);
