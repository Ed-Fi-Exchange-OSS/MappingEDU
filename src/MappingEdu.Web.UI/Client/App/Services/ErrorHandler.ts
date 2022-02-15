// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.services.error-handler
//

var m = angular.module('app.services.error-handler', []);


// ****************************************************************************
// Service handleErrorService
//

interface IHandleErrorService {
    addError(error, viewModel): void;
    clearErrors(viewModel): void;
    handleErrors(error, viewModel): void;
}

m.service('handleErrorService', ['_', (_) => {

    function getExceptionMessages(errorData, viewModel) {
        if (_.isUndefined(errorData) || _.isUndefined(errorData.ExceptionMessage)) {
            return false;
        }

        viewModel.errorData.errors.push(errorData.ExceptionMessage.trim());
        getExceptionMessages(errorData.InnerException, viewModel);
        return true;
    }

    function getErrorMessages(error, viewModel) {
        if (_.isUndefined(error) || _.isUndefined(error.data)) {
            viewModel.errorData.errors.push('Unknown system error');
            return;
        }

        if (getExceptionMessages(error, viewModel)) {
            return;
        }

        if (getExceptionMessages(error.data, viewModel)) {
            return;
        }

        if (!_.isUndefined(error.data.Message)) {
            viewModel.errorData.errors.push(error.data.Message);
        }

        if (!_.isUndefined(error.data.MessageDetail)) {
            viewModel.errorData.errors.push(error.data.MessageDetail);
            return;
        }

        if (!_.isUndefined(error.data.ValidationErrors)) {
            viewModel.errorData.errors = _.map(error.data.ValidationErrors, validationError => validationError.ErrorMessage);
            return;
        }

        viewModel.errorData.errors.push('Unknown system error');
    }

    function clearErrors(viewModel: any) {
        viewModel.errorData = viewModel.errorData || {};
        viewModel.errorData.errors = [];
        viewModel.errorData.hasError = false;
    }

    // service

    return <IHandleErrorService>{

        handleErrors(error, viewModel) {
            console.log(error);
            clearErrors(viewModel);
            getErrorMessages(error, viewModel);
            viewModel.errorData.hasError = true;
        },

        addError(error, viewModel) {
            console.log(error);
            viewModel.errorData = viewModel.errorData || {};
            viewModel.errorData.errors = viewModel.errorData.errors || [];

            if (!_.contains(viewModel.errorData.errors, error))
                viewModel.errorData.errors.push(error);

            viewModel.errorData.hasError = true;
        },

        clearErrors: clearErrors
    }

}]);
