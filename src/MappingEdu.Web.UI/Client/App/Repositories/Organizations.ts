// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.repositories.organizations
//

var m = angular.module('app.repositories.organizations', ['restangular']);


// ****************************************************************************
// Interface Organization
//
 
interface IOrganization extends restangular.IElement {
    Id?: string;
    OrganizationId?: string;
    Domains: Array<string>;
    Description: string;
    Name: string;
    sref(): string;
}


// ****************************************************************************
// Interface IOrganizationsRepository
//

interface IOrganizationsRepository {
    create(organization: IOrganization): angular.IPromise<IOrganization>;
    find(id: string): angular.IPromise<IOrganization>;
    getAll(): angular.IPromise<Array<IOrganization>>;
    remove(organization: IOrganization): angular.IPromise<any>;
    save(organization: IOrganization): angular.IPromise<any>;
    toggle(organization: IOrganization): angular.IPromise<any>;
    getUsers(id: string): angular.IPromise<Array<IUser>>;
    addUser(organizationId: string, userId: string): angular.IPromise<IUser>;
    removeUser(organizationId: string, userId: string): angular.IPromise<any>;
}

// ****************************************************************************
// Organizations repository
//

m.factory('organizations', ['Restangular', (restangular: restangular.IService) => {

    // extend 

    restangular.extendModel('organizations', (model: IOrganization) => {
        return model;
    });

    // methods

    function create(organization: IOrganization) {
        return <angular.IPromise<IOrganization>>restangular.all('organizations').post(organization);
    }

    function find(id: string) {
        return <angular.IPromise<IOrganization>>restangular.one('organizations', id).get();
    }

    function getAll() {
        return restangular.all('organizations').getList();
    }

    function remove(organization: IOrganization) {
        return organization.remove();
    }

    function save(organization: IOrganization) {
        return restangular.copy(organization).put(); //Hack do to restangular bug
    }

    function toggle(organization: IOrganization) {
        return organization.customPOST(null, 'toggle');
    }

    function getUsers(id: string) {
        return <angular.IPromise<Array<IUser>>>restangular.one('organizations', id).customGET('users');
    }

    function addUser(organizationId: string, userId: string) {
        return <angular.IPromise<IUser>>restangular.one('organizations', organizationId).customPOST({ "UserId": userId }, 'users');
    }

    function removeUser(organizationId: string, userId: string) {
        return restangular.one('organizations', organizationId).customDELETE('users/' + userId);
    }

    var repository: IOrganizationsRepository = {
        create: create,
        find: find,
        getAll: getAll,
        remove: remove,
        save: save,
        toggle: toggle,
        getUsers: getUsers,
        addUser: addUser,
        removeUser: removeUser
    };

    return repository;
}]); 