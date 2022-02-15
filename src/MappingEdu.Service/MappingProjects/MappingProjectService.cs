// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using MappingEdu.Core.DataAccess.Repositories;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.AutoMap;
using MappingEdu.Service.Logging;
using MappingEdu.Service.Model.DataStandard;
using MappingEdu.Service.Model.Logging;
using MappingEdu.Service.Model.MappingProject;
using ProjectStatusType = MappingEdu.Core.Domain.Enumerations.ProjectStatusType;

namespace MappingEdu.Service.MappingProjects
{
    public interface IMappingProjectService
    {
        MappingProjectViewModel Clone(Guid mappingProjectId, MappingProjectCloneModel model);

        void Delete(Guid mappingProjectId);

        MappingProjectViewModel[] Get();

        MappingProjectViewModel[] Get(bool orphaned);

        string GetCreator(Guid mappingProjectId);

        List<string> GetOwners(Guid mappingProjectId);

        MappingProjectViewModel Get(Guid mappingProjectId);

        MappingProjectViewModel[] GetPublic();

        DataStandardMappingProjectsViewModel[] GetSourceMappingProjects(Guid dataStandardId);

        DataStandardMappingProjectsViewModel[] GetTargetMappingProjects(Guid dataStandardId);

        MappingProjectViewModel Post(MappingProjectCreateModel model, bool autoMap = false);

        MappingProjectViewModel Put(Guid mappingProjectId, MappingProjectEditModel project);

        void TogglePublic(Guid mappingProjectId);
    }

    public class MappingProjectService : IMappingProjectService
    {
        private readonly IAutoMapService _autoMapService;
        private readonly ILoggingService _loggingService;
        private readonly IMapper _mapper;
        private readonly IMappingProjectRepository _mappingProjectRepository;
        private readonly IUserRepository _userRepository;

        public MappingProjectService(IMappingProjectRepository mappingProjectRepository, IMapper mapper, ILoggingService loggingService, IAutoMapService autoMapService, IUserRepository userRepository)
        {
            _loggingService = loggingService;
            _mappingProjectRepository = mappingProjectRepository;
            _mapper = mapper;
            _autoMapService = autoMapService;
            _userRepository = userRepository;

        }

        public MappingProjectViewModel Clone(Guid mappingProjectId, MappingProjectCloneModel model)
        {
            if (string.IsNullOrWhiteSpace(model.CloneProjectName))
                throw new ArgumentNullException();

            var project = GetMappingProject(mappingProjectId);
            if (!Principal.Current.IsAdministrator && !project.HasAccess(MappingProjectUser.MappingProjectUserRole.View))
                throw new SecurityException("User needs at least View Access to peform this action");

            var clone = project.Clone();
            clone.ProjectName = model.CloneProjectName;
            clone.Users.Add(new MappingProjectUser(clone.MappingProjectId, Principal.Current.UserId, MappingProjectUser.MappingProjectUserRole.Owner));

            _mappingProjectRepository.Add(clone);
            _mappingProjectRepository.SaveChanges();

            return Get(clone.MappingProjectId);
        }

        public void Delete(Guid mappingProjectId)
        {
            // Here we will implement the "fake" delete
            var mappingProject = GetMappingProject(mappingProjectId, MappingProjectUser.MappingProjectUserRole.Owner);
            mappingProject.IsActive = false;
            _mappingProjectRepository.SaveChanges();

            _loggingService.Post(new LoggingCreateModel
            {
                Source = "Delete",
                Message = string.Format("Deleted Mapping Project {0} ({1})", mappingProject.ProjectName, mappingProject.MappingProjectId)
            });
        }

        public MappingProjectViewModel[] Get()
        {
            var mappingProjects = _mappingProjectRepository.GetAll();

            var viewModels = new List<MappingProjectViewModel>();

            foreach (var mappingProject in mappingProjects)
            {
                var viewModel = _mapper.Map<MappingProjectViewModel>(mappingProject);

                viewModel.CreateBy = GetFormattedCreator(mappingProject);

                viewModels.Add(viewModel);
            }

            return viewModels.ToArray();
        }

        public MappingProjectViewModel[] Get(bool orphaned)
        {
            var mappingProjects = orphaned ? _mappingProjectRepository.GetAll().Where(x => x.Users.Count == 0)
                : _mappingProjectRepository.GetAll().Where(x => x.Users.Count > 0);
            var viewModel = _mapper.Map<MappingProjectViewModel[]>(mappingProjects);
            return viewModel;
        }

        public MappingProjectViewModel Get(Guid mappingProjectId)
        {
            var mappingProject = GetMappingProject(mappingProjectId);
            var viewModel = _mapper.Map<MappingProjectViewModel>(mappingProject);
            return viewModel;
        }
        public string GetCreator(Guid mappingProjectId)
        {
            var mappingProject = GetMappingProject(mappingProjectId);

            return GetFormattedCreator(mappingProject);
        }

        public List<string> GetOwners(Guid mappingProjectId)
        {
            var mappingProject = GetMappingProject(mappingProjectId);
            var owners = mappingProject.Users.Where(x => x.Role == MappingProjectUser.MappingProjectUserRole.Owner);
            return owners.Select(x => x.User.FirstName + " " + x.User.LastName).ToList();
        }

        public MappingProjectViewModel[] GetPublic()
        {
            var mappingProjects = _mappingProjectRepository.GetAll().Where(x => x.IsPublic);
            var viewModel = _mapper.Map<MappingProjectViewModel[]>(mappingProjects);
            return viewModel;
        }

        public DataStandardMappingProjectsViewModel[] GetSourceMappingProjects(Guid dataStandardId)
        {
            var sourceMappingProjects = _mappingProjectRepository.GetSourceMappingProjects(dataStandardId);
            var viewModels = _mapper.Map<DataStandardMappingProjectsViewModel[]>(sourceMappingProjects);
            return viewModels;
        }

        public DataStandardMappingProjectsViewModel[] GetTargetMappingProjects(Guid dataStandardId)
        {
            var targetMappingProjects = _mappingProjectRepository.GetTargetMappingProjects(dataStandardId);
            var viewModels = _mapper.Map<DataStandardMappingProjectsViewModel[]>(targetMappingProjects);
            return viewModels;
        }

        public MappingProjectViewModel Post(MappingProjectCreateModel model, bool autoMap = false)
        {
            if(Principal.Current.IsGuest)
                throw new SecurityException("A guest account cannot create a mapping project");

            // Validation - Project Names must be unique
            if (_mappingProjectRepository.GetAllQueryable().Any(
                x => x.ProjectName.ToLower() == model.ProjectName.ToLower() && x.IsActive))
            {
                throw new ArgumentException(
                    "The Project Name must be unique. Please provide a new name for this Mapping Project.");
            }

            var mappingProject = new MappingProject
            {
                ProjectName = model.ProjectName,
                Description = model.Description,
                SourceDataStandardMappedSystemId = model.SourceDataStandardId,
                TargetDataStandardMappedSystemId = model.TargetDataStandardId,
                ProjectStatusTypeId = ProjectStatusType.Active.Id,
                IsActive = true,
                Users = new List<MappingProjectUser>(),
                UserUpdates = new List<MappingProjectUpdate>(),
                FlaggedBy = new List<ApplicationUser>(),
                EntityHints = new List<EntityHint>(),
                MappingProjectSynonyms = new List<MappingProjectSynonym>()
            };

            if (Principal.Current.UserId != "0") //TODO: Refactor for System Accounts
            {
                var user = new MappingProjectUser(mappingProject.MappingProjectId, Principal.Current.UserId, MappingProjectUser.MappingProjectUserRole.Owner);
                mappingProject.Users.Add(user);
            }

            _mappingProjectRepository.Add(mappingProject);
            _mappingProjectRepository.SaveChanges();

            if (autoMap)
                _autoMapService.CreateAutoMappings(model.SourceDataStandardId, model.TargetDataStandardId, mappingProject.MappingProjectId);

            return Get(mappingProject.MappingProjectId);
        }

        public void TogglePublic(Guid mappingProjectId)
        {
            if (!Principal.Current.IsAdministrator)
                throw new SecurityException("Only an admin can change a mapping projects public status");

            var mappingProject = GetMappingProject(mappingProjectId);

            if (!mappingProject.IsPublic) { 
                mappingProject.SourceDataStandard.IsPublic = true;
                mappingProject.TargetDataStandard.IsPublic = true;
            }

            mappingProject.IsPublic = !mappingProject.IsPublic;
            _mappingProjectRepository.SaveChanges();
        }

        public MappingProjectViewModel Put(Guid mappingProjectId, MappingProjectEditModel editModel)
        {
            // Validation - Project Names must be unique
            if (_mappingProjectRepository.GetAllQueryable().Any(
                x => x.ProjectName.ToLower() == editModel.ProjectName.ToLower()
                     && x.MappingProjectId != mappingProjectId && x.IsActive))
            {
                throw new ArgumentException(
                    "The Project Name must be unique. Please provide a new name for this Mapping Project.");
            }

            var mappingProject = GetMappingProject(mappingProjectId, MappingProjectUser.MappingProjectUserRole.Edit);
            MapEditModelToMappingProjectModel(editModel, mappingProject);
            _mappingProjectRepository.SaveChanges();

            return Get(mappingProjectId);
        }

        private MappingProject GetMappingProject(Guid mappingProjectId, MappingProjectUser.MappingProjectUserRole role = MappingProjectUser.MappingProjectUserRole.Guest)
        {
            var mappingProject = _mappingProjectRepository.Get(mappingProjectId);

            if (mappingProject == null)
                throw new Exception(string.Format("Mapping Project with id '{0}' does not exist.", mappingProjectId));

            if (!Principal.Current.IsAdministrator && !mappingProject.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0}-level access to peform this action", role));

            return mappingProject;
        }

        private static void MapEditModelToMappingProjectModel(MappingProjectEditModel editModel, MappingProject mappingProject)
        {
            mappingProject.ProjectName = editModel.ProjectName;
            mappingProject.Description = editModel.Description;
            mappingProject.TargetDataStandardMappedSystemId = editModel.TargetDataStandardId;
            mappingProject.SourceDataStandardMappedSystemId = editModel.SourceDataStandardId;
            mappingProject.ProjectStatusTypeId = editModel.ProjectStatusTypeId;
        }

        private string GetFormattedCreator(MappingProject mappingProject)
        {
            ApplicationUser creator = null;
            if (mappingProject.CreateById.HasValue)
            {
                creator = _userRepository.GetAllUsers().FirstOrDefault(x => x.Id == mappingProject.CreateById.Value.ToString());
                if (creator != null) return creator.FirstName + " " + creator.LastName;
            }

            creator = _userRepository.GetAllUsers().FirstOrDefault(x => x.Email == mappingProject.CreateBy);
            if (creator != null) return creator.FirstName + " " + creator.LastName;

            return mappingProject.CreateBy;
        }
    }
}