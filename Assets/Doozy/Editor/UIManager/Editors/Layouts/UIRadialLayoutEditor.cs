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
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Layouts;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Editors.Layouts
{
    [CustomEditor(typeof(UIRadialLayout), true)]
    public class UIRadialLayoutEditor : UnityEditor.Editor
    {
        private static IEnumerable<Texture2D> radialLayoutIconTextures => EditorMicroAnimations.UIManager.Icons.RadialLayout;

        private static Color accentColor => EditorColors.UIManager.LayoutComponent;
        private static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.LayoutComponent;

        private UIRadialLayout castedTarget => (UIRadialLayout)target;
        private IEnumerable<UIRadialLayout> castedTargets => targets.Cast<UIRadialLayout>();

        private VisualElement root { get; set; }

        private FluidComponentHeader componentHeader { get; set; }

        private Slider radiusSlider { get; set; }
        private FloatField radiusFloatField { get; set; }
        private FloatField maxRadiusFloatField { get; set; }
        private Slider minAngleSlider { get; set; }
        private FloatField minAngleFloatField { get; set; }
        private Slider maxAngleSlider { get; set; }
        private FloatField maxAngleFloatField { get; set; }
        private Slider startAngleSlider { get; set; }
        private FloatField startAngleFloatField { get; set; }
        private FloatField spacingFloatField { get; set; }
        private FluidToggleSwitch clockwiseSwitch { get; set; }
        private FluidToggleSwitch autoRebuildSwitch { get; set; }
        private FluidToggleSwitch rotateChildrenSwitch { get; set; }
        private Slider childRotationSlider { get; set; }
        private FloatField childRotationFloatField { get; set; }
        private FluidToggleSwitch controlChildWidthSwitch { get; set; }
        private FloatField childWidthFloatField { get; set; }
        private FluidToggleSwitch radiusControlsWidthSwitch { get; set; }
        private Slider radiusWidthFactorSlider { get; set; }
        private FloatField radiusWidthFactorFloatField { get; set; }
        private FluidToggleSwitch controlChildHeightSwitch { get; set; }
        private FloatField childHeightFloatField { get; set; }
        private FluidToggleSwitch radiusControlsHeightSwitch { get; set; }
        private Slider radiusHeightFactorSlider { get; set; }
        private FloatField radiusHeightFactorFloatField { get; set; }

        private FluidField radiusField { get; set; }
        private FluidField maxRadiusField { get; set; }
        private FluidField minAngleField { get; set; }
        private FluidField maxAngleField { get; set; }
        private FluidField startAngleField { get; set; }
        private FluidField spacingField { get; set; }
        private FluidComponentHeader childRotationHeader { get; set; }
        private FluidField childRotationField { get; set; }
        private FluidComponentHeader childWidthHeader { get; set; }
        private FluidField childWidthField { get; set; }
        private FluidField radiusWidthFactorField { get; set; }
        private FluidComponentHeader childHeightHeader { get; set; }
        private FluidField childHeightField { get; set; }
        private FluidField radiusHeightFactorField { get; set; }

        private SerializedProperty propertyAutoRebuild { get; set; }
        private SerializedProperty propertyChildHeight { get; set; }
        private SerializedProperty propertyChildRotation { get; set; }
        private SerializedProperty propertyChildWidth { get; set; }
        private SerializedProperty propertyClockwise { get; set; }
        private SerializedProperty propertyControlChildHeight { get; set; }
        private SerializedProperty propertyControlChildWidth { get; set; }
        private SerializedProperty propertyMaxAngle { get; set; }
        private SerializedProperty propertyMaxRadius { get; set; }
        private SerializedProperty propertyMinAngle { get; set; }
        private SerializedProperty propertyRadius { get; set; }
        private SerializedProperty propertyRadiusControlsHeight { get; set; }
        private SerializedProperty propertyRadiusControlsWidth { get; set; }
        private SerializedProperty propertyRadiusHeightFactor { get; set; }
        private SerializedProperty propertyRadiusWidthFactor { get; set; }
        private SerializedProperty propertyRotateChildren { get; set; }
        private SerializedProperty propertySpacing { get; set; }
        private SerializedProperty propertyStartAngle { get; set; }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        private void OnDestroy()
        {
            componentHeader?.Recycle();
            clockwiseSwitch?.Recycle();
            autoRebuildSwitch?.Recycle();
            rotateChildrenSwitch?.Recycle();
            controlChildWidthSwitch?.Recycle();
            radiusControlsWidthSwitch?.Recycle();
            controlChildHeightSwitch?.Recycle();
            radiusControlsHeightSwitch?.Recycle();
            radiusField?.Recycle();
            maxRadiusField?.Recycle();
            minAngleField?.Recycle();
            maxAngleField?.Recycle();
            startAngleField?.Recycle();
            spacingField?.Recycle();
            childRotationHeader?.Recycle();
            childRotationField?.Recycle();
            childWidthHeader?.Recycle();
            childWidthField?.Recycle();
            radiusWidthFactorField?.Recycle();
            childHeightHeader?.Recycle();
            childHeightField?.Recycle();
            radiusHeightFactorField?.Recycle();

        }

        private void FindProperties()
        {
            propertyAutoRebuild = serializedObject.FindProperty("AutoRebuild");
            propertyChildHeight = serializedObject.FindProperty("ChildHeight");
            propertyChildRotation = serializedObject.FindProperty("ChildRotation");
            propertyChildWidth = serializedObject.FindProperty("ChildWidth");
            propertyClockwise = serializedObject.FindProperty("Clockwise");
            propertyControlChildHeight = serializedObject.FindProperty("ControlChildHeight");
            propertyControlChildWidth = serializedObject.FindProperty("ControlChildWidth");
            propertyMaxAngle = serializedObject.FindProperty("MaxAngle");
            propertyMaxRadius = serializedObject.FindProperty("MaxRadius");
            propertyMinAngle = serializedObject.FindProperty("MinAngle");
            propertyRadius = serializedObject.FindProperty("Radius");
            propertyRadiusControlsHeight = serializedObject.FindProperty("RadiusControlsHeight");
            propertyRadiusControlsWidth = serializedObject.FindProperty("RadiusControlsWidth");
            propertyRadiusHeightFactor = serializedObject.FindProperty("RadiusHeightFactor");
            propertyRadiusWidthFactor = serializedObject.FindProperty("RadiusWidthFactor");
            propertyRotateChildren = serializedObject.FindProperty("RotateChildren");
            propertySpacing = serializedObject.FindProperty("Spacing");
            propertyStartAngle = serializedObject.FindProperty("StartAngle");
        }

        private void InitializeEditor()
        {
            FindProperties();

            root = new VisualElement();

            componentHeader =
                FluidComponentHeader.Get()
                    .SetElementSize(ElementSize.Large)
                    .SetAccentColor(accentColor)
                    .SetComponentNameText((ObjectNames.NicifyVariableName(nameof(UIRadialLayout))))
                    .SetIcon(radialLayoutIconTextures.ToList())
                    .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1049100324/UIRadialLayout?atlOrigin=eyJpIjoiNDU1ZWE0ZDE3YzM5NDJkYWI5NWM2M2I3MzkxZDZiZjEiLCJwIjoiYyJ9")
                    .AddYouTubeButton();

            radiusSlider = new Slider(0f, propertyMaxRadius.floatValue).SetName("Radius Slider").ResetLayout().SetStyleFlexGrow(1).BindToProperty(propertyRadius);
            radiusFloatField = new FloatField().SetName("Radius Value").ResetLayout().SetStyleWidth(60).BindToProperty(propertyRadius);
            maxRadiusFloatField = new FloatField().SetName("Max Radius Value").ResetLayout().SetStyleWidth(60).BindToProperty(propertyMaxRadius);
            minAngleSlider = new Slider(0f, 360f).SetName("Min Angle Slider").ResetLayout().SetStyleFlexGrow(1).BindToProperty(propertyMinAngle);
            minAngleFloatField = new FloatField().SetName("Min Angle Value").ResetLayout().SetStyleWidth(60).BindToProperty(propertyMinAngle);
            maxAngleSlider = new Slider(0f, 360f).SetName("Max Angle Slider").ResetLayout().SetStyleFlexGrow(1).BindToProperty(propertyMaxAngle);
            maxAngleFloatField = new FloatField().SetName("Max Angle Value").ResetLayout().SetStyleWidth(60).BindToProperty(propertyMaxAngle);
            startAngleSlider = new Slider(0f, 360f).SetName("Start Angle Slider").ResetLayout().SetStyleFlexGrow(1).BindToProperty(propertyStartAngle);
            startAngleFloatField = new FloatField().SetName("Start Angle Value").ResetLayout().SetStyleWidth(60).BindToProperty(propertyStartAngle);
            spacingFloatField = new FloatField().SetName("Spacing Value").ResetLayout().SetStyleWidth(60).BindToProperty(propertySpacing);
            clockwiseSwitch = FluidToggleSwitch.Get().SetName("Clockwise Switch").SetLabelText("Clockwise").SetToggleAccentColor(selectableAccentColor).BindToProperty(propertyClockwise);
            autoRebuildSwitch = FluidToggleSwitch.Get().SetName("Auto Rebuild Switch").SetLabelText("Auto Rebuild").SetToggleAccentColor(selectableAccentColor).BindToProperty(propertyAutoRebuild);

            childRotationHeader = FluidComponentHeader.Get().SetElementSize(ElementSize.Tiny).SetAccentColor(accentColor).SetComponentNameText("Child Rotation");
            rotateChildrenSwitch = FluidToggleSwitch.Get().SetName("Rotate Children Switch").SetTooltip("Rotate Children").SetToggleAccentColor(selectableAccentColor).BindToProperty(propertyRotateChildren);
            childRotationSlider = new Slider(0f, 360f).SetName("Child Rotation Slider").ResetLayout().SetStyleFlexGrow(1).BindToProperty(propertyChildRotation);
            childRotationFloatField = new FloatField().SetName("Child Rotation Value").ResetLayout().SetStyleWidth(60).BindToProperty(propertyChildRotation);

            childWidthHeader = FluidComponentHeader.Get().SetElementSize(ElementSize.Tiny).SetAccentColor(accentColor).SetComponentNameText("Child Width");
            controlChildWidthSwitch = FluidToggleSwitch.Get().SetName("Control Child Width Switch").SetTooltip("Control Child Width").SetToggleAccentColor(selectableAccentColor).BindToProperty(propertyControlChildWidth);
            childWidthFloatField = new FloatField().SetName("Child Width Value").ResetLayout().SetStyleWidth(60).BindToProperty(propertyChildWidth);
            radiusControlsWidthSwitch = FluidToggleSwitch.Get().SetName("Radius Controls Width Switch").SetTooltip("Radius Controls Width").SetToggleAccentColor(selectableAccentColor).BindToProperty(propertyRadiusControlsWidth);
            radiusWidthFactorSlider = new Slider(0f, 1f).SetName("Radius Width Factor Slider").ResetLayout().SetStyleFlexGrow(1).BindToProperty(propertyRadiusWidthFactor);
            radiusWidthFactorFloatField = new FloatField().SetName("Radius Width Factor Value").ResetLayout().SetStyleWidth(60).BindToProperty(propertyRadiusWidthFactor);

            childHeightHeader = FluidComponentHeader.Get().SetElementSize(ElementSize.Tiny).SetAccentColor(accentColor).SetComponentNameText("Child Height");
            controlChildHeightSwitch = FluidToggleSwitch.Get().SetName("Control Child Height Switch").SetTooltip("Control Child Height").SetToggleAccentColor(selectableAccentColor).BindToProperty(propertyControlChildHeight);
            childHeightFloatField = new FloatField().SetName("Child Height Value").ResetLayout().SetStyleWidth(60).BindToProperty(propertyChildHeight);
            radiusControlsHeightSwitch = FluidToggleSwitch.Get().SetName("Radius Controls Height Switch").SetTooltip("Radius Controls Height").SetToggleAccentColor(selectableAccentColor).BindToProperty(propertyRadiusControlsHeight);
            radiusHeightFactorSlider = new Slider(0f, 1f).SetName("Radius Height Factor Slider").ResetLayout().SetStyleFlexGrow(1).BindToProperty(propertyRadiusHeightFactor);
            radiusHeightFactorFloatField = new FloatField().SetName("Radius Height Factor Value").ResetLayout().SetStyleWidth(60).BindToProperty(propertyRadiusHeightFactor);

            spacingField =
                FluidField.Get()
                    .SetStyleFlexGrow(0)
                    .SetLabelText("Spacing")
                    .AddFieldContent(spacingFloatField);

            radiusField =
                FluidField.Get()
                    .SetLabelText("Radius")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(radiusSlider)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(radiusFloatField)
                    );

            maxRadiusField =
                FluidField.Get()
                    .SetStyleFlexGrow(0)
                    .SetLabelText("Max Radius")
                    .AddFieldContent(maxRadiusFloatField);

            minAngleField =
                FluidField.Get()
                    .SetLabelText("Min Angle")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(minAngleSlider)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(minAngleFloatField)
                    );

            maxAngleField =
                FluidField.Get()
                    .SetLabelText("Max Angle")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(maxAngleSlider)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(maxAngleFloatField)
                    );

            startAngleField =
                FluidField.Get()
                    .SetLabelText("Start Angle")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(startAngleSlider)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(startAngleFloatField)
                    );


            childRotationField =
                FluidField.Get()
                    .SetLabelText("Rotate Children")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(rotateChildrenSwitch)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(childRotationSlider)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(childRotationFloatField)
                    );


            childWidthField =
                FluidField.Get()
                    .SetStyleFlexGrow(0)
                    .SetLabelText("Control Child Width")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(controlChildWidthSwitch)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(childWidthFloatField)
                    );


            radiusWidthFactorField =
                FluidField.Get()
                    .SetLabelText("Radius Width Factor")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(radiusControlsWidthSwitch)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(radiusWidthFactorSlider)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(radiusWidthFactorFloatField)
                    );
            
            childHeightField =
                FluidField.Get()
                    .SetStyleFlexGrow(0)
                    .SetLabelText("Control Child Height")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(controlChildHeightSwitch)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(childHeightFloatField)
                    );


            radiusHeightFactorField =
                FluidField.Get()
                    .SetLabelText("Radius Height Factor")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(radiusControlsHeightSwitch)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(radiusHeightFactorSlider)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(radiusHeightFactorFloatField)
                    );

            radiusSlider.RegisterValueChangedCallback(evt => radiusSlider.value = evt.newValue.Round(0));
            radiusFloatField.RegisterValueChangedCallback(evt => radiusFloatField.value = evt.newValue.Round(0));
            maxRadiusFloatField.RegisterValueChangedCallback(evt =>
            {
                float value = Mathf.Max(evt.newValue, radiusSlider.lowValue);
                radiusSlider.highValue = value;
            });

            minAngleSlider.RegisterValueChangedCallback(evt => minAngleSlider.value = evt.newValue.Round(0));
            maxAngleSlider.RegisterValueChangedCallback(evt => maxAngleSlider.value = evt.newValue.Round(0));
            startAngleSlider.RegisterValueChangedCallback(evt => startAngleSlider.value = evt.newValue.Round(0));
            childRotationSlider.RegisterValueChangedCallback(evt => childRotationSlider.value = evt.newValue.Round(0));
           
            //child width
            childWidthFloatField.SetEnabled(propertyControlChildWidth.boolValue);
            radiusWidthFactorField.SetEnabled(propertyControlChildWidth.boolValue);
            controlChildWidthSwitch.SetOnValueChanged(evt =>
            {
                childWidthFloatField.SetEnabled(evt.newValue);
                radiusWidthFactorField.SetEnabled(evt.newValue);
            });
            
            radiusWidthFactorSlider.SetEnabled(propertyRadiusControlsWidth.boolValue);
            radiusWidthFactorFloatField.SetEnabled(propertyRadiusControlsWidth.boolValue);
            radiusControlsWidthSwitch.SetOnValueChanged(evt =>
            {
                radiusWidthFactorSlider.SetEnabled(evt.newValue);
                radiusWidthFactorFloatField.SetEnabled(evt.newValue);
            });
            
            //child height
            childHeightFloatField.SetEnabled(propertyControlChildHeight.boolValue);
            radiusHeightFactorField.SetEnabled(propertyControlChildHeight.boolValue);
            controlChildHeightSwitch.SetOnValueChanged(evt =>
            {
                childHeightFloatField.SetEnabled(evt.newValue);
                radiusHeightFactorField.SetEnabled(evt.newValue);
            });
            
            radiusHeightFactorSlider.SetEnabled(propertyRadiusControlsHeight.boolValue);
            radiusHeightFactorFloatField.SetEnabled(propertyRadiusControlsHeight.boolValue);
            radiusControlsHeightSwitch.SetOnValueChanged(evt =>
            {
                radiusHeightFactorSlider.SetEnabled(evt.newValue);
                radiusHeightFactorFloatField.SetEnabled(evt.newValue);
            });

        }

        private void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleMarginLeft(40)
                        .AddChild(clockwiseSwitch)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(autoRebuildSwitch)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(spacingField)
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(radiusField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(maxRadiusField)
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(minAngleField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(maxAngleField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(startAngleField)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(childRotationHeader) //child rotation
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(childRotationField)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(childWidthHeader) //child width
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(childWidthField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(radiusWidthFactorField)
                )
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(childHeightHeader) //child height
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(childHeightField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(radiusHeightFactorField)
                )
                ;
        }
    }
}
