// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.repositories.users
//

var m = angular.module('app.repositories.users', ['restangular']);


// ****************************************************************************
// Interface User
//
 
interface IUser extends restangular.IElement {
    ConfirmPassword: string;
    Email: string;
    EmailConfirmed: boolean;
    FirstName: string;
    Id?: string;
    IsAdministrator: boolean;
    LastLogin: Date;
    LastName: string;
    Password: string;
    UserName: string;
    fullName(): string;
}


// ****************************************************************************
// Interface CurrentUser
//
 
interface ICurrentUser extends restangular.IElement {
    ConfirmPassword: string;
    Email: string;
    EmailConfirmed: boolean;
    FirstName: string;
    Id?: string;
    IsAdministrator: Boolean;
    IsGuest: Boolean;
    LastLogin: Date;
    LastName: string;
    MappedSystems: Array<IUserDataStandard>;
    Password: string;
    Projects: Array<IUserMappingProject>;
    Roles: string[];
    UserName: string;
    fullName(): string;
}
// ****************************************************************************
// Interface IUserRegistrationResult
//
 
interface IUserRegistrationResult {
    EmailSent: Boolean;
    UserCreated: Boolean;
}

// ****************************************************************************
// Interface User Mapping Project
//
 
interface IUserMappingProject {
    Id: string;
    Role: number;
    Name: string;
    Description: string;
}

// ****************************************************************************
// Interface User Data Standard
//
 
interface IUserDataStandard {
    Id: string;
    Role: number;
    Version: string;
    Name: string;
}


// ****************************************************************************
// Interface IUsersRepository
//

interface IUsersRepository {
    checkExistsByEmail(email: string): angular.IPromise<any>;
    create(user: IUser): angular.IPromise<IUserRegistrationResult>;
    confirmEmail(token: string, user: IUser): angular.IPromise<any>;
    find(id: string): angular.IPromise<IUser>;
    findUsersByEmail(email: string): angular.IPromise<Array<IUser>>;
    forgotPassword(email: string): angular.IPromise<any>;
    getAll(): angular.IPromise<Array<IUser>>;
    getGuestIsActive(): angular.IPromise<boolean>;
    getOrganizations(id: string): angular.IPromise<Array<IOrganization>>;
    getProjects(id: string): angular.IPromise<Array<IUserMappingProject>>;
    getStandards(id: string): angular.IPromise<Array<IUserDataStandard>>;
    me(): angular.IPromise<ICurrentUser>;
    remove(userId: string): angular.IPromise<any>;
    resendEmail(id: string): angular.IPromise<any>;
    resetPassword(token: string, user: IUser): angular.IPromise<any>;
    save(user: IUser): angular.IPromise<any>;
    saveMe(user: any): angular.IPromise<any>;
    toggle(user: IUser): angular.IPromise<any>;
    toggleActive(id: string): angular.IPromise<any>;
}

// ****************************************************************************
// Users repository
//

m.factory('users', ['Restangular', (restangular: restangular.IService) => {

    // extend fullName()

    restangular.extendModel('users', (model: IUser) => {
        model.fullName = function () {
            return this.FirstName + ' ' + this.LastName;
        };
        return model;
    });

    restangular.extendModel('users', (model: ICurrentUser) => {
        model.fullName = function () {
            return this.FirstName + ' ' + this.LastName;
        };
        return model;
    });

    // methods

    function create(user: IUser) {
        return <angular.IPromise<IUserRegistrationResult>>restangular.all('users').post(user);
    }

    function checkExistByEmail(email: string) {
        return restangular.all('users').customPOST({}, 'exists', { email: email });
    }

    function find(id: string) {
        return <angular.IPromise<IUser>>restangular.one('users', id).get();
    }

    function guestIsActive() {
        return <angular.IPromise<boolean>>restangular.one('users/guest-is-active').get();
    }

    function findUsersByEmail(email: string) {
        return restangular.all('users').customGET('', { email: email });
    }

    function getAll() {
        return restangular.all('users').getList();
    }

    function me() {
        return <angular.IPromise<ICurrentUser>>restangular.all('users').customGET('me');
    }

    function remove(userId: string) {
        return restangular.one('users', userId).remove();
    }

    function save(user: IUser) {
        return restangular.copy(user).put(); //Hack do to restangular bug
    }

    function saveMe(user: any) {
        return restangular.one('users/me').customPUT(user);
    }

    function toggle(user: IUser) {
        return user.customPOST(null, 'toggle');
    }

    function getOrganizations(id: string) {
        return <angular.IPromise<Array<IOrganization>>>restangular.one('users', id).customGET('organizations');
    }

    function getProjects(id: string) {
        return <angular.IPromise<Array<IUserMappingProject>>>restangular.one('users', id).customGET('projects');
    }

    function getStandards(id: string) {
        return <angular.IPromise<Array<IUserDataStandard>>>restangular.one('users', id).customGET('standards');
    }
    
    function resendEmail(id: string) {
        return restangular.one('users', id).customPOST({}, 'resend-email');
    }

    function toggleActive(id: string) {
        return restangular.one('users', id).customPOST({}, 'toggle-active');
    }

    function confirmEmail(token: string, user: IUser) {
        return restangular.one('users', user.Id).customPOST(user, `confirm?code=${token}`);
    }

    function forgotPassword(email: string) {
        return restangular.one('users', email).customPOST({}, 'forgot-password');
    }

    function resetPassword(token: string, user: IUser) {
        return restangular.one('users', user.Id).customPOST(user, `reset-password?token=${token}`);
    }

    var repository: IUsersRepository = {
        create: create,
        checkExistsByEmail: checkExistByEmail,
        find: find,
        findUsersByEmail: findUsersByEmail,
        getAll: getAll,
        getGuestIsActive: guestIsActive,
        me: me,
        remove: remove,
        save: save,
        saveMe: saveMe,
        toggle: toggle,
        getOrganizations: getOrganizations,
        getProjects: getProjects,
        getStandards: getStandards,
        resendEmail: resendEmail,
        confirmEmail: confirmEmail,
        forgotPassword: forgotPassword,
        resetPassword: resetPassword,
        toggleActive: toggleActive
    };

    return repository;
}]); 