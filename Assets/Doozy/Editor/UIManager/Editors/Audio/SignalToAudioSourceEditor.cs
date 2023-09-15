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
using Doozy.Runtime.UIManager.Audio;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Editors.Audio
{
    [CustomEditor(typeof(SignalToAudioSource), true)]
    public class SignalToAudioSourceEditor : UnityEditor.Editor
    {
        private SignalToAudioSource castedTarget => (SignalToAudioSource)target;
        private IEnumerable<SignalToAudioSource> castedTargets => targets.Cast<SignalToAudioSource>();

        private static Color accentColor => EditorColors.UIManager.AudioComponent;
        private static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.AudioComponent;

        private static IEnumerable<Texture2D> componentIconTextures => EditorMicroAnimations.UIManager.Icons.SignalToAudioSource;

        private VisualElement root { get; set; }
        private FluidComponentHeader componentHeader { get; set; }

        private SerializedProperty propertyStreamId { get; set; }
        private SerializedProperty propertyAudioSource { get; set; }

        private FluidField streamIdFluidField { get; set; }
        private FluidField audioSourceFluidField { get; set; }

        private PropertyField streamIdPropertyField { get; set; }
        private ObjectField audioSourceObjectField { get; set; }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        private void OnDestroy()
        {
            componentHeader?.Recycle();

            streamIdFluidField?.Recycle();
            audioSourceFluidField?.Recycle();
        }

        private void FindProperties()
        {
            propertyStreamId = serializedObject.FindProperty("StreamId");
            propertyAudioSource = serializedObject.FindProperty("AudioSource");
        }

        private void InitializeEditor()
        {
            FindProperties();

            root = new VisualElement();

            componentHeader =
                FluidComponentHeader.Get()
                    .SetElementSize(ElementSize.Large)
                    .SetAccentColor(accentColor)
                    .SetComponentNameText("Signal To AudioSource")
                    .SetIcon(componentIconTextures.ToList())
                    .AddManualButton()
                    .AddYouTubeButton();

            streamIdPropertyField = DesignUtils.NewPropertyField(propertyStreamId);
            streamIdFluidField = FluidField.Get().AddFieldContent(streamIdPropertyField);

            audioSourceObjectField = DesignUtils.NewObjectField(propertyAudioSource, typeof(AudioSource)).SetStyleFlexGrow(1).SetTooltip("Target AudioSource");
            audioSourceFluidField = FluidField.Get().SetLabelText("Audio Source").SetIcon(EditorMicroAnimations.EditorUI.Icons.Sound).AddFieldContent(audioSourceObjectField);
        }

        private void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(streamIdFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(audioSourceFluidField)
                .AddChild(DesignUtils.endOfLineBlock)
                ;
        }
    }
}
