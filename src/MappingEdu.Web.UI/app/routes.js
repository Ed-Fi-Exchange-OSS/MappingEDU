// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appMappingEdu').config(['$stateProvider', '$urlRouterProvider', function routingFunction($stateProvider, $urlRouterProvider) {
        $urlRouterProvider
            .when('/p/:id', '/p/:id/i')
            .when('/m/:id', '/m/:id/i')
            .when('/el?mappingProjectId&dataStandardId', '/el/i?mappingProjectId&dataStandardId')
            .when('/enm/:id?mappingProjectId&dataStandardId', '/enm/:id/i?mappingProjectId&dataStandardId')
            .when('/g/:id?mappingProjectId&dataStandardId', '/g/:id/i?mappingProjectId&dataStandardId')
            .otherwise('/');

        $stateProvider
            .state('home', {
                title: 'MappingEDU',
                url: '/',
                controller: 'homeController',
                templateUrl: 'app/home/homeView.html',
                controllerAs: 'homeViewModel',
                data: {
                    roles: ['user']   
                },
                resolve: {
                    authorize: ['security', function(security) {
                        return security.authorization.authorize();
                    }]
                }
            })
            .state('login', {
                title: 'MappingEdu',
                url: '/login',
                controller: 'loginController',
                templateUrl: 'app/login/login/loginView.html',
                controllerAs: 'loginViewModel'
            }) 
            .state('forgot', {
                title: 'MappingEdu',
                url: '/forgot',
                controller: 'passwordResetController',
                templateUrl: 'app/login/password-reset/passwordResetView.html',
                controllerAs: 'passwordResetModel'
            })
            .state('login', {
                title: 'MappingEdu',
                url: '/login',
                controller: 'loginController',
                templateUrl: 'app/login/login/loginView.html',
                controllerAs: 'loginViewModel'
            })
            .state('forgot', {
                title: 'MappingEdu',
                url: '/forgot',
                controller: 'passwordResetController',
                templateUrl: 'app/login/password-reset/passwordResetView.html',
                controllerAs: 'passwordResetModel'
            })
            .state('styles', {
                title: 'Styles',
                url: '/s',
                controller: 'stylesController',
                templateUrl: 'app/stylesDemo/stylesView.html',
                controllerAs: 'stylesViewModel',
                data: {
                    roles: ['admin']
                }
            })
            .state('createDataStandard', {
                title: 'Create Data Standard',
                url: '/x/CreateNewDataStandard',
                templateUrl: 'app/dataStandard/editDataStandard.html',
                controller: 'dataStandardController',
                controllerAs: 'dataStandardViewModel',
                data: {
                    roles: ['user']
                }
            })
            .state('createMappingProject', {
                title: 'Create Mapping Project',
                url: '/x/createNewMappingProject',
                templateUrl: 'app/mappingProject/editMappingProject.html',
                controller: 'createNewMappingProjectController',
                controllerAs: 'createNewMappingProjectViewModel',
                data: {
                    roles: ['user']
                }
            })
            .state('mappingProject', {
                url: '/p/:id',
                templateUrl: 'app/mappingProject/mappingProjectDetailView.html',
                controller: 'mappingProjectDetailController',
                controllerAs: 'mappingProjectDetailViewModel',
                data: {
                    roles: ['user']
                }
            })
            .state('mappingProject.info', {
                title: 'Mapping Project Info',
                url: '/i',
                parent: 'mappingProject',
                controller: 'mappingProjectInfoController',
                templateUrl: 'app/mappingProject/mappingProjectInfoView.html',
                controllerAs: 'mappingProjectInfoViewModel'
            })
            .state('mappingProject.dashboard', {
                title: 'Mapping Project Dashboard',
                url: '/d',
                controller: 'mappingProjectDashboardController',
                templateUrl: 'app/mappingProject/mappingProjectDashboardView.html',
                controllerAs: 'mappingProjectDashboardViewModel',
                data: {
                    roles: ['user']
                }
            })
            .state('mappingProject.mappingSummary', {
                title: 'Mapping Project Mapping Summary',
                url: '/s',
                controller: 'mappingProjectMappingSummaryController',
                templateUrl: 'app/mappingProject/mappingProjectMappingSummaryView.html',
                controllerAs: 'mappingProjecMappingSummaryViewModel',
                data: {
                    roles: ['user']
                }
            })
            .state('mappingProject.reports', {
                title: 'Mapping Project Reports',
                url: '/r',
                controller: 'mappingProjectReportsController',
                templateUrl: 'app/mappingProject/mappingProjectReportsView.html',
                controllerAs: 'mappingProjectReportsViewModel',
                data: {
                    roles: ['user']
                }
            })
            .state('mappingProject.actions', {
                title: 'Mapping Project Functions',
                url: '/f',
                controller: 'mappingProjectActionsController',
                templateUrl: 'app/mappingProject/mappingProjectActionsView.html',
                controllerAs: 'mappingProjectActionsViewModel',
                data: {
                    roles: ['user']
                }
            })
            .state('mappingProject.reviewQueue', {
                title: 'Mapping Project Queue',
                url: '/q?filter',
                controller: 'mappingProjectReviewQueueController',
                templateUrl: 'app/mappingProject/mappingProjectReviewQueueView.html',
                controllerAs: 'mappingProjectReviewQueueViewModel',
                data: {
                    roles: ['user']
                }
            })
            .state('dataStandard', {
                url: '/m/:id',
                templateUrl: 'app/dataStandard/dataStandardDetailView.html',
                controller: 'dataStandardDetailController',
                controllerAs: 'dataStandardDetailViewModel',
                data: {
                    roles: ['user']
                }
            })
            .state('dataStandard.info', {
                title: 'Data Standard Info',
                url: '/i',
                templateUrl: 'app/dataStandard/dataStandardInfoView.html',
                controller: 'dataStandardInfoController',
                controllerAs: 'dataStandardInfoViewModel',
                data: {
                    roles: ['user']
                }
            })
            .state('dataStandard.sourceProjects', {
                title: 'Data Standard Source Projects',
                url: '/s',
                templateUrl: 'app/dataStandard/dataStandardSourceView.html',
                controller: 'dataStandardSourceController',
                controllerAs: 'dataStandardSourceViewModel',
                data: {
                    roles: ['user']
                }
            })
            .state('dataStandard.targetProjects', {
                title: 'Data Standard Target Projects',
                url: '/t',
                templateUrl: 'app/dataStandard/dataStandardTargetView.html',
                controller: 'dataStandardTargetController',
                controllerAs: 'dataStandardTargetViewModel',
                data: {
                    roles: ['user']
                }
            })
            .state('dataStandard.elementGroups', {
                title: 'Data Standard Element Groups',
                url: '/e',
                templateUrl: 'app/dataStandard/dataStandardElementGroupsView.html',
                controller: 'dataStandardElementGroupsController',
                controllerAs: 'dataStandardElementGroupsViewModel',
                data: {
                    roles: ['user']
                }
            })
            .state('dataStandard.elementList', {
                title: 'Data Standard Element List',
                url: '/l?filter',
                templateUrl: 'app/dataStandard/dataStandardElementListView.html',
                controller: 'dataStandardElementListController',
                controllerAs: 'dataStandardElementListViewModel',
                data: {
                    roles: ['user']
                }
            })
            .state('entityDetail', {
                url: '/enm/:id?mappingProjectId&dataStandardId',
                templateUrl: 'app/entityDetail/entityDetailView.html',
                controller: 'entityDetailController',
                controllerAs: 'entityDetailViewModel',
                data: {
                    roles: ['user']
                }
            })
            .state('entityDetail.info', {
                title: 'Entity Detail Info',
                url: '/i',
                templateUrl: 'app/entityDetail/entityDetailInfoView.html',
                controller: 'entityDetailInfoController',
                controllerAs: 'entityDetailInfoViewModel',
                data: {
                    roles: ['user']
                }
            })
            .state('entityDetail.actions', {
                title: 'Data Standard Entity Actions',
                url: '/a',
                templateUrl: 'app/entityDetail/entityDetailActionsView.html',
                controller: 'entityDetailActionsController',
                controllerAs: 'entityDetailActionsViewModel',
                data: {
                    roles: ['user']
                }
            })
            .state('elementDetail', {
                url: '/el?mappingProjectId&dataStandardId&filter&resume&current&listContextId',
                templateUrl: 'app/elementDetail/elementDetailView.html',
                controller: 'elementDetailController',
                controllerAs: 'elementDetailViewModel',
                data: {
                    roles: ['user']
                }
            })
            .state('elementDetail.info', {
                title: 'Element Detail Info',
                url: '/i',
                templateUrl: 'app/elementDetail/elementDetailInfoView.html',
                controller: 'elementDetailInfoController',
                controllerAs: 'elementDetailInfoViewModel',
                data: {
                    roles: ['user']
                }
            })
            .state('elementDetail.mapping', {
                title: 'Element Detail Mapping',
                url: '/m',
                templateUrl: 'app/elementDetail/elementDetailMappingView.html',
                controller: 'elementDetailMappingController',
                controllerAs: 'elementDetailMappingViewModel',
                data: {
                    roles: ['user']
                }
            })
            .state('elementDetail.actions', {
                title: 'Element Detail Actions',
                url: '/a',
                templateUrl: 'app/elementDetail/elementDetailActionsView.html',
                controller: 'elementDetailActionsController',
                controllerAs: 'elementDetailActionsViewModel',
                data: {
                    roles: ['user']
                }
            })
            .state('elementDetail.enumerationDetails', {
                title: 'Enumeration Details',
                url: '/en',
                templateUrl: 'app/elementDetail/enumerationDetailsView.html',
                controller: 'enumerationDetailsController',
                controllerAs: 'enumerationDetailsViewModel',
                data: {
                    roles: ['user']
                }
            })
            .state('elementDetail.enumerationUsage', {
                title: 'Enumeration Usage',
                url: '/enu',
                templateUrl: 'app/elementDetail/enumerationUsageView.html',
                controller: 'enumerationUsageController',
                controllerAs: 'enumerationUsageViewModel',
                data: {
                    roles: ['user']
                }
            })
            .state('elementGroupDetail', {
                url: '/g/:id?mappingProjectId&dataStandardId',
                templateUrl: 'app/elementGroup/elementGroupDetailView.html',
                controller: 'elementGroupDetailController',
                controllerAs: 'elementGroupDetailViewModel'
            })
            .state('elementGroupDetail.info', {
                title: 'Element Group Detail Info',
                url: '/i',
                templateUrl: 'app/elementGroup/elementGroupDetailInfoView.html',
                controller: 'elementGroupDetailInfoController',
                controllerAs: 'elementGroupDetailInfoViewModel',
                data: {
                    roles: ['user']
                }
            })
            .state('elementGroupDetail.actions', {
                title: 'Element Group Detail Actions',
                url: '/i',
                templateUrl: 'app/elementGroup/elementGroupDetailActionsView.html',
                controller: 'elementGroupDetailActionsController',
                controllerAs: 'elementGroupDetailActionsViewModel',
                data: {
                    roles: ['user']
                }
            });
    }
]);