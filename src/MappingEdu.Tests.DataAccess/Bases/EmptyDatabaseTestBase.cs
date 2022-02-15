// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using Autofac;
using AutoMapper;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.DataAccess.Services.Validation;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Mappings;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.Services;
using MappingEdu.Core.Domain.System;
using MappingEdu.Service;
using MappingEdu.Service.Mappings;
using MappingEdu.Tests.Business.Builders;
using MappingEdu.Tests.DataAccess.Utilities;
using NCrunch.Framework;
using NUnit.Framework;
using Should;
using ItemChangeType = MappingEdu.Core.Domain.Enumerations.ItemChangeType;
using MappingMethodType = MappingEdu.Core.Domain.Enumerations.MappingMethodType;
using ProjectStatusType = MappingEdu.Core.Domain.Enumerations.ProjectStatusType;
using WorkflowStatusType = MappingEdu.Core.Domain.Enumerations.WorkflowStatusType;

namespace MappingEdu.Tests.DataAccess.Bases
{
    [ExclusivelyUses("MappingEdu")]
    public class EmptyDatabaseTestBase
    {
        private readonly string _connectionString;
        protected IContainer _container;
        private EntityContext _databaseContext;
        public IIdentityFactory identityFactory;

        public EmptyDatabaseTestBase()
        {
            identityFactory = new IdentityFactory();
            _connectionString = "MappingEdu";
        }

        protected EntityContext DatabaseContext { get; set; }

        protected EntityContext TestDatabaseContext
        {
            get { return _databaseContext ?? (_databaseContext = CreateDbContext()); }
        }

        protected CustomDetailMetadata CreateCustomDetailMetadata(
            EntityContext dbContext, MappedSystem mappedSystem, string displayName, bool isBoolean, bool isCoreDetail)
        {
            CustomDetailMetadata detail = New.CustomDetailMetadata
                .WithMappedSystem(mappedSystem)
                .WithDisplayName(displayName)
                .WithIsBoolean(isBoolean)
                .WithIsCoreDetail(isCoreDetail);

            dbContext.CustomDetailMetadatas.Add(detail);
            SaveChanges(dbContext).ShouldBeGreaterThan(0);

            return detail;
        }

        protected CustomMigration CreateCustomMigration(EntityContext dbContext, string migrationName)
        {
            var customMigration = new CustomMigration {Name = migrationName};
            dbContext.CustomMigrations.Add(customMigration);
            dbContext.SaveChanges();
            customMigration.CustomMigrationId.ShouldNotEqual(0);
            return customMigration;
        }

        protected EntityContext CreateDbContext()
        {
            var validateEntityInstances = _container.Resolve<IEnumerable<IValidateEntity>>().ToArray();
            return new EntityContext(validateEntityInstances, _connectionString);
        }

        protected SystemItem CreateDomain(EntityContext dbContext, MappedSystem mappedSystem, string domainName, string domainDefinition = null, string domainUrl = null, bool isActive = true)
        {
            SystemItem domain = New.SystemItem.AsDomain
                .WithName(domainName)
                .WithDefinition(domainDefinition)
                .WithUrl(domainUrl)
                .IsActive(isActive)
                .WithMappedSystem(mappedSystem);
            dbContext.SystemItems.Add(domain);
            SaveChanges(dbContext).ShouldBeGreaterThan(0);
            domain.SystemItemId.ShouldNotEqual(Guid.Empty);

            return domain;
        }

        protected SystemItem CreateElement(EntityContext dbContext, SystemItem entity, string elementName, string elementDefinition = null)
        {
            SystemItem element = New.SystemItem.AsElement
                .WithName(elementName)
                .WithDefinition(elementDefinition)
                .WithParentSystemItem(entity)
                .WithMappedSystem(entity.MappedSystem)
                .IsActive();
            dbContext.SystemItems.Add(element);
            SaveChanges(dbContext).ShouldBeGreaterThan(0);
            element.SystemItemId.ShouldNotEqual(Guid.Empty);

            return element;
        }

        protected SystemItem CreateEntity(EntityContext dbContext, SystemItem domain, string entityName, string entityDefinition = null)
        {
            SystemItem entity = New.SystemItem.AsEntity
                .WithName(entityName)
                .WithDefinition(entityDefinition)
                .WithParentSystemItem(domain)
                .WithMappedSystem(domain.MappedSystem)
                .IsActive();
            dbContext.SystemItems.Add(entity);
            SaveChanges(dbContext).ShouldBeGreaterThan(0);
            entity.SystemItemId.ShouldNotEqual(Guid.Empty);

            return entity;
        }

        protected SystemItem CreateEnumeration(EntityContext dbContext, SystemItem domain, string enumerationName, string enumerationDefinition = null)
        {
            SystemItem enumeration = New.SystemItem.AsEnumeration
                .WithName(enumerationName)
                .WithDefinition(enumerationDefinition)
                .WithParentSystemItem(domain)
                .WithMappedSystem(domain.MappedSystem)
                .IsActive();
            dbContext.SystemItems.Add(enumeration);
            SaveChanges(dbContext).ShouldBeGreaterThan(0);

            return enumeration;
        }

        protected SystemEnumerationItem CreateEnumerationItem(
            EntityContext dbContext, SystemItem enumeration, string codeValue, string description, string shortDescription)
        {
            SystemEnumerationItem enumerationItem = New.SystemEnumerationItem
                .WithMinimalPersistenceProperties(enumeration, codeValue)
                .WithDescription(description)
                .WithShortDescription(shortDescription);
            dbContext.SystemEnumerationItems.Add(enumerationItem);
            SaveChanges(dbContext).ShouldBeGreaterThan(0);

            return enumerationItem;
        }

        protected MapNote CreateMapNote(EntityContext dbContext, SystemItemMap systemItemMap, string noteTitle,
            string noteText)
        {
            MapNote mapNote = New.MapNote.WithSystemItemMap(systemItemMap)
                .WithTitle(noteTitle)
                .WithNotes(noteText)
                .WithCreateById(Guid.Empty);
            dbContext.MapNotes.Add(mapNote);
            SaveChanges(dbContext).ShouldBeGreaterThan(0);

            return mapNote;
        }

        protected MappedSystem CreateMappedSystem(EntityContext dbContext, string systemName, string systemVersion, bool isActive = true)
        {
            MappedSystem mappedSystem = New.MappedSystem.WithSystemName(systemName).WithSystemVersion(systemVersion).IsActive(isActive);
            dbContext.MappedSystems.Add(mappedSystem);
            SaveChanges(dbContext).ShouldBeGreaterThan(0);
            mappedSystem.MappedSystemId.ShouldNotEqual(Guid.Empty);

            return mappedSystem;
        }

        protected MappingProject CreateMappingProject(EntityContext dbContext, string projectName,
            string description, MappedSystem sourceDataStandard, MappedSystem targetDataStandard, bool isActive = true)
        {
            MappingProject mappingProject = New.MappingProject
                .WithProjectName(projectName)
                .WithDescription(description)
                .WithTarget(targetDataStandard)
                .WithSource(sourceDataStandard)
                .WithProjectStatusType(ProjectStatusType.Active.Id)
                .IsActive(isActive);
            dbContext.MappingProjects.Add(mappingProject);
            SaveChanges(dbContext).ShouldBeGreaterThan(0);
            mappingProject.MappingProjectId.ShouldNotEqual(Guid.Empty);

            return mappingProject;
        }

        protected Note CreateNote(EntityContext dbContext, SystemItem systemItem, string noteTitle, string noteText)
        {
            Note note = New.Note.WithSystemItem(systemItem)
                .WithTitle(noteTitle)
                .WithNotes(noteText)
                .WithCreateById(Guid.Empty);
            dbContext.Notes.Add(note);
            SaveChanges(dbContext);
            note.NoteId.ShouldNotEqual(Guid.Empty);

            return note;
        }

        protected SystemEnumerationItemMap CreatEnumerationItemMap(EntityContext dbContext, SystemItemMap systemItemMap, SystemEnumerationItem sourceSystemEnumerationItem)
        {
            SystemEnumerationItemMap enumerationItemMap = New.EnumerationItemMap
                .WithSourceSystemEnumerationItem(sourceSystemEnumerationItem)
                .WithSystemItemMap(systemItemMap);
            dbContext.SystemEnumerationItemMaps.Add(enumerationItemMap);
            SaveChanges(dbContext).ShouldBeGreaterThan(0);

            return enumerationItemMap;
        }

        protected SystemItem CreateSubEntity(EntityContext dbContext, SystemItem entity, string entityName, string entityDefinition = null)
        {
            SystemItem subEntity = New.SystemItem.AsSubEntity
                .WithName(entityName)
                .WithDefinition(entityDefinition)
                .WithParentSystemItem(entity)
                .WithMappedSystem(entity.MappedSystem)
                .IsActive();
            dbContext.SystemItems.Add(subEntity);
            SaveChanges(dbContext).ShouldBeGreaterThan(0);
            entity.SystemItemId.ShouldNotEqual(Guid.Empty);

            return subEntity;
        }

        protected SystemItemCustomDetail CreateSystemItemCustomDetail(
            EntityContext dbContext, SystemItem systemItem, CustomDetailMetadata customDetailMetadata, string value)
        {
            var systemItemCustomDetail = New.SystemItemCustomDetail
                .WithSystemItem(systemItem)
                .WithCustomDetailMetadata(customDetailMetadata)
                .WithValue(value);

            dbContext.SystemItemCustomDetails.Add(systemItemCustomDetail);
            SaveChanges(dbContext).ShouldBeGreaterThan(0);

            return systemItemCustomDetail;
        }

        protected SystemItemMap CreateSystemItemMap(EntityContext dbContext, MappingProject mappingProject, SystemItem sourceSystemItem)
        {
            SystemItemMap systemItemMap = New.SystemItemMap
                .WithMappingProject(mappingProject)
                .WithSourceSystemItem(sourceSystemItem)
                .WithWorkflowStatus(WorkflowStatusType.Incomplete)
                .WithMappingMethod(MappingMethodType.EnterMappingBusinessLogic);
            dbContext.SystemItemMaps.Add(systemItemMap);
            SaveChanges(dbContext).ShouldBeGreaterThan(0);

            return systemItemMap;
        }

        protected ApplicationUser CreateUser(EntityContext dbContext, string id, string firstName, string lastName, string email)
        {
            var user = new ApplicationUser
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = email
            };
            
            dbContext.Users.AddOrUpdate(user);
            SaveChanges(dbContext);
            return user;
        }

        protected SystemItemVersionDelta CreateVersion(
            EntityContext dbContext, SystemItem oldSystemItem, SystemItem newSystemItem, ItemChangeType itemChangeType)
        {
            SystemItemVersionDelta version = New.SystemItemVersionDelta
                .WithOldSystemItem(oldSystemItem)
                .WithNewSystemItem(newSystemItem)
                .WithChangeType(itemChangeType.Id);
            dbContext.SystemItemVersionDeltas.Add(version);
            dbContext.SaveChanges();
            version.SystemItemVersionDeltaId.ShouldNotEqual(Guid.Empty);

            return version;
        }

        protected virtual void EstablishContext()
        {
            // this will create a new database context to be used within the
            // scope of each test...this is used to get past an error when
            // deleting an object within a test
            DatabaseContext = CreateDbContext();
        }

        protected T GetInstance<T>()
        {
            return _container.Resolve<T>();
        }

        protected T[] GetInstances<T>()
        {
            return _container.Resolve<IEnumerable<T>>().ToArray();
        }

        public List<DbValidationError> GetValidationErrors(DbEntityValidationException exception)
        {
            return exception.EntityValidationErrors.SelectMany(validationResult => validationResult.ValidationErrors).ToList();
        }

        public string GetValidationErrorString(DbEntityValidationException exception)
        {
            var messageBuilder = new StringBuilder();
            foreach (var validationError in exception.EntityValidationErrors.SelectMany(validationResult => validationResult.ValidationErrors))
            {
                messageBuilder.AppendLine(validationError.ErrorMessage);
            }

            return messageBuilder.ToString();
        }

        protected void InitSystemClock(DateTime now)
        {
            SystemClock.Now = () => now;
        }

        private int SaveChanges(EntityContext dbContext)
        {
            try
            {
                return dbContext.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityError in ex.EntityValidationErrors)
                {
                    Debug.WriteLine("Entity of type '{0}' in state '{1}' has the following validation errors:",
                        entityError.Entry.Entity.GetType().Name, entityError.Entry.State);
                    foreach (var error in entityError.ValidationErrors)
                    {
                        Debug.WriteLine("- Property: '{0}', Error: '{1}'", error.PropertyName, error.ErrorMessage);
                    }
                }
            }

            return 0;
        }

        [OneTimeSetUp]
        public void Setup()
        {
            Thread.CurrentPrincipal = new ClaimsPrincipal(identityFactory.CreateIdentity("UNIT_TESTS", "username", Guid.Empty.ToString(), true));

            var builder = new ContainerBuilder();
            builder.RegisterModule(new ServicesModule());
            _container = builder.Build();

            InitSystemClock(DateTime.Now);

            Mapper.AddProfile(new DomainMappingProfile());
            Mapper.AddProfile(new ServiceModelProfile());
            Mapper.AssertConfigurationIsValid();

            new TestDatabaseCleaner(CreateDbContext()).CleanEntityTables();

            // TODO: Create a global approach to security in unit tests (cpt)

            EstablishContext();

            if (DatabaseContext == null) return;

            var user = CreateUser(DatabaseContext, Guid.Empty.ToString(), "test", "user", "test@dataaccess.com");
            var principal = new ClaimsPrincipal(identityFactory.CreateIdentity("UNIT_TESTS", user.UserName, user.Id, true));
            Thread.CurrentPrincipal = principal;
        }
    }
}
