// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.favorite

var m = angular.module('app.directives.favorite', []);

m.directive('maFavorite', ['repositories', 'settings', 'services', (repositories: IRepositories, settings: ISystemSettings, services: IServices) => {
        return {
            restrict: 'E',
            scope: {
                model: '='
            },
            templateUrl: `${settings.directiveBaseUri}/Favorite/Favorite.tpl.html`,
            link: (scope) => {

                scope.random = Math.round(Math.random() * 1000000);

                var me: ICurrentUser;
                services.profile.me().then(data => { me = data; });

                scope.toggle = () => {
                    scope.model.disabled = true;
                    if (scope.model.DataStandardId) {
                        repositories.dataStandard.toggleFlagged(scope.model.DataStandardId, me.Id).then(() => {
                        }, error => {
                            scope.model.Flagged = !scope.model.Flagged;
                            services.logger.error('Error updating favorites.', error);
                        }).finally(() => {
                            scope.model.disabled = false;
                        });
                    } else if (scope.model.MappingProjectId) {
                        repositories.mappingProject.toggleFlagged(scope.model.MappingProjectId, me.Id).then(() => {
                        }, error => {
                            scope.model.Flagged = !scope.model.Flagged;
                            services.logger.error('Error updating favorites.', error);
                        }).finally(() => {
                            scope.model.disabled = false;
                        });
                    } else {
                        services.logger.error('Error model id undefined');
                    }
                }
            }
        }
    }
]);
