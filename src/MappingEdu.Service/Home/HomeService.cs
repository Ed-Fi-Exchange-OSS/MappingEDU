// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using MappingEdu.Core.DataAccess.Repositories;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.Home;

namespace MappingEdu.Service.Home
{
    public interface IHomeService
    {
        HomeViewModel Get();
    }

    public class HomeService : IHomeService
    {
        private IMappingProjectRepository _mappingProjectRepository;
        private IMappedSystemRepository _mappedSystemRepository;

        public HomeService(IMappingProjectRepository mappingProjectRepository, IMappedSystemRepository mappedSystemRepository )
        {
            _mappingProjectRepository = mappingProjectRepository;
            _mappedSystemRepository = mappedSystemRepository;
        }

        public HomeViewModel Get()
        {
            var mappingProjects = _mappingProjectRepository.GetAll();
            var dataStandards = _mappedSystemRepository.GetAll();

            var standardList = (from standard in dataStandards
                let mappedSystemUpdate = standard.UserUpdates.OrderByDescending(x => x.UpdateDate).FirstOrDefault(x => x.UserId == Principal.Current.UserId)
                select new DataStandardListItem()
                {
                    UserUpdateDate = (mappedSystemUpdate != null) ? mappedSystemUpdate.UpdateDate : DateTime.MinValue,
                    SystemName = standard.SystemName,
                    SystemVersion = standard.SystemVersion,
                    DataStandardId = standard.MappedSystemId,
                    Flagged = standard.FlaggedBy.Select(x => x.Id).Contains(Principal.Current.UserId),
                    ContainsExtensions = (standard.AreExtensionsPublic || standard.HasAccess(MappedSystemUser.MappedSystemUserRole.Owner)) && standard.Extensions.Any()
                    
                }).ToList();

            var projectList = (from project in mappingProjects
                              let mappingProjectUpdate = project.UserUpdates.OrderByDescending(x => x.UpdateDate).FirstOrDefault(x => x.UserId == Principal.Current.UserId)
                              let source = standardList.FirstOrDefault(x => x.DataStandardId == project.SourceDataStandardMappedSystemId)
                              let target = standardList.FirstOrDefault(x => x.DataStandardId == project.TargetDataStandardMappedSystemId)
                              select new MappingProjectListItem()
                                {
                                    UserUpdateDate = (mappingProjectUpdate != null) ? mappingProjectUpdate.UpdateDate : DateTime.MinValue,
                                    MappingProjectId = project.MappingProjectId,
                                    ProjectName = project.ProjectName,
                                    ProjectStatusTypeName = project.ProjectStatusType.Name,
                                    SourceDataStandard = source,
                                    TargetDataStandard = target,
                                    Flagged = project.FlaggedBy.Select(x => x.Id).Contains(Principal.Current.UserId),
                                    Notifications = project.UserNotifications.Count(x => !x.HasSeen && !x.IsDismissed && Principal.Current.UserId == x.UserId)
                                 }).ToList();

            var homeViewModel = new HomeViewModel
            {
                DataStandardList = standardList,
                MappingProjectList = projectList
            };
            return homeViewModel;
        }
    }
}