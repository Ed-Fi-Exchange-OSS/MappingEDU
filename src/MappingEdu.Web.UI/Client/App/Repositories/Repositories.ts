// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Interfaces IRepositories
//

interface IRepositories {
    authentication: IAuthenticationRepository;
    autoMap: IAutoMapRespository;
    customDetailMetadata: ICustomDetailMetadataRepository;
    dataStandard: IDataStandardRepository;
    element: IElementRepository;
    elementGroup: IElementGroupRepository;
    entity: IEntityRepository;
    home: IHomeRepository;
    logging: ILoggingRepository;
    mappingProject: IMappingProjectsRepository;
    notifications: INotificationRepository;
    organizations: IOrganizationsRepository;
    systemConstant: ISystemConstantRepository;
    systemItem: ISystemItemRepository;
    users: IUsersRepository;
}


// ****************************************************************************
// Interface IPagedResult
//

interface IPagedResult<T> {
    items: Array<T>;
    totalRecords: number;
}


// ****************************************************************************
// Module app.repositories
//

var m = angular.module('app.repositories', [
    'app.repositories.authentication',
    'app.repositories.auto-map',
    'app.repositories.custom-detail-metadata',
    'app.repositories.data-standard',
    'app.repositories.element',
    'app.repositories.element-group',
    'app.repositories.entity',
    'app.repositories.home',
    'app.repositories.logging',
    'app.repositories.mapping-project',
    'app.repositories.notifications',
    'app.repositories.organizations', 
    'app.repositories.system-constant',
    'app.repositories.system-item',
    'app.repositories.users'
]);


// ****************************************************************************
// Service 'repositories'
//

m.factory('repositories', [
    'authentication',
    'app.repositories.auto-map',
    'app.repositories.custom-detail-metadata',
    'app.repositories.data-standard',
    'app.repositories.element',
    'app.repositories.element-group',
    'app.repositories.entity',
    'app.repositories.home',
    'app.repositories.logging',
    'app.repositories.mapping-project',
    'app.repositories.notifications',
    'organizations',
    'app.repositories.system-constant',
    'app.repositories.system-item',
    'users',
    (
        authentication: IAuthenticationRepository,
        autoMap: IAutoMapRespository,
        customDetailMetadata: ICustomDetailMetadataRepository,
        dataStandard: IDataStandardRepository,
        element: IElementRepository,
        elementGroup: IElementGroupRepository,
        entity: IEntityRepository,
        home: IHomeRepository,
        logging: ILoggingRepository,
        mappingProjects: IMappingProjectsRepository,
        notifications: INotificationRepository,
        organizations: IOrganizationsRepository,
        systemConstant: ISystemConstantRepository,
        systemItem: ISystemItemRepository,
        users: IUsersRepository
    ) => {

        var repositories: IRepositories = {
            authentication: authentication,
            autoMap: autoMap,
            customDetailMetadata: customDetailMetadata,
            dataStandard: dataStandard,
            element: element,
            elementGroup: elementGroup,
            entity: entity,
            mappingProject: mappingProjects,
            notifications: notifications,
            home: home,
            logging: logging,
            organizations: organizations,
            systemConstant: systemConstant,
            systemItem: systemItem,
            users: users
        };
        return repositories;
    }
]);