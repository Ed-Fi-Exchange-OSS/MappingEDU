// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using MappingEdu.Common.Exceptions;
using MappingEdu.Core.DataAccess.Repositories;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Logging;
using MappingEdu.Service.Model.DataStandard;
using MappingEdu.Service.Model.Logging;

namespace MappingEdu.Service.MappedSystems
{
    public interface IDataStandardService
    {
        void Delete(Guid mappedSystemId);

        DataStandardViewModel[] Get();

        DataStandardViewModel[] Get(bool orphaned);

        DataStandardViewModel Get(Guid mappedSystemId);

        string GetCreator(Guid mappedSystemId);

        List<string> GetOwners(Guid mappedSystemId);

        DataStandardViewModel[] GetAllWithoutNextVersions();

        DataStandardViewModel[] GetPublic();

        bool IsExtended(Guid mappedSystemId);

        DataStandardViewModel Post(DataStandardCreateModel model);

        DataStandardViewModel Put(Guid mappedSystemId, DataStandardEditModel system);

        void TogglePublic(Guid mappedSystemId);

        void TogglePublicExtensions(Guid mappedSystemId);
    }

    public class DataStandardService : IDataStandardService
    {
        private readonly ILoggingService _loggingService;
        private readonly IMappedSystemRepository _mappedSystemRepository;
        private readonly ISystemItemRepository _systemItemRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public DataStandardService(IMappedSystemRepository mappedSystemRepository, IMapper mapper, ILoggingService loggingService, ISystemItemRepository systemItemRepository, IUserRepository userRepository)
        {
            _loggingService = loggingService;
            _mappedSystemRepository = mappedSystemRepository;
            _mapper = mapper;
            _systemItemRepository = systemItemRepository;
            _userRepository = userRepository;
        }

        public void Delete(Guid mappedSystemId)
        {
            var mappedSystem = GetMappedSystem(mappedSystemId, MappedSystemUser.MappedSystemUserRole.Owner);

            var projects = new List<MappingProject>();
            projects.AddRange(mappedSystem.MappingProjectsWhereSource.Where(x => x.IsActive));
            projects.AddRange(mappedSystem.MappingProjectsWhereTarget.Where(x => x.IsActive));

            if (projects.Any())
            {
                var message = projects.Aggregate("The following projects must be deleted first: ",
                    (current, project) => current + (project.ProjectName + ", "));

                throw new BusinessException(message.Substring(0, message.Length - 2));
            }

            // here we will implement the "fake" delete
            mappedSystem.IsActive = false;

            _mappedSystemRepository.SaveChanges();

            _loggingService.Post(new LoggingCreateModel()
            {
                Source = "Delete",
                Message = string.Format("Deleted Data Standard {0} ({1})", mappedSystem.SystemName, mappedSystem.MappedSystemId)
            });
        }

        public DataStandardViewModel[] Get()
        {
            var mappedSystems = _mappedSystemRepository.GetAll();
            var viewModel = _mapper.Map<DataStandardViewModel[]>(mappedSystems);
            return viewModel;
        }

        public DataStandardViewModel[] Get(bool orphaned)
        {
            var mappedSystems = (orphaned) ? _mappedSystemRepository.GetAll().Where(x => x.Users.Count == 0)
                : _mappedSystemRepository.GetAll().Where(x => x.Users.Count > 0);
            var viewModel = _mapper.Map<DataStandardViewModel[]>(mappedSystems);
            return viewModel;
        }

        public DataStandardViewModel Get(Guid mappedSystemId)
        {
            var mappedSystem = GetMappedSystem(mappedSystemId);
            var viewModel = _mapper.Map<DataStandardViewModel>(mappedSystem);

            var next = GetNextMappedSystem(mappedSystemId);
            if (next == null) return viewModel;

            viewModel.NextDataStandard = _mapper.Map<DataStandardViewModel>(next);;
            viewModel.NextDataStandardId = next.MappedSystemId;
            return viewModel;
        }

        public string GetCreator(Guid mappedSystemId)
        {
            var mappedSystem = GetMappedSystem(mappedSystemId);
            ApplicationUser creator = null;
            if (mappedSystem.CreateById.HasValue)
            {
                creator = _userRepository.GetAllUsers().FirstOrDefault(x => x.Id == mappedSystem.CreateById.Value.ToString());
                if (creator != null) return creator.FirstName + " " + creator.LastName;
            }

            creator = _userRepository.GetAllUsers().FirstOrDefault(x => x.Email == mappedSystem.CreateBy);
            if (creator != null) return creator.FirstName + " " + creator.LastName;

            return mappedSystem.CreateBy;
        }

        public List<string> GetOwners(Guid mappedSystemId)
        {
            var mappedSystem = GetMappedSystem(mappedSystemId);
            var owners = mappedSystem.Users.Where(x => x.Role == MappedSystemUser.MappedSystemUserRole.Owner);
            return owners.Select(x => x.User.FirstName + " " + x.User.LastName).ToList();
        }


        public DataStandardViewModel[] GetAllWithoutNextVersions()
        {
            var previousStandardIds = _mappedSystemRepository.GetAll()
                .Where(x => x.PreviousMappedSystemId.HasValue && x.IsActive).Select(x => x.PreviousMappedSystemId);
            var standards = _mappedSystemRepository.GetAll().Where(x => !previousStandardIds.Contains(x.MappedSystemId) && x.IsActive);
            var viewModels = _mapper.Map<DataStandardViewModel[]>(standards);
            return viewModels;
        }

        public DataStandardViewModel[] GetPublic()
        {
            var mappedSystems = _mappedSystemRepository.GetAll().Where(x => x.IsPublic);
            var viewModel = _mapper.Map<DataStandardViewModel[]>(mappedSystems);
            return viewModel;
        }

        public bool IsExtended(Guid mappedSystemId)
        {
            GetMappedSystem(mappedSystemId);
            return _systemItemRepository.GetAllQueryable().Any(x => x.MappedSystemId == mappedSystemId && x.IsExtended && !x.MappedSystemExtensionId.HasValue);
        }


        public DataStandardViewModel Post(DataStandardCreateModel model)
        {
            if (Principal.Current.IsGuest)
                throw new SecurityException("A guest cannot create a data standard");

            if (IsDuplicateNameAndVersion(Guid.Empty, model.SystemName, model.SystemVersion))
                throw new Exception("Data Standard Name and Version combination must be unique.");

            var mappedSystem = new MappedSystem
            {
                SystemName = model.SystemName,
                SystemVersion = model.SystemVersion,
                PreviousMappedSystemId = model.PreviousDataStandardId,
                IsActive = true,
                Users = new List<MappedSystemUser>(),
                FlaggedBy = new List<ApplicationUser>()
            };

            if (Principal.Current.UserId != "0") //TODO Refactor for System Account
            {
                var user = new MappedSystemUser(mappedSystem.MappedSystemId, Principal.Current.UserId, MappedSystemUser.MappedSystemUserRole.Owner);
                mappedSystem.Users.Add(user);
            }

            _mappedSystemRepository.Add(mappedSystem);
            _mappedSystemRepository.SaveChanges();

            return Get(mappedSystem.MappedSystemId);
        }

        public DataStandardViewModel Put(Guid mappedSystemId, DataStandardEditModel system)
        {
            var mappedSystem = GetMappedSystem(mappedSystemId, MappedSystemUser.MappedSystemUserRole.Edit);

            if (IsDuplicateNameAndVersion(mappedSystemId, system.SystemName, system.SystemVersion))
                throw new Exception("Data Standard Name and Version combination must be unique.");
            
            MapEditModelToMappedSystemModel(system, mappedSystem);

            if (system.PreviousDataStandardId.HasValue && WillCauseSelfReferencingLoop(mappedSystemId, system.PreviousDataStandardId.Value))
                throw new BusinessException("Previous Data Standard will cause a self referencing loop");

            _mappedSystemRepository.SaveChanges();

            return Get(mappedSystemId);
        }

        public void TogglePublic(Guid mappedSystemId)
        {
            if (!Principal.Current.IsAdministrator)
                throw new SecurityException("Only an admin can change a standards public status");

            var mappedSystem = GetMappedSystem(mappedSystemId);

            if (mappedSystem.IsPublic)
            {
                var projects = mappedSystem.MappingProjectsWhereSource.Where(x => x.IsPublic && x.IsActive).ToList();
                projects.AddRange(mappedSystem.MappingProjectsWhereTarget.Where(x => x.IsPublic && x.IsActive));

                if (projects.Any())
                {
                    var message = projects.Aggregate("The following projects must have public access removed first: ",
                        (current, project) => current + (project.ProjectName + ", "));


                    throw new BusinessException(message.Substring(0, message.Length - 2));
                }
            }


            mappedSystem.IsPublic = !mappedSystem.IsPublic;
            _mappedSystemRepository.SaveChanges();
        }

        public void TogglePublicExtensions(Guid mappedSystemId)
        {
            if (!Principal.Current.IsAdministrator)
                throw new SecurityException("Only an admin can change a standards public status");

            var mappedSystem = GetMappedSystem(mappedSystemId);
            mappedSystem.AreExtensionsPublic = !mappedSystem.AreExtensionsPublic;
            _mappedSystemRepository.SaveChanges();
        }

        private MappedSystem GetMappedSystem(Guid mappedSystemId, MappedSystemUser.MappedSystemUserRole role = MappedSystemUser.MappedSystemUserRole.Guest)
        {
            var mappedSystem = _mappedSystemRepository.Get(mappedSystemId);

            if (mappedSystem == null)
                throw new Exception(string.Format("Mapped System with id '{0}' does not exist.", mappedSystemId));

            if (!Principal.Current.IsAdministrator && !mappedSystem.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));

            if (!mappedSystem.IsActive)
                throw new Exception(string.Format("Mapped System with id '{0}' is marked as deleted.", mappedSystemId));

            return mappedSystem;
        }

        private MappedSystem GetNextMappedSystem(Guid mappedSystemId)
        {
            var mappedSystem = _mappedSystemRepository.GetAllQueryable()
                .FirstOrDefault(m => m.PreviousMappedSystemId != null && m.IsActive && m.PreviousMappedSystemId == mappedSystemId);

            return mappedSystem;
        }

        private bool IsDuplicateNameAndVersion(Guid id, string name, string version)
        {
            var mappedSystems = _mappedSystemRepository.GetAll();
            var duplicate =
                mappedSystems.FirstOrDefault(
                    ms => string.Equals(ms.SystemName, name, StringComparison.OrdinalIgnoreCase)
                          && string.Equals(ms.SystemVersion, version, StringComparison.OrdinalIgnoreCase) && ms.IsActive);

            if (duplicate != null && id != Guid.Empty)
                duplicate = duplicate.MappedSystemId != id ? duplicate : null;

            return null != duplicate;
        }

        private static void MapEditModelToMappedSystemModel(DataStandardEditModel editModel, MappedSystem mappedSystem)
        {
            mappedSystem.SystemName = editModel.SystemName;
            mappedSystem.SystemVersion = editModel.SystemVersion;
            mappedSystem.PreviousMappedSystemId = editModel.PreviousDataStandardId;
        }

        private bool WillCauseSelfReferencingLoop(Guid standardId, Guid previousStandardId)
        {
            var standard = _mappedSystemRepository.Get(standardId);
            while (standard.PreviousMappedSystemId.HasValue)
            {
                if (standard.PreviousMappedSystemId.Value == standardId)
                    return true;

                standard = standard.PreviousVersionMappedSystem;
            }
            return false;
        }
    }
}