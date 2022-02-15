// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Repositories;
using ItemType = MappingEdu.Core.Domain.Enumerations.ItemType;

namespace MappingEdu.Core.Services.Mapping
{
    public interface IBusinessLogicParser
    {
        SystemItem[] ParseReferencedSystemItems(string businessLogic, bool isEnumeration, MappedSystem targetMappedSystem);
    }

    public class BusinessLogicParser : IBusinessLogicParser
    {
        private readonly ISystemItemRepository _systemItemRepository;

        public BusinessLogicParser(ISystemItemRepository systemItemRepository)
        {
            _systemItemRepository = systemItemRepository;
        }

        public SystemItem[] ParseReferencedSystemItems(string businessLogic, bool isEnumeration, MappedSystem targetMappedSystem)
        {
            var targetSystemItems = new List<SystemItem>();

            if (string.IsNullOrWhiteSpace(businessLogic))
            {
                return targetSystemItems.ToArray();
            }

            // (?<=[) - positive look behind for [
            // .+? - non greedy match for the content
            // (?=]) - positive look ahead for [
            // matching all instances of [Domain.Entity.Element] or [Domain.Entity.SubEntity.Element] within the business logic string
            var regexBrackets = new Regex(@"(?<=\[).+?(?=\])");

            // (?=\.) - positive look ahead for period
            // [^.]+ - match for all content that is not a period
            // | or
            // (?<=\.) - positive look behind for period
            // [^.]+ - match for all content that is not a period
            // matching each individual item name in Domain.Entity.Element
            var regexPeriodDelimited = new Regex(@"[^.]+(?=\.)|(?<=\.)[^.]+");

            var bracketMatch = regexBrackets.Match(businessLogic);

            while (bracketMatch.Success)
            {
                var periodMatches = regexPeriodDelimited.Matches(bracketMatch.Value);

                if (isEnumeration)
                {
                    if (periodMatches.Count < 2)
                    {
                        SystemItem targetEntityItem = null;
                        foreach (Match periodMatch in periodMatches)
                            targetEntityItem = FindTargetSystemItem(targetMappedSystem, targetEntityItem, periodMatch.Value);
                       
                        //throw new Exception(
                        //    "Enumeration Business Logic mapping target must contain at least a Domain and Enumeration in the format [Domain.Enumeration].");
                    }
                }
                else if (periodMatches.Count < 3)
                {
                    SystemItem targetEntityItem = null;
                    foreach (Match periodMatch in periodMatches)
                        targetEntityItem = FindTargetSystemItem(targetMappedSystem, targetEntityItem, periodMatch.Value);

                    //throw new Exception(
                    //    "Business Logic mapping target must contain at least a Domain, Entity, and Element in the format [Domain.Entity.Element].");
                }

                SystemItem targetSystemItem = null;
                foreach (Match periodMatch in periodMatches)
                    targetSystemItem = FindTargetSystemItem(targetMappedSystem, targetSystemItem, periodMatch.Value);

                targetSystemItems.Add(targetSystemItem);

                bracketMatch = bracketMatch.NextMatch();
            }

            return targetSystemItems.ToArray();
        }

        private SystemItem FindTargetSystemItem(MappedSystem mappedSystem, SystemItem parentSystemItem, string nextTargetItemName)
        {
            SystemItem newTargetSystemItem;

            if (null == parentSystemItem)
            {
                newTargetSystemItem =
                    _systemItemRepository.GetAllItems().FirstOrDefault(
                        si => si.ItemName.Equals(nextTargetItemName, StringComparison.OrdinalIgnoreCase) &&
                              si.MappedSystemId == mappedSystem.MappedSystemId &&
                              si.ItemTypeId.Equals(ItemType.Domain.Id));

                if (null == newTargetSystemItem)
                {
                    throw new Exception(
                        string.Format(
                            "{0} not found as Domain for {1} {2}.", nextTargetItemName, mappedSystem.SystemName, mappedSystem.SystemVersion));
                }
            }
            else
            {
                newTargetSystemItem =
                    _systemItemRepository.GetAllItems().FirstOrDefault(
                        si => si.ItemName.Equals(nextTargetItemName, StringComparison.OrdinalIgnoreCase) &&
                              si.MappedSystemId == mappedSystem.MappedSystemId &&
                              si.ParentSystemItemId == parentSystemItem.SystemItemId);

                if (null == newTargetSystemItem)
                {
                    throw new Exception(string.Format("{0} not found as child of {1}.", nextTargetItemName, parentSystemItem.ItemName));
                }
            }

            return newTargetSystemItem;
        }
    }
}