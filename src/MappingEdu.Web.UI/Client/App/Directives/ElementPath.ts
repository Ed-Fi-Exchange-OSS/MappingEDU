// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.directives.element-path
//

var m = angular.module('app.directives.element-path', []);


// ****************************************************************************
// Directive ma-element-path
//

m.directive('maElementPath', ['repositories', 'services', (repositories: IRepositories, services: IServices) => {

    function assignHrefs(segments, contextId, context, lastSegmentIsElement) {
        angular.forEach(segments, (segment, index) => {
            if (index === 0) segment.sref = `app.element-group.detail.info({${context}Id: '${contextId}', id: '${segment.SystemItemId}'})`;
            else if (index < segments.length - 1 || !lastSegmentIsElement) segment.sref = `app.entity.detail.info({${context}Id: '${contextId}', id: '${segment.SystemItemId}'})`;
            else if (index === segments.length - 1 && lastSegmentIsElement) segment.sref = `app.element.detail.mapping({${context}Id: '${contextId}', elementId: '${segment.SystemItemId}'})`;
        });
    }

    function getSegmentHtml(scope, segment, index) {
        var html = '';

        html += '<div style="display: inline-block">';
        html += scope.viewOnly ? '<span ' : `<a ui-sref="${segment.sref}" `;
        html += `ng-mouseenter="getItem(segments[${index}])" `;
        html += `uib-popover-template="'popover_definition_${scope.random}.html'" popover-trigger="'mouseenter'" popover-placement="right"`;
        html += 'class="';
        if (!(scope.segments.length - 1 === index && scope.lastSegmentIsElement) && !segment.IsExtended) html += 'path-segment';
        else if (!(scope.segments.length - 1 === index && scope.lastSegmentIsElement) && segment.IsExtended) html += 'path-segment-c';
        else if (scope.segments.length - 1 === index && scope.lastSegmentIsElement && !segment.IsExtended) html += 'standard-a';
        else if (scope.segments.length - 1 === index && scope.lastSegmentIsElement && segment.IsExtended) html += 'standard-c';

        html += '">';
        if (segment.IsExtended) {
            if (segment.Extension && segment.Extension.ShortName) html += `(${segment.Extension.ShortName}) `;
            else if (segment.ExtensionShortName) html += `(${segment.ExtensionShortName}) `;
            else html += '<i class="fa fa-extended"></i> ';
        }
        html += segment.Name ? segment.Name : segment.ItemName;
        html += scope.viewOnly ? '</span> ' : '</a>';
        html += '</div>';

        return html;
    }

    function getHtml(scope) {
        var html = '';

        scope.random = Math.floor((Math.random() * 100000) + 1);

        for (var i = 0; i < ((scope.segments.length - 1 < scope.depth) ? scope.segments.length : (scope.depth - 2)); i++) {

            html += getSegmentHtml(scope, scope.segments[i], i);
            if (scope.segments.length - 1 !== i) html += '<i class="fa fa-caret-right separator"></i>';

        }

        if (scope.segments.length - 1 >= scope.depth) {
            var isExtended = false;
            for (var i = scope.depth - 2; i < scope.segments.length; i++) if (scope.segments[i].IsExtended) isExtended = true;

            html += '<div style="display: inline-block">';
            html += '<span class="path-segment';
            if (isExtended) html += '-c';
            html += `" uib-popover-template="'popover_path_${scope.random}.html'">`;
            html += '<i class="fa fa-ellipsis-h"></i>';
            html += '</span>';
            html += '</div>';

            //build popover
            html += `<script type="text/ng-template" id="popover_path_${scope.random}.html">`;
            for (var i = scope.depth - 2; i < scope.segments.length - 1; i++) {
                html += getSegmentHtml(scope, scope.segments[i], i);
                if (scope.segments.length - 2 !== i) html += '<i class="fa fa-caret-right separator"></i>';
            }
            html += '</script>';

            html += '<i class="fa fa-caret-right separator"></i>';
            html += getSegmentHtml(scope, scope.segments[scope.segments.length - 1], scope.segments.length - 1);
        }

        if (scope.element) {
            html += '<i class="fa fa-caret-right separator"></i>';

            html += '<div style="display: inline-block">';
            html += scope.viewOnly ? '<span ' : `<a ui-sref="${scope.element.sref}" `;
            html += 'ng-mouseenter="getItem(element)" ';
            html += `uib-popover-template="'popover_definition_${scope.random}.html'" popover-trigger="'mouseenter'" `;
            html += (scope.isExtended) ? 'class="standard-c">' : 'class="standard-a">';

            if (scope.element.IsExtended) {
                if (scope.element.Extension && scope.element.Extension.ShortName) html += `(${scope.element.Extension.ShortName}) `;
                else if (scope.element.ExtensionShortName) html += `(${scope.element.ExtensionShortName}) `;
                else html += '<i class="fa fa-extended"></i> ';
            }
            html += scope.element.Name ? scope.element.Name : scope.element.ItemName;
            html += scope.viewOnly ? '</span> ' : '</a>';
            html += '</div>';
        }

        //Definition Template
        html += `
            <script type="text/ng-template" id="popover_definition_${scope.random}.html">
                <div>
                    <i ng-class="{'fa': !currentDefinition, 'fa-spinner': !currentDefinition, 'fa-spin': !currentDefinition }"></i>
                    {{currentDefinition}}
                </div>
            </script>
        `;

        return html;
    }

    return {
        restrict: 'E',
        scope: {
            contextId: '=',
            segments: '=',
            element: '=',
            linkPath: '=',
            lastSegmentIsElement: '=',
            depth: '=?',
            control: '=?',
            viewOnly: '=?'
        },
        link(scope, element, attrs) {

            var context = attrs.context;

            scope.depth = scope.depth || 3;
            context = context || 'mappingProject';

            if (!scope.segments) return;
            if (scope.element) scope.element.sref = `app.element.detail.mapping({${context}Id: '${scope.contextId}', elementId: '${scope.element.SystemItemId}'})`;

            assignHrefs(scope.segments, scope.contextId, context, scope.lastSegmentIsElement);

            scope.reloadPath = (segments, elmnt) => {
                element.empty();

                scope.segments = segments;
                scope.element = elmnt;

                var el = angular.element(getHtml(scope));
                var compiled = services.compile(el);
                element.append(el);
                compiled(scope);

            }
            scope.reloadPath(scope.segments, scope.element);

            scope.getItem = segment => {
                scope.currentDefinition = null;
                if (segment.SystemItemId && !segment.Definition) {
                    repositories.systemItem.find(segment.SystemItemId).then(item => {
                        if (item.Definition) segment.Definition = item.Definition;
                        else segment.Definition = 'None';

                        scope.currentDefinition = segment.Definition;
                    });
                } else {
                    scope.currentDefinition = segment.Definition;
                }
            }

        }
    }
}]);
