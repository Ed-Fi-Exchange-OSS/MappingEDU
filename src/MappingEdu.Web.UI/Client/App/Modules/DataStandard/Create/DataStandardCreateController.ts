// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.data-standard.create
//

var m = angular.module('app.modules.data-standard.create', []);


// ****************************************************************************
// Configure 
//

m.config(['$stateProvider', 'settings', ($stateProvider: ng.ui.IStateProvider, settings: ISystemSettings) => {

    $stateProvider
        .state('app.data-standard.create', {
            url: '/create',
            data: {
                roles: ['user'],
                title: 'Create Data Standard'
            },
            views: {
                'content@': {
                    templateUrl: `${settings.moduleBaseUri}/DataStandard/Create/DataStandardCreateView.tpl.html`,
                    controller: 'app.modules.data-standard.create',
                    controllerAs: 'dataStandardViewModel'
                }
            }
        });
}]);

// ****************************************************************************
// Controller app.modules.data-standard.create
//

m.controller('app.modules.data-standard.create', ['$scope', 'services', 'repositories',
    function ($scope, services: IServices, repositories: IRepositories) {

       services.logger.debug('Loaded controller app.modules.data-standard.create');

       var vm = this;

       if(!$scope.standard) $scope.standard = {};

       repositories.dataStandard.getAllWithoutNextVersions().then(standards => {

           vm.dataStandards = standards;
           if ($scope.standard.PreviousDataStandard)
               vm.dataStandards.push($scope.standard.PreviousDataStandard);

       }, error => {
            services.logger.error('Error loading data standards.', error.data);
        });

        vm.name = 'dataStandardController';

        vm.cancel = dataStandard => {
            $scope.$broadcast('show-errors-reset');
            if (!_.isUndefined(dataStandard) && !_.isUndefined(dataStandard.DataStandardId)) {
                var element = <any> angular.element(document.querySelector('#editDataStandardModal')); // TODO: Cast correctly (cpt)
                element.modal('hide');
            } else
                services.state.go('app.home');
        };
        
        vm.save = dataStandard => {
         
            $scope.$broadcast('show-errors-check-valid');
            
            if (_.isUndefined(dataStandard) || $scope.dataStandardForm.$invalid)
                return undefined;

            var existing = _.find(this.dataStandards, (standard: any) => (angular.equals(standard.SystemName.toLowerCase(), dataStandard.SystemName.toLowerCase()) &&
                angular.equals(standard.SystemVersion.toLowerCase(), dataStandard.SystemVersion.toLowerCase()) &&
                (_.isUndefined(dataStandard.DataStandardId) || !angular.equals(standard.DataStandardId, dataStandard.DataStandardId))));

            if (existing) {
                services.errors.addError('Data Standard Name and Version must be unique.', this);
                return undefined;
            }

            if (!_.isUndefined(dataStandard.DataStandardId)) {
                repositories.dataStandard.save(dataStandard)
                    .then(() => {
                        services.logger.success('Saved data standard.');
                        this.success = true;
                        var element = <any>angular.element(document.querySelector('#editDataStandardModal')); // TODO: Cast correctly (cpt)
                        element.modal('hide');
                    })
                    .catch(error => {
                            services.errors.handleErrors(error, this);
                        }
                    );
            } else {
                repositories.dataStandard.create(dataStandard)
                    .then(data => {
                        services.logger.success('Created data standard.');
                        services.profile.clearProfile();
                        this.success = true;
                        services.state.go('app.data-standard.edit.groups', { dataStandardId: data.DataStandardId });
                    })
                    .catch(error => {
                        services.errors.handleErrors(error, this);
                    }
                );
            }
            return this;
        };

        vm.delete = dataStandard => {
            repositories.dataStandard.remove(dataStandard.DataStandardId)
                .then(() => {
                    services.logger.success('Deleted data standard.');
                    vm.success = true;
                    services.timeout(() => { vm.success = false; }, 2000);
                })
                .catch(error => {
                        services.errors.handleErrors(error, vm);
                    }
                );
        };
    }
]);