// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI.Components
{
    public class ComponentField : VisualElement
    {
        public enum Size
        {
            None,
            Large,
            Small
        }

        public static Color backgroundColor => EditorColors.Default.FieldBackground;
        public static Font fieldNameFont => EditorFonts.Inter.Medium;

        public TemplateContainer templateContainer { get; }
        public VisualElement layoutContainer { get; }
        public Image fieldIcon { get; }
        public VisualElement nameAndFieldContainer { get; }
        public Label fieldNameLabel { get; }
        public VisualElement fieldContent { get; }

        private Size m_CurrentSize = Size.None;

        public ComponentField(Size componentSize, string fieldName, VisualElement field, Texture2D icon = null)
        {
            Add(templateContainer = EditorLayouts.EditorUI.ComponentField.CloneTree());

            templateContainer
                .AddStyle(EditorStyles.EditorUI.ComponentField);

            layoutContainer =  templateContainer.Q<VisualElement>("LayoutContainer");
            fieldIcon = layoutContainer.Q<Image>("FieldIcon");
            nameAndFieldContainer = layoutContainer.Q<VisualElement>("NameAndFieldContainer");
            fieldNameLabel = layoutContainer.Q<Label>("FieldName");
            fieldContent = layoutContainer.Q<VisualElement>("FieldContent");
            
            fieldContent.AddChild(field);

            layoutContainer.SetStyleBackgroundColor(backgroundColor);
            
            fieldIcon.SetStyleDisplay(icon == null ? DisplayStyle.None : DisplayStyle.Flex)
                .SetStyleBackgroundImage(icon)
                .SetStyleBackgroundImageTintColor(EditorColors.Default.FieldIcon);
            
            fieldIcon.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -20));

            fieldNameLabel.SetStyleDisplay(fieldName.IsNullOrEmpty() ? DisplayStyle.None : DisplayStyle.Flex);
            fieldNameLabel .SetText(fieldName)
                .SetStyleUnityFont(fieldNameFont);
            
            field
                .SetStyleMargins(0)
                .SetStyleFlexGrow(1);


            SetStyle(componentSize);
        }

        private void Refresh()
        {
            nameAndFieldContainer.SetStyleMarginLeft(GetNameAndFieldContainerMarginLeft());
        }

        private int GetNameAndFieldContainerMarginLeft()
        {
            if (fieldIcon.GetStyleBackgroundImageTexture2D() == null) return 0;
            switch (m_CurrentSize)
            {
                case Size.Large: return 60;
                case Size.Small: return 40;
                case Size.None: return 0;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private void SetStyle(Size size)
        {
            if (size == m_CurrentSize) return;

            string currentClassName = m_CurrentSize.ToString();

            fieldIcon.RemoveClass(currentClassName);
            nameAndFieldContainer.RemoveClass(currentClassName);
            fieldNameLabel.RemoveClass(currentClassName);

            if (size == Size.None)
            {
                m_CurrentSize = size;
                Refresh();
                return;
            }

            string newClassName = size.ToString();
            fieldIcon.AddClass(newClassName);
            nameAndFieldContainer.AddClass(newClassName);
            fieldNameLabel.AddClass(newClassName);

            m_CurrentSize = size;
            Refresh();
        }
    }
}
