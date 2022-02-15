// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.element
//

var m = angular.module('app.modules.element', [
    'app.modules.element.detail',
    'app.modules.element.matchmaker'
]);


// ****************************************************************************
// Configure 
//

m.config(['$urlRouterProvider', '$stateProvider', 'enumerations', ($urlRouterProvider: ng.ui.IUrlRouterProvider, $stateProvider: ng.ui.IStateProvider, enumerations: IEnumerations) => {

    $stateProvider
        .state('app.element', {
            abstract: true,
            url: '/element?mappingProjectId&dataStandardId&filter&elementListFilter&listContextId',
            data: {
                roles: ['user', 'guest'],
                access: enumerations.UserAccess[enumerations.UserAccess.map(x => x.DisplayText).indexOf('Guest')].Id
            },
            resolve: {
                model: ['repositories', '$stateParams', (repositories: IRepositories, $stateParams) => {
                    if ($stateParams.dataStandardId) return repositories.dataStandard.find($stateParams.dataStandardId);
                    else if ($stateParams.mappingProjectId) return repositories.mappingProject.find($stateParams.mappingProjectId);
                }],
                readOnly: ['services', '$stateParams', (services: IServices, $stateParams) => {
                    if ($stateParams.dataStandardId) return services.profile.isReadOnlyDataStandard($stateParams.dataStandardId);
                    else if($stateParams.mappingProjectId) return services.profile.isReadOnlyMappingProject($stateParams.mappingProjectId);
                }],
                templates: ['repositories', '$stateParams', (repositories: IRepositories, $stateParams) => {
                    if ($stateParams.dataStandardId) return null;
                    else if ($stateParams.mappingProjectId) return repositories.mappingProject.template.getAll($stateParams.mappingProjectId);
                }]
            }
        });

}]);

// ****************************************************************************
// Run 
// Filter can be passed through the Url and evaluated to get list of elements

m.run(['$rootScope', '$state', 'services', 'repositories', ($rootScope, $state, services: IServices, repositories: IRepositories) => {

    $rootScope.$on('$stateChangeStart', (event, toState, toParams) => {

        if (toState.name.indexOf('app.element') > -1)
        {
            if (toParams.listContextId) {
                repositories.systemItem.detail(toParams.listContextId).then(entity => {
                    var ids = [];

                    services.underscore.sortBy(<Array<any>>entity.ChildSystemItems, (item) => {
                        return item.ItemName;
                    });

                    services.underscore.each(entity.ChildSystemItems, (item: any) => {
                        if (item.ItemTypeId > 3) ids.push({ ElementId: item.SystemItemId });
                    });

                    if (toParams.mappingProjectId) services.session.cloneToSession('elementQueues', toParams.mappingProjectId, ids);
                    else services.session.cloneToSession('elementLists', toParams.dataStandardId, ids);

                    services.state.go('app.element.detail.mapping', {
                        mappingProjectId: toParams.mappingProjectId,
                        dataStandardId: toParams.dataStandardId,
                        elementId: toParams.elementId
                    }, { location: 'replace' });

                }, error => {
                    services.logger.error('Error loading ids.', error.data);
                });
            } else if (toParams.mappingProjectId && toParams.filter) {

                event.preventDefault();

                var queueFilter = toParams.filter;
                var resume = toParams.resume;

                $rootScope.loading = true;
                repositories.element.mappingProjectElements.find(toParams.mappingProjectId, queueFilter || 'AllIncomplete')
                    .then(data => {
                        var elementId = (data) ? data[0].ElementId : null;
                        if (resume && elementId) {
                            var maxDateElement = services.underscore.max(<Array<any>>data, element => new Date(element.UpdateDate));
                            elementId = (maxDateElement) ? maxDateElement.ElementId : elementId;
                        }
                        services.session.cloneToSession('elementQueues', toParams.mappingProjectId, data);
                        services.state.go('app.element.detail.mapping', {
                            mappingProjectId: toParams.mappingProjectId,
                            elementId: elementId
                        }, { location: 'replace' });
                    }, error => {
                        services.logger.error('Error loading mapping project elements.', error.data);
                        $rootScope.loading = false;
                    }
                    );
            } else if (toParams.elementListFilter) {
                event.preventDefault();

                var listFilter = JSON.parse(toParams.elementListFilter);

                if (toParams.mappingProjectId) {

                    var promise: angular.IPromise<any[]>;
                    if (listFilter.fromNotifications) promise = repositories.notifications.getElements(listFilter, toParams.mappingProjectId);
                    else promise = repositories.mappingProject.reviewQueue.getElements(toParams.mappingProjectId, $.param(listFilter));

                    promise.then((ids) => {
                        services.session.cloneToSession('elementQueues', toParams.mappingProjectId, ids);
                        services.state.go('app.element.detail.mapping', {
                            mappingProjectId: toParams.mappingProjectId,
                            elementId: toParams.elementId
                        }, { location: 'replace' });
                    });
                } else {
                    repositories.dataStandard.elements.getElements(toParams.dataStandardId, $.param(listFilter)).then((data) => {
                        services.session.cloneToSession('elementLists', toParams.dataStandardId, data);
                        services.state.go('app.element.detail.info', {
                            dataStandardId: toParams.dataStandardId,
                            elementId: toParams.elementId
                        }, { location: 'replace' });
                    });
                }
            }
        }
    });
}]);
