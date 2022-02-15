// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.element-group.new-system-item
//

var m = angular.module('app.repositories.element-group.new-system-item', ['restangular']);


// ****************************************************************************
// Interface New System Item
//
 
interface INewSystemItem extends restangular.IElement {
    //TODO
}

// ****************************************************************************
// Interface INewSystemItemRepository
//

interface INewSystemItemRepository {
    create(systemItem: any): angular.IPromise<any>;
}

// ****************************************************************************
// New System Item repository
//

m.factory('app.repositories.element-group.new-system-item', ['Restangular', (restangular: restangular.IService) => {

    restangular.extendModel('NewSystemItem', (model: INewSystemItem) => {
        return model;
    });

    // methods

    function create(systemItem: any) {
        return restangular.one('NewSystemItem').customPOST(systemItem);
    }

    var repository: INewSystemItemRepository = {
        create: create
    };

    return repository;
}]); 



