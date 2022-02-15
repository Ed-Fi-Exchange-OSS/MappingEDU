// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;

namespace MappingEdu.Service.Model.ViewOptions
{
    public class AddDialogOptions
    {
        private readonly List<string> _cssClasses = new List<string>();

        public string CancelButtonText { get; private set; }

        public string CssAdditions
        {
            get { return string.Join(" ", _cssClasses); }
        }

        public string DomIdPropertyName { get; private set; }

        public string ErrorCollectionProperty { get; private set; }

        public ErrorPanelBindingOptions ErrorPanelOptions
        {
            get { return Options.ErrorPanel.Visible(ErrorVisibleProperty).Collection(ErrorCollectionProperty); }
        }

        public string ErrorVisibleProperty { get; private set; }

        public string OkButtonText { get; private set; }

        public string TemplateName { get; private set; }

        public string TitleText { get; private set; }

        public AddDialogOptions DomId(string propertyName)
        {
            DomIdPropertyName = propertyName;
            return this;
        }

        public AddDialogOptions Title(string text)
        {
            TitleText = text;
            return this;
        }

        public AddDialogOptions Template(string name)
        {
            TemplateName = name;
            return this;
        }

        public AddDialogOptions OkButton(string buttonText)
        {
            OkButtonText = buttonText;
            return this;
        }

        public AddDialogOptions CancelButton(string buttonText)
        {
            CancelButtonText = buttonText;
            return this;
        }

        public AddDialogOptions ErrorVisible(string property)
        {
            ErrorVisibleProperty = property;
            return this;
        }

        public AddDialogOptions ErrorCollection(string property)
        {
            ErrorCollectionProperty = property;
            return this;
        }

        public AddDialogOptions AddCssClass(string css)
        {
            _cssClasses.Add(css);
            return this;
        }
    }
}