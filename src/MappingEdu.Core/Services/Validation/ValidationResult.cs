// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Common.Extensions;

namespace MappingEdu.Core.Services.Validation
{
    public class ValidationResult
    {
        public IValidationFailure[] Failures { get; private set; }

        public bool IsValid
        {
            get { return Failures.Length == 0; }
        }

        public ValidationRuleDescription[] ValidationRuleDescriptions { get; set; }

        public ValidationResult()
        {
            Failures = new IValidationFailure[0];
        }

        public ValidationResult(IValidationFailure[] validationFailures, ValidationRuleDescription[] validationRuleDescriptions)
        {
            Failures = validationFailures;
            ValidationRuleDescriptions = validationRuleDescriptions;
        }

        public FailureWithCount[] UniqueFailureTypesAndCount()
        {
            return Failures.Select(x => x.FailingRule).Distinct().Select(y =>
            {
                var count = Failures.Count(z => z.FailingRule == y);
                return new FailureWithCount {FailingRule = y, Count = count};
            }).OrderByDescending(k => k.Count).ToArray();
        }

        public void ClearFailures()
        {
            Failures = new IValidationFailure[0];
        }

        public void AddFailure(IValidationFailure failure)
        {
            Failures = Failures.UnionElement(failure).ToArray();
        }

        public class FailureWithCount
        {
            public int Count { get; set; }

            public Type FailingRule { get; set; }

            public override string ToString()
            {
                return string.Format("{0}: {1}", Count, FailingRule.Name.SpaceCamelHumps());
            }
        }
    }
}