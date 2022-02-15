// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Security.Claims;
using System.Threading;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.Services;
using Rhino.Mocks;

namespace MappingEdu.Tests.Business.Bases
{
    public abstract class TestBase
    {
        protected IIdentityFactory IdentityFactory;
        public TestBase()
        {
            IdentityFactory = new IdentityFactory();
        }
        protected T GenerateStub<T>() where T : class
        {
            Thread.CurrentPrincipal = new ClaimsPrincipal(IdentityFactory.CreateIdentity("UNIT_TESTS", "username", "0", true));
            return MockRepository.GenerateStub<T>();
        }

        protected void InitSystemClock(DateTime now)
        {
            SystemClock.Now = () => now;
        }
    }
}