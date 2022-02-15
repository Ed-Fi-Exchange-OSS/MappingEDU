// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Interfaces app.services
//
 
interface IServices {
    animate: angular.animate.IAnimateService,
    compile: ng.ICompileService;
    cookie: any;
    datatables: IDatatablesService;
    document: ng.IDocumentService;
    errors: IHandleErrorService;
    events: IEvents;
    filter: ng.IFilterService;
    loading: any;
    location: ng.ILocationService;
    logger: ILogger;
    logging: ILoggingService;
    modal: ng.ui.bootstrap.IModalService;
    navbar: INavbarService,
    profile: IProfileService;
    q: ng.IQService,
    session: ISessionService;
    state: ng.ui.IStateService;
    template: ng.ITemplateRequestService;
    timeout: ng.ITimeoutService;
    underscore: UnderscoreStatic;
    upload: ng.angularFileUpload.IUploadService;
    utils: IUtils;
    window: ng.IWindowService;
}


// ****************************************************************************
// Module app.services
//

var m = angular.module('app.services', [
    'app.services.datatables',
    'app.services.events',
    'app.services.error-handler',
    'app.services.logger',
    'app.services.logging',
    'app.services.navbar',
    'app.services.profile',
    'app.services.session',
    'app.services.underscore',
    'app.services.utils',
    'ui.router'
]);


// ****************************************************************************
// Service 'services'
//

m.factory('services', [
    '$animate',
    '$compile',
    'ipCookie',
    'app.services.datatables',
    '$document',
    'handleErrorService',
    'events',
    '$filter',
    '$loading',
    'logger',
    'loggingService',
    '$state',
    '$location',
    '$uibModal',
    'navbar',
    'profile',
    '$q',
    'sessionService',
    '$templateRequest',
    '$timeout',
    '_',
    'Upload',
    'utils',
    '$window',
    (
        animate: angular.animate.IAnimateService,
        compile: ng.ICompileService,
        cookie: any,
        datatables: IDatatablesService,
        document: ng.IDocumentService,
        errors: IHandleErrorService,
        events: IEvents,
        filter: ng.IFilterService,
        loading: any,
        logger: ILogger,
        logging: ILoggingService,
        state: ng.ui.IStateService,
        location: ng.ILocationService,
        modal: ng.ui.bootstrap.IModalService,
        navbar: INavbarService,
        profile: IProfileService,
        q: ng.IQService,
        session: ISessionService,
        template: ng.ITemplateRequestService,
        timeout: ng.ITimeoutService,
        underscore: UnderscoreStatic,
        upload: ng.angularFileUpload.IUploadService,
        utils: IUtils,
        window: ng.IWindowService
    ) => {

        var services: IServices = {
            animate: animate,
            compile: compile,
            cookie: cookie,
            datatables: datatables,
            document: document,
            errors: errors,
            events: events,
            filter: filter,
            loading: loading,
            location: location,
            logger: logger,
            logging: logging,
            modal: modal,
            navbar: navbar,
            profile: profile,
            q: q,
            session: session,
            state: state,
            template: template,
            timeout: timeout,
            underscore: underscore,
            upload: upload,
            utils: utils,
            window: window
        };
        return services;
    }
]); 