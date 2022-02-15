// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modals
//

var m = angular.module('app.modals', [
    'app.modals.auto-mapper',
    'app.modals.bulk-mapping',
    'app.modals.custom-detail-form',
    'app.modals.delta-form',
    'app.modals.enumeration-auto-mapper',
    'app.modals.mapping-project-queue-filter-form',
    'app.modals.system-item-custom-detail-form',
    'app.modals.enumeration-item-form',
    'app.modals.enumeration-map-form',
    'app.modals.share',
    'app.modals.system-item-form',
    'app.modals.welcome'
]);

// ****************************************************************************
// Interfaces app.services
//

interface IModals {
    autoMapper(project: any): ng.ui.bootstrap.IModalServiceInstance;
    bulkMapping(project: any, systemItem: any, itemTypeId: any): ng.ui.bootstrap.IModalServiceInstance;
    customDetailForm(dataStandardId: any, customDetail: any): ng.ui.bootstrap.IModalServiceInstance;
    deltaForm(isPrevious: any, delta: any, standard: any): ng.ui.bootstrap.IModalServiceInstance;
    enumerationAutoMapper(sourceEnumerationItems: any, targetEnumerationItems: any): ng.ui.bootstrap.IModalServiceInstance;
    enumerationItemForm(enumeration: any, systemItemId: any): ng.ui.bootstrap.IModalServiceInstance;
    enumerationMapForm(enumeration: any, targetEnumerationItems: any, systemItemMapId: any): ng.ui.bootstrap.IModalServiceInstance;
    mappingProjectQueueFilterForm(mappingProjectId: string, filter: any, elementGroups: Array<any>, creaters: Array<any>, updaters: Array<any>): ng.ui.bootstrap.IModalServiceInstance;
    share(service: any, type: any, modelId: any): ng.ui.bootstrap.IModalServiceInstance;
    systemItemCustomDetailForm(systemItemId: string, systemItemCustomDetails: any): ng.ui.bootstrap.IModalServiceInstance;
    systemItemForm(dataStandardId: string, systemItem: any, itemTypeId: any): ng.ui.bootstrap.IModalServiceInstance;
    welcome(): ng.ui.bootstrap.IModalServiceInstance;
}

m.factory('modals', [
    'app.modals.auto-mapper',
    'app.modals.bulk-mapping',
    'app.modals.custom-detail-form',
    'app.modals.delta-form',
    'app.modals.enumeration-auto-mapper',
    'app.modals.enumeration-item-form',
    'app.modals.enumeration-map-form',
    'app.modals.mapping-project-queue-filter-form',
    'app.modals.share',
    'app.modals.system-item-custom-detail-form',
    'app.modals.system-item-form',
    'app.modals.welcome',
    (
        autoMapper,
        bulkMapping,
        customDetailForm,
        deltaForm,
        enumerationAutoMapper,
        enumerationItemForm,
        enumerationMapForm,
        mappingProjectQueueFilterForm,
        share,
        systemItemCustomDetailForm,
        systemItemForm,
        welcome
    ) => {

    var modals: IModals = {
        autoMapper: autoMapper,
        bulkMapping: bulkMapping,
        customDetailForm: customDetailForm,
        deltaForm: deltaForm,
        enumerationAutoMapper: enumerationAutoMapper,
        enumerationItemForm: enumerationItemForm,
        enumerationMapForm: enumerationMapForm,
        mappingProjectQueueFilterForm: mappingProjectQueueFilterForm,
        share: share,
        systemItemCustomDetailForm: systemItemCustomDetailForm,
        systemItemForm: systemItemForm,
        welcome: welcome
    }

    return modals;

}]); 
