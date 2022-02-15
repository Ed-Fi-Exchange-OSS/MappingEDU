// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

// ****************************************************************************
// Module app.directives.fixed-table
//

var m = angular.module('app.directives.fixed-table', []);


// ****************************************************************************
// Directive ma-fixed-table
//

m.directive('maFixedTable', ['services', (services: IServices) => ({
    restrict: 'A',
    scope: {
        fixedColumns: '=',
        tableHeight: '=',
        tableReload: '=',
        tableScroll: '=',
        autoResize: '=',
        fixedRightColumns: '='
    },
    compile: (element, attrs) => {
        var html = angular.element('<div/>').append(element).html();

        element.remove();

        var tableHeight;
        var fixedColumns = 0;
        var fixedRightColumns = 0

        if (attrs['fixedColumns']) fixedColumns = attrs['fixedColumns'];
        if (attrs['tableHeight']) tableHeight = attrs['tableHeight'];
        if (attrs['fixedRightColumns']) fixedRightColumns = attrs['fixedRightColumns'];

        var newElement = angular.element(html);

        var trHead = angular.element(newElement.find('thead > tr'));
        var trHeadClasses = trHead.attr('class');
        var trHeadStyles = trHead.attr('style');

        var trFoot = angular.element(newElement.find('tfoot > tr'));
        var hasTfoot = trFoot.length;

        var trFootClasses: string;
        var trFootStyles: string;

        if (hasTfoot) {
            trFootClasses = trFoot.attr('class');
            trFootStyles = trFoot.attr('style');
        }

        var classes = newElement.attr('class');
        var styles = newElement.attr('style');

        var random = Math.round(Math.random() * 100000);

        var wrapperHtml = `<div id="fixed-table-wrapper-${random}" style="position: relative; top: 0; left: 0" ma-width-changed="onSizeChange"></div>`;

        var newHtml = `<div id="fixed-table-${random}" style="width: 100%; overflow-x: hidden; visibility: hidden; padding-right: 10px; z-index: 10">`;
        newHtml += '    <div id="left" style="float: left">';
        newHtml += `        <table id="fixed-table-1" class="${classes ? classes : ''}" style="${styles ? styles : ''}">`;
        newHtml += `            <thead>`;
        newHtml += `               <tr class="${trHeadClasses ? trHeadClasses : ''}" style="${trHeadStyles ? trHeadStyles : ''}" id="fixed-header-left"></tr>`;
        newHtml += '           </thead>';
        newHtml += '        </table>';
        newHtml += `        <div id="fixed-cells-left" style="${tableHeight ? `max-height: ${tableHeight}px; ` : ''}overflow: hidden; position: relative">`;
        newHtml += `           <table id="fixed-table-2" class="${classes ? classes : ''}" style="${styles ? styles : ''}">`;
        newHtml += '               <tbody id="fixed-columns-left-body" vs-repeat="29" vs-excess="20" vs-scroll-parent="#fixed-cells-left"></tbody>';
        newHtml += '           </table>';
        newHtml += '        </div>';

        if (hasTfoot) {

            newHtml += `<table id="fixed-table-3" class="${classes ? classes : ''}" style="${styles ? styles : ''}">`;
            newHtml += `    <tfoot>`;
            newHtml += `        <tr id="fixed-footer-left" class="${trFootClasses ? trFootClasses : ''}" style="${trFootStyles ? trFootStyles : ''}"></tr>`;
            newHtml += '   </tfoot>';
            newHtml += '</table>';
        }

        newHtml += '    </div>';
        newHtml += '    <div id="center" style="float: left; overflow: hidden; position: relative; padding-bottom: 12px">';
        newHtml += '        <div id="fixed-table-header">';
        newHtml += `            <table id="fixed-table-4" class="${classes ? classes : ''}" style="${styles ? styles : ''}">`;
        newHtml += '                <thead>';
        newHtml += `                    <tr class="${trHeadClasses ? trHeadClasses : ''}" style="${trHeadStyles ? trHeadStyles : ''}" id="fixed-header"></tr>`;
        newHtml += '                </thead>';
        newHtml += '           </table>';
        newHtml += '       </div>';
        newHtml += `        <div id="fixed-cells" style="${tableHeight ? `max-height: ${tableHeight}px; ` : ''} overflow: hidden; position: relative">`;
        newHtml += `            <table id="fixed-table-5" class="${classes ? classes : ''}" style=" ${styles ? styles : ''}">`;
        newHtml += '                <tbody id="scroll-body" vs-repeat="29" vs-excess="20" vs-scroll-parent="#fixed-cells"></tbody>';
        newHtml += '           </table>';
        newHtml += '       </div>';

        if (hasTfoot) {
            newHtml += '<div id="fixed-table-footer">';
            newHtml += `    <table id="fixed-table-6" class="${classes ? classes : ''}" style="${styles ? styles : ''}">`;
            newHtml += `        <tfoot>`;
            newHtml += `             <tr id="fixed-footer" class="${trFootClasses ? trFootClasses : ''}" style="${trFootStyles ? trFootStyles : ''}" ></tr>`;
            newHtml += '       </tfoot>';
            newHtml += '    </table>';
            newHtml += '</div>';
        }
        newHtml += '</div>';

        if (fixedRightColumns) {
            newHtml += '    <div id="right" style="float: left;">';
            newHtml += `        <table id="fixed-table-7" class="${classes ? classes : ''}" style="${styles ? styles : ''}">`;
            newHtml += `            <thead>`;
            newHtml += `               <tr class="${trHeadClasses ? trHeadClasses : ''}" style="${trHeadStyles ? trHeadStyles : ''}" id="fixed-header-right"></tr>`;
            newHtml += '           </thead>';
            newHtml += '        </table>';
            newHtml += `        <div id="fixed-cells-right" style="${tableHeight ? `max-height: ${tableHeight}px; ` : ''}overflow: hidden; position: relative">`;
            newHtml += `           <table id="fixed-table-8" class="${classes ? classes : ''}" style="${styles ? styles : ''}">`;
            newHtml += '               <tbody id="fixed-columns-right-body" vs-excess="20" vs-repeat="29" vs-scroll-parent="#fixed-cells-right"></tbody>';
            newHtml += '           </table>';
            newHtml += '        </div>';

            if (hasTfoot) {

                newHtml += `<table id="fixed-table-9" class="${classes ? classes : ''}" style="${styles ? styles : ''}">`;
                newHtml += `    <tfoot>`;
                newHtml += `        <tr id="fixed-footer-right" class="${trFootClasses ? trFootClasses : ''}" style="${trFootStyles ? trFootStyles : ''}"></tr>`;
                newHtml += '   </tfoot>';
                newHtml += '</table>';
            }

            newHtml += '    </div>';
        }
        newHtml += '</div>';
        newHtml += `<div id="hidden-div-${random}" style="position: absolute; Left: 0; Top: 0"></div>`;

        var dynamicTable = angular.element(newHtml);

        var headerColumns = newElement.find('thead > tr > th, thead > tr > td');
        var fixedHeaderLeftEl = dynamicTable.find('#fixed-header-left');
        var fixedHeaderEl = dynamicTable.find('#fixed-header');
        var fixedHeaderRightEl = dynamicTable.find('#fixed-header-right');

        angular.forEach(headerColumns, (column, index: number) => {
            if (index < fixedColumns) {
                fixedHeaderLeftEl.append(column);
                fixedHeaderLeftEl.append(column);
            } else if (index >= (headerColumns.length - fixedRightColumns)) {
                fixedHeaderRightEl.append(column);
            } else {
                fixedHeaderEl.append(column);
            }
        });

        var rows = newElement.find('tbody > tr');
        var fixedColumnsLeftBody = angular.element(dynamicTable.find('#fixed-columns-left-body'));
        var scrollTableBody = angular.element(dynamicTable.find('#scroll-body'));
        var fixedColumnsRightBody = angular.element(dynamicTable.find('#fixed-columns-right-body'));
        var fixedTableSizeBody = angular.element(dynamicTable.find('#fixed-columns-size'));

        angular.forEach(rows, (row) => {

            var rowEl = angular.element(row);
            var rowEl1 = angular.element('<div/>').append(rowEl).html();
            var rowEl2 = angular.copy(rowEl1);
            var rowEl3 = angular.copy(rowEl1);

            var fixedLeftColumnTr = angular.element(rowEl1);
            var mainBodyTr = angular.element(rowEl2);
            var fixedRightColumnTr = angular.element(rowEl3);

            fixedLeftColumnTr.html('');
            mainBodyTr.html('');
            fixedRightColumnTr.html('');

            var rowColumns = rowEl.find('th, td');

            angular.forEach(rowColumns, (column, index: number) => {
                var colElement = angular.element(column);

                //For the vsrepeater
                if (rowEl.attr('ng-repeat')) {
                    angular.forEach(colElement, col => {
                        angular.forEach(col.attributes, attr => {
                            if (colElement.attr('ng-repeat')) colElement.attr(attr.name, attr.value.split('$parent.$index').join('($parent.$index + startIndex)'));
                            else colElement.attr(attr.name, attr.value.split('$index').join('($index + startIndex)'));
                        });

                        if (colElement.attr('ng-repeat')) colElement.html(colElement.html().split('$parent.$index').join('($parent.$index + startIndex)'));
                        else colElement.html(colElement.html().split('$index').join('($index + startIndex)'));
                    });
                }

                if (index < fixedColumns) {
                    fixedLeftColumnTr.append(colElement);
                } else if (index >= (rowColumns.length - fixedRightColumns)) {
                    fixedRightColumnTr.append(colElement);
                } else {
                    mainBodyTr.append(colElement);
                }
            });

            fixedColumnsLeftBody.append(fixedLeftColumnTr);
            fixedTableSizeBody.append(angular.copy(fixedLeftColumnTr));

            scrollTableBody.append(mainBodyTr);
            fixedColumnsRightBody.append(fixedRightColumnTr);
        });

        var fixedFooterLeftEl: angular.IAugmentedJQuery;
        var fixedFooterEl: angular.IAugmentedJQuery;
        var fixedFooterRightEl: angular.IAugmentedJQuery;
        if (hasTfoot) {
            fixedFooterLeftEl = angular.element(dynamicTable.find('#fixed-footer-left'));
            fixedFooterEl = angular.element(dynamicTable.find('#fixed-footer'));
            fixedFooterRightEl = angular.element(dynamicTable.find('#fixed-footer-right'));

            var footerColumns = newElement.find('tfoot > tr > th, tfoot > tr > td');

            angular.forEach(footerColumns, (column, index: number) => {
                if (index < fixedColumns) {
                    fixedFooterLeftEl.append(column);
                } else if (index >= (footerColumns.length - fixedRightColumns)) {
                    fixedFooterRightEl.append(column);
                } else {
                    fixedFooterEl.append(column);
                }
            });
        }

        var fixedTable1 = angular.element(dynamicTable.find('#fixed-table-1'));
        var fixedTable2 = angular.element(dynamicTable.find('#fixed-table-2'));
        var fixedTable3 = angular.element(dynamicTable.find('#fixed-table-3'));
        var fixedTable4 = angular.element(dynamicTable.find('#fixed-table-4'));
        var fixedTable5 = angular.element(dynamicTable.find('#fixed-table-5'));
        var fixedTable6 = angular.element(dynamicTable.find('#fixed-table-6'));
        var fixedTable7 = angular.element(dynamicTable.find('#fixed-table-7'));
        var fixedTable8 = angular.element(dynamicTable.find('#fixed-table-8'));
        var fixedTable9 = angular.element(dynamicTable.find('#fixed-table-9'));

        fixedTable1.css('margin-bottom', '0');
        fixedTable2.css('margin-bottom', '0');
        fixedTable3.css('margin-bottom', '0');
        fixedTable4.css('margin-bottom', '0');
        fixedTable5.css('margin-bottom', '0');
        fixedTable6.css('margin-bottom', '0');
        fixedTable7.css('margin-bottom', '0');
        fixedTable8.css('margin-bottom', '0');
        fixedTable9.css('margin-bottom', '0');

        newHtml = angular.element('<div/>').append(dynamicTable).html();

        var pseudoTranscludeFn = (scope, html, cloneAttachFn) => {
            return services.compile(html)(scope, cloneAttachFn);
        };

        return (scope, _e) => {
            pseudoTranscludeFn(scope, wrapperHtml, (wrapper) => {
                if (!scope.tableReload) scope.tableReload = '';
                if (!scope.tableScroll) scope.tableReload = '';
                if (scope.autoResize === undefined) scope.autoResize = true;
                scope.wrapperId = `#fixed-table-wrapper-${random}`;
                scope.fixedTableId = `#fixed-table-${random}`;
                scope.hiddenDivId = `#hidden-div-${random}`;
                pseudoTranscludeFn(scope.$parent, newHtml, (clone) => {
                    wrapper.append(clone);
                    _e.before(wrapper);
                });
            });
        }
    },
    controller: ['$scope', 'services', ($scope, services: IServices) => {

        services.logger.debug('Loaded controller app.directives.fixed-table');

        function reload(returnToTop?: boolean, resizing?: boolean) {

            var fixedTable = angular.element($scope.fixedTableId);

            var fixedTable1 = angular.element(fixedTable.find('#fixed-table-1'));
            var fixedTable2 = angular.element(fixedTable.find('#fixed-table-2'));
            var fixedTable3 = angular.element(fixedTable.find('#fixed-table-3'));
            var fixedTable4 = angular.element(fixedTable.find('#fixed-table-4'));
            var fixedTable5 = angular.element(fixedTable.find('#fixed-table-5'));
            var fixedTable6 = angular.element(fixedTable.find('#fixed-table-6'));
            var fixedTable7 = angular.element(fixedTable.find('#fixed-table-7'));
            var fixedTable8 = angular.element(fixedTable.find('#fixed-table-8'));
            var fixedTable9 = angular.element(fixedTable.find('#fixed-table-9'));

            //Used to avoid flashing. Gives the appearance of resizing
            if (!resizing) {
                var hiddenDiv = angular.element($scope.hiddenDivId);
                var leftTables = angular.element(fixedTable.find('#left'));
                var centerTables = angular.element(fixedTable.find('#center'));
                var rightTables = angular.element(fixedTable.find('#right'));

                hiddenDiv.css('visibility', '');
                fixedTable.css('visibility', 'hidden');
                centerTables.width(0);
                services.timeout(() => {
                    centerTables.width(fixedTable.width() - leftTables.width() - rightTables.width() - 10);
                    fixedTable.css('visibility', '');
                    hiddenDiv.css('visibility', 'hidden');
                    hiddenDiv.html(fixedTable.html());
                });
            }

            if ($scope.autoResize) {
                fixedTable4.css('table-layout', '');
                fixedTable5.css('table-layout', '');
                if (fixedTable3.length) fixedTable6.css('table-layout', '');   
            }

            if ($scope.autoResize) {
                services.timeout(() => {

                    var headerColumns = fixedTable1.find('thead > tr > th, thead > tr > td').add(fixedTable4.find('thead > tr > th, thead > tr > td').add(fixedTable7.find('thead > tr > th, thead > tr > td')));
                    var cellColumns = fixedTable2.find('tbody > tr:first > th, tbody > tr:first > td').add(fixedTable5.find('tbody > tr:first > th, tbody > tr:first > td').add(fixedTable8.find('tbody > tr:first > th, tbody > tr:first > td')));
                    var footerColumns;
                    if (fixedTable3.length) {
                        footerColumns = fixedTable3.find('tfoot > tr > th, tfoot > tr > td').add(fixedTable6.find('tfoot > tr > th, tfoot > tr > td').add(fixedTable9.find('tfoot > tr > th, tfoot > tr > td')));
                    }

                    var headerWidths = [];
                    var footerWidths = [];
                    var cellWidths = [];
                    for (var i = 0; i < headerColumns.length; i++) {
                        headerWidths.push(angular.element(headerColumns[i]).width());
                        cellWidths.push(angular.element(cellColumns[i]).width());
                        if (fixedTable3.length) footerWidths.push(angular.element(footerColumns[i]).width());
                    }

                    fixedTable4.css('table-layout', 'fixed');
                    fixedTable5.css('table-layout', 'fixed');
                    if (fixedTable3.length) fixedTable6.css('table-layout', 'fixed');

                    for (var i = 0; i < headerColumns.length; i++) {

                        var maxWidth = Math.max(headerWidths[i], cellWidths[i], fixedTable3.length ? footerWidths[i] : 0);
                        angular.element(headerColumns[i]).width(maxWidth);
                        angular.element(cellColumns[i]).width(maxWidth);
                        if (fixedTable3.length) angular.element(footerColumns[i]).width(maxWidth);
                    }
                });
            }

            services.timeout(() => {
                var fixedTableCells: any = angular.element(fixedTable.find('#fixed-cells'));
                var fixedTableLeftColumns: any = angular.element(fixedTable.find('#fixed-cells-left'));
                var centerDiv: any = angular.element(fixedTable.find('#center'));

                centerDiv.perfectScrollbar('update');
                fixedTableCells.perfectScrollbar('update');
                fixedTableLeftColumns.perfectScrollbar('update');

                angular.element(centerDiv.find('.ps-scrollbar-y-rail')).css('right', null);
                angular.element(fixedTableCells.find('.ps-scrollbar-x-rail')).css('display', 'none');
                angular.element(fixedTableLeftColumns.find('.ps-scrollbar-y-rail')).css('right', null);
                angular.element(fixedTableLeftColumns.find('.ps-scrollbar-y-rail')).css('left', '3px');

                if (returnToTop) {
                    centerDiv.scrollLeft(0);
                    fixedTableCells.scrollTop(0);
                    fixedTableCells.scrollLeft(0);
                    fixedTableCells.css('left', '0');
                }
            });
        }

        $scope.tableReload = reload;
        services.timeout(() => {
            var fixedTable = angular.element($scope.fixedTableId);
            reload();

            var fixedTableCells: any = angular.element(fixedTable.find('#fixed-cells'));
            var fixedTableLeftColumns: any = angular.element(fixedTable.find('#fixed-cells-left'));
            var fixedTableRightColumns: any = angular.element(fixedTable.find('#fixed-cells-right'));
            var centerDiv: any = angular.element(fixedTable.find('#center'));

            centerDiv.perfectScrollbar();
            fixedTableCells.perfectScrollbar();
            fixedTableLeftColumns.perfectScrollbar();

            angular.element(centerDiv.find('.ps-scrollbar-y-rail')).css('right', null);
            angular.element(fixedTableCells.find('.ps-scrollbar-x-rail')).css('display', 'none');
            angular.element(fixedTableLeftColumns.find('.ps-scrollbar-y-rail')).css('right', null);
            angular.element(fixedTableLeftColumns.find('.ps-scrollbar-y-rail')).css('left', '3px');

            centerDiv.on('scroll', function () {
                fixedTableCells.scrollLeft(centerDiv.scrollLeft());
                fixedTableCells.css('left', `${centerDiv.scrollLeft()}px`);
            });

            fixedTableCells.on('scroll', function () {
                fixedTableLeftColumns.scrollTop(fixedTableCells.scrollTop());
                fixedTableRightColumns.scrollTop(fixedTableCells.scrollTop());
            });

            fixedTableLeftColumns.on('scroll', function () {
                fixedTableCells.scrollTop(fixedTableLeftColumns.scrollTop());
                fixedTableRightColumns.scrollTop(fixedTableLeftColumns.scrollTop());
            });

            // Starts invisible so the user doesn't see anything popping in
            fixedTable.css('visibility', '');
            fixedTable.css('padding-right', '0px');
        });

        $scope.resizeCells = () => {
            var fixedTable = angular.element($scope.fixedTableId);
            var fixedTableCells: any = angular.element(fixedTable.find('#fixed-cells'));
            var fixedTableHeader: any = angular.element(fixedTable.find('#fixed-table-header'));
            var fixedTableFooter: any = angular.element(fixedTable.find('#fixed-table-footer'));

            var headerCells = fixedTableHeader.find('th, td');
            var footerCells = fixedTableFooter.find('th, td');

            var cells: any = [];
            if (fixedTableCells.find('tr')[1])
                cells = $(fixedTableCells.find('tr')[1]).find('th, td');

            if (cells.length > 0) {
                angular.forEach(headerCells, (cell, index) => {
                    var headerWidth = $(cell).width();
                    var footerWidth = $(footerCells[index]).width();
                    var cellWidth = $(cells[index]).width();

                    var maxWidth = Math.max(headerWidth, footerWidth, cellWidth) + 10;

                    $(cell).width(maxWidth);
                    $(footerCells[index]).width(maxWidth);
                    $(cells[index]).width(maxWidth);

                    $(cell).css('min-width', `${maxWidth}px`);
                    $(footerCells[index]).css('min-width', `${maxWidth}px`);
                    $(cells[index]).css('min-width', `${maxWidth}px`);
                });
            }
        }

        $scope.resize = () => {
            //$scope.resizeCells();

            var fixedTable = angular.element($scope.fixedTableId);
            var leftTables = angular.element(fixedTable.find('#left'));
            var centerTables = angular.element(fixedTable.find('#center'));
            var rightTables = angular.element(fixedTable.find('#right'));

            centerTables.width(fixedTable.width() - leftTables.width() - rightTables.width() - 10);
        }

        var windowResize = false;
        var changes = 0; //To avoid div flicker
        $scope.onSizeChange = (newVal, oldVal) => {
            changes++;
            services.timeout(() => {
                changes--;
                if (changes === 0) {
                    if (!windowResize) $scope.resize();
                    else windowResize = false;
                }
            });
        }

        angular.element(window).resize(() => {
            windowResize = true;
            $scope.resize();
        });

        function tableScroll(width, height)
        {
            var fixedTable = angular.element($scope.fixedTableId);
            var fixedTableCells: any = angular.element(fixedTable.find('#fixed-cells'));
            var fixedTableLeftColumns: any = angular.element(fixedTable.find('#fixed-cells-left'));
            var fixedTableRightColumns: any = angular.element(fixedTable.find('#fixed-cells-right'));
            var fixedTableHeader: any = angular.element(fixedTable.find('#fixed-table-header'));
            var fixedTableFooter: any = angular.element(fixedTable.find('#fixed-table-footer'));
            var fixedTable3 = angular.element(fixedTable.find('#fixed-table-3'));

            if (width !== undefined) {
                fixedTableHeader.scrollLeft(width);
                fixedTableCells.scrollLeft(width);
                if (fixedTable3.length) fixedTableFooter.scrollLeft(width);  
            }

            if (height !== undefined) {
                fixedTableLeftColumns.scrollTop(height);
                fixedTableCells.scrollTop(height);
                fixedTableRightColumns.scrollTop(height);   
            }
        }

        $scope.tableScroll = tableScroll;

    }]
})]);
