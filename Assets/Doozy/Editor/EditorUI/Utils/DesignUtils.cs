// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Doozy.Editor.EditorUI.Utils
{
    public static class DesignUtils
    {
        private static Font s_defaultFont;
        public static Font unityDefaultFont => s_defaultFont ? s_defaultFont : s_defaultFont = new VisualElement().GetStyleUnityFont();

        public const int k_FieldLabelFontSize = 9;
        public const int k_NormalLabelFontSize = 11;
        public const int k_Spacing = 4;
        public const int k_Spacing2X = k_Spacing * 2;
        public const int k_Spacing3X = k_Spacing * 3;
        public const int k_Spacing4X = k_Spacing * 4;
        public const int k_EndOfLineSpacing = k_Spacing * 6;
        public const int k_ToolbarHeight = 32;

        public static VisualElement GetToolbarContainer() =>
            row
                .SetStyleHeight(k_ToolbarHeight)
                .SetStylePaddingLeft(k_Spacing)
                .SetStylePaddingRight(k_Spacing)
                .SetStyleAlignItems(Align.Center)
                .SetStyleJustifyContent(Justify.FlexEnd)
                .SetStyleBackgroundColor(EditorColors.Default.BoxBackground);
        
        public static VisualElement GetSpaceBlock(int size, string name = "") =>
            GetSpaceBlock(size, size, name);

        public static VisualElement GetSpaceBlock(int width, int height, string name = "") =>
            new VisualElement()
                .SetName($"{name} Space Block ({width}x{height})")
                .SetStyleWidth(width)
                .SetStyleHeight(height)
                .SetStyleAlignSelf(Align.Center)
                .SetStyleFlexShrink(0);

        public static VisualElement endOfLineBlock => GetSpaceBlock(0, k_EndOfLineSpacing, "End of Line");
        public static VisualElement spaceBlock => GetSpaceBlock(k_Spacing, k_Spacing);
        public static VisualElement spaceBlock2X => GetSpaceBlock(k_Spacing2X, k_Spacing2X);
        public static VisualElement spaceBlock3X => GetSpaceBlock(k_Spacing3X, k_Spacing3X);
        public static VisualElement spaceBlock4X => GetSpaceBlock(k_Spacing4X, k_Spacing4X);

        public const int k_FieldBorderRadius = 6;

        public const int k_FieldNameTiny = 10;
        public const int k_FieldNameSmall = 10;
        public const int k_FieldNameNormal = 11;
        public const int k_FieldNameLarge = 11;

        public static Color placeholderColor => EditorColors.Default.Placeholder;

        public static Color tabButtonColorOff => EditorColors.Default.FieldBackground;
        public static Color fieldBackgroundColor => EditorColors.Default.FieldBackground;
        public static Color fieldIconColor => EditorColors.Default.FieldIcon;
        public static Color fieldNameTextColor => EditorColors.Default.TextSubtitle;
        public static Font fieldNameTextFont => EditorFonts.Ubuntu.Light;

        public static Color dividerColor => EditorColors.Default.FieldBackground;

        public static Color disabledTextColor => EditorColors.Default.Placeholder;

        public static Color callbacksColor => EditorColors.Default.Action;
        public static EditorSelectableColorInfo callbackSelectableColor => EditorSelectableColors.Default.Action;

        /// <summary> Get a new VisualElement as a column </summary>
        public static VisualElement column =>
            new VisualElement().SetName("Column").SetStyleFlexDirection(FlexDirection.Column).SetStyleFlexGrow(1);

        /// <summary> Get a new VisualElement as a row </summary>
        public static VisualElement row =>
            new VisualElement().SetName("Row").SetStyleFlexDirection(FlexDirection.Row).SetStyleFlexGrow(1);

        /// <summary> Get a new VisualElement as empty flexible space </summary>
        public static VisualElement flexibleSpace =>
            new VisualElement()
                .SetName("Flexible Space")
                .SetStyleFlexGrow(1);

        public static VisualElement fieldContainer =>
            new VisualElement()
                .SetStylePadding(k_Spacing)
                .SetStyleBackgroundColor(fieldBackgroundColor)
                .SetStyleBorderRadius(k_FieldBorderRadius);

        public static Label fieldLabel =>
            new Label()
                .ResetLayout()
                .SetStyleUnityFont(fieldNameTextFont)
                .SetStyleFontSize(k_FieldLabelFontSize)
                .SetStyleColor(fieldNameTextColor);

        public static Label NewFieldNameLabel(string text) =>
            fieldLabel
                .SetName($"Label: {text}")
                .SetText(text);

        public static Label NewLabel(string text, int labelFontSize = k_NormalLabelFontSize) =>
            new Label(text)
                .SetName($"Label: {text}")
                .ResetLayout()
                .SetStyleTextAlign(TextAnchor.MiddleLeft)
                .SetStyleFontSize(labelFontSize);

        public static EnumField NewEnumField(SerializedProperty property, bool invisibleField = false) =>
            NewEnumField(property.propertyPath, invisibleField);

        public static EnumField NewEnumField(string bindingPath, bool invisibleField = false) =>
            new EnumField().SetBindingPath(bindingPath)
                .SetName($"Enum: {bindingPath}")
                .ResetLayout()
                .SetStyleFlexShrink(1)
                .SetStyleAlignSelf(Align.Center)
                .SetStyleDisplay(invisibleField ? DisplayStyle.None : DisplayStyle.Flex);

        public static PropertyField NewPropertyField(SerializedProperty property, bool invisibleField = false) =>
            NewPropertyField(property.propertyPath, invisibleField);

        public static PropertyField NewPropertyField(string bindingPath, bool invisibleField = false) =>
            new PropertyField().SetBindingPath(bindingPath)
                .SetName($"Property: {bindingPath}")
                .ResetLayout()
                .SetStyleFlexGrow(1)
                .SetStyleDisplay(invisibleField ? DisplayStyle.None : DisplayStyle.Flex);

        public static ObjectField NewObjectField(SerializedProperty property, Type objectType, bool allowSceneObjects = true, bool invisibleField = false) =>
            NewObjectField(property.propertyPath, objectType, allowSceneObjects, invisibleField);

        public static ObjectField NewObjectField(string bindingPath, Type objectType, bool allowSceneObjects = true, bool invisibleField = false) =>
            new ObjectField
                {
                    bindingPath = bindingPath,
                    objectType = objectType,
                    allowSceneObjects = allowSceneObjects
                }
                .SetName($"Object: {bindingPath}")
                .ResetLayout()
                .SetStyleFlexShrink(1)
                .SetStyleAlignSelf(Align.Center)
                .SetStyleDisplay(invisibleField ? DisplayStyle.None : DisplayStyle.Flex);

        public static IntegerField NewIntegerField(SerializedProperty property, bool invisibleField = false) =>
            NewIntegerField(property.propertyPath, invisibleField);

        public static IntegerField NewIntegerField(string bindingPath, bool invisibleField = false) =>
            new IntegerField().SetBindingPath(bindingPath)
                .SetName($"Int: {bindingPath}")
                .ResetLayout()
                .SetStyleFlexShrink(1)
                .SetStyleAlignSelf(Align.Center)
                .SetStyleDisplay(invisibleField ? DisplayStyle.None : DisplayStyle.Flex);

        public static FloatField NewFloatField(SerializedProperty property, bool invisibleField = false) =>
            NewFloatField(property.propertyPath, invisibleField);

        public static FloatField NewFloatField(string bindingPath, bool invisibleField = false) =>
            new FloatField().SetBindingPath(bindingPath)
                .SetName($"Int: {bindingPath}")
                .ResetLayout()
                .SetStyleFlexShrink(1)
                .SetStyleAlignSelf(Align.Center)
                .SetStyleDisplay(invisibleField ? DisplayStyle.None : DisplayStyle.Flex);

        public static Vector2Field NewVector2Field(SerializedProperty property, bool invisibleField = false) =>
            NewVector2Field(property.propertyPath, invisibleField);

        public static Vector2Field NewVector2Field(string bindingPath, bool invisibleField = false) =>
            new Vector2Field().SetBindingPath(bindingPath)
                .SetName($"Int: {bindingPath}")
                .ResetLayout()
                .SetStyleFlexShrink(1)
                .SetStyleAlignSelf(Align.Center)
                .SetStyleDisplay(invisibleField ? DisplayStyle.None : DisplayStyle.Flex);

        public static Vector3Field NewVector3Field(SerializedProperty property, bool invisibleField = false) =>
            NewVector3Field(property.propertyPath, invisibleField);

        public static Vector3Field NewVector3Field(string bindingPath, bool invisibleField = false) =>
            new Vector3Field().SetBindingPath(bindingPath)
                .SetName($"Int: {bindingPath}")
                .ResetLayout()
                .SetStyleFlexShrink(1)
                .SetStyleAlignSelf(Align.Center)
                .SetStyleDisplay(invisibleField ? DisplayStyle.None : DisplayStyle.Flex);

        public static TextField NewTextField(SerializedProperty property, bool isDelayed = false, bool invisibleField = false) =>
            NewTextField(property.propertyPath, isDelayed, invisibleField);

        public static TextField NewTextField(string bindingPath, bool isDelayed = false, bool invisibleField = false)
        {
            TextField field =
                new TextField().SetBindingPath(bindingPath)
                    .SetName($"Text: {bindingPath}")
                    .ResetLayout()
                    .SetStyleFlexShrink(1)
                    .SetStyleAlignSelf(Align.Center)
                    .SetStyleDisplay(invisibleField ? DisplayStyle.None : DisplayStyle.Flex);

            field.isDelayed = isDelayed;

            return field;
        }

        public static Toggle NewToggle(SerializedProperty property, bool invisibleField = false) =>
            NewToggle(property.propertyPath, invisibleField);

        public static Toggle NewToggle(string bindingPath, bool invisibleField = false) =>
            new Toggle().SetBindingPath(bindingPath)
                .SetName($"Toggle: {bindingPath}")
                .ResetLayout()
                .SetStyleAlignSelf(Align.Center)
                .SetStyleDisplay(invisibleField ? DisplayStyle.None : DisplayStyle.Flex);

        public static FluidToggleButtonTab NameTab() =>
            FluidToggleButtonTab.Get()
                .SetTabPosition(TabPosition.TabOnBottom)
                .SetElementSize(ElementSize.Tiny)
                .SetIcon(EditorMicroAnimations.EditorUI.Icons.Label)
                .SetLabelText("Name")
                .SetContainerColorOff(tabButtonColorOff);

        /// <summary> Fluid button used under headers for in-editor actions </summary>
        public static FluidButton SystemButton(IEnumerable<Texture2D> textures) =>
            FluidButton
                .Get()
                .SetIcon(textures)
                .SetStyleAlignSelf(Align.FlexStart)
                .SetElementSize(ElementSize.Tiny);

        /// <summary>
        /// Sorts the component order in the Inspector
        /// <para/> Fluid button used under headers for in-editor actions
        /// </summary>
        public static FluidButton SystemButton_SortComponents(GameObject targetGameObject, params string[] customSortedComponentNames) =>
            SystemButton(EditorMicroAnimations.EditorUI.Icons.SortAz)
                .SetTooltip("Sort the components, on this gameObject, in a custom alphabetical order")
                .SetOnClick(() => EditorUtils.SortComponents(targetGameObject, customSortedComponentNames));

        /// <summary>
        /// Rename the target gameObject to the given new name (has Undo)
        /// <para/> Fluid button used under headers for in-editor actions
        /// </summary>
        public static FluidButton SystemButton_RenameComponent(GameObject targetGameObject, Func<string> newName) =>
            SystemButton(EditorMicroAnimations.EditorUI.Icons.Edit)
                .SetTooltip($"Rename GameObject")
                .SetOnClick(() =>
                {
                    Undo.RecordObject(targetGameObject, "Rename");
                    targetGameObject.name = newName.Invoke();
                });

        /// <summary>
        /// Rename the target gameObject to the given new name (has Undo)
        /// <para/> Fluid button used under headers for in-editor actions
        /// </summary>
        public static FluidButton SystemButton_RenameAsset(Object targetAsset, Func<string> newName) =>
            SystemButton(EditorMicroAnimations.EditorUI.Icons.Edit)
                .SetTooltip($"Rename Asset")
                .SetOnClick(() =>
                {
                    if (targetAsset == null) return;
                    if (newName == null) return;
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(targetAsset), newName.Invoke());
                    EditorUtility.SetDirty(targetAsset);
                    AssetDatabase.SaveAssets();
                });

        /// <summary> Get a FluidToggleButtonTab set up to be used under a ComponentHeader bar </summary>
        /// <param name="textures"> Tab Button icon textures </param>
        /// <param name="selectableAccentColor"> Accent color for the tab button </param>
        public static FluidToggleButtonTab GetTabButtonForComponentSection(IEnumerable<Texture2D> textures = null, EditorSelectableColorInfo selectableAccentColor = null)
        {
            FluidToggleButtonTab tabButton =
                FluidToggleButtonTab.Get()
                    .SetContainerColorOff(tabButtonColorOff)
                    .SetTabPosition(TabPosition.TabOnBottom)
                    .SetElementSize(ElementSize.Small);

            if (textures != null)
                tabButton.SetIcon(textures);
            if (selectableAccentColor != null)
                tabButton.SetToggleAccentColor(selectableAccentColor);

            tabButton.iconReaction?.SetDuration(0.4f);

            return tabButton;
        }

        /// <summary> Get a FluidToggleButtonTab, an EnabledIndicator and a container with them assembled, set up to be used under a ComponentHeader bar </summary>
        /// <param name="textures"> Tab Button icon textures </param>
        /// <param name="selectableAccentColor"> Accent color for the tab button </param>
        /// <param name="accentColor"> Accent color for the indicator </param>
        public static (FluidToggleButtonTab, EnabledIndicator, VisualElement) GetTabButtonForComponentSectionWithEnabledIndicator(IEnumerable<Texture2D> textures, EditorSelectableColorInfo selectableAccentColor, Color accentColor)
        {
            FluidToggleButtonTab tab = GetTabButtonForComponentSection(textures, selectableAccentColor);
            EnabledIndicator indicator = EnabledIndicator.Get().SetEnabledColor(accentColor);
            VisualElement container = column.SetStyleFlexGrow(0).AddChild(indicator).AddChild(tab);
            return (tab, indicator, container);
        }

        public static FluidButton GetNewTinyButton
        (
            string text,
            IEnumerable<Texture2D> textures,
            EditorSelectableColorInfo selectableColor = null,
            string tooltip = ""
        ) =>
            FluidButton.Get()
                .SetLabelText(text)
                .SetIcon(textures)
                .SetAccentColor(selectableColor ?? EditorSelectableColors.Default.ButtonIcon)
                .SetTooltip(tooltip)
                .SetButtonStyle(ButtonStyle.Contained)
                .SetElementSize(ElementSize.Tiny);
    }


}
