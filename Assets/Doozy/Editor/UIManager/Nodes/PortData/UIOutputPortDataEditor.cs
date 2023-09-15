// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.Common.Drawers;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Nody;
using Doozy.Editor.Reactor.Internal;
using Doozy.Editor.Signals.ScriptableObjects;
using Doozy.Editor.Signals.Windows;
using Doozy.Editor.UIElements;
using Doozy.Editor.UIManager.ScriptableObjects;
using Doozy.Editor.UIManager.Windows;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager;
using Doozy.Runtime.UIManager.Nodes.PortData;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Nodes.PortData
{
    public class UIOutputPortDataEditor : VisualElement
    {
        private FlowNodeView nodeView { get; }
        private FlowPort port { get; }
        private UIOutputPortData data { get; set; }

        private EnabledIndicator connectionIndicator { get; set; }

        private Image icon { get; set; }
        private Texture2DReaction iconReaction { get; set; }
        private EnumField triggerEnumField { get; set; }
        private FloatField timeDelayFloatField { get; set; }
        private EnumField commandToggleEnumField { get; set; }
        private EnumField commandShowHideEnumField { get; set; }

        private VisualElement settingsContainer { get; set; }

        public UIOutputPortDataEditor(FlowPort port, FlowNodeView nodeView)
        {
            this.port = port;
            this.nodeView = nodeView;
            data = this.port.GetValue<UIOutputPortData>();

            InitializeEditor();
            Compose();

            Undo.undoRedoPerformed += RefreshData;
        }

        private void RefreshData()
        {
            if (port == null) return;
            if (port.node == null) return;
            connectionIndicator?.Toggle(port.isConnected, true);
            data = port.GetValue<UIOutputPortData>();
            {

            }
            port.RefreshPortEditor();
            port.RefreshPortView();
        }

        private void InitializeEditor()
        {
            connectionIndicator =
                EnabledIndicator.Get()
                    .SetIcon(EditorMicroAnimations.Nody.Icons.One)
                    .SetEnabledColor(EditorColors.Nody.Output)
                    .SetSize(20)
                    .Toggle(port.isConnected, false);

            icon =
                new Image()
                    .SetName("Icon")
                    .ResetLayout()
                    .SetStyleSize(24)
                    .SetStyleAlignSelf(Align.Center)
                    .SetStyleBackgroundImageTintColor(EditorColors.Default.FieldIcon);

            iconReaction =
                icon.GetTexture2DReaction().SetEditorHeartbeat();

            triggerEnumField =
                new EnumField(data.Trigger)
                    .SetName("Trigger")
                    .ResetLayout()
                    .SetStyleFlexGrow(1)
                    .SetStyleMarginLeft(10);
            
            triggerEnumField.SetValueWithoutNotify(data.Trigger);
            triggerEnumField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(port.node, "Set trigger");                    //save for undo
                data.Trigger = (UIOutputPortData.TriggerCondition)evt.newValue; //update value
                SaveChangedValue();
            });

            
            timeDelayFloatField =
                new FloatField()
                    .SetName("Time Delay Value")
                    .ResetLayout()
                    .SetStyleDisplay(DisplayStyle.None)
                    .SetStyleFlexGrow(1)
                    .SetStyleMarginLeft(DesignUtils.k_Spacing);

            timeDelayFloatField.SetValueWithoutNotify(data.TimeDelay);
            timeDelayFloatField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(port.node, "Set toggle trigger"); //save for undo
                data.TimeDelay = evt.newValue;                      //update value
                SaveChangedValue();
            });
            
            commandToggleEnumField =
                new EnumField(data.CommandToggle)
                    .SetName("Command Toggle")
                    .ResetLayout()
                    .SetStyleFlexShrink(0)
                    .SetStyleDisplay(DisplayStyle.None)
                    .SetStyleMarginLeft(DesignUtils.k_Spacing)
                    .SetStyleWidth(48);

            commandToggleEnumField.SetValueWithoutNotify(data.CommandToggle);
            commandToggleEnumField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(port.node, "Set toggle mode");  //save for undo
                data.CommandToggle = (CommandToggle)evt.newValue; //update value
                SaveChangedValue();
            });

            
            commandShowHideEnumField =
                new EnumField(data.CommandShowHide)
                    .SetName("Command ShowHide")
                    .ResetLayout()
                    .SetStyleFlexShrink(0)
                    .SetStyleDisplay(DisplayStyle.None)
                    .SetStyleMarginLeft(DesignUtils.k_Spacing)
                    .SetStyleWidth(56);
            
            commandShowHideEnumField.SetValueWithoutNotify(data.CommandToggle);
            commandShowHideEnumField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(port.node, "Set show/hide mode");   //save for undo
                data.CommandShowHide = (CommandShowHide)evt.newValue; //update value
                SaveChangedValue();
            });

            settingsContainer =
                new VisualElement()
                    .SetStyleFlexGrow(1)
                    .SetStyleMarginLeft(-2)
                    .SetStyleMarginTop(DesignUtils.k_Spacing);

            Undo.undoRedoPerformed += () =>
            {
                if (port == null) return;
                if (port.node == null) return;
                data = port.GetValue<UIOutputPortData>();
                triggerEnumField.SetValueWithoutNotify(data.Trigger);
                connectionIndicator.Toggle(port.isConnected, true);
                EditorUtility.SetDirty(port.node);
                RefreshPortEditor();
                iconReaction.Play();
            };

            RefreshPortEditor();
        }

        private void SaveChangedValue(bool refreshPortEditor = true)
        {
            port.SetValue(data);                        // apply value
            EditorUtility.SetDirty(port.node);          // mark dirty
            port.node.RefreshNodeView();                // update node view
            iconReaction.Play();                        // animate icon
            if (refreshPortEditor) RefreshPortEditor(); // refresh editor
        }

        private void Compose()
        {
            this
                .SetName("Port Data")
                .SetStyleFlexDirection(FlexDirection.Row)
                .SetStyleBackgroundColor(EditorColors.Default.FieldBackground)
                .SetStyleBorderRadius(DesignUtils.k_Spacing)
                .SetStylePadding(DesignUtils.k_Spacing)
                .SetStyleMarginBottom(DesignUtils.k_Spacing)
                .AddChild(connectionIndicator)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    DesignUtils.column
                        .AddChild
                        (
                            DesignUtils.row
                                .AddChild(icon)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(triggerEnumField)
                                .AddChild(timeDelayFloatField)
                                .AddChild(commandToggleEnumField)
                                .AddChild(commandShowHideEnumField)
                        )
                        .AddChild
                        (
                            DesignUtils.row
                                .AddChild(settingsContainer)
                        )
                )
                ;
        }

        public void RefreshPortEditor()
        {
            settingsContainer.RecycleAndClear();
            iconReaction.SetTextures(GetTextures(data.Trigger));
            timeDelayFloatField.SetStyleDisplay(data.Trigger == UIOutputPortData.TriggerCondition.TimeDelay ? DisplayStyle.Flex : DisplayStyle.None);
            commandToggleEnumField.SetStyleDisplay(data.Trigger == UIOutputPortData.TriggerCondition.UIToggle ? DisplayStyle.Flex : DisplayStyle.None);
            commandShowHideEnumField.SetStyleDisplay(data.Trigger == UIOutputPortData.TriggerCondition.UIView ? DisplayStyle.Flex : DisplayStyle.None);

            settingsContainer.AddChild
            (
                data.Trigger == UIOutputPortData.TriggerCondition.TimeDelay ? GetTimeDelayContainer() :
                data.Trigger == UIOutputPortData.TriggerCondition.Signal ? GetSignalContainer() :
                data.Trigger == UIOutputPortData.TriggerCondition.UIButton ? GetUIButtonContainer() :
                data.Trigger == UIOutputPortData.TriggerCondition.UIToggle ? GetUIToggleContainer() :
                data.Trigger == UIOutputPortData.TriggerCondition.UIView ? GetUIViewContainer() : throw new ArgumentOutOfRangeException()
            );
        }

        private VisualElement GetTimeDelayContainer()
        {
            VisualElement container =
                new VisualElement()
                    .SetName("TimeDelay Container");
            return container;
        }

        private VisualElement GetSignalContainer()
        {
            VisualElement container =
                new VisualElement()
                    .SetName("Signal Container");

            EnumField signalValueTypeEnumField =
                new EnumField(SignalPayload.ValueType.None)
                    .SetName("Signal ValueType")
                    .ResetLayout()
                    .SetStyleWidth(80);

            signalValueTypeEnumField.SetValueWithoutNotify(data.SignalPayload.signalValueType);
            signalValueTypeEnumField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(port.node, "Set Signal ValueType");
                data.SignalPayload.signalValueType = (SignalPayload.ValueType)evt.newValue;
                SaveChangedValue(false);
            });

            FluidField valueTypeFluidField =
                FluidField.Get()
                    .SetStyleFlexGrow(0)
                    .SetStyleMarginLeft(36)
                    .SetLabelText("Value Type")
                    .AddFieldContent
                    (
                        signalValueTypeEnumField
                    );

            FluidField valueFluidField =
                FluidField.Get()
                    .SetLabelText("Value");


            VisualElement streamIdElement = 
                CategoryNameIdUtils.CreateView
                (
                    () => data.SignalPayload.streamId.Category,
                    categoryValue =>
                    {
                        Undo.RecordObject(port.node, "Set Category");
                        data.SignalPayload.streamId.Category = categoryValue;
                        SaveChangedValue(false);
                    },
                    () => data.SignalPayload.streamId.Name,
                    nameValue =>
                    {
                        Undo.RecordObject(port.node, "Set Name");
                        data.SignalPayload.streamId.Name = nameValue;
                        SaveChangedValue(false);
                    },
                    () => data.SignalPayload.streamId.Custom,
                    customValue =>
                    {
                        Undo.RecordObject(port.node, "Set Custom");
                        data.SignalPayload.streamId.Custom = customValue;
                        SaveChangedValue(false);
                    },
                    () => StreamIdDatabase.instance.database.GetCategories(),
                    targetCategory => StreamIdDatabase.instance.database.GetNames(targetCategory),
                    EditorMicroAnimations.Signals.Icons.StreamDatabase,
                    StreamsDatabaseWindow.Open,
                    "Open Streams Database Window",
                    StreamIdDatabase.instance,
                    EditorSelectableColors.Signals.Stream
                )
                .SetName("UIButton Container");

            UpdateValueFieldContainer(data.SignalPayload.signalValueType);

            signalValueTypeEnumField.RegisterValueChangedCallback(evt =>
            {
                if (evt?.newValue == null) return;
                UpdateValueFieldContainer((SignalPayload.ValueType)evt.newValue);
            });

            container.RegisterCallback<DetachFromPanelEvent>(evt =>
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
                        IntegerField integerField = new IntegerField().ResetLayout().SetName("Int Value");
                        integerField.isDelayed = true;
                        integerField.SetValueWithoutNotify(data.SignalPayload.integerValue);
                        integerField.RegisterValueChangedCallback(evt =>
                        {
                            Undo.RecordObject(port.node, "Set Signal Value");
                            data.SignalPayload.integerValue = evt.newValue;
                            SaveChangedValue(false);
                        });
                        valueFluidField.AddFieldContent(integerField);
                        break;
                    case SignalPayload.ValueType.Boolean:
                        FluidToggleCheckbox checkbox = FluidToggleCheckbox.Get().SetName("Bool Value");
                        checkbox.SetIsOn(data.SignalPayload.booleanValue, false);
                        checkbox.SetOnValueChanged(evt =>
                        {
                            Undo.RecordObject(port.node, "Set Signal Value");
                            data.SignalPayload.booleanValue = evt.newValue;
                            SaveChangedValue(false);
                        });
                        valueFluidField.AddFieldContent(checkbox);
                        break;
                    case SignalPayload.ValueType.Float:
                        FloatField floatField = new FloatField().ResetLayout().SetName("Float Value");
                        floatField.isDelayed = true;
                        floatField.SetValueWithoutNotify(data.SignalPayload.floatValue);
                        floatField.RegisterValueChangedCallback(evt =>
                        {
                            Undo.RecordObject(port.node, "Set Signal Value");
                            data.SignalPayload.floatValue = evt.newValue;
                            SaveChangedValue(false);
                        });
                        valueFluidField.AddFieldContent(floatField);
                        break;
                    case SignalPayload.ValueType.String:
                        TextField textField = new TextField().ResetLayout().SetName("Text Value");
                        textField.isDelayed = true;
                        textField.SetValueWithoutNotify(data.SignalPayload.stringValue);
                        textField.RegisterValueChangedCallback(evt =>
                        {
                            Undo.RecordObject(port.node, "Set Signal Value");
                            data.SignalPayload.stringValue = evt.newValue;
                            SaveChangedValue(false);
                        });
                        valueFluidField.AddFieldContent(textField);
                        break;
                    case SignalPayload.ValueType.Color:
                        ColorField colorField = new ColorField().ResetLayout().SetName("Color Value");
                        colorField.SetValueWithoutNotify(data.SignalPayload.colorValue);
                        colorField.RegisterValueChangedCallback(evt =>
                        {
                            Undo.RecordObject(port.node, "Set Signal Value");
                            data.SignalPayload.colorValue = evt.newValue;
                            SaveChangedValue(false);
                        });
                        valueFluidField.AddFieldContent(colorField);
                        break;
                    case SignalPayload.ValueType.Vector2:
                        Vector2Field vector2Field = new Vector2Field().ResetLayout().SetName("Vector2 Value");
                        vector2Field.SetValueWithoutNotify(data.SignalPayload.vector2Value);
                        vector2Field.RegisterValueChangedCallback(evt =>
                        {
                            Undo.RecordObject(port.node, "Set Signal Value");
                            data.SignalPayload.vector2Value = evt.newValue;
                            SaveChangedValue(false);
                        });
                        valueFluidField.AddFieldContent(vector2Field);
                        break;
                    case SignalPayload.ValueType.Vector3:
                        Vector3Field vector3Field = new Vector3Field().ResetLayout().SetName("Vector3 Value");
                        vector3Field.SetValueWithoutNotify(data.SignalPayload.vector3Value);
                        vector3Field.RegisterValueChangedCallback(evt =>
                        {
                            Undo.RecordObject(port.node, "Set Signal Value");
                            data.SignalPayload.vector3Value = evt.newValue;
                            SaveChangedValue(false);
                        });
                        valueFluidField.AddFieldContent(vector3Field);
                        break;
                    case SignalPayload.ValueType.Vector4:
                        Vector4Field vector4Field = new Vector4Field().ResetLayout().SetName("Vector4 Value");
                        vector4Field.SetValueWithoutNotify(data.SignalPayload.vector4Value);
                        vector4Field.RegisterValueChangedCallback(evt =>
                        {
                            Undo.RecordObject(port.node, "Set Signal Value");
                            data.SignalPayload.vector4Value = evt.newValue;
                            SaveChangedValue(false);
                        });
                        valueFluidField.AddFieldContent(vector4Field);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            container
                .AddChild(streamIdElement)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(valueTypeFluidField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(valueFluidField)
                );

            return container;
        }

        private VisualElement GetUIButtonContainer()
        {
            return CategoryNameIdUtils.CreateView
                (
                    () => data.ButtonId.Category,
                    categoryValue =>
                    {
                        Undo.RecordObject(port.node, "Set Category");
                        data.ButtonId.Category = categoryValue;
                        SaveChangedValue(false);
                    },
                    () => data.ButtonId.Name,
                    nameValue =>
                    {
                        Undo.RecordObject(port.node, "Set Name");
                        data.ButtonId.Name = nameValue;
                        SaveChangedValue(false);
                    },
                    () => data.ButtonId.Custom,
                    customValue =>
                    {
                        Undo.RecordObject(port.node, "Set Custom");
                        data.ButtonId.Custom = customValue;
                        SaveChangedValue(false);
                    },
                    () => UIButtonIdDatabase.instance.database.GetCategories(),
                    targetCategory => UIButtonIdDatabase.instance.database.GetNames(targetCategory),
                    EditorMicroAnimations.UIManager.Icons.ButtonsDatabase,
                    ButtonsDatabaseWindow.Open,
                    "Open Buttons Database Window",
                    UIButtonIdDatabase.instance,
                    EditorSelectableColors.UIManager.UIComponent
                )
                .SetName("UIButton Container");
        }

        private VisualElement GetUIToggleContainer()
        {
            return CategoryNameIdUtils.CreateView
                (
                    () => data.ToggleId.Category,
                    categoryValue =>
                    {
                        Undo.RecordObject(port.node, "Set Category");
                        data.ToggleId.Category = categoryValue;
                        SaveChangedValue(false);
                    },
                    () => data.ToggleId.Name,
                    nameValue =>
                    {
                        Undo.RecordObject(port.node, "Set Name");
                        data.ToggleId.Name = nameValue;
                        SaveChangedValue(false);
                    },
                    () => data.ToggleId.Custom,
                    customValue =>
                    {
                        Undo.RecordObject(port.node, "Set Custom");
                        data.ToggleId.Custom = customValue;
                        SaveChangedValue(false);
                    },
                    () => UIToggleIdDatabase.instance.database.GetCategories(),
                    targetCategory => UIToggleIdDatabase.instance.database.GetNames(targetCategory),
                    EditorMicroAnimations.UIManager.Icons.TogglesDatabase,
                    TogglesDatabaseWindow.Open,
                    "Open Toggles Database Window",
                    UIToggleIdDatabase.instance,
                    EditorSelectableColors.UIManager.UIComponent
                )
                .SetName("UIToggle Container");
        }

        private VisualElement GetUIViewContainer()
        {
            return CategoryNameIdUtils.CreateView
                (
                    () => data.ViewId.Category,
                    categoryValue =>
                    {
                        Undo.RecordObject(port.node, "Set Category");
                        data.ViewId.Category = categoryValue;
                        SaveChangedValue(false);
                    },
                    () => data.ViewId.Name,
                    nameValue =>
                    {
                        Undo.RecordObject(port.node, "Set Name");
                        data.ViewId.Name = nameValue;
                        SaveChangedValue(false);
                    },
                    () => data.ViewId.Custom,
                    customValue =>
                    {
                        Undo.RecordObject(port.node, "Set Custom");
                        data.ViewId.Custom = customValue;
                        SaveChangedValue(false);
                    },
                    () => UIViewIdDatabase.instance.database.GetCategories(),
                    targetCategory => UIViewIdDatabase.instance.database.GetNames(targetCategory),
                    EditorMicroAnimations.UIManager.Icons.ViewsDatabase,
                    ViewsDatabaseWindow.Open,
                    "Open Views Database Window",
                    UIViewIdDatabase.instance,
                    EditorSelectableColors.UIManager.UIComponent
                )
                .SetName("UIToggle Container");
        }

        private static IEnumerable<Texture2D> GetTextures(UIOutputPortData.TriggerCondition trigger)
        {
            switch (trigger)
            {
                case UIOutputPortData.TriggerCondition.TimeDelay:
                    return EditorMicroAnimations.EditorUI.Icons.Hourglass;
                case UIOutputPortData.TriggerCondition.Signal:
                    return EditorMicroAnimations.Signals.Icons.SignalStream;
                case UIOutputPortData.TriggerCondition.UIButton:
                    return EditorMicroAnimations.UIManager.Icons.Buttons;
                case UIOutputPortData.TriggerCondition.UIToggle:
                    return EditorMicroAnimations.UIManager.Icons.UIToggleCheckbox;
                case UIOutputPortData.TriggerCondition.UIView:
                    return EditorMicroAnimations.UIManager.Icons.Views;
                default:
                    throw new ArgumentOutOfRangeException(nameof(trigger), trigger, null);
            }
        }
    }
}
