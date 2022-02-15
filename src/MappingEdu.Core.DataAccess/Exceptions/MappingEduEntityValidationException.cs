// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;

namespace MappingEdu.Core.DataAccess.Exceptions
{
    [Serializable]
    public class MappingEduEntityValidationException : DbEntityValidationException
    {
        public MappingEduValidationError[] DbValidationErrors
        {
            get
            {
                return EntityValidationErrors.SelectMany(err => err.ValidationErrors,
                    (err, obj) => new MappingEduValidationError
                    {
                        ErrorMessage = obj.ErrorMessage,
                        PropertyName = obj.PropertyName
                    }).ToArray();
            }
        }

        public override string Message
        {
            get
            {
                var newMessage = new StringBuilder();
                newMessage.AppendLine(base.Message);

                foreach (var entityValidationError in EntityValidationErrors)
                {
                    if (newMessage.Length > 0)
                        newMessage.AppendLine();

                    var entityType = entityValidationError.Entry.Entity.GetType();
                    var entityTypeName = entityType.Name;
                    if (entityTypeName.Contains("_") && entityType.BaseType != null)
                        entityTypeName = entityType.BaseType.Name;

                    newMessage.AppendFormat("Entity of type '{0}' in state '{1}' has the following validation errors:", entityTypeName, entityValidationError.Entry.State);

                    foreach (var validationError in entityValidationError.ValidationErrors)
                    {
                        newMessage.AppendLine();
                        if (!string.IsNullOrWhiteSpace(validationError.PropertyName))
                            newMessage.AppendFormat("{0}: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        else
                            newMessage.AppendFormat("{0}", validationError.ErrorMessage);
                    }
                }
                return newMessage.ToString();
            }
        }

        public MappingEduEntityValidationException(DbEntityValidationException exception)
            : base(exception.Message, exception.EntityValidationErrors, exception)
        {
        }
    }

    /// <summary>
    ///     This class is used to provide "pretty" serialization properties.
    /// </summary>
    [Serializable]
    public class MappingEduValidationError
    {
        public string ErrorMessage;
        public string PropertyName;
    }
}