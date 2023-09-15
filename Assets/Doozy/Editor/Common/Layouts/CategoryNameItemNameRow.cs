// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Runtime.Common;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Local

namespace Doozy.Editor.Common.Layouts
{
    public class CategoryNameItemNameRow : PoolableElement<CategoryNameItemNameRow>
    {
        public TemplateContainer templateContainer { get; }
        public VisualElement layoutContainer { get; }
        public VisualElement leftContainer { get; }
        public VisualElement middleContainer { get; }
        public Label nameLabel { get; }
        public TextField nameTextField { get; }
        public VisualElement rightContainer { get; }

        public FluidToggleButtonTab buttonRename { get; }
        public FluidButton buttonSave { get; }
        public FluidButton buttonCancel { get; }
        public FluidButton buttonRemoveItem { get; }

        private static Color layoutContainerNormalColor => EditorColors.Default.Background;
        private static Color layoutContainerHoverColor => EditorColors.Default.WindowHeaderBackground;
        private static Color initialContainerColor => layoutContainerNormalColor;
        private static Color textColor => EditorColors.Default.TextDescription;

        public static Font font => EditorFonts.Ubuntu.Light;

        public CategoryNameItem target { get; private set; }

        public UnityAction<CategoryNameItem> removeHandler { get; set; }
        public Func<CategoryNameItem, string, bool> saveHandler { get; set; }

        public static CategoryNameItemNameRow Get(CategoryNameItem categoryNameItem, Func<CategoryNameItem, string, bool> saveCallback, UnityAction<CategoryNameItem> removeCallback) =>
            Get().SetTarget(categoryNameItem).SetSaveHandler(saveCallback).SetRemoveHandler(removeCallback);

        public CategoryNameItemNameRow()
        {
            this.SetStyleFlexShrink(0);

            Add(templateContainer = EditorLayouts.Common.CategoryNameItemNameRow.CloneTree());

            templateContainer
                .SetStyleFlexGrow(1)
                .AddStyle(EditorStyles.Common.CategoryNameItemNameRow);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            leftContainer = layoutContainer.Q<VisualElement>(nameof(leftContainer));
            middleContainer = layoutContainer.Q<VisualElement>(nameof(middleContainer));
            nameLabel = middleContainer.Q<Label>(nameof(nameLabel));
            nameTextField = middleContainer.Q<TextField>(nameof(nameTextField));
            rightContainer = layoutContainer.Q<VisualElement>(nameof(rightContainer));

            layoutContainer.SetStyleBackgroundColor(initialContainerColor);
            // layoutContainer.RegisterCallback<MouseEnterEvent>(evt => layoutContainer.SetStyleBackgroundColor(layoutContainerHoverColor));
            // layoutContainer.RegisterCallback<MouseLeaveEvent>(evt => layoutContainer.SetStyleBackgroundColor(layoutContainerNormalColor));

            nameLabel.SetStyleColor(textColor);
            nameLabel.SetStyleUnityFont(font);

            buttonRename = NewButtonRename();
            buttonSave = NewButtonSave().SetStyleDisplay(DisplayStyle.None);
            buttonCancel = NewButtonCancel().SetStyleDisplay(DisplayStyle.None);
            buttonRemoveItem = NewButtonRemoveItem();

            buttonRename.SetOnValueChanged(change =>
            {
                if (change.newValue)
                {
                    nameLabel.SetStyleDisplay(DisplayStyle.None);
                    buttonRemoveItem.SetStyleDisplay(DisplayStyle.None);
                    nameTextField.SetStyleDisplay(DisplayStyle.Flex);
                    buttonSave.SetStyleDisplay(DisplayStyle.Flex);
                    buttonCancel.SetStyleDisplay(DisplayStyle.Flex);
                    return;
                }

                nameLabel.SetStyleDisplay(DisplayStyle.Flex);
                buttonRemoveItem.SetStyleDisplay(DisplayStyle.Flex);
                nameTextField.SetStyleDisplay(DisplayStyle.None);
                buttonSave.SetStyleDisplay(DisplayStyle.None);
                buttonCancel.SetStyleDisplay(DisplayStyle.None);
            });

            buttonSave.SetOnClick(() =>
            {
                if (saveHandler == null) throw new NullReferenceException(nameof(saveHandler));
                bool result = saveHandler.Invoke(target, nameTextField.value);
                if (!result) return;
                buttonRename.SetIsOn(false);
            });

            buttonCancel.SetOnClick(() => buttonRename.SetIsOn(false));

            buttonRemoveItem.SetOnClick(() =>
            {
                if (removeHandler == null) throw new NullReferenceException(nameof(removeHandler));
                removeHandler.Invoke(target);
            });

            leftContainer
                .AddChild(buttonRename);

            rightContainer
                .AddChild(buttonSave)
                .AddChild(buttonCancel)
                .AddChild(buttonRemoveItem);
        }

        public override void Reset()
        {
            target = null;
            nameLabel.text = string.Empty;
            nameTextField.value = string.Empty;

            buttonRename.isOn = false;

            removeHandler = null;
            saveHandler = null;

            nameLabel.SetStyleDisplay(DisplayStyle.Flex);
            buttonRemoveItem.SetStyleDisplay(DisplayStyle.Flex);
            nameTextField.SetStyleDisplay(DisplayStyle.None);
            buttonSave.SetStyleDisplay(DisplayStyle.None);
            buttonCancel.SetStyleDisplay(DisplayStyle.None);
        }

        public CategoryNameItemNameRow SetTarget(CategoryNameItem categoryNameItem)
        {
            Reset();
            target = categoryNameItem;
            nameLabel.text = target.name;
            nameTextField.value = target.name;
            SetEnabled(!name.Equals(CategoryNameItem.k_DefaultName));
            return this;
        }

        public CategoryNameItemNameRow SetSaveHandler(Func<CategoryNameItem, string, bool> saveCallback)
        {
            saveHandler = saveCallback;
            return this;
        }

        public CategoryNameItemNameRow SetRemoveHandler(UnityAction<CategoryNameItem> removeCallback)
        {
            removeHandler = removeCallback;
            return this;
        }

        private static FluidToggleButtonTab NewButtonRename() =>
            FluidToggleButtonTab.Get()
                .SetElementSize(ElementSize.Tiny)
                .SetIcon(EditorMicroAnimations.EditorUI.Icons.Edit)
                .SetToggleAccentColor(EditorSelectableColors.Default.Action)
                .SetTooltip("Rename");

        private static FluidButton NewButtonSave() =>
            FluidButton.Get()
                .SetElementSize(ElementSize.Tiny)
                .SetIcon(EditorMicroAnimations.EditorUI.Icons.Save)
                .SetAccentColor(EditorSelectableColors.Default.Add)
                .SetTooltip("Save");

        private static FluidButton NewButtonCancel() =>
            FluidButton.Get()
                .SetElementSize(ElementSize.Tiny)
                .SetIcon(EditorMicroAnimations.EditorUI.Icons.Close)
                .SetAccentColor(EditorSelectableColors.Default.Remove)
                .SetTooltip("Cancel");

        private static FluidButton NewButtonRemoveItem() =>
            FluidButton.Get()
                .SetElementSize(ElementSize.Tiny)
                .SetIcon(EditorMicroAnimations.EditorUI.Icons.Minus)
                .SetAccentColor(EditorSelectableColors.Default.Remove)
                .SetTooltip("Remove");

    }
}
