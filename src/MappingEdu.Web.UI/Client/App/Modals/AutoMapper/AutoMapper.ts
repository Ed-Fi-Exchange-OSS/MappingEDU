// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.modal.auto-mapper
//

var m = angular.module('app.modals.auto-mapper', []);

// ****************************************************************************
// Service app.modals.auto-mapper
//

m.factory('app.modals.auto-mapper', ['settings', 'services', (settings, services) => {
    return (project) => {

        var modal: ng.ui.bootstrap.IModalSettings = {
            backdrop: 'static',
            keyboard: false,
            templateUrl: `${settings.modalBaseUri}/AutoMapper/AutoMapper.tpl.html`,
            controller: 'app.modals.auto-mapper',
            size: 'lg',
            windowClass: 'large-modal',
            resolve: {
                mappingProject: () => { return project; }
            }
        }
        return services.modal.open(modal);
    }
}]);


// ****************************************************************************
// Controller app.modal.auto-mapper
//

m.controller('app.modals.auto-mapper', [
    '$', '$scope', '$rootScope', '$uibModalInstance', 'repositories', 'services', 'enumerations', 'mappingProject',
    ($, $scope, $rootScope, $uibModalInstance: angular.ui.bootstrap.IModalServiceInstance, repositories: IRepositories,
        services: IServices, enumerations: IEnumerations, mappingProject) => {

        services.logger.debug('Loaded controller app.modules.mapping-project.create.auto-mapper');

        $scope.state = 1;
        $scope.mappingProject = mappingProject;
        $scope.autoMapResults = [];

        $scope.transitiveStandards = services.underscore.groupBy(services.underscore.where($scope.suggestions, x => x.SuggestionType.Id === 1), 'TransitiveStandardId');

        $scope.getAutoMapResults = () => {
            $scope.state = 2;
            $rootScope.progress.start();
            $rootScope.progress.setHeight("5px");
            repositories.autoMap.mappingSuggestions(mappingProject, mappingProject.MappingProjectId).then(data => {
                angular.forEach(data, (autoMap) => {
                    if (!autoMap.SourceSystemItem) autoMap.SourceSystemItem = {
                        DomainItemPath: ''
                    };
                    if (!autoMap.TargetSystemItem) autoMap.TargetSystemItem = {
                        DomainItemPath: ''
                    };
                    autoMap.SourceSystemItem.PathSegments = (autoMap.SourceSystemItem.DomainItemPath) ? autoMap.SourceSystemItem.DomainItemPath.split('.') : [];
                    autoMap.TargetSystemItem.PathSegments = (autoMap.TargetSystemItem.DomainItemPath) ? autoMap.TargetSystemItem.DomainItemPath.split('.') : [];
                    if (autoMap.CommonSystemItem !== null) {
                        autoMap.CommonSystemItem.PathSegments = autoMap.CommonSystemItem.DomainItemPath.split('.');
                    }
                });
                $scope.autoMapperResults = data;
                angular.forEach($scope.autoMapResults, (result) => {
                    result.MappingMethodTypeId = 1;
                });
                $scope.draw(data);
                $scope.state = 3;
            }).finally(() => {
                $rootScope.progress.complete();
                $rootScope.progress.reset();
            });
        }

        $scope.draw = (autoMapperResults) => {
            $scope.table = $('#autoMapperTable').DataTable(
                {
                    data: autoMapperResults,
                    deferRender: true,
                    columns: [
                        {
                            data: 'SourceSystemItem.DomainItemPath',
                            visible: false,
                            searchable: true
                        },
                        {
                            data: 'BusinessLogic',
                            visible: false,
                            searchable: true
                        },
                        {
                            data: 'Reason.Name',
                            visible: false,
                            searchable: true
                        },
                        {
                            data: null
                        },
                        {
                            data: null
                        },
                        {
                            data: null
                        }
                    ],
                    columnDefs: [
                        {
                            targets: 3,
                            render(map, type, row, meta) {
                                var source = map.SourceSystemItem;
                                var html = '<span style="font-size: 13px">';
                                var pathLength = source.PathSegments.length;
                                var totalLength = 400 - (8.75 * source.PathSegments[pathLength - 1].length);
                                var ellipsis = false;
                                angular.forEach(source.PathSegments, (path, index) => {
                                    if (index + 1 < pathLength) {
                                        totalLength -= (path.length * 8.75);
                                        if (totalLength > 0) {
                                            html += `<span class="path-segment"><span style="font-size: 13px" >${path}</span></span>`;
                                            html += '<i class="fa fa-caret-right separator"></i>';
                                        }
                                        else {
                                            if (!ellipsis) {
                                                html += '<span>';
                                                html += '<div class="dropdown data-standard-dropdown">';
                                                html += '<a class="dropdown-toggle" data-toggle="dropdown" role="button" ';
                                                html += 'aria-haspopup="true" aria-expanded="false">';
                                                html += '<i class="fa fa-ellipsis-h" style="margin-left: 0px"></i></a>';
                                                html += '<ul class="dropdown-menu data-standard-path">';
                                                ellipsis = true;
                                            }
                                            html += `<li>&nbsp;&nbsp;<span class="path-segment"><span style="font-size: 13px" >${path}</span></span></li>`;

                                            if (index + 2 == pathLength) {
                                                html += '</ul></div></span>'
                                                html += '<i class="fa fa-caret-right separator"></i>';
                                            }
                                        }
                                    }
                                    else html += `<span class="standard-a">${path}</span>`;
                                });
                                html += '</span>'
                                return html;
                            }
                        },
                        {
                            targets: 4,
                            render(map, type, row, meta) {
                                var html = '<div style="font-size: 13px" >';
                                if (map.CommonSystemItem) {
                                    html = 'Similar mapping to: </br>';
                                    html += ` <span class="path-segment-c">${map.CommonDataStandard.SystemName} (${map.CommonDataStandard.SystemVersion})</span>`;
                                    html += '<i class="fa fa-caret-right separator"></i>';
                                    var pathLength = map.CommonSystemItem.PathSegments.length;
                                    var totalLength = 150 - (8 * (map.CommonSystemItem.PathSegments[pathLength - 1].length + map.CommonDataStandard.SystemName.length + map.CommonDataStandard.SystemVersion.length));
                                    var ellipsis = false;
                                    angular.forEach(map.CommonSystemItem.PathSegments, (path, index) => {
                                        if (index + 1 < pathLength) {
                                            totalLength -= (path.length * 8);
                                            if (totalLength > 0) {
                                                html += `<span style="font-size: 13px" class="path-segment-c"><span style="font-size: 13px">${path}</span></span>`;
                                                html += '<i class="fa fa-caret-right separator"></i>';
                                            }
                                            else {
                                                if (!ellipsis) {
                                                    html += '<span>';
                                                    html += '<div class="dropdown data-standard-dropdown-c">';
                                                    html += '<a class="dropdown-toggle" data-toggle="dropdown" role="button" ';
                                                    html += 'aria-haspopup="true" aria-expanded="false">';
                                                    html += '<i style="color: rgb(97, 188, 171)" class="fa fa-ellipsis-h" style="margin-left: 0px"></i></a>';
                                                    html += '<ul class="dropdown-menu data-standard-path">';
                                                    ellipsis = true;
                                                }
                                                html += `<li>&nbsp;&nbsp;<span style="font-size: 13px" class="path-segment-c"><span style="font-size: 13px" >${path}</span></span></li>`;

                                                if (index + 2 == pathLength) {
                                                    html += '</ul></div></span>'
                                                    html += '<i class="fa fa-caret-right separator"></i>';
                                                }
                                            }
                                        }
                                        else html += `<span style="font-size: 13px" class="standard-c">${path}</span>`;
                                    });
                                } else {
                                    if (map.PreviousSourceDataStandard && map.PreviousTargetDataStandard) {
                                        html += `<span>Similar mapping between:</span>`;
                                        html += `<span style="font-size: 13px" class="standard-c">${map.PreviousSourceDataStandard.SystemName} (${map.PreviousSourceDataStandard.SystemVersion})</span>`;
                                        html += '&nbsp;<i class="fa fa-caret-right separator"></i>';
                                        html += `<span style="font-size: 13px" class="standard-c">${map.PreviousTargetDataStandard.SystemName} (${map.PreviousTargetDataStandard.SystemVersion})</span>.`;
                                    }
                                    else if (map.PreviousSourceDataStandard) {
                                        html += `<span>Similar mapping between:</span>`;
                                        html += `<span style="font-size: 13px" class="standard-c">${map.PreviousSourceDataStandard.SystemName} (${map.PreviousSourceDataStandard.SystemVersion})</span>`;
                                        html += '&nbsp;<i class="fa fa-caret-right separator"></i>';
                                        html += `<span style="font-size: 13px" class="standard-c">${$scope.mappingProject.TargetDataStandard.SystemName} (${$scope.mappingProject.TargetDataStandard.SystemVersion})</span>.`;
                                    }
                                    else if (map.PreviousTargetDataStandard) {
                                        html += `<span>Similar mapping between:</span>`;
                                        html += `<span style="font-size: 13px" class="standard-c">${$scope.mappingProject.SourceDataStandard.SystemName} (${$scope.mappingProject.SourceDataStandard.SystemVersion})</span>`;
                                        html += '&nbsp;<i class="fa fa-caret-right separator"></i>';
                                        html += `<span style="font-size: 13px" class="standard-c">${map.PreviousTargetDataStandard.SystemName} (${map.PreviousTargetDataStandard.SystemVersion})</span>.`;
                                    }
                                    else {
                                        html += `<span style="font-size: 13px" class="standard-c">${map.Reason.Name}</span>`;
                                    }
                                }
                                html += '</div>';
                                return html;
                            }

                        },
                        {
                            targets: 5,
                            render(map, type, row, meta) {
                                var html = '<div style="font-size: 13px">';
                                if (map.MappingMethod.Id === 1) { //Enter Business Logic
                                    var businessLogic = map.BusinessLogic;
                                    if (businessLogic && businessLogic.indexOf('[') > -1 && businessLogic.indexOf(']') > -1) {
                                        businessLogic = businessLogic.split('[').join('<span class="standard-b">');
                                        businessLogic = businessLogic.split(']').join('</span>');
                                    }
                                    if(businessLogic)
                                        html += businessLogic;
                                    if (html.indexOf('\r\n') > -1)
                                        html = html.split('\r\n').join('<br/>');
                                }
                                else if (map.MappingMethod.Id === 3) { //Mark for Extension
                                    html += 'Mark For Extension';
                                }
                                else if (map.MappingMethod.Id === 4) { //Mark for Omission
                                    html += 'Mark for Omission';
                                    if (map.OmissionReason) {
                                        html += '<br/>';
                                        html += map.OmissionReason;
                                    }
                                }
                                html += '</div>';
                                return html;
                            }
                        }
                    ]
                });
        }



        $scope.close = () => {
            $uibModalInstance.dismiss();
        }

        $scope.create = (autoMap) => {
            $rootScope.progress.start();
            return repositories.mappingProject.create(mappingProject, autoMap).then(
                data => {
                    services.logger.success('Created mapping project.');
                    $uibModalInstance.close(data.MappingProjectId);
                }, error => {
                    services.logger.error('Error creating mapping project.', error.data);
                }).finally(() => {
                    $rootScope.progress.complete();
                    $rootScope.progress.reset();
            });
        }

        $scope.addResults = () => {
            $rootScope.progress.start();
            return repositories.autoMap.addResults(mappingProject).then(() => {
                $uibModalInstance.close();
                services.logger.success('Added auto mappings to mapping project.');
            }, error => {
                services.logger.error('Error adding auto mappings to mapping project.', error.data);
            }).finally(() => {
                    $rootScope.progress.complete();
                    $rootScope.progress.reset();
                });
        }
    }
]);
