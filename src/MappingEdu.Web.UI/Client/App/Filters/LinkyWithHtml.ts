// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


// ****************************************************************************
// Module app.filters.linky-with-html
//

var m = angular.module('app.filters.linky-with-html', []);


// ****************************************************************************
// Filter linkyWithHtml
//

m.filter('linkyWithHtml', ['$filter', $filter => (value) => ($filter('linky')(value, '_blank').replace(/\&gt;/g, '>').replace(/\&lt;/g, '<'))]);
 