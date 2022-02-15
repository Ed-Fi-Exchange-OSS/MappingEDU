﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.manage.system-constants.text-constant
//

var m = angular.module('app.modules.manage.system-constants.constant-boolean', []);


// ****************************************************************************
// Directive app.modules.manage.system-constants
//

m.directive('constantBoolean', ['settings', 'services', 'repositories', '$compile',
    (settings, services: IServices, repositories: IRepositories, $compile) => {
    return {
        restrict: 'A',
        scope: {
            constant: '=',
            index: '='
        },
        link: (scope, element, attrs) => {

            var template = '<tr>' +
                '<td>{{constant.Name}}</td>' +
                '<td ng-if="!constant.edit">' +
                     '<input type="checkbox" ng-model="constant.BooleanValue" style="height: 30px; width: 30px;" disabled/>' +
                '</td>' +
                '<td ng-if="constant.edit">' +
                     '<input type="checkbox" ng-model="constant.TempValue" style="height: 30px; width: 30px;" />' +
                '</td>' +
                '<td ng-if="!constant.edit" class="text-center">' +
                    '<button class="btn btn-edit" ng-click="edit(constant)"><i class="fa"></i> Edit</button>' +
                '</td>' +
                '<td ng-if="constant.edit" class="text-center">' +
                    '<div class="pull-right">' +
                        '<button class="btn btn-save" ng-click="save(constant)"><i class="fa"></i> Save</button>' +
                        '<button class="btn btn-cancel" ng-click="constant.edit = false"><i class="fa"></i> Cancel</button>' +
                     '</div>' +
                '</td>' +
                '</tr>';

            var templateElement = angular.element(template);
            element.replaceWith(templateElement);
            $compile(templateElement)(scope);

            scope.edit = constant => {
                constant.TempValue = angular.copy(constant.BooleanValue);
                constant.edit = true;
            }

            scope.save = constant => {
                constant.BooleanValue = angular.copy(constant.TempValue);
                repositories.systemConstant.save(constant).then(() => {
                    constant.edit = false;
                    constant.Value = (constant.BooleanValue) ? 'True' : 'False';
                    services.logger.success(`Updated ${constant.Name}.`);
                }, error => {
                    services.logger.error(`Error updating ${constant.Name}.`, error);
                });
            }

        }
    }
}])