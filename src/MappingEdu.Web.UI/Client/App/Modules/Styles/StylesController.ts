// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.styles-demo
//

var m = angular.module('app.modules.styles-demo', ['app.modules.styles-demo.service']);

// ****************************************************************************
// Configure 
//

m.config(['$urlRouterProvider', '$stateProvider', 'settings', ($urlRouterProvider: ng.ui.IUrlRouterProvider, $stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $stateProvider
        .state('app.styles', {
            url: '/styles',
            data: {
                title: 'Styles',
                roles: ['admin']
            },
            views: {
                'content@': {
                    templateUrl: `${settings.moduleBaseUri}/styles/stylesView.tpl.html`,
                    controller: 'app.modules.styles-demo',
                    controllerAs: 'stylesViewModel'
                }
            }
        });
}]);


// ****************************************************************************
// Controller stylesController
//

m.controller('app.modules.styles-demo', ['stylesService', 'services',
    function (stylesService, services: IServices) {

    services.logger.debug('Loaded controller app.modules.styles-demo');

    var stylesViewModel = this;

    var h1 = $('h1');
    stylesViewModel.h1Styles =
        [
            'font-family: ' + h1.css('font-family'),
            'font-weight: ' + h1.css('font-weight'),
            'font-size: ' + h1.css('font-size'),
            'color: ' + h1.css('color')
        ];

    var h2 = $('h2');
    stylesViewModel.h2Styles =
        [
            'font-family: ' + h2.css('font-family'),
            'font-weight: ' + h2.css('font-weight'),
            'font-size: ' + h2.css('font-size'),
            'color: ' + h2.css('color')
        ];

    var h3 = $('h3');
    stylesViewModel.h3Styles =
        [
            'font-family: ' + h3.css('font-family'),
            'font-weight: ' + h3.css('font-weight'),
            'font-size: ' + h3.css('font-size'),
            'color: ' + h3.css('color')
        ];

    var a = $('a');
    stylesViewModel.aStyles =
        [
            'font-family: ' + a.css('font-family'),
            'font-weight: ' + a.css('font-weight'),
            'font-size: ' + a.css('font-size'),
            'color: ' + a.css('color')
        ];

    var bodyText = $('p.bodyText');
    stylesViewModel.bodyTextStyles =
        [
            'font-family: ' + bodyText.css('font-family'),
            'font-weight: ' + bodyText.css('font-weight'),
            'font-size: ' + bodyText.css('font-size'),
            'color: ' + bodyText.css('color')
        ];

    var pageTitle = $('p.page-title');
    stylesViewModel.pageTitleStyles =
        [
            'font-family: ' + pageTitle.css('font-family'),
            'font-weight: ' + pageTitle.css('font-weight'),
            'font-size: ' + pageTitle.css('font-size'),
            'color: ' + pageTitle.css('color'),
            'text-transform: ' + pageTitle.css('text-transform')
        ];

    var standardA = $('span.standard-a');
    stylesViewModel.standardAStyles =
        [
            'font-family: ' + standardA.css('font-family'),
            'font-weight: ' + standardA.css('font-weight'),
            'font-size: ' + standardA.css('font-size'),
            'color: ' + standardA.css('color'),
            'background-color: ' + standardA.css('background-color')
        ];

    var standardB = $('span.standard-b');
    stylesViewModel.standardBStyles =
        [
            'font-family: ' + standardB.css('font-family'),
            'font-weight: ' + standardB.css('font-weight'),
            'font-size: ' + standardB.css('font-size'),
            'color: ' + standardB.css('color'),
            'background-color: ' + standardB.css('background-color')
        ];

    var greenGood = $('p.good');
    stylesViewModel.greenGoodStyles =
        [
            'font-family: ' + greenGood.css('font-family'),
            'font-weight: ' + greenGood.css('font-weight'),
            'font-size: ' + greenGood.css('font-size'),
            'color: ' + greenGood.css('color')
        ];

    var redBad = $('p.bad');
    stylesViewModel.redBadStyles =
        [
            'font-family: ' + redBad.css('font-family'),
            'font-weight: ' + redBad.css('font-weight'),
            'font-size: ' + redBad.css('font-size'),
            'color: ' + redBad.css('color')
        ];

    var p = $('p.no-class');
    stylesViewModel.pStyles =
        [
            'font-family: ' + p.css('font-family'),
            'font-weight: ' + p.css('font-weight'),
            'font-size: ' + p.css('font-size'),
            'color: ' + p.css('color')
        ];

    var headerText = $('p.header-text');
    stylesViewModel.headerTextStyles =
        [
            'font-family: ' + headerText.css('font-family'),
            'font-weight: ' + headerText.css('font-weight'),
            'font-size: ' + headerText.css('font-size'),
            'color: ' + headerText.css('color')
        ];

    var accentText = $('p.accent');
    stylesViewModel.accentStyles =
        [
            'font-family: ' + accentText.css('font-family'),
            'font-weight: ' + accentText.css('font-weight'),
            'font-size: ' + accentText.css('font-size'),
            'color: ' + accentText.css('color')
        ];

    var button = $('p button.btn-happy');
    stylesViewModel.buttonStyles =
        [
            'font-family: ' + button.css('font-family'),
            'font-weight: ' + button.css('font-weight'),
            'font-size: ' + button.css('font-size'),
            'color: ' + button.css('color')
        ];

    var buttonCancel = $('p button.btn-cancel');
    stylesViewModel.buttonCancelStyles =
        [
            'font-family: ' + buttonCancel.css('font-family'),
            'font-weight: ' + buttonCancel.css('font-weight'),
            'font-size: ' + buttonCancel.css('font-size'),
            'color: ' + buttonCancel.css('color')
        ];

    var buttonRedirect = $('p button.btn-redirect');
    stylesViewModel.buttonRedirectStyles =
        [
            'font-family: ' + buttonRedirect.css('font-family'),
            'font-weight: ' + buttonRedirect.css('font-weight'),
            'font-size: ' + buttonRedirect.css('font-size'),
            'color: ' + buttonRedirect.css('color')
        ];

    var buttonDelete = $('p button.btn-delete');
    stylesViewModel.buttonDeleteStyles =
        [
            'font-family: ' + buttonDelete.css('font-family'),
            'font-weight: ' + buttonDelete.css('font-weight'),
            'font-size: ' + buttonDelete.css('font-size'),
            'color: ' + buttonDelete.css('color')
        ];
}
]);