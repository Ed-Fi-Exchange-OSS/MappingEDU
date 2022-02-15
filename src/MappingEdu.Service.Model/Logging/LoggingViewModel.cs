// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Service.Model.Datatables;

namespace MappingEdu.Service.Model.Logging
{
    public class LoggingViewModel
    {
        public DateTime Date { get; set; }

        public string Level { get; set; }

        public string Message { get; set; }

        public string User { get; set; }

    }

    public class LoggingDeleteModel
    {
        public DateTime DeleteDate { get; set; }

        public string Password { get; set; }
    }


    public class LoggingDataTablesModel : DatatablesModel
    {
        public string[] Levels { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}