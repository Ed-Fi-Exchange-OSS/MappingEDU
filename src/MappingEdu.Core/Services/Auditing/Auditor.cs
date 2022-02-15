// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using log4net;

namespace MappingEdu.Core.Services.Auditing
{
    public interface IAuditor
    {
        void Error(string message, params object[] values);

        void Warn(string message, params object[] values);

        void Info(string message, params object[] values);

        void Fatal(string message, params object[] values);

        Audit[] GetAll();
    }

    public class Auditor : IAuditor
    {
        private readonly List<Audit> _audits;
        private readonly ILog _logger = LogManager.GetLogger(typeof (Auditor));

        public Auditor()
        {
            _audits = new List<Audit>();
        }

        public void Error(string message, params object[] values)
        {
            Audit(AuditLevel.Error, message, values);
        }

        public void Warn(string message, params object[] values)
        {
            Audit(AuditLevel.Warning, message, values);
        }

        public void Info(string message, params object[] values)
        {
            Audit(AuditLevel.Info, message, values);
        }

        public void Fatal(string message, params object[] values)
        {
            Audit(AuditLevel.Fatal, message, values);
        }

        public Audit[] GetAll()
        {
            return _audits.ToArray();
        }

        private void Audit(AuditLevel level, string message, object[] values)
        {
            var audit = new Audit
            {
                Message = string.Format(message, values),
                AuditLevel = level
            };
            _logger.Info(audit.ToString());
            _audits.Add(audit);
        }
    }
}