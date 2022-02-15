// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Breadcrumb Module 
//

var m = angular.module('appCommon');


// ****************************************************************************
// Breadcrumb Interfaces 
//

interface IBreadcrumbSegment {
    sref: string;
    text: string;
    Children?: Array<IBreadcrumbSegment>;
}


// ****************************************************************************
// Breadcrumb Service 
//

m.service('breadcrumbService', ['$rootScope', '$state', ($rootScope, $state) => {

        $rootScope.links = [];

        var homeSegment = { sref: 'home', text: 'Home' };
        var mappingProjectSegment = <IBreadcrumbSegment>{};
        var dataStandardSegment = <IBreadcrumbSegment> {};
        var currentSegment = <IBreadcrumbSegment> {};

        return {
            withHome() {
                $rootScope.links = [homeSegment];
                return this;
            },
            withMappingProject(mappingProject, childState) {
                var state = 'mappingProject';
                state += childState ? `.${childState}` : '';
                mappingProjectSegment = { sref: state + '({id:\'' + mappingProject.MappingProjectId + '\'})', text: mappingProject.ProjectName };
                mappingProjectSegment.Children = [
                    { sref: `mappingProject.reviewQueue({id:'${mappingProject.MappingProjectId}'})`, text: 'Queue' },
                    { sref: `mappingProject.dashboard({id:'${mappingProject.MappingProjectId}'})`, text: 'Dashboard' },
                    { sref: `mappingProject.info({id:'${mappingProject.MappingProjectId}'})`, text: 'Info' }
                ];
                $rootScope.links = [homeSegment, mappingProjectSegment];
                if (currentSegment == {})
                    $rootScope.links = [homeSegment, mappingProjectSegment];
                else
                    $rootScope.links = [homeSegment, mappingProjectSegment, currentSegment];
                return this;
            },
            withDataStandard(dataStandard) {
                dataStandardSegment = { sref: `dataStandard({id:'${dataStandard.DataStandardId}'})`, text: dataStandard.SystemName + ' ' + dataStandard.SystemVersion };
                dataStandardSegment.Children = [
                    { sref: `dataStandard.elementList({id:'${dataStandard.DataStandardId}'})`, text: 'Element List' },
                    { sref: `dataStandard.info({id:'${dataStandard.DataStandardId}'})`, text: 'Info' }
                ];

                if (currentSegment == {})
                    $rootScope.links = [homeSegment, dataStandardSegment];
                else
                    $rootScope.links = [homeSegment, dataStandardSegment, currentSegment];
                return this;
            },
            withCurrent() {
                var index = $rootScope.links.indexOf(currentSegment);
                if (index >= 0)
                    $rootScope.links.splice(index, 1);
                currentSegment = { sref: $state.current.name, text: $state.current.title };
                $rootScope.links.push(currentSegment);
                return this;
            },
            clear() {
                $rootScope.links = [];
                return this;
            }
        }
    }
]);