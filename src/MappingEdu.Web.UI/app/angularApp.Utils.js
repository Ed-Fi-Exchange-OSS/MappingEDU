// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

var angularApp = angularApp || {};
angularApp.Utils = angularApp.Utils || {};

angularApp.Utils.getCaretPosition = function (control) {
    var position = 0;

    if (document.selection) {
        control.focus();
        var sel = document.selection.createRange();
        sel.moveStart('character', -control.value.length);
        position = sel.text.length;
    } else if (control.selectionStart || control.selectionStart == '0') {
        position = control.selectionStart;
    }

    return position;
}

angularApp.Utils.setCaretPosition = function(control, startPos, endPos) {
    if (typeof endPos === 'undefined')
        endPos = startPos;

    if (control.setSelectionRange)
    {
        control.focus();
        control.setSelectionRange(startPos,endPos);
    }
    else if (control.createTextRange) {
        var range = control.createTextRange();
        range.collapse(true);
        range.moveEnd('character', startPos);
        range.moveStart('character', endPos);
        range.select();
    }
}

angularApp.Utils.insertIntoBusinessLogic = function (originalText, textToInsert, insertPosition) {
    if (null == originalText)
        return textToInsert;

    if (null == textToInsert)
        return originalText;

    var startPart = originalText.substring(0, insertPosition);
    var endPart = originalText.substring(insertPosition, originalText.length);
    return startPart + textToInsert + endPart;
}

angularApp.Utils.insertPathIntoBusinessLogic = function (originalText, textToInsert, insertPosition) {
    if (null == originalText)
        return textToInsert;

    var startPart = originalText.substring(0, insertPosition);
    var endPart = originalText.substring(insertPosition, originalText.length);
    return startPart + (angularApp.Utils.endsWith(startPart, ' ') ? '[' : ' [') + textToInsert +
        (angularApp.Utils.startsWith(endPart, ' ') ? ']' : '] ') + endPart;
}

angularApp.Utils.startsWith = function (str, prefix) {
    return str.indexOf(prefix) === 0;
}

angularApp.Utils.endsWith = function (str, suffix) {
    return str.indexOf(suffix, str.length - suffix.length) !== -1;
}

angularApp.Utils.appendToEndOfBusinessLogic = function (originalText, textToInsert) {
    if (null == originalText)
        return textToInsert;

    if (null == textToInsert)
        return originalText;

    return originalText + textToInsert;
}

angularApp.Utils.appendPathToEndOfBusinessLogic = function (originalText, textToInsert) {
    if (null == originalText)
        return ' [' + textToInsert + '] ';

    return originalText + ' [' + textToInsert + '] ';
}
