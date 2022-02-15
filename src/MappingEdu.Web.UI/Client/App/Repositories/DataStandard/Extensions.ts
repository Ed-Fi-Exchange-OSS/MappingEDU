// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.data-standard.extensions
//

var m = angular.module('app.repositories.data-standard.extensions', ['restangular']);


// ****************************************************************************
// Interface IMappedSystemExtensionsRepository
//

interface IMappedSystemExtensionsRepository {
    downloadExtensionDifferences(mappedSystemId: string, model: any): angular.IPromise<any>;
    downloadableReport(mappedSystemId: string): angular.IPromise<any>;
    getAll(mappedSystemId: string): angular.IPromise<Array<any>>;
    getLinkableStandards(mappedSystemId: string): angular.IPromise<Array<any>>;
    get(mappedSystemId: string, mappedSystemExtensionId: string): angular.IPromise<any>;
    getReport(mappedSystemId: string, parentSystemItemId: string): angular.IPromise<Array<any>>;
    getReportDetail(mappedSystemId: string, model: any): angular.IPromise<Array<any>>;
    post(mappedSystemId: string, model: any): angular.IPromise<any>;
    getLinkingDetail(mappedSystemId: string, model: any): angular.IPromise<any>;
    hasExtensions(mappedSystemId: string): angular.IPromise<boolean>;
    put(mappedSystemId: string, mappedSystemExtensionId: string, model: any): angular.IPromise<any>;
    remove(mappedSystemId: string, mappedSystemExtensionId: string): angular.IPromise<void>;
}

// ****************************************************************************
// Repository Extensions
//

m.factory('app.repositories.data-standard.extensions', [
    '$q', 'Restangular', 'settings', ($q: ng.IQService, restangular: restangular.IService, settings: ISystemSettings) => {

        // methods

        function downloadExtensionDifferences(mappedSystemId: string, model: any) {
            return restangular.one('MappedSystem/', mappedSystemId).one('Download-Extension-Differences').customPOST(model);
        }

        function downloadableReport(mappedSystemId: string) {
            var deferred = $q.defer();

            restangular.one('MappedSystem', mappedSystemId).one('Download-Extension-Report').get().then((token) => {
                var link = document.createElement('a');
                link.href = `${settings.apiBaseUri}/MappedSystemExtensionReport/${token}`;
                document.body.appendChild(link);
                link.click();
                deferred.resolve();
            }, error => {
                deferred.reject(error);
            });

            return deferred.promise;
        }

        function getAll(mappedSystemId: string) {
            return restangular.one('MappedSystem/', mappedSystemId).one('Extension').get();
        }

        function getLinkableStandards(mappedSystemId: string) {
            return restangular.one('MappedSystem/', mappedSystemId).one('Linkable-Standards').get();
        }

        function get(mappedSystemId: string, mappedSystemExtensionId: string) {
            return restangular.one('MappedSystem/', mappedSystemId).one('Extension', mappedSystemExtensionId).get();
        }

        function getReport(mappedSystemId: string, parentSystemItemId: string) {
            return restangular.one('MappedSystem/', mappedSystemId).customGET('Extension-Report', { parentSystemItemId: parentSystemItemId });
        }

        function getReportDetail(mappedSystemId: string, model: any) {
            return restangular.one('MappedSystem/', mappedSystemId).customPOST(model, 'Extension-Report-Detail');
        }

        function getLinkingDetail(mappedSystemId: string, model: any) {
            return restangular.one('MappedSystem/', mappedSystemId).one('Extension-Linking-Detail').customPOST(model);
        }

        function hasExtensions(mappedSystemId: string) {
            return restangular.one('MappedSystem/', mappedSystemId).one('Has-Extensions').get();
        }

        function post(mappedSystemId: string, model: any) {
            return restangular.one('MappedSystem/', mappedSystemId).one('Extension').customPOST(model);
        }

        function put(mappedSystemId: string, mappedSystemExtensionId: string, model: any) {
            return restangular.one('MappedSystem/', mappedSystemId).one('Extension', mappedSystemExtensionId).customPUT(model);
        }

        function remove(mappedSystemId: string, mappedSystemExtensionId: string) {
            return restangular.one('MappedSystem/', mappedSystemId).one('Extension', mappedSystemExtensionId).remove();
        }

        var repository: IMappedSystemExtensionsRepository = {
            downloadExtensionDifferences: downloadExtensionDifferences,
            downloadableReport: downloadableReport,
            getAll: getAll,
            getLinkableStandards: getLinkableStandards,
            get: get,
            getLinkingDetail: getLinkingDetail,
            getReport: getReport,
            getReportDetail: getReportDetail,
            hasExtensions: hasExtensions,
            post: post,
            put: put,
            remove: remove
        };

        return repository;
    }
]);