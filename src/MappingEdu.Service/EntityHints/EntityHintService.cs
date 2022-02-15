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
using MappingEdu.Service.Util;

namespace MappingEdu.Service.EntityHints
{
    public interface IEntityHintService
    {
        void Delete(Guid mappingProjectId, Guid entityHintId);

        EntityHintViewModel Get(Guid entityHintId, Guid mappingProjectId);

        EntityHintViewModel[] Get(Guid mappingProjectId);

        EntityHintViewModel GetEntityFilter(Guid mappingProjectId, Guid systemItemId);

        EntityHintViewModel Post(Guid mappingProjectId, EntityHintEditModel model);

        EntityHintViewModel Put(Guid mappingProjectId, Guid entityHintId, EntityHintEditModel model);
    }

    public class EntityHintService : IEntityHintService {

        private readonly IRepository<EntityHint> _entityHintRepository;
        private readonly IMappingProjectRepository _mappingProjectRepository;
        private readonly ISystemItemRepository _systemItemRepository;

        public EntityHintService(IRepository<EntityHint> entityHintRepository, IMappingProjectRepository mappingProjectRepository, ISystemItemRepository systemItemRepository)
        {
            _entityHintRepository = entityHintRepository;
            _mappingProjectRepository = mappingProjectRepository;
            _systemItemRepository = systemItemRepository;
        }

        public EntityHintViewModel Get(Guid entityHintId, Guid mappingProjectId)
        {
            var entityHint = GetEntityHint(entityHintId, mappingProjectId);
            return new EntityHintViewModel
            {
                EntityHintId = entityHint.EntityHintId,
                SourceEntityId = entityHint.SourceEntityId,
                SourceEntity = new EntityHintEntityModel()
                {
                    SystemItemId = entityHint.SourceEntityId,
                    ItemTypeId = entityHint.SourceEntity.ItemTypeId,
                    DomainItemPath = Utility.GetDomainItemPath(entityHint.SourceEntity)
                },
                TargetEntityId = entityHint.TargetEntityId,
                TargetEntity = new EntityHintEntityModel()
                {
                    SystemItemId = entityHint.TargetEntityId,
                    ItemTypeId = entityHint.TargetEntity.ItemTypeId,
                    DomainItemPath = Utility.GetDomainItemPath(entityHint.TargetEntity)
                },
            };
        }

        public EntityHintViewModel[] Get(Guid mappingProjectId)
        {
            GetMappingProject(mappingProjectId);

            var hints = _entityHintRepository.GetAllQueryable().Where(x => x.MappingProjectId == mappingProjectId);

            var entityHints = hints.ToList().Select(x => 
                new EntityHintViewModel
                {
                    EntityHintId = x.EntityHintId,
                    MappingProjectId = x.MappingProjectId,
                    SourceEntityId = x.SourceEntityId,
                    SourceEntity = new EntityHintEntityModel()
                    {
                        SystemItemId = x.SourceEntityId,
                        ItemTypeId = x.SourceEntity.ItemTypeId,
                        DomainItemPath = Utility.GetDomainItemPath(x.SourceEntity)
                    },
                    TargetEntityId = x.TargetEntityId,
                    TargetEntity = new EntityHintEntityModel()
                    {
                        SystemItemId = x.TargetEntityId,
                        ItemTypeId = x.TargetEntity.ItemTypeId,
                        DomainItemPath = Utility.GetDomainItemPath(x.TargetEntity)
                    },
                }).ToArray();

            return entityHints;
        }


        public EntityHintViewModel GetEntityFilter(Guid mappingProjectId, Guid systemItemId)
        {
            GetMappingProject(mappingProjectId);
            var systemItem = _systemItemRepository.Get(systemItemId);

            var segments = Utility.GetAllItemSegments(systemItem, false).ToArray();

            if (!segments.Any()) return null;

            var entityHint = _entityHintRepository.Get(segments[segments.Length - 1].SystemItemId);

            var i = segments.Length - 2;
            while (entityHint == null && i >= 0)
            {
                var id = segments[i].SystemItemId;
                entityHint = _entityHintRepository.GetAllQueryable().FirstOrDefault(x => x.SourceEntityId == id && x.MappingProjectId == mappingProjectId);
                i--;
            }

            if (entityHint == null) return null;

            return new EntityHintViewModel
            {
                EntityHintId = entityHint.EntityHintId,
                MappingProjectId = entityHint.MappingProjectId,
                SourceEntityId = entityHint.SourceEntityId,
                SourceEntity = new EntityHintEntityModel()
                {
                    DomainItemPath = Utility.GetDomainItemPath(entityHint.SourceEntity),
                    ItemTypeId = entityHint.SourceEntity.ItemTypeId,
                    SystemItemId = entityHint.SourceEntityId
                },
                TargetEntityId = entityHint.TargetEntityId,
                TargetEntity = new EntityHintEntityModel()
                {
                    DomainItemPath = Utility.GetDomainItemPath(entityHint.TargetEntity),
                    ItemTypeId = entityHint.TargetEntity.ItemTypeId,
                    SystemItemId = entityHint.TargetEntityId
                }
            };
        }

        public EntityHintViewModel Post(Guid mappingProjectId, EntityHintEditModel model)
        {
            GetMappingProject(mappingProjectId, MappingProjectUser.MappingProjectUserRole.Edit);

            var entityHint = new EntityHint
            {
                MappingProjectId = mappingProjectId,
                SourceEntityId = model.SourceEntityId,
                SourceEntity = _systemItemRepository.Get(model.SourceEntityId),
                TargetEntityId = model.TargetEntityId,
                TargetEntity = _systemItemRepository.Get(model.TargetEntityId)
            };

            _entityHintRepository.Add(entityHint);
            _entityHintRepository.SaveChanges();

            return Get(entityHint.EntityHintId, mappingProjectId);
        }

        public EntityHintViewModel Put(Guid mappingProjectId, Guid entityHintId, EntityHintEditModel model)
        {
            var entityHint = GetEntityHint(entityHintId, mappingProjectId, MappingProjectUser.MappingProjectUserRole.Edit);

            entityHint.SourceEntityId = model.SourceEntityId;
            entityHint.SourceEntity = _systemItemRepository.Get(model.SourceEntityId);
            entityHint.TargetEntityId = model.TargetEntityId;
            entityHint.TargetEntity = _systemItemRepository.Get(model.TargetEntityId);

            _entityHintRepository.SaveChanges();

            return Get(entityHint.EntityHintId, mappingProjectId);
        }

        public void Delete(Guid mappingProjectId, Guid entityHintId)
        {
            var entityHint = GetEntityHint(entityHintId, mappingProjectId, MappingProjectUser.MappingProjectUserRole.Edit);

            _entityHintRepository.Delete(entityHint);
            _entityHintRepository.SaveChanges();
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

        private EntityHint GetEntityHint(Guid entityHintId, Guid mappingProjectId, MappingProjectUser.MappingProjectUserRole role = MappingProjectUser.MappingProjectUserRole.Guest)
        {
            var entityHint = _entityHintRepository.Get(entityHintId);
            if (null == entityHint)
                throw new NotFoundException(string.Format("Entity Hint with id '{0}' does not exist.", entityHintId));
            if (entityHint.MappingProjectId != mappingProjectId)
                throw new NotFoundException(string.Format("Mapping Project with id '{0}' does not have Entity Hint Id of '{1}'.", mappingProjectId, entityHintId));
            if (!Principal.Current.IsAdministrator && !entityHint.MappingProject.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));

            return entityHint;
        }

    }
}