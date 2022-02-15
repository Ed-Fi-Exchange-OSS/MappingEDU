// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.repositories.element
//

var m = angular.module('app.repositories.element',
    ['restangular',
        'app.repositories.element.detail',
        'app.repositories.element.mapping',
        'app.repositories.element.enumeration',
        'app.repositories.element.enumeration-item',
        'app.repositories.element.enumeration-item-mapping',
        'app.repositories.element.map-note',
        'app.repositories.element.mapping-project-elements',
        'app.repositories.element.matchmaker',
        'app.repositories.element.next-version-delta',
        'app.repositories.element.note',
        'app.repositories.element.previous-version-delta',
        'app.repositories.element.system-item-detail',
        'app.repositories.element.system-item-enumeration',
        'app.repositories.element.workflow-status']);


// ****************************************************************************
// Interface Element
//
 
interface IElement extends restangular.IElement {
    //TODO
}

// ****************************************************************************
// Interface IElementRepository
//

interface IElementRepository {
    detail: IElementDetailRepository;
    mapping: IElementMappingRepository;
    enumeration: IEnumerationRepository;
    enumerationItem: IEnumerationItemRepository;
    enumerationItemMapping: IEnumerationItemMappingRepository;
    mapNote: IMapNoteRepository;
    mappingProjectElements: IMappingProjectElementsRepository;
    matchmaker: IMatchmakerRepository;
    nextVersionDelta: INextVersionDeltaRepository;
    note: INoteRepository;
    previousVersionDelta: IPreviousVersionDeltaRepository;
    systemItemDetail: ISystemItemDetailRepository;
    systemItemEnumeration: ISystemItemEnumerationRepository;
    workflowStatus: IWorkflowStatusRepository;
    getAll(id: string): angular.IPromise<any>;
    getChildren(id: string): angular.IPromise<any>;
    find(mappingProjectId: string, id: string): angular.IPromise<any>;
    save(id: string, data: any): angular.IPromise<any>;
    search(dataStandardId: string, parentSystemItemId?: string, searchText?:string, domainItemPath?: string );
    remove(element: any): angular.IPromise<any>;
}

// ****************************************************************************
// Element repository
//

m.factory('app.repositories.element', [
    'Restangular',
    'app.repositories.element.detail',
    'app.repositories.element.mapping',
    'app.repositories.element.enumeration',
    'app.repositories.element.enumeration-item',
    'app.repositories.element.enumeration-item-mapping',
    'app.repositories.element.map-note',
    'app.repositories.element.mapping-project-elements',
    'app.repositories.element.matchmaker',
    'app.repositories.element.next-version-delta',
    'app.repositories.element.note',
    'app.repositories.element.previous-version-delta',
    'app.repositories.element.system-item-detail',
    'app.repositories.element.system-item-enumeration',
    'app.repositories.element.workflow-status',

    (restangular: restangular.IService,
        detail: IElementDetailRepository,
        mapping: IElementMappingRepository,
        enumeration: IEnumerationRepository,
        enumerationItem: IEnumerationItemRepository,
        enumerationItemMapping: IEnumerationItemMappingRepository,
        mapNote: IMapNoteRepository,
        mappingProjectElements: IMappingProjectElementsRepository,
        matchmaker: IMatchmakerRepository,
        nextVersionDelta: INextVersionDeltaRepository,
        note: INoteRepository,
        previousVersionDelta: IPreviousVersionDeltaRepository,
        systemItemDetail: ISystemItemDetailRepository,
        systemItemEnumeration: ISystemItemEnumerationRepository,
        workflowStatus: IWorkflowStatusRepository) => {

        restangular.extendModel('Element', (model: IElement) => {
            return model;
        });

        // methods

        function getAll(id: string) {
            return restangular.one('Element/' + id).get();
        }

        function getChildren(id: string) {
            return restangular.one('Element/' + id).customGET('children');
        }

        function find(mappingProjectId: string, id: string) {
            return restangular.one('Element/' + mappingProjectId, id).get();
        }

        function save(id: string, data: any) {
            return restangular.one('Element', id).customPUT(data);
        }

        function search(dataStandardId: string, parentSystemItemId?: string, searchText?: string, domainItemPath?: string) {
            return restangular.one('ElementSearch', dataStandardId).get({ parentSystemItemId: parentSystemItemId, searchText: searchText, domainItemPath: domainItemPath });
        }

        function remove(element: any) {
            return restangular.one('Element', element.SystemItemId).remove();
        }

        var repository: IElementRepository = {
            detail: detail,
            mapping: mapping,
            enumeration: enumeration,
            enumerationItem: enumerationItem,
            enumerationItemMapping: enumerationItemMapping,
            mapNote: mapNote,
            mappingProjectElements: mappingProjectElements,
            matchmaker: matchmaker,
            nextVersionDelta: nextVersionDelta,
            note: note,
            previousVersionDelta: previousVersionDelta,
            systemItemDetail: systemItemDetail,
            systemItemEnumeration: systemItemEnumeration,
            workflowStatus: workflowStatus,
            getAll: getAll,
            getChildren: getChildren,
            find: find,
            save: save,
            search: search,
            remove: remove
        };

        return repository;
    }]); 