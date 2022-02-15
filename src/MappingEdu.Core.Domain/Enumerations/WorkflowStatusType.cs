// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.Domain.Enumerations
{
    public class WorkflowStatusType : DatabaseEnumeration<WorkflowStatusType, int>
    {
        public static readonly WorkflowStatusType Incomplete = new WorkflowStatusType(1, "Incomplete");
        public static readonly WorkflowStatusType Complete = new WorkflowStatusType(2, "Complete");
        public static readonly WorkflowStatusType Reviewed = new WorkflowStatusType(3, "Reviewed");
        public static readonly WorkflowStatusType Approved = new WorkflowStatusType(4, "Approved");
        //Forced into database in Migration UpdateTableName for MappingProjectQueueFilter needs the Id 0
        public static readonly WorkflowStatusType Unknown = new WorkflowStatusType(0, "Unknown", false, true);

        private readonly int _id;
        private readonly bool _isInDatabase;
        private readonly string _name;

        public override int Id
        {
            get { return _id; }
        }

        protected override bool IsInDatabase
        {
            get { return _isInDatabase; }
        }

        public override string Name
        {
            get { return _name; }
        }

        private WorkflowStatusType(int id, string name, bool isInDatabase = true, bool isDefault = false)
            : base(isDefault)
        {
            _id = id;
            _name = name;
            _isInDatabase = isInDatabase;
        }
    }
}