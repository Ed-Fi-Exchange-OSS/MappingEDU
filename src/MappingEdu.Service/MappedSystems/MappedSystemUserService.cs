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
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Email;
using MappingEdu.Service.Logging;
using MappingEdu.Service.Model.Logging;
using MappingEdu.Service.Model.MappedSystem;
using Constants = MappingEdu.Common.Constants;

namespace MappingEdu.Service.MappedSystems
{
    public interface IMappedSystemUserService
    {
        Task<MappedSystemUserModel> AddUserToMappedSystem(Guid mappedSystemId, MappedSystemUserAddModel model);

        MappedSystemUserModel GetMappedSystemUser(Guid mappedSystemId, Guid userId);

        MappedSystemUserModel[] GetTaggableUsersForMappedSystem(Guid mappedSystemId);

        MappedSystemUserModel[] GetUsersForMappedSystem(Guid mappedSystemId);

        void RemoveUserFromMappedSystem(Guid mappedSystemId, string userId);

        void ToggleFlagged(Guid mappedSystemId, Guid userId);
    }

    public class MappedSystemUserService : IMappedSystemUserService
    {
        private readonly ILoggingService _loggingService;
        private readonly IRepository<MappedSystem> _mappedSystemRepository;
        private readonly IUserRepository _userRepository;

        public MappedSystemUserService(IUserRepository userRepository, IRepository<MappedSystem> mappedSystemRepository, ILoggingService loggingService)
        {
            _loggingService = loggingService;
            _userRepository = userRepository;
            _mappedSystemRepository = mappedSystemRepository;
        }

        public async Task<MappedSystemUserModel> AddUserToMappedSystem(Guid mappedSystemId, MappedSystemUserAddModel model)
        {
            var mappedSystem = GetMappedSystem(mappedSystemId, MappedSystemUser.MappedSystemUserRole.Owner);
            var user = _userRepository.FindByEmail(model.Email);
            if (user == null)
                throw new NotFoundException("Unabled to find user with email: " + model.Email);

            var mappedSystemUser = mappedSystem.Users.SingleOrDefault(o => o.UserId == user.Id);
            if (mappedSystemUser == null)
            {
                mappedSystemUser = new MappedSystemUser(mappedSystemId, user.Id, model.Role);
                mappedSystem.Users.Add(mappedSystemUser);
                _loggingService.Post(new LoggingCreateModel
                {
                    Source = "Shared",
                    Message = string.Format("Shared Data Standard {0} ({1}) to {2} {3} ({4}) with {5} access",
                        mappedSystem.SystemName, mappedSystem.MappedSystemId, user.FirstName, user.LastName, user.Id, model.Role)
                });
            }
            else
            {
                if (Principal.Current.UserId == user.Id)
                    throw new SecurityException("Users cannot change their role on a project");

                _loggingService.Post(new LoggingCreateModel
                {
                    Source = "Shared",
                    Message = string.Format("Changed Access from {0} to {1} for {2} {3} ({4}) on Data Standard {5} ({6})",
                        mappedSystemUser.Role, model.Role, user.FirstName, user.LastName, user.Id, mappedSystem.SystemName, mappedSystem.MappedSystemId)
                });
                mappedSystemUser.Role = model.Role;
            }
            _mappedSystemRepository.SaveChanges();

            var sharedFrom = _userRepository.FindById(Principal.Current.UserId);

            string html;
            using (var reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Templates/Email/ShareDataStandardTemplate.html")))
                html = reader.ReadToEnd();

            var baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/');
            var callbackUrl = new Url(string.Format("{0}/#!/data-standard/detail/{1}", baseUrl, mappedSystemId));

            html = html.Replace("{{USER}}", user.FirstName + " " + user.LastName)
                .Replace("{{URL}}", callbackUrl.Value)
                .Replace("{{SHAREUSER}}", sharedFrom.FirstName + " " + sharedFrom.LastName)
                .Replace("{{ACCESS}}", model.Role.ToString().ToLower())
                .Replace("{{NAME}}", mappedSystem.SystemName);

            var emailService = new EmailService();
            await emailService.SendEmail(Configuration.Email.From, user.Email, "MappingEDU - Shared Data Standard", html);

            _loggingService.Post(new LoggingCreateModel
            {
                Source = "Email",
                Message = string.Format("Data Standard Share Email sent to {0} {1} ({2})", user.FirstName, user.LastName, user.Id)
            });

            return new MappedSystemUserModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id,
                Role = model.Role
            };
        }

        public MappedSystemUserModel GetMappedSystemUser(Guid mappedSystemId, Guid userId)
        {
            var mappedSystem = GetMappedSystem(mappedSystemId);

            if (!Principal.Current.IsAdministrator && Principal.Current.UserId != userId.ToString() && !mappedSystem.HasAccess(MappedSystemUser.MappedSystemUserRole.Edit))
                throw new SecurityException("User doesn't have access to see other users access levels");

            var user = mappedSystem.Users.SingleOrDefault(x => x.UserId == userId.ToString());

            if (user != null) return Mapper.Map<MappedSystemUserModel>(user);
        
            if (!mappedSystem.IsPublic && !Principal.Current.IsAdministrator)
                throw new SecurityException("User does not have access to this data standard");

            var currentUser = _userRepository.FindById(userId);
            if (currentUser == null)
                throw new NotFoundException("User doesn't exist");

            return new MappedSystemUserModel
            {
                FirstName = currentUser.FirstName,
                Email = currentUser.Email,
                LastName = currentUser.LastName,
                Id = userId.ToString(),
                Role = _userRepository.IsGuest(userId) ? MappedSystemUser.MappedSystemUserRole.Guest : (_userRepository.IsAdministrator(userId) ? MappedSystemUser.MappedSystemUserRole.Owner : MappedSystemUser.MappedSystemUserRole.View)
            };
        }

        public MappedSystemUserModel[] GetTaggableUsersForMappedSystem(Guid mappedSystemId)
        {
            var mappedSystem = GetMappedSystem(mappedSystemId, MappedSystemUser.MappedSystemUserRole.Edit);

            var adminRole = _userRepository.GetAvailableRoles().FirstOrDefault(x => x.Name == Constants.Permissions.Administrator);
            if (adminRole == null)
                throw new NotFoundException("Unable to Find Admin Role");

            var admins = _userRepository.GetAllUsers().Where(x => x.IsActive && x.Roles.Select(y => y.RoleId).Contains(adminRole.Id)).Select(x => new MappedSystemUserModel
            {
                Email = x.Email,
                FirstName = x.FirstName,
                Id = x.Id,
                LastName = x.LastName
            }).ToList();

            var users = mappedSystem.Users.Where(x => x.Role > MappedSystemUser.MappedSystemUserRole.View).Select(x => new MappedSystemUserModel
            {
                Email = x.User.Email,
                FirstName = x.User.FirstName,
                Id = x.UserId,
                LastName = x.User.LastName
            }).ToList();

            users.AddRange(admins);

            return users.GroupBy(x => x.Id).Select(x => x.First()).ToArray();
        }

        public MappedSystemUserModel[] GetUsersForMappedSystem(Guid mappedSystemId)
        {
            var mappedSystem = GetMappedSystem(mappedSystemId, MappedSystemUser.MappedSystemUserRole.Edit);
            return Mapper.Map<MappedSystemUserModel[]>(mappedSystem.Users);
        }

        public void RemoveUserFromMappedSystem(Guid mappedSystemId, string userId)
        {
            var mappedSystem = GetMappedSystem(mappedSystemId, MappedSystemUser.MappedSystemUserRole.Owner);

            if(Principal.Current.UserId == userId)
                throw new SecurityException("Users cannot delete themselves from a project");

            var mappedSystemUser = mappedSystem.Users.SingleOrDefault(o => o.UserId == userId);
            if (mappedSystemUser == null) return;

            var user = _userRepository.FindById(userId);
            var projects = user.Projects.Where(x => x.MappingProject.SourceDataStandard.Users.Any(y => x.MappingProject.IsActive && y.UserId == userId && y.MappedSystemId == mappedSystemId) ||
                                                    x.MappingProject.TargetDataStandard.Users.Any(y => x.MappingProject.IsActive && y.UserId == userId && y.MappedSystemId == mappedSystemId))
                .Select(x => x.MappingProject).ToList();

            if (projects.Any())
            {
                var message = projects.Aggregate("Need to remove user from the following projects first: ",
                    (current, project) => current + (project.ProjectName + ", "));

                throw new BusinessException(message.Substring(0, message.Length - 2));
            }

            mappedSystem.Users.Remove(mappedSystemUser);
            _mappedSystemRepository.SaveChanges();

            _loggingService.Post(new LoggingCreateModel
            {
                Source = "Shared",
                Message = string.Format("Removed access to Data Standard {0} ({1}) for {2} {3} ({4})",
                    mappedSystem.SystemName, mappedSystem.MappedSystemId, user.FirstName, user.LastName, user.Id)
            });
        }

        public void ToggleFlagged(Guid mappedSystemId, Guid userId)
        {
            var mappedSystem = GetMappedSystem(mappedSystemId);
            if(Principal.Current.IsGuest)
                throw new SecurityException("Guests cannot star standards");

            var userFlag = mappedSystem.FlaggedBy.SingleOrDefault(o => o.Id == userId.ToString());
            if (userFlag == null)
            {
                var user = _userRepository.FindById(userId);

                if (null == user)
                    throw new NotFoundException("Unabled to find user with id: " + userId);
                mappedSystem.FlaggedBy.Add(user);
            }
            else
                mappedSystem.FlaggedBy.Remove(userFlag);

            _mappedSystemRepository.SaveChanges();
        }

        private MappedSystem GetMappedSystem(Guid mappedSystemId, MappedSystemUser.MappedSystemUserRole role = MappedSystemUser.MappedSystemUserRole.Guest)
        {
            var mappedSystem = _mappedSystemRepository.Get(mappedSystemId);

            if (mappedSystem == null)
                throw new Exception(string.Format("Mapped System with id '{0}' does not exist.", mappedSystemId));

            if (!Principal.Current.IsAdministrator && !mappedSystem.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));

            if (!mappedSystem.IsActive)
                throw new Exception(string.Format("Mapped System with id '{0}' is marked as deleted.", mappedSystemId));

            return mappedSystem;
        }
    }
}
