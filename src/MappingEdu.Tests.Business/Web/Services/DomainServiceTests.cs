// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Domains;
using MappingEdu.Service.Model.Domain;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Should;

namespace MappingEdu.Tests.Business.Web.Services
{
    public class DomainServiceTests
    {
        [TestFixture]
        public class When_getting_specific_domain : TestBase
        {
            private readonly Guid _id = Guid.NewGuid();
            private readonly Guid _mappedSystemId = Guid.NewGuid();
            private readonly DomainViewModel _expected = new DomainViewModel();
            private DomainViewModel _result;

            [OneTimeSetUp]
            public void Setup()
            {
                var mappedSystem = New.MappedSystem.WithId(_mappedSystemId);
                SystemItem systemItem = New.SystemItem.AsDomain.IsActive().WithId(_id).WithMappedSystem(mappedSystem);

                var domainRepository = GenerateStub<ISystemItemRepository>();
                var mapper = GenerateStub<IMapper>();

                domainRepository.Stub(x => x.Get(_id)).Return(systemItem);
                mapper.Stub(x => x.Map<DomainViewModel>(systemItem)).Return(_expected);

                IDomainService service = new DomainService(domainRepository, null, mapper);
                _result = service.Get(_mappedSystemId, _id);
            }

            [Test]
            public void Should_populate_view_model()
            {
                _result.ShouldNotBeNull();
                _result.ShouldEqual(_expected);
            }
        }

        [TestFixture]
        public class When_getting_specific_domain_that_does_not_exist : TestBase
        {
            private bool _exceptionThrown;
            private Exception _exception;
            private readonly Guid _id = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var domainRepository = GenerateStub<ISystemItemRepository>();
                domainRepository.Stub(x => x.Get(_id)).Return(null);

                IDomainService service = new DomainService(domainRepository, null, null);
                try
                {
                    service.Get(Guid.NewGuid(), _id);
                }
                catch (Exception e)
                {
                    _exceptionThrown = true;
                    _exception = e;
                }
            }

            [Test]
            public void Should_give_meaningful_error_message()
            {
                _exception.Message.ShouldEqual(string.Format("The System Item with id '{0}' does not exist.", _id));
            }

            [Test]
            public void Should_throw_exception()
            {
                _exceptionThrown.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_getting_specific_domain_for_wrong_mapped_system : TestBase
        {
            private bool _exceptionThrown;
            private Exception _exception;
            private readonly Guid _mappedSystemId = Guid.NewGuid();
            private readonly Guid _systemItemId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var mappedSystem = New.MappedSystem.WithId(Guid.NewGuid());
                SystemItem systemItem = New.SystemItem.WithId(_systemItemId).IsActive().AsDomain.WithMappedSystem(mappedSystem);

                var domainRepository = GenerateStub<ISystemItemRepository>();
                domainRepository.Stub(x => x.Get(_systemItemId)).Return(systemItem);

                IDomainService service = new DomainService(domainRepository, null, null);
                try
                {
                    service.Get(_mappedSystemId, _systemItemId);
                }
                catch (Exception e)
                {
                    _exceptionThrown = true;
                    _exception = e;
                }
            }

            [Test]
            public void Should_give_meaningful_error_message()
            {
                _exception.Message.ShouldEqual(string.Format("Domain System Item with id '{0}' does not have mapped system id of '{1}'.", _systemItemId, _mappedSystemId));
            }

            [Test]
            public void Should_throw_exception()
            {
                _exceptionThrown.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_getting_all_domains_for_mapped_system : TestBase
        {
            private readonly Guid _mappedSystemId = Guid.NewGuid();
            private DomainViewModel[] _expected;
            private DomainViewModel[] _result;

            [OneTimeSetUp]
            public void Setup()
            {
                MappedSystem mappedSystem = New.MappedSystem.WithId(_mappedSystemId);
                SystemItem systemItem1 = New.SystemItem.AsDomain.WithMappedSystem(mappedSystem).IsActive();
                SystemItem systemItem2 = New.SystemItem.AsDomain.WithMappedSystem(mappedSystem).IsActive();
                SystemItem systemItem3 = New.SystemItem.AsDomain.WithMappedSystem(mappedSystem).IsActive();

                var systemItems = new[] {systemItem1, systemItem2, systemItem3};

                var systemItemRepository = GenerateStub<ISystemItemRepository>();
                systemItemRepository.Stub(x => x.GetWhere(_mappedSystemId, null)).Return(systemItems);

                _expected = new[] {new DomainViewModel(), new DomainViewModel(), new DomainViewModel()};
                var mapper = GenerateStub<IMapper>();
                mapper.Stub(x => x.Map<DomainViewModel[]>(systemItems)).Return(_expected);

                IDomainService service = new DomainService(systemItemRepository, null, mapper);
                _result = service.Get(_mappedSystemId);
            }

            [Test]
            public void Should_populate_view_model()
            {
                _result.ShouldNotBeNull();
                _result.Length.ShouldEqual(3);
                _result.ShouldEqual(_expected);
            }
        }

        [TestFixture]
        public class When_updating_a_domain_that_does_not_exist : TestBase
        {
            private bool _exceptionThrown;
            private Exception _exception;
            private readonly Guid _id = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var domainRepository = GenerateStub<ISystemItemRepository>();
                domainRepository.Stub(x => x.Get(_id)).Return(null);

                IDomainService service = new DomainService(domainRepository, null, null);
                try
                {
                    var editModel = new DomainEditModel
                    {
                        SystemItemId = _id
                    };

                    service.Put(new Guid(), editModel);
                }
                catch (Exception e)
                {
                    _exceptionThrown = true;
                    _exception = e;
                }
            }

            [Test]
            public void Should_give_meaningful_error_message()
            {
                _exception.Message.ShouldEqual(string.Format("The System Item with id '{0}' does not exist.", _id));
            }

            [Test]
            public void Should_throw_exception()
            {
                _exceptionThrown.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_deleting_a_domain_that_does_not_exist : TestBase
        {
            private bool _exceptionThrown;
            private Exception _exception;
            private readonly Guid _id = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var domainRepository = GenerateStub<ISystemItemRepository>();
                domainRepository.Stub(x => x.Get(_id)).Return(null);

                IDomainService service = new DomainService(domainRepository, null, null);
                try
                {
                    service.Delete(Guid.NewGuid(), _id);
                }
                catch (Exception e)
                {
                    _exceptionThrown = true;
                    _exception = e;
                }
            }

            [Test]
            public void Should_give_meaningful_error_message()
            {
                _exception.Message.ShouldEqual(string.Format("The System Item with id '{0}' does not exist.", _id));
            }

            [Test]
            public void Should_throw_exception()
            {
                _exceptionThrown.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_getting_system_item_that_is_not_a_domain : TestBase
        {
            private readonly Guid _mappedSystemId = Guid.NewGuid();
            private bool _exceptionThrown;
            private Exception _exception;
            private readonly Guid _id = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var mappedSystem = New.MappedSystem.WithId(_mappedSystemId);
                SystemItem systemItem = New.SystemItem.AsElement.IsActive().WithId(_id).WithMappedSystem(mappedSystem);

                var domainRepository = GenerateStub<ISystemItemRepository>();
                domainRepository.Stub(x => x.Get(_id)).Return(systemItem);

                IDomainService service = new DomainService(domainRepository, null, null);
                try
                {
                    service.Get(_mappedSystemId, _id);
                }
                catch (Exception e)
                {
                    _exceptionThrown = true;
                    _exception = e;
                }
            }

            [Test]
            public void Should_give_meaningful_error_message()
            {
                _exception.Message.ShouldEqual(string.Format("The System Item with id '{0}' is not a Domain System Item.", _id));
            }

            [Test]
            public void Should_throw_exception()
            {
                _exceptionThrown.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_getting_system_item_that_has_been_marked_as_deleted : TestBase
        {
            private readonly Guid _mappedSystemId = Guid.NewGuid();
            private bool _exceptionThrown;
            private Exception _exception;
            private readonly Guid _id = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var mappedSystem = New.MappedSystem.WithId(_mappedSystemId);
                SystemItem systemItem = New.SystemItem.AsDomain.NotActive().WithId(_id).WithMappedSystem(mappedSystem);

                var domainRepository = GenerateStub<ISystemItemRepository>();
                domainRepository.Stub(x => x.Get(_id)).Return(systemItem);

                IDomainService service = new DomainService(domainRepository, null, null);
                try
                {
                    service.Get(_mappedSystemId, _id);
                }
                catch (Exception e)
                {
                    _exceptionThrown = true;
                    _exception = e;
                }
            }

            [Test]
            public void Should_give_meaningful_error_message()
            {
                _exception.Message.ShouldEqual(string.Format("Domain System Item with id '{0}' is marked as deleted.", _id));
            }

            [Test]
            public void Should_throw_exception()
            {
                _exceptionThrown.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_creating_system_item_for_a_mapped_system_that_has_been_marked_as_deleted : TestBase
        {
            private readonly Guid _mappedSystemId = Guid.NewGuid();
            private bool _exceptionThrown;
            private Exception _exception;

            [OneTimeSetUp]
            public void Setup()
            {
                var mappedSystem = New.MappedSystem.WithId(_mappedSystemId).NotActive().WithSystemName("My System");

                var mappedSystemRepository = GenerateStub<IRepository<MappedSystem>>();
                mappedSystemRepository.Stub(x => x.Get(_mappedSystemId)).Return(mappedSystem);

                IDomainService service = new DomainService(null, mappedSystemRepository, null);
                try
                {
                    var domain = new DomainCreateModel
                    {
                        DataStandardId = _mappedSystemId
                    };

                    service.Post(domain);
                }
                catch (Exception e)
                {
                    _exceptionThrown = true;
                    _exception = e;
                }
            }

            [Test]
            public void Should_give_meaningful_error_message()
            {
                _exception.Message.ShouldEqual("Mapped System 'My System' is marked as deleted.");
            }

            [Test]
            public void Should_throw_exception()
            {
                _exceptionThrown.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_creating_system_item_for_a_mapped_system_that_does_not_exist : TestBase
        {
            private readonly Guid _mappedSystemId = Guid.NewGuid();
            private bool _exceptionThrown;
            private Exception _exception;

            [OneTimeSetUp]
            public void Setup()
            {
                var mappedSystemRepository = GenerateStub<IRepository<MappedSystem>>();
                mappedSystemRepository.Stub(x => x.Get(_mappedSystemId)).Return(null);

                IDomainService service = new DomainService(null, mappedSystemRepository, null);
                try
                {
                    var domain = new DomainCreateModel
                    {
                        DataStandardId = _mappedSystemId
                    };

                    service.Post(domain);
                }
                catch (Exception e)
                {
                    _exceptionThrown = true;
                    _exception = e;
                }
            }

            [Test]
            public void Should_give_meaningful_error_message()
            {
                _exception.Message.ShouldEqual(string.Format("Mapped System with id '{0}' does not exist.", _mappedSystemId));
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