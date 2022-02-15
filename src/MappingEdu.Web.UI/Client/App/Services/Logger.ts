// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Interfaces ILogger
//

interface ILogger {
    info(message: string);
    success(message: string);
    warning(message: string);
    error(message: string, obj?: any);
    debug(message: string, obj?: any);
    dump(arr: any, level: number);
}


// ****************************************************************************
// Module app.services.logger
//

var m = angular.module('app.services.logger', []);


// ****************************************************************************
// Service 'logger' (Toastr)
//

m.factory('logger', ['settings', (settings: ISystemSettings) => {

    toastr.options.timeOut = 3500; // 2 second toast timeout
    toastr.options.positionClass = 'toast-bottom-right';
    
    function log(message, obj?) {
        var console = window.console;
        if (obj) {
            if (console && console.log && console.log.apply && console.group) {
                console.group(message);
                console.log(obj);
                console.groupEnd();
            }
        } else {
            !!console && console.log(message);
        }
    }

    function dump(arr: any, level: number) {
        var dumpedText = '';
        if (!level)
            level = 0;

        //The padding given at the beginning of the line.
        var levelPadding = '';
        for (var j = 0; j < level + 1; j++)
            levelPadding += '    ';
        if (typeof (arr) == 'object') { //Array/Hashes/Objects
            for (var item in arr) {
                if (arr.hasOwnProperty(item)) {
                    var value = arr[item];
                    if (typeof (value) == 'object') { //If it is an array,
                        dumpedText += levelPadding + '\'' + item + '\' ...\n';
                        dumpedText += dump(value, level + 1);
                    } else {
                        dumpedText += levelPadding + '\'' + item + '\' => "' + value + '"\n';
                    }
                }
            }
        }
        else { //Stings/Chars/Numbers etc.
            dumpedText = `===>${arr}<===(${typeof (arr)})`;
        }
        return dumpedText;
    }
    
    function debug(message, obj?) {
        if (settings.debugEnabled)
            log(`Debug: ${message}`, JSON.stringify(obj));
    }

    function info(message: string) {
        toastr.info(message);
        log(`Info: ${message}`);
    }

    function success(message: string) {
        toastr.success(message);
        log(`Success: ${message}`);
    }

    function warning(message: string) {
        toastr.warning(message);
        log(`Warning: ${message}`);
    }

    function error(message: string, data?: any) {
     
        if (!data) {
            toastr.error(message);
            log(message);
            return;
        }
        else if (data.modelState) {
            var errorList = '<ul>';
            for (var key in data.modelState) {
                if (data.modelState.hasOwnProperty(key)) {
                    errorList += `<li>${data.modelState[key][0]}</li>`;
                }
            }
            errorList += '</ul>';
            toastr.error(errorList, message);
        }
        else if (data.error_description) {
            toastr.error(data.error_description, message);
            log(data.error_description, message);
        }
        else if (data.message) {
            toastr.error(data.message, message);
            log(data.message, message);
        }
        else if (data.data) {
            toastr.error(data.data, message);
            log(data.data, message);
        }
        else {
            toastr.error(data);
            log(data);
        }
    }
    
    var logger: ILogger = {
        error: error,
        info: info,
        success: success,
        warning: warning,
        debug: debug,
        dump: dump
    };

    return logger;
}]); 
