// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using log4net;
using MappingEdu.Core.DataAccess.Entities.Mappings;
using MappingEdu.Core.DataAccess.Entities.Tracker;
using MappingEdu.Core.DataAccess.Exceptions;
using MappingEdu.Core.DataAccess.Services.Validation;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Services.Configuration;
using MappingEdu.Service.Model.MappingProject;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MappingEdu.Core.DataAccess.Entities
{
    public class EntityContext : IdentityDbContext<ApplicationUser>
    {
        private readonly object _lockObject = new object();
        private readonly ILog _logger = LogManager.GetLogger(typeof (EntityContext));
        private readonly IValidateEntity[] _validateEntities;
        private bool _loaded;
        private TimeSpan _validationEntitiesLoadTime;

        static EntityContext()
        {
            Database.SetInitializer<EntityContext>(null);
        }

        public EntityContext() : this(null, MappingEduConfiguration.ProdConnectionStringNameKey)
        {
        }

        public EntityContext(IValidateEntity[] validateEntities) : this(validateEntities, MappingEduConfiguration.ProdConnectionStringNameKey)
        {
        }

        public EntityContext(IValidateEntity[] validateEntities, string connectionStringName) : base(connectionStringName)
        {
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = true;
            
            _validateEntities = validateEntities;

            if (connectionStringName.Contains(";"))
            {
                var message = string.Format("{0} is setup to only work with connection string names (not connection string contents)", typeof (EntityContext).Name);
                throw new Exception(message);
            }
            ConnectionStringName = connectionStringName;
        }

        public DbSet<AutoMappingReasonType> AutoMappingReasonTypes { get; set; }

        public DbSet<BuildVersion> BuildVersions { get; set; }

        public DbSet<CompleteStatusType> CompleteStatusTypes { get; set; }

        public string ConnectionStringName { get; set; }

        public DbSet<CustomDetailMetadata> CustomDetailMetadatas { get; set; }

        public DbSet<CustomMigration> CustomMigrations { get; set; }

        public DbSet<EntityHint> EntityHints { get; set; }

        public DbSet<EnumerationMappingStatusReasonType> EnumerationMappingStatusReasonTypes { get; set; }

        public DbSet<EnumerationMappingStatusType> EnumerationMappingStatusTypes { get; set; }

        public DbSet<ItemChangeType> ItemChangeTypes { get; set; }

        public DbSet<ItemDataType> ItemDataTypes { get; set; }

        public DbSet<ItemType> ItemTypes { get; set; }

        public DbSet<Log> Logs { get; set; }

        public DbSet<MapNote> MapNotes { get; set; }

        public DbSet<MappedSystem> MappedSystems { get; set; }

        public DbSet<MappedSystemExtension> MappedSystemExtensions { get; set; }

        public DbSet<MappedSystemUpdate> MappedSystemUpdates { get; set; }

        public DbSet<MappedSystemUser> MappedSystemUsers { get; set; }

        public DbSet<MappingMethodType> MappingMethodTypes { get; set; }

        public DbSet<MappingProject> MappingProjects { get; set; }

        public DbSet<MappingProjectQueueFilter> MappingProjectQueueFilters { get; set; }

        public DbSet<MappingProjectSynonym> MappingProjectSynonyms { get; set; }

        public DbSet<MappingProjectUpdate> MappingProjectUpdates { get; set; }

        public DbSet<MappingProjectUser> MappingProjectUsers { get; set; }

        public DbSet<MappingStatusReasonType> MappingStatusReasonTypes { get; set; }

        public DbSet<MappingStatusType> MappingStatusTypes { get; set; }

        public DbSet<MappingProjectTemplate> MappingProjectTemplates { get; set; }

        public DbSet<Note> Notes { get; set; }

        public DbSet<Organization> Organizations { get; set; }

        public DbSet<ProjectStatusType> ProjectStatusTypes { get; set; }

        public DbSet<SystemConstant> SystemConstants { get; set; }

        public DbSet<SystemConstantType> SystemConstantTypes { get; set; }

        public DbSet<SystemEnumerationItemMap> SystemEnumerationItemMaps { get; set; }

        public DbSet<SystemEnumerationItem> SystemEnumerationItems { get; set; }

        public DbSet<SystemItemCustomDetail> SystemItemCustomDetails { get; set; }

        public DbSet<SystemItemMap> SystemItemMaps { get; set; }

        public DbSet<SystemItem> SystemItems { get; set; }

        public DbSet<SystemItemVersionDelta> SystemItemVersionDeltas { get; set; }

        public DbSet<UserNotification> UserNotifications { get; set; }

        public DbSet<WorkflowStatusType> WorkflowStatusTypes { get; set; }

        public TimeSpan GetValidationEntityLoadTime()
        {
            if (_validationEntitiesLoadTime == null)
                throw new Exception("Validation Entities not loaded.");
            return _validationEntitiesLoadTime;
        }

        public void LoadValidationEntities()
        {
            lock (_lockObject)
            {
                if (_loaded)
                    return;

                var sw = new Stopwatch();
                var fullLoadTime = new Stopwatch();

                fullLoadTime.Start();

                Configuration.LazyLoadingEnabled = false;
                sw.Start();
                (from cst in CompleteStatusTypes select cst).Load();
                sw.Stop();
                _logger.InfoFormat("CompleteStatusTypes took {0} to load.", sw.Elapsed);

                sw.Restart();
                (from ict in AutoMappingReasonTypes select ict).Load();
                sw.Stop();
                _logger.InfoFormat("AutoMappingReasonTypes took {0} to load.", sw.Elapsed);

                sw.Restart();
                (from ict in EnumerationMappingStatusTypes select ict).Load();
                sw.Stop();
                _logger.InfoFormat("EnumerationMappingStatusTypes took {0} to load.", sw.Elapsed);

                sw.Restart();
                (from ict in EnumerationMappingStatusReasonTypes select ict).Load();
                sw.Stop();
                _logger.InfoFormat("EnumerationMappingStatusReasonTypes took {0} to load.", sw.Elapsed);

                sw.Restart();
                (from ict in ItemChangeTypes select ict).Load();
                sw.Stop();
                _logger.InfoFormat("ItemChangeTypes took {0} to load.", sw.Elapsed);

                sw.Restart();
                (from idt in ItemDataTypes select idt).Load();
                sw.Stop();
                _logger.InfoFormat("ItemDataTypes took {0} to load.", sw.Elapsed);

                sw.Restart();
                (from it in ItemTypes select it).Load();
                sw.Stop();
                _logger.InfoFormat("ItemTypes took {0} to load.", sw.Elapsed);

                sw.Restart();
                (from ms in MappedSystems select ms).Load();
                sw.Stop();
                _logger.InfoFormat("MappedSystems took {0} to load.", sw.Elapsed);

                sw.Restart();
                (from mst in MappingStatusTypes select mst).Load();
                sw.Stop();
                _logger.InfoFormat("MappingStatusTypes took {0} to load.", sw.Elapsed);

                sw.Restart();
                (from n in Notes select n).Load();
                sw.Stop();
                _logger.InfoFormat("Notes took {0} to load.", sw.Elapsed);

                sw.Restart();
                (from srt in MappingStatusReasonTypes select srt).Load();
                sw.Stop();
                _logger.InfoFormat("MappingStatusReasonTypes took {0} to load.", sw.Elapsed);

                sw.Restart();
                (from sei in SystemEnumerationItems select sei).Load();
                sw.Stop();
                _logger.InfoFormat("SystemEnumerationItems took {0} to load.", sw.Elapsed);

                sw.Restart();
                (from seim in SystemEnumerationItemMaps select seim).Load();
                sw.Stop();
                _logger.InfoFormat("SystemEnumerationItemMaps took {0} to load.", sw.Elapsed);

                sw.Restart();
                (from si in SystemItems select si).Load();
                sw.Stop();
                _logger.InfoFormat("SystemItems took {0} to load.", sw.Elapsed);

                sw.Restart();
                (from sim in SystemItemMaps select sim).Load();
                sw.Stop();
                _logger.InfoFormat("SystemItemMaps took {0} to load.", sw.Elapsed);

                sw.Restart();
                (from mp in MappingProjects select mp).Load();
                sw.Stop();
                _logger.InfoFormat("MappingProjects took {0} to load.", sw.Elapsed);

                sw.Restart();
                (from mp in EntityHints select mp).Load();
                sw.Stop();
                _logger.InfoFormat("EntityHints took {0} to load.", sw.Elapsed);

                sw.Restart();
                (from mp in MappingProjectSynonyms select mp).Load();
                sw.Stop();
                _logger.InfoFormat("MappingProjectSynonyms took {0} to load.", sw.Elapsed);

                sw.Restart();
                (from mp in MappingProjectTemplates select mp).Load();
                sw.Stop();
                _logger.InfoFormat("MappingProjectTemplates took {0} to load.", sw.Elapsed);

                sw.Restart();
                (from pst in ProjectStatusTypes select pst).Load();
                sw.Stop();
                _logger.InfoFormat("ProjectStatusTypes took {0} to load.", sw.Elapsed);

                sw.Restart();
                (from un in UserNotifications select un).Load();
                sw.Stop();
                _logger.InfoFormat("UserNotifications took {0} to load.", sw.Elapsed);

                sw.Restart();
                (from un in MappingProjectQueueFilters select un).Load();
                sw.Stop();
                _logger.InfoFormat("MappingProjectQueueFilters took {0} to load.", sw.Elapsed);

                fullLoadTime.Stop();

                sw.Restart();
                (from un in MappedSystemExtensions select un).Load();
                sw.Stop();
                _logger.InfoFormat("MappedSystemExtensions took {0} to load.", sw.Elapsed);

                fullLoadTime.Stop();

                _validationEntitiesLoadTime = fullLoadTime.Elapsed;
                _loaded = true;
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Configurations.Add(new AutoMappingReasonTypeMapping());
            modelBuilder.Configurations.Add(new BuildVersionMapping());
            modelBuilder.Configurations.Add(new CompleteStatusTypeMapping());
            modelBuilder.Configurations.Add(new CustomDetailMetadataMapping());
            modelBuilder.Configurations.Add(new CustomMigrationMapping());
            modelBuilder.Configurations.Add(new EntityHintMapping());
            modelBuilder.Configurations.Add(new EnumerationMappingStatusReasonTypeMapping());
            modelBuilder.Configurations.Add(new ItemChangeTypeMapping());
            modelBuilder.Configurations.Add(new ItemDataTypeMapping());
            modelBuilder.Configurations.Add(new ItemTypeMapping());
            modelBuilder.Configurations.Add(new LogMapping());
            modelBuilder.Configurations.Add(new MapNoteMapping());
            modelBuilder.Configurations.Add(new MappedSystemExtensionMapping());
            modelBuilder.Configurations.Add(new MappedSystemMapping());
            modelBuilder.Configurations.Add(new MappedSystemUserMapping());
            modelBuilder.Configurations.Add(new MappedSystemUpdateMapping());
            modelBuilder.Configurations.Add(new MappingMethodTypeMapping());
            //modelBuilder.Configurations.Add(new MappingProjectAttachmentMapping());
            modelBuilder.Configurations.Add(new MappingProjectMapping());
            modelBuilder.Configurations.Add(new MappingProjectQueueFilterMapping());
            modelBuilder.Configurations.Add(new MappingProjectSynonymMapping());
            modelBuilder.Configurations.Add(new MappingProjectTemplateMapping());
            modelBuilder.Configurations.Add(new MappingProjectUserMapping());
            modelBuilder.Configurations.Add(new MappingProjectUpdateMapping());
            modelBuilder.Configurations.Add(new MappingStatusReasonTypeMapping());
            modelBuilder.Configurations.Add(new MappingStatusTypeMapping());
            modelBuilder.Configurations.Add(new NoteMapping());
            modelBuilder.Configurations.Add(new OrganizationMapping());
            modelBuilder.Configurations.Add(new ProjectStatusTypeMapping());
            modelBuilder.Configurations.Add(new SystemContantMapping());
            modelBuilder.Configurations.Add(new SystemConstantTypeMapping());
            modelBuilder.Configurations.Add(new SystemEnumerationItemMapping());
            modelBuilder.Configurations.Add(new SystemEnumerationItemMapMapping());
            modelBuilder.Configurations.Add(new SystemItemCustomDetailMapping());
            modelBuilder.Configurations.Add(new SystemItemMapping());
            modelBuilder.Configurations.Add(new SystemItemMapMapping());
            modelBuilder.Configurations.Add(new SystemItemVersionDeltaMapping());
            modelBuilder.Configurations.Add(new UserNotificationMapping());
            modelBuilder.Configurations.Add(new WorkflowStatusTypeMapping());

            modelBuilder.Entity<MappedSystem>()
                .HasMany<ApplicationUser>(s => s.FlaggedBy)
                .WithMany(c => c.FlaggedMappedSystems)
                .Map(cs =>
                {
                    cs.MapLeftKey("MappedSystemId");
                    cs.MapRightKey("ApplicationUserId");
                    cs.ToTable("FlaggedMappedSystem");
                });

            modelBuilder.Entity<MappingProject>()
                .HasMany<ApplicationUser>(s => s.FlaggedBy)
                .WithMany(c => c.FlaggedProjects)
                .Map(cs =>
                {
                    cs.MapLeftKey("MappingProjectId");
                    cs.MapRightKey("ApplicationUserId");
                    cs.ToTable("FlaggedMappingProjectId");
                });
        }

        public override int SaveChanges()
        {

            var entityChangeTracker = new EntityChangeTracker();
            var mappingProjectChangeTracker = new MappingProjectChangeTracker();
            var mappedSystemChangeTracker = new MappedSystemChangeTracker();
            
            entityChangeTracker.Process(this, ChangeTracker);
            if (Principal.Current != null && Principal.Current.UserId != "0" && Principal.Current.UserId != Guid.Empty.ToString()) //For Migrations
            {
                mappingProjectChangeTracker.Process(this, ChangeTracker);
                mappedSystemChangeTracker.Process(this, ChangeTracker);
            }

            try
            {
                if (Principal.Current == null || Principal.Current.UserId == Guid.Empty.ToString()) return base.SaveChanges();

                var systemItems = ChangeTracker.Entries<SystemItem>().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified).Select(x => x.Entity).ToList();
                var changes = base.SaveChanges();

                if (systemItems.Any())
                    UpdateSystemItems(systemItems);

                return changes;

            }
            catch (DbEntityValidationException e)
            {
                throw new MappingEduEntityValidationException(e);
            }
            catch (DbUpdateException e)
            {
                throw new MappingEduUpdateException(e);
            }
        }

        public override Task<int> SaveChangesAsync()
        {

            var entityChangeTracker = new EntityChangeTracker();
            var mappingProjectChangeTracker = new MappingProjectChangeTracker();
            var mappedSystemChangeTracker = new MappedSystemChangeTracker();

            entityChangeTracker.Process(this, ChangeTracker);
            if (Principal.Current != null && Principal.Current.UserId != "0" && Principal.Current.UserId != Guid.Empty.ToString()) //For Migrations            {
            { 
                mappingProjectChangeTracker.Process(this, ChangeTracker);
                mappedSystemChangeTracker.Process(this, ChangeTracker);
            }

            try
            {
                if (Principal.Current == null || Principal.Current.UserId == Guid.Empty.ToString()) return base.SaveChangesAsync();

                var systemItems = ChangeTracker.Entries<SystemItem>().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified).Select(x => x.Entity).ToList();
                var changes = base.SaveChangesAsync();

                if (systemItems.Any())
                    UpdateSystemItems(systemItems);

                return changes;

            }
            catch (DbEntityValidationException e)
            {
                throw new MappingEduEntityValidationException(e);
            }
            catch (DbUpdateException e)
            {
                throw new MappingEduUpdateException(e);
            }
        }

        private DataTable CreateDataTable<T>(T[] ids)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(T));

            if (ids != null)
            {
                foreach (var id in ids)
                {
                    var row = dataTable.NewRow();
                    row.SetField("Id", id);
                    dataTable.Rows.Add(row);
                }
            }

            return dataTable;
        }

        private void UpdateSystemItems(List<SystemItem> systemItems)
        {
            if (systemItems.Count() > 1)
            {
                var mappedSystemIds = systemItems.Select(x => x.MappedSystemId).Distinct().ToArray();
                Database.Connection.Open();
                var cmd = Database.Connection.CreateCommand();
                cmd.CommandText = "[UpdateAllPaths] @MappedSystemIds";
                cmd.Parameters.Add(new SqlParameter { ParameterName = "MappedSystemIds", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(mappedSystemIds), TypeName = "dbo.UniqueIdentiferId" });
                cmd.ExecuteNonQuery();
                Database.Connection.Close();
            }
            else
            {
                Database.Connection.Open();
                var cmd = Database.Connection.CreateCommand();
                cmd.CommandText = "[UpdatePaths] @SystemItemId";
                cmd.Parameters.Add(new SqlParameter { ParameterName = "SystemItemId", Value = systemItems.First().SystemItemId });
                cmd.ExecuteNonQuery();
                Database.Connection.Close();
            }
        }

        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            var dbValidationErrors = new List<DbValidationError>();
            var iEntity = entityEntry.Entity as IEntity;

            if (iEntity != null && _validateEntities != null)
            {
                var validateEntity = _validateEntities.SingleOrDefault(x => x.CanValidate(iEntity));
                if (validateEntity != null && entityEntry.State == EntityState.Added)
                    dbValidationErrors.AddRange(validateEntity.ValidateForAdd(iEntity));
                else if (validateEntity != null && entityEntry.State == EntityState.Modified)
                    dbValidationErrors.AddRange(validateEntity.ValidateForUpdate(iEntity));
            }

            var baseResult = base.ValidateEntity(entityEntry, items);
            dbValidationErrors.ForEach(x => baseResult.ValidationErrors.Add(x));

            return baseResult;
        }
    }
}
