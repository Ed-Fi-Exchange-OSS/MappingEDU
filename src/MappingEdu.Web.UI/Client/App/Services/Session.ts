// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.services.session
//

var m = angular.module('app.services.session', []);


// ****************************************************************************
// Interfaces
//
// TODO: Cast arguments properly (cpt)

interface ISessionService {
    clearSection(section: any);
    cloneFromSession(section: any, key: any);
    cloneToSession(section: any, key: any, data: any);
}


// ****************************************************************************
// Service sessionService
//

m.service('sessionService', () => {

    var session = <ISessionService> {};

    session.cloneToSession = (section, key, data) => {
        //session.data[section] = session.data[section] || [];
        //session.data[section][key] = JSON.parse(JSON.stringify(data));
        var compositeKey = section ? section + '.' + key : key;
        sessionStorage.setItem(compositeKey, JSON.stringify(data));
    }

    session.cloneFromSession = (section, key) => {
        //session.data[section] = session.data[section] || [];
        //if (key in session.data[section])
        //    return JSON.parse(JSON.stringify(session.data[section][key]));
        var compositeKey = section ? section + '.' + key : key;
        var value = sessionStorage.getItem(compositeKey);
        if (value != null)
            return JSON.parse(value);

        return undefined;
    }

    session.clearSection = section => {
        for (var i = 0; i < sessionStorage.length; i++) {
            var key = sessionStorage.key(i);
            if (key.indexOf(section + '.') === 0)
                sessionStorage.removeItem(key);
        }
    }

    return session;
});