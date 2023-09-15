// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Input;
using Doozy.Runtime.UIManager.Listeners;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Editors.Listeners
{
    [CustomEditor(typeof(UIViewListener), true)]
    public class UIViewListenerEditor : UnityEditor.Editor
    {
        private static IEnumerable<Texture2D> componentIconTextures => EditorMicroAnimations.UIManager.Icons.UIViewListener;

        private static Color accentColor => EditorColors.UIManager.ListenerComponent;
        private static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.ListenerComponent;

        private UIViewListener castedTarget => (UIViewListener)target;
        private IEnumerable<UIViewListener> castedTargets => targets.Cast<UIViewListener>();

        private VisualElement root { get; set; }

        private FluidComponentHeader componentHeader { get; set; }

        private PropertyField viewIdPropertyField { get; set; }
        private EnumField commandEnumField { get; set; }
        private PropertyField callbackPropertyField { get; set; }
        private ObjectField multiplayerInfoObjectField { get; set; }
        
        private FluidField idField { get; set; }
        private FluidField callbackField { get; set; }
        private FluidField multiplayerInfoField { get; set; }

        private SerializedProperty propertyViewId { get; set; }
        private SerializedProperty propertyCommand { get; set; }
        private SerializedProperty propertyCallback { get; set; }
        private SerializedProperty propertyMultiplayerInfo { get; set; }
        
        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        private void OnDestroy()
        {
            componentHeader?.Recycle();
            idField?.Recycle();
            callbackField?.Recycle();
            multiplayerInfoField?.Recycle();
        }

        private void FindProperties()
        {
            propertyViewId = serializedObject.FindProperty("ViewId");
            propertyCommand = serializedObject.FindProperty("Command");
            propertyCallback = serializedObject.FindProperty("Callback");
            propertyMultiplayerInfo = serializedObject.FindProperty("MultiplayerInfo");
        }

        private void InitializeEditor()
        {
            FindProperties();

            root = new VisualElement();

            componentHeader =
                FluidComponentHeader.Get()
                    .SetElementSize(ElementSize.Large)
                    .SetAccentColor(accentColor)
                    .SetComponentNameText((ObjectNames.NicifyVariableName(nameof(UIViewListener))))
                    .SetIcon(componentIconTextures.ToList())
                    .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048150120/UIView+Listener?atlOrigin=eyJpIjoiYmZhYTA3ZTMxZTgyNDkzNWI1ZDBjOTA1NDVlNGJiNTEiLCJwIjoiYyJ9")
                    .AddYouTubeButton();

            viewIdPropertyField =
                DesignUtils.NewPropertyField(propertyViewId);

            commandEnumField =
                DesignUtils.NewEnumField(propertyCommand)
                    .SetStyleMarginTop(12)
                    .SetStyleWidth(60);

            callbackPropertyField =
                DesignUtils.NewPropertyField(propertyCallback);

            idField =
                FluidField.Get()
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(viewIdPropertyField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(commandEnumField)
                    );

            callbackField =
                FluidField.Get()
                    .AddFieldContent
                        (callbackPropertyField);

            root.schedule.Execute(() => callbackField.Q<FluidToggleSwitch>()?.Recycle());
              
            multiplayerInfoObjectField =
                DesignUtils.NewObjectField(propertyMultiplayerInfo, typeof(MultiplayerInfo))
                    .SetStyleFlexGrow(1);
            
            multiplayerInfoField =
                FluidField.Get()
                    .SetLabelText("Player Index")
                    .AddFieldContent(multiplayerInfoObjectField)
                    .SetStyleMarginTop(DesignUtils.k_Spacing2X)
                    .SetStyleDisplay(UIManagerInputSettings.instance.multiplayerMode ? DisplayStyle.Flex : DisplayStyle.None);
        }

        private void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(idField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(callbackField)
                .AddChild(multiplayerInfoField)
                ;
        }
    }
}
