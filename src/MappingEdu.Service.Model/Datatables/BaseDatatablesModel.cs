// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;

namespace MappingEdu.Service.Model.Datatables
{
    public class DatatablesColumn
    {
        public string data { get; set; }
        public string name { get; set; }
        public bool orderable { get; set; }
        public DatatablesSearch search { get; set; }
        public bool searchable { get; set; }
    }

    public class DatatablesSearch
    {
        public bool regex { get; set; }
        public string value { get; set; }
    }

    public class DatatablesOrder
    {
        public int column { get; set; }
        public string dir { get; set; }
    }

    public class DatatablesModel
    {
        public List<DatatablesColumn> columns { get; set; }
        public int draw { get; set; }
        public int length { get; set; }
        public List<DatatablesOrder> order { get; set; }
        public DatatablesSearch search { get; set; }
        public int start { get; set; } 
    }

    public class DatatablesReturnModel<T>
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<T> data { get; set; }
    }
}