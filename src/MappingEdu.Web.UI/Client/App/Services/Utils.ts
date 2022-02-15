// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.services.utils
//

var m = angular.module('app.services.utils', []);


// ****************************************************************************
// Interfaces
//

interface IUtils {
    appendPathToEndOfBusinessLogic(originalText, textToInsert): string;
    appendToEndOfBusinessLogic(originalText, textToInsert): string;
    endsWith(str: string, suffix: string): boolean;
    getCaretPosition(control): number;
    insertIntoBusinessLogic(originalText, textToInsert, insertPosition): string;
    insertPathIntoBusinessLogic(originalText, textToInsert, insertPosition): string;
    setCaretPosition(control, startPos, endPos): void;
    startsWith(str: string, prefix: string) : boolean;
}


// ****************************************************************************
// Service utils
//

m.factory('utils', () => {

    function appendPathToEndOfBusinessLogic(originalText, textToInsert) {
        if (null == originalText)
            return `[${textToInsert}]`;

        return originalText + ' [' + textToInsert + '] ';
    }

    function appendToEndOfBusinessLogic(originalText, textToInsert) {
        if (null == originalText)
            return `[${textToInsert}]`;

        if (null == textToInsert)
            return originalText;

        return originalText + textToInsert;
    }

    function endsWith(str, suffix)
    {
        return (str.indexOf(suffix, str.length - suffix.length) !== -1);
    }

    function getCaretPosition(control) {
        var position = 0;

        var doc = <any> document; // TODO: Cast properly (cpt)
        if (doc.selection) {
            control.focus();
            var sel = doc.selection.createRange();
            sel.moveStart('character', -control.value.length);
            position = sel.text.length;
        } else if (control.selectionStart || control.selectionStart == '0') {
            position = control.selectionStart;
        }

        return position;
    }

    function insertIntoBusinessLogic(originalText, textToInsert, insertPosition) {
        if (null == originalText)
            return `[${textToInsert}]`;

        if (null == textToInsert)
            return originalText;

        var startPart = originalText.substring(0, insertPosition);
        var endPart = originalText.substring(insertPosition, originalText.length);
        return startPart + textToInsert + endPart;
    }

    function insertPathIntoBusinessLogic(originalText, textToInsert, insertPosition) {
        if (null == originalText)
            return `[${textToInsert}]`;

        var startPart = originalText.substring(0, insertPosition);
        var endPart = originalText.substring(insertPosition, originalText.length);
        return startPart + (this.endsWith(startPart, ' ') ? '[' : ' [') + textToInsert + (this.startsWith(endPart, ' ') ? ']' : '] ') + endPart;
    }

    function setCaretPosition(control, startPos, endPos) {
        if (typeof endPos === 'undefined')
            endPos = startPos;

        if (control.setSelectionRange) {
            control.focus();
            control.setSelectionRange(startPos, endPos);
        }
        else if (control.createTextRange) {
            var range = control.createTextRange();
            range.collapse(true);
            range.moveEnd('character', startPos);
            range.moveStart('character', endPos);
            range.select();
        }
    }

    function startsWith(str, prefix) {
        return (str.indexOf(prefix) === 0);
    }

    var utils: IUtils = {
        appendPathToEndOfBusinessLogic: appendPathToEndOfBusinessLogic,
        appendToEndOfBusinessLogic: appendToEndOfBusinessLogic,
        endsWith: endsWith,
        getCaretPosition: getCaretPosition,
        insertIntoBusinessLogic: insertIntoBusinessLogic,
        insertPathIntoBusinessLogic: insertPathIntoBusinessLogic,
        setCaretPosition: setCaretPosition,
        startsWith: startsWith
    };

    return utils;
});