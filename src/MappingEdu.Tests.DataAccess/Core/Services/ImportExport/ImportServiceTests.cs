// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Core.Services.Import;
using MappingEdu.Tests.DataAccess.Bases;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.DataAccess.Core.Services.ImportExport
{
    public class ImportServiceTests
    {
        [TestFixture]
        public class When_importing_a_new_mapped_system : EmptyDatabaseTestBase
        {
            private ImportResult _importResult;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var serializedMappedSystem = new SerializedMappedSystem
                {
                    Name = "My Imported System",
                    Version = "1.2.3",
                    CustomDetails = new[]
                    {
                        new SerializedCustomDetailMetadata
                        {
                            Name = "Prop1",
                            IsBoolean = false
                        },
                        new SerializedCustomDetailMetadata
                        {
                            Name = "Another Custom Detail",
                            IsBoolean = true
                        }
                    },
                    Domains = new[]
                    {
                        new SerializedDomain
                        {
                            Name = "Domain1",
                            Entities = new[]
                            {
                                new SerializedEntity
                                {
                                    Name = "Entity1",
                                    Elements = new[]
                                    {
                                        new SerializedElement
                                        {
                                            Name = "Element1",
                                            CustomDetails = new[]
                                            {
                                                new SerializedElementCustomDetail
                                                {
                                                    Name = "Prop1",
                                                    Value = "Element1 prop1 value"
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            Enumerations = new[]
                            {
                                new SerializedEnumeration
                                {
                                    Name = "Enumeration1",
                                    EnumerationValues = new[]
                                    {
                                        new SerializedEnumerationValue
                                        {
                                            CodeValue = "code1"
                                        }
                                    }
                                }
                            }
                        }
                    }
                };

                var importService = GetInstance<IImportService>();
                _importResult = importService.Import(serializedMappedSystem, new ImportOptions());
            }

            [Test]
            public void Should_add_new_custom_details()
            {
                var dbContext = CreateDbContext();

                var customDetail1 = dbContext.CustomDetailMetadatas.FirstOrDefault(x => x.DisplayName == "Prop1");
                customDetail1.ShouldNotBeNull();
                customDetail1.IsBoolean.ShouldBeFalse();

                var customDetail2 = dbContext.CustomDetailMetadatas.FirstOrDefault(x => x.DisplayName == "Another Custom Detail");
                customDetail2.ShouldNotBeNull();
                customDetail2.IsBoolean.ShouldBeTrue();
            }

            [Test]
            public void Should_add_new_enumeration_items()
            {
                var dbContext = CreateDbContext();

                var enumerationValue = dbContext.SystemEnumerationItems.FirstOrDefault();
                enumerationValue.ShouldNotBeNull();
                enumerationValue.CodeValue.ShouldEqual("code1");
            }

            [Test]
            public void Should_add_new_mapped_system()
            {
                var dbContext = CreateDbContext();
                var mappedSystem = dbContext.MappedSystems.SingleOrDefault(x => x.MappedSystemId == _importResult.MappedSystemId);
                mappedSystem.ShouldNotBeNull();
                mappedSystem.SystemName.ShouldEqual("My Imported System");
                mappedSystem.SystemVersion.ShouldEqual("1.2.3");
            }

            [Test]
            public void Should_add_new_system_item_custom_details()
            {
                var dbContext = CreateDbContext();

                var systemItemCustomDetail = dbContext.SystemItemCustomDetails.FirstOrDefault();
                systemItemCustomDetail.ShouldNotBeNull();
                systemItemCustomDetail.Value.ShouldEqual("Element1 prop1 value");
            }

            [Test]
            public void Should_add_new_system_items()
            {
                var dbContext = CreateDbContext();

                var domain = dbContext.SystemItems.SingleOrDefault(x => x.ItemTypeId == ItemType.Domain.Id);
                domain.ShouldNotBeNull();
                domain.ItemName.ShouldEqual("Domain1");

                var entity = dbContext.SystemItems.SingleOrDefault(x => x.ItemTypeId == ItemType.Entity.Id);
                entity.ShouldNotBeNull();
                entity.ItemName.ShouldEqual("Entity1");

                var element = dbContext.SystemItems.SingleOrDefault(x => x.ItemTypeId == ItemType.Element.Id);
                element.ShouldNotBeNull();
                element.ItemName.ShouldEqual("Element1");

                var enumeration = dbContext.SystemItems.SingleOrDefault(x => x.ItemTypeId == ItemType.Enumeration.Id);
                enumeration.ShouldNotBeNull();
                enumeration.ItemName.ShouldEqual("Enumeration1");
            }

            [Test]
            public void Should_import_with_success()
            {
                _importResult.ShouldNotBeNull();
                _importResult.MappedSystemId.ShouldNotEqual(Guid.Empty);
            }
        }

        [TestFixture]
        public class When_importing_an_existing_mapped_system : EmptyDatabaseTestBase
        {
            private ImportResult _importResult;
            private Guid _mappedSystemId;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, "My Imported System", "1.2.3");
                var domain = CreateDomain(dbContext, mappedSystem, "Domain1");
                var entity = CreateEntity(dbContext, domain, "Entity1");
                CreateElement(dbContext, entity, "Element1");
                _mappedSystemId = mappedSystem.MappedSystemId;

                var serializedMappedSystem = new SerializedMappedSystem
                {
                    Name = "My Imported System",
                    Version = "1.2.3",
                    Domains = new[]
                    {
                        new SerializedDomain
                        {
                            Name = "Domain1",
                            Entities = new[]
                            {
                                new SerializedEntity
                                {
                                    Name = "Entity1",
                                    Definition = "new definition",
                                    Elements = new[]
                                    {
                                        new SerializedElement
                                        {
                                            Name = "Element1",
                                            Definition = "apple banana carrot"
                                        },
                                        new SerializedElement
                                        {
                                            Name = "Element2",
                                            Definition = "this is a new element"
                                        }
                                    }
                                }
                            }
                        }
                    }
                };

                var importService = GetInstance<IImportService>();
                _importResult = importService.Import(serializedMappedSystem, new ImportOptions {UpsertBasedOnName = true});
            }

            [Test]
            public void Should_add_new_system_items()
            {
                var dbContext = CreateDbContext();

                var domain = dbContext.SystemItems.SingleOrDefault(x => x.ItemTypeId == ItemType.Domain.Id);
                domain.ShouldNotBeNull();
                domain.ItemName.ShouldEqual("Domain1");

                var entity = dbContext.SystemItems.SingleOrDefault(x => x.ItemTypeId == ItemType.Entity.Id);
                entity.ShouldNotBeNull();
                entity.ItemName.ShouldEqual("Entity1");
                entity.Definition.ShouldEqual("new definition");

                var element1 = dbContext.SystemItems.SingleOrDefault(x => x.ItemTypeId == ItemType.Element.Id && x.ItemName == "Element1");
                element1.ShouldNotBeNull();
                element1.Definition.ShouldEqual("apple banana carrot");

                var element2 = dbContext.SystemItems.SingleOrDefault(x => x.ItemTypeId == ItemType.Element.Id && x.ItemName == "Element2");
                element2.ShouldNotBeNull();
                element2.Definition.ShouldEqual("this is a new element");
            }

            [Test]
            public void Should_import_with_success()
            {
                _importResult.ShouldNotBeNull();
                _importResult.MappedSystemId.ShouldEqual(_mappedSystemId);
            }

            [Test]
            public void Should_not_add_new_mapped_system()
            {
                var dbContext = CreateDbContext();
                dbContext.MappedSystems.Count().ShouldEqual(1);
            }
        }
    }
}