// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.ScriptableObjects;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Reactor.Layouts
{
    public sealed class ReactorPresetsWindowLayout : FluidWindowLayout
    {
        public override Color accentColor => EditorColors.Reactor.Red;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Reactor.Red;

        private SerializedObject serializedObject { get; }

        public override List<Texture2D> animatedIconTextures => EditorMicroAnimations.Reactor.Icons.UIAnimationPreset;
     
        public FluidPlaceholder comingSoonPlaceholder { get; private set; }
        
        public ReactorPresetsWindowLayout()
        {
            serializedObject = new SerializedObject(UIAnimationPresetDatabase.instance);

            AddHeader("UIAnimation Presets", "Presets Database", animatedIconTextures);

            
            comingSoonPlaceholder =
                FluidPlaceholder.Get()
                    .SetIcon(EditorMicroAnimations.EditorUI.Placeholders.ComingSoon)
                    .SetLabelText("Coming Soon")
                    .SetStyleFlexGrow(1)
                    .Show();

            content
                .AddChild(comingSoonPlaceholder);
            
            FluidButton findLocationButton = FluidButton.Get()
                .SetLabelText("Find Database Location")
                .SetIcon(EditorMicroAnimations.EditorUI.Icons.Location)
                .SetAccentColor(EditorSelectableColors.Default.ButtonIcon)
                .SetTooltip("Ping the UIAnimation Presets Database in the Project view")
                .SetButtonStyle(ButtonStyle.Clear)
                .SetElementSize(ElementSize.Tiny)
                .SetStyleWidth(130)
                .SetStyleMarginBottom(spacing * 2)
                .SetOnClick(() => EditorGUIUtility.PingObject(UIAnimationPresetDatabase.instance));

            FluidButton refreshDatabaseButton = FluidButton.Get()
                .SetLabelText("Refresh Database")
                .SetAccentColor(selectableAccentColor)
                .SetTooltip("Search for all UIAnimationPresets assets in the project")
                .SetButtonStyle(ButtonStyle.Contained)
                .SetElementSize(ElementSize.Small)
                .SetStyleWidth(130)
                .SetOnClick(() => UIAnimationPresetDatabase.instance.RefreshDatabase(true, true))
                .SetIcon(EditorMicroAnimations.EditorUI.Icons.Refresh);
            
            //SIDE MENU - ToolbarContainer - Refresh button
            sideMenu
                .toolbarContainer
                .SetStyleDisplay(DisplayStyle.Flex)
                .AddChild(findLocationButton)
                .AddChild(refreshDatabaseButton);

            foreach (UIAnimationType animationType in Enum.GetValues(typeof(UIAnimationType)))
            {
                // int numberOfPresets = UIAnimationPresetDatabase.instance.GetPresetGroup(animationType).presets.Count;
                string buttonLabelText = $"{animationType} ({NumberOfPresets(animationType)} presets)";
                sideMenu
                    .AddButton(buttonLabelText, selectableAccentColor)
                    .SetOnValueChanged(evt =>
                    {
                        if (!evt.newValue) return;
                        ShowPresetGroup(animationType);
                    });
            }
        }

        private static string NumberOfPresets(UIAnimationType animationType) =>
            UIAnimationPresetDatabase.instance.GetPresetGroup(animationType).presets.Count.ToString();

        private void ShowPresetGroup(UIAnimationType animationType)
        {
            //ToDo: create animation preview editor engine
            // content.Clear();
            comingSoonPlaceholder.Play();
            switch (animationType)
            {
                case UIAnimationType.Show:
                    break;
                case UIAnimationType.Hide:
                    break;
                case UIAnimationType.Loop:
                    break;
                case UIAnimationType.Custom:
                    break;
                case UIAnimationType.Button:
                    break;
                case UIAnimationType.State:
                    break;
                case UIAnimationType.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(animationType), animationType, null);
            }
        }
    }
}
