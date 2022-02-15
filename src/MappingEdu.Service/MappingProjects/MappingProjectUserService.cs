// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using MappingEdu.Common.Exceptions;
using MappingEdu.Core.DataAccess.Repositories;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Email;
using MappingEdu.Service.Logging;
using MappingEdu.Service.Model.Logging;
using MappingEdu.Service.Model.MappingProject;
using Constants = MappingEdu.Common.Constants;

namespace MappingEdu.Service.MappingProjects
{
    public interface IMappingProjectUserService
    {
        Task<MappingProjectUserModel> AddUserToMappingProject(Guid mappingProjectId, MappingProjectUserAddModel model);

        MappingProjectUserModel GetMappingProjectUser(Guid mappingProjectId, Guid userId);

        MappingProjectUserModel[] GetUsersForMappingProject(Guid mappingProjectId);

        MappingProjectUserModel[] GetTaggableUsersForMappingProjet(Guid mappingProjectId);

        void RemoveUserFromMappingProject(Guid mappingProjectId, string userId);

        void ToggleFlagged(Guid mappingProjectId, Guid userId);
    }

    public class MappingProjectUserService : IMappingProjectUserService
    {
        private readonly ILoggingService _loggingService;
        private readonly IMappingProjectRepository _mappingProjectRepository;
        private readonly IMappedSystemRepository _mappedSystemRepository;
        private readonly IUserRepository _userRepository;

        public MappingProjectUserService(IUserRepository userRepository, IMappingProjectRepository mappingProjectRepository, IMappedSystemRepository mappedSystemRepository, 
            ILoggingService loggingService)
        {
            _loggingService = loggingService;
            _mappingProjectRepository = mappingProjectRepository;
            _mappedSystemRepository = mappedSystemRepository;
            _userRepository = userRepository;
        }

        public async Task<MappingProjectUserModel> AddUserToMappingProject(Guid mappingProjectId, MappingProjectUserAddModel model)
        {
            //For Mapping Project
            var mappingProject = GetMappingProject(mappingProjectId, MappingProjectUser.MappingProjectUserRole.Owner);

            var user = _userRepository.FindByEmail(model.Email);
            if (user == null)
                throw new NotFoundException("Unabled to find user with email: " + model.Email);

            var mappingProjectUser = mappingProject.Users.SingleOrDefault(o => o.UserId == user.Id);
            if (null == mappingProjectUser)
            {
                mappingProjectUser = new MappingProjectUser(mappingProjectId, user.Id, model.Role);
                mappingProject.Users.Add(mappingProjectUser);
                _loggingService.Post(new LoggingCreateModel()
                {
                    Source = "Shared",
                    Message = string.Format("Shared Mapping Project {0} ({1}) to {2} {3} ({4}) with {5} access",
                        mappingProject.ProjectName, mappingProject.MappingProjectId, user.FirstName, user.LastName, user.Id, mappingProjectUser.Role)
                });
            }
            else
            {
                if (Principal.Current.UserId == user.Id && !Principal.Current.IsAdministrator)
                    throw new SecurityException("Users cannot change their their on role on a project");

                _loggingService.Post(new LoggingCreateModel()
                {
                    Source = "Shared",
                    Message = string.Format("Changed Access from {0} to {1} for {2} {3} ({4}) on Mapping Project {5} ({6})",
                        mappingProjectUser.Role, model.Role, user.FirstName, user.LastName, user.Id, mappingProject.ProjectName, mappingProject.MappingProjectId)
                });
                mappingProjectUser.Role = model.Role;
            }

            //For Target Data Standard
            var target = _mappedSystemRepository.Get(mappingProject.TargetDataStandardMappedSystemId);
            var targetUser = target.Users.SingleOrDefault(o => o.UserId == user.Id);
            if (null == targetUser)
            {
                targetUser = new MappedSystemUser(target.MappedSystemId, user.Id, MappedSystemUser.MappedSystemUserRole.View);
                target.Users.Add(targetUser);
                _loggingService.Post(new LoggingCreateModel()
                {
                    Source = "Shared",
                    Message = string.Format("Shared Data Standard {0} ({1}) to {2} {3} ({4}) with {5} access",
                        target.SystemName, target.MappedSystemId, user.FirstName, user.LastName, user.Id, MappedSystemUser.MappedSystemUserRole.View)
                });
            }

            //For Source Data Standard
            var source = _mappedSystemRepository.Get(mappingProject.SourceDataStandardMappedSystemId);
            var sourceUser = source.Users.SingleOrDefault(o => o.UserId == user.Id);
            if (null == sourceUser)
            {
                sourceUser = new MappedSystemUser(source.MappedSystemId, user.Id, MappedSystemUser.MappedSystemUserRole.View);
                source.Users.Add(sourceUser);
                _loggingService.Post(new LoggingCreateModel()
                {
                    Source = "Shared",
                    Message = string.Format("Shared Data Standard {0} ({1}) to {2} {3} ({4}) with {5} access",
                        source.SystemName, source.MappedSystemId, user.FirstName, user.LastName, user.Id, MappedSystemUser.MappedSystemUserRole.View)
                });
            }
            _mappingProjectRepository.SaveChanges();

            var sharedFrom = _userRepository.FindById(Principal.Current.UserId);

            string html;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Templates/Email/ShareMappingProjectTemplate.html")))
                html = reader.ReadToEnd();

            string baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/');
            var callbackUrl = new Url(string.Format("{0}/#!/p/{1}", baseUrl, mappingProjectId)); //TODO Change after Mapping Project Route Refactor

            html = html.Replace("{{USER}}", user.FirstName + " " + user.LastName)
                .Replace("{{URL}}", callbackUrl.Value)
                .Replace("{{SHAREUSER}}", sharedFrom.FirstName + " " + sharedFrom.LastName)
                .Replace("{{ACCESS}}", model.Role.ToString().ToLower())
                .Replace("{{NAME}}", mappingProject.ProjectName);

            EmailService emailService = new EmailService();
            await emailService.SendEmail(Configuration.Email.From, user.Email, "MappingEDU - Shared Mapping Project", html);

            _loggingService.Post(new LoggingCreateModel()
            {
                Source = "Email",
                Message = string.Format("Mapping Project Share Email sent to {0} {1} ({2})", user.FirstName, user.LastName, user.Id)
            });

            return new MappingProjectUserModel()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id,
                Role = model.Role
            };
        }

        public MappingProjectUserModel GetMappingProjectUser(Guid mappingProjectId, Guid userId)
        {
            var mappingProject = GetMappingProject(mappingProjectId);

            if (!Principal.Current.IsAdministrator && Principal.Current.UserId != userId.ToString() && !mappingProject.HasAccess(MappingProjectUser.MappingProjectUserRole.Edit))
                throw new SecurityException("Don't have access to see other users access levels");

            var user = mappingProject.Users.SingleOrDefault(x => x.UserId == userId.ToString());

            if (user != null) return Mapper.Map<MappingProjectUserModel>(user);

            if (!mappingProject.IsPublic && !Principal.Current.IsAdministrator)
                throw new SecurityException("User does not have access to this mapping project");

            var currentUser = _userRepository.FindById(userId);
            if(currentUser == null)
                throw new NotFoundException("User doesn't exist");

            return new MappingProjectUserModel
            {
                FirstName = currentUser.FirstName,
                Email = currentUser.Email,
                LastName = currentUser.LastName,
                Id = userId.ToString(),
                Role = _userRepository.IsGuest(userId) ? MappingProjectUser.MappingProjectUserRole.Guest : (_userRepository.IsAdministrator(userId) ? MappingProjectUser.MappingProjectUserRole.Owner : MappingProjectUser.MappingProjectUserRole.View)
            };
        }

        public MappingProjectUserModel[] GetTaggableUsersForMappingProjet(Guid mappingProjectId)
        {
            var mappingProject = GetMappingProject(mappingProjectId, MappingProjectUser.MappingProjectUserRole.Edit);

            var adminRole = _userRepository.GetAvailableRoles().FirstOrDefault(x => x.Name == Constants.Permissions.Administrator);
            if(adminRole == null)
                throw new NotFoundException("Unable to Find Admin Role");
            
            var admins = _userRepository.GetAllUsers().Where(x => x.IsActive && x.Roles.Select(y => y.RoleId).Contains(adminRole.Id)).Select(x => new MappingProjectUserModel
            {
                Email = x.Email,
                FirstName = x.FirstName,
                Id = x.Id,
                LastName = x.LastName
            }).ToList();

            var users = mappingProject.Users.Where(x => x.Role > MappingProjectUser.MappingProjectUserRole.View).Select(x => new MappingProjectUserModel
            {
                Email = x.User.Email,
                FirstName = x.User.FirstName,
                Id = x.UserId,
                LastName = x.User.LastName
            }).ToList();

            users.AddRange(admins);

            return users.GroupBy(x => x.Id).Select(x => x.First()).ToArray();
        }

        public MappingProjectUserModel[] GetUsersForMappingProject(Guid mappingProjectId)
        {
            var mappingProject = GetMappingProject(mappingProjectId, MappingProjectUser.MappingProjectUserRole.Edit);
            return Mapper.Map<MappingProjectUserModel[]>(mappingProject.Users);
        }

        public void RemoveUserFromMappingProject(Guid mappingProjectId, string userId)
        {
            var mappingProject = GetMappingProject(mappingProjectId, MappingProjectUser.MappingProjectUserRole.Owner);

            if (Principal.Current.UserId == userId)
                throw new SecurityException("User cannot delete themselves from a project");

            var mappingUser = mappingProject.Users.SingleOrDefault(o => o.UserId == userId);
            if (mappingUser != null)
            {
                mappingProject.Users.Remove(mappingUser);
                _mappingProjectRepository.SaveChanges();

                var user = _userRepository.FindById(userId);

                _loggingService.Post(new LoggingCreateModel()
                {
                    Source = "Shared",
                    Message = string.Format("Removed access to Mapping Project {0} ({1}) for {2} {3} ({4})",
                        mappingProject.ProjectName, mappingProject.MappingProjectId, user.FirstName, user.LastName, user.Id)
                });
            }
        }

        public void ToggleFlagged(Guid mappingProjectId, Guid userId)
        {
            var mappingProject = GetMappingProject(mappingProjectId, MappingProjectUser.MappingProjectUserRole.View);

            var userFlag = mappingProject.FlaggedBy.SingleOrDefault(o => o.Id == userId.ToString());
            if (userFlag == null)
            {
                var user = _userRepository.FindById(userId.ToString());

                if (null == user)
                    throw new NotFoundException("Unable to find user with id: " + userId);
                mappingProject.FlaggedBy.Add(user);
            }
            else
                mappingProject.FlaggedBy.Remove(userFlag);

            _mappedSystemRepository.SaveChanges();
        }

        private MappingProject GetMappingProject(Guid mappingProjectId, MappingProjectUser.MappingProjectUserRole role = MappingProjectUser.MappingProjectUserRole.Guest)
        {
            var mappingProject = _mappingProjectRepository.Get(mappingProjectId);

            if (mappingProject == null)
                throw new Exception(string.Format("Mapping Project with id '{0}' does not exist.", mappingProjectId));

            if (!Principal.Current.IsAdministrator && !mappingProject.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));

            return mappingProject;
        }
    }
}
