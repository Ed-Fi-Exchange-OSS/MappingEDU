// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using MappingEdu.Core.Repositories;

namespace MappingEdu.Core.Services.CustomMigration
{
    public interface ICustomMigrationService
    {
        string[] AppliedMigrations { get; }

        bool DatabaseIsUpToDate { get; }

        string[] LocalMigrations { get; }

        string[] PendingMigrations { get; }

        void ApplyPendingMigrations();
    }

    public class CustomMigrationService : ICustomMigrationService
    {
        private readonly ICustomMigration[] _customMigrations;
        private readonly IRepository<Domain.CustomMigration> _repository;

        public CustomMigrationService(IRepository<Domain.CustomMigration> repository, ICustomMigration[] customMigrations)
        {
            _repository = repository;
            _customMigrations = customMigrations;
        }

        public bool DatabaseIsUpToDate
        {
            get { return !PendingMigrations.Any(); }
        }

        public string[] LocalMigrations
        {
            get
            {
                var names = _customMigrations.OrderBy(x => x.Name).Select(x => x.Name);
                return names.ToArray();
            }
        }

        public string[] AppliedMigrations
        {
            get
            {
                var customMigrations = _repository.GetAll().OrderBy(x => x.Name);
                var names = customMigrations.Select(x => x.Name);
                return names.ToArray();
            }
        }

        public string[] PendingMigrations
        {
            get
            {
                var localMigrations = LocalMigrations;
                var appliedMigrations = AppliedMigrations;
                var pendingMigrations = localMigrations.Where(x => !appliedMigrations.Contains(x));
                return pendingMigrations.ToArray();
            }
        }

        public void ApplyPendingMigrations()
        {
            var pendingMigrationNames = PendingMigrations;

            var pendingMigrations = _customMigrations.Where(x => pendingMigrationNames.Contains(x.Name)).OrderBy(x => x.Name).ToArray();
            foreach (var pendingMigration in pendingMigrations)
            {
                pendingMigration.Apply();
                var customMigration = new Domain.CustomMigration {Name = pendingMigration.Name};
                _repository.Add(customMigration);
                _repository.SaveChanges();
            }
        }
    }
}