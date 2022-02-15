// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using FakeItEasy;
using MappingEdu.Common;
using MappingEdu.Common.Configuration;
using MappingEdu.Core.Domain.Security;
using NUnit.Framework;
using System;
using System.Linq;
using System.Security.Claims;

namespace MappingEdu.Tests.Core.Domain.Tests.Security
{
    [TestFixture]
    public abstract class IdentityFactoryTest
    {
        IConfigurationStore _mockConfigurationStore;
        IIdentityFactory _identityFactory;
        private ClaimsIdentity identity;
        private const string FAKE_AUTH_TYPE = "fake_auth_type";
        private const string FAKE_ID = "fake_id";
        private const string FAKE_NAME = "fake_name";

        protected abstract void AdditionalArrangement();

        [OneTimeSetUp]
        public void Arrange()
        {
            _mockConfigurationStore = A.Fake<IConfigurationStore>();
            _identityFactory = new IdentityFactory();
            AdditionalArrangement();
        }

        [TestFixture]
        public abstract class When_creating_guest_identity : IdentityFactoryTest
        {

            [TestFixture]
            public class Common_claims_are_present : When_creating_guest_identity
            {
                protected override void AdditionalArrangement()
                {
                    identity = _identityFactory.CreateGuestIdentity(FAKE_AUTH_TYPE, FAKE_ID);
                }
                [Test]
                public void Then_Issued_Claim_Is_Present()
                {
                    var issuedClaimIsPresent = identity.Claims
                            .Any(claim => claim.Type == IdentityFactory.IssuedKey);

                    Assert.IsTrue(issuedClaimIsPresent);
                }

                [Test]
                public void Then_UserID_Claim_Is_Present()
                {
                    var userIdClaimIsPresent = identity.Claims
                            .Any(claim => claim.Type == IdentityFactory.UserIdKey);

                    Assert.IsTrue(userIdClaimIsPresent);
                }

                [Test]
                public void Then_Name_Claim_Is_Present()
                {
                    var userIdClaimIsPresent = identity.Claims
                            .Any(claim => claim.Type == ClaimTypes.Name);

                    Assert.IsTrue(userIdClaimIsPresent);
                }
            }
        }

        [TestFixture]
        public abstract class When_creating_authenticated_identity : IdentityFactoryTest
        {
            protected override void AdditionalArrangement()
            {
                identity = _identityFactory.CreateIdentity(FAKE_AUTH_TYPE, FAKE_NAME, FAKE_ID);
            }

            [TestFixture]
            public class Common_claims_are_present : When_creating_authenticated_identity
            {
                [Test]
                public void Then_Issued_Claim_Is_Present()
                {
                    var issuedClaimIsPresent = identity.Claims
                            .Any(claim => claim.Type == IdentityFactory.IssuedKey);

                    Assert.IsTrue(issuedClaimIsPresent);
                }

                [Test]
                public void Then_UserID_Claim_Is_Present()
                {
                    var userIdClaimIsPresent = identity.Claims
                            .Any(claim => claim.Type == IdentityFactory.UserIdKey);

                    Assert.IsTrue(userIdClaimIsPresent);
                }

                [Test]
                public void Then_Name_Claim_Is_Present()
                {
                    var userIdClaimIsPresent = identity.Claims
                            .Any(claim => claim.Type == ClaimTypes.Name);

                    Assert.IsTrue(userIdClaimIsPresent);
                }

                [Test]
                public void Then_user_role_is_assigned()
                {
                    var hasUserRole = identity.Claims
                        .Any(claim => (claim.Type == ClaimTypes.Role && claim.Value == Constants.Permissions.User));
                    Assert.IsTrue(hasUserRole);
                }
            }

            [TestFixture]
            public abstract class Given_Admin_User : When_creating_authenticated_identity
            {
                [SetUp]
                public void SetUp()
                {
                    identity = _identityFactory.CreateIdentity(FAKE_AUTH_TYPE, FAKE_NAME, FAKE_ID, isAdministrator: true);
                }

                [Test]
                public void Then_admin_role_is_assigned()
                {
                    var hasAdminRole = identity.Claims
                        .Any(claim => (claim.Type == ClaimTypes.Role && claim.Value == Constants.Permissions.User));
                    Assert.IsTrue(hasAdminRole);

                }
            }
        }
    }
}
