// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.manage.users.directives.user-form
//

var m = angular.module('app.modules.manage.users.directives.user-form', []);


// ****************************************************************************
// Directive user-form
//

m.directive('userForm', ['settings', (settings: ISystemSettings) => ({
    restrict: 'E',
    templateUrl: `${settings.moduleBaseUri}/manage/users/directives/userForm.tpl.html`,
    controller: 'userFormController',
    controllerAs: 'userFormViewModel',
    scope: {
        user: '='
    }
})]);


// ****************************************************************************
// Controller userFormController
//

m.controller('userFormController', ['$scope', '$state', 'repositories', 'services',
    function ($scope, $state, repositories: IRepositories, services: IServices) {

        services.logger.debug('Loading userFormController.');

        var model = this;

        model.isCurrentUser = false;

        model.user = angular.copy($scope.user);
        model.currentEmail = ($scope.user) ? angular.copy($scope.user.Email) : null;

        services.profile.me().then((data) => {
            if (model.user && data.Id === model.user.Id) model.isCurrentUser = true;
        });

        if (!model.user) model.user = { IsAdmin: false };

        model.save = () => {
            model.user.UserName = model.user.Email;
            if (model.user.Id) {
                return repositories.users.save(model.user).then(() => {
                    $scope.user = angular.copy(model.user);
                    services.logger.success('Updated user.');
                }, (error) => {
                    services.logger.error('Error updating user.', error.data);
                });
            } else {
                return repositories.users.create(model.user).then(() => {
                    services.logger.success('Created user.');
                    $state.go('app.manage.users', {}, {reload: true});
                }, (error) => {
                    services.logger.error('Error creating users.', error.data);
                });
            }
        };

        model.cancel = () => {
            if (!model.user.Id) $state.go('app.manage.users');
            model.user = angular.copy($scope.user);
        };

//        model.emailIsAvailable = repositories.users.checkExistsByEmail;
    }
]);
