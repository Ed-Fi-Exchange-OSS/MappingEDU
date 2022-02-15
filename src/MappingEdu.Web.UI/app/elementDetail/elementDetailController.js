// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appElementDetail').controller('elementDetailController', [
    '_', '$timeout', '$scope', '$state', '$stateParams', 'mappingProjectService', 'dataStandardService', 'mappingProjectElementsService',
    'elementService', 'elementDetailService', 'enumerationService', 'breadcrumbService', 'handleErrorService', 'sessionService',
    function (_, $timeout, $scope, $state, $stateParams, mappingProjectService, dataStandardService, mappingProjectElementsService,
        elementService, elementDetailService, enumerationService, breadcrumbService, handleErrorService, sessionService) {

        var elementDetailViewModel = this;
        elementDetailViewModel.name = 'elementDetailController';

        elementDetailViewModel.emptyGuid = '{00000000-0000-0000-0000-000000000000}';

        elementDetailViewModel.mappingProjectId = $stateParams.mappingProjectId || elementDetailViewModel.emptyGuid;
        elementDetailViewModel.dataStandardId = $stateParams.dataStandardId || elementDetailViewModel.emptyGuid;
        elementDetailViewModel.isMappingProject = elementDetailViewModel.mappingProjectId != elementDetailViewModel.emptyGuid;
        elementDetailViewModel.id = elementDetailViewModel.isMappingProject ?
            elementDetailViewModel.mappingProjectId : elementDetailViewModel.dataStandardId;

        elementDetailViewModel.filter = $stateParams.filter;
        elementDetailViewModel.resume = $stateParams.resume;
        elementDetailViewModel.current = $stateParams.current;
        elementDetailViewModel.listContextId = $stateParams.listContextId;

        elementDetailViewModel.controlElementPath = {};

        elementDetailViewModel.pageTitle = 'Element Detail';

        elementDetailViewModel.getMappingProjectDetails = function() {
            elementDetailViewModel.loading = true;
            mappingProjectService.get(elementDetailViewModel.mappingProjectId)
                .then(function(data) {
                    elementDetailViewModel.mappingProject = data;
                    breadcrumbService.withMappingProject(elementDetailViewModel.mappingProject, 'dashboard');
                    angular.element("#editProjectModal").on('hidden.bs.modal', function() {
                        elementDetailViewModel.getMappingProjectDetails();
                    });
                    broadcastMappingProjectChangedEvent();
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, elementDetailViewModel);
                })
                .finally(function() {
                    elementDetailViewModel.loading = false;
                });
        };

        $scope.$on('workflow-status-updated', function() {
            elementDetailViewModel.getMappingProjectDetails();
        });

        elementDetailViewModel.getDataStandardDetails = function() {
            elementDetailViewModel.loading = true;
            dataStandardService.get(elementDetailViewModel.dataStandardId)
                .then(function(data) {
                    elementDetailViewModel.dataStandard = data;
                    $scope.dataStandard = elementDetailViewModel.dataStandard;
                    breadcrumbService.withDataStandard(elementDetailViewModel.dataStandard);
                    angular.element("#editDataStandardModal").on('hidden.bs.modal', function() {
                        elementDetailViewModel.getDataStandardDetails();
                    });
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, elementDetailViewModel);
                })
                .finally(function() {
                    elementDetailViewModel.loading = false;
                });
        };

        elementDetailViewModel.getElementData = function() {
            if (_.isUndefined(elementDetailViewModel.currentElementId)) {
                elementDetailViewModel.element = undefined;
                return;
            }

            var timer = $timeout(showLoading, 100);
            if (elementDetailViewModel.isMappingProject) {
                elementService.get(elementDetailViewModel.mappingProjectId, elementDetailViewModel.currentElementId)
                    .then(function(data) {
                        elementDetailViewModel.element = data;
                        elementDetailViewModel.element.isMappingProject = elementDetailViewModel.isMappingProject;
                        sessionService.cloneToSession('elementDetail', elementDetailViewModel.mappingProjectId, elementDetailViewModel.element);
                        elementDetailViewModel.element.ElementDetails.ItemDataTypeId = elementDetailViewModel.element.ElementDetails.ItemDataType.Id;
                        reloadPath();
                        checkForEnumeration();
                        broadcastElementFetchedEvent();
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, elementDetailViewModel);
                    })
                    .finally(function() {
                        $timeout.cancel(timer);
                        elementDetailViewModel.loading = false;
                    });
            } else {
                elementService.getAll(elementDetailViewModel.currentElementId)
                    .then(function(data) {
                        elementDetailViewModel.element = data;
                        elementDetailViewModel.element.isMappingProject = elementDetailViewModel.isMappingProject;
                        sessionService.cloneToSession('elementDetail', elementDetailViewModel.dataStandardId, elementDetailViewModel.element);
                        elementDetailViewModel.element.ElementDetails.ItemDataTypeId = elementDetailViewModel.element.ElementDetails.ItemDataType.Id;
                        reloadPath();
                        checkForEnumeration();
                        broadcastElementFetchedEvent();
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, elementDetailViewModel);
                    })
                    .finally(function() {
                        $timeout.cancel(timer);
                        elementDetailViewModel.loading = false;
                    });
            }
        };

        function checkForEnumeration() {
            var found = _.find(elementDetailViewModel.tabs, function(item) {
                return item.label == 'Details';
            });
            if (elementDetailViewModel.element.ElementDetails.ItemType.Id === 5) {
                if (!found) {
                    elementDetailViewModel.tabs.push({ link: 'elementDetail.enumerationDetails', label: 'Details' });
                    elementDetailViewModel.tabs.push({ link: 'elementDetail.enumerationUsage', label: 'Usage' });
                }
                elementDetailViewModel.getEnumerationData();
            } else {
                if (found) {
                    var tabs = elementDetailViewModel.tabs;
                    var index = tabs.indexOf(found);
                    if (index > 0) {
                        if (angularApp.Utils.endsWith($state.current.name, 'enumerationDetails'))
                            $state.go('^.info');
                        tabs.splice(index, 1);
                    }
                }
            }
        }

        elementDetailViewModel.getEnumerationData = function() {
            var timer = $timeout(showLoading, 100);
            enumerationService.get(elementDetailViewModel.currentElementId)
                .then(function(data) {
                    elementDetailViewModel.enumeration = data;
                    elementDetailViewModel.enumeration.isMappingProject = elementDetailViewModel.isMappingProject;
                    sessionService.cloneToSession('enumerationDetail', elementDetailViewModel.id, elementDetailViewModel.enumeration);
                    broadcastEnumerationFetchedEvent();
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, elementDetailViewModel);
                })
                .finally(function() {
                    $timeout.cancel(timer);
                    elementDetailViewModel.loading = false;
                });
        };

        function showLoading() {
            elementDetailViewModel.loading = true;
        }

        function reloadPath() {
            if (typeof elementDetailViewModel.controlElementPath.reloadPath === "function")
                elementDetailViewModel.controlElementPath.reloadPath(elementDetailViewModel.element.ElementDetails.PathSegments, null);
        }

        function broadcastElementFetchedEvent() {
            $scope.$broadcast('element-fetched', elementDetailViewModel.element);
        }

        function broadcastEnumerationFetchedEvent() {
            $scope.$broadcast('enumeration-fetched', elementDetailViewModel.enumeration);
        }

        $scope.$on('element-changed', function() {
            elementDetailViewModel.getElementData();
        });

        elementDetailViewModel.getElementIds = function() {
            elementDetailViewModel.loading = true;
            var elementQueue = sessionService.cloneFromSession('elementQueues', elementDetailViewModel.listContextId || elementDetailViewModel.mappingProjectId);
            var elementList = sessionService.cloneFromSession('elementLists', elementDetailViewModel.listContextId || elementDetailViewModel.dataStandardId);
            elementDetailViewModel.elements = elementQueue && elementDetailViewModel.current && elementDetailViewModel.mappingProjectId != null
                ? elementQueue
                : elementList && elementDetailViewModel.current && elementDetailViewModel.dataStandardId != null
                ? elementList
                : null;

            if (elementDetailViewModel.elements == null) {
                mappingProjectElementsService.get(elementDetailViewModel.mappingProjectId, elementDetailViewModel.filter || 'AllIncomplete')
                    .then(function(data) {
                        elementDetailViewModel.elements = data;
                        clearElementDetailSessionInfo();
                        setCurrentElementInfo();
                        elementDetailViewModel.getElementData();
                    })
                    .catch(function(error) {
                        handleErrorService.handleErrors(error, elementDetailViewModel);
                    })
                    .finally(function() {
                        elementDetailViewModel.loading = false;
                    });
            } else {
                clearElementDetailSessionInfo();
                setCurrentElementInfo();
                $timeout(function() {
                    elementDetailViewModel.getElementData();
                }, 100);
                elementDetailViewModel.loading = false;
            }
        };

        function setCurrentElementInfo() {
            if (elementDetailViewModel.elements.length == 0) {
                elementDetailViewModel.currentElementNumber = 0;
                elementDetailViewModel.currentElementId = undefined;
                return;
            }

            if (elementDetailViewModel.resume) {
                var maxDateElement = _.max(elementDetailViewModel.elements, function(element) {
                    return new Date(element.UpdateDate);
                });

                var index = elementDetailViewModel.elements.indexOf(maxDateElement);

                elementDetailViewModel.currentElementNumber = index + 1;
                elementDetailViewModel.currentElementId = elementDetailViewModel.elements[index].ElementId;
            } else if (elementDetailViewModel.current) {
                elementDetailViewModel.currentElementNumber =
                    elementDetailViewModel.elements.indexOf(
                        elementDetailViewModel.elements.filter(
                            function(val) { return val.ElementId === elementDetailViewModel.current; })[0]) + 1;
                if (elementDetailViewModel.currentElementNumber == 0) {
                    elementDetailViewModel.elements.push({ ElementId: elementDetailViewModel.current });
                    elementDetailViewModel.currentElementNumber = elementDetailViewModel.elements.length;
                }
                elementDetailViewModel.currentElementId = elementDetailViewModel.current;
            } else {
                elementDetailViewModel.currentElementNumber = 1;
                elementDetailViewModel.currentElementId = elementDetailViewModel.elements[0].ElementId;
            }
        }

        function clearElementDetailSessionInfo() {
            sessionService.clearSection('elementDetail');
        }

        function broadcastMappingProjectChangedEvent() {
            $scope.$broadcast('mapping-project-changed', elementDetailViewModel.mappingProject);
        }

        elementDetailViewModel.load = function() {
            if (elementDetailViewModel.isMappingProject)
                elementDetailViewModel.getMappingProjectDetails();
            else
                elementDetailViewModel.getDataStandardDetails();
            elementDetailViewModel.getElementIds();
        };

        elementDetailViewModel.load();

        elementDetailViewModel.previous = function() {
            if (elementDetailViewModel.currentElementNumber > 1) {
                elementDetailViewModel.currentElementNumber--;
                elementDetailViewModel.currentElementId =
                    elementDetailViewModel.elements[elementDetailViewModel.currentElementNumber - 1].ElementId;
                elementDetailViewModel.getElementData();
            }
        }

        elementDetailViewModel.next = function() {
            if (elementDetailViewModel.currentElementNumber < elementDetailViewModel.elements.length) {
                elementDetailViewModel.currentElementNumber++;
                elementDetailViewModel.currentElementId =
                    elementDetailViewModel.elements[elementDetailViewModel.currentElementNumber - 1].ElementId;
                elementDetailViewModel.getElementData();
            }
        };

        elementDetailViewModel.onPage = function(sref) {
            return $state.current.name === sref;
        };

        elementDetailViewModel.tabs = [
            { link: 'elementDetail.info', label: 'Info' },
            { link: 'elementDetail.mapping', label: 'Mapping' },
        ];

        if (elementDetailViewModel.dataStandardId != elementDetailViewModel.emptyGuid) {
            elementDetailViewModel.tabs.push({ link: 'elementDetail.actions', label: 'Actions' });
        }

        elementDetailViewModel.showTabs = function() {
            return _.some(elementDetailViewModel.tabs, function(tab) {
                return $state.is(tab.link);
            });
        };


        elementDetailViewModel.save = function() {
            $scope.$broadcast('show-errors-check-valid');

            if ($scope.elementForm.$invalid)
                return;

            elementDetailService.update(elementDetailViewModel.element.ElementDetails.SystemItemId, elementDetailViewModel.element.ElementDetails)
                .then(function(data) {
                    $("#elementModal").modal('hide');
                    elementDetailViewModel.getElementData();
                })
                .catch(function(error) {
                    handleErrorService.handleErrors(error, elementDetailViewModel);
                });
        };

        elementDetailViewModel.cancel = function() {
            $scope.$broadcast('show-errors-reset');
            $("#elementModal").modal('hide');
            elementDetailViewModel.getElementData();
        };
    }
]);