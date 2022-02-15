// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.repositories.mapping-projects
//

var m = angular.module('app.repositories.data-standard',
    ['restangular',
     'app.repositories.data-standard.elements',
     'app.repositories.data-standard.extensions',
     'app.repositories.data-standard.import',
     'app.repositories.data-standard.next',
     'app.repositories.data-standard.source',
     'app.repositories.data-standard.target',
     'app.services.navbar']);


// ****************************************************************************
// Interface Mapping Project
//
 
interface IDataStandard extends restangular.IElement {
    CreateDate: string;
    DataStandardId?: string;
    IsActive: boolean;
    PreviousDataStandard?: IDataStandard;
    PreviousDataStandardId?: string;
    SystemName: string;
    SystemVersion: string;
    UpdateDate: string;
    sref?: string;
    AreExtensionsPublic: string;
}

// ****************************************************************************
// Interface Mapping Project Clone
//

interface IDataStandardClone extends IDataStandard {
    SimilarVersioning: boolean;
}

// ****************************************************************************
// Interface Share Standard To User Model
//

interface IShareStandardToUser {
    Email: string;
    Role: number;
}

// ****************************************************************************
// Interface Mapping Project Users
//

interface IDataStandardUser {
    FirstName?: string;
    LastName?: string;
    Email?: string;
    Id: string;
    UserId?: string;
    Role: number;
}


// ****************************************************************************
// Interface IMappingProjectsRepository
//

interface IDataStandardRepository {
    elements: IDataStandardElementsRepository;
    extensions: IMappedSystemExtensionsRepository;
    next: IDataStandardNextRepository;
    source: IDataStandardSourceRepository;
    target: IDataStandardTargetRepository;
    import: IDataStandardImportRepository,
    exportData(id: string): any;
    getAll(): angular.IPromise<Array<IDataStandard>>;
    getAllWithoutNextVersions(): angular.IPromise<Array<IDataStandard>>;
    getAllOrphaned(): angular.IPromise<Array<IDataStandard>>;
    getAllPublic(): angular.IPromise<Array<IDataStandard>>;
    getCreator(id: string): angular.IPromise<any>;
    getOwners(id: string): angular.IPromise<any>;
    getExportToken(id: string, data?): angular.IPromise<any>;
    find(id: string): angular.IPromise<IDataStandard>;
    create(dataStandard: any): angular.IPromise<any>;
    save(dataStandard: IDataStandard): angular.IPromise<any>;
    remove(id: string): angular.IPromise<any>;
    clone(id: string, standard: any): angular.IPromise<IDataStandardClone>;
    isExtended(id: string): angular.IPromise<boolean>;
    getUser(standardId: string, userId: string): angular.IPromise<IDataStandardUser>;
    getTaggableUsers(projectId: string): angular.IPromise<Array<IDataStandardUser>>;
    toggleFlagged(standardId: string, userId: string): angular.IPromise<any>;
    togglePublic(standardId: string): angular.IPromise<any>;
    togglePublicExtensions(standardId: string): angular.IPromise<any>;
    getUsers(standardId: string): angular.IPromise<Array<IDataStandardUser>>;
    addUser(standardId: string, user: IShareStandardToUser): angular.IPromise<any>;
    removeUser(standardId: string, userId: string): angular.IPromise<any>;
}

// ****************************************************************************
// Mapping Projects repository
//

m.factory('app.repositories.data-standard', [
    'Restangular',
    'app.repositories.data-standard.elements',
    'app.repositories.data-standard.extensions',
    'app.repositories.data-standard.import',
    'app.repositories.data-standard.next',
    'app.repositories.data-standard.source',
    'app.repositories.data-standard.target',
    'navbar',
    (
        restangular: restangular.IService,
        elements: IDataStandardElementsRepository,
        extensions: IMappedSystemExtensionsRepository,
        importRepository: IDataStandardImportRepository,
        next: IDataStandardNextRepository,
        source: IDataStandardSourceRepository,
        target: IDataStandardTargetRepository,
        navbar: INavbarService) => {

    restangular.extendModel('DataStandard', (model: IDataStandard) => {
        return model;
    });
    
    // methods

    function exportData(id: string) {
        return restangular.one('DataStandard', id).withHttpConfig({ responseType: 'arraybuffer' }).customGET('export');
    }

    function getAll() {
        return restangular.all('DataStandard').getList();
    }

    function getAllOrphaned() {
        return restangular.all('DataStandard').all('orphaned').getList();
    }

    function getAllPublic() {
        return restangular.all('DataStandard').all('public').getList();
    }

    function getOwners(id: string) {
        return restangular.one('DataStandard', id).customGET('owners');
    }

    function getCreator(id: string) {
        return restangular.one('DataStandard', id).customGET('creator');
    }

    function getAllWithoutNextVersions() {
        return restangular.all('DataStandard').all('without-next-versions').getList();
    }

    function getExportToken(id: string, data?) {
        return restangular.one('DataStandard', id).one('Export').customPOST(data, 'token');
    }

    function find(id: string) {
       return restangular.one('DataStandard', id).get();
    }

    function create(dataStandard: IDataStandard) {
        return restangular.one('DataStandard').customPOST(dataStandard).then(data => {
            navbar.update();
            return data;
        });
    }

    function save(dataStandard: IDataStandard) {
        return restangular.one('DataStandard', dataStandard.DataStandardId).customPUT(dataStandard).then(data => {
            navbar.update();
            return data;
        });
    }

    function remove(id: string) {
        return restangular.one('DataStandard', id).remove().then(data => {
            navbar.update();
            return data;
        });
    }

    function clone(id: string, standard: any) {
        return restangular.one('DataStandard', id).customPOST(standard, 'clone').then(data => {
            navbar.update();
            return data;
        });
    }

    function isExtended(id: string) {
        return restangular.one('DataStandard', id).customGET('is-extended');
    }

    function getUser(standardId: string, userId: string) {
        return restangular.one('MappedSystem', standardId).customGET(`user/${userId}`);
    }

    function toggleFlagged(standardId: string, userId: string) {
        navbar.toggleStandardFlag(standardId);
        return restangular.one(`MappedSystem/${standardId}/user/${userId}`).post('toggle-flag');
    }

    function togglePublic(standardId: string) {
        return restangular.one(`DataStandard/${standardId}`).post('toggle-public');
    }

    function togglePublicExtensions(standardId: string) {
        return restangular.one(`DataStandard/${standardId}`).post('toggle-public-extensions');
    }

    function getUsers(standardId: string) {
        return restangular.one('MappedSystem', standardId).customGET('users');
    }

    function getTaggableUsers(standardId: string) {
        return restangular.one('MappedSystem', standardId).customGET('taggable-users');
    }

    function addUser(standardId: string, user: IShareStandardToUser) {
        return restangular.one('MappedSystem', standardId).customPOST(user, 'users');
    }

    function removeUser(standardId: string, userId: string) {
        return restangular.one('MappedSystem', standardId).customDELETE(`users/${userId}`);
    }

    var repository: IDataStandardRepository = {
        elements: elements,
        extensions: extensions,
        import: importRepository,
        next: next,
        source: source,
        target: target,
        exportData: exportData,
        getAll: getAll,
        getAllOrphaned: getAllOrphaned,
        getAllWithoutNextVersions: getAllWithoutNextVersions,
        getAllPublic: getAllPublic,
        getExportToken: getExportToken,
        getOwners: getOwners,
        getCreator: getCreator,
        isExtended: isExtended,
        find: find,
        create: create,
        save: save,
        remove: remove,
        clone: clone,
        getUser: getUser,
        toggleFlagged: toggleFlagged,
        togglePublic: togglePublic,
        togglePublicExtensions: togglePublicExtensions,
        getUsers: getUsers,
        getTaggableUsers: getTaggableUsers,
        addUser: addUser,
        removeUser: removeUser
    };

    return repository;
}]); 