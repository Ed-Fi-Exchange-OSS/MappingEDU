// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Security;
using MappingEdu.Common.Exceptions;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.EntityHints;
using MappingEdu.Service.Model.MappingProject;

namespace MappingEdu.Service.MappingProjects
{
    public interface IMappingProjectSynonymService
    {
        void Delete(Guid mappingProjectId, Guid synonymId);

        MappingProjectSynonymModel Get(Guid synonymId, Guid mappingProjectId);

        MappingProjectSynonymModel[] Get(Guid mappingProjectId);

        MappingProjectSynonymModel Post(Guid mappingProjectId, MappingProjectSynonymModel model);

        MappingProjectSynonymModel Put(Guid mappingProjectId, Guid synonymId, MappingProjectSynonymModel model);
    }

    public class MappingProjectSynonymService : IMappingProjectSynonymService {

        private readonly IRepository<MappingProjectSynonym> _synonymRepository;
        private readonly IMappingProjectRepository _mappingProjectRepository;

        public MappingProjectSynonymService(IRepository<MappingProjectSynonym> synonymRepository, IMappingProjectRepository mappingProjectRepository)
        {
            _synonymRepository = synonymRepository;
            _mappingProjectRepository = mappingProjectRepository;
        }

        public MappingProjectSynonymModel Get(Guid synonymId, Guid mappingProjectId)
        {
            var synonym = GetSynonym(synonymId, mappingProjectId);
            return new MappingProjectSynonymModel
            {
                MappingProjectSynonymId = synonym.MappingProjectSynonymId,
                MappingProjectId = synonym.MappingProjectId,
                SourceWord = synonym.SourceWord,
                TargetWord = synonym.TargetWord
            };
        }

        public MappingProjectSynonymModel[] Get(Guid mappingProjectId)
        {
            var mappingProject = GetMappingProject(mappingProjectId);

            var synonyms = _synonymRepository.GetAllQueryable().Where(x => x.MappingProjectId == mappingProjectId);

            return synonyms.ToList().Select(x =>
                new MappingProjectSynonymModel
                {
                    MappingProjectSynonymId = x.MappingProjectSynonymId,
                    MappingProjectId = x.MappingProjectId,
                    SourceWord = x.SourceWord,
                    TargetWord = x.TargetWord
                }).ToArray();
        }

        public MappingProjectSynonymModel Post(Guid mappingProjectId, MappingProjectSynonymModel model)
        {
            GetMappingProject(mappingProjectId, MappingProjectUser.MappingProjectUserRole.Edit);

            var synonym = new MappingProjectSynonym
            {
                MappingProjectId = mappingProjectId,
                SourceWord = model.SourceWord,
                TargetWord = model.TargetWord
            };

            _synonymRepository.Add(synonym);
            _synonymRepository.SaveChanges();

            model.MappingProjectSynonymId = synonym.MappingProjectSynonymId;

            return model;
        }

        public MappingProjectSynonymModel Put(Guid mappingProjectId, Guid synonymId, MappingProjectSynonymModel model)
        {
            var synonym = GetSynonym(synonymId, mappingProjectId, MappingProjectUser.MappingProjectUserRole.Edit);

            synonym.SourceWord = model.SourceWord;
            synonym.TargetWord = model.TargetWord;

            _synonymRepository.SaveChanges();

            return model;
        }

        public void Delete(Guid mappingProjectId, Guid synonymId)
        {
            var entityHint = GetSynonym(synonymId, mappingProjectId, MappingProjectUser.MappingProjectUserRole.Edit);

            _synonymRepository.Delete(entityHint);
            _synonymRepository.SaveChanges();
        }

        private MappingProject GetMappingProject(Guid mappingProjectId, MappingProjectUser.MappingProjectUserRole role = MappingProjectUser.MappingProjectUserRole.Guest)
        {
            var mappingProject = _mappingProjectRepository.Get(mappingProjectId);
            if (null == mappingProject)
                throw new NotFoundException(string.Format("Mapping Project with id '{0}' does not exist.", mappingProjectId));
            if (!Principal.Current.IsAdministrator && !mappingProject.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));
            return mappingProject;
        }

        private MappingProjectSynonym GetSynonym(Guid synonymId, Guid mappingProjectId, MappingProjectUser.MappingProjectUserRole role = MappingProjectUser.MappingProjectUserRole.Guest)
        {
            var synonym = _synonymRepository.Get(synonymId);
            if (null == synonym)
                throw new NotFoundException(string.Format("Mapping Project Synonym with id '{0}' does not exist.", synonymId));
            if (synonym.MappingProjectId != mappingProjectId)
                throw new NotFoundException(string.Format("Mapping Project with id '{0}' does not have EMapping Project Synonym of '{1}'.", mappingProjectId, synonymId));
            if (!Principal.Current.IsAdministrator && !synonym.MappingProject.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));

            return synonym;
        }

    }
}