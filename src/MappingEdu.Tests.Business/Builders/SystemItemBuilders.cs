// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.System;
using ItemType = MappingEdu.Core.Domain.Enumerations.ItemType;

namespace MappingEdu.Tests.Business.Builders
{
    public class SystemItemBuilder
    {
        private readonly SystemItem _systemItem;

        public SystemItemBuilder AsDomain
        {
            get { return WithType(ItemType.Domain); }
        }

        public SystemItemBuilder AsElement
        {
            get { return WithType(ItemType.Element); }
        }

        public SystemItemBuilder AsEntity
        {
            get { return WithType(ItemType.Entity); }
        }

        public SystemItemBuilder AsEnumeration
        {
            get { return WithType(ItemType.Enumeration); }
        }

        public SystemItemBuilder AsSubEntity
        {
            get { return WithType(ItemType.SubEntity); }
        }

        public SystemItemBuilder()
        {
            _systemItem = BuildDefault();
        }

        private static SystemItem BuildDefault()
        {
            return new SystemItem {SystemItemId = Guid.NewGuid()};
        }

        public SystemItemBuilder WithId(Guid id)
        {
            _systemItem.SystemItemId = id;
            return this;
        }

        public SystemItemBuilder WithName(string name)
        {
            _systemItem.ItemName = name;
            return this;
        }

        public SystemItemBuilder WithDefinition(string definition)
        {
            _systemItem.Definition = definition;
            return this;
        }

        public SystemItemBuilder WithType(ItemType type)
        {
            _systemItem.ItemType = type;
            return this;
        }

        public SystemItemBuilder WithUrl(string url)
        {
            _systemItem.ItemUrl = url;
            return this;
        }

        public SystemItemBuilder WithMappedSystem(MappedSystem mappedSystem)
        {
            _systemItem.MappedSystem = mappedSystem;
            _systemItem.MappedSystemId = mappedSystem.MappedSystemId;
            mappedSystem.SystemItems.Add(_systemItem);
            return this;
        }

        public SystemItemBuilder WithParentSystemItem(SystemItem parentItem)
        {
            parentItem.ChildSystemItems.Add(_systemItem);
            _systemItem.ParentSystemItem = parentItem;
            _systemItem.ParentSystemItemId = parentItem.SystemItemId;
            return this;
        }

        public SystemItemBuilder WithChildSystemItem(SystemItem childItem)
        {
            _systemItem.ChildSystemItems.Add(childItem);
            childItem.ParentSystemItem = _systemItem;
            childItem.ParentSystemItemId = _systemItem.SystemItemId;
            return this;
        }

        public SystemItemBuilder WithPreviousSystemItemVersion(SystemItemVersionDelta previousItem)
        {
            _systemItem.PreviousSystemItemVersionDeltas.Add(previousItem);
            return this;
        }

        public SystemItemBuilder WithNextSystemItemVersion(SystemItemVersionDelta nextItem)
        {
            _systemItem.NextSystemItemVersionDeltas.Add(nextItem);
            return this;
        }

        public SystemItemBuilder WithEnumerationItem(SystemEnumerationItem enumerationItem)
        {
            _systemItem.SystemEnumerationItems.Add(enumerationItem);
            enumerationItem.SystemItem = _systemItem;
            enumerationItem.SystemItemId = _systemItem.SystemItemId;
            return this;
        }

        public static implicit operator SystemItem(SystemItemBuilder builder)
        {
            return builder._systemItem;
        }

        public SystemItemBuilder WithMinimalPersistenceProperties(MappedSystem mappedSystem, string itemName, string definition)
        {
            WithMappedSystem(mappedSystem);
            _systemItem.ItemName = itemName;
            _systemItem.Definition = definition;
            _systemItem.IsActive = true;
            return this;
        }

        public SystemItemBuilder IsActive(bool isActive)
        {
            _systemItem.IsActive = isActive;
            return this;
        }

        public SystemItemBuilder IsActive()
        {
            _systemItem.IsActive = true;
            return this;
        }

        public SystemItemBuilder NotActive()
        {
            _systemItem.IsActive = false;
            return this;
        }
    }
}