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
using Doozy.Runtime.UIManager.Listeners;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Editors.Listeners
{
    [CustomEditor(typeof(SignalListener), true)]
    public class SignalListenerEditor : UnityEditor.Editor
    {
        private static IEnumerable<Texture2D> componentIconTextures => EditorMicroAnimations.UIManager.Icons.SignalListener;

        private static Color accentColor => EditorColors.UIManager.ListenerComponent;
        private static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.ListenerComponent;

        private SignalListener castedTarget => (SignalListener)target;
        private IEnumerable<SignalListener> castedTargets => targets.Cast<SignalListener>();

        private VisualElement root { get; set; }

        private FluidComponentHeader componentHeader { get; set; }

        private PropertyField streamIdPropertyField { get; set; }
        private PropertyField callbackPropertyField { get; set; }

        private FluidField idField { get; set; }
        private FluidField callbackField { get; set; }

        private SerializedProperty propertyStreamId { get; set; }
        private SerializedProperty propertyCallback { get; set; }

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
        }

        private void FindProperties()
        {
            propertyStreamId = serializedObject.FindProperty("StreamId");
            propertyCallback = serializedObject.FindProperty("Callback");
        }

        private void InitializeEditor()
        {
            FindProperties();

            root = new VisualElement();

            componentHeader =
                FluidComponentHeader.Get()
                    .SetElementSize(ElementSize.Large)
                    .SetAccentColor(accentColor)
                    .SetComponentNameText((ObjectNames.NicifyVariableName(nameof(SignalListener))))
                    .SetIcon(componentIconTextures.ToList())
                    .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048772618/Signal+Listener?atlOrigin=eyJpIjoiNmM1M2MyZDg1ZDM4NGYzZTljMWM3ZWZiNDYyZjg2MjAiLCJwIjoiYyJ9")
                    .AddYouTubeButton();

            streamIdPropertyField =
                DesignUtils.NewPropertyField(propertyStreamId);

            callbackPropertyField =
                DesignUtils.NewPropertyField(propertyCallback);

            idField =
                FluidField.Get()
                    .AddFieldContent
                        (streamIdPropertyField);

            callbackField =
                FluidField.Get()
                    .AddFieldContent
                        (callbackPropertyField);

            root.schedule.Execute(() => callbackField.Q<FluidToggleSwitch>()?.Recycle());

        }

        private void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(idField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(callbackField)
                ;
        }


    }
}
