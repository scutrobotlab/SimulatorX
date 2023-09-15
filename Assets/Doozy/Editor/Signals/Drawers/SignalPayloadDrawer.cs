// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.Signals.Drawers
{
    [CustomPropertyDrawer(typeof(SignalPayload), true)]
    public class SignalPayloadDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty propertyStreamId = property.FindPropertyRelative("StreamId");
            SerializedProperty propertySignalValueType = property.FindPropertyRelative("SignalValueType");
            SerializedProperty propertyIntegerValue = property.FindPropertyRelative("IntegerValue");
            SerializedProperty propertyBooleanValue = property.FindPropertyRelative("BooleanValue");
            SerializedProperty propertyFloatValue = property.FindPropertyRelative("FloatValue");
            SerializedProperty propertyStringValue = property.FindPropertyRelative("StringValue");
            SerializedProperty propertyColorValue = property.FindPropertyRelative("ColorValue");
            SerializedProperty propertyVector2Value = property.FindPropertyRelative("Vector2Value");
            SerializedProperty propertyVector3Value = property.FindPropertyRelative("Vector3Value");
            SerializedProperty propertyVector4Value = property.FindPropertyRelative("Vector4Value");

            var drawer = new VisualElement();

            EnumField signalValueTypeEnumField =
                DesignUtils.NewEnumField(propertySignalValueType)
                    .SetStyleWidth(120);

            FluidField valueTypeFluidField =
                FluidField.Get()
                    .SetStyleFlexGrow(0)
                    .SetStyleMarginLeft(38)
                    .SetLabelText("Value Type")
                    .AddFieldContent
                    (
                        signalValueTypeEnumField
                    );

            FluidField valueFluidField =
                FluidField.Get()
                    .SetLabelText("Value");

            drawer
                .AddChild(DesignUtils.NewPropertyField(propertyStreamId))
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(valueTypeFluidField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(valueFluidField)
                );


            UpdateValueFieldContainer((SignalPayload.ValueType)propertySignalValueType.enumValueIndex);
            
            signalValueTypeEnumField.RegisterValueChangedCallback(evt =>
            {
                if (evt?.newValue == null) return;
                UpdateValueFieldContainer((SignalPayload.ValueType)evt.newValue);
            });

            drawer.RegisterCallback<DetachFromPanelEvent>(evt =>
            {
                valueTypeFluidField?.Recycle();
                valueFluidField?.Recycle();
            });


            void UpdateValueFieldContainer(SignalPayload.ValueType valueType)
            {
                valueFluidField.ClearFieldContent();

                switch (valueType)
                {
                    case SignalPayload.ValueType.None:
                        valueFluidField.AddFieldContent
                        (
                            DesignUtils.NewFieldNameLabel("---")
                                .ResetLayout()
                                .SetStyleFlexGrow(1)
                                .SetStyleAlignSelf(Align.Center)
                        );
                        break;
                    case SignalPayload.ValueType.Integer:
                        valueFluidField.AddFieldContent
                        (
                            new IntegerField()
                                .ResetLayout()
                                .SetName("Integer Value")
                                .BindToProperty(propertyIntegerValue)
                        );
                        break;
                    case SignalPayload.ValueType.Boolean:
                        valueFluidField.AddFieldContent
                        (
                            FluidToggleCheckbox.Get()
                                .BindToProperty(propertyBooleanValue)
                        );
                        break;
                    case SignalPayload.ValueType.Float:
                        valueFluidField.AddFieldContent
                        (
                            new FloatField()
                                .ResetLayout()
                                .SetName("Float Value")
                                .BindToProperty(propertyFloatValue)
                        );
                        break;
                    case SignalPayload.ValueType.String:
                        valueFluidField.AddFieldContent
                        (
                            new TextField()
                                .ResetLayout()
                                .SetName("String Value")
                                .BindToProperty(propertyStringValue)
                        );
                        break;
                    case SignalPayload.ValueType.Color:
                        valueFluidField.AddFieldContent
                        (
                            new ColorField()
                                .ResetLayout()
                                .SetName("Color Value")
                                .BindToProperty(propertyColorValue)
                        );
                        break;
                    case SignalPayload.ValueType.Vector2:
                        valueFluidField.AddFieldContent
                        (
                            new Vector2Field()
                                .ResetLayout()
                                .SetName("Vector2 Value")
                                .BindToProperty(propertyVector2Value)
                        );
                        break;
                    case SignalPayload.ValueType.Vector3:
                        valueFluidField.AddFieldContent
                        (
                            new Vector3Field()
                                .ResetLayout()
                                .SetName("Vector3 Value")
                                .BindToProperty(propertyVector3Value)
                        );
                        break;
                    case SignalPayload.ValueType.Vector4:
                        valueFluidField.AddFieldContent
                        (
                            new Vector4Field()
                                .ResetLayout()
                                .SetName("Vector4 Value")
                                .BindToProperty(propertyVector4Value)
                        );
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                valueFluidField.Bind(property.serializedObject);

            }

            drawer.Bind(property.serializedObject);
            return drawer;

        }
    }
}
