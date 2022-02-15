// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using FakeItEasy;
using MappingEdu.Common;
using MappingEdu.Common.Configuration;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Service.Membership;
using MappingEdu.Service.Model.Membership;
using MappingEdu.Web.UI.Infrastructure.Helpers;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace MappingEdu.Tests.Unit.Web.UI.Tests.Infraestructure.Providers
{
    [TestFixture]
    public abstract class AuthorizationServerHelperTest
    {
        private AuthorizationServerHelper _authorizationServerHelper;
        private IIdentityFactory _identityFactoryMock;
        private IConfigurationStore _configurationStoreMock;
        private IUserService _userServiceMock;
        private const string FAKE_AUTH_TYPE = "fake_auth_type";
        private const string FAKE_RIGHT_AUTHENTICATED_USER = "user";
        private const string FAKE_WRONG_AUTHENTICATED_USER = "wrong_user";
        private string GUEST_USER = Constants.Security.GuestUsername;
        private string GUEST_PASSWORD = Constants.Security.GuestPassword;
        private const string FAKE_USER_ID = "FAKE_ID";

        protected abstract void AdditionalArrangement();

        [OneTimeSetUp]
        public void Arrange()
        {
            _authorizationServerHelper = new AuthorizationServerHelper();
            _identityFactoryMock = A.Fake<IIdentityFactory>();
            _configurationStoreMock = A.Fake<IConfigurationStore>();
            _userServiceMock = A.Fake<IUserService>();

            // Both guest and normal login are mocked in order to test if the right method call is being made.
            A.CallTo(() => _identityFactoryMock.CreateGuestIdentity(FAKE_AUTH_TYPE, FAKE_USER_ID))
                .Returns(new System.Security.Claims.ClaimsIdentity());

            // At this point, if the user is admin or not is not relevant for testing the AuthorizationServerHelper functionality
            A.CallTo(() => _identityFactoryMock.CreateIdentity(FAKE_AUTH_TYPE, FAKE_RIGHT_AUTHENTICATED_USER, FAKE_USER_ID, true))
                .Returns(new System.Security.Claims.ClaimsIdentity());

            A.CallTo(() => _userServiceMock.AuthenticateAsync(GUEST_USER, GUEST_PASSWORD))
                .Returns(new UserModel { Id = FAKE_USER_ID });

            A.CallTo(() => _userServiceMock.AuthenticateAsync(FAKE_RIGHT_AUTHENTICATED_USER, GUEST_PASSWORD))
                .Returns(
                new UserModel
                {
                    Id = FAKE_USER_ID,
                    UserName =
                    FAKE_RIGHT_AUTHENTICATED_USER,
                    IsAdministrator = true
                });

            UserModel wrong_return = null;
            A.CallTo(() => _userServiceMock.AuthenticateAsync(FAKE_WRONG_AUTHENTICATED_USER, GUEST_PASSWORD))
                .Returns(wrong_return);

            AdditionalArrangement();
        }

        [TestFixture]
        public class Given_Guest_Account : AuthorizationServerHelperTest
        {
            protected override void AdditionalArrangement()
            { }

            [SetUp]
            public async Task SetUp()
            {
                await _authorizationServerHelper.GenerateIdentity(
                    GUEST_USER,
                    GUEST_PASSWORD,
                    FAKE_AUTH_TYPE,
                    _identityFactoryMock,
                    _configurationStoreMock,
                    _userServiceMock
                    );
            }

            [Test]
            public void Create_Guest_Identity_Is_Called()
            {
                A.CallTo(
                    () => _identityFactoryMock.CreateGuestIdentity(FAKE_AUTH_TYPE, FAKE_USER_ID))
                    .MustHaveHappenedOnceExactly();
            }

        }

        [TestFixture]
        public class Given_Registered_Account : AuthorizationServerHelperTest
        {
            protected override void AdditionalArrangement()
            { }

            [SetUp]
            public async Task SetUp()
            {
                await _authorizationServerHelper.GenerateIdentity(
                    FAKE_RIGHT_AUTHENTICATED_USER,
                    GUEST_PASSWORD, //We are not testing
                    FAKE_AUTH_TYPE,
                    _identityFactoryMock,
                    _configurationStoreMock,
                    _userServiceMock
                    );
            }

            [Test]
            public void When_Credentials_Are_Correct_Create_Identity_Is_Called()
            {
                A.CallTo(
                    () => _identityFactoryMock.CreateIdentity(FAKE_AUTH_TYPE, FAKE_RIGHT_AUTHENTICATED_USER, FAKE_USER_ID, true))
                    .MustHaveHappened();
            }

            [Test]
            public void When_Credentials_Are_Wrong_An_Exception_Is_Thrown()
            {
                Assert.ThrowsAsync<InvalidOperationException>(async () => await _authorizationServerHelper.GenerateIdentity(
                        FAKE_WRONG_AUTHENTICATED_USER,
                        GUEST_PASSWORD,
                        FAKE_AUTH_TYPE,
                        _identityFactoryMock,
                        _configurationStoreMock,
                        _userServiceMock
                        ));
            }

        }
    }
}
