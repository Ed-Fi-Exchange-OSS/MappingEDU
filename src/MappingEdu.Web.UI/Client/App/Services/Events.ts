// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Interfaces app.services.events 
//

interface IEvents {
    on(event: string, callback: any, unsubscribeOnResponse?: boolean): void;
    emit(event: string, extraParameters?: any);
}


// ****************************************************************************
// Module app.services.events
//

var m = angular.module('app.services.events', []);


// ****************************************************************************
// Service 'events' 
//

m.factory('events', () => {
    var el = document.createElement('div');
    var eventsListener: IEvents = {
        on(event, callback, unsubscribeOnResponse?) {
            $(el).on(event, function () {
                if (unsubscribeOnResponse) {
                    $(el).off(event);
                }
                callback.apply(this, arguments); //invoke client callback
            });
        },
        emit(event, extraParameters?) {
            $(el).trigger(event, extraParameters);
        }
    };
    return eventsListener;
});  