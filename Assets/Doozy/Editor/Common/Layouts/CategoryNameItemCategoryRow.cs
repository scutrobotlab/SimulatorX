// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Common.Layouts
{
    public class CategoryNameItemCategoryRow : VisualElement
    {
        public TemplateContainer templateContainer { get; }
        public VisualElement layoutContainer { get; }
        public Label nameLabel { get; }
        public VisualElement buttonsContainer { get; }

        public static Font font => EditorFonts.Ubuntu.Regular;
        private static Color textColor => EditorColors.Default.TextDescription;
        
        public FluidButton buttonRemoveCategory { get; }
        
        public string target { get; private set; }
        
        public UnityAction<string> removeHandler { get; set; }
        
        public CategoryNameItemCategoryRow()
        {
            this.SetStyleFlexShrink(0);
            
            Add(templateContainer = EditorLayouts.Common.CategoryNameItemCategoryRow.CloneTree());

            templateContainer
                .SetStyleFlexGrow(1)
                .AddStyle(EditorStyles.Common.CategoryNameItemCategoryRow);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            nameLabel = layoutContainer.Q<Label>(nameof(nameLabel));
            buttonsContainer = layoutContainer.Q<VisualElement>(nameof(buttonsContainer));

            nameLabel.SetStyleColor(textColor);
            nameLabel.SetStyleUnityFont(font);

            buttonRemoveCategory = NewButtonRemoveCategory();
            
            buttonRemoveCategory.SetOnClick(() =>
            {
                if (removeHandler == null) throw new NullReferenceException(nameof(removeHandler));
                removeHandler.Invoke(target);
            });
            
            buttonsContainer.Add(buttonRemoveCategory);
        }

        public void Reset()
        {
            target = string.Empty;
            nameLabel.text = string.Empty;
        }
        
        public CategoryNameItemCategoryRow SetTarget(string category)
        {
            target = category;
            nameLabel.text = category;
            return this;
        }
        
        public CategoryNameItemCategoryRow SetRemoveHandler(UnityAction<string> removeCallback)
        {
            removeHandler = removeCallback;
            return this;
        }
        
        public CategoryNameItemCategoryRow ShowRemoveCategoryButton()
        {
            buttonRemoveCategory.SetStyleDisplay(DisplayStyle.Flex);
            return this;
        }
        
        public CategoryNameItemCategoryRow HideRemoveCategoryButton()
        {
            buttonRemoveCategory.SetStyleDisplay(DisplayStyle.None);
            return this;
        }
        
        private static FluidButton NewButtonRemoveCategory() =>
            FluidButton.Get()
                .SetElementSize(ElementSize.Small)
                .SetIcon(EditorMicroAnimations.EditorUI.Icons.Minus)
                .SetAccentColor(EditorSelectableColors.Default.Remove)
                .SetTooltip("Remove Category");
    }
}
