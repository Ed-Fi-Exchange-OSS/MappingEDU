// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.textbox-options
//

var m = angular.module('app.directives.textbox-options', []);


// ****************************************************************************
// Directive ma-textbox-options
//

m.directive('maTextboxOptions', ['$document', 'services', 'settings', ($document: ng.IDocumentService, services: IServices, settings: ISystemSettings) => ({
    restrict: 'E',
    scope: {
        showOptions: '=',
        keepOpen: '=',
        model: '=',
        onCancel: '=',
        onSave: '=',
        deleted: '=',
        clickable: '@' //give everi=ything this class that is clickable without saving
    },
    templateUrl: `${settings.directiveBaseUri}/TextboxOptions/TextboxOptions.tpl.html`,
    link(scope) {

        var showOptionsWatch = scope.$watch('showOptions', (newVal) => {
            if (newVal) {
                services.timeout(() => {
                    $document.bind('click.textbox', (event) => {
                        var element = angular.element(event.target);
                        if (scope.showOptions &&
                            !scope.keepOpen &&
                            !element.parent().hasClass('btn-textbox-options') &&
                            !element.hasClass('btn-textbox-options') &&
                            !element.hasClass('mapping-options') &&
                            !element.hasClass(scope.clickable) &&
                            element[0].tagName.toLowerCase() !== 'ma-textbox-options') {

                            scope.save();
                        }
                    });
                });
            } else {
                $document.unbind('.textbox');
            }
        });

        var deletedWatch = scope.$watch('deleted', (newVal) => {
            if (newVal) {
                $document.unbind('.textbox');

                //Clears Watches
                showOptionsWatch();
                deletedWatch();
            }
        });

        scope.save = () => {
            scope.onSave(scope.model);
        }

        scope.cancel = () => {
            scope.onCancel(scope.model);
        }
    }
})]);
