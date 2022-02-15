// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modals.share
//

var m = angular.module('app.modals.share', []);

// ****************************************************************************
// Service app.modals.share
//

m.factory('app.modals.share', ['settings', 'services', (settings, services) => {
    return (service, type, modelId)  => {

        var modal: ng.ui.bootstrap.IModalSettings = {
            backdrop: 'static',
            keyboard: false,
            size: 'lg',
            controller: 'app.modals.share',
            templateUrl: `${settings.modalBaseUri}/Share/ShareView.tpl.html`,
            resolve: {
                service: () => { return service },
                modelType: () => { return type },
                modelId: () => { return modelId }
            }
        }
        return services.modal.open(modal);
    }
}]);

// ****************************************************************************
// Controller app.modals.share
//

m.controller('app.modals.share', ['$', '$scope', '$uibModalInstance', 'modelType', 'service', 'modelId', 'repositories', 'services',
    ($, $scope, $uibModalInstance: angular.ui.bootstrap.IModalServiceInstance, modelType, service, modelId, repositories: IRepositories, services: IServices) => {

        services.logger.debug('Loaded controller app.modals.share');

        $scope.usersByEmail = [];

        $scope.selected = {
            Users: []
        };

        $scope.type = modelType;

        $scope.permissonOptions = [
            {
                value: 1,
                label: 'Can View'
            },
            {
                value: 2,
                label: 'Can Edit'
            },
            {
                value: 99,
                label: 'Owner'
            }
        ];

        $scope.setValid = () => {
            if ($scope.selected.Users.length > 0)
                $scope.emailValid = true;
        }

        $scope.isEmail = (text, form) => {

            if (!$scope.selected.Users) $scope.selected.Users = [];

            if (!text || text === '') {
                $scope.emailValid = true;
                return;
            }

            form.users.$setDirty();

            if ($scope.selected.Users.map(x => x.Email).indexOf(text) < 0) {
                $scope.searchingForEmail = true;
                repositories.users.checkExistsByEmail(text).then((data) => {
                    $scope.emailValid = true;
                    $scope.usersByEmail.push(data);
                }, error => {
                    $scope.emailValid = false;
                }).finally(() => {
                    $scope.searchingForEmail = false;
                });
            } else {
                $scope.emailValid = false;
            }
        }

        $scope.share = (selected, form) => {

            var shares = [];
            var notShared = [];

            if (selected.Users.length === 0)
                return undefined;

            var deferred = services.q.defer();
            var numOfUsers = selected.Users.length;
            var i = 0;

            var errors = 0;
            var successes = 0;

            angular.forEach(selected.Users, user => {

                var share = {
                    Email: user.Email,
                    Role: selected.Role
                }

                service.addUser(modelId, share).then((data) => {
                    successes++;
                    shares.push(data);     
                }, error => {
                    errors++;
                    services.logger.error(`Error sharing to ${share.Email}.`);
                    notShared.push(user.Email);
                }).finally(() => {
                    i++;
                    if (errors === 0)
                        services.logger.success(`Shared to user${(successes > 1) ? 's' : ''}.`);
                    else if (successes > 0 && errors > 0)
                        services.logger.warning(`Error sharing to all user${(numOfUsers > 1) ? 's' : ''}.`);

                    if (numOfUsers === i) {
                        deferred.resolve();
                        $uibModalInstance.close(shares);
                    }
                });

                return deferred;
            });

            return deferred.promise;
        }
       
        $scope.close = () => {
            $uibModalInstance.dismiss();
        }

        repositories.systemConstant.find('Terms of Use').then((data) => {
            $scope.termsOfUse = data.Value;
            services.timeout(() => {
                $('#termsOfUse').perfectScrollbar();
            }, 100);
        });
    }
]);
