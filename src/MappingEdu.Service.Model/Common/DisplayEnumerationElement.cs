// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Service.Model.Common
{
    public class DisplayEnumerationElement
    {
        public string DisplayText { get; set; }

        public object Id { get; set; }

        protected bool Equals(DisplayEnumerationElement other)
        {
            return Equals(Id, other.Id) && string.Equals(DisplayText, other.DisplayText);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DisplayEnumerationElement) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Id != null ? Id.GetHashCode() : 0)*397) ^ (DisplayText != null ? DisplayText.GetHashCode() : 0);
            }
        }
    }

    public class DisplayEnumeration
    {
        public string DisplayText { get; set; }

        public int Id { get; set; }
    }
}