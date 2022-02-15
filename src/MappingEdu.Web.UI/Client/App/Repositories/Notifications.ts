// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.repositories.notifications
//

var m = angular.module('app.repositories.notifications', ['restangular']);


// ****************************************************************************
// Interface INotificationRepository
//

interface INotificationRepository {
    dismissMany(ids: Array<string>): angular.IPromise<void>;
    dismissOne(id: string): angular.IPromise<void>;
    getUnread(id?: string): angular.IPromise<number>;
    getElements(filter: any, id?: string): angular.IPromise<Array<any>>;
}

// ****************************************************************************
// SystemConstants repository
//

m.factory('app.repositories.notifications', ['Restangular', (restangular: restangular.IService) => {

    // methods

    function dismissMany(ids: Array<string>) {
        return restangular.one('Notifications').customDELETE('dismiss', { notificationIds: ids});
    }

    function dismissOne(id: string) {
        return restangular.one('Notifications', id).customDELETE('dismiss');
    }

    function getUnread(id?: string) {
        return restangular.one('Notifications').customGET('unread', {mappingProjectId: id});
    }

    function getElements(filter: any, id?: string) {
        return restangular.one('Notifications').customPOST(filter, 'Elements', { mappingProjectId: id });
    }
    
    var repository: INotificationRepository = {
        dismissMany: dismissMany,
        dismissOne: dismissOne,
        getUnread: getUnread,
        getElements: getElements
    };

    return repository;
}]); 