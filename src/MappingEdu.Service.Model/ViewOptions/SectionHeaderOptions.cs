// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Service.Model.ViewOptions
{
    public class SectionHeaderOptions
    {
        public string DataTarget { get; set; }

        public string EditButtonDataBinding { get; set; }

        public string SectionBind { get; set; }

        public string SectionTitle { get; set; }

        public string SectionTitleId { get; set; }

        public bool ShouldDataBindEditButton
        {
            get { return !string.IsNullOrWhiteSpace(EditButtonDataBinding); }
        }

        private bool ShouldIncludeSectionTitleId
        {
            get { return !string.IsNullOrWhiteSpace(SectionTitleId); }
        }

        public string GetIdAttributeMarkup()
        {
            var markup = ShouldIncludeSectionTitleId ? string.Format(" id=\"{0}\"", SectionTitleId) : string.Empty;
            return markup;
        }

        //public ISectionMarker TestMarker { get; set; }

        //public string HeaderTestCss
        //{
        //    get { return TestMarker != null ? TestMarker.Header.Css() : string.Empty; }

        //}
    }
}