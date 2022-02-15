// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.repositories.element.matchmaker
//

var m = angular.module('app.repositories.element.matchmaker', ['restangular']);

// ****************************************************************************
// Interface IMatchmakerRepository
//

interface IMatchmakerRepository {
    search(searchText: string, itemTypeId: string, id: string): angular.IPromise<any>;
    suggest(mappingProjectId: string, systemItemId: string): angular.IPromise<any>;
}

// ****************************************************************************
// Repository Matchmaker
//

m.factory('app.repositories.element.matchmaker', ['Restangular', '$q', (restangular: restangular.IService, $q: angular.IQService) => {

    var searchResults = {};

    // methods

    function search(searchText: string, itemTypeId: string, id: string) {
        var defered = $q.defer();

        if (searchResults[id] && searchResults[id][itemTypeId]) {
            defered.resolve(angular.copy(searchResults[id][itemTypeId]));
        } else {
           restangular.one(`ElementSearch/${id}?itemTypeId=${itemTypeId}&searchText=${searchText}`).get().then((data) => {
                if (!searchResults[id]) searchResults[id] = {};
                searchResults[id][itemTypeId] = data;
                defered.resolve(angular.copy(data));
            });
        }
        return defered.promise;
    }

    function suggest(mappingProjectId: string, systemItemId: string) {
        return restangular.one(`ElementSuggest/${mappingProjectId}`, systemItemId).get();
    }

    var repository: IMatchmakerRepository = {
        search: search,
        suggest: suggest
    };

    return repository;
}]); 

