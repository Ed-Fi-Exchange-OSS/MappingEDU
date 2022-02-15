// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.repositories.mapping-project
//

var m = angular.module('app.repositories.mapping-project', [
    'restangular',
    'app.repositories.mapping-project.dashboard',
    'app.repositories.mapping-project.reports',
    'app.repositories.mapping-project.review-queue',
    'app.repositories.mapping-project.status',
    'app.repositories.mapping-project.summary',
    'app.repositories.mapping-project.synonym',
    'app.repositories.mapping-project.system-item-maps',
    'app.repositories.mapping-project.template',
    'app.services.navbar']);


// ****************************************************************************
// Interface Mapping Project
//
 
interface IMappingProject extends restangular.IElement {
    //TODO
}

// ****************************************************************************
// Interface Mapping Project Users
//

interface IMappingProjectUser {
    FirstName?: string;
    LastName?: string;
    Email?: string;
    Id: string;
    UserId?: string;
    Role: number;
}

interface IShareProjectToUser {
    Email: string;
    Role: number;
}


// ****************************************************************************
// Interface IMappingProjectsRepository
//

interface IMappingProjectsRepository {
    systemItemMaps: ISystemItemMapsRepository;
    dashboard: IMappingProjectDashboardRepository;
    reports: IMappingProjectReportsRepository;
    reviewQueue: IMappingProjectReviewQueueRepository;
    status: IMappingProjectStatusRepository;
    summary: IMappingProjectSummaryRepository;
    synonym: IMappingProjectSynonymRepository;
    template: IMappingProjectTemplateRepository;
    getAll(): angular.IPromise<any>;
    getAllOrphaned(): angular.IPromise<any>;
    getAllPublic(): angular.IPromise<any>;
    getCreator(id: string): angular.IPromise<any>;
    getOwners(id: string): angular.IPromise<any>;
    find(id: string): angular.IPromise<any>;
    create(data: any, autoMap?: boolean): angular.IPromise<any>;
    save(id: string, data: any): angular.IPromise<any>;
    remove(id: string): angular.IPromise<any>;
    clone(mappingProjectId: string, data: any): angular.IPromise<any>;
    getUser(projectId: string, userId: string): angular.IPromise<IMappingProjectUser>;
    getTaggableUsers(projectId: string): angular.IPromise<IMappingProjectUser>;
    toggleFlagged(projectId: string, userId: string): angular.IPromise<any>;
    togglePublic(projectId: string): angular.IPromise<any>;
    getUsers(projectId: string): angular.IPromise<Array<IMappingProjectUser>>;
    addUser(projectId: string, user: IShareProjectToUser): angular.IPromise<any>;
    removeUser(projectId: string, userId: string): angular.IPromise<any>;
}

// ****************************************************************************
// Mapping Projects repository
//

m.factory('app.repositories.mapping-project', [
    'Restangular',
    'app.repositories.mapping-project.dashboard',
    'app.repositories.mapping-project.reports',
    'app.repositories.mapping-project.review-queue',
    'app.repositories.mapping-project.status',
    'app.repositories.mapping-project.summary',
    'app.repositories.mapping-project.synonym',
    'app.repositories.mapping-project.system-item-maps',
    'app.repositories.mapping-project.template',
    'navbar',
    (restangular: restangular.IService,
     dashboard: IMappingProjectDashboardRepository,
     reports: IMappingProjectReportsRepository,
     reviewQueue: IMappingProjectReviewQueueRepository,
     status: IMappingProjectStatusRepository,
     summary: IMappingProjectSummaryRepository,
     synonym: IMappingProjectSynonymRepository,
     systemItemMaps: ISystemItemMapsRepository,
     template: IMappingProjectTemplateRepository,
     navbar: INavbarService) => {

    restangular.extendModel('MappingProject', (model: IMappingProject) => {
        return model;
    });

    // cache
    
    
    
    // methods

    function getAll() {
        return restangular.all('MappingProject').getList();
    }

    function getAllOrphaned() {
        return restangular.all('MappingProject').all('orphaned').getList();
    }

    function getAllPublic() {
        return restangular.all('MappingProject').all('public').getList();
    }

    function getOwners(id: string) {
        return restangular.one('MappingProject', id).customGET('owners');
    }

    function getCreator(id: string) {
        return restangular.one('MappingProject', id).customGET('creator');
    }

    function find(id: string) {
        return restangular.one('MappingProject', id).get();
    }

    function create(data: any, autoMap?: boolean) {
        return restangular.one(`MappingProject${(autoMap) ? '?autoMap=true' : ''}`).customPOST(data).then(data => {
            navbar.update();
            return data;
        });
    }

    function save(id: string, data: any) {
        return restangular.one('MappingProject', id).customPUT(data).then(data => {
            navbar.update();
            return data;
        });
    }

    function remove(id: string) {
        return restangular.one('MappingProject', id).remove().then(data => {
            navbar.update();
            return data;
        });
    }

    function clone(mappingProjectId, data: any) {
        return restangular.one('MappingProject', mappingProjectId).customPOST(data, 'clone').then(data => {
            navbar.update();
            return data;
        });
    }

    function getUser(projectId: string, userId: string) {
        return restangular.one('MappingProject', projectId).customGET(`user/${userId}`);
    }

    function getUsers(projectId: string) {
        return restangular.one('MappingProject', projectId).customGET('users');
    }

    function getTaggableUsers(projectId: string) {
        return restangular.one('MappingProject', projectId).customGET('taggable-users');
    }

    function toggleFlagged(projectId: string, userId: string) {
        navbar.toggleProjectFlag(projectId);
        return restangular.one(`MappingProject/${projectId}/user/${userId}`).post('toggle-flag');
    }

    function togglePublic(projectId: string) {
        return restangular.one(`MappingProject/${projectId}`).post('toggle-public');
    }

    function addUser(projectId: string, user: IShareProjectToUser) {
        return restangular.one('MappingProject', projectId).customPOST(user, 'users');
    }

    function removeUser(projectId: string, userId: string) {
        return restangular.one('MappingProject', projectId).customDELETE('users/' + userId);
    }

    var repository: IMappingProjectsRepository = {
        systemItemMaps: systemItemMaps,
        dashboard: dashboard,
        reports: reports,
        reviewQueue: reviewQueue,
        status: status,
        summary: summary,
        synonym: synonym,
        template: template,
        getAll: getAll,
        getAllOrphaned: getAllOrphaned,
        getAllPublic: getAllPublic,
        getCreator: getCreator,
        getOwners: getOwners,
        find: find,
        create: create,
        save: save, 
        remove: remove,
        clone: clone,
        getUser: getUser,
        getTaggableUsers: getTaggableUsers,
        toggleFlagged: toggleFlagged,
        togglePublic: togglePublic,
        getUsers: getUsers,
        addUser: addUser,
        removeUser: removeUser
};

    return repository;
}]); 
