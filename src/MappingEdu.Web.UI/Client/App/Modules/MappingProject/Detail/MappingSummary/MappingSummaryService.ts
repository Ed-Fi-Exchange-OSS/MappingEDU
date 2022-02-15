// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Interfaces IMappingSummaryService
//

interface IMappingSummaryService {
    buildHrefs(mappingProjectId: string, filter: any, itemTypeId?: number);
    buildHeaderHrefs(mappingProjectId: string, itemTypeId?: number);
    buildDomainHrefs(mappingProjectId: string, domain: any, itemTypeId?: number);
    buildEntityHrefs(mappingProjectId: string, domain: any, entity: any, itemTypeId?: number);
    buildValuesMatrix(mappingProjectId: string, detail: any, itemTypeId?: number);
    setPercents(detail: any);
}


// ****************************************************************************
// Module app.services.logger
//

var m = angular.module('app.mapping-project.detail.mapping-summary.service', []);


// ****************************************************************************
// Service 'logger' (Toastr)
//

m.service('mappingSummaryService', ['services', 'repositories', (services: IServices, repositories: IRepositories) => {

    function buildHrefs(mappingProjectId, filter, itemTypeId) {
        var hrefs = {};

        var itemType = {};
        if (itemTypeId === 4) itemType['element'] = true;
        else if (itemTypeId === 5) itemType['enumeration'] = true;

        filter.itemTypes = itemType;

        hrefs['total'] = services.state.href('app.mapping-project.detail.review-queue', {
            mappingProjectId: mappingProjectId,
            filter: JSON.stringify(filter)
        });

        var statuses = [0, 1, 2, 3, 4];
        var methods = [1, 3, 4];

        angular.forEach(statuses, status => {

            var filterStatuses = {};
            filterStatuses[status] = true;

            filter.methods = {};
            filter.statuses = filterStatuses;

            hrefs[`Status_${status}`] = services.state.href('app.mapping-project.detail.review-queue', {
                mappingProjectId: mappingProjectId,
                filter: JSON.stringify(filter)
            });

        });

        angular.forEach(methods, method => {

            var filterMethods = {};
            filterMethods[method] = true;

            filter.methods = filterMethods;
            filter.statuses = {};

            hrefs[`Method_${method}`] = services.state.href('app.mapping-project.detail.review-queue', {
                mappingProjectId: mappingProjectId,
                filter: JSON.stringify(filter)
            });

        });

        return hrefs;
    }
    
    function buildDomainHrefs(mappingProjectId, domain, itemTypeId) {

         var queueFilters = services.session.cloneFromSession('queueFilters', mappingProjectId);

         var elementGroups = {};
         if (domain.SystemItemId !== '00000000-0000-0000-0000-000000000000')
             elementGroups[domain.SystemItemId] = true;

         var filter = {
             elementGroups: elementGroups,
             methods: {},
             statuses: {},
             flagged: false,
             globalSearch: '',
             pageSize: (queueFilters) ? queueFilters.pageSize : 10,
             pageNo: 0,
             orderBy: (queueFilters) ? queueFilters.orderBy : []
         };

         return buildHrefs(mappingProjectId, filter, itemTypeId);
    }

     function buildHeaderHrefs(mappingProjectId, itemTypeId) {

         var queueFilters = services.session.cloneFromSession('queueFilters', mappingProjectId);

         var filter = {
             elementGroups: {},
             methods: {},
             statuses: {},
             flagged: false,
             globalSearch: '',
             pageSize: (queueFilters) ? queueFilters.pageSize : 10,
             pageNo: 0,
             orderBy: (queueFilters) ? queueFilters.orderBy : []
         };

         return buildHrefs(mappingProjectId, filter, itemTypeId);
     }

     function buildEntityHrefs(mappingProjectId, domain, entity, itemTypeId) {

         var queueFilters = services.session.cloneFromSession('queueFilters', mappingProjectId);

         var search = domain.ItemName + '.' + entity.ItemName;
         if (entity.ItemTypeId < 4) search += '.';

         var elementGroups = {};
         if (domain.SystemItemId !== '00000000-0000-0000-0000-000000000000')
            elementGroups[domain.SystemItemId] = true;

         var filter = {
             elementGroups: elementGroups,
             methods: {},
             statuses: {},
             flagged: false,
             globalSearch: search,
             pageSize: (queueFilters) ? queueFilters.pageSize : 10,
             pageNo: 0,
             orderBy: (queueFilters) ? queueFilters.orderBy : []
         };

         return buildHrefs(mappingProjectId, filter, itemTypeId);
     }

     function buildValuesMatrix(mappingProjectId, detail, itemTypeId) {

         var queueFilters = services.session.cloneFromSession('queueFilters', mappingProjectId);

         var search = '';
         var elementGroups = {};
         if (detail.Parent) {
             search = detail.Parent.ItemName + '.' + detail.ItemName;
             if (detail.ItemTypeId < 4) search += '.';
             elementGroups[detail.Parent.SystemItemId] = true;
         } else if (detail.SystemItemId !== '00000000-0000-0000-0000-000000000000') {
             elementGroups[detail.SystemItemId] = true;
         }

         var itemType = {};
         if (itemTypeId === 4) itemType['element'] = true;
         else if (itemTypeId === 5) itemType['enumeration'] = true;

         var filter = {
             elementGroups: elementGroups,
             methods: {},
             statuses: {},
             itemTypes: itemType,
             flagged: false,
             globalSearch: search,
             pageSize: (queueFilters) ? queueFilters.pageSize : 10,
             pageNo: 0,
             orderBy: (queueFilters) ? queueFilters.orderBy : []
         };

         var values = [
             [{ Value: 0, Href: '' }, { Value: 'N/A', Href: '' }, { Value: 'N/A', Href: '' }, { Value: 'N/A', Href: '' }, { Value: 'N/A', Href: '' }, { Value: 0, Href: '' }],
             [{ Value: 'N/A', Href: '' }, { Value: 0, Href: '' }, { Value: 0, Href: '' }, { Value: 0, Href: '' }, { Value: 0, Href: '' }, { Value: 0, Href: '' }],
             [{ Value: 'N/A', Href: '' }, { Value: 0, Href: '' }, { Value: 0, Href: '' }, { Value: 0, Href: '' }, { Value: 0, Href: '' }, { Value: 0, Href: '' }],
             [{ Value: 'N/A', Href: '' }, { Value: 0, Href: '' }, { Value: 0, Href: '' }, { Value: 0, Href: '' }, { Value: 0, Href: '' }, { Value: 0, Href: '' }],
             [{ Value: 'N/A', Href: '' }, { Value: 0, Href: '' }, { Value: 0, Href: '' }, { Value: 0, Href: '' }, { Value: 0, Href: '' }, { Value: 0, Href: '' }],
             [{ Value: 0, Href: '' }, { Value: 0, Href: '' }, { Value: 0, Href: '' }, { Value: 0, Href: '' }, { Value: 0, Href: '' }, { Value: 0, Href: '' }]
         ];

         for (var i = 0; i < 6; i++) {
             for (var j = 0; j < 6; j++) {
                 var statuses = {};
                 var methods = {};

                 if (i === 0 || j === 0) {
                     statuses[0] = true;
                 } else if (i === 5 && j === 5) {
                     
                 } else if(j === 5) {
                     methods[i] = true;
                 } else if (i === 5) {
                     statuses[j] = true;
                 } else {
                     statuses[j] = true;
                     methods[i] = true;
                 }

                 if(values[i][j].Value !== 'N/A') {
                     filter.statuses = statuses;
                     filter.methods = methods;

                     values[i][j].Href = services.state.href('app.mapping-project.detail.review-queue', {
                        mappingProjectId: mappingProjectId,
                        filter: JSON.stringify(filter)
                    });
                 }
             }
         }

         return values;
     }

     function setPercents(detail) {
         detail.Percents = [
             { Href: detail.Hrefs['Status_0'], Label: 'Unmapped', Percent: Math.round(100 * (detail.Unmapped / detail.Total)) },
             { Href: detail.Hrefs['Status_1'], Label: 'Incomplete', Percent: Math.round(100 * (detail.Incomplete / detail.Total)) },
             { Href: detail.Hrefs['Status_2'], Label: 'Completed', Percent: Math.round(100 * (detail.Completed / detail.Total)) },
             { Href: detail.Hrefs['Status_3'], Label: 'Reviewed', Percent: Math.round(100 * (detail.Reviewed / detail.Total)) },
             { Href: detail.Hrefs['Status_4'], Label: 'Approved', Percent: Math.round(100 * (detail.Approved / detail.Total)) }
         ];
     }

     var service: IMappingSummaryService = {
         buildHrefs: buildHrefs,
         buildDomainHrefs: buildDomainHrefs,
         buildHeaderHrefs: buildHeaderHrefs,
         buildEntityHrefs: buildEntityHrefs,
         buildValuesMatrix: buildValuesMatrix,
         setPercents: setPercents
     };

     return service;
}]); 
