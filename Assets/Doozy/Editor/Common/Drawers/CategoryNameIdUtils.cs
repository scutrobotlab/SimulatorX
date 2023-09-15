// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Events;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Common;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
namespace Doozy.Editor.Common.Drawers
{
    public static class CategoryNameIdUtils
    {
        public static VisualElement CreateDrawer
        (
            SerializedProperty property,
            Func<IEnumerable<string>> getCategories,
            Func<string, IEnumerable<string>> getNames,
            IEnumerable<Texture2D> openDatabaseButtonIconTextures,
            UnityAction openDatabaseButtonCallback,
            string openDatabaseButtonTooltip,
            IUpdateCallback databaseUpdateCallback,
            EditorSelectableColorInfo selectableAccentColor
        )
        {
            VisualElement drawer =
                DesignUtils.row
                    .SetName("drawer");

            SerializedProperty category = property.FindPropertyRelative(nameof(CategoryNameId.Category));
            SerializedProperty name = property.FindPropertyRelative(nameof(CategoryNameId.Name));
            SerializedProperty custom = property.FindPropertyRelative(nameof(CategoryNameId.Custom));

            var categories = new List<string> { CategoryNameId.defaultCategory };
            PopupField<string> categoryPopup =
                new PopupField<string>(categories, CategoryNameId.defaultCategory)
                    .SetName(nameof(categoryPopup))
                    .ResetLayout()
                    .SetTooltip("Category");

            void RefreshCategories()
            {
                categories.Clear();
                categories.AddRange(getCategories.Invoke());
            }

            var names = new List<string> { CategoryNameId.defaultName };
            PopupField<string> namePopup =
                new PopupField<string>(names, CategoryNameId.defaultName)
                    .SetName(nameof(namePopup))
                    .ResetLayout()
                    .SetTooltip("Name");

            void RefreshNames(string targetCategory)
            {
                names.Clear();
                names.AddRange(getNames.Invoke(targetCategory));
            }

            FluidButton openDatabaseButton =
                NewOpenDatabaseButton()
                    .SetIcon(openDatabaseButtonIconTextures)
                    .SetAccentColor(selectableAccentColor)
                    .SetOnClick(openDatabaseButtonCallback)
                    .SetTooltip(openDatabaseButtonTooltip);

            TextField categoryTextField =
                DesignUtils.NewTextField(category, true)
                    .ResetLayout()
                    .SetStyleFlexGrow(1);

            TextField nameTextField =
                DesignUtils.NewTextField(name, true)
                    .ResetLayout()
                    .SetStyleFlexGrow(1);

            FluidToggleSwitch customSwitch =
                NewCustomSwitch()
                    .BindToProperty(custom)
                    .SetToggleAccentColor(selectableAccentColor);

            bool hasCustom = true;
            RefreshCategories();
            if (categories.Contains(category.stringValue))
            {
                RefreshNames(category.stringValue);
                if (names.Contains(name.stringValue))
                {
                    hasCustom = false;
                }
            }

            if (custom.boolValue != hasCustom)
            {
                custom.boolValue = hasCustom;
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }

            if (!hasCustom)
            {
                RefreshNames(category.stringValue);
                categoryPopup.SetValueWithoutNotify(category.stringValue);
                namePopup.SetValueWithoutNotify(name.stringValue);
            }

            categoryPopup.SetStyleDisplay(hasCustom ? DisplayStyle.None : DisplayStyle.Flex);
            namePopup.SetStyleDisplay(hasCustom ? DisplayStyle.None : DisplayStyle.Flex);
            categoryTextField.SetStyleDisplay(hasCustom ? DisplayStyle.Flex : DisplayStyle.None);
            nameTextField.SetStyleDisplay(hasCustom ? DisplayStyle.Flex : DisplayStyle.None);

            categoryPopup.RegisterValueChangedCallback(evt =>
            {
                RefreshNames(evt.newValue);
                namePopup.SetValueWithoutNotify(names[0]);

                categoryTextField.value = evt.newValue;
                nameTextField.value = names[0];
                property.serializedObject.ApplyModifiedProperties();
            });

            namePopup.RegisterValueChangedCallback(evt =>
            {
                nameTextField.value = evt.newValue;
                property.serializedObject.ApplyModifiedProperties();
            });

            drawer.schedule.Execute(() => customSwitch.OnValueChanged = OnCustomValueChanged).ExecuteLater(100);

            void OnCustomValueChanged(FluidBoolEvent evt)
            {
                openDatabaseButton.iconReaction?.Play();
                categoryPopup.SetStyleDisplay(evt.newValue ? DisplayStyle.None : DisplayStyle.Flex);
                namePopup.SetStyleDisplay(evt.newValue ? DisplayStyle.None : DisplayStyle.Flex);
                categoryTextField.SetStyleDisplay(evt.newValue ? DisplayStyle.Flex : DisplayStyle.None);
                nameTextField.SetStyleDisplay(evt.newValue ? DisplayStyle.Flex : DisplayStyle.None);

                if (evt.newValue) //Custom ENABLED --> stop here
                {
                    RefreshCategories();
                    categoryPopup.SetValueWithoutNotify(CategoryNameId.defaultCategory);

                    RefreshNames(CategoryNameId.defaultCategory);
                    namePopup.SetValueWithoutNotify(CategoryNameId.defaultName);
                    return;
                }

                //Custom DISABLED --> update popups
                RefreshCategories();
                if (!categories.Contains(category.stringValue))
                {
                    categoryPopup.SetValueWithoutNotify(CategoryNameId.defaultCategory);
                    categoryTextField.value = CategoryNameId.defaultCategory;

                    RefreshNames(CategoryNameId.defaultCategory);
                    namePopup.SetValueWithoutNotify(CategoryNameId.defaultName);
                    nameTextField.value = CategoryNameId.defaultName;

                    property.serializedObject.ApplyModifiedProperties();
                    return;
                }

                RefreshNames(category.stringValue);
                namePopup.SetValueWithoutNotify(names.Contains(name.stringValue) ? name.stringValue : names[0]);
                categoryPopup.SetValueWithoutNotify(category.stringValue);

                categoryTextField.value = categoryPopup.value;
                nameTextField.value = namePopup.value;
                property.serializedObject.ApplyModifiedProperties();
            }

            void Refresh()
            {
                if (property.serializedObject.targetObject == null)
                    return;

                if (custom.boolValue) //Custom ENABLED --> stop here 
                    return;

                //Custom DISABLED --> update popups
                RefreshCategories();
                if (!categories.Contains(category.stringValue))
                {
                    categoryPopup.SetValueWithoutNotify(CategoryNameId.defaultCategory);
                    categoryTextField.value = CategoryNameId.defaultCategory;

                    RefreshNames(CategoryNameId.defaultCategory);
                    namePopup.SetValueWithoutNotify(CategoryNameId.defaultName);
                    nameTextField.value = CategoryNameId.defaultName;

                    property.serializedObject.ApplyModifiedProperties();
                    return;
                }

                RefreshNames(category.stringValue);
                namePopup.SetValueWithoutNotify(names.Contains(name.stringValue) ? name.stringValue : names[0]);
                categoryPopup.SetValueWithoutNotify(category.stringValue);

                categoryTextField.value = categoryPopup.value;
                nameTextField.value = namePopup.value;

                property.serializedObject.ApplyModifiedProperties();
            }

            drawer.schedule.Execute(Refresh).Every(200);

            drawer.RegisterCallback<AttachToPanelEvent>(evt => databaseUpdateCallback.AddOnUpdateCallback(Refresh));
            drawer.RegisterCallback<DetachFromPanelEvent>(evt => databaseUpdateCallback.RemoveOnUpdateCallback(Refresh));

            Label GetLabel(string text) =>
                DesignUtils.NewFieldNameLabel(text).SetStyleMarginBottom(2);

            drawer
                .AddChild(openDatabaseButton.SetName("OpenDatabaseButton"))
                .AddChild
                (
                    DesignUtils.column
                        .AddChild(GetLabel("Category"))
                        .AddChild(categoryPopup.SetStyleFlexGrow(1))
                        .AddChild(categoryTextField.SetStyleFlexGrow(1).SetStyleMarginBottom(1))
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    DesignUtils.column
                        .AddChild(GetLabel("Name"))
                        .AddChild(namePopup.SetStyleFlexGrow(1))
                        .AddChild(nameTextField.SetStyleFlexGrow(1).SetStyleMarginBottom(1))
                )
                .AddChild(customSwitch);

            return drawer;
        }

        public static VisualElement CreateView
        (
            Func<string> getCategory,
            UnityAction<string> setCategory,
            Func<string> getName,
            UnityAction<string> setName,
            Func<bool> getCustom,
            UnityAction<bool> setCustom,
            Func<IEnumerable<string>> getCategories,
            Func<string, IEnumerable<string>> getNames,
            IEnumerable<Texture2D> openDatabaseButtonIconTextures,
            UnityAction openDatabaseButtonCallback,
            string openDatabaseButtonTooltip,
            IUpdateCallback databaseUpdateCallback,
            EditorSelectableColorInfo selectableAccentColor
        )
        {
            VisualElement container =
                DesignUtils.row
                    .SetName("container");

            var categories = new List<string> { CategoryNameId.defaultCategory };
            PopupField<string> categoryPopup =
                new PopupField<string>(categories, CategoryNameId.defaultCategory)
                    .SetName(nameof(categoryPopup))
                    .ResetLayout()
                    .SetTooltip("Category");

            void RefreshCategories()
            {
                categories.Clear();
                categories.AddRange(getCategories.Invoke());
            }

            var names = new List<string> { CategoryNameId.defaultName };
            PopupField<string> namePopup =
                new PopupField<string>(names, CategoryNameId.defaultName)
                    .SetName(nameof(namePopup))
                    .ResetLayout()
                    .SetTooltip("Name");

            void RefreshNames(string targetCategory)
            {
                names.Clear();
                names.AddRange(getNames.Invoke(targetCategory));
            }

            FluidButton openDatabaseButton =
                NewOpenDatabaseButton()
                    .SetIcon(openDatabaseButtonIconTextures)
                    .SetAccentColor(selectableAccentColor)
                    .SetOnClick(openDatabaseButtonCallback)
                    .SetTooltip(openDatabaseButtonTooltip);

            TextField categoryTextField =
                new TextField()
                    .SetName("category")
                    .ResetLayout()
                    .SetStyleFlexGrow(1);
            categoryTextField.SetValueWithoutNotify(getCategory.Invoke());
            categoryTextField.RegisterValueChangedCallback(evt => setCategory.Invoke(evt.newValue));

            TextField nameTextField =
                new TextField()
                    .SetName("name")
                    .ResetLayout()
                    .SetStyleFlexGrow(1);
            nameTextField.SetValueWithoutNotify(getName.Invoke());
            nameTextField.RegisterValueChangedCallback(evt => setName.Invoke(evt.newValue));

            FluidToggleSwitch customSwitch =
                NewCustomSwitch()
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetIsOn(getCustom.Invoke(), false);
            customSwitch.SetOnClick(() => setCustom.Invoke(!customSwitch.isOn));

            bool hasCustom = true;
            RefreshCategories();
            if (categories.Contains(getCategory.Invoke()))
            {
                RefreshNames(getCategory.Invoke());
                if (names.Contains(getName.Invoke()))
                {
                    hasCustom = false;
                }
            }

            if (getCustom.Invoke() != hasCustom)
            {
                setCustom.Invoke(hasCustom);
            }

            if (!hasCustom)
            {
                RefreshNames(getCategory.Invoke());
                categoryPopup.SetValueWithoutNotify(getCategory.Invoke());
                namePopup.SetValueWithoutNotify(getName.Invoke());
            }

            categoryPopup.SetStyleDisplay(hasCustom ? DisplayStyle.None : DisplayStyle.Flex);
            namePopup.SetStyleDisplay(hasCustom ? DisplayStyle.None : DisplayStyle.Flex);
            categoryTextField.SetStyleDisplay(hasCustom ? DisplayStyle.Flex : DisplayStyle.None);
            nameTextField.SetStyleDisplay(hasCustom ? DisplayStyle.Flex : DisplayStyle.None);

            categoryPopup.RegisterValueChangedCallback(evt =>
            {
                RefreshNames(evt.newValue);
                namePopup.SetValueWithoutNotify(names[0]);

                categoryTextField.value = evt.newValue;
                nameTextField.value = names[0];
            });

            namePopup.RegisterValueChangedCallback(evt =>
            {
                nameTextField.value = evt.newValue;
            });

            container.schedule.Execute(() => customSwitch.OnValueChanged = OnCustomValueChanged).ExecuteLater(100);

            void OnCustomValueChanged(FluidBoolEvent evt)
            {
                openDatabaseButton.iconReaction?.Play();
                categoryPopup.SetStyleDisplay(evt.newValue ? DisplayStyle.None : DisplayStyle.Flex);
                namePopup.SetStyleDisplay(evt.newValue ? DisplayStyle.None : DisplayStyle.Flex);
                categoryTextField.SetStyleDisplay(evt.newValue ? DisplayStyle.Flex : DisplayStyle.None);
                nameTextField.SetStyleDisplay(evt.newValue ? DisplayStyle.Flex : DisplayStyle.None);

                if (evt.newValue) //Custom ENABLED --> stop here
                {
                    RefreshCategories();
                    categoryPopup.SetValueWithoutNotify(CategoryNameId.defaultCategory);

                    RefreshNames(CategoryNameId.defaultCategory);
                    namePopup.SetValueWithoutNotify(CategoryNameId.defaultName);
                    return;
                }

                //Custom DISABLED --> update popups
                RefreshCategories();
                if (!categories.Contains(getCategory.Invoke()))
                {
                    categoryPopup.SetValueWithoutNotify(CategoryNameId.defaultCategory);
                    categoryTextField.value = CategoryNameId.defaultCategory;

                    RefreshNames(CategoryNameId.defaultCategory);
                    namePopup.SetValueWithoutNotify(CategoryNameId.defaultName);
                    nameTextField.value = CategoryNameId.defaultName;

                    return;
                }

                RefreshNames(getCategory.Invoke());
                namePopup.SetValueWithoutNotify(names.Contains(getName.Invoke()) ? getName.Invoke() : names[0]);
                categoryPopup.SetValueWithoutNotify(getCategory.Invoke());

                categoryTextField.value = categoryPopup.value;
                nameTextField.value = namePopup.value;
            }

            void Refresh()
            {
                if (getCustom.Invoke()) //Custom ENABLED --> stop here 
                    return;

                //Custom DISABLED --> update popups
                RefreshCategories();
                if (!categories.Contains(getCategory.Invoke()))
                {
                    categoryPopup.SetValueWithoutNotify(CategoryNameId.defaultCategory);
                    categoryTextField.value = CategoryNameId.defaultCategory;

                    RefreshNames(CategoryNameId.defaultCategory);
                    namePopup.SetValueWithoutNotify(CategoryNameId.defaultName);
                    nameTextField.value = CategoryNameId.defaultName;

                    return;
                }

                RefreshNames(getCategory.Invoke());
                namePopup.SetValueWithoutNotify(names.Contains(getName.Invoke()) ? getName.Invoke() : names[0]);
                categoryPopup.SetValueWithoutNotify(getCategory.Invoke());

                categoryTextField.value = categoryPopup.value;
                nameTextField.value = namePopup.value;
            }

            container.schedule.Execute(Refresh).Every(200);

            container.RegisterCallback<AttachToPanelEvent>(evt => databaseUpdateCallback.AddOnUpdateCallback(Refresh));
            container.RegisterCallback<DetachFromPanelEvent>(evt => databaseUpdateCallback.RemoveOnUpdateCallback(Refresh));

            Label GetLabel(string text) =>
                DesignUtils.NewFieldNameLabel(text).SetStyleMarginBottom(2);

            container
                .AddChild(openDatabaseButton.SetName("OpenDatabaseButton"))
                .AddChild
                (
                    DesignUtils.column
                        .AddChild(GetLabel("Category"))
                        .AddChild(categoryPopup.SetStyleFlexGrow(1))
                        .AddChild(categoryTextField.SetStyleFlexGrow(1).SetStyleMarginBottom(1))
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    DesignUtils.column
                        .AddChild(GetLabel("Name"))
                        .AddChild(namePopup.SetStyleFlexGrow(1))
                        .AddChild(nameTextField.SetStyleFlexGrow(1).SetStyleMarginBottom(1))
                )
                .AddChild(customSwitch);

            return container;
        }

        private static FluidButton NewOpenDatabaseButton() =>
            FluidButton.Get()
                .SetButtonStyle(ButtonStyle.Clear)
                .SetElementSize(ElementSize.Normal)
                .SetStyleAlignSelf(Align.Center)
                .SetStyleMarginRight(DesignUtils.k_Spacing * 2);

        private static FluidToggleSwitch NewCustomSwitch() =>
            FluidToggleSwitch.Get("Custom")
                .SetStyleAlignSelf(Align.FlexEnd)
                .SetStyleMarginBottom(-2)
                .SetStyleMarginLeft(DesignUtils.k_Spacing * 2);
    }
}
