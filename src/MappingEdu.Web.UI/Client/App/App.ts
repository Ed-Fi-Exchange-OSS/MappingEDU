// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// Shims to deal with upgrade problems
angular.lowercase = (input: string) => (typeof input === 'string') ? input.toLowerCase() : input;
angular.uppercase = (input: string) => (typeof input === 'string') ? input.toUpperCase() : input;


// ****************************************************************************
// Core Application Module 
//

var m = angular.module('app', [
    'app.constants',
    'app.security',
    'app.repositories',
    'app.services',
    'app.directives',
    'app.modals',
    'app.filters',
    'app.interceptors',
    'app.modules.login',
    'app.modules.data-standard',
    'app.modules.element',
    'app.modules.element-group',
    'app.modules.entity',
    'app.modules.errors',
    'app.modules.home',
    'app.modules.layout',
    'app.modules.manage',
    'app.modules.mapping-project',
    'app.modules.styles-demo',
    'chart.js',
    'ui.select',
    'ui.router',
    'ui.bootstrap',
    'uiSwitch',
    'restangular',
    'datatables',
    'monospaced.elastic',
    'ngMessages',
    'ngFileUpload',
    'ngAnimate',
    'ngSanitize',
    'ngProgress',
    'angularPromiseButtons',
    'darthwade.dwLoading',
    'textAngular',
    'fixed.table.header',
    'mj.scrollingTabs',
    'vs-repeat',
    'ipCookie'
]); 

// ****************************************************************************
// Module Constants
//
 
m.value('$', $);
m.value('_', _);


// ****************************************************************************
// Configure 
//

m.config([
    '$httpProvider',
    '$locationProvider',
    '$uibModalProvider',
    '$stateProvider',
    '$uibTooltipProvider',
    '$urlRouterProvider',
    'RestangularProvider',
    'settings',
    (
        $httpProvider: ng.IHttpProvider,
        $locationProvider: ng.ILocationProvider,
        $uibModalProvider: ng.ui.bootstrap.IModalProvider,
        $stateProvider: ng.ui.IStateProvider,
        $uibTooltipProvider: ng.ui.bootstrap.ITooltipProvider,
        $urlRouterProvider: ng.ui.IUrlRouterProvider,
        restangularProvider: restangular.IProvider,
        settings: ISystemSettings
    ) => {
        $locationProvider.html5Mode(false);
        $uibTooltipProvider.setTriggers({ 'show': 'hide' });
        $urlRouterProvider.otherwise('/');
        restangularProvider.setBaseUrl(settings.apiBaseUri);
        restangularProvider.setRestangularFields({ id: 'Id' });
        restangularProvider.setDefaultHeaders({
            'Cache-Control': 'no-cache, no-store, must-revalidate',
            'Pragma': 'no-cache',
            'Expires': 0
        });

        //Default Modal settings
        $uibModalProvider.options = {
            backdrop: 'static',
            keyboard: false,
            animation: true
        }

        // security 

        $stateProvider.state('app', {
            resolve: {
                identity: ['security', (security: ISecurity) => security.principal.identity()],
                authorize: ['security', 'identity', (security: ISecurity) => security.authorization.authorize()]
            },
            views: {
                'navbar@': {
                    templateUrl: `${settings.moduleBaseUri}/layout/navbar.tpl.html`,
                    controller: 'app.layout.navbar'
                },
                'footer@': {
                    templateUrl: `${settings.moduleBaseUri}/layout/footer.tpl.html`,
                    controller: 'app.layout.footer'
                }
            }
        });

        $httpProvider.interceptors.push('cacheInterceptor');
    }
]);


// ****************************************************************************
// Run 
//

m.run(['$rootScope', '$state', 'security', 'services', 'repositories', 'ngProgressFactory',
    ($rootScope, $state, security: ISecurity, services: IServices, repositories: IRepositories, ngProgressFactory: NgProgress.INgProgressProvider) => {

        $rootScope.progress = ngProgressFactory.createInstance();

        $rootScope.$on("$stateChangeError", console.log.bind(console));

        security.principal.configure({
            requestTokenName: 'Authorization',
            storageKey: 'principal.identity',
            storageMode: PrincipalStorageMode.Session
        });

        $rootScope.$on('$stateChangeStart', (event, toState, toStateParams, fromState, fromStateParams) => {

            $rootScope.toState = toState;
            $rootScope.toStateParams = toStateParams;
            $rootScope.fromState = fromState;
            $rootScope.fromStateParams = fromStateParams;
            $rootScope.currentState = toState.name;
            $rootScope.loading = true;

            if (!security.principal.isIdentityResolved()) {
                event.preventDefault();
                security.principal.identity().then(() => {
                    services.state.go(toState, toStateParams);
                }).catch(angular.noop);
            } 
            else if (toState.name.indexOf('app.login') === -1) {

                security.principal.identity().then(() => {
                    if (security.principal.isIdentityResolved()) {
                        security.authorization.authorize();
                    }

                    var activeStandardId = null;
                    var activeProjectId = null;

                    // Mapping Project and Data Standard Security
                    if (security.principal.isAuthenticated()) {
                        if (toState.name.indexOf('data-standard') > -1 ||
                            (toState.name.indexOf('element') > -1 && toStateParams.dataStandardId) ||
                            (toState.name.indexOf('entity') > -1 && toStateParams.dataStandardId) ||
                            (toState.name.indexOf('enumeration') > -1 && toStateParams.dataStandardId)) {
                            security.authorization
                                .authorizeDataStandard(toStateParams.dataStandardId,
                                toState.data.access,
                                toState.data.extensionsPublicAccess);
                            services.profile.dataStandardAccess(toStateParams.dataStandardId);
                            activeStandardId = toStateParams.dataStandardId;
                        } else if (toState.name.indexOf('element') > -1 ||
                            toState.name.indexOf('entity') > -1 ||
                            toState.name.indexOf('enumeration') > -1) {
                            security.authorization
                                .authorizeMappingProject(toStateParams.mappingProjectId, toState.data.access);
                            services.profile.mappingProjectAccess(toStateParams.mappingProjectId);
                            activeProjectId = toStateParams.mappingProjectId;
                        } else if (toState.name.indexOf('mapping-project') > -1) {
                            security.authorization.authorizeMappingProject(toStateParams.id, toState.data.access);
                            services.profile.mappingProjectAccess(toStateParams.id);
                            activeProjectId = toStateParams.id;
                        }

                        services.navbar.set(activeStandardId, activeProjectId);
                    }
                }).catch(angular.noop);
            }
        });

        $rootScope.$on('$stateChangeSuccess', (event, to, toParams, from, fromParams) => {

            angular.element(document.querySelectorAll('.container-fluid')).removeClass('container-fluid').addClass('container');

            $rootScope.loading = false;

            if (!_.isUndefined($state.current.title))

            // Title was getting changed before the browser could add the previous one to the history, 
            //  so had to add a timeout here
            services.timeout(() => {
                document.title = $state.current.title + ($state.current.title !== 'MappingEDU' ? ' - MappingEDU' : '');
            }, 100);

            var goBackToStates = ['app.entity.detail.info', 'app.data-standard.edit.elements', 'app.data-standard.edit.browse',
                'app.mapping-project.detail.review-queue', 'app.mapping-project.detail.dashboard', 'app.mapping-project.detail.notifications',
                'app.data-standard.edit.extensions.report', 'app.data-standard.edit.extension-elements'];

            if (_.contains(goBackToStates, to.name))
                services.session.cloneToSession('navigation', 'goBack', { state: to, params: toParams });

                
        });
    }
]);


// ****************************************************************************
// Controller app.container
//

m.controller('app.container', ['$scope', '$window', 'services', 'settings', 'security', ($scope, $window: ng.IWindowService, services: IServices, settings: ISystemSettings, security: ISecurity) => {

        services.logger.debug('Loading controller app.container.');
        
        security.principal.identity().then(data => {
            if (data) $scope.username = data.name;
        }).catch(angular.noop);

        $scope.init = () => {
            services.profile.me().then(data => {
                $scope.isAdmin = data.IsAdministrator;
                $scope.username = data.fullName();
            }).catch(angular.noop);   
        }

        if (services.state.current.name.indexOf('app.login') !== -1) $scope.init();

        services.events.on('event:login', () => {
            $scope.init();
        });
    }
]);
