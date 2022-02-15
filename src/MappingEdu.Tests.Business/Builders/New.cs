// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Tests.Business.Builders
{
    public static class New
    {
        public static CustomDetailMetadataBuilder CustomDetailMetadata
        {
            get { return new CustomDetailMetadataBuilder(); }
        }

        public static EnumerationItemMapBuilder EnumerationItemMap
        {
            get { return new EnumerationItemMapBuilder(); }
        }

        public static MapNoteBuilder MapNote
        {
            get { return new MapNoteBuilder(); }
        }

        public static MappedSystemBuilder MappedSystem
        {
            get { return new MappedSystemBuilder(); }
        }

        public static MappingProjectBuilder MappingProject
        {
            get { return new MappingProjectBuilder(); }
        }

        public static NoteBuilder Note
        {
            get { return new NoteBuilder(); }
        }

        public static SystemEnumerationItemBuilder SystemEnumerationItem
        {
            get { return new SystemEnumerationItemBuilder(); }
        }

        public static SystemEnumerationItemRuleBuilder SystemEnumerationItemRule
        {
            get { return new SystemEnumerationItemRuleBuilder(); }
        }

        public static SystemItemBuilder SystemItem
        {
            get { return new SystemItemBuilder(); }
        }

        public static SystemItemCustomDetailBuilder SystemItemCustomDetail
        {
            get { return new SystemItemCustomDetailBuilder(); }
        }

        public static SystemItemMapBuilder SystemItemMap
        {
            get { return new SystemItemMapBuilder(); }
        }

        public static SystemItemRuleBuilder SystemItemRule
        {
            get { return new SystemItemRuleBuilder(); }
        }

        public static SystemItemVersionDeltaBuilder SystemItemVersionDelta
        {
            get { return new SystemItemVersionDeltaBuilder(); }
        }
    }
}