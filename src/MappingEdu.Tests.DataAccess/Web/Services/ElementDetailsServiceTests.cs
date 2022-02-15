// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Service.Model.ElementDetails;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.DataAccess.Bases;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.DataAccess.Web.Services
{
    public class ElementDetailsServiceTests
    {
        private const string systemName = "System Name";
        private const string systemVersion = "1.0.0";
        private const string entityName = "Entity Name";
        private const string entityDefinition = "Entity Definition";
        private const string elementName = "Element Name";
        private const string elementDefinition = "Element Definition";
        private const string technicalName = "Technical Name";

        [TestFixture]
        public class When_saving_new_element : EmptyDatabaseTestBase
        {
            private readonly int? _elementFieldLength = 20;
            private readonly ItemDataType _elementDataType = ItemDataType.String;

            private Guid _entityId;

            private ElementDetailsViewModel _result;
            private const string _domainName = "Domain Name";
            private const string _domainDefinition = "Domain Definition";
            private const string _domainUrl = "http://domain.url";
            private const string _elementUrl = "http://element.url";
            private const string _dataTypeSource = "data type source";

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, systemName, systemVersion);
                var domain = CreateDomain(dbContext, mappedSystem, _domainName, _domainDefinition, _domainUrl);
                var entity = CreateEntity(dbContext, domain, entityName, entityDefinition);
                _entityId = entity.SystemItemId;

                var elementCreateModel = new ElementDetailsCreateModel
                {
                    ItemName = elementName,
                    TechnicalName = technicalName,
                    Definition = elementDefinition,
                    ItemDataTypeId = _elementDataType.Id,
                    DataTypeSource = _dataTypeSource,
                    FieldLength = _elementFieldLength,
                    ItemUrl = _elementUrl,
                    ParentSystemItemId = _entityId
                };

                var elementDetailsService = GetInstance<IElementDetailsService>();
                _result = elementDetailsService.Post(elementCreateModel);
            }

            [Test]
            public void Should_create_element()
            {
                var dbContext = CreateDbContext();

                var element = dbContext.SystemItems.First(x => x.ParentSystemItemId == _entityId);
                element.SystemItemId.ShouldNotEqual(Guid.Empty);
                element.ItemName.ShouldEqual(elementName);
                element.TechnicalName.ShouldEqual(technicalName);
                element.Definition.ShouldEqual(elementDefinition);
                element.ItemDataType.ShouldEqual(_elementDataType);
                element.DataTypeSource.ShouldEqual(_dataTypeSource);
                element.FieldLength.ShouldEqual(_elementFieldLength);
                element.ItemUrl.ShouldEqual(_elementUrl);
                element.ParentSystemItemId.ShouldEqual(_entityId);
                element.ItemType.ShouldEqual(ItemType.Element);
                element.IsActive.ShouldBeTrue();
            }

            [Test]
            public void Should_return_new_view_model()
            {
                _result.ShouldNotBeNull();
                _result.SystemItemId.ShouldNotEqual(Guid.Empty);

                _result.ItemName.ShouldEqual(elementName);
                _result.TechnicalName.ShouldEqual(technicalName);
                _result.Definition.ShouldEqual(elementDefinition);
                _result.ItemDataType.ShouldEqual(_elementDataType);
                _result.DataTypeSource.ShouldEqual(_dataTypeSource);
                _result.FieldLength.ShouldEqual(_elementFieldLength);
                _result.ItemUrl.ShouldEqual(_elementUrl);
            }
        }

        [TestFixture]
        public class When_setting_element : EmptyDatabaseTestBase
        {
            private Guid _entityId;
            private Guid _elementId;
            private const string _domainName = "Domain Name";
            private const string _domainDefinition = "Domain Definition";
            private const string _domainUrl = "http://domain.url";
            private const string _newItemName = "Updated Element Name";
            private const string _newDefinition = "Updated Element Definition";
            private const string _newTechnicalName = "Updated Technical Name";
            private const string _newItemUrl = "http:\\UpdatedItemUrl";
            private const string _newDataTypeSource = "data type source";
            private readonly int? _newFieldLength = 50;
            private readonly ItemDataType _newItemDataType = ItemDataType.Currency;
            private ElementDetailsViewModel _result;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, systemName, systemVersion);
                var domain = CreateDomain(dbContext, mappedSystem, _domainName, _domainDefinition, _domainUrl);
                var entity = CreateEntity(dbContext, domain, entityName, entityDefinition);
                var element = CreateElement(dbContext, entity, elementName, elementDefinition);
                _elementId = element.SystemItemId;
                _entityId = entity.SystemItemId;

                var elementEditModel = new ElementDetailsEditModel
                {
                    ItemName = _newItemName,
                    TechnicalName = _newTechnicalName,
                    Definition = _newDefinition,
                    ItemDataTypeId = _newItemDataType.Id,
                    FieldLength = _newFieldLength,
                    ItemUrl = _newItemUrl,
                    DataTypeSource = _newDataTypeSource
                };

                var elementDetailsService = GetInstance<IElementDetailsService>();
                _result = elementDetailsService.Put(_elementId, elementEditModel);
            }

            [Test]
            public void Should_return_updated_view_model()
            {
                _result.ShouldNotBeNull();
                _result.SystemItemId.ShouldEqual(_elementId);

                _result.ItemName.ShouldEqual(_newItemName);
                _result.TechnicalName.ShouldEqual(_newTechnicalName);
                _result.Definition.ShouldEqual(_newDefinition);
                _result.ItemDataType.ShouldEqual(_newItemDataType);
                _result.DataTypeSource.ShouldEqual(_newDataTypeSource);
                _result.FieldLength.ShouldEqual(_newFieldLength);
                _result.ItemUrl.ShouldEqual(_newItemUrl);
            }

            [Test]
            public void Should_update_element()
            {
                var dbContext = CreateDbContext();

                var element = dbContext.SystemItems.First(x => x.SystemItemId == _elementId);
                element.SystemItemId.ShouldNotEqual(Guid.Empty);
                element.ItemName.ShouldEqual(_newItemName);
                element.TechnicalName.ShouldEqual(_newTechnicalName);
                element.Definition.ShouldEqual(_newDefinition);
                element.ItemDataType.ShouldEqual(_newItemDataType);
                element.DataTypeSource.ShouldEqual(_newDataTypeSource);
                element.FieldLength.ShouldEqual(_newFieldLength);
                element.ItemUrl.ShouldEqual(_newItemUrl);
                element.ParentSystemItemId.ShouldEqual(_entityId);
                element.ItemType.ShouldEqual(ItemType.Element);
                element.IsActive.ShouldBeTrue();
            }
        }
    }
}