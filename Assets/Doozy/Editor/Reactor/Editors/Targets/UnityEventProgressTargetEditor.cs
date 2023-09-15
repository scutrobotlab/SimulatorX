// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
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
using UnityEngine.Events;
using UnityEngine.UIElements;
namespace Doozy.Editor.Reactor.Editors.Targets
{
    [CustomEditor(typeof(UnityEventProgressTarget), true)]
    public class UnityEventProgressTargetEditor: ProgressTargetEditor
    {
        public override IEnumerable<Texture2D> targetIconTextures => EditorMicroAnimations.Reactor.Icons.UnityEventProgressTarget;

        private FluidToggleSwitch wholeNumbersSwitch { get; set; }
        private SerializedProperty propertyWholeNumbers { get; set; }

        private FluidToggleSwitch useMultiplierSwitch { get; set; }
        private SerializedProperty propertyUseMultiplier { get; set; }

        private FloatField multiplierFloatField { get; set; }
        private SerializedProperty propertyMultiplier { get; set; }
        
        private PropertyField targetPropertyField { get; set; }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();

            wholeNumbersSwitch?.Recycle();
            useMultiplierSwitch?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyWholeNumbers = serializedObject.FindProperty("WholeNumbers");
            propertyUseMultiplier = serializedObject.FindProperty("UseMultiplier");
            propertyMultiplier = serializedObject.FindProperty("Multiplier");
        }

          protected override void InitializeEditor()
        {
            FindProperties();

            root = new VisualElement();

            componentHeader = FluidComponentHeader.Get()
                .SetAccentColor(accentColor)
                .SetSecondaryIcon(targetIconTextures.ToList())
                .SetElementSize(ElementSize.Tiny)
                .SetComponentTypeText("Progress Target")
                .SetComponentNameText("UnityEvent")
                .AddManualButton()
                .AddYouTubeButton();
            
            targetModeEnumField =
                DesignUtils.NewEnumField(propertyTargetMode)
                    .SetStyleFlexGrow(1);
            
            targetModeFluidField =
                FluidField.Get()
                    .SetLabelText("Target Mode")
                    .AddFieldContent(targetModeEnumField);
            
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


            targetPropertyField =
                DesignUtils.NewPropertyField(propertyTarget);
        }

        protected override void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(targetPropertyField)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(targetModeFluidField)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(wholeNumbersSwitch)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(useMultiplierSwitch)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(multiplierFloatField)
                )
                .AddChild(DesignUtils.spaceBlock2X);
        }
    }
}
