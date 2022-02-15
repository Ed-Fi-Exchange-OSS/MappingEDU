// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Interfaces 
//

interface IAuthorization {
    authorize(): void;
    authorizeDataStandard(dataStandardId: string, role: number, extensionPublicRole?: number): void;
    authorizeMappingProject(dataStandardId: string, role: number): void;
}

// ****************************************************************************
// Module app.security.authorization
//

var m = angular.module('app.security.authorization', ['app.services.logger']);


// ****************************************************************************
// Factory app.security.authorization 
//

m.factory('authorization', ['$rootScope', 'principal', 'repositories', 'services', 'enumerations',
    ($rootScope, principal: IPrincipal, repositories: IRepositories, services: IServices, enumerations: IEnumerations) => {

        function authorize() {

            services.logger.debug('Authorizing');

            return principal.identity().then(() => {

                var isAuthenticated = principal.isAuthenticated();

                if ($rootScope.toState.data && $rootScope.toState.data.roles && $rootScope.toState.data.roles.length > 0 && !principal.isInAnyRole($rootScope.toState.data.roles)) {

                    if (isAuthenticated) {
                        services.logger.debug(`Not authorized for desired state: ${$rootScope.toState}`);

                        console.log('redirect 5');

                        services.session.cloneToSession('navigation', 'error', {
                            description: 'You are unauthorized to view this page.',
                            to: {
                                state: $rootScope.toState,
                                params: $rootScope.toStateParams
                            }, 
                            from: {
                                state: $rootScope.fromState,
                                params: $rootScope.fromStateParams
                            }
                        });

                        services.state.go('app.errors.access-denied', {}, { location: 'replace' }); // user is signed in but not authorized for desired state
                    }
                    else {
                        //services.logger.debug('User is not authenticated');
                        //// user is not authenticated. stow the state they wanted before you
                        //// send them to the signin state, so you can return them when you're done
                        //$rootScope.returnToState = $rootScope.toState;
                        //$rootScope.returnToStateParams = $rootScope.toStateParams;

                        //// now, send them to the signin state so they can log in
                        //services.logger.debug('Redirecting to log-on');
                        //services.state.go('app.home');
                    }
                }
            });
        }

        function redirectDataStandard(dataStandardId, role) {
            var needAccess = enumerations.UserAccess[enumerations.UserAccess.map(x => x.Id).indexOf(role)].DisplayText;

            console.log('redirect 1');

            services.session.cloneToSession('navigation', 'error', {
                description: `You need at least ${needAccess} access to view this page.`,
                dataStandardId: dataStandardId,
                to: {
                    state: $rootScope.toState,
                    params: $rootScope.toStateParams
                },
                from: {
                    state: $rootScope.fromState,
                    params: $rootScope.fromStateParams
                }
            });

            services.state.go('app.errors.access-denied', {}, { location: 'replace' });
        }

        function authorizeDataStandard(dataStandardId: string, role: number, extensionPublicRole?: number) {

            services.logger.debug('Authorizing Data Standard Access');

            services.profile.me().then((me) => {
                if (me.IsAdministrator) return;

                services.profile.dataStandardAccess(dataStandardId).then((data: IDataStandardUser) => {
                    if (dataStandardId && data.Role < role) {
                        if (extensionPublicRole != undefined || data.Role < extensionPublicRole) {
                            repositories.dataStandard.find(dataStandardId).then(standard => {
                                if (!standard.AreExtensionsPublic) redirectDataStandard(dataStandardId, extensionPublicRole);
                            });
                        } else {
                            redirectDataStandard(dataStandardId, role);
                        }
                    }
                }, error => {
                    services.logger.error('', error.data);
                    redirectDataStandard(dataStandardId, role);
                });
            });
        }

        function authorizeMappingProject(mappingProjectId: string, role: number) {

            services.logger.debug('Authorizing Mapping Project Access');

            services.profile.me().then((me) => {
                if (me.IsAdministrator) return;

                services.profile.mappingProjectAccess(mappingProjectId).then((data: IMappingProjectUser) => {
                    if (mappingProjectId && data.Role < role) {

                        console.log('redirect 3');

                        var needAccess = enumerations.UserAccess[enumerations.UserAccess.map(x => x.Id).indexOf(role)].DisplayText;

                        services.session.cloneToSession('navigation', 'error', {
                            description: `You need at least ${needAccess} access to view this page.`,
                            mappingProjectId: mappingProjectId,
                            to: {
                                state: $rootScope.toState,
                                params: $rootScope.toStateParams
                            },
                            from: {
                                state: $rootScope.fromState,
                                params: $rootScope.fromStateParams
                            }
                        });

                        services.state.go('app.errors.access-denied', {}, { location: 'replace'});

                    }
                }, error => {
                    services.logger.error('', error.data);

                    var needAccess = enumerations.UserAccess[enumerations.UserAccess.map(x => x.Id).indexOf(role)].DisplayText;

                    console.log('redirect 4');

                    services.session.cloneToSession('navigation', 'error', {
                        description: `You need at least ${needAccess} access to view this page.`,
                        mappingProjectId: mappingProjectId,
                        to: {
                            state: $rootScope.toState,
                            params: $rootScope.toStateParams
                        },
                        from: {
                            state: $rootScope.fromState,
                            params: $rootScope.fromStateParams
                        }
                    });

                    services.state.go('app.errors.access-denied', {}, { location: 'replace' });

                });
            });
        }

        var authorization: IAuthorization = {
            authorize: authorize,
            authorizeDataStandard: authorizeDataStandard,
            authorizeMappingProject: authorizeMappingProject
        };
        return authorization;
    }
]); 