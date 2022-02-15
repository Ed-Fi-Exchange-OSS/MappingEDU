// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Service.Model.Migration
{
    public class MigrationStatusModel
    {
        public string[] AppliedCustomMigrations { get; set; }

        public string[] AppliedEntityFrameworkMigrations { get; set; }

        public string DatabaseCatalog { get; set; }

        public string DatabaseServer { get; set; }

        public string ErrorMessage { get; set; }

        public string[] PendingCustomMigrations { get; set; }

        public string[] PendingEntityFrameworkMigrations { get; set; }

        public bool ShowAppliedCustomMigrations
        {
            get { return AppliedCustomMigrations.Length > 0; }
        }

        public bool ShowAppliedEntityFrameworkMigrations
        {
            get { return AppliedEntityFrameworkMigrations.Length > 0; }
        }

        public bool ShowErrorMessage
        {
            get { return !string.IsNullOrWhiteSpace(ErrorMessage); }
        }

        public bool ShowPendingCustomMigrations
        {
            get { return PendingCustomMigrations.Length > 0; }
        }

        public bool ShowPendingEntityFrameworkMigrations
        {
            get { return PendingEntityFrameworkMigrations.Length > 0; }
        }

        public MigrationStatusModel()
        {
            PendingEntityFrameworkMigrations = new string[0];
            AppliedEntityFrameworkMigrations = new string[0];
        }
    }
}