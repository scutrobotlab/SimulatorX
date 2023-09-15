// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Reactor.Targets.ProgressTargets;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;
namespace Doozy.Editor.Reactor.Editors.Targets
{
    [CustomEditor(typeof(AudioMixerProgressTarget), true)]
    public class AudioMixerProgressTargetEditor : ProgressTargetEditor
    {
        public override IEnumerable<Texture2D> targetIconTextures => EditorMicroAnimations.Reactor.Icons.AudioMixerProgressTarget;

        private TextField exposedParameterNameTextField { get; set; }
        private FluidField exposedParameterNameFluidField { get; set; }
        private SerializedProperty propertyExposedParameterName { get; set; }

        private FluidToggleSwitch useLogarithmicConversionSwitch { get; set; }
        private FluidField useLogarithmicConversionFluidField { get; set; }
        private SerializedProperty propertyUseLogarithmicConversion { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            exposedParameterNameFluidField?.Recycle();
            useLogarithmicConversionSwitch?.Recycle();
            useLogarithmicConversionFluidField?.Dispose();
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyExposedParameterName = serializedObject.FindProperty("ExposedParameterName");
            propertyUseLogarithmicConversion = serializedObject.FindProperty("UseLogarithmicConversion");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(AudioMixer)))
                .AddManualButton()
                .AddYouTubeButton();

            targetObjectField
                .SetObjectType(typeof(AudioMixer));

            exposedParameterNameTextField =
                DesignUtils.NewTextField(propertyExposedParameterName)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("Name of exposed parameter in the target AudioMixer");

            exposedParameterNameFluidField =
                FluidField.Get()
                    .SetLabelText("Exposed Parameter Name")
                    .AddFieldContent(exposedParameterNameTextField);

            useLogarithmicConversionSwitch =
                FluidToggleSwitch.Get()
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetLabelText("Use Logarithmic Conversion")
                    .SetTooltip("Lower the sensitivity of the slider by using a logarithmic conversion")
                    .BindToProperty(propertyUseLogarithmicConversion);
            
            useLogarithmicConversionFluidField =
                FluidField.Get()
                    .AddFieldContent(useLogarithmicConversionSwitch);

            useLogarithmicConversionFluidField.fieldContent.SetStyleJustifyContent(Justify.Center);
            
            targetModeFluidField.SetEnabled(false);
        }

        protected override void Compose()
        {
            base.Compose();
            root
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(exposedParameterNameFluidField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(useLogarithmicConversionFluidField)
                )
                .AddChild(DesignUtils.spaceBlock2X);
        }
    }
}