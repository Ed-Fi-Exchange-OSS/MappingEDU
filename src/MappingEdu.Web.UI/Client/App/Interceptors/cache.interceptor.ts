// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module("app.interceptors.cache", [])
    .factory("cacheInterceptor", ['$injector', ($injector) => {

        function endsWith(string, searchString, position?) {
            if (!(position < string.length))
                position = string.length;
            else
                position |= 0;
            return string.substr(position - searchString.length, searchString.length) === searchString;
        }

        return {
            'request': (config) => {
                var settings = $injector.get('settings');

                if (endsWith(config.url.toLowerCase(), '.html') && config.url.toLowerCase().indexOf('client/app/') >= 0 ) {
                    config.url += ('?v=' + settings.version);
                }
                return config;
            }
        }
    }]);