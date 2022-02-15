// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.services.breadcrumbs
//

var m = angular.module('app.services.breadcrumbs', []);


// ****************************************************************************
// Breadcrumb Interfaces 
//

interface IBreadcrumbSegment {
    sref: string;
    text: string;
    Children?: Array<IBreadcrumbSegment>;
}

interface IBreadCrumbService {
    clear(): IBreadCrumbService;
    withCurrent(): IBreadCrumbService;
    withDataStandard(dataStandard): IBreadCrumbService;
    withHome(): IBreadCrumbService;
    withUser(user): IBreadCrumbService;
    withOrganization(organization): IBreadCrumbService;
    withMappingProject(mappingProject, childState?): IBreadCrumbService;
}

// ****************************************************************************
// Service breadcrumbService
//

m.factory('breadcrumbService', ['$rootScope', '$state', ($rootScope, $state) => {

    $rootScope.links = [];

    var homeSegment = { sref: 'app.home', text: 'Home' };
    var mappingProjectSegment = <IBreadcrumbSegment>{};
    var dataStandardSegment = <IBreadcrumbSegment>{};
    var userSegment = <IBreadcrumbSegment>{};
    var organizationSegment = <IBreadcrumbSegment>{};
    var currentSegment = <IBreadcrumbSegment> {};

    function withHome() {
        $rootScope.links = [homeSegment];
        return this;
    }

    function withMappingProject(mappingProject, childState?) {
        var state = 'mappingProject';
        state += childState ? `.${childState}` : '';
        mappingProjectSegment = { sref: state + '({id:\'' + mappingProject.MappingProjectId + '\'})', text: mappingProject.ProjectName };
        mappingProjectSegment.Children = [
            { sref: `app.mapping-project.detail.review-queue({id:'${mappingProject.MappingProjectId}'})`, text: 'Queue' },
            { sref: `app.mapping-project.detail.dashboard({id:'${mappingProject.MappingProjectId}'})`, text: 'Dashboard' },
            { sref: `app.mapping-project.detail.info({id:'${mappingProject.MappingProjectId}'})`, text: 'Info' }
        ];
        $rootScope.links = [homeSegment, mappingProjectSegment];
        if (currentSegment == {})
            $rootScope.links = [homeSegment, mappingProjectSegment];
        else
            $rootScope.links = [homeSegment, mappingProjectSegment, currentSegment];
        return this;
    }

    function withDataStandard(dataStandard) {
        dataStandardSegment = { sref: `app.data-standard.edit({id:'${dataStandard.DataStandardId}'})`, text: dataStandard.SystemName + ' ' + dataStandard.SystemVersion };
        dataStandardSegment.Children = [
            { sref: `app.data-standard.edit.elements({dataStandardId:'${dataStandard.DataStandardId}'})`, text: 'Element List' },
            { sref: `app.data-standard.edit.info({dataStandardId:'${dataStandard.DataStandardId}'})`, text: 'Info' }
        ];

        if (currentSegment === {})
            $rootScope.links = [homeSegment, dataStandardSegment];
        else
            $rootScope.links = [homeSegment, dataStandardSegment, currentSegment];

        return this;
    }

    function withUser(user) {
        userSegment = { sref: `app.manage.users.edit({id:'${user.Id}'})`, text: user.FirstName + ' ' + user.LastName };
        $rootScope.links = [homeSegment, userSegment];
        return this;
    }

    function withOrganization(organization) {
        organizationSegment = { sref: `app.manage.organization.edit({id:'${organization.Id}'})`, text: organization.Name };
        $rootScope.links = [homeSegment, organizationSegment];
        return this;
    }

    function withCurrent() {
        var index = $rootScope.links.indexOf(currentSegment);
        if (index >= 0)
            $rootScope.links.splice(index, 1);
        currentSegment = { sref: $state.current.name, text: $state.current.data.title };
        $rootScope.links.push(currentSegment);
        return this;
    }

    function clear() {
        $rootScope.links = [];
        return this;
    }

    return <IBreadCrumbService> {
        withCurrent: withCurrent,
        withDataStandard: withDataStandard,
        withHome: withHome,
        withUser: withUser,
        withOrganization: withOrganization,
        withMappingProject: withMappingProject,
        clear: clear
    }

}]);
