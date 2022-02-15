// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modals.welcome
//

var m = angular.module('app.modals.welcome', []);

// ****************************************************************************
// Service app.modals.welcome
//

m.factory('app.modals.welcome', ['settings', 'services', (settings, services) => {
    return ()  => {

        var modal: ng.ui.bootstrap.IModalSettings = {
            backdrop: 'static',
            keyboard: false,
            size: 'lg',
            controller: 'app.modals.welcome',
            templateUrl: `${settings.modalBaseUri}/Welcome/WelcomeView.tpl.html`
        }

        return services.modal.open(modal);
    }
}]);

// ****************************************************************************
// Controller app.modals.share
//

m.controller('app.modals.welcome', ['$scope', '$uibModalInstance', 'repositories', 'services',
    ($scope, $uibModalInstance: angular.ui.bootstrap.IModalServiceInstance, repositories: IRepositories, services: IServices) => {

        services.logger.debug('Loaded controller app.modals.welcome');

        $scope.update = () => {
            repositories.users.saveMe({ FirstName: $scope.firstName, LastName: $scope.lastName }).then(() => {
                services.profile.update().then(() => {
                    $uibModalInstance.dismiss();
                });
            });
        };

    }
]);
