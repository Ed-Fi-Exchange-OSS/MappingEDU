// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.element.workflow-status
//

var m = angular.module('app.repositories.element.workflow-status', ['restangular']);


// ****************************************************************************
// Interface WorkflowStatus
//
 
interface IWorkflowStatus extends restangular.IElement {
    //TODO
}

// ****************************************************************************
// Interface IWorkflowStatusRepository
//

interface IWorkflowStatusRepository {
    save(id: string, id2: string, data: any): angular.IPromise<any>;
}

// ****************************************************************************
// Workflow Status repository
//

m.factory('app.repositories.element.workflow-status', ['Restangular', (restangular: restangular.IService) => {

    restangular.extendModel('WorkflowStatus', (model: IWorkflowStatus) => {
        return model;
    });

    // methods

    function save(id: string, id2: string, data: any) {
        return restangular.one('WorkflowStatus/' + id, id2).customPUT(data);
    }

    var repository: IWorkflowStatusRepository = {
        save: save
    };

    return repository;
}]); 