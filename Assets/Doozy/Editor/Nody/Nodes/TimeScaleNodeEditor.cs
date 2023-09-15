// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Nody.Nodes.Internal;
using Doozy.Runtime.Nody.Nodes;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.Nody.Nodes
{
    [CustomEditor(typeof(TimeScaleNode))]
    public class TimeScaleNodeEditor : FlowNodeEditor
    {
        public override IEnumerable<Texture2D> nodeIconTextures => EditorMicroAnimations.Nody.Icons.TimeScaleNode;

        private FloatField targetValueFloatField { get; set; }
        private FluidToggleSwitch animateValueSwitch { get; set; }
        private FloatField animationDurationFloatField { get; set; }
        private Label animationDurationLabel { get; set; }
        private EnumField animationEaseEnumField { get; set; }
        private FluidToggleCheckbox waitForAnimationToFinishCheckbox { get; set; }

        private FluidField targetValueField { get; set; }
        private FluidField animationField { get; set; }
        private FluidField waitForAnimationToFinishField { get; set; }

        private SerializedProperty propertyTargetValue { get; set; }
        private SerializedProperty propertyAnimateValue { get; set; }
        private SerializedProperty propertyAnimationDuration { get; set; }
        private SerializedProperty propertyAnimationEase { get; set; }
        private SerializedProperty propertyWaitForAnimationToFinish { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            animateValueSwitch?.Recycle();
            waitForAnimationToFinishCheckbox?.Recycle();
            targetValueField?.Recycle();
            animationField?.Recycle();
            waitForAnimationToFinishField?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyTargetValue = serializedObject.FindProperty(nameof(TimeScaleNode.TargetValue));
            propertyAnimateValue = serializedObject.FindProperty(nameof(TimeScaleNode.AnimateValue));
            propertyAnimationDuration = serializedObject.FindProperty(nameof(TimeScaleNode.AnimationDuration));
            propertyAnimationEase = serializedObject.FindProperty(nameof(TimeScaleNode.AnimationEase));
            propertyWaitForAnimationToFinish = serializedObject.FindProperty(nameof(TimeScaleNode.WaitForAnimationToFinish));
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(TimeScaleNode)))
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048117283/Time+Scale+Node?atlOrigin=eyJpIjoiMzE5NmQ5NjJjZmRkNGUwOTk3NTFmZmMyYjQzYWNkN2MiLCJwIjoiYyJ9")
                .AddYouTubeButton();

            targetValueFloatField = DesignUtils.NewFloatField(propertyTargetValue).SetStyleFlexGrow(1);
            animateValueSwitch = FluidToggleSwitch.Get().BindToProperty(propertyAnimateValue);
            animationDurationFloatField = DesignUtils.NewFloatField(propertyAnimationDuration).SetStyleFlexGrow(1);
            animationDurationLabel = DesignUtils.fieldLabel.SetText("seconds");
            animationEaseEnumField = DesignUtils.NewEnumField(propertyAnimationEase).SetStyleFlexGrow(1);
            waitForAnimationToFinishCheckbox = FluidToggleCheckbox.Get().SetLabelText("Wait for animation to finish").BindToProperty(propertyWaitForAnimationToFinish);

            animateValueSwitch.SetOnValueChanged(evt =>
            {
                if (evt?.newValue == null) return;
                animationDurationFloatField.SetEnabled(evt.newValue);
                animationDurationLabel.SetEnabled(evt.newValue);
                animationEaseEnumField.SetEnabled(evt.newValue);
                waitForAnimationToFinishCheckbox.SetEnabled(evt.newValue);
            });
            
            animationDurationFloatField.SetEnabled(propertyAnimateValue.boolValue);
            animationDurationLabel.SetEnabled(propertyAnimateValue.boolValue);
            animationEaseEnumField.SetEnabled(propertyAnimateValue.boolValue);
            waitForAnimationToFinishCheckbox.SetEnabled(propertyAnimateValue.boolValue);

            targetValueField =
                FluidField.Get()
                    .SetStyleFlexGrow(1)
                    .SetLabelText("Target Value")
                    .AddFieldContent(targetValueFloatField);

            animationField =
                FluidField.Get()
                    .SetStyleFlexGrow(1)
                    .SetLabelText("Animation")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .SetStyleAlignItems(Align.Center)
                            .AddChild(animateValueSwitch)
                            .AddChild(DesignUtils.spaceBlock2X)
                            .AddChild(animationDurationFloatField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(animationDurationLabel)
                            .AddChild(DesignUtils.spaceBlock2X)
                            .AddChild(animationEaseEnumField)
                    );
        }

        protected override void Compose()
        {
            base.Compose();

            root
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(targetValueField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(animationField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(waitForAnimationToFinishCheckbox)
                ;
        }
    }
}
