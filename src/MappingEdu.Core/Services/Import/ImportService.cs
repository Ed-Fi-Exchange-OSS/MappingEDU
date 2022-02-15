// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Repositories;
using MappingEdu.Core.Services.Auditing;

namespace MappingEdu.Core.Services.Import
{
    public interface IImportService
    {
        ImportResult Import(SerializedMappedSystem serializedMappedSystem, ImportOptions options);
    }

    public class ImportService : IImportService
    {
        private readonly IAuditor _auditor;
        private readonly ISerializedToMappedSystemMapper _mappedSystemMapper;
        private readonly IRepository<MappedSystem> _repository;
        
        public ImportService(ISerializedToMappedSystemMapper mappedSystemMapper, IRepository<MappedSystem> repository, IAuditor auditor)
        {
            _mappedSystemMapper = mappedSystemMapper;
            _repository = repository;
            _auditor = auditor;
        }

        public ImportResult Import(SerializedMappedSystem serializedMappedSystem, ImportOptions options)
        {
            MappedSystem mappedSystem = null;
            if (serializedMappedSystem.Id.HasValue)
                mappedSystem = _repository.Get(serializedMappedSystem.Id.Value);

            if (mappedSystem == null && options.UpsertBasedOnName)
                mappedSystem = _repository.GetAll().FirstOrDefault(x => x.SystemName == serializedMappedSystem.Name && x.SystemVersion == serializedMappedSystem.Version);

            if (mappedSystem == null)
            {
                mappedSystem = new MappedSystem
                {
                    SystemName = serializedMappedSystem.Name,
                    SystemVersion = serializedMappedSystem.Version
                };
                _repository.Add(mappedSystem);
                _auditor.Info("Created new mapped system {0} ({1})", serializedMappedSystem.Name, serializedMappedSystem.Version);
            }

            _mappedSystemMapper.Map(serializedMappedSystem, mappedSystem, options);

            _repository.SaveChanges();

            return new ImportResult
            {
                Errors = _auditor.GetAll().Where(m => Equals(m.AuditLevel, AuditLevel.Error)).Select(x => x.Message).ToArray(),
                Success = _auditor.GetAll().All(x => Equals(x.AuditLevel, AuditLevel.Info) || Equals(x.AuditLevel, AuditLevel.Warning)),
                Warnings = _auditor.GetAll().Where(m => Equals(m.AuditLevel, AuditLevel.Warning)).Select(x => x.Message).ToArray(),
                MappedSystemId = mappedSystem.MappedSystemId
            };
        }
    }
}