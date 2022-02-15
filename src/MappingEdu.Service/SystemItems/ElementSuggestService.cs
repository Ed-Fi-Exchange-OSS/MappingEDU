// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;
using MappingEdu.Common.Exceptions;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.EntityHints;
using MappingEdu.Service.MappingProjects;
using MappingEdu.Service.Model.ElementDetails;
using MappingEdu.Service.Model.MappingProject;
using MappingEdu.Service.Providers;
using MappingEdu.Service.Util;
using MappingMethodType = MappingEdu.Core.Domain.Enumerations.MappingMethodType;

namespace MappingEdu.Service.SystemItems
{
    public class SuggestElementModel
    {
        public string BusinessLogic { get; set; }
        public bool IsSuggestedMapping { get; set; }
        public int? MappingMethodTypeId { get; set; }
        public string OmissionReason { get; set; }
        public double Percentage { get; set; }
        public ICollection<string> Reasons { get; set; }
        public string Definition { get; set; }
        public Guid DomainId { get; set; }
        public string DomainItemPath { get; set; }
        public string DomainName { get; set; }
        public bool IsExtended { get; set; }
        public Guid SystemItemId { get; set; }
    }

    public class TargetSuggestItems
    {
        public string Definition { get; set; }
        public Guid DomainId { get; set; }
        public string DomainItemPath { get; set; }
        public string DomainName { get; set; }
        public bool IsExtended { get; set; }
        public Guid SystemItemId { get; set; }
    }

    public class SuggestMappingModel
    {
        public string BusinessLogic { get; set; }
        public bool IsSuggestedMapping { get; set; }
        public int? MappingMethodTypeId { get; set; }
        public string OmissionReason { get; set; }
        public double Percentage { get; set; }
        public ICollection<string> Reasons { get; set; }
        public ICollection<TargetSuggestItems> TargetSuggestItems { get; set; }
    }

    public class SuggestModel
    {
        public ICollection<SuggestMappingModel> MappingSuggestions { get; set; }
        public ICollection<SuggestElementModel> ElementSuggestions { get; set; }  
    }

    public interface IElementSuggestService
    {
        SuggestModel SuggestElements(Guid systemItemId, Guid targetDataStandardId);
    }

    public class ElementSuggestService : IElementSuggestService
    {
        private readonly IRepository<MappedSystem> _dataStandardRepository;
        private readonly ILoggingProvider<ElementSuggestService> _logger;
        private readonly IMappingProjectRepository _mappingProjectRepository;
        private readonly ISystemItemRepository _systemItemRepository;
        private readonly IEntityHintService _entityHintService;
        private readonly IMappingProjectSynonymService _synonymService;

        public ElementSuggestService(
            ISystemItemRepository systemItemRepository, IRepository<MappedSystem> dataStandardRepository, ILoggingProvider<ElementSuggestService> logger,
            IMappingProjectRepository mappingProjectRepository, IEntityHintService entityHintService, IMappingProjectSynonymService synonymService)
        {
            _systemItemRepository = systemItemRepository;
            _dataStandardRepository = dataStandardRepository;
            _logger = logger;
            _mappingProjectRepository = mappingProjectRepository;
            _entityHintService = entityHintService;
            _synonymService = synonymService;
        }
        
        /* Element Suggestions come from anytime the source/target elements appear in business logic
         * Mapping Suggestions come when business logic can be assumed
         *  i.e. If there is similar mapping project or previous source */

        public SuggestModel SuggestElements(Guid mappingProjectId, Guid systemItemId)
        {
            var suggestList = new SuggestModel
            {
               ElementSuggestions = new List<SuggestElementModel>(),
               MappingSuggestions = new List<SuggestMappingModel>()
            };

            _logger.LogInfo("User accessed suggestions");

            var systemItem = _systemItemRepository.Get(systemItemId);
            var mappingProject = _mappingProjectRepository.Get(mappingProjectId);

            if(mappingProject == null)
                throw new NotFoundException(string.Format("Mapping Project with id '{0}' does not exist.", mappingProjectId));
            if (!Principal.Current.IsAdministrator && !mappingProject.HasAccess())
                throw new SecurityException("User needs at least Guest Access on both standards to peform this action");

            var domainItemPath = Utility.GetDomainItemPath(systemItem);
            var targetDataStandardId = mappingProject.TargetDataStandardMappedSystemId;
            var targetSystemItems = _systemItemRepository.GetAllForComparison(targetDataStandardId).ToList();
            var synonyms = _synonymService.Get(mappingProjectId);

            //Compares Domain Item Path excluding the Element Name and selects results with at least a 50% Match
            UpdateList(suggestList.ElementSuggestions, GetEntityMatchResults(domainItemPath, mappingProjectId, systemItem, targetSystemItems, synonyms));

            //Compares Definition Strings and selects results with at least a 50% Match
            if (systemItem.Definition != null) UpdateList(suggestList.ElementSuggestions, GetDefinitionResults(systemItem.Definition, systemItem.ItemTypeId, targetSystemItems, synonyms));

            //Compares Item Name Strings and selects results with at least a 50% Match
            UpdateList(suggestList.ElementSuggestions, GetNameMatchResults(systemItem.ItemName, systemItem.ItemTypeId, targetSystemItems, synonyms));

            //Gets Results from other mapping projects that contain both standards
            UpdateList(suggestList, GetSimilarProjectResults(systemItem, targetDataStandardId, mappingProjectId));

            //Gets Results where there is a common mapping between standards
            UpdateList(suggestList.ElementSuggestions, GetTransiviteResults(systemItem, targetDataStandardId, targetSystemItems));

            //Get Previous Source Matchess
            UpdateList(suggestList, GetPreviousSourceResults(systemItem, targetDataStandardId));

            //Get Previous Target Matches
            UpdateList(suggestList, GetPreviousTargetResults(systemItem, targetDataStandardId, targetSystemItems));

            //Get Previous Source Previous Target Matches
            UpdateList(suggestList, GetPreviousPreviousResults(systemItem, targetDataStandardId, targetSystemItems));

            //Compares Item Technical Name Strings and selects results with at least a 50% Match
            if (systemItem.TechnicalName != null) UpdateList(suggestList.ElementSuggestions, GetTechnicalNameMatchResults(systemItem.TechnicalName, systemItem.ItemTypeId, targetSystemItems, synonyms));

            //Re ranks based on who has the most common words
            GetCommonWordsMatch(domainItemPath, systemItem.ItemTypeId, suggestList.ElementSuggestions);

            suggestList.ElementSuggestions = suggestList.ElementSuggestions.OrderByDescending(item => item.Percentage).ThenBy(item => item.DomainItemPath).ToArray();
            return suggestList;
        }

        private void GetCommonWordsMatch(string domainItemPath, int itemTypeId, IEnumerable<SuggestElementModel> targetItems)
        {
            foreach (var item in targetItems)
            {
                var similarity = Math.Round(CalculateCommonWords(domainItemPath, item.DomainItemPath) * 100, 2);
                item.Percentage += (100 - item.Percentage) * (similarity * .1) / 100;
                item.Percentage = Math.Round(item.Percentage, 2);
            }
        }

        private double CalculateCommonWords(string source, string target)
        {
            if ((source == null) || (target == null)) return 0.0;

            var r = new Regex("([A-Z]+[a-z]+)");

            source = source.Replace(".", " ");
            target = target.Replace(".", " ");
            source = r.Replace(source, m => (m.Value.Length > 3 ? m.Value : m.Value.ToLower()) + " ");
            target = r.Replace(target, m => (m.Value.Length > 3 ? m.Value : m.Value.ToLower()) + " ");

            source = source.ToLower().Replace("  ", " ");
            target = target.ToLower().Replace("  ", " ");

            source = Utility.RemoveStopWords(source);
            source = Utility.ReplaceCommonWords(source);
            target = Utility.RemoveStopWords(target);
            target = Utility.ReplaceCommonWords(target);

            if ((source.Length == 0) || (target.Length == 0)) return 0.0;
            if (source == target) return 1.0;

            var sourceWords = source.Split(' ').Where(x => x != "").Select(x => (x.EndsWith("s") ? x.Remove(x.Length - 1) : x)).OrderBy(x => x).Distinct();
            var targetWords = target.Split(' ').Where(x => x != "").Select(x => (x.EndsWith("s") ? x.Remove(x.Length - 1) : x)).OrderBy(x => x).Distinct();

            var intersect = sourceWords.Intersect(targetWords).Count();

            return ((double)intersect / sourceWords.Count() + (double)intersect / targetWords.Count()) * .5;
        }

        private double CalculateSimilarity(string source, string target, MappingProjectSynonymModel[] synonyms)
        {
            if ((source == null) || (target == null)) return 0.0;

            var r = new Regex("([A-Z]+[a-z]+)");

            source = r.Replace(source, m => (m.Value.Length > 3 ? m.Value : m.Value.ToLower()) + " ");
            target = r.Replace(target, m => (m.Value.Length > 3 ? m.Value : m.Value.ToLower()) + " ");

            source = source.Replace(".", " ");
            target = target.Replace(".", " ");

            source = source.ToLower().Replace("  ", " ");
            target = target.ToLower().Replace("  ", " ");

            source = Utility.RemoveStopWords(source);
            target = Utility.RemoveStopWords(target);

            var sourceWords = source.Split(' ').Where(x => x != "").Select(x => (x.EndsWith("s") ? x.Remove(x.Length - 1) : x)).OrderBy(x => x).Distinct();
            var targetWords = target.Split(' ').Where(x => x != "").Select(x => (x.EndsWith("s") ? x.Remove(x.Length - 1) : x)).OrderBy(x => x).Distinct();

            var intersect = sourceWords.Intersect(targetWords).Count();

            if ((double) sourceWords.Count()/intersect < .5 && (double) targetWords.Count()/intersect < .5) return 0;

                if ((source.Length == 0) || (target.Length == 0)) return 0.0;
            if (source == target) return 1.0;

            var stepsToSame = ComputeLevenshteinDistance(source.ToLower(), target.ToLower());
            var percentage = 1.0 - stepsToSame / (double)Math.Max(source.Length, target.Length);

            //If source is completely in target or vice versa
            if (percentage < .8 && target.Contains(source))
            {
                var tempPercentage = ((double)source.Length / target.Length) * 2.0;
                if (tempPercentage > .8) tempPercentage = .8;
                percentage = (percentage > tempPercentage) ? percentage : tempPercentage;
            }
            else if (percentage < .8 && source.Contains(target))
            {
                var tempPercentage = ((double)target.Length / source.Length) * 2.0;
                if (tempPercentage > .8) tempPercentage = .8;
                percentage = (percentage > tempPercentage) ? percentage : tempPercentage;
            }

            percentage = percentage * .7 + ((double)intersect / sourceWords.Count() + (double)intersect / targetWords.Count()) * .15;

            return percentage;
        }

        private int ComputeLevenshteinDistance(string source, string target)
        {
            if ((source == null) || (target == null)) return 0;
            if ((source.Length == 0) || (target.Length == 0)) return 0;
            if (source == target) return source.Length;

            var sourceWordCount = source.Length;
            var targetWordCount = target.Length;

            // Step 1
            if (sourceWordCount == 0)
                return targetWordCount;

            if (targetWordCount == 0)
                return sourceWordCount;

            var distance = new int[sourceWordCount + 1, targetWordCount + 1];

            // Step 2
            for (var i = 0; i <= sourceWordCount; distance[i, 0] = i++) ;
            for (var j = 0; j <= targetWordCount; distance[0, j] = j++) ;

            for (var i = 1; i <= sourceWordCount; i++)
            {
                for (var j = 1; j <= targetWordCount; j++)
                {
                    // Step 3
                    var cost = target[j - 1] == source[i - 1] ? 0 : 1;

                    // Step 4
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                }
            }

            return distance[sourceWordCount, targetWordCount];
        }

        private static SystemItem FindExactMatch(IEnumerable<SystemItem> possibleSystemItems, SystemItem systemItem)
        {
            return
                possibleSystemItems.FirstOrDefault(
                    item => Utility.GetDomainItemPath(item) == Utility.GetDomainItemPath(systemItem));
        }

        private static IEnumerable<SystemItem> FindExactMatches(
            IEnumerable<SystemItem> possibleSystemItems, SystemItem systemItem)
        {
            return
                possibleSystemItems.Where(
                    item => Utility.GetDomainItemPath(item) == Utility.GetDomainItemPath(systemItem));
        }

        private SuggestElementModel GetElementModel(
            ElementDetailsSearchModel model, string reason, double percentage, int? mappingMethodId = null, string businessLogic = null, string omissionReason = null)
        {
            return new SuggestElementModel
            {
                Definition = model.Definition,
                DomainItemPath = model.DomainItemPath,
                DomainId = model.DomainId,
                DomainName = model.DomainItemPath.Split('.')[0],
                IsExtended = model.IsExtended,
                SystemItemId = model.SystemItemId,
                Reasons = new List<string> { reason },
                Percentage = Math.Round(percentage, 2),
                BusinessLogic = businessLogic,
                MappingMethodTypeId = mappingMethodId,
            };
        }


        private SuggestElementModel GetElementModel(
            SystemItem model, string reason, double percentage, int? mappingMethodId = null, string businessLogic = null, string omissionReason = null)
        {
            var domainItemPath = Utility.GetDomainItemPath(model);
            var domain = Utility.GetDomain(model);
            return new SuggestElementModel
            {
                Definition = model.Definition,
                DomainItemPath = domainItemPath,
                DomainId = domain.SystemItemId,
                DomainName = domain.ItemName,
                IsExtended = model.IsExtended,
                SystemItemId = model.SystemItemId,
                Reasons = new List<string> { reason },
                Percentage = Math.Round(percentage, 2),
                BusinessLogic = businessLogic,
                MappingMethodTypeId = mappingMethodId
            };
        }

        private SuggestMappingModel GetMappingModel(
    SystemItemMap model, string reason, double percentage, string businessLogic = null)
        {
            var suggestMapModel = new SuggestMappingModel
            {
                TargetSuggestItems = new List<TargetSuggestItems>(),
                Reasons = new List<string> {reason},
                Percentage = percentage,
                BusinessLogic = (businessLogic != null) ? businessLogic : model.BusinessLogic,
                OmissionReason = model.OmissionReason,
                MappingMethodTypeId = model.MappingMethodTypeId
            };

            foreach (var target in model.TargetSystemItems)
            {
                var domainItemPath = Utility.GetDomainItemPath(target);
                var domain = Utility.GetDomain(target);

                suggestMapModel.TargetSuggestItems.Add(new TargetSuggestItems
                {
                    Definition = target.Definition,
                    DomainItemPath = domainItemPath,
                    DomainId = domain.SystemItemId,
                    DomainName = domain.ItemName,
                    IsExtended = target.IsExtended,
                    SystemItemId = target.SystemItemId
                });
            }

            return suggestMapModel;
        }

        private IEnumerable<SuggestElementModel> GetNameMatchResults(
            string elementName, int itemTypeId, IEnumerable<ElementDetailsSearchModel> targetItems, MappingProjectSynonymModel[] synonyms)
        {
            return targetItems.Where(x => x.ItemTypeId == itemTypeId)
                .Select(x =>
                {
                    var targetName = x.ItemName;
                    var reasons = new List<string>();
                    foreach (var synonym in synonyms.Where(y => elementName.Contains(y.SourceWord) &&
                        targetName.Contains(y.TargetWord)).OrderBy(y => y.SourceWord.Length))
                    {
                        targetName = targetName.Replace(synonym.TargetWord, synonym.SourceWord);
                        reasons.Add(String.Format("- Synonym between {0} and {1}.", synonym.SourceWord, synonym.TargetWord));
                    }

                    return new
                    {
                        similarity = Math.Round(CalculateSimilarity(elementName, targetName, synonyms) * 100, 2),
                        reasons = reasons,
                        item = x
                    };
                })
                .Where(x => x.similarity >= 50)
                .Select(x =>
                {
                    var item = GetElementModel(x.item, x.similarity == 100 ? "Same Element Name." : "Similar Element Name.", x.similarity * .7);
                    foreach (var reason in x.reasons) { item.Reasons.Add(reason); }
                    return item;
                });
        }

        private IEnumerable<SuggestElementModel> GetTechnicalNameMatchResults(
            string technicalName, int itemTypeId, IEnumerable<ElementDetailsSearchModel> targetItems, MappingProjectSynonymModel[] synonyms)
        {
            return targetItems.Where(x => x.ItemTypeId == itemTypeId && x.TechnicalName != null && technicalName != null)
                .Select(x => new { similarity = Math.Round(CalculateSimilarity(technicalName, x.TechnicalName, synonyms) * 100, 2), item = x })
                .Where(x => x.similarity >= 50)
                .Select(x => GetElementModel(x.item, x.similarity == 100 ? "Same Technical Name." : "Similar Technical Name.", x.similarity * .5));
        }

        private IEnumerable<SuggestElementModel> GetEntityMatchResults(
    string domainItemPath, Guid mappingProjectId, SystemItem systemItem, IEnumerable<ElementDetailsSearchModel> targetItems, MappingProjectSynonymModel[] synonyms)
        {

            var split = domainItemPath.Split('.');
            var sourceDomainItemPath = string.Join(".", split.Take(split.Length - 1));
            var entityFilter = _entityHintService.GetEntityFilter(mappingProjectId, systemItem.SystemItemId);

            return targetItems.Where(x => x.ItemTypeId == systemItem.ItemTypeId)
                .Select(x =>
                {
                    var targetDomainItemPath = "";
                    var reasons = new List<string>();
                    if (entityFilter != null && x.DomainItemPath.StartsWith(entityFilter.TargetEntity.DomainItemPath + '.'))
                    {
                        targetDomainItemPath = x.DomainItemPath.Replace(entityFilter.TargetEntity.DomainItemPath + '.', entityFilter.SourceEntity.DomainItemPath + '.');
                        reasons.Add(String.Format("- Entity Hint between {0} and {1}.", entityFilter.SourceEntity.DomainItemPath, entityFilter.TargetEntity.DomainItemPath));
                    }
                    else targetDomainItemPath = x.DomainItemPath;

                    split = targetDomainItemPath.Split('.');
                    targetDomainItemPath = string.Join(".", split.Take(split.Length - 1));

                    foreach (var synonym in synonyms.Where(y => sourceDomainItemPath.Contains(y.SourceWord) &&
                        targetDomainItemPath.Contains(y.TargetWord)).OrderBy(y => y.SourceWord.Length))
                    {
                        targetDomainItemPath = targetDomainItemPath.Replace(synonym.TargetWord, synonym.SourceWord);
                        reasons.Add(String.Format("- Synonym between {0} and {1}.", synonym.SourceWord, synonym.TargetWord));
                    }


                    return new
                    {
                        similarity = Math.Round(CalculateSimilarity(sourceDomainItemPath, targetDomainItemPath, synonyms) * 100, 2),
                        reasons = reasons,
                        item = x
                    };
                })
                .Where(x => x.similarity >= 50)
                .Select(x =>
                {
                    var item = GetElementModel(x.item, x.similarity == 100 ? "Same Entity Name." : "Similar Entity Name.", x.similarity * .5);
                    foreach (var reason in x.reasons) { item.Reasons.Add(reason); }
                    return item;
                });
        }

        private IEnumerable<SuggestElementModel> GetPathMatchResults(
            string domainItemPath, Guid mappingProjectId, SystemItem systemItem, ElementDetailsSearchModel[] targetItems, MappingProjectSynonymModel[] synonyms)
        {
            var entityFilter = _entityHintService.GetEntityFilter(mappingProjectId, systemItem.SystemItemId);

            return targetItems.Where(x => x.ItemTypeId == systemItem.ItemTypeId)
                .Select(x =>
                {
                    var targetDomainItemPath = "";
                    var reasons = new List<string>();
                    if (entityFilter != null && x.DomainItemPath.StartsWith(entityFilter.TargetEntity.DomainItemPath + '.'))
                    {
                        targetDomainItemPath = x.DomainItemPath.Replace(entityFilter.TargetEntity.DomainItemPath + '.', entityFilter.SourceEntity.DomainItemPath + '.');
                        reasons.Add(String.Format("- Entity Hint between {0} and {1}.", entityFilter.SourceEntity.DomainItemPath, entityFilter.TargetEntity.DomainItemPath));
                    }
                    else targetDomainItemPath = x.DomainItemPath;

                    foreach (var synonym in synonyms.Where(y => domainItemPath.Contains(y.SourceWord) && 
                        targetDomainItemPath.Contains(y.TargetWord)).OrderBy(y => y.SourceWord.Length))
                    {
                        targetDomainItemPath = targetDomainItemPath.Replace(synonym.TargetWord, synonym.SourceWord);
                        reasons.Add(String.Format("- Synonym between {0} and {1}.", synonym.SourceWord, synonym.TargetWord));
                    }

                    return new
                    {
                        similarity = Math.Round(CalculateSimilarity(domainItemPath, targetDomainItemPath, synonyms)*100, 2),
                        reasons = reasons,
                        item = x
                    };
                })
                .Where(x => x.similarity >= 50)
                .Select(x =>
                {
                    var item = GetElementModel(x.item, x.similarity == 100 ? "Same Path." : "Similar Path.", x.similarity*.5);
                    foreach (var reason in x.reasons) { item.Reasons.Add(reason); }
                    return item;
                });
        }

        private IEnumerable<SuggestElementModel> GetDefinitionResults(
            string definition, int itemTypeId, IEnumerable<ElementDetailsSearchModel> targetItems, MappingProjectSynonymModel[] synonyms)
        {
            return targetItems.Where(x => x.Definition != null && x.ItemTypeId == itemTypeId)
                .Select(x =>
                {
                    var targetDefinition = x.Definition;
                    var reasons = new List<string>();
                    foreach (var synonym in synonyms.Where(y => definition.Contains(y.SourceWord) &&
                        targetDefinition.Contains(y.TargetWord)).OrderBy(y => y.SourceWord.Length))
                    {
                        targetDefinition = targetDefinition.Replace(synonym.TargetWord, synonym.SourceWord);
                        reasons.Add(String.Format("- Synonym between {0} and {1}.", synonym.SourceWord, synonym.TargetWord));
                    }

                    return new
                    {
                        similarity = Math.Round(CalculateSimilarity(definition, targetDefinition, synonyms)*100, 2),
                        reasons = reasons,
                        item = x
                    };
                })
                .Where(x => x.similarity >= 50)
                .Select(x =>
                {
                    var item = GetElementModel(x.item, x.similarity == 100 ? "Same Definition." : "Similar Definition.", x.similarity*.6);
                    foreach (var reason in x.reasons) { item.Reasons.Add(reason); }
                    return item;
                });
        }

        private SuggestModel GetPreviousTargetResults(
            SystemItem systemItem, Guid targetDataStandardId, IEnumerable<ElementDetailsSearchModel> targetSystemItems)
        {
            var sourceDataStandard = systemItem.MappedSystem;
            var targetDataStandard = _dataStandardRepository.Get(targetDataStandardId);
            var previousTarget = targetDataStandard.PreviousVersionMappedSystem;

            var suggestModel = new SuggestModel
            {
                ElementSuggestions = new List<SuggestElementModel>(),
                MappingSuggestions = new List<SuggestMappingModel>()
            };

            if (null == previousTarget)
                return suggestModel;

            foreach (var map in systemItem.SourceSystemItemMaps.Where(x => x.MappingProject.TargetDataStandardMappedSystemId == previousTarget.MappedSystemId))
            {
                if (map.MappingMethodType == MappingMethodType.EnterMappingBusinessLogic && map.BusinessLogic != null)
                {
                    var businessLogic = map.BusinessLogic;
                    var createSuggestedMapping = true;

                    foreach (var targetSystemItem in map.TargetSystemItems)
                    {
                        if (targetSystemItem.NextSystemItemVersionDeltas.Any())
                        {
                            var suggestions = targetSystemItem.NextSystemItemVersionDeltas
                                .Where(vd => vd.NewSystemItem != null && vd.NewSystemItem.MappedSystem.MappedSystemId == targetDataStandardId)
                                .Select(vd =>
                                    GetElementModel(vd.NewSystemItem,
                                        string.Format("{0} {1} to {2} {3} Previous Target Delta match ({4}).",
                                            sourceDataStandard.SystemName, sourceDataStandard.SystemVersion,
                                            previousTarget.SystemName, previousTarget.SystemVersion,
                                            map.MappingProject.ProjectName),
                                        80));

                            if (suggestions.Count() > 1) createSuggestedMapping = false;

                            foreach (var suggestion in suggestions)
                            {
                                suggestModel.ElementSuggestions.Add(suggestion);
                                if (createSuggestedMapping)
                                {
                                    var targetDomainItemPath = Utility.GetDomainItemPath(targetSystemItem);
                                    businessLogic.Replace(targetDomainItemPath, suggestion.DomainItemPath);
                                }
                            }

                        }
                        else
                        {
                            var path = Utility.GetDomainItemPath(targetSystemItem);
                            var targetItem = targetSystemItems.FirstOrDefault(x => x.DomainItemPath == path);
                            if (targetItem != null)
                            {
                                suggestModel.ElementSuggestions.Add(
                                    GetElementModel(targetItem,
                                        string.Format("{0} {1} to {2} {3} Previous Target Path match ({4}).",
                                            sourceDataStandard.SystemName, sourceDataStandard.SystemVersion,
                                            previousTarget.SystemName, previousTarget.SystemVersion,
                                            map.MappingProject.ProjectName),
                                        80));
                            }
                            else createSuggestedMapping = false;
                        }
                    }

                    if (createSuggestedMapping)
                    {
                        suggestModel.MappingSuggestions.Add(GetMappingModel(map, string.Format("{0} {1} to {2} {3} Previous Target Mapping ({4}).",
                            sourceDataStandard.SystemName, sourceDataStandard.SystemVersion,
                            previousTarget.SystemName, previousTarget.SystemVersion,
                            map.MappingProject.ProjectName), 80, businessLogic));
                    }
                }
                else
                {
                    suggestModel.MappingSuggestions.Add(GetMappingModel(map, string.Format("{0} {1} to {2} {3} Previous Target Mapping ({4}).",
                            sourceDataStandard.SystemName, sourceDataStandard.SystemVersion,
                            previousTarget.SystemName, previousTarget.SystemVersion,
                            map.MappingProject.ProjectName), 80));
                }
            }

            return suggestModel;
        }

        private SuggestModel GetSimilarProjectResults(SystemItem systemItem, Guid targetDataStandardId, Guid currentMappingProjectId)
        {
            var suggestModel = new SuggestModel
            {
                ElementSuggestions = new List<SuggestElementModel>(),
                MappingSuggestions = new List<SuggestMappingModel>()
            };

            var mappings = systemItem.SourceSystemItemMaps.Where(x => x.MappingProject.IsActive && x.MappingProject.TargetDataStandardMappedSystemId == targetDataStandardId &&
                x.MappingProjectId != currentMappingProjectId &&
                (Principal.Current.IsAdministrator || x.MappingProject.IsPublic || x.MappingProject.Users.Any(y => y.UserId == Principal.Current.UserId)));

            foreach (var map in mappings)
            {
                var reason = string.Format("Similar Project Mapping ({0} {1} -> {2} {3})",
                    map.MappingProject.SourceDataStandard.SystemName, map.MappingProject.SourceDataStandard.SystemVersion,
                    map.MappingProject.TargetDataStandard.SystemName, map.MappingProject.TargetDataStandard.SystemVersion);

                foreach (var targetItem in map.TargetSystemItems)
                {
                    suggestModel.ElementSuggestions.Add(GetElementModel(targetItem, reason, 80));
                }

                suggestModel.MappingSuggestions.Add(GetMappingModel(map, reason, 80));
            }

            mappings = systemItem.TargetSystemItemMaps.Where(x => x.MappingProject.IsActive && x.MappingProject.SourceDataStandardMappedSystemId == targetDataStandardId &&
                            x.MappingProjectId != currentMappingProjectId &&
                            (Principal.Current.IsAdministrator || x.MappingProject.IsPublic || x.MappingProject.Users.Any(y => y.UserId == Principal.Current.UserId)));

            foreach (var map in mappings)
            {
                suggestModel.ElementSuggestions.Add(GetElementModel(map.SourceSystemItem,
                    string.Format("Similar Project Mapping ({0} {1} -> {2} {3})",
                        map.MappingProject.SourceDataStandard.SystemName, map.MappingProject.SourceDataStandard.SystemVersion,
                        map.MappingProject.TargetDataStandard.SystemName, map.MappingProject.TargetDataStandard.SystemVersion),
                    80));
            }

            return suggestModel;
        } 

        private SuggestModel GetPreviousPreviousResults(SystemItem systemItem, Guid targetDataStandardId, IEnumerable<ElementDetailsSearchModel> targetSystemItems)
        {
            var suggestModel = new SuggestModel
            {
                ElementSuggestions = new List<SuggestElementModel>(),
                MappingSuggestions = new List<SuggestMappingModel>()
            };

            var previousSystemItems = new List<SystemItem>();
            var previousSource = systemItem.MappedSystem.PreviousVersionMappedSystem;
            var previousTarget = _dataStandardRepository.Get(targetDataStandardId).PreviousVersionMappedSystem;

            if (previousSource == null || previousTarget == null) return suggestModel;
            var previousDeltas = true;

            if (systemItem.PreviousSystemItemVersionDeltas.Any()) previousSystemItems = systemItem.PreviousSystemItemVersionDeltas.Select(x => x.OldSystemItem).ToList();
            else
            {
                previousDeltas = false;
                var items = _systemItemRepository.GetAllItems().Where(
                     si => si.MappedSystemId == systemItem.MappedSystem.PreviousMappedSystemId
                           && si.ItemName == systemItem.ItemName
                           && si.ParentSystemItem.ItemName == systemItem.ParentSystemItem.ItemName
                           && si.ItemTypeId == systemItem.ItemTypeId).ToArray();

                var previousSystemItem = FindExactMatch(items, systemItem);
                if (null != previousSystemItem) previousSystemItems.Add(previousSystemItem);
            }

            if (!previousSystemItems.Any()) return suggestModel;

            foreach (var item in previousSystemItems)
            {
                foreach (var map in item.SourceSystemItemMaps.Where(x => x.MappingProject.TargetDataStandard.MappedSystemId == previousTarget.MappedSystemId))
                {
                    if (map.MappingMethodType == MappingMethodType.EnterMappingBusinessLogic && map.BusinessLogic != null)
                    {
                        var businessLogic = map.BusinessLogic;
                        var createSuggestedMapping = true;

                        foreach (var targetSystemItem in map.TargetSystemItems)
                        {
                            if (targetSystemItem.NextSystemItemVersionDeltas.Any())
                            {
                                var deltaSuggestions = targetSystemItem.NextSystemItemVersionDeltas
                                    .Where(vd => vd.NewSystemItem != null && vd.NewSystemItem.MappedSystem.MappedSystemId == targetDataStandardId)
                                    .Select(vd =>
                                        GetElementModel(vd.NewSystemItem,
                                            string.Format("{0} {1} to {2} {3} Previous Target Delta match ({4}).",
                                                previousSource.SystemName, previousSource.SystemVersion,
                                                previousTarget.SystemName, previousTarget.SystemVersion,
                                                map.MappingProject.ProjectName),
                                            80));

                                if (deltaSuggestions.Count() > 1) createSuggestedMapping = false;

                                foreach (var suggestion in deltaSuggestions)
                                {
                                    suggestModel.ElementSuggestions.Add(suggestion);
                                    if (createSuggestedMapping)
                                    {
                                        var targetDomainItemPath = Utility.GetDomainItemPath(targetSystemItem);
                                        businessLogic.Replace(targetDomainItemPath, suggestion.DomainItemPath);
                                    }
                                }
                            }
                            else
                            {
                                var path = Utility.GetDomainItemPath(targetSystemItem);
                                var targetItem = targetSystemItems.FirstOrDefault(x => x.DomainItemPath == path);
                                if (targetItem != null)
                                {
                                    suggestModel.ElementSuggestions.Add(
                                        GetElementModel(targetItem,
                                            string.Format("{0} {1} to {2} {3} Previous Source {4} and  Previous Target Path match. ({5})",
                                                previousSource.SystemName, previousSource.SystemVersion,
                                                previousTarget.SystemName, previousTarget.SystemVersion, (previousDeltas) ? "Delta" : "Path",
                                                map.MappingProject.ProjectName),
                                            80));
                                }
                            }
                        }


                        if (createSuggestedMapping)
                        {
                            suggestModel.MappingSuggestions.Add(GetMappingModel(map, 
                                string.Format("{0} {1} to {2} {3} Previous Source {4} and  Previous Target match. ({5})",
                                    previousSource.SystemName, previousSource.SystemVersion,
                                    previousTarget.SystemName, previousTarget.SystemVersion, (previousDeltas) ? "Delta" : "Path",
                                    map.MappingProject.ProjectName), 
                                80, businessLogic));
                        }
                    }
                    else
                    {
                        suggestModel.MappingSuggestions.Add(GetMappingModel(map, 
                            string.Format("{0} {1} to {2} {3} Previous Source {4} and  Previous Target match. ({5})",
                                previousSource.SystemName, previousSource.SystemVersion,
                                previousTarget.SystemName, previousTarget.SystemVersion, (previousDeltas) ? "Delta" : "Path",
                                map.MappingProject.ProjectName), 
                            80));
                    }
                }
            }


            return suggestModel;
        }

        private SuggestModel GetPreviousSourceResults(SystemItem systemItem, Guid targetDataStandardId)
        {
            var suggestModel = new SuggestModel
            {
                ElementSuggestions = new List<SuggestElementModel>(),
                MappingSuggestions = new List<SuggestMappingModel>()
            };

            var previousSystemItems = new List<SystemItem>();
            var previousSource = systemItem.MappedSystem.PreviousVersionMappedSystem;

            if (previousSource == null) return suggestModel;
            var previousDeltas = true;

            if (systemItem.PreviousSystemItemVersionDeltas.Any()) previousSystemItems = systemItem.PreviousSystemItemVersionDeltas.Select(x => x.OldSystemItem).ToList();
            else
            {
                previousDeltas = false;
                var items = _systemItemRepository.GetAllItems().Where(
                     si => si.MappedSystemId == systemItem.MappedSystem.PreviousMappedSystemId
                           && si.ItemName == systemItem.ItemName
                           && si.ParentSystemItem.ItemName == systemItem.ParentSystemItem.ItemName
                           && si.ItemTypeId == systemItem.ItemTypeId).ToArray();

                var previousSystemItem = FindExactMatch(items, systemItem);
                if (null != previousSystemItem) previousSystemItems.Add(previousSystemItem);
            }

            if (!previousSystemItems.Any()) return suggestModel;

            foreach (var item in previousSystemItems)
            {
                foreach (var map in item.SourceSystemItemMaps.Where(x => x.MappingProject.IsActive && 
                    (Principal.Current.IsAdministrator || x.MappingProject.IsPublic || x.MappingProject.Users.Any(y => y.UserId == Principal.Current.UserId))))
                {
                    var reason = string.Format("{0} {1} to {2} {3} Previous Source {4} match. ({5})",
                        previousSource.SystemName, previousSource.SystemVersion,
                        map.MappingProject.TargetDataStandard.SystemName, map.MappingProject.TargetDataStandard.SystemVersion, (previousDeltas) ? "Delta" : "Path",
                        map.MappingProject.ProjectName);

                    foreach (var targetItem in map.TargetSystemItems.Where(x => x.MappedSystemId == targetDataStandardId))
                        suggestModel.ElementSuggestions.Add(GetElementModel(targetItem, reason, 80));

                    suggestModel.MappingSuggestions.Add(GetMappingModel(map, reason, 80));
                }
            }


            return suggestModel;
        }

        private IEnumerable<SuggestElementModel> GetTransiviteResults(
            SystemItem systemItem, Guid targetDataStandardId, IEnumerable<ElementDetailsSearchModel> targetItems)
        {
            var suggestTargetModels = new List<SuggestElementModel>();
            var sourceDataStandardId = systemItem.MappedSystemId;

            var sourceMappingProjectTargets = _mappingProjectRepository.GetAllQueryable().Where(x => x.IsActive).Where(x => x.SourceDataStandardMappedSystemId == sourceDataStandardId).Select(x => x.TargetDataStandardMappedSystemId).ToList();
            var sourceMappingProjectSources = _mappingProjectRepository.GetAllQueryable().Where(x => x.IsActive).Where(x => x.TargetDataStandardMappedSystemId == sourceDataStandardId).Select(x => x.SourceDataStandardMappedSystemId).ToList();

            var targetMappingProjectTargets = _mappingProjectRepository.GetAllQueryable().Where(x => x.IsActive).Where(x => x.SourceDataStandardMappedSystemId == targetDataStandardId).Select(x => x.TargetDataStandardMappedSystemId).ToList();
            var targetMappingProjectSources = _mappingProjectRepository.GetAllQueryable().Where(x => x.IsActive).Where(x => x.TargetDataStandardMappedSystemId == targetDataStandardId).Select(x => x.SourceDataStandardMappedSystemId).ToList();

            var trasitiveStandardIds = sourceMappingProjectTargets.Union(sourceMappingProjectSources).Intersect(targetMappingProjectSources.Union(targetMappingProjectTargets)).ToList();

            foreach (var transitiveId in trasitiveStandardIds)
            {
                var targetsFromSourceMappings = systemItem.SourceSystemItemMaps.Where(x => x.TargetSystemItems.Count() == 1 && x.TargetSystemItems.First().MappedSystemId == transitiveId).Select(x => x.TargetSystemItems.First());
                var sourceFromTargetMappings = systemItem.TargetSystemItemMaps.Where(x => x.SourceSystemItem.MappedSystemId == transitiveId && x.TargetSystemItems.Count() == 1).Select(x => x.SourceSystemItem);
                var commonTransitiveItems = sourceFromTargetMappings.ToList();
                commonTransitiveItems.AddRange(targetsFromSourceMappings);

                foreach (var commonTransitiveItem in commonTransitiveItems)
                {
                    var commonTransitiveItemAsSource = commonTransitiveItem.SourceSystemItemMaps.Where(x => x.TargetSystemItems.Count() == 1 && x.TargetSystemItems.First().MappedSystemId == targetDataStandardId).Select(x => x.TargetSystemItems.First());
                    var commonTransitiveItemAsTarget = commonTransitiveItem.TargetSystemItemMaps.Where(x => x.SourceSystemItem.MappedSystemId == targetDataStandardId && x.TargetSystemItems.Count() == 1).Select(x => x.SourceSystemItem);
                    var commonTargetItems = commonTransitiveItemAsSource.ToList();
                    commonTargetItems.AddRange(commonTransitiveItemAsTarget);

                    suggestTargetModels.AddRange(commonTargetItems.Select(item => GetElementModel(targetItems.First(x => x.SystemItemId == item.SystemItemId), string.Format("Common Mapping match to {0} {1}", commonTransitiveItem.MappedSystem.SystemName, commonTransitiveItem.MappedSystem.SystemVersion), 80)));
                }
            }

            return suggestTargetModels;
        }

        private void UpdateList(ICollection<SuggestElementModel> list, IEnumerable<SuggestElementModel> items)
        {
            foreach (var item in items)
            {
                var found = list.FirstOrDefault(x => x.SystemItemId == item.SystemItemId);
                if (found == null) list.Add(item);
                else
                {
                    foreach (var reason in item.Reasons) { found.Reasons.Add(reason); }
                    found.Percentage += (100 - found.Percentage) * item.Percentage / 100;
                    found.Percentage = Math.Round(found.Percentage, 2);
                }
            }
        }

        private void UpdateList(SuggestModel model, SuggestModel newSuggestions)
        {
            foreach (var item in newSuggestions.ElementSuggestions)
            {
                var found = model.ElementSuggestions.FirstOrDefault(x => x.SystemItemId == item.SystemItemId);
                if (found == null) model.ElementSuggestions.Add(item);
                else
                {
                    foreach (var reason in item.Reasons) { found.Reasons.Add(reason); }
                    found.Percentage += (100 - found.Percentage) * item.Percentage / 100;
                    found.Percentage = Math.Round(found.Percentage, 2);
                }
            }

            foreach (var item in newSuggestions.MappingSuggestions)
            {
                var found = model.MappingSuggestions.FirstOrDefault(x => x.BusinessLogic == item.BusinessLogic);
                if (found == null) model.MappingSuggestions.Add(item);
                else
                {
                    foreach (var reason in item.Reasons) { found.Reasons.Add(reason); }
                    found.Percentage += (100 - found.Percentage) * item.Percentage / 100;
                    found.Percentage = Math.Round(found.Percentage, 2);
                }
            }
        }
    }
}