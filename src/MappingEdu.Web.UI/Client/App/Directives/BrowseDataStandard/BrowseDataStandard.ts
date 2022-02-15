// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.browse-standard
//

var m = angular.module('app.directives.browse-standard', []);


// ****************************************************************************
// Directive
//

m.directive('maBrowseDataStandard', ['settings', (settings: ISystemSettings) => ({
    restrict: 'E',
    templateUrl: `${settings.directiveBaseUri}/BrowseDataStandard/browseDataStandard.tpl.html`,
    scope: {
        standardId: '=',
        domainId: '=',
        selectedItem: '=',
        browseElement: '=',
        onOpen: '=?'
    },
    controller: 'app.directives.browse-standard',
    controllerAs: 'browseModel'
})]);


// ****************************************************************************
// Controller app.directives.browse-standard
//

m.controller('app.directives.browse-standard', [
    '$', '$scope', 'services', 'repositories',
    function($, $scope, services: IServices, repositories: IRepositories) {

        services.logger.debug('Loaded app.modules.data-standard.directives.browse');

        var vm = this;
        vm.id = $scope.standardId;
        vm.filters = [];
        vm.lists = [];

        vm.browseId = Math.floor(Math.random() * 10000);

        vm.getNext = (element, index) => {
            if (element.PathSegments[index]) {
                var found = services.underscore.find(<Array<any>>vm.lists[index].Children, (x => x.ItemName === element.PathSegments[index] && (element.PathSegments[index + 1] || x.ItemTypeId === element.ItemTypeId)));
                var itemIndex = services.underscore.indexOf(<Array<any>>vm.lists[index].Children, found);
                vm.setCollapsed(found, itemIndex, index, true).then(() => {
                    vm.getNext(element, index + 1);
                });
            } else {
                $scope.elementLoading = false;
                services.loading.finish(`element_${vm.browseId}`);
                services.timeout(() => { updateScrollbar(index + 1); }, 100);
            }

        }

        vm.goToElement = element => {
            $scope.elementLoading = true;
            services.loading.start(`element_${vm.browseId}`);

            element.PathSegments = element.DomainItemPath.split('.');

            for (var i = 0; i < vm.lists.length; i++) {
                angular.forEach(vm.lists[i].Children, item => { item.Collapsed = false; });
            }
            var found = services.underscore.find(<Array<any>>vm.lists[0].Children, x => x.ItemName === element.PathSegments[0]);
            var index = services.underscore.indexOf(<Array<any>>vm.lists[0].Children, found);

            vm.setCollapsed(found, index, 0, true).then(() => {
                vm.getNext(element, 1);
            });
        }

        $scope.$watch('browseElement', (newVal, oldVal) => {
            if (newVal && vm.lists && vm.lists.length > 0 && $scope.systemItemId !== newVal.SystemItemId) {
                vm.goToElement(newVal);
            }
        });

        function updateScrollbar(listIndex) {

            $scope.listIndex = listIndex;

            var sideScroller = $(`#side-scroller_${vm.browseId}`);
            if (!sideScroller.hasClass('ps-container')) {
                sideScroller.perfectScrollbar({ suppressScrollY: true });
            };
            var pathScroller = $(`#path-scroller_${vm.browseId}`);
            if (!pathScroller.hasClass('ps-container')) {
                pathScroller.perfectScrollbar({ suppressScrollY: true });
            };
            var infoScroller = $(`#info-scroller_${vm.browseId}`);
            var enumerationsScroller = $(`#enumerations-scroller_${vm.browseId}`);

            var pathWidth = 0;
            for (var i = 0; i <= listIndex; i++) {
                pathWidth += $(`#pathSegment${i}_${vm.browseId}`).width();
            }
            pathScroller.animate({
                scrollLeft: pathWidth - sideScroller.width()
            }, 800);
            sideScroller.animate({
                scrollLeft: ((listIndex) * 400) - sideScroller.width()
            }, 800);

            enumerationsScroller.perfectScrollbar('destroy');
            enumerationsScroller.perfectScrollbar();

            infoScroller.perfectScrollbar('destroy');
            infoScroller.perfectScrollbar({ suppressScrollX: true });

            services.timeout(() => {
                sideScroller.perfectScrollbar('update');
                pathScroller.perfectScrollbar('update');
            }, 850);

        }

        $scope.onOpen = () => {
            services.timeout(() => {
                updateScrollbar($scope.listIndex);
            }, 100);
        };

        vm.scrollUp = (listIndex) => {
            $(`#list${listIndex}_${vm.browseId}`).animate({ scrollTop: 0 }, 100);
            $(`#list${listIndex}_${vm.browseId}`).perfectScrollbar('destroy');
            $(`#list${listIndex}_${vm.browseId}`).perfectScrollbar();
        }

        vm.scrollUpEnumerations = () => {
            $(`#enumerations-scroller_${vm.browseId}`).animate({ scrollTop: 0 }, 100);
            $(`#enumerations-scroller_${vm.browseId}`).perfectScrollbar('destroy');
            $(`#enumerations-scroller_${vm.browseId}`).perfectScrollbar();
        }

        function setSref(listContextId = null) {
            if ( $scope.selectedItem.ItemTypeId === 1)
                 $scope.selectedItem.sref = services.state.href('app.element-group.detail.info', { id:  $scope.selectedItem.SystemItemId, dataStandardId: vm.id });

            else if ( $scope.selectedItem.ItemTypeId === 2 ||  $scope.selectedItem.ItemTypeId === 3)
                 $scope.selectedItem.sref = services.state.href('app.entity.detail.info', { id:  $scope.selectedItem.SystemItemId, dataStandardId: vm.id });

            else if ( $scope.selectedItem.ItemTypeId === 4 ||  $scope.selectedItem.ItemTypeId === 5)
                 $scope.selectedItem.sref = services.state.href('app.element.detail.info', { elementId:  $scope.selectedItem.SystemItemId, dataStandardId: vm.id, listContextId: listContextId });
        }

        if (vm.lists.length === 0) {
            $scope.domainsLoading = true;
            repositories.element.getChildren(vm.id).then(domains => {
                vm.lists = [
                    {
                        SystemItemId: 1,
                        ItemName: 'Element Groups',
                        Children: angular.copy(domains)
                    }
                ];
                if ($scope.browseElement && $scope.browseElement.DomainItemPath) {
                    vm.goToElement($scope.browseElement);
                }
                else if (domains.length == 1) {
                    vm.setCollapsed(vm.lists[0].Children[0], 0, 0);
                }
                else if ($scope.domainId) {
                    var domainIndex = null;
                    angular.forEach(domains, (domain, index) => {
                        domain.DomainItemPath = domain.ItemName;
                        if (domain.SystemItemId === $scope.domainId) domainIndex = index;
                    });
                    if (domainIndex != null) vm.setCollapsed(vm.lists[0].Children[domainIndex], domainIndex, 0);
                }
                vm.filters.push({ allItemTypes: true });
                services.timeout(() => { $(`#list0_${vm.browseId}`).perfectScrollbar() }, 100);
            }).finally(() => {
                $scope.domainsLoading = false;
            });
        } else {
            updateScrollbar(vm.lists.length - 1);
             $scope.selectedItem = services.underscore.find(<Array<any>>vm.lists[vm.lists.length - 1].Children, item => item.Collapsed);
            if (! $scope.selectedItem)  $scope.selectedItem = vm.lists[vm.lists.length - 1];
            setSref();
        }

        vm.showSearchBar = (listIndex) => {
            $(`#side-scroller_${vm.browseId}`).animate({
                scrollLeft: ((listIndex) * 400)
            }, 400);
        }

        vm.applyItemTypes = (filter, listIndex) => {
            var itemTypes = [];
            if (filter.itemTypes) {
                for (var key in filter.itemTypes) {
                    if (filter.itemTypes[key]) {
                        itemTypes.push(key);
                    }
                }
            }
            filter.allItemTypes = itemTypes.length === 0;
            vm.scrollUp(listIndex);
        }

        vm.getHeight = (itemIndex, list) => {
            var height = 0;
            for (var i = 0; i < itemIndex; i++) {
                if (list.Children[i].Definition) height += 65;
                else height += 44;
            }
            return height;
        }

        vm.setCollapsed = (item, itemIndex, listIndex, loadingElement) => {
            item.index = itemIndex;

            var deferred = services.q.defer();

            if (item.Collapsed) {
                for (var i = vm.lists.length - 1; i > listIndex; i--) {
                    angular.forEach(vm.lists[i].Children, item => { item.Collapsed = false; });
                }
                vm.lists.splice(listIndex + 1);
                vm.filters.splice(listIndex + 1);

                item.Collapsed = false;
                $scope.systemItemId = (listIndex > 0) ? vm.lists[listIndex].SystemItemId : null;
                $scope.selectedItem = (listIndex > 0) ? vm.lists[listIndex] : null;
                services.timeout(() => {
                    if (!loadingElement) updateScrollbar(listIndex + 1);
                    deferred.resolve();
                }, 100);
            }
            else {
                for (var i = vm.lists.length - 1; i > listIndex - 1; i--) {
                    angular.forEach(vm.lists[i].Children, item => { item.Collapsed = false; });
                }
                vm.lists.splice(listIndex + 1);
                vm.filters.splice(listIndex + 1);

                item.DomainItemPath = '';

                for (var i = 1; i < vm.lists.length; i++) {
                     item.DomainItemPath += vm.lists[i].ItemName + '.';
                }
                item.DomainItemPath += item.ItemName;

                $scope.systemItemId = item.SystemItemId;
                $scope.selectedItem = item;   

                setSref(vm.lists[listIndex].SystemItemId);
                item.Collapsed = true;

                vm.filters.push({ allItemTypes: true });

                if(!loadingElement) services.timeout(() => {
                    updateScrollbar(listIndex + ((item.ItemType.Id >= 4) ? 1 : 2));
                }, 50);

                var scrollHeight = vm.getHeight(itemIndex, vm.lists[listIndex]);
                $(`#list${listIndex}_${vm.browseId}`).animate({ scrollTop: scrollHeight }, 800);

                if (item.Children) {
                    vm.lists.push(item);
                    services.timeout(() => {
                        $(`#list${listIndex + 1}_${vm.browseId}`).perfectScrollbar('destroy');
                        $(`#list${listIndex + 1}_${vm.browseId}`).perfectScrollbar();
                        deferred.resolve();
                    }, 100);
                } else {
                    if (item.ItemTypeId < 4) {
                        vm.lists.push(item);
                        if (!loadingElement) services.timeout(() => { services.loading.start(`loading-list${listIndex + 1}_${vm.browseId}`); }, 50);
                        repositories.element.getChildren(item.SystemItemId).then(data => {
                            if (data.length != 0) {
                                item.Children = data;
                                item.ChildrenTypes = [];
                                for (var i = 1; i < 6; i++) {
                                    var found = services.underscore.find(<Array<any>>data, child => child.ItemTypeId === i);
                                    if (found) item.ChildrenTypes.push(found.ItemType);
                                }

                                var ids = [];
                                var children = services.underscore.sortBy(<Array<any>>data, (systemItem) => { return systemItem.ItemName; });

                                angular.forEach(children, child => {
                                    child.DomainItemPath = item.DomainItemPath + '.' + child.ItemName;
                                    if(child.ItemTypeId >= 4) ids.push({ ElementId: child.SystemItemId });
                                });

                                services.session.cloneToSession('elementLists', item.SystemItemId, ids);
                            }
                        }).finally(() => {
                            services.timeout(() => {
                                $(`#list${listIndex + 1}_${vm.browseId}`).perfectScrollbar('destroy');
                                $(`#list${listIndex + 1}_${vm.browseId}`).perfectScrollbar();
                                if (!loadingElement) services.loading.finish(`loading-list${listIndex + 1}_${vm.browseId}`);
                                deferred.resolve();
                            }, 100);
                        });
                        
                    } else if (item.ItemTypeId >= 4) {

                        if (!item.CustomDetails) {
                            repositories.element.systemItemDetail.getCustomDetails(item.SystemItemId).then(details => {
                                if (details) item.CustomDetails = details;
                                else item.CustomDetails = [];
                            });
                        }

                        if (item.ItemTypeId === 5) {
                            vm.enumerationFilter = '';
                            if (!item.EnumerationItems) {
                                if (!loadingElement) services.timeout(() => { services.loading.start(`loading-enumerations_${vm.browseId}`); }, 50);
                                repositories.element.enumeration.find(item.SystemItemId)
                                    .then(data => {
                                        item.EnumerationItems = data.EnumerationItems;
                                    }, error => {
                                        services.logger.error('Error loading eumerations.', error.data);
                                    })
                                    .finally(() => {
                                        if (!loadingElement) services.timeout(() => {
                                            services.loading.finish(`loading-enumerations_${vm.browseId}`);
                                        }, 100);
                                    });
                            }
                        }

                        deferred.resolve();
                    }
                }
            }

            return deferred.promise;
        }
    }
]);