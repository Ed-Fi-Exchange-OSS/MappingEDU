// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using MappingEdu.Core.DataAccess.Repositories;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Mapping;
using MappingEdu.Service.MappedSystems;
using MappingEdu.Service.Model.DataStandard;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Should;

namespace MappingEdu.Tests.Business.Web.Services
{
    public class DataStandardServiceTests
    {
        [TestFixture]
        public class When_getting_all_mapped_systems : TestBase
        {
            private DataStandardViewModel[] _result;

            [OneTimeSetUp]
            public void Setup()
            {
                MappedSystem system1 = New.MappedSystem.IsActive();
                MappedSystem system2 = New.MappedSystem.IsActive();

                var systems = new[]
                {
                    system1,
                    system2
                };

                var repository = GenerateStub<IMappedSystemRepository>();
                var mapper = GenerateStub<IMapper>();

                // stub out the get all method
                repository.Expect(x => x.GetAll()).Return(systems);

                // stub out the map method
                mapper
                    .Stub(x => x.Map<DataStandardViewModel[]>(systems))
                    .Return(new[] {new DataStandardViewModel(), new DataStandardViewModel()});

                IDataStandardService service = new DataStandardService(repository, mapper, null, null, null);
                _result = service.Get();
            }

            [Test]
            public void Should_populate_view_model()
            {
                _result.ShouldNotBeNull();
                _result.Length.ShouldEqual(2);
            }
        }

        [TestFixture]
        public class When_getting_specific_mapped_system : TestBase
        {
            private readonly Guid _mappedSystemId1 = Guid.NewGuid();
            private readonly DataStandardViewModel _expected = new DataStandardViewModel();
            private DataStandardViewModel _result;

            [OneTimeSetUp]
            public void Setup()
            {
                MappedSystem system = New.MappedSystem.WithId(_mappedSystemId1).IsActive();

                var repository = GenerateStub<IMappedSystemRepository>();
                var mapper = GenerateStub<IMapper>();

                repository.Expect(x => x.Get(_mappedSystemId1)).Return(system);
                repository.Stub(x => x.GetAllQueryable()).Return(new List<MappedSystem>().AsQueryable());
                mapper.Stub(x => x.Map<DataStandardViewModel>(system)).Return(_expected);

                IDataStandardService service = new DataStandardService(repository, mapper, null, null, null);
                _result = service.Get(_mappedSystemId1);
            }

            [Test]
            public void Should_populate_view_model()
            {
                _result.ShouldNotBeNull();
                _result.ShouldEqual(_expected);
            }
        }

        [TestFixture]
        public class When_getting_mapped_system_that_does_not_exist : TestBase
        {
            private Exception _exception;
            private bool _exceptionThrown;
            private readonly Guid _invalidMappedSystemId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var repository = GenerateStub<IMappedSystemRepository>();
                repository.Stub(x => x.Get(_invalidMappedSystemId)).Return(null);

                IDataStandardService service = new DataStandardService(repository, null, null, null, null);
                try
                {
                    service.Get(_invalidMappedSystemId);
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

            [Test]
            public void Should_throw_exception_with_good_message()
            {
                _exception.Message.ShouldEqual("Mapped System with id '" + _invalidMappedSystemId + "' does not exist.");
            }
        }

        [TestFixture]
        public class When_getting_mapped_system_that_has_been_marked_as_deleted : TestBase
        {
            private Exception _exception;
            private bool _exceptionThrown;
            private readonly Guid _mappedSystemId1 = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var mappedSystem = New.MappedSystem.WithId(_mappedSystemId1).NotActive();

                var repository = GenerateStub<IMappedSystemRepository>();
                repository.Stub(x => x.Get(_mappedSystemId1)).Return(mappedSystem);

                IDataStandardService service = new DataStandardService(repository, null, null, null, null);
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

            [Test]
            public void Should_throw_exception_with_good_message()
            {
                _exception.Message.ShouldEqual("Mapped System with id '" + _mappedSystemId1 + "' is marked as deleted.");
            }
        }
    }
}