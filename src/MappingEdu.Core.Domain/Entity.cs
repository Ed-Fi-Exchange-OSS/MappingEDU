// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Core.Domain
{
    public abstract class Entity : IEntity
    {
        public string CreateBy { get; set; }

        public bool HasId
        {
            get { return Id != default(Guid); }
        }

        protected abstract Guid Id { get; }

        public Guid? CreateById { get; set; }

        public string UpdateBy { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public Guid? UpdateById { get; set; }

        protected bool Equals(Entity other)
        {
            if (Id == default(Guid))
                return false;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Entity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}