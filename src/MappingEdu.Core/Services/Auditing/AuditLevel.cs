// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

#region

using MappingEdu.Core.Domain.Enumerations;

#endregion

namespace MappingEdu.Core.Services.Auditing
{
    public class AuditLevel : Enumeration<AuditLevel, int>
    {
        public static AuditLevel Error = new AuditLevel(1, "ERROR");
        public static AuditLevel Warning = new AuditLevel(2, "Warning");
        public static AuditLevel Info = new AuditLevel(3, "Info");
        public static AuditLevel Fatal = new AuditLevel(4, "FATAL");
        private readonly int _id;

        public override int Id
        {
            get { return _id; }
        }

        public string Level { get; private set; }

        public AuditLevel(int id, string level)
        {
            _id = id;
            Level = level;
        }

        public override string ToString()
        {
            return Level;
        }
    }
}