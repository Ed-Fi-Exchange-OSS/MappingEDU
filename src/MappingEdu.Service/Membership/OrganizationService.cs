// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Security;
using AutoMapper;
using MappingEdu.Common.Exceptions;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Logging;
using MappingEdu.Service.Model.Logging;
using MappingEdu.Service.Model.Membership;

namespace MappingEdu.Service.Membership
{
    public interface IOrganizationService
    {
        void AddOrganizationUser(Guid organizationId, OrganizationUserCreateModel model);

        OrganizationModel CreateOrganization(OrganizationCreateModel model);

        void DeleteOrganization(Guid organizationId);

        void DeleteOrganizationUser(Guid organizationId, string userId);

        OrganizationModel FindOrganizationById(Guid organizationId);

        OrganizationModel[] GetAllOrganizations();

        OrganizationUserModel[] GetAllOrganizationUsers(Guid organizationId);

        void UpdateOrganization(Guid id, OrganizationUpdateModel model);
    }

    public class OrganizationService : IOrganizationService
    {
        private readonly ILoggingService _loggingService;
        private readonly IRepository<Organization> _organizationRepository;
        private readonly IUserRepository _userRepository;

        public OrganizationService(IRepository<Organization> organizationRepository, ILoggingService loggingService, IUserRepository userRepository)
        {
            _loggingService = loggingService;
            _organizationRepository = organizationRepository;
            _userRepository = userRepository;
        }

        public void AddOrganizationUser(Guid organizationId, OrganizationUserCreateModel model)
        {
            IsAdmin();
            var organization = _organizationRepository.Get(organizationId);
            if (null == organization)
                throw new NotFoundException("Unable to find organization with id: " + organizationId);

            var user = _userRepository.FindById(model.UserId);
            if (user == null)
                throw new NotFoundException("Unable to find user with id: " + model.UserId);

            if (organization.Users.All(o => o.Id != model.UserId))
                organization.Users.Add(user);

            _organizationRepository.SaveChanges();
        }

        public OrganizationModel CreateOrganization(OrganizationCreateModel model)
        {
            IsAdmin();
            var organization = new Organization(model.Name)
            {
                Description = model.Description,
                Domains = string.Join(Configuration.Membership.OrganizationDomainDelimiter.ToString(), model.Domains)
            };
            _organizationRepository.Add(organization);

            _organizationRepository.SaveChanges();

            return Mapper.Map<OrganizationModel>(organization);
        }

        public void DeleteOrganization(Guid organizationId)
        {
            IsAdmin();
            var organization = _organizationRepository.Get(organizationId);
            _organizationRepository.Delete(organizationId);
            _organizationRepository.SaveChanges();

            _loggingService.Post(new LoggingCreateModel()
            {
                Source = "Delete",
                Message = string.Format("Deleted Organization {0} ({1})", organization.Name, organization.OrganizationId)
            });
        }

        public void DeleteOrganizationUser(Guid organizationId, string userId)
        {
            IsAdmin();
            var organization = _organizationRepository.Get(organizationId);
            if (null == organization)
                throw new NotFoundException("Unable to find organization with id: " + organizationId);

            var user = organization.Users.FirstOrDefault(o => o.Id == userId);
            if (user != null)
            {
                organization.Users.Remove(user);
                _organizationRepository.SaveChanges();
            }
        }

        public OrganizationModel FindOrganizationById(Guid organizationId)
        {
            IsAdmin();
            var organization = _organizationRepository.Get(organizationId);
            if (null == organization)
                throw new NotFoundException("Unable to find organization with id: " + organizationId);

            return Mapper.Map<OrganizationModel>(organization);
        }

        public OrganizationModel[] GetAllOrganizations()
        {
            IsAdmin();
            var organizations = _organizationRepository.GetAll();
            return Mapper.Map<OrganizationModel[]>(organizations);
        }

        public OrganizationUserModel[] GetAllOrganizationUsers(Guid organizationId)
        {
            IsAdmin();
            var organization = _organizationRepository.Get(organizationId);
            if (null == organization)
                throw new NotFoundException("Unable to find organization with id: " + organizationId);

            return Mapper.Map<OrganizationUserModel[]>(organization.Users);
        }

        public void UpdateOrganization(Guid id, OrganizationUpdateModel model)
        {
            IsAdmin();
            var organization = _organizationRepository.Get(id);
            if (null == organization)
                throw new NotFoundException("Unable to find organization with id: " + id);

            organization.Name = model.Name;
            organization.Description = model.Description;
            organization.Domains = string.Join(Configuration.Membership.OrganizationDomainDelimiter.ToString(), model.Domains);

            _organizationRepository.SaveChanges();
        }

        private void IsAdmin()
        {
            if(!Principal.Current.IsAdministrator)
                throw new SecurityException("Only an admin has access to edit organizations");
        }
    }
}