// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.services.profile
//

var m = angular.module('app.services.profile', ['app.repositories']);


// ****************************************************************************
// Interface IProfileService
//

interface IProfileService {
    clearProfile(): void;
    dataStandardAccess(dataStandardId: string): angular.IPromise<IDataStandardUser>;
    updateDataStandardAccess(dataStandardId: string): angular.IPromise<IDataStandardUser>;
    isReadOnlyDataStandard(dataStandardId: string): angular.IPromise<boolean>;
    mappingProjectAccess(mappingProjectId: string): angular.IPromise<IMappingProjectUser>;
    me(): angular.IPromise<ICurrentUser>;
    update(): angular.IPromise<ICurrentUser>;
    updateMappingProjectAccess(mappingProjectId: string): angular.IPromise<IMappingProjectUser>;
    isReadOnlyMappingProject(mappingProjectId: string): angular.IPromise<boolean>;
}


// ****************************************************************************
// Profile
//

m.factory('profile', ['principal', 'repositories', '$rootScope', '$q', 'logger', '_', (principal: IPrincipal, repositories: IRepositories, $rootScope, $q, logger, _: UnderscoreStatic) => {

    var user: ICurrentUser = null;

    //user the guids as a key
    var standards = {};
    var projects = {};

    function me(): angular.IPromise<ICurrentUser> {
        var deferred = $q.defer();
        if (user === null && principal.isAuthenticated()) {
            logger.debug('Retrieving profile');
            repositories.users.me().then((data) => {
                logger.debug('profile loaded');
                user = data;
                $rootScope.username = (data.FirstName) ? `${data.FirstName} ${data.LastName}` : data.UserName;

                deferred.resolve(user);
            });
        } else if (!principal.isAuthenticated()) {
            deferred.resolve({FirstName: 'Guest', LastName: 'Account', IsGuest: true});
        } else  {
            deferred.resolve(user);
        }
        return deferred.promise;
    }


    function update(): angular.IPromise<ICurrentUser> {
        user = null;
        return me();
    }

    function clearProfile(): void {
        user = null;
        standards = {};
        projects = {};
    }

    function updateDataStandardAccess(dataStandardId: string): angular.IPromise<IDataStandardUser> {
        var deferred = $q.defer();
        me().then((data) => {
            return repositories.dataStandard.getUser(dataStandardId, data.Id).then((data) => {
                standards[dataStandardId] = data;
                deferred.resolve(data);
            }, error => {
                standards[dataStandardId] = {};
                deferred.resolve(standards[dataStandardId]);
            });
        });
        return deferred.promise;
    }

    function dataStandardAccess(dataStandardId: string): angular.IPromise<IDataStandardUser> {

        var deferred = $q.defer();
        if (dataStandardId && standards[dataStandardId] == null) {
            updateDataStandardAccess(dataStandardId).then((data) => {
                deferred.resolve(data);
            });
        }
        else deferred.resolve(standards[dataStandardId]);
        return deferred.promise;
    }

    function isReadOnlyDataStandard(dataStandardId: string): angular.IPromise<boolean> {
        var deferred = $q.defer();
        dataStandardAccess(dataStandardId).then(data => {
            me().then((me) => {
                if (data.Role > 1 || me.IsAdministrator) deferred.resolve(false);
                else deferred.resolve(true);
            });
        });
        return deferred.promise;
    }

    function updateMappingProjectAccess(mappingProjectId: string): angular.IPromise<IMappingProjectUser> {
        var deferred = $q.defer();
        me().then((data) => {
            return repositories.mappingProject.getUser(mappingProjectId, data.Id).then((data) => {
                projects[mappingProjectId] = data;
                deferred.resolve(projects[mappingProjectId]);
            }, error => {
                projects[mappingProjectId] = {};
                deferred.resolve(projects[mappingProjectId]);
            });
        });
        return deferred.promise;
    }

    function mappingProjectAccess(mappingProjectId: string): angular.IPromise<IMappingProjectUser> {
        var deferred = $q.defer();
        if (mappingProjectId && projects[mappingProjectId] == null) {
            updateMappingProjectAccess(mappingProjectId).then((data) => {
                deferred.resolve(data);
            });
        }
        else deferred.resolve(projects[mappingProjectId]);
        return deferred.promise;
    }

    function isReadOnlyMappingProject(mappingProjectId: string): angular.IPromise<boolean> {
        var deferred = $q.defer();
        mappingProjectAccess(mappingProjectId).then(data => {
            me().then((me) => {
                console.log(data);
                if (data.Role > 1 || me.IsAdministrator) deferred.resolve(false);
                else deferred.resolve(true);
            });
        });
        return deferred.promise;
    }

    var profile: IProfileService = {
        me: me,
        update: update,
        clearProfile: clearProfile,
        dataStandardAccess: dataStandardAccess,
        updateDataStandardAccess: updateDataStandardAccess,
        mappingProjectAccess: mappingProjectAccess,
        updateMappingProjectAccess: updateMappingProjectAccess,
        isReadOnlyDataStandard: isReadOnlyDataStandard,
        isReadOnlyMappingProject: isReadOnlyMappingProject
    };
    return profile;
}]);
