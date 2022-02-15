// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using MappingEdu.Common.Configuration;
using MappingEdu.Common.Exceptions;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.DataAccess.Util;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Service.Email;
using MappingEdu.Service.Logging;
using MappingEdu.Service.Model.Datatables;
using MappingEdu.Service.Model.Logging;
using MappingEdu.Service.Model.Membership;
using MappingEdu.Service.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Constants = MappingEdu.Common.Constants;

namespace MappingEdu.Service.Membership
{
    public interface IUserService
    {
        Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login);

        Task<UserModel> AuthenticateAsync(UserLoginInfo loginInfo);

        Task<UserModel> AuthenticateAsync(string userName, string password);

        Task<UserModel> CheckExistsByEmail(string email);

        Task ConfirmEmailAsync(string id, string code, UserPasswordResetModel model);

        Task<IdentityResult> CreateAsync(UserCreateModel model);

        Task<IdentityResult> DeleteAsync(string id);

        Task<ICollection<UserModel>> FindAllUserByEmailAsync(string email);

        Task<UserModel> FindUserByIdAsync(string id);

        Task<ICollection<UserOrganizationModel>> GetAllUserOrganizations(string userId);

        Task<ICollection<UserMappingProjectModel>> GetAllUserProjects(string userId);

        Task<ICollection<UserModel>> GetAllUsers();

        Task<PagedResult<UserModel>> GetAllUsersPaging(DatatablesModel mode);

        Task<ICollection<UserMappedSystemModel>> GetAllUserStandards(string userId);

        Task<CurrentUserModel> GetCurrentUserAsync();

        Task<bool> IsGuestAccountActive();

        Task RegisterUserAsync(UserCreateModel userModel);

        Task ResendActivationEmail(string userId);

        Task ResetPassword(string id, string token, UserPasswordResetModel model);

        Task SendResetPasswordEmail(string email);

        Task<IdentityResult> UpdateAsync(string id, UserUpdateModel model);

        Task<IdentityResult> UpdateCurrentAsync(CurrentUserUpdateModel model);

        Task<IdentityResult> ToggleActive(string id);
    }

    public class UserService : IUserService, IDisposable
    {
        private readonly ILoggingService _loggingService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfigurationStore _configurationStore;

        public UserService(EntityContext context, ILoggingService loggingService, IConfigurationStore configurationStore)
        {
            _loggingService = loggingService;
            _configurationStore = configurationStore;

            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            UserValidator<ApplicationUser> validator = new UserValidator<ApplicationUser>(_userManager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            _userManager.UserValidator = validator;
            var provider = DataProtectionProvider._dataProtectionProvider;
            _userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(
                provider.Create("ApplicationToken"));
        }

        public void Dispose()
        {
            if (_userManager != null)
                _userManager.Dispose();
        }

        public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            return await _userManager.AddLoginAsync(userId, login);
        }

        public async Task<UserModel> AuthenticateAsync(string userName, string password)
        {
            var user = await _userManager.FindAsync(userName, password);

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(userName);
                if (user == null)
                    throw new NotFoundException("The user name or password you have entered is invalid.");

                user = await _userManager.FindAsync(user.UserName, password);
                if (user == null)
                    throw new NotFoundException("The user name or password you have entered is invalid.");
            }

            if(!user.IsActive)
                throw new BusinessException("Account has been deactivated.");

            if (!user.EmailConfirmed)
                throw new BusinessException("Account has not been activated");

            return await MapUserWithRolesAsync(user);
        }

        public async Task<UserModel> AuthenticateAsync(UserLoginInfo loginInfo)
        {
            var user = await _userManager.FindAsync(loginInfo);

            if (user == null)
                throw new NotFoundException("The user name or password you have entered is invalid.");
            if (!user.EmailConfirmed)
                throw new BusinessException("Account has not been activated");

            return await MapUserWithRolesAsync(user);
        }

        public async Task<UserModel> CheckExistsByEmail(string email)
        {
            CantBeGuest();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || email == "guest")
                return null;

            return await MapUserWithRolesAsync(user);
        }

        public async Task ConfirmEmailAsync(string userId, string token, UserPasswordResetModel model)
        {
            try
            {
                var tokenBytes = HttpServerUtility.UrlTokenDecode(token);
                if (tokenBytes != null) token = Encoding.UTF8.GetString(tokenBytes);
                else
                    throw new BusinessException("UserId or Token is Invalid");
            }
            catch (FormatException fex)
            {
                throw new BusinessException("UserId or Token is Invalid", fex);
            }

            var result = await _userManager.ConfirmEmailAsync(userId, token);
            if (!result.Succeeded)
                throw new BusinessException(result.Errors.SingleOrDefault());

            await _userManager.AddPasswordAsync(userId, model.Password);
        }

        public async Task<IdentityResult> CreateAsync(UserCreateModel model)
        {
            MustBeAdmin();
            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.UserName
            };

            return await _userManager.CreateAsync(user, model.Password);
        }

        public async Task<IdentityResult> DeleteAsync(string id)
        {
            MustBeAdmin();
            var user = await _userManager.FindByIdAsync(id);

            if (null == user)
                throw new NotFoundException("Unable to find user with id: " + id);

            if (user.Projects.Count > 0 || user.MappedSystems.Count > 0)
                throw new BusinessException("Cannot delete a user that is shared on a Mapping Project or Data Standard.");

            var roles = await _userManager.GetRolesAsync(id);

            if(roles.Contains("guest"))
                throw new BusinessException("Cannot delete a guest account.");

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                throw new BusinessException(result.Errors.FirstOrDefault());

            _loggingService.Post(new LoggingCreateModel
            {
                Source = "Delete",
                Message = string.Format("Deleted User {0} {1} ({2})", user.FirstName, user.LastName, user.Id)
            });

            return null;
        }

        public async Task<ICollection<UserModel>> FindAllUserByEmailAsync(string email)
        {
            MustBeAdmin();
            var users = await _userManager.Users.Where(o => o.Email == email).ToListAsync(); //TODO: Implement paging (cpt)
            var userModels = new Collection<UserModel>();
            foreach (var user in users)
            {
                var userModel = await MapUserWithRolesAsync(user);
                userModels.Add(userModel);
            }
            return userModels;
        }

        public async Task<UserModel> FindUserByIdAsync(string id)
        {
            if(Principal.Current.UserId != id) MustBeAdmin();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new NotFoundException("Unable to find user with id: " + id);
            return await MapUserWithRolesAsync(user);
        }

        public async Task<ICollection<UserOrganizationModel>> GetAllUserOrganizations(string userId)
        {
            if (Principal.Current.UserId != userId) MustBeAdmin();
            var user = await _userManager.FindByIdAsync(userId);
            if (null == user)
                throw new NotFoundException("Unable to find user with id: " + userId);

            return Mapper.Map<UserOrganizationModel[]>(user.Organizations);
        }

        public async Task<ICollection<UserMappingProjectModel>> GetAllUserProjects(string userId)
        {
            if (Principal.Current.UserId != userId) MustBeAdmin();
            var user = await _userManager.FindByIdAsync(userId);
            if (null == user)
                throw new NotFoundException("Unable to find user with id: " + userId);

            return Mapper.Map<UserMappingProjectModel[]>(user.Projects.Where(x => x.MappingProject.IsActive));
        }

        public async Task<ICollection<UserModel>> GetAllUsers()
        {
            MustBeAdmin();
            var users = await _userManager.Users.ToListAsync(); //TODO: Implement paging (cpt)
            var userModels = new Collection<UserModel>();
            foreach (var user in users)
            {
                var userModel = await MapUserWithRolesAsync(user);
                userModels.Add(userModel);
            }
            return userModels;
        }

        public async Task<PagedResult<UserModel>> GetAllUsersPaging(DatatablesModel model)
        {
            MustBeAdmin();

            var users = _userManager.Users;
            var roles = await _roleManager.Roles.ToListAsync();
            var total = await users.CountAsync();

            var adminRole = roles.First(o => o.Name == Constants.Permissions.Administrator).Id;
            var guestRole = roles.First(o => o.Name == Constants.Permissions.Guest).Id;

            if (model.search != null && model.search.value != null)
            {
                users = users.Where(x => 
                (x.FirstName + " " + x.LastName).Contains(model.search.value) ||
                (x.Email).Contains(model.search.value) || 
                (x.Organizations.Count > 0 ? x.Organizations.FirstOrDefault().Name : "").Contains(model.search.value) ||
                (x.Roles.Count > 0 ? (x.Roles.Any(y => y.RoleId == adminRole)) ? "Admin" : (x.Roles.Any(y => y.RoleId == guestRole) ? "Guest" : "User") : "User").Contains(model.search.value));
            }

            var filtered = await users.CountAsync();

            switch (model.order[0].column)
            {
                case 0: users = model.order[0].dir == "asc" ? 
                        users.OrderBy(x => x.FirstName + " " + x.LastName) : 
                        users.OrderByDescending(x => x.FirstName + " " + x.LastName);
                    break;

                case 1: users = model.order[0].dir == "asc" ?
                        users.OrderBy(x => x.Email) :
                        users.OrderByDescending(x => x.Email);
                    break;

                case 2: users = model.order[0].dir == "asc" ?
                        users.OrderBy(x => x.Organizations.Count > 0 ? x.Organizations.FirstOrDefault().Name : "") :
                        users.OrderByDescending(x => x.Organizations.Count > 0 ? x.Organizations.FirstOrDefault().Name : "");
                    break;

                case 3: users = model.order[0].dir == "asc" ?
                        users.OrderBy(x => x.Roles.Count > 0 ? (x.Roles.Any(y => y.RoleId == adminRole)) ? "Admin" : (x.Roles.Any(y => y.RoleId == guestRole) ? "Guest" : "User") : "User").ThenBy(x => x.FirstName + " " + x.LastName) :
                        users.OrderByDescending(x => x.Roles.Count > 0 ? (x.Roles.Any(y => y.RoleId == adminRole)) ? "Admin" : (x.Roles.Any(y => y.RoleId == guestRole) ? "Guest" : "User") : "User").ThenBy(x => x.FirstName + " " + x.LastName);
                    break;

                default: users = model.order[0].dir == "asc" ? 
                         users.OrderBy(x => x.FirstName + " " + x.LastName) : 
                         users.OrderByDescending(x => x.FirstName + " " + x.LastName);
                    break;
            }

            users = users.Skip(model.start).Take(model.length);

            var userModels = new Collection<UserModel>();
            foreach (var user in users)
            {
                var userModel = await MapUserWithRolesAsync(user);
                userModels.Add(userModel);
            }

            return new PagedResult<UserModel>()
            {
                Items = userModels,
                TotalFiltered = filtered,
                TotalRecords = total
            };
        }

        public async Task<ICollection<UserMappedSystemModel>> GetAllUserStandards(string userId)
        {
            if (Principal.Current.UserId != userId) MustBeAdmin();
            var user = await _userManager.FindByIdAsync(userId);
            if (null == user)
                throw new NotFoundException("Unable to find user with id: " + userId);

            return Mapper.Map<UserMappedSystemModel[]>(user.MappedSystems.Where(x => x.MappedSystem.IsActive));
        }

        public async Task<CurrentUserModel> GetCurrentUserAsync()
        {
            var principal = Principal.Current;
            if (null == principal)
                throw new SecurityException("Unauthorized");

            var user = await _userManager.FindByIdAsync(principal.UserId);
            if (user == null)
                throw new NotFoundException("Unable to find current user");

            return new CurrentUserModel(await MapUserWithRolesAsync(user))
            {
                MappedSystems = await GetAllUserStandards(user.Id),
                Projects = await GetAllUserProjects(user.Id)
            };
        }

        public async Task RegisterUserAsync(UserCreateModel model)
        {
            MustBeAdmin();
            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                throw new Exception(result.Errors.ToList()[0]);

            var roles = new List<string> {Constants.Permissions.User};
            if (model.IsAdministrator)
                roles.Add(Constants.Permissions.Administrator);

            await _userManager.AddToRolesAsync(user.Id, roles.ToArray());

            await SendActivationEmailAsync(user);
        }

        public async Task<bool> IsGuestAccountActive()
        {
            var user = await _userManager.FindByEmailAsync("guest@example.com");

            return user != null && user.IsActive;
        }

        public async Task ResendActivationEmail(string userId)
        {
            MustBeAdmin();
            var user = await _userManager.FindByIdAsync(userId);
            if (user.EmailConfirmed)
                throw new BusinessException("User Account has been activated");

            await SendActivationEmailAsync(user);
        }

        public async Task ResetPassword(string userId, string token, UserPasswordResetModel model)
        {
            var tokenBytes = HttpServerUtility.UrlTokenDecode(token);
            if (tokenBytes == null)
                throw new BusinessException("Invalid UserId or Token");

            token = Encoding.UTF8.GetString(tokenBytes);
            var result = await _userManager.ResetPasswordAsync(userId, token, model.Password);

            if (!result.Succeeded)
                throw new BusinessException(result.Errors.SingleOrDefault());
        }

        public async Task SendResetPasswordEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new NotFoundException("Unable to find user with email: " + email);

            if(user.UserName == "guest")
                throw new BusinessException("Cannot reset guest password");

            string html;
            using (var reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Templates/Email/ResetPasswordTemplate.html")))
                html = reader.ReadToEnd();

            var code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
            var codeBytes = Encoding.UTF8.GetBytes(code);
            code = HttpServerUtility.UrlTokenEncode(codeBytes);
            var baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/');
            var callbackUrl = new Url(string.Format("{0}/#!/login/reset-password/{1}?code={2}", baseUrl, user.Id, code));

            html = html.Replace("{{USER}}", user.FirstName + " " + user.LastName)
                .Replace("{{URL}}", callbackUrl.Value);

            var emailService = new EmailService();
            await emailService.SendEmail(Configuration.Email.From, user.Email, "MappingEDU - Reset Password", html);

            _loggingService.Post(new LoggingCreateModel
            {
                Source = "Email",
                Message = string.Format("Reset Password Email sent to {0} {1} ({2})", user.FirstName, user.LastName, user.Id)
            });
        }

        public async Task<IdentityResult> UpdateAsync(string id, UserUpdateModel model)
        {
            MustBeAdmin();
            var user = await _userManager.FindByIdAsync(id);
            if (null == user)
                throw new NotFoundException("Unable to find user with id: " + id);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.UserName = model.UserName;
            user.Email = model.Email;

            if (model.IsAdministrator)
                await _userManager.AddToRoleAsync(id, Constants.Permissions.Administrator);
            else
                await _userManager.RemoveFromRoleAsync(id, Constants.Permissions.Administrator);

            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> UpdateCurrentAsync(CurrentUserUpdateModel model)
        {
            var user = await _userManager.FindByIdAsync(Principal.Current.UserId);

            if (null == user)
                throw new NotFoundException("Unable to find user with id: " + Principal.Current.UserId);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> ToggleActive(string id)
        {
            MustBeAdmin();
            var user = await _userManager.FindByIdAsync(id);
            if (null == user)
                throw new NotFoundException("Unable to find user with id: " + id);

            user.IsActive = !user.IsActive;

            return await _userManager.UpdateAsync(user);
          
        }

        private static void MustBeAdmin()
        {
            if(!Principal.Current.IsAdministrator)
                throw new SecurityException("Only an admin can edit a user");
        }

        private static void CantBeGuest()
        {
            if (Principal.Current.IsGuest)
                throw new SecurityException("Only an user has access");
        }

        private async Task<UserModel> MapUserWithRolesAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user.Id);

            var model = new UserModel
            {
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                FirstName = user.FirstName,
                Id = user.Id,
                IsActive = user.IsActive, 
                IsAdministrator = roles.Any(o => o == Constants.Permissions.Administrator),
                IsGuest = roles.Any(o => o == Constants.Permissions.Guest),
                LastName = user.LastName,
                Roles = roles.ToArray(),
                UserName = user.UserName
            };

            return model;
        }

        private IList<string> MockTestRoles()
        {
            return new string[] {
                Constants.Permissions.Administrator,
                Constants.Permissions.User
            };
        }

        private async Task SendActivationEmailAsync(ApplicationUser user)
        {
            string html;
            using (var reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Templates/Email/NewUserTemplate.html")))
                html = reader.ReadToEnd();

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);
            var codeBytes = Encoding.UTF8.GetBytes(code);
            code = HttpServerUtility.UrlTokenEncode(codeBytes);
            var baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/');
            var callbackUrl = new Url(string.Format("{0}/#!/login/confirm-email/{1}?code={2}", baseUrl, user.Id, code));

            html = html.Replace("{{USER}}", user.FirstName + " " + user.LastName)
                       .Replace("{{URL}}", callbackUrl.Value)
                       .Replace("{{HTTP-HOMEPAGE}}", baseUrl)
                       .Replace("{{HOMEPAGE}}", HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/'));

            var emailService = new EmailService();
            await emailService.SendEmail(Configuration.Email.From, user.Email, "MappingEDU - New Account", html);

            _loggingService.Post(new LoggingCreateModel
            {
                Source = "Email",
                Message = string.Format("Activation Email sent to {0} {1} ({2})", user.FirstName, user.LastName, user.Id)
            });
        }
    }
}
