// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.manage.organizations.directives.organization-form
//

var m = angular.module('app.modules.manage.organizations.directives.organization-form', []);


// ****************************************************************************
// Directive organization-form
//

m.directive('organizationForm', ['settings', (settings: ISystemSettings) => ({
    restrict: 'E',
    templateUrl: `${settings.moduleBaseUri}/manage/organizations/directives/organizationForm.tpl.html`,
    controller: 'organizationFormController',
    controllerAs: 'organizationFormViewModel',
    scope: {
        organization: '='
    }
})]);


// ****************************************************************************
// Controller organizationFormController
//

m.controller('organizationFormController', ['$scope', '$state', 'repositories', 'services', 'settings',
    function ($scope, $state, repositories: IRepositories, services: IServices, settings: ISystemSettings) {

        services.logger.debug('Loading organizationFormController.');

        var model = this;

        model.organization = angular.copy($scope.organization);

        model.save = () => {

            if (model.organization.StringDomains)
                model.organization.Domains = model.organization.StringDomains.split(settings.deliminator);

            if (model.organization.Id) {
                return repositories.organizations.save(model.organization).then(() => {
                    $scope.organization = angular.copy(model.organization);
                    services.logger.success('Updated organization.');
                }, (error) => {
                    services.logger.error('Error updating organization.', error.data);
                });
            } else {
                return repositories.organizations.create(model.organization).then((data) => {
                    services.logger.success('Created organization.');
                    if (!$scope.organization) $state.go('app.manage.organizations', {}, {reload: true});
                }, (error) => {
                    services.logger.error('Error creating organization.', error.data);
                });
            }
        }

        model.cancel = () => {
            if (!$scope.organization) $state.go('app.manage.organizations');
            model.organization = angular.copy($scope.organization);
        }
    }
]);