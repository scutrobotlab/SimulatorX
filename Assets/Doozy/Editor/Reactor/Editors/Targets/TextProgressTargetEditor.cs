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
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
namespace Doozy.Editor.Reactor.Editors.Targets
{
    [CustomEditor(typeof(TextProgressTarget), true)]
    public class TextProgressTargetEditor : ProgressTargetEditor
    {
        public override IEnumerable<Texture2D> targetIconTextures => EditorMicroAnimations.Reactor.Icons.TextProgressTarget;

        private FluidToggleSwitch wholeNumbersSwitch { get; set; }
        private SerializedProperty propertyWholeNumbers { get; set; }

        private FluidToggleSwitch useMultiplierSwitch { get; set; }
        private SerializedProperty propertyUseMultiplier { get; set; }

        private FloatField multiplierFloatField { get; set; }
        private SerializedProperty propertyMultiplier { get; set; }

        private TextField prefixTextField { get; set; }
        private FluidField prefixFluidField { get; set; }
        private SerializedProperty propertyPrefix { get; set; }

        private TextField suffixTextField { get; set; }
        private FluidField suffixFluidField { get; set; }
        private SerializedProperty propertySuffix { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            wholeNumbersSwitch?.Recycle();
            useMultiplierSwitch?.Recycle();
            prefixFluidField?.Recycle();
            suffixFluidField?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyWholeNumbers = serializedObject.FindProperty("WholeNumbers");
            propertyUseMultiplier = serializedObject.FindProperty("UseMultiplier");
            propertyMultiplier = serializedObject.FindProperty("Multiplier");
            propertyPrefix = serializedObject.FindProperty("Prefix");
            propertySuffix = serializedObject.FindProperty("Suffix");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(Text)))
                .AddManualButton()
                .AddYouTubeButton();

            targetObjectField
                .SetObjectType(typeof(Text));

            wholeNumbersSwitch =
                FluidToggleSwitch.Get()
                    .SetLabelText("Whole Numbers")
                    .SetToggleAccentColor(selectableAccentColor)
                    .BindToProperty(propertyWholeNumbers);

            useMultiplierSwitch =
                FluidToggleSwitch.Get()
                    .SetLabelText("Use Multiplier")
                    .SetToggleAccentColor(selectableAccentColor)
                    .BindToProperty(propertyUseMultiplier);

            multiplierFloatField =
                DesignUtils.NewFloatField(propertyMultiplier)
                    .SetStyleFlexGrow(1);

            multiplierFloatField.SetEnabled(propertyUseMultiplier.boolValue);
            useMultiplierSwitch.SetOnValueChanged(evt =>
            {
                if (evt?.newValue == null) return;
                multiplierFloatField.SetEnabled(evt.newValue);
            });

            prefixTextField =
                DesignUtils.NewTextField(propertyPrefix)
                    .SetStyleFlexGrow(1);

            prefixFluidField =
                FluidField.Get("Prefix", prefixTextField);

            suffixTextField =
                DesignUtils.NewTextField(propertySuffix)
                    .SetStyleFlexGrow(1);

            suffixFluidField =
                FluidField.Get("Suffix", suffixTextField);
        }

        protected override void Compose()
        {
            base.Compose();

            root
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(wholeNumbersSwitch)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(useMultiplierSwitch)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(multiplierFloatField)
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(prefixFluidField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(suffixFluidField)
                )
                .AddChild(DesignUtils.spaceBlock2X);
        }
    }
}
