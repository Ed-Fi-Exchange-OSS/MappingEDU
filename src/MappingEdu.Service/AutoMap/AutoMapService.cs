// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.DataAccess.Repositories;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Core.Services.Mapping;
using MappingEdu.Service.Model.DataStandard;
using MappingEdu.Service.Model.ElementDetails;
using AutoMappingReasonType = MappingEdu.Core.Domain.Enumerations.AutoMappingReasonType;
using ItemChangeType = MappingEdu.Core.Domain.Enumerations.ItemChangeType;
using ItemType = MappingEdu.Core.Domain.Enumerations.ItemType;
using MappingMethodType = MappingEdu.Core.Domain.Enumerations.MappingMethodType;
using WorkflowStatusType = MappingEdu.Core.Domain.Enumerations.WorkflowStatusType;

namespace MappingEdu.Service.AutoMap
{
    public interface IAutoMapService
    {
        void CreateAutoMappings(Guid sourceDataStandardId, Guid targetDataStandardId, Guid mappingProjectId);

        void DeltaCopy(Guid standardId);

        ICollection<Model.AutoMap.AutoMap> GetAutoMapResults(Guid sourceDataStandardId, Guid targetDataStandardId, Guid mappingProjectId = new Guid());
    }

    public class AutoMapService : IAutoMapService
    {
        private readonly EntityContext _context;
        private readonly IBusinessLogicParser _businessLogicParser;
        private readonly IMapper _mapper;
        private readonly IMappingProjectRepository _mappingProjectRepository;
        private readonly IMappedSystemRepository _mappedSystemRepository;
        private readonly ISystemItemMapRepository _systemItemMapRepository;
        private readonly ISystemItemRepository _systemItemRepository;

        public AutoMapService(IMappingProjectRepository mappingProjectRepository, IMapper mapper, ISystemItemRepository systemItemRepository,
            ISystemItemMapRepository systemItemMapRepository, IMappedSystemRepository mappedSystemRepository, EntityContext context, IBusinessLogicParser businessLogicParser)
        {
            _businessLogicParser = businessLogicParser;
            _mappingProjectRepository = mappingProjectRepository;
            _mapper = mapper;
            _systemItemRepository = systemItemRepository;
            _systemItemMapRepository = systemItemMapRepository;
            _mappedSystemRepository = mappedSystemRepository;
            _context = context;
        }

        public void CreateAutoMappings(Guid sourceDataStandardId, Guid targetDataStandardId, Guid mappingProjectId)
        {
            var autoMapResults = GetAutoMapResults(sourceDataStandardId, targetDataStandardId, mappingProjectId);
            var mappingProject = _mappingProjectRepository.Get(mappingProjectId);
            var targetStandard = _mappedSystemRepository.Get(targetDataStandardId);
            var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

            try
            {
                // Adds 1000's of Mappings used to speed up insert
                _context.Configuration.AutoDetectChangesEnabled = false;

                foreach(var mapping in autoMapResults)
                {
                    var systemItemMap = new SystemItemMap
                    {
                        MappingProjectId = mappingProject.MappingProjectId,
                        MappingProject = mappingProject,
                        SourceSystemItemId = mapping.SourceSystemItem.SystemItemId,
                        BusinessLogic = mapping.BusinessLogic,
                        WorkflowStatusTypeId = WorkflowStatusType.Complete.Id,
                        MappingMethodTypeId = mapping.MappingMethod.Id,
                        IsAutoMapped = true
                    };
                    
                    try
                    {
                        if (mapping.MappingMethod == MappingMethodType.EnterMappingBusinessLogic)
                            systemItemMap.TargetSystemItems = _businessLogicParser.ParseReferencedSystemItems(mapping.BusinessLogic,
                                mapping.SourceSystemItem.ItemTypeId == ItemType.Enumeration.Id, targetStandard);
                    }
                    catch (Exception) { continue; }
                    _systemItemMapRepository.Add(systemItemMap);
                }

                _systemItemMapRepository.SaveChanges();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
                transaction.Dispose();
            }
        }

        public void DeltaCopy(Guid standardId)
        {
            var standard = _mappedSystemRepository.Get(standardId);
            if (!standard.PreviousMappedSystemId.HasValue || !standard.ClonedFromMappedSystemId.HasValue 
                || !standard.ClonedFromMappedSystem.PreviousMappedSystemId.HasValue) return;

            var previousStandard = _mappedSystemRepository.Get(standard.PreviousMappedSystemId.Value);
            if (!previousStandard.ClonedFromMappedSystemId.HasValue
                || previousStandard.ClonedFromMappedSystemId.Value != standard.ClonedFromMappedSystem.PreviousMappedSystemId.Value) return;

            var clonedStandard = standard.ClonedFromMappedSystem;
            var standardItems = _systemItemRepository.GetAllForComparison(standardId);
            var previousItems = _systemItemRepository.GetAllForComparison(previousStandard.MappedSystemId);
            var clonedItems = _systemItemRepository.GetAllForComparison(standard.ClonedFromMappedSystemId.Value);
            var previousClonedItems = _systemItemRepository.GetAllForComparison(previousStandard.ClonedFromMappedSystemId.Value);

            var commonPathsBetweenStandardandClone = GetSamePathResults(standardItems, clonedItems);
            var commonPathsBetweenPreviousStandardandClone = GetSamePathResults(previousItems, previousClonedItems);
            var transaction = _context.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

            try
            {
                _context.Configuration.AutoDetectChangesEnabled = false;

                foreach (var item in clonedStandard.SystemItems)
                {
                    foreach (var delta in item.PreviousSystemItemVersionDeltas)
                    {
                        var newVersion = commonPathsBetweenStandardandClone.FirstOrDefault(x => delta.NewSystemItemId.HasValue && x.TargetSystemItems.FirstOrDefault() != null && x.TargetSystemItems.First().SystemItemId == delta.NewSystemItemId.Value);
                        var oldVersion = commonPathsBetweenPreviousStandardandClone.FirstOrDefault(x => delta.OldSystemItemId.HasValue && x.TargetSystemItems.FirstOrDefault() != null && x.TargetSystemItems.First().SystemItemId == delta.OldSystemItemId.Value);

                        if (newVersion != null && oldVersion != null)
                        {
                            _context.SystemItemVersionDeltas.Add(new SystemItemVersionDelta()
                            {
                                Description = delta.Description,
                                OldSystemItemId = oldVersion.SourceSystemItem.SystemItemId,
                                NewSystemItemId = newVersion.SourceSystemItem.SystemItemId,
                                ItemChangeTypeId = delta.ItemChangeTypeId,
                            });
                        }
                        else if (newVersion != null && delta.ItemChangeTypeId != ItemChangeType.ChangedElement.Id && delta.ItemChangeTypeId != ItemChangeType.ChangedEntity.Id)
                        {
                            _context.SystemItemVersionDeltas.Add(new SystemItemVersionDelta()
                            {
                                Description = delta.Description,
                                NewSystemItemId = newVersion.SourceSystemItem.SystemItemId,
                                ItemChangeTypeId = delta.ItemChangeTypeId,
                            });
                        }
                    }
                }
                _context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
            }
            finally
            {
                _context.Configuration.AutoDetectChangesEnabled = true;
                transaction.Dispose();
            }
        }

        public ICollection<Model.AutoMap.AutoMap> GetAutoMapResults(Guid sourceDataStandardId, Guid targetDataStandardId, Guid mappingProjectId = new Guid())
        {
            var sourceStandard = _mappedSystemRepository.Get(sourceDataStandardId);
            var targetStandard = _mappedSystemRepository.Get(targetDataStandardId);

            var sourceSystemItems = _systemItemRepository.GetAllForComparison(sourceDataStandardId).ToList();
            var targetSystemItems = _systemItemRepository.GetAllForComparison(targetDataStandardId).ToList();

            var resultsSamePath = GetSamePathResults(sourceSystemItems, targetSystemItems);
            var resultsSimilarProject = GetSimilarProjectResults(sourceDataStandardId, targetDataStandardId, sourceSystemItems, targetSystemItems, mappingProjectId);

            var sourcePreviousResults = new List<Model.AutoMap.AutoMap>();
            var commonPathsBetweenSourceVersions = new List<Model.AutoMap.AutoMap>();

            if (sourceStandard.PreviousMappedSystemId.HasValue)
            {
                var previousSourceSystemItems = _systemItemRepository.GetAllForComparison(sourceStandard.PreviousMappedSystemId.Value).ToList();

                var previousSourceVersionDelta = sourceStandard.SystemItems.Where(x => x.PreviousSystemItemVersionDeltas.Count(y => y.OldSystemItemId != null) == 1).Select(x => x.PreviousSystemItemVersionDeltas);
                commonPathsBetweenSourceVersions.AddRange(previousSourceVersionDelta.Where(x => x.First().NewSystemItemId.HasValue && x.First().OldSystemItemId.HasValue).Select(
                    delta => new Model.AutoMap.AutoMap()
                    {
                        SourceSystemItem = previousSourceSystemItems.First(x => delta.Select(y => y.OldSystemItemId).Contains(x.SystemItemId)),
                        TargetSystemItems = sourceSystemItems.Where(x => x.SystemItemId == delta.First().NewSystemItemId).ToList(),
                        MappingMethod = MappingMethodType.EnterMappingBusinessLogic,
                        Reason = AutoMappingReasonType.PreviousVersionDelta
                }));

                commonPathsBetweenSourceVersions.AddRange(GetSamePathResults(previousSourceSystemItems, sourceSystemItems));
                sourcePreviousResults = GetSourcePreviousResults(sourceStandard.PreviousMappedSystemId.Value, targetDataStandardId, commonPathsBetweenSourceVersions, targetSystemItems).ToList();
            }

            var targetPreviousResults = new List<Model.AutoMap.AutoMap>();
            var commonPathsBetweenTargetVersions = new List<Model.AutoMap.AutoMap>();

            if (targetStandard.PreviousMappedSystemId.HasValue)
            {
                var previousTargetSystemItems = _systemItemRepository.GetAllForComparison(targetStandard.PreviousMappedSystemId.Value).ToList();

                var previousTargetVersionDelta = targetStandard.SystemItems.Where(x => x.PreviousSystemItemVersionDeltas.Count(y => y.OldSystemItemId != null) == 1).Select(x => x.PreviousSystemItemVersionDeltas);
                commonPathsBetweenTargetVersions.AddRange(previousTargetVersionDelta.Select(
                    delta => new Model.AutoMap.AutoMap()
                    {
                        SourceSystemItem = previousTargetSystemItems.First(x => x.SystemItemId == delta.First().OldSystemItemId),
                        TargetSystemItems = new List<ElementDetailsSearchModel>()
                        {
                          targetSystemItems.First(x => x.SystemItemId == delta.First().NewSystemItemId)
                        },
                        MappingMethod = MappingMethodType.EnterMappingBusinessLogic,
                        Reason = AutoMappingReasonType.PreviousVersionDelta
                    }));

                commonPathsBetweenTargetVersions.AddRange(GetSamePathResults(previousTargetSystemItems, targetSystemItems)); 
                targetPreviousResults = GetTargetPreviousResults(sourceDataStandardId, targetStandard.PreviousMappedSystemId.Value, commonPathsBetweenTargetVersions, sourceSystemItems).ToList();
            }

            var previousPreviousResults = new List<Model.AutoMap.AutoMap>();

            if (sourceStandard.PreviousMappedSystemId.HasValue && targetStandard.PreviousMappedSystemId.HasValue)
                previousPreviousResults = GetPreviousPreviousResults(sourceStandard.PreviousMappedSystemId.Value, targetStandard.PreviousMappedSystemId.Value, commonPathsBetweenSourceVersions, commonPathsBetweenTargetVersions).ToList();

            var transitiveResults = GetTransiviteResults(sourceDataStandardId, targetDataStandardId, sourceSystemItems, targetSystemItems);

            var totalResults = resultsSamePath
                .Concat(resultsSimilarProject)
                .Concat(sourcePreviousResults)
                .Concat(targetPreviousResults)
                .Concat(transitiveResults)
                .Concat(previousPreviousResults);

            return FilterResults(totalResults, mappingProjectId);
        }

        private ICollection<Model.AutoMap.AutoMap> FilterResults(IEnumerable<Model.AutoMap.AutoMap> results, Guid mappingProjectId)
        {
            var returnResults = new List<Model.AutoMap.AutoMap>();
            var groups = results.Where(x => x.SourceSystemItem.ItemTypeId == ItemType.Element.Id || x.SourceSystemItem.ItemTypeId == ItemType.Enumeration.Id).GroupBy(x => x.SourceSystemItem);
            foreach (var group in groups)
            {
                if (group.Count() == 1) returnResults.Add(group.First());
                else
                {
                    var bestReason = group.Min(x => x.Reason.Id);
                    if (group.Count(x => x.Reason.Id == bestReason) == 1) returnResults.Add(group.First(x => x.Reason.Id == bestReason));
                    else
                    {
                        var compareTo = new Model.AutoMap.AutoMap();
                        var add = true;
                        foreach (var map in group.Where(x => x.Reason.Id == bestReason))
                        {
                            if (compareTo.SourceSystemItem == null) compareTo = map;
                            else
                            {
                                if (compareTo.TargetSystemItems == null || map.TargetSystemItems == null)
                                {
                                    if (compareTo.TargetSystemItems == null && map.TargetSystemItems == null) continue;
                                    add = false;
                                    break;
                                }

                                if (compareTo.TargetSystemItems.Select(x => x.SystemItemId).SequenceEqual(map.TargetSystemItems.Select(y => y.SystemItemId))) continue;
                                add = false;
                                break;
                            }
                        }
                        if(add) returnResults.Add(group.First(x => x.Reason.Id == bestReason));
                    }
                }
            }

            if (mappingProjectId == Guid.Empty)
                return returnResults.ToList();

            var mappingProjectMappingSources = _systemItemMapRepository.GetAllQueryable().Where(x => x.MappingProjectId == mappingProjectId).Select(x => x.SourceSystemItemId);
            return returnResults.Where(x => !mappingProjectMappingSources.Contains(x.SourceSystemItem.SystemItemId)).ToList();
        }

        private ICollection<Model.AutoMap.AutoMap> GetSimilarProjectResults(Guid sourceDataStandardId, Guid targetDataStandardId, 
            IEnumerable<ElementDetailsSearchModel> sourceSystemItems, IEnumerable<ElementDetailsSearchModel> targetSystemItems, Guid mappingProjectId = new Guid())
        {
            var results = new List<Model.AutoMap.AutoMap>();

            var mappingProjects = _mappingProjectRepository
                .GetAllQueryable().Where(x => x.IsActive && (Principal.Current.IsAdministrator || x.IsPublic || x.Users.Any(y => y.UserId == Principal.Current.UserId)) && (x.MappingProjectId != mappingProjectId))
                .Where(x => (x.SourceDataStandardMappedSystemId == sourceDataStandardId && x.TargetDataStandardMappedSystemId == targetDataStandardId) ||
                            (x.TargetDataStandardMappedSystemId == targetDataStandardId && x.TargetDataStandardMappedSystemId == sourceDataStandardId));

            foreach (var mappingProject in mappingProjects)
            {
                if (mappingProject.SourceDataStandardMappedSystemId == sourceDataStandardId)
                {
                    foreach (var mapping in mappingProject.SystemItemMaps)
                    {
                        var source = sourceSystemItems.FirstOrDefault(x => x.SystemItemId == mapping.SourceSystemItemId);
                        if (source == null) continue;

                        var map = new Model.AutoMap.AutoMap()
                        {
                            SourceSystemItem = source,
                            BusinessLogic = mapping.BusinessLogic,
                            OmissionReason = mapping.OmissionReason,
                            MappingMethod = mapping.MappingMethodType,
                            TargetSystemItems = new List<ElementDetailsSearchModel>(),
                            Reason = AutoMappingReasonType.SimilarMappingProject,
                            MappingProjectName = mappingProject.ProjectName
                        };

                        foreach (var target in mapping.TargetSystemItems)
                        {
                            var targetItem = targetSystemItems.FirstOrDefault(x => x.SystemItemId == target.SystemItemId);
                            if (targetItem == null) continue;

                            map.TargetSystemItems.Add(targetItem);
                        }

                        results.Add(map);
                    }
                }
                else
                {
                    foreach (var mapping in mappingProject.SystemItemMaps.Where(x => x.TargetSystemItems.Count == 1 && x.MappingMethodType == MappingMethodType.EnterMappingBusinessLogic))
                    {
                        var source = targetSystemItems.FirstOrDefault(x => x.SystemItemId == mapping.SourceSystemItemId);
                        if (source == null) continue;

                        var target = sourceSystemItems.FirstOrDefault(x => x.SystemItemId == mapping.TargetSystemItems.First().SystemItemId);
                        if (target == null) continue;

                        var map = new Model.AutoMap.AutoMap()
                        {
                            SourceSystemItem = source,
                            BusinessLogic = target.DomainItemPath,
                            MappingMethod = MappingMethodType.EnterMappingBusinessLogic,
                            TargetSystemItems = new List<ElementDetailsSearchModel> { target },
                            Reason = AutoMappingReasonType.SimilarMappingProject,
                            MappingProjectName = mappingProject.ProjectName
                        };

                        results.Add(map);
                    }
                }
            }

            return results;
        }

        private IEnumerable<Model.AutoMap.AutoMap> GetPreviousPreviousResults(Guid previousSourceDataStandardId, Guid previousTargetDataStandardId,
            IEnumerable<Model.AutoMap.AutoMap> commonPathsBetweenSourceVersions, IEnumerable<Model.AutoMap.AutoMap> commonPathsBetweenTargetVersions)
        {
            var previousSourceStandard = _mapper.Map<DataStandardViewModel>(_mappedSystemRepository.Get(previousSourceDataStandardId));
            var previousTargetStandard = _mapper.Map<DataStandardViewModel>(_mappedSystemRepository.Get(previousTargetDataStandardId));

            var results = new List<Model.AutoMap.AutoMap>();

            var mappingProjects = _mappingProjectRepository
                .GetAllQueryable().Where(x => x.IsActive && (Principal.Current.IsAdministrator || x.IsPublic || x.Users.Any(y => y.UserId == Principal.Current.UserId)))
                .Where(x => (x.SourceDataStandardMappedSystemId == previousSourceDataStandardId && x.TargetDataStandardMappedSystemId == previousTargetDataStandardId) ||
                    (x.TargetDataStandardMappedSystemId == previousSourceDataStandardId && x.SourceDataStandardMappedSystemId == previousTargetDataStandardId));

            foreach (var mappingProject in mappingProjects)
            {
                if (mappingProject.SourceDataStandardMappedSystemId == previousSourceDataStandardId)
                {
                    foreach (var mapping in _systemItemMapRepository.GetAllForComparison(mappingProject.MappingProjectId))
                    {
                        var autoMap = commonPathsBetweenSourceVersions.FirstOrDefault(x => x.SourceSystemItem.SystemItemId == mapping.SourceSystemItemId);
                        if (autoMap == null) continue;
                        if (mapping.MappingMethodTypeId == MappingMethodType.EnterMappingBusinessLogic.Id && mapping.TargetSystemItems.Any())
                        {
                            if (mapping.TargetSystemItems
                                .Where(x => x.MappedSystemId == previousTargetDataStandardId)
                                .Select(x => x.SystemItemId)
                                .Except(commonPathsBetweenTargetVersions.Select(y => y.SourceSystemItem.SystemItemId)).Any()) continue;

                            var map = new Model.AutoMap.AutoMap()
                            {
                                SourceSystemItem = autoMap.TargetSystemItems.First(),
                                BusinessLogic = mapping.BusinessLogic,
                                MappingMethod = MappingMethodType.EnterMappingBusinessLogic,
                                TargetSystemItems = new List<ElementDetailsSearchModel>(),
                                Reason = AutoMappingReasonType.PreviousPrevious,
                                PreviousTargetDataStandard = previousTargetStandard,
                                PreviousSourceDataStandard = previousSourceStandard,
                                MappingProjectName = mappingProject.ProjectName
                            };

                            foreach (var targetItem in mapping.TargetSystemItems.Where(x => x.MappedSystemId == previousTargetDataStandardId && (x.ItemTypeId == ItemType.Element.Id || x.ItemTypeId == ItemType.Enumeration.Id)))
                            {
                                var path = commonPathsBetweenTargetVersions.First(x => x.SourceSystemItem.SystemItemId == targetItem.SystemItemId);
                                if (path.Reason == AutoMappingReasonType.PreviousVersionDelta)
                                    map.BusinessLogic = map.BusinessLogic.Replace(path.SourceSystemItem.DomainItemPath, path.TargetSystemItems.First().DomainItemPath);
                                map.TargetSystemItems.Add(path.TargetSystemItems.First());
                            }

                            results.Add(map);
                        }
                        else if (mapping.MappingMethodTypeId == MappingMethodType.MarkForExtension.Id)
                        {
                            results.Add(new Model.AutoMap.AutoMap()
                            {
                                SourceSystemItem = autoMap.TargetSystemItems.First(),
                                MappingMethod = MappingMethodType.MarkForExtension,
                                Reason = AutoMappingReasonType.PreviousPrevious,
                                PreviousTargetDataStandard = previousTargetStandard,
                                PreviousSourceDataStandard = previousSourceStandard,
                                MappingProjectName = mappingProject.ProjectName
                            });
                        }
                        else if (mapping.MappingMethodTypeId == MappingMethodType.MarkForOmission.Id)
                        {
                            results.Add(new Model.AutoMap.AutoMap()
                            {
                                SourceSystemItem = autoMap.TargetSystemItems.First(),
                                OmissionReason = mapping.OmissionReason,
                                MappingMethod = MappingMethodType.MarkForOmission,
                                Reason = AutoMappingReasonType.PreviousPrevious,
                                PreviousTargetDataStandard = previousTargetStandard,
                                PreviousSourceDataStandard = previousSourceStandard,
                                MappingProjectName = mappingProject.ProjectName
                            });
                        }
                    }
                }
            }

            return results;
        }

        private IEnumerable<Model.AutoMap.AutoMap> GetSamePathResults(IEnumerable<ElementDetailsSearchModel> sourceSystemItems, IEnumerable<ElementDetailsSearchModel> targetSystemItems)
        {
            return (from s in sourceSystemItems
                join t in targetSystemItems
                    on s.DomainItemPath equals t.DomainItemPath
                where s.ItemTypeId == t.ItemTypeId
                select new Model.AutoMap.AutoMap
                {
                    BusinessLogic = String.Format("[{0}]", t.DomainItemPath),
                    SourceSystemItem = s,
                    TargetSystemItems = new List<ElementDetailsSearchModel> { t },
                    MappingMethod = MappingMethodType.EnterMappingBusinessLogic,
                    Reason = AutoMappingReasonType.SamePath
                }).ToList();
        }

        private IEnumerable<Model.AutoMap.AutoMap> GetSourcePreviousResults(Guid previousSourceDataStandardId, Guid targetDataStandardId, 
            IEnumerable<Model.AutoMap.AutoMap> commonPathsBetweenVersions, IEnumerable<ElementDetailsSearchModel> targetItems )
        {
            var previousSourceStandard = _mapper.Map<DataStandardViewModel>(_mappedSystemRepository.Get(previousSourceDataStandardId));

            var results = new List<Model.AutoMap.AutoMap>();

            var mappingProjects = _mappingProjectRepository
                .GetAllQueryable().Where(x => x.IsActive && (Principal.Current.IsAdministrator || x.IsPublic || x.Users.Any(y => y.UserId == Principal.Current.UserId)))
                .Where(x => (x.SourceDataStandardMappedSystemId == previousSourceDataStandardId && x.TargetDataStandardMappedSystemId == targetDataStandardId) ||
                    (x.TargetDataStandardMappedSystemId == previousSourceDataStandardId && x.SourceDataStandardMappedSystemId == targetDataStandardId));

            foreach (var mappingProject in mappingProjects)
            {
                if (mappingProject.SourceDataStandardMappedSystemId == previousSourceDataStandardId)
                {
                    foreach (var mapping in _systemItemMapRepository.GetAllForComparison(mappingProject.MappingProjectId))
                    {
                        if (commonPathsBetweenVersions.Count(x => x.SourceSystemItem.SystemItemId == mapping.SourceSystemItemId) != 1) continue;

                        var autoMap = commonPathsBetweenVersions.First(x => x.SourceSystemItem.SystemItemId == mapping.SourceSystemItemId);
                        if (mapping.MappingMethodTypeId == MappingMethodType.EnterMappingBusinessLogic.Id && mapping.TargetSystemItems.Any())
                        {
                            results.Add(new Model.AutoMap.AutoMap()
                            {
                                SourceSystemItem = autoMap.TargetSystemItems.First(),
                                BusinessLogic = (autoMap.Reason == AutoMappingReasonType.PreviousVersionDelta) ? mapping.BusinessLogic.Replace(autoMap.SourceSystemItem.DomainItemPath, autoMap.TargetSystemItems.First().DomainItemPath) : mapping.BusinessLogic,
                                MappingMethod = MappingMethodType.EnterMappingBusinessLogic,
                                TargetSystemItems = mapping.TargetSystemItems.Where(x => (x.ItemTypeId == ItemType.Element.Id || x.ItemTypeId == ItemType.Enumeration.Id) && x.MappedSystemId == targetDataStandardId).Select(x => targetItems.First(y => y.SystemItemId == x.SystemItemId)).ToList(),
                                Reason = AutoMappingReasonType.PreviousSourceVersion,
                                PreviousSourceDataStandard = previousSourceStandard,
                                MappingProjectName = mappingProject.ProjectName
                            });
                        }
                        else if (mapping.MappingMethodTypeId == MappingMethodType.MarkForExtension.Id)
                        {
                            results.Add(new Model.AutoMap.AutoMap()
                            {
                                SourceSystemItem = autoMap.TargetSystemItems.First(),
                                MappingMethod = MappingMethodType.MarkForExtension,
                                Reason = AutoMappingReasonType.PreviousSourceVersion,
                                PreviousSourceDataStandard = previousSourceStandard,
                                MappingProjectName = mappingProject.ProjectName
                            });
                        }
                        else if (mapping.MappingMethodTypeId == MappingMethodType.MarkForOmission.Id)
                        {
                            results.Add(new Model.AutoMap.AutoMap()
                            {
                                SourceSystemItem = autoMap.TargetSystemItems.First(),
                                OmissionReason = mapping.OmissionReason,
                                MappingMethod = MappingMethodType.MarkForOmission,
                                Reason = AutoMappingReasonType.PreviousSourceVersion,
                                PreviousSourceDataStandard = previousSourceStandard,
                                MappingProjectName = mappingProject.ProjectName
                            });
                        }
                    }
                }
                else
                {
                   results.AddRange(from mapping in _systemItemMapRepository.GetAllForComparison(mappingProject.MappingProjectId)
                                     .Where(x => x.TargetSystemItems.Count() == 1 
                                     && x.MappingMethodTypeId == MappingMethodType.EnterMappingBusinessLogic.Id
                                     && commonPathsBetweenVersions.Select(y => y.SourceSystemItem.SystemItemId).Contains(x.TargetSystemItems.First().SystemItemId))

                        let autoMap = commonPathsBetweenVersions.FirstOrDefault(x => x.SourceSystemItem.SystemItemId == mapping.TargetSystemItems.First().SystemItemId)
                        where autoMap != null
                        select new Model.AutoMap.AutoMap()
                        {
                            SourceSystemItem = autoMap.TargetSystemItems.First(),
                            BusinessLogic = string.Format("[{0}]", targetItems.First(x => x.SystemItemId == mapping.SourceSystemItemId).DomainItemPath),
                            MappingMethod = MappingMethodType.EnterMappingBusinessLogic,
                            Reason = AutoMappingReasonType.PreviousSourceVersion,
                            PreviousSourceDataStandard = previousSourceStandard,
                            MappingProjectName = mappingProject.ProjectName
                        });
                }
            }

            return results;
        }

        private IEnumerable<Model.AutoMap.AutoMap> GetTargetPreviousResults(Guid sourceDataStandardId, Guid previousTargetDataStandardId, 
            IEnumerable<Model.AutoMap.AutoMap> commonPathsBetweenVersions, IEnumerable<ElementDetailsSearchModel> sourceItems)
        {
            var previousTargetStandard = _mapper.Map<DataStandardViewModel>(_mappedSystemRepository.Get(previousTargetDataStandardId));

            var results = new List<Model.AutoMap.AutoMap>();

            var mappingProjects = _mappingProjectRepository
                .GetAllQueryable().Where(x => x.IsActive && (Principal.Current.IsAdministrator || x.IsPublic || x.Users.Any(y => y.UserId == Principal.Current.UserId)))
                .Where(x => (x.SourceDataStandardMappedSystemId == sourceDataStandardId && x.TargetDataStandardMappedSystemId == previousTargetDataStandardId) ||
                    (x.TargetDataStandardMappedSystemId == sourceDataStandardId && x.SourceDataStandardMappedSystemId == previousTargetDataStandardId));

            foreach (var mappingProject in mappingProjects)
            {
                if (mappingProject.TargetDataStandardMappedSystemId == previousTargetDataStandardId)
                {
                    foreach (var mapping in _systemItemMapRepository.GetAllForComparison(mappingProject.MappingProjectId))
                    {
                        var source = sourceItems.FirstOrDefault(x => x.SystemItemId == mapping.SourceSystemItemId);
                        if (source == null) continue;

                        if (mapping.MappingMethodTypeId == MappingMethodType.EnterMappingBusinessLogic.Id && mapping.TargetSystemItems != null && mapping.BusinessLogic != null)
                        {
                            if (mapping.TargetSystemItems
                                .Where(x => x.MappedSystemId == previousTargetDataStandardId)
                                .Select(x => x.SystemItemId)
                                .Except(commonPathsBetweenVersions.Select(y => y.SourceSystemItem.SystemItemId)).Any()) continue;

                            var map = new Model.AutoMap.AutoMap()
                            {
                                SourceSystemItem = source,
                                BusinessLogic = mapping.BusinessLogic,
                                MappingMethod = MappingMethodType.EnterMappingBusinessLogic,
                                TargetSystemItems = new List<ElementDetailsSearchModel>(),
                                Reason = AutoMappingReasonType.PreviousTargetVersion,
                                PreviousTargetDataStandard = previousTargetStandard,
                                MappingProjectName = mappingProject.ProjectName
                            };

                            foreach (var targetItem in mapping.TargetSystemItems.Where(x => x.MappedSystemId == previousTargetDataStandardId && (x.ItemTypeId == ItemType.Element.Id || x.ItemTypeId == ItemType.Enumeration.Id )))
                            {
                                var path = commonPathsBetweenVersions.First(x => x.SourceSystemItem.SystemItemId == targetItem.SystemItemId);
                                if (path.Reason == AutoMappingReasonType.PreviousVersionDelta)
                                    map.BusinessLogic = map.BusinessLogic.Replace(path.SourceSystemItem.DomainItemPath, path.TargetSystemItems.First().DomainItemPath);
                                map.TargetSystemItems.Add(path.TargetSystemItems.First());
                            }

                            results.Add(map);
                        }
                        else if (mapping.MappingMethodTypeId == MappingMethodType.MarkForExtension.Id)
                        {
                            results.Add(new Model.AutoMap.AutoMap()
                            {
                                BusinessLogic = "Mark for Extension",
                                SourceSystemItem = source,
                                MappingMethod = MappingMethodType.MarkForExtension,
                                Reason = AutoMappingReasonType.PreviousTargetVersion,
                                PreviousTargetDataStandard = previousTargetStandard,
                                MappingProjectName = mappingProject.ProjectName
                            });
                        }
                        else if (mapping.MappingMethodTypeId == MappingMethodType.MarkForOmission.Id)
                        {
                            results.Add(new Model.AutoMap.AutoMap()
                            {
                                OmissionReason = mapping.OmissionReason,
                                SourceSystemItem = source,
                                MappingMethod = MappingMethodType.MarkForOmission,
                                Reason = AutoMappingReasonType.PreviousTargetVersion,
                                PreviousTargetDataStandard = previousTargetStandard,
                                MappingProjectName = mappingProject.ProjectName
                            });
                        }
                    }
                }
            }
            return results;
        }

        private ICollection<Model.AutoMap.AutoMap> GetTransiviteResults(Guid sourceDataStandardId, Guid targetDataStandardId,
            ICollection<ElementDetailsSearchModel> sourceSystemItems, ICollection<ElementDetailsSearchModel> targetSystemItems)
        {
            var baseQuery = _mappingProjectRepository.GetAllQueryable().Where(x => x.IsActive && (Principal.Current.IsAdministrator || x.IsPublic || x.Users.Any(y => y.UserId == Principal.Current.UserId)));

            var sourceMappingProjectTargets = baseQuery.Where(x => x.SourceDataStandardMappedSystemId == sourceDataStandardId).Select(x => x.TargetDataStandardMappedSystemId).ToList();
            var sourceMappingProjectSources = baseQuery.Where(x => x.TargetDataStandardMappedSystemId == sourceDataStandardId).Select(x => x.SourceDataStandardMappedSystemId).ToList();

            var targetMappingProjectTargets = baseQuery.Where(x => x.SourceDataStandardMappedSystemId == targetDataStandardId).Select(x => x.TargetDataStandardMappedSystemId).ToList();
            var targetMappingProjectSources = baseQuery.Where(x => x.TargetDataStandardMappedSystemId == targetDataStandardId).Select(x => x.SourceDataStandardMappedSystemId).ToList();

            var trasitiveStandardIds = sourceMappingProjectTargets.Union(sourceMappingProjectSources).Intersect(targetMappingProjectSources.Union(targetMappingProjectTargets)).ToList();
            var transitiveResults = new List<Model.AutoMap.AutoMap>();

            foreach (var mappedSystemId in trasitiveStandardIds)
            {
                var transtiveSystemItems = _systemItemRepository.GetAllForComparison(mappedSystemId)
                .Where(x => x.ItemTypeId == ItemType.Element.Id || x.ItemTypeId == ItemType.Enumeration.Id).ToList();

                var transitiveToSource = ConvertToAutoMapsForTransitive(mappedSystemId, sourceDataStandardId, transtiveSystemItems, sourceSystemItems);
                var transitiveToTarget = ConvertToAutoMapsForTransitive(mappedSystemId, targetDataStandardId, transtiveSystemItems, targetSystemItems);

                var standard = _mappedSystemRepository.Get(mappedSystemId);

                var groupedTransitiveMappings = transitiveToSource.Concat(transitiveToTarget);
                var groupingResults = groupedTransitiveMappings.Where(x => x.SourceSystemItem != null).GroupBy(x => x.SourceSystemItem.SystemItemId);
                foreach (var mappingGroup in groupingResults)
                {
                    var autoMapsSources = mappingGroup.Where(x => x.CommonDataStandard.DataStandardId == sourceDataStandardId).ToList();
                    var autoMapsTargets = mappingGroup.Where(x => x.CommonDataStandard.DataStandardId == targetDataStandardId).ToList();

                    if (autoMapsTargets.Count() == 1 && autoMapsSources.Any())
                    {
                        var target = autoMapsTargets.First();
                        foreach (var source in autoMapsSources)
                        {
                            transitiveResults.Add(new Model.AutoMap.AutoMap
                            {
                                BusinessLogic = String.Format("[{0}]", target.TargetSystemItems.First().DomainItemPath),
                                SourceSystemItem = source.TargetSystemItems.First(),
                                TargetSystemItems = new List<ElementDetailsSearchModel> { target.TargetSystemItems.First() },
                                MappingMethod = MappingMethodType.EnterMappingBusinessLogic,
                                CommonSystemItem = source.SourceSystemItem,
                                CommonDataStandard = _mapper.Map<DataStandardViewModel>(standard),
                                Reason = AutoMappingReasonType.Transitive,
                                MappingProjectName = autoMapsTargets.First().MappingProjectName
                            });
                        }
                    }
                }
            }
            return transitiveResults;
        }

        private ICollection<Model.AutoMap.AutoMap> ConvertToAutoMapsForTransitive(Guid sourceDataStandardId, Guid targetDataStandardId,
            ICollection<ElementDetailsSearchModel> sourceSystemItems, ICollection<ElementDetailsSearchModel> targetSystemItems)
        {
            var autoMaps = new List<Model.AutoMap.AutoMap>();

            var mappingProjects = _mappingProjectRepository
                .GetAllQueryable().Where(x => x.IsActive && (Principal.Current.IsAdministrator || x.IsPublic || x.Users.Any(y => y.UserId == Principal.Current.UserId)))
                .Where(x => (x.SourceDataStandardMappedSystemId == sourceDataStandardId && x.TargetDataStandardMappedSystemId == targetDataStandardId) ||
                    (x.TargetDataStandardMappedSystemId == sourceDataStandardId && x.SourceDataStandardMappedSystemId == targetDataStandardId));

            foreach (var mappingProject in mappingProjects)
            {
                if (mappingProject.SourceDataStandardMappedSystemId == sourceDataStandardId)
                {
                    autoMaps.AddRange(_systemItemMapRepository.GetAllForComparison(mappingProject.MappingProjectId).Where(x => x.TargetSystemItems != null && x.TargetSystemItems.Count(y => (y.ItemTypeId == ItemType.Element.Id || y.ItemTypeId == ItemType.Enumeration.Id) && y.MappedSystemId == targetDataStandardId) == 1).Select(mapping => new Model.AutoMap.AutoMap
                    {
                        SourceSystemItem = sourceSystemItems.FirstOrDefault(x => x.SystemItemId == mapping.SourceSystemItemId),
                        TargetSystemItems = new List<ElementDetailsSearchModel> {targetSystemItems.First(x => x.SystemItemId == mapping.TargetSystemItems.First(y => (y.ItemTypeId == ItemType.Element.Id || y.ItemTypeId == ItemType.Enumeration.Id) && y.MappedSystemId == targetDataStandardId).SystemItemId)},
                        CommonDataStandard = new DataStandardViewModel {DataStandardId = targetDataStandardId},
                        MappingProjectName = mappingProject.ProjectName
                    }));
                }
                else
                {
                    autoMaps.AddRange(_systemItemMapRepository.GetAllForComparison(mappingProject.MappingProjectId).Where(x => x.TargetSystemItems != null && x.TargetSystemItems.Count(y => (y.ItemTypeId == ItemType.Element.Id || y.ItemTypeId == ItemType.Enumeration.Id)) == 1).Select(mapping => new Model.AutoMap.AutoMap
                    {
                        SourceSystemItem = sourceSystemItems.FirstOrDefault(x =>
                        {
                            var firstOrDefault = mapping.TargetSystemItems.FirstOrDefault(y => (y.ItemTypeId == ItemType.Element.Id || y.ItemTypeId == ItemType.Enumeration.Id));
                            return firstOrDefault != null && x.SystemItemId == firstOrDefault.SystemItemId;
                        }),
                        TargetSystemItems = new List<ElementDetailsSearchModel> {targetSystemItems.First(x => x.SystemItemId == mapping.SourceSystemItemId)},
                        CommonDataStandard = new DataStandardViewModel {DataStandardId = targetDataStandardId},
                        MappingProjectName = mappingProject.ProjectName
                    }));
                }
            }

            return autoMaps;
        }
    }
}
