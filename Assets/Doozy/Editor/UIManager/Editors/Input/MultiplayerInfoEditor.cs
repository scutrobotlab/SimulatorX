// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIElements;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Input;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Editors.Input
{
    [CustomEditor(typeof(MultiplayerInfo), true)]
    public class MultiplayerInfoEditor : UnityEditor.Editor
    {
        private static IEnumerable<Texture2D> multiplayerInfoIconTextures => EditorMicroAnimations.UIManager.Icons.MultiplayerInfo;

        private static Color accentColor => EditorColors.UIManager.InputComponent;
        private static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.InputComponent;

        private MultiplayerInfo castedTarget => (MultiplayerInfo)target;
        private IEnumerable<MultiplayerInfo> castedTargets => targets.Cast<MultiplayerInfo>();

        private VisualElement root { get; set; }

        private PropertyField playerInputPropertyField { get; set; }
        private IntegerField customPlayerIndexIntegerField { get; set; }

        private FluidComponentHeader componentHeader { get; set; }
        private FluidToggleSwitch autoUpdateSwitch { get; set; }
        private FluidField playerInputField { get; set; }
        private FluidField customPlayerIndexField { get; set; }
        private FluidToggleSwitch useCustomPlayerIndexSwitch { get; set; }


        private SerializedProperty propertyAutoUpdate { get; set; }
        private SerializedProperty propertyPlayerInput { get; set; }
        private SerializedProperty propertyCustomPlayerIndex { get; set; }
        private SerializedProperty propertyUseCustomPlayerIndex { get; set; }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        private void OnDestroy()
        {
            componentHeader?.Recycle();
            autoUpdateSwitch?.Recycle();
            playerInputField?.Recycle();
            customPlayerIndexField?.Recycle();
            useCustomPlayerIndexSwitch?.Recycle();
        }

        private void FindProperties()
        {
            propertyAutoUpdate = serializedObject.FindProperty("AutoUpdate");
            propertyPlayerInput = serializedObject.FindProperty("PlayerInput");
            propertyCustomPlayerIndex = serializedObject.FindProperty("CustomPlayerIndex");
            propertyUseCustomPlayerIndex = serializedObject.FindProperty("UseCustomPlayerIndex");
        }

        private void InitializeEditor()
        {
            FindProperties();

            root = new VisualElement();

            componentHeader =
                FluidComponentHeader.Get()
                    .SetElementSize(ElementSize.Large)
                    .SetAccentColor(accentColor)
                    .SetComponentNameText((ObjectNames.NicifyVariableName(nameof(MultiplayerInfo))))
                    .SetIcon(multiplayerInfoIconTextures.ToList())
                    .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1046642771/Multiplayer+Info?atlOrigin=eyJpIjoiM2RlOGU4MDljZjE1NDYxNWE3NTdjY2JjY2U0MjkxMGUiLCJwIjoiYyJ9")
                    .AddYouTubeButton();

            autoUpdateSwitch =
                FluidToggleSwitch.Get()
                    .SetLabelText("AutoUpdate")
                    .SetStyleMarginLeft(40)
                    .BindToProperty(propertyAutoUpdate);

            playerInputPropertyField =
                DesignUtils.NewPropertyField(propertyPlayerInput)
                    .TryToHideLabel()
                    .SetStyleFlexGrow(1);

            customPlayerIndexIntegerField =
                DesignUtils.NewIntegerField(propertyCustomPlayerIndex)
                    .SetStyleWidth(80)
                    .SetStyleFlexShrink(1);

            customPlayerIndexIntegerField.SetEnabled(propertyUseCustomPlayerIndex.boolValue);

            useCustomPlayerIndexSwitch =
                FluidToggleSwitch.Get()
                    .BindToProperty(propertyUseCustomPlayerIndex);

            useCustomPlayerIndexSwitch.SetOnValueChanged(evt => customPlayerIndexIntegerField.SetEnabled(evt.newValue));
            
            playerInputField =
                FluidField.Get()
                    .SetLabelText("Player Input")
                    .AddFieldContent(playerInputPropertyField);

            customPlayerIndexField =
                FluidField.Get()
                    .SetStyleFlexGrow(0)
                    .SetStyleFlexShrink(1)
                    .SetLabelText("Custom Player Index")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(useCustomPlayerIndexSwitch)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(customPlayerIndexIntegerField)
                    );
        }

        private void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(autoUpdateSwitch)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(playerInputField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(customPlayerIndexField)
                )
                ;
        }
    }
}
