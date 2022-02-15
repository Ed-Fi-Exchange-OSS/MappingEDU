// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module aapp.modules.elements.modals.delta-form
//

var m = angular.module('app.modals.delta-form', []);

// ****************************************************************************
// Service app.modals.delta-form
//

m.factory('app.modals.delta-form', ['settings', 'services', (settings, services) => {
    return (isPrevious, delta, standard) => {

        var modal: ng.ui.bootstrap.IModalSettings = {
            backdrop: 'static',
            keyboard: false,
            windowClass: 'modal-xl',
            controller: 'app.modals.delta-form',
            templateUrl: `${settings.modalBaseUri}/DeltaForm/DeltaFormView.tpl.html`,
            resolve: {
                isPrevious: () => { return isPrevious },
                delta: () => { return delta },
                standard: () => { return standard }
            }
        };

        return services.modal.open(modal);
    }
}]);

// ****************************************************************************
// Controller app.modules.elements.modals.delta-form
//

m.controller('app.modals.delta-form', ['$', '$scope', '$uibModalInstance', 'enumerations', 'repositories', 'services', 'delta', 'isPrevious', 'standard',
    ($, $scope, $uibModalInstance: angular.ui.bootstrap.IModalServiceInstance, enumerations: IEnumerations,
        repositories: IRepositories, services: IServices, delta, isPrevious, standard) => {

        services.logger.debug('Loaded controller app.modules.elements.modals.versions');

        var table;
        $scope.type = isPrevious ? 'Previous' : 'Next';
        $scope.delta = angular.copy(delta);
        if (!$scope.delta.Segments) $scope.delta.Segments = [];
        $scope.allElementGroups = true;
        $scope.allItemTypes = true;
        $scope.standard = standard;
        $scope.showElements = false;

        $scope.itemChangeTypes = (isPrevious) ?
            services.underscore.filter(enumerations.ItemChangeType, item => (item.DisplayText.indexOf('Deleted') != 0 && item.DisplayText != '')) :
            services.underscore.filter(enumerations.ItemChangeType, item => { return item.DisplayText.indexOf('Added') != 0 && item.DisplayText !== ''; });

        repositories.elementGroup.getAll(standard.DataStandardId).then(data => {
            $scope.elementGroups = data;
        });

        function drawTable() {
            table = $('#elementListTable');
            table = table.DataTable({
                serverSide: true,
                processing: true,
                ajax: {
                    url: `api/ElementList/${standard.DataStandardId}/delta-paging`,
                    type: 'POST',
                    data: (data) => {
                        data.ElementGroups = [];
                        if ($scope.selectedElementGroups) {
                            for (var groupKey in $scope.selectedElementGroups) {
                                if ($scope.selectedElementGroups[groupKey])
                                    data.ElementGroups.push(groupKey);
                            }
                        }
                        $scope.allElementGroups = data.ElementGroups.length === 0;

                        data.ItemTypes = [];
                        if ($scope.selectedItemTypes) {
                            for (var key in $scope.selectedItemTypes) {
                                if ($scope.selectedItemTypes[key])
                                    data.ItemTypes.push(key);
                            }
                        }
                        $scope.allItemTypes = data.ItemTypes.length === 0;
                        return data;
                    },
                    dataSrc: (data) => {
                        angular.forEach(data.data, item => {
                            item.Segments = (item.PathSegments) ? item.PathSegments : [];
                            item.Segments.push(item.Element);
                        });
                        $scope.systemItems = data.data;
                        return data.data;
                    }
                },
                createdRow(row) {
                    services.compile(angular.element(row).contents())($scope);
                },
                columnDefs: [
                    {
                        targets: 0,
                        render(item, type, row) {
                            return $scope.displayItem(row.Segments);
                        }
                    },
                    {
                        targets: 1,
                        render(item, type, row) {
                            return `<span>${(row.Element.ItemTypeName == 'SubEntity') ? 'Sub Entity' : row.Element.ItemTypeName }</span>`;
                        }
                    },
                    {
                        targets: 2,
                        render(item, type, row, meta) {
                            return `<a role="button" class="btn btn-happy" ng-click="selectItem(systemItems[${meta.row}], delta)">Select</a>`;
                        }
                    }
                ]
            });
            $('.dataTables_processing').html('<div class="loading-inner"><img src= "Client/Content/Images/Loading.gif" alt= "Loading">Loading...</div>');
        }

        $scope.displayItem = (segments) => {

            if (segments.length === 0) return '';

            var html = '';
            for (var i = 0; i < ((segments.length > 5) ? 4 : segments.length - 1); i++) {
                html += `<span class="path-segment">${segments[i].Name ? segments[i].Name : segments[i].ItemName}</span>`;
                html += '<i class="fa fa-caret-right separator"></i>';
            }
            if (segments.length > 5) {
                html += '<div class="dropdown data-standard-dropdown">';
                html += '<a class="dropdown-toggle" data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false">';
                html += '<i class="fa fa-ellipsis-h"></i>';
                html += '</a>';
                html += '<ul class="dropdown-menu data-standard-path">';
                html += `<li><span class="path-segment">${segments[4].Name ? segments[4].Name : segments[4].ItemName}</span></li>`;
                for (var i = 5; i < segments.length - 1; i++) {
                    html += '<li><i class="fa fa-caret-down separator"></i>';
                    html += `<span class="path-segment">${segments[i].Name ? segments[i].Name : segments[i].ItemName}</span></li>`;
                }
                html += '</ul>';
                html += '</div>';
                html += '<i class="fa fa-caret-right separator"></i>';
            }
            html += `<span class="standard-a">${segments[segments.length - 1].Name ? segments[segments.length - 1].Name : segments[segments.length - 1].ItemName}</span>`;
            return html;
        }

        $scope.selectedElementGroups = {};
        $scope.itemTypes = enumerations.ItemType;
        $scope.selectedItemTypes = {};

        services.timeout(() => { drawTable()}, 10);

        $scope.selectItem = (item, delta) => {
            delta.Segments = item.Segments;
            if (isPrevious) delta.OldSystemItemId = item.Element.SystemItemId;
            else delta.NewSystemItemId = item.Element.SystemItemId;

            $scope.showElements = false;
        }

        $scope.clear = delta => {
            if (isPrevious) delta.OldSystemItemId = null;
            else delta.NewSystemItemId = null;

            delta.Segments = [];
        }

        $scope.clearElementGroups = () => {
            if ($scope.allElementGroups) {
                for (var key in $scope.elementGroups)
                    $scope.elementGroups[key] = false;
            }
        };

        $scope.clearItemTypes = () => {
            if ($scope.itemTypes) {
                for (var key in $scope.itemTypes)
                    $scope.itemTypes[key] = false;
            }
        };

        $scope.applyFilter = () => { table.ajax.reload(); };

        $scope.close = () => {
            $uibModalInstance.dismiss();
        }

        function GetPathSegments(systemItemNode) {
            var pathSegments = [];
            if (systemItemNode) {
                var node = systemItemNode;
                while (node !== null) {
                    pathSegments.unshift({
                        ItemName: node.ItemName,
                        SystemItemId: node.SystemItemId,
                    });
                    node = node.ParentSystemItem;
                }
            } else {
                pathSegments.push({ ItemName: 'None' });
            }

            return pathSegments;
        }

        $scope.save = delta => 
        {
            if (isPrevious) {
                if (delta.PreviousVersionId) {
                    return repositories.element.previousVersionDelta.save(delta.NewSystemItemId, delta.PreviousVersionId, delta)
                        .then(data => {
                            services.logger.success('Saved previous version delta.');
                            data.PreviousVersionId = data.SystemItemVersionDeltaId;
                            data.OldSystemItemPathSegments = GetPathSegments(data.OldSystemItem);

                            $uibModalInstance.close(data);
                        }, error => {
                            services.logger.error('Error saving previous version delta.', error.data);
                        });
                } else {
                    return repositories.element.previousVersionDelta.create(delta.NewSystemItemId, delta)
                        .then(data => {
                            services.logger.success('Created previous version delta.');
                            data.PreviousVersionId = data.SystemItemVersionDeltaId;
                            data.OldSystemItemPathSegments = GetPathSegments(data.OldSystemItem);

                            $uibModalInstance.close(data);
                        }, error => {
                            services.logger.error('Error creating previous version delta.', error.data);
                        });
                }
            } else {
                if (delta.NextVersionId) {
                    return repositories.element.nextVersionDelta.save(delta.OldSystemItemId, delta.NextVersionId, delta)
                        .then(data => {
                            services.logger.success('Saved next version delta.');
                            data.NextVersionId = data.SystemItemVersionDeltaId;
                            data.NewSystemItemPathSegments = GetPathSegments(data.NewSystemItem);

                            $uibModalInstance.close(data);
                        }, error => {
                            services.logger.error('Error saving next version delta.', error.data);
                        });
                } else {
                    return repositories.element.nextVersionDelta.create(delta.OldSystemItemId, delta)
                        .then(data => {
                            services.logger.success('Created next version delta.');
                            data.NextVersionId = data.SystemItemVersionDeltaId;
                            data.NewSystemItemPathSegments = GetPathSegments(data.NewSystemItem);

                            $uibModalInstance.close(data);
                        }, error => {
                            services.logger.error('Error creating next version delta.', error.data);
                        });
                }
            }
            
        }
    }
]);
