// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using MappingEdu.Core.DataAccess.Repositories;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.AutoMap;
using MappingEdu.Service.Model.DataStandard;
using MappingEdu.Service.Model.ElementDetails;
using MappingEdu.Tests.Business.Bases;
using NUnit.Framework;
using Rhino.Mocks;
using Should;
using AutoMappingReasonType = MappingEdu.Core.Domain.Enumerations.AutoMappingReasonType;
using ItemType = MappingEdu.Core.Domain.Enumerations.ItemType;
using MappingMethodType = MappingEdu.Core.Domain.Enumerations.MappingMethodType;

namespace MappingEdu.Tests.Business.Service.AutoMap
{
    public class AutoMapServiceFixtures
    {
        [TestFixture]
        public class When_Running_Auto_Mapper_ : TestBase
        {
            private readonly Guid suppliedStandardAId = new Guid("00000000-0000-0000-0000-000000000001");
            private readonly Guid suppliedStandardBId = new Guid("00000000-0000-0000-0000-000000000002");
            private readonly Guid suppliedStandardCId = new Guid("00000000-0000-0000-0000-000000000003");

            private readonly Guid suppliedSystemItemAId = new Guid("00000000-0000-0000-0000-000000000011");
            private readonly Guid suppliedSystemItemBId = new Guid("00000000-0000-0000-0000-000000000012");
            private readonly Guid suppliedSystemItemCId = new Guid("00000000-0000-0000-0000-000000000013");
            private readonly Guid suppliedSystemItemDId = new Guid("00000000-0000-0000-0000-000000000014");

            private readonly Guid suppliedMappingProjectAId = new Guid("00000000-0000-0000-0000-000000000021");
            private readonly Guid suppliedMappingProjectBId = new Guid("00000000-0000-0000-0000-000000000022");

            private readonly MappedSystem emptySystem = new MappedSystem();

            [OneTimeSetUp]
            public void Setup()
            {
            }

            [Test]
            public void Should_Return_A_Same_Path_Mapping_Suggestion()
            {
                var sourceSystemItem = new ElementDetailsSearchModel() { DomainItemPath = "Domain.Entity.Element", SystemItemId = suppliedSystemItemAId, ItemTypeId = ItemType.Element.Id };
                var targetSystemItem = new ElementDetailsSearchModel() { DomainItemPath = "Domain.Entity.Element", SystemItemId = suppliedSystemItemBId, ItemTypeId = ItemType.Element.Id };

                var systemItemRepository = GenerateStub<ISystemItemRepository>();
                var mappedSystemRepository = GenerateStub<IMappedSystemRepository>();
                var mappingProjectRepository = GenerateStub<IMappingProjectRepository>();

                systemItemRepository.Stub(x => x.GetAllForComparison(suppliedStandardAId))
                    .Return(new List<ElementDetailsSearchModel> { sourceSystemItem }.ToArray());

                systemItemRepository.Stub(x => x.GetAllForComparison(suppliedStandardBId))
                    .Return(new List<ElementDetailsSearchModel> { targetSystemItem }.ToArray());

                mappedSystemRepository.Stub(x => x.Get(suppliedStandardAId)).Return(emptySystem);
                mappedSystemRepository.Stub(x => x.Get(suppliedStandardBId)).Return(emptySystem);
                mappingProjectRepository.Stub(x => x.GetAllQueryable()).Return(new List<MappingProject>().AsQueryable());

                var service = new AutoMapService(mappingProjectRepository, null, systemItemRepository, null, mappedSystemRepository, null, null);

                var result = service.GetAutoMapResults(suppliedStandardAId, suppliedStandardBId);

                result.Count.ShouldBeGreaterThan(0);
                result.Count.ShouldEqual(1);
                result.First().BusinessLogic.ShouldEqual(String.Format("[{0}]", "Domain.Entity.Element"));
                result.First().Reason.ShouldEqual(AutoMappingReasonType.SamePath);
            }

            [Test]
            public void Should_Not_Return_A_Same_Path_Suggestion_When_Item_Types_Are_Not_Equal()
            {
                var sourceSystemItem = new ElementDetailsSearchModel() { DomainItemPath = "Domain.Entity.Element", SystemItemId = suppliedSystemItemAId, ItemTypeId = ItemType.Element.Id };
                var targetSystemItem = new ElementDetailsSearchModel() { DomainItemPath = "Domain.Entity.Element", SystemItemId = suppliedSystemItemBId, ItemTypeId = ItemType.Enumeration.Id };

                var systemItemRepository = GenerateStub<ISystemItemRepository>();
                var mappedSystemRepository = GenerateStub<IMappedSystemRepository>();
                var mappingProjectRepository = GenerateStub<IMappingProjectRepository>();

                systemItemRepository.Stub(x => x.GetAllForComparison(suppliedStandardAId))
                    .Return(new List<ElementDetailsSearchModel> {sourceSystemItem}.ToArray());

                systemItemRepository.Stub(x => x.GetAllForComparison(suppliedStandardBId))
                    .Return(new List<ElementDetailsSearchModel> {targetSystemItem}.ToArray());

                mappedSystemRepository.Stub(x => x.Get(suppliedStandardAId)).Return(emptySystem);
                mappedSystemRepository.Stub(x => x.Get(suppliedStandardBId)).Return(emptySystem);
                mappingProjectRepository.Stub(x => x.GetAllQueryable()).Return(new List<MappingProject>().AsQueryable());

                var service = new AutoMapService(mappingProjectRepository, null, systemItemRepository, null, mappedSystemRepository, null, null);

                var result = service.GetAutoMapResults(suppliedStandardAId, suppliedStandardBId);

                result.Count.ShouldEqual(0);
            }

            [Test]
            public void Should_Return_A_Previous_Source_Version_Suggestion()
            {
                var sourceSystemItem = new ElementDetailsSearchModel() { DomainItemPath = "StandardADomain.Entity.Element", SystemItemId = suppliedSystemItemAId, ItemTypeId = ItemType.Element.Id };
                var previousSourceSystemItem = new ElementDetailsSearchModel() { DomainItemPath = "StandardADomain.Entity.Element", SystemItemId = suppliedSystemItemBId, ItemTypeId = ItemType.Element.Id };
                var targetSystemItem = new ElementDetailsSearchModel() { DomainItemPath = "StandardBDomain.Entity.Element", SystemItemId = suppliedSystemItemCId, ItemTypeId = ItemType.Element.Id };

                var systemItemRepository = GenerateStub<ISystemItemRepository>();
                var mappedSystemRepository = GenerateStub<IMappedSystemRepository>();
                var mappingProjectRepository = GenerateStub<IMappingProjectRepository>();
                var systemItemMapRepository = GenerateStub<ISystemItemMapRepository>();
                var mapper = GenerateStub<IMapper>();

                systemItemRepository.Stub(x => x.GetAllForComparison(suppliedStandardAId))
                    .Return(new List<ElementDetailsSearchModel> { sourceSystemItem }.ToArray());

                systemItemRepository.Stub(x => x.GetAllForComparison(suppliedStandardBId))
                    .Return(new List<ElementDetailsSearchModel> { previousSourceSystemItem }.ToArray());

                systemItemRepository.Stub(x => x.GetAllForComparison(suppliedStandardCId))
                    .Return(new List<ElementDetailsSearchModel> { targetSystemItem }.ToArray());

                mappedSystemRepository.Stub(x => x.Get(suppliedStandardAId)).Return(new MappedSystem { PreviousMappedSystemId = suppliedStandardBId });
                mappedSystemRepository.Stub(x => x.Get(suppliedStandardBId)).Return(emptySystem);
                mappedSystemRepository.Stub(x => x.Get(suppliedStandardCId)).Return(emptySystem);

                mapper.Stub(x => x.Map<DataStandardViewModel>(emptySystem)).Return(new DataStandardViewModel());

                mappingProjectRepository.Stub(x => x.GetAllQueryable())
                    .Return(new List<MappingProject>{
                        new MappingProject
                        {
                            SourceDataStandardMappedSystemId = suppliedStandardBId,
                            TargetDataStandardMappedSystemId = suppliedStandardCId,
                            MappingProjectId = suppliedMappingProjectAId,
                            IsActive = true
                        }
                }.AsQueryable());

                systemItemMapRepository.Stub(x => x.GetAllForComparison(suppliedMappingProjectAId))
                    .Return(new List<SystemItemMap>
                    {
                        new SystemItemMap
                        {
                            MappingMethodTypeId = MappingMethodType.EnterMappingBusinessLogic.Id,
                            BusinessLogic = String.Format("[{0}]", targetSystemItem.DomainItemPath),
                            SourceSystemItemId = suppliedSystemItemBId,
                            TargetSystemItems = new List<SystemItem>
                            {
                                new SystemItem
                                {
                                    SystemItemId = suppliedSystemItemCId,
                                    ItemTypeId = ItemType.Element.Id,
                                    MappedSystemId = suppliedStandardBId
                                }
                            }
                        }       
                    }.ToArray());

                var service = new AutoMapService(mappingProjectRepository, mapper, systemItemRepository, systemItemMapRepository, mappedSystemRepository, null, null);

                var result = service.GetAutoMapResults(suppliedStandardAId, suppliedStandardCId);

                result.Count.ShouldBeGreaterThan(0);
                result.Count.ShouldEqual(1);
                result.First().BusinessLogic.ShouldEqual(String.Format("[{0}]", targetSystemItem.DomainItemPath));
                result.First().SourceSystemItem.SystemItemId.ShouldEqual(suppliedSystemItemAId);
                result.First().Reason.ShouldEqual(AutoMappingReasonType.PreviousSourceVersion);
            }

            [Test]
            public void Should_Return_A_Previous_Target_Version_Suggestion()
            {
                var sourceSystemItem = new ElementDetailsSearchModel() { DomainItemPath = "StandardADomain.Entity.Element", SystemItemId = suppliedSystemItemAId, ItemTypeId = ItemType.Element.Id };
                var targetSystemItem = new ElementDetailsSearchModel() { DomainItemPath = "StandardBDomain.Entity.Element", SystemItemId = suppliedSystemItemBId, ItemTypeId = ItemType.Element.Id };
                var previousTargetSystemItem = new ElementDetailsSearchModel() { DomainItemPath = "StandardBDomain.Entity.Element", SystemItemId = suppliedSystemItemCId, ItemTypeId = ItemType.Element.Id };

                var systemItemRepository = GenerateStub<ISystemItemRepository>();
                var mappedSystemRepository = GenerateStub<IMappedSystemRepository>();
                var mappingProjectRepository = GenerateStub<IMappingProjectRepository>();
                var systemItemMapRepository = GenerateStub<ISystemItemMapRepository>();
                var mapper = GenerateStub<IMapper>();

                systemItemRepository.Stub(x => x.GetAllForComparison(suppliedStandardAId))
                    .Return(new List<ElementDetailsSearchModel> { sourceSystemItem }.ToArray());

                systemItemRepository.Stub(x => x.GetAllForComparison(suppliedStandardBId))
                    .Return(new List<ElementDetailsSearchModel> { targetSystemItem }.ToArray());

                systemItemRepository.Stub(x => x.GetAllForComparison(suppliedStandardCId))
                    .Return(new List<ElementDetailsSearchModel> { previousTargetSystemItem }.ToArray());

                mapper.Stub(x => x.Map<DataStandardViewModel>(emptySystem)).Return(new DataStandardViewModel());

                mappedSystemRepository.Stub(x => x.Get(suppliedStandardAId)).Return(emptySystem);
                mappedSystemRepository.Stub(x => x.Get(suppliedStandardBId)).Return(new MappedSystem { PreviousMappedSystemId = suppliedStandardCId });
                mappedSystemRepository.Stub(x => x.Get(suppliedStandardCId)).Return(emptySystem);

                mappingProjectRepository.Stub(x => x.GetAllQueryable())
                    .Return(new List<MappingProject>{
                        new MappingProject
                        {
                            SourceDataStandardMappedSystemId = suppliedStandardAId,
                            TargetDataStandardMappedSystemId = suppliedStandardCId,
                            MappingProjectId = suppliedMappingProjectAId,
                            IsActive = true
                        }
                }.AsQueryable());

                systemItemMapRepository.Stub(x => x.GetAllForComparison(suppliedMappingProjectAId))
                    .Return(new List<SystemItemMap>
                    {
                        new SystemItemMap
                        {
                            MappingMethodTypeId = MappingMethodType.EnterMappingBusinessLogic.Id,
                            BusinessLogic = String.Format("[{0}]", targetSystemItem.DomainItemPath),
                            SourceSystemItemId = suppliedSystemItemAId,
                            TargetSystemItems = new List<SystemItem>
                            {
                                new SystemItem
                                {
                                    SystemItemId = suppliedSystemItemCId,
                                    ItemTypeId = ItemType.Element.Id,
                                    MappedSystemId = suppliedStandardBId
                                }
                            }
                        }
                    }.ToArray());

                var service = new AutoMapService(mappingProjectRepository, mapper, systemItemRepository, systemItemMapRepository, mappedSystemRepository, null, null);

                var result = service.GetAutoMapResults(suppliedStandardAId, suppliedStandardBId);

                result.Count.ShouldBeGreaterThan(0);
                result.Count.ShouldEqual(1);
                result.First().BusinessLogic.ShouldEqual(String.Format("[{0}]", targetSystemItem.DomainItemPath));
                result.First().SourceSystemItem.SystemItemId.ShouldEqual(suppliedSystemItemAId);
                result.First().Reason.ShouldEqual(AutoMappingReasonType.PreviousTargetVersion);
            }

            [Test]
            public void Should_Return_A_Transitive_Suggestion()
            {
                var sourceSystemItem = new ElementDetailsSearchModel() { DomainItemPath = "StandardADomain.Entity.Element", SystemItemId = suppliedSystemItemAId, ItemTypeId = ItemType.Element.Id };
                var targetSystemItem = new ElementDetailsSearchModel() { DomainItemPath = "StandardBDomain.Entity.Element", SystemItemId = suppliedSystemItemBId, ItemTypeId = ItemType.Element.Id };
                var transitiveSystemItem = new ElementDetailsSearchModel() { DomainItemPath = "StandardCDomain.Entity.Element", SystemItemId = suppliedSystemItemCId, ItemTypeId = ItemType.Element.Id };

                var systemItemRepository = GenerateStub<ISystemItemRepository>();
                var mappedSystemRepository = GenerateStub<IMappedSystemRepository>();
                var mappingProjectRepository = GenerateStub<IMappingProjectRepository>();
                var systemItemMapRepository = GenerateStub<ISystemItemMapRepository>();
                var mapper = GenerateStub<IMapper>();

                systemItemRepository.Stub(x => x.GetAllForComparison(suppliedStandardAId))
                    .Return(new List<ElementDetailsSearchModel> { sourceSystemItem }.ToArray());

                systemItemRepository.Stub(x => x.GetAllForComparison(suppliedStandardBId))
                    .Return(new List<ElementDetailsSearchModel> { targetSystemItem }.ToArray());

                systemItemRepository.Stub(x => x.GetAllForComparison(suppliedStandardCId))
                    .Return(new List<ElementDetailsSearchModel> { transitiveSystemItem }.ToArray());

                mappedSystemRepository.Stub(x => x.Get(suppliedStandardAId)).Return(emptySystem);
                mappedSystemRepository.Stub(x => x.Get(suppliedStandardBId)).Return(emptySystem);

                mappingProjectRepository.Stub(x => x.GetAllQueryable())
                    .Return(new List<MappingProject>{
                        new MappingProject
                        {
                            SourceDataStandardMappedSystemId = suppliedStandardAId,
                            TargetDataStandardMappedSystemId = suppliedStandardCId,
                            MappingProjectId = suppliedMappingProjectAId,
                            IsActive = true
                        },
                        new MappingProject
                        {
                            SourceDataStandardMappedSystemId = suppliedStandardBId,
                            TargetDataStandardMappedSystemId = suppliedStandardCId,
                            MappingProjectId = suppliedMappingProjectBId,
                            IsActive = true
                        },
                }.AsQueryable());

                systemItemMapRepository.Stub(x => x.GetAllForComparison(suppliedMappingProjectAId))
                  .Return(new List<SystemItemMap>
                  {
                        new SystemItemMap
                        {
                            MappingMethodTypeId = MappingMethodType.EnterMappingBusinessLogic.Id,
                            BusinessLogic = String.Format("[{0}]", targetSystemItem.DomainItemPath),
                            SourceSystemItemId = suppliedSystemItemAId,
                            TargetSystemItems = new List<SystemItem>
                            {
                                new SystemItem
                                {
                                    SystemItemId = suppliedSystemItemCId,
                                    ItemTypeId = ItemType.Element.Id,
                                    MappedSystemId = suppliedStandardBId
                                }
                            }
                        }
                  }.ToArray());

                systemItemMapRepository.Stub(x => x.GetAllForComparison(suppliedMappingProjectBId))
                  .Return(new List<SystemItemMap>
                  {
                        new SystemItemMap
                        {
                            MappingMethodTypeId = MappingMethodType.EnterMappingBusinessLogic.Id,
                            BusinessLogic = String.Format("[{0}]", targetSystemItem.DomainItemPath),
                            SourceSystemItemId = suppliedSystemItemBId,
                            TargetSystemItems = new List<SystemItem>
                            {
                                new SystemItem
                                {
                                    SystemItemId = suppliedSystemItemCId,
                                    ItemTypeId = ItemType.Element.Id,
                                    MappedSystemId = suppliedStandardBId
                                }
                            }
                        }
                  }.ToArray());

                mapper.Stub(x => x.Map<DataStandardViewModel>(emptySystem)).Return(new DataStandardViewModel());

                var service = new AutoMapService(mappingProjectRepository, mapper, systemItemRepository, systemItemMapRepository, mappedSystemRepository, null, null);

                var result = service.GetAutoMapResults(suppliedStandardAId, suppliedStandardBId);

                result.Count.ShouldBeGreaterThan(0);
                result.Count.ShouldEqual(1);
                result.First().BusinessLogic.ShouldEqual(String.Format("[{0}]", targetSystemItem.DomainItemPath));
                result.First().SourceSystemItem.SystemItemId.ShouldEqual(suppliedSystemItemAId);
                result.First().Reason.ShouldEqual(AutoMappingReasonType.Transitive);
            }

            [Test]
            public void Should_Return_2_Transitive_Suggestions()
            {
                var sourceSystemItem = new ElementDetailsSearchModel() { DomainItemPath = "StandardADomain.Entity.Element", SystemItemId = suppliedSystemItemAId, ItemTypeId = ItemType.Element.Id };
                var sourceSystemItem2 = new ElementDetailsSearchModel() { DomainItemPath = "StandardADomain.Entity.Element2", SystemItemId = suppliedSystemItemDId, ItemTypeId = ItemType.Element.Id };
                var targetSystemItem = new ElementDetailsSearchModel() { DomainItemPath = "StandardBDomain.Entity.Element", SystemItemId = suppliedSystemItemBId, ItemTypeId = ItemType.Element.Id };
                var transitiveSystemItem = new ElementDetailsSearchModel() { DomainItemPath = "StandardCDomain.Entity.Element", SystemItemId = suppliedSystemItemCId, ItemTypeId = ItemType.Element.Id };

                var systemItemRepository = GenerateStub<ISystemItemRepository>();
                var mappedSystemRepository = GenerateStub<IMappedSystemRepository>();
                var mappingProjectRepository = GenerateStub<IMappingProjectRepository>();
                var systemItemMapRepository = GenerateStub<ISystemItemMapRepository>();
                var mapper = GenerateStub<IMapper>();

                systemItemRepository.Stub(x => x.GetAllForComparison(suppliedStandardAId))
                    .Return(new List<ElementDetailsSearchModel> { sourceSystemItem, sourceSystemItem2 }.ToArray());

                systemItemRepository.Stub(x => x.GetAllForComparison(suppliedStandardBId))
                    .Return(new List<ElementDetailsSearchModel> { targetSystemItem }.ToArray());

                systemItemRepository.Stub(x => x.GetAllForComparison(suppliedStandardCId))
                    .Return(new List<ElementDetailsSearchModel> { transitiveSystemItem }.ToArray());

                mappedSystemRepository.Stub(x => x.Get(suppliedStandardAId)).Return(emptySystem);
                mappedSystemRepository.Stub(x => x.Get(suppliedStandardBId)).Return(emptySystem);

                mappingProjectRepository.Stub(x => x.GetAllQueryable())
                    .Return(new List<MappingProject>{
                        new MappingProject
                        {
                            SourceDataStandardMappedSystemId = suppliedStandardAId,
                            TargetDataStandardMappedSystemId = suppliedStandardCId,
                            MappingProjectId = suppliedMappingProjectAId,
                            IsActive = true
                        },
                        new MappingProject
                        {
                            SourceDataStandardMappedSystemId = suppliedStandardBId,
                            TargetDataStandardMappedSystemId = suppliedStandardCId,
                            MappingProjectId = suppliedMappingProjectBId,
                            IsActive = true
                        },
                }.AsQueryable());

                systemItemMapRepository.Stub(x => x.GetAllForComparison(suppliedMappingProjectAId))
                  .Return(new List<SystemItemMap>
                  {
                        new SystemItemMap
                        {
                            MappingMethodTypeId = MappingMethodType.EnterMappingBusinessLogic.Id,
                            BusinessLogic = String.Format("[{0}]", targetSystemItem.DomainItemPath),
                            SourceSystemItemId = suppliedSystemItemAId,
                            TargetSystemItems = new List<SystemItem>
                            {
                                new SystemItem
                                {
                                    SystemItemId = suppliedSystemItemCId,
                                    ItemTypeId = ItemType.Element.Id,
                                    MappedSystemId = suppliedStandardBId
                                }
                            }
                        },
                        new SystemItemMap
                        {
                            MappingMethodTypeId = MappingMethodType.EnterMappingBusinessLogic.Id,
                            BusinessLogic = String.Format("[{0}]", targetSystemItem.DomainItemPath),
                            SourceSystemItemId = suppliedSystemItemDId,
                            TargetSystemItems = new List<SystemItem>
                            {
                                new SystemItem
                                {
                                    SystemItemId = suppliedSystemItemCId,
                                    ItemTypeId = ItemType.Element.Id,
                                    MappedSystemId = suppliedStandardBId
                                }
                            }
                        }
                  }.ToArray());

                systemItemMapRepository.Stub(x => x.GetAllForComparison(suppliedMappingProjectBId))
                  .Return(new List<SystemItemMap>
                  {
                        new SystemItemMap
                        {
                            MappingMethodTypeId = MappingMethodType.EnterMappingBusinessLogic.Id,
                            BusinessLogic = String.Format("[{0}]", targetSystemItem.DomainItemPath),
                            SourceSystemItemId = suppliedSystemItemBId,
                            TargetSystemItems = new List<SystemItem>
                            {
                                new SystemItem
                                {
                                    SystemItemId = suppliedSystemItemCId,
                                    ItemTypeId = ItemType.Element.Id,
                                    MappedSystemId = suppliedStandardBId
                                }
                            }
                        }
                  }.ToArray());

                mapper.Stub(x => x.Map<DataStandardViewModel>(emptySystem)).Return(new DataStandardViewModel());

                var service = new AutoMapService(mappingProjectRepository, mapper, systemItemRepository, systemItemMapRepository, mappedSystemRepository, null, null);

                var result = service.GetAutoMapResults(suppliedStandardAId, suppliedStandardBId);

                result.Count.ShouldBeGreaterThan(0);
                result.Count.ShouldEqual(2);
            }
        }
    }
}
