// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.MappedSystems;
using MappingEdu.Service.Model.DataStandard;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Should;

namespace MappingEdu.Tests.Business.Web.Services
{
    public class NextDataStandardServiceTests
    {
        [TestFixture]
        public class When_getting_next_mapped_system : TestBase
        {
            private readonly Guid _mappedSystemId1 = Guid.NewGuid();
            private readonly Guid _mappedSystemId2 = Guid.NewGuid();
            private readonly DataStandardViewModel _expected = new DataStandardViewModel();
            private DataStandardViewModel _result;

            [OneTimeSetUp]
            public void Setup()
            {
                MappedSystem system = New.MappedSystem.WithId(_mappedSystemId1).IsActive();
                MappedSystem nextSystem = New.MappedSystem.WithId(_mappedSystemId2).WithPreviousVersion(system).IsActive();
                var repository = GenerateStub<IRepository<MappedSystem>>();
                var mapper = GenerateStub<IMapper>();

                repository.Expect(x => x.GetAll()).Return(new[] {system, nextSystem});
                mapper.Stub(x => x.Map<DataStandardViewModel>(nextSystem)).Return(_expected);

                INextDataStandardService service = new NextDataStandardService(repository, mapper);
                _result = service.Get(_mappedSystemId1);
            }

            [Test]
            public void Should_populate_view_model()
            {
                _result.ShouldEqual(_expected);
            }
        }

        [TestFixture]
        public class When_getting_next_mapped_system_that_does_not_exist : TestBase
        {
            private DataStandardViewModel _result;
            private readonly Guid _invalidMappedSystemId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var repository = GenerateStub<IRepository<MappedSystem>>();
                repository.Stub(x => x.GetAll()).Return(new MappedSystem[] {});
                var mapper = GenerateStub<IMapper>();

                INextDataStandardService service = new NextDataStandardService(repository, mapper);
                _result = service.Get(_invalidMappedSystemId);
            }

            [Test]
            public void Should_return_null()
            {
                _result.ShouldBeNull();
                ;
            }
        }

        [TestFixture]
        public class When_getting_next_mapped_system_that_has_been_marked_as_deleted : TestBase
        {
            private Exception _exception;
            private bool _exceptionThrown;
            private readonly Guid _mappedSystemId1 = Guid.NewGuid();
            private readonly Guid _mappedSystemId2 = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                MappedSystem system = New.MappedSystem.WithId(_mappedSystemId1).IsActive();
                MappedSystem nextSystem = New.MappedSystem.WithId(_mappedSystemId2).WithPreviousVersion(system);
                var repository = GenerateStub<IRepository<MappedSystem>>();
                var mapper = GenerateStub<IMapper>();

                repository.Stub(x => x.GetAll()).Return(new[] {system, nextSystem});

                INextDataStandardService service = new NextDataStandardService(repository, null);

                try
                {
                    service.Get(_mappedSystemId1);
                }
                catch (Exception ex)
                {
                    _exceptionThrown = true;
                    _exception = ex;
                }
            }

            [Test]
            public void Should_throw_exception()
            {
                _exceptionThrown.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }
    }
}
