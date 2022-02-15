// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modules.layout
//

var m = angular.module('app.modules.layout', ['app.layout.breadcrumb']);


// ****************************************************************************
// Controller app.layout.navbar
//

m.controller('app.layout.navbar', [
    '$rootScope', '$scope', 'security', 'services', 'repositories', 'settings',
    ($rootScope: any, $scope: any, security: ISecurity, services: IServices, repositories: IRepositories, settings: ISystemSettings) => {

    services.logger.debug('Loading app.layout.navbar controller');

    $scope.$state = services.state;
    $scope.links = $rootScope.links;
    $scope.docsUri = settings.docsBaseUri;

    $scope.home = { sref: 'app.home', text: 'Home' };

    $rootScope.$watch('links', () => {
        $scope.links = $rootScope.links;
    });

    security.principal.identity().then(data => {
        if (data) {

            services.profile.me().then(me => {
                $scope.isAdmin = me.IsAdministrator;
                $scope.isGuest = me.IsGuest;
            });

            repositories.systemConstant.find('Documentation Site').then(data => {
                $scope.helpUrl = data.Value;
            }, error => {
                services.logger.error('Error retrieving documentation site url.', error.data);
                });

            repositories.systemConstant.find('Terms of Use Url').then(data => {
                $scope.termsOfUseUrl = data.Value;
            }, error => {
                services.logger.error('Error retrieving terms of use site url.', error.data);
            });
        }
    });

    $scope.manageHref = () => {
        return services.state.href('app.manage.users');
    }
     
    $scope.toggleSidebar = () => {
        services.logger.debug('toggling sidebar');
        services.events.emit('event:sidebar-toggle');
    };

    $scope.signout = () => {
        services.logging.add({
            Source: 'app.layout',
            Message: 'User logged out'
        });
        security.principal.logout();
        services.profile.clearProfile();
        security.principal.identity().then(() => {
            services.state.transitionTo('app.home', null, { reload: true });
        });
    };

    $scope.dashboardHref = id => {
        return services.state.href('app.mapping-project.detail.dashboard', { id: id });
    }

    $scope.summaryHref = id => {
        return services.state.href('app.mapping-project.detail.mapping-summary', { id: id });
    }

    $scope.projectInfoHref = id => {
        return services.state.href('app.mapping-project.detail.info', { id: id });
    }

    $scope.reviewQueueHref = id => {
        return services.state.href('app.mapping-project.detail.review-queue', { id: id });
    }

    $scope.projectListHref = () => {
        return services.state.href('app.mapping-project.list');
    }

    $scope.standardInfoHref = id => {
        return services.state.href('app.data-standard.edit.info', { dataStandardId: id });
    }

    $scope.elementListHref = id => {
        return services.state.href('app.data-standard.edit.elements', { dataStandardId: id });
    }

    $scope.standardListHref = () => {
        return services.state.href('app.data-standard.list');
    }

    $scope.browseHref = (id) => {
        return services.state.href('app.data-standard.edit.browse', { dataStandardId: id });
    }
}]);

// ****************************************************************************
// Controller layout.footer 
//

m.controller('app.layout.footer', ['$scope', '$state', 'security', 'services', ($scope: any, $state: ng.ui.IState, security: ISecurity, services: IServices) => {
    services.logger.debug('Loading controller app.layout.footer');
}]);
