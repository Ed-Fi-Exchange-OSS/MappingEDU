// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.mapping-project.detail.mapping-summary.detail

var m = angular.module('app.modules.mapping-project.detail.mapping-summary.detail', []);

m.directive('detailSummary', ['settings', (settings: ISystemSettings) => {
    return {
        restrict: 'E',
        templateUrl: `${settings.moduleBaseUri}/MappingProject/Detail/MappingSummary/Summaries/DetailSummary/DetailSummaryView.tpl.html`,
        scope: {
            summaryDetail: '=',
            itemType: '=',
            loading: '=',
            pieChart: '='
        },
        controller: 'app.modules.mapping-project.detail.mapping-summary.detail as vm'
    }
}]);

m.controller('app.modules.mapping-project.detail.mapping-summary.detail', ['$scope', '$stateParams', 'repositories', 'services', 'mappingSummaryService',
    function ($scope, $stateParams, repositories: IRepositories, services: IServices, mappingSummaryService: IMappingSummaryService) {

        services.logger.debug('Loaded contoller app.modules.mapping-project.detail.mapping-summary.detail');

        var vm = this;
        vm.hover = null;
        vm.id = $stateParams.id;

        vm.statusColors = ['#67EFFF', '#4ECCED', '#38B5E6', '#2095D2', '#0976BD'];
        vm.methodColors = ['#68C9B7', '#61BCAB', '#54A294', '#407C71', '#1F3C37'];

        vm.methodLabels = ['Unmapped', 'Mapped', null, 'Extended', 'Omitted'];
        vm.methodLabelsWithoutInclusion = ['Unmapped', 'Mapped', 'Extended', 'Omitted'];
        vm.statusLabels = ['Unmapped', 'Incomplete', 'Completed', 'Reviewed', 'Approved'];

        vm.options = { tooltipTemplate: '<%=label%>: <%= value %> (<%= Math.round(circumference / 6.283 * 100) %>%)' };

        vm.summaryDetail = $scope.summaryDetail;

        vm.changePieChart = (pieChart) => {

            var summaryFilter = services.session.cloneFromSession('mappingSummary', vm.id);
            if (!summaryFilter) summaryFilter = {};

            summaryFilter.pieChart = pieChart;
            $scope.pieChart = pieChart;

            services.session.cloneToSession('mappingSummary', vm.id, summaryFilter);
        }
    }
]);
