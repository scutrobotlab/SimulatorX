// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.Common.Utils;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Input;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using ObjectNames = Doozy.Runtime.Common.Utils.ObjectNames;

namespace Doozy.Editor.UIManager.Layouts.Settings
{
    public sealed class InputSettingsWindowLayout : FluidWindowLayout, IUIManagerSettingsWindowLayout
    {
        public override string layoutName => "Input Settings";
        public override List<Texture2D> animatedIconTextures => EditorMicroAnimations.UIManager.Icons.KeyMapper;
        public override Color accentColor => EditorColors.UIManager.Settings;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.Settings;

        private SerializedObject serializedObject { get; set; }
        private SerializedProperty propertyDefaultPlayerIndex { get; set; }
        private SerializedProperty propertyMultiplayerMode { get; set; }
        private SerializedProperty propertyBackButtonCooldown { get; set; }

        private IntegerField defaultPlayerIndexIntegerField { get; set; }
        private FluidToggleSwitch multiplayerModeSwitch { get; set; }
        private FloatField backButtonCooldownFloatField { get; set; }

        private static IEnumerable<Texture2D> resetTextures => EditorMicroAnimations.EditorUI.Icons.Reset;
        private FluidButton resetPlayerIndexButton { get; set; }
        private FluidButton resetMultiplayerModeButton { get; set; }
        private FluidButton resetBackButtonCooldownButton { get; set; }
        private FluidButton saveButton { get; set; }

        private FluidField playerIndexField { get; set; }
        private FluidField playerMultiplayerMode { get; set; }
        private FluidField playerBackButtonCooldown { get; set; }

        private FluidToggleButtonTab inputSystemPackageTabButton { get; set; }
        private FluidToggleButtonTab legacyInputManagerTabButton { get; set; }
        private FluidToggleButtonTab customInputTabButton { get; set; }

        public override void OnDestroy()
        {
            base.OnDestroy();
            multiplayerModeSwitch?.Recycle();
            playerIndexField?.Recycle();
            playerMultiplayerMode?.Recycle();
            playerBackButtonCooldown?.Recycle();
            resetPlayerIndexButton?.Recycle();
            resetBackButtonCooldownButton?.Recycle();
            resetMultiplayerModeButton?.Recycle();
            saveButton?.Recycle();

            inputSystemPackageTabButton?.Recycle();
            legacyInputManagerTabButton?.Recycle();
            customInputTabButton?.Recycle();
        }

        public InputSettingsWindowLayout()
        {
            AddHeader("Input Settings", "Runtime Global Settings", animatedIconTextures);
            sideMenu.Dispose(); //remove side menu
            FindProperties();
            Initialize();
            Compose();
        }

        private void FindProperties()
        {
            serializedObject = new SerializedObject(UIManagerInputSettings.instance);
            propertyDefaultPlayerIndex = serializedObject.FindProperty("DefaultPlayerIndex");
            propertyMultiplayerMode = serializedObject.FindProperty("MultiplayerMode");
            propertyBackButtonCooldown = serializedObject.FindProperty("BackButtonCooldown");
        }

        private void Initialize()
        {
            defaultPlayerIndexIntegerField =
                DesignUtils.NewIntegerField(propertyDefaultPlayerIndex)
                    .SetTooltip("Default player index value (used for global user)")
                    .SetStyleWidth(60);

            multiplayerModeSwitch =
                FluidToggleSwitch.Get()
                    .SetTooltip("True if Multiplayer Mode is enabled")
                    .SetToggleAccentColor(selectableAccentColor)
                    .BindToProperty(propertyMultiplayerMode);

            backButtonCooldownFloatField =
                DesignUtils.NewFloatField(propertyBackButtonCooldown)
                    .SetTooltip("Cooldown after the 'Back' button was fired (to prevent spamming and accidental double execution)")
                    .SetStyleWidth(60);

            resetPlayerIndexButton =
                DesignUtils.SystemButton(resetTextures)
                    .SetTooltip("Reset")
                    .SetOnClick(() =>
                    {
                        propertyDefaultPlayerIndex.intValue = -UIManagerInputSettings.k_LifeTheUniverseAndEverything;
                        serializedObject.ApplyModifiedProperties();
                    });

            resetMultiplayerModeButton =
                DesignUtils.SystemButton(resetTextures)
                    .SetTooltip("Reset")
                    .SetOnClick(() =>
                    {
                        propertyMultiplayerMode.boolValue = false;
                        serializedObject.ApplyModifiedProperties();
                    });

            resetBackButtonCooldownButton =
                DesignUtils.SystemButton(resetTextures)
                    .SetTooltip("Reset")
                    .SetOnClick(() =>
                    {
                        propertyBackButtonCooldown.floatValue = UIManagerInputSettings.k_BackButtonCooldown;
                        serializedObject.ApplyModifiedProperties();
                    });

            playerIndexField =
                FluidField.Get()
                    .SetStyleFlexGrow(0)
                    .SetElementSize(ElementSize.Normal)
                    .SetLabelText("Default Player Index")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(DesignUtils.flexibleSpace)
                            .AddChild(defaultPlayerIndexIntegerField.DisableElement())
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(resetPlayerIndexButton)
                    );

            playerMultiplayerMode =
                FluidField.Get()
                    .SetStyleFlexGrow(0)
                    .SetElementSize(ElementSize.Normal)
                    .SetLabelText("Multiplayer Mode")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(DesignUtils.flexibleSpace)
                            .AddChild(multiplayerModeSwitch)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(resetMultiplayerModeButton)
                    );

            playerBackButtonCooldown =
                FluidField.Get()
                    .SetStyleFlexGrow(0)
                    .SetElementSize(ElementSize.Normal)
                    .SetLabelText("'Back' button cooldown")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(DesignUtils.flexibleSpace)
                            .AddChild(backButtonCooldownFloatField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(resetBackButtonCooldownButton)
                    );

            saveButton =
                FluidButton.Get()
                    .SetButtonStyle(ButtonStyle.Contained)
                    .SetElementSize(ElementSize.Small)
                    .SetLabelText("Save")
                    .SetIcon(EditorMicroAnimations.EditorUI.Icons.Save)
                    .SetOnClick(() =>
                    {
                        EditorUtility.SetDirty(UIManagerInputSettings.instance);
                        AssetDatabase.SaveAssets();
                    });

            FluidToggleButtonTab GetTab() =>
                FluidToggleButtonTab.Get()
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetContainerColorOff(DesignUtils.tabButtonColorOff);
            
            inputSystemPackageTabButton =
                GetTab()
                    .SetLabelText($"{ObjectNames.NicifyVariableName(InputHandling.InputSystemPackage.ToString())}")
                    .SetTabPosition(TabPosition.TabOnLeft)
                    .SetOnClick(() =>
                    {
                        DefineSymbolsUtils.RemoveGlobalDefine("LEGACY_INPUT_MANAGER");
                        DefineSymbolsUtils.AddGlobalDefine("INPUT_SYSTEM_PACKAGE");
                        legacyInputManagerTabButton.isOn = false;
                        customInputTabButton.isOn = false;
                    });

            legacyInputManagerTabButton =
                GetTab()
                    .SetLabelText($"{ObjectNames.NicifyVariableName(InputHandling.LegacyInputManager.ToString())}")
                    .SetTabPosition(TabPosition.TabInCenter)
                    .SetOnClick(() =>
                    {
                        DefineSymbolsUtils.RemoveGlobalDefine("INPUT_SYSTEM_PACKAGE");
                        DefineSymbolsUtils.AddGlobalDefine("LEGACY_INPUT_MANAGER");
                        inputSystemPackageTabButton.isOn = false;
                        customInputTabButton.isOn = false;
                    });

            customInputTabButton =
                GetTab()
                    .SetLabelText($"{ObjectNames.NicifyVariableName(InputHandling.CustomInput.ToString())}")
                    .SetTabPosition(TabPosition.TabOnRight)
                    .SetOnClick(() =>
                    {
                        DefineSymbolsUtils.RemoveGlobalDefine("INPUT_SYSTEM_PACKAGE");
                        DefineSymbolsUtils.RemoveGlobalDefine("LEGACY_INPUT_MANAGER");
                        inputSystemPackageTabButton.isOn = false;
                        legacyInputManagerTabButton.isOn = false;
                    });
            
            inputSystemPackageTabButton.isOn = UIManagerInputSettings.k_InputHandling == InputHandling.InputSystemPackage;
            legacyInputManagerTabButton.isOn = UIManagerInputSettings.k_InputHandling == InputHandling.LegacyInputManager;
            customInputTabButton.isOn = UIManagerInputSettings.k_InputHandling == InputHandling.CustomInput;

            content.Bind(serializedObject);
        }

        private void Compose()
        {
            content
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleFlexGrow(0)
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild(inputSystemPackageTabButton)
                        .AddSpace(1,1)
                        .AddChild(legacyInputManagerTabButton)
                        .AddSpace(1,1)
                        .AddChild(customInputTabButton)
                        .AddChild(DesignUtils.flexibleSpace)
                )
                .AddChild(DesignUtils.spaceBlock4X)
                .AddChild(playerIndexField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(playerMultiplayerMode)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(playerBackButtonCooldown)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleFlexGrow(0)
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild(saveButton)
                )
                ;
        }
    }
}
