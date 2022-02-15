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
using MappingEdu.Service.Model.MappingProject;

namespace MappingEdu.Service.MappingProjects
{
    public interface IMappingProjectTemplateService
    {
        void Delete(Guid mappingProjectId, Guid templateId);

        MappingProjectTemplateModel Get(Guid templateId, Guid mappingProjectId);

        MappingProjectTemplateModel[] Get(Guid mappingProjectId);

        MappingProjectTemplateModel Post(Guid mappingProjectId, MappingProjectTemplateModel model);

        MappingProjectTemplateModel Put(Guid mappingProjectId, Guid synonymId, MappingProjectTemplateModel model);
    }

    public class MappingProjectTemplateService : IMappingProjectTemplateService
    {

        private readonly IRepository<MappingProjectTemplate> _templateRepository;
        private readonly IMappingProjectRepository _mappingProjectRepository;

        public MappingProjectTemplateService(IRepository<MappingProjectTemplate> templateRepository, IMappingProjectRepository mappingProjectRepository)
        {
            _templateRepository = templateRepository;
            _mappingProjectRepository = mappingProjectRepository;
        }

        public MappingProjectTemplateModel Get(Guid templateId, Guid mappingProjectId)
        {
            var template = GetTemplate(templateId, mappingProjectId);
            return new MappingProjectTemplateModel
            {
                MappingProjectTemplateId = template.MappingProjectTemplateId,
                MappingProjectId = template.MappingProjectId,
                Template = template.Template,
                Title = template.Title
            };
        }

        public MappingProjectTemplateModel[] Get(Guid mappingProjectId)
        {
            var mappingProject = GetMappingProject(mappingProjectId);

            var synonyms = _templateRepository.GetAllQueryable().Where(x => x.MappingProjectId == mappingProjectId);

            return synonyms.ToList().Select(x =>
                new MappingProjectTemplateModel
                {
                    MappingProjectTemplateId = x.MappingProjectTemplateId,
                    MappingProjectId = x.MappingProjectId,
                    Template = x.Template,
                    Title = x.Title
                }).ToArray();
        }

        public MappingProjectTemplateModel Post(Guid mappingProjectId, MappingProjectTemplateModel model)
        {
            GetMappingProject(mappingProjectId, MappingProjectUser.MappingProjectUserRole.Edit);

            var template = new MappingProjectTemplate
            {
                MappingProjectId = mappingProjectId,
                Title = model.Title,
                Template = model.Template
            };

            _templateRepository.Add(template);
            _templateRepository.SaveChanges();

            model.MappingProjectTemplateId = template.MappingProjectTemplateId;

            return model;
        }

        public MappingProjectTemplateModel Put(Guid mappingProjectId, Guid templateId, MappingProjectTemplateModel model)
        {
            var template = GetTemplate(templateId, mappingProjectId, MappingProjectUser.MappingProjectUserRole.Edit);

            template.Template = model.Template;
            template.Title = model.Title;

            _templateRepository.SaveChanges();

            return model;
        }

        public void Delete(Guid mappingProjectId, Guid templateId)
        {
            var template = GetTemplate(templateId, mappingProjectId, MappingProjectUser.MappingProjectUserRole.Edit);

            _templateRepository.Delete(template);
            _templateRepository.SaveChanges();
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

        private MappingProjectTemplate GetTemplate(Guid templateId, Guid mappingProjectId, MappingProjectUser.MappingProjectUserRole role = MappingProjectUser.MappingProjectUserRole.Guest)
        {
            var template = _templateRepository.Get(templateId);
            if (null == template)
                throw new NotFoundException(string.Format("Mapping Project Template with id '{0}' does not exist.", templateId));
            if (template.MappingProjectId != mappingProjectId)
                throw new NotFoundException(string.Format("Mapping Project with id '{0}' does not have Mapping Project Template of '{1}'.", mappingProjectId, templateId));
            if (!Principal.Current.IsAdministrator && !template.MappingProject.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));

            return template;
        }

    }
}