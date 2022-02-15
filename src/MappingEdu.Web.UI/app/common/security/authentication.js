// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

angular.module('appCommon').factory('authentication', [

    function () {
        var authenticate = this;

        authenticate.authenticate = function (username, password) {
            var promise = new Promise(function(resolve, reject) {
                if ((username == "admin" && password == "password") || (username == "user" && password == "password")) {
                    resolve({
                        access_token: "ydoNx0T8VqBCPtBgEBZI-vRWDyKaFJ09KBva9RBxvYrXJFiTBU1aCchEkzJ667UPJ-EMACuqq1z5P50SrnUdiZYpC9W5LWrdgZg_5-wv3aanYDDZOzdHQmgnEUcZCkoTsl70aFQ6tJtBnIF37acw9UiOUv77XsCgJDiyR6n1J8shOPisf-d6rSXNn19frXxuCj4aN4PhN_L4Q7uTcfgLlYv7P1llzvNPqF_nHMMvOL39RBHZ19KYJlV22J7CQukRYmq1cwTpF_gMk0wmvtyJo84gNXVet8GD-UIK9E18MXAO87k6eMMT7rPc4-E90taJ6odoQUN_iBp8y1I6lb5hsXFHNXwjlsGTC-imYfhaEZ-3DA00bgFpS05COux_PGhL",
                        expires_in: 31525999,
                        token_type: "bearer",
                        fromServer: true,
                        route: "access_token"
                    });
                } else {
                    reject({});
                }
            });
            return promise;
        }

        return authenticate;
    }
]);