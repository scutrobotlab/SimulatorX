// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Mody.Components;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.UIElements;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Mody.Drawers.ModyActions
{
    public abstract class BaseModyActionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        protected static Color accentColor => EditorColors.Mody.Action;
        protected static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Mody.Action;

        public static Color idleColor => EditorColors.Mody.StateIdle;
        public static Color startDelayColor => EditorColors.Mody.StateIdle;
        public static Color runningColor => EditorColors.Mody.StateActive;
        public static Color cooldownColor => EditorColors.Mody.StateCooldown;

        protected static EdgeValues animatedContainerFluidContainerPadding => new EdgeValues(22, DesignUtils.k_Spacing, 0, DesignUtils.k_EndOfLineSpacing);

        private static EnumField NewInvisibleEnumField(string bindingPath) => new EnumField().SetBindingPath(bindingPath).SetStyleDisplay(DisplayStyle.None);

        protected static FluidToggleIconButton NewExpandCollapseActionButton(FluidComponentHeader header, FluidAnimatedContainer openedDrawerAnimatedContainer)
        {
            FluidToggleIconButton button =
                FluidToggleIconButton.Get()
                    .SetElementSize(ElementSize.Tiny)
                    .SetStyleAlignSelf(Align.Center)
                    .SetAnimationTrigger(IconAnimationTrigger.OnValueChanged)
                    .SetStylePaddingLeft(DesignUtils.k_Spacing)
                    .SetStylePaddingRight(DesignUtils.k_Spacing)
                    .SetIcon(EditorMicroAnimations.EditorUI.Components.CarretRightToDown);

            button.SetToggleAccentColor(selectableAccentColor);
            button.iconReaction.SetDuration(FluidAnimatedContainer.k_ReactionDuration);

            button.SetOnValueChanged(evt =>
            {
                header.rotatedIconReaction?.Play();
                openedDrawerAnimatedContainer.Toggle(evt.newValue, evt.animateChange);
            });

            return button;
        }

        protected static FluidToggleSwitch NewDisableActionSwitch(SerializedProperty property)
        {
            SerializedProperty targetProperty = property.FindPropertyRelative("ActionEnabled");

            return FluidToggleSwitch.Get()
                .SetTooltip("Disable this Action and send it to the Available Actions drawer (on top)")
                .SetStyleFlexShrink(0)
                .SetStyleAlignSelf(Align.Center)
                .SetStyleMarginLeft(DesignUtils.k_Spacing)
                .BindToProperty(targetProperty);
            // .SetStyleDisplay(EditorApplication.isPlayingOrWillChangePlaymode ? DisplayStyle.None : DisplayStyle.Flex);
        }

        protected static FluidComponentHeader NewActionHeader(ModyAction modyAction)
        {
            string componentTypeName = "Action";
            if (modyAction.HasValue && modyAction.ValueType != null)
                componentTypeName += $" <{modyAction.ValueType.PrettyName()}>";

            FluidComponentHeader header =
                FluidComponentHeader.Get()
                    .SetElementSize(ElementSize.Tiny)
                    .SetRotatedIcon(EditorMicroAnimations.Mody.Icons.ModyAction)
                    .SetAccentColor(accentColor)
                    .SetComponentNameText(modyAction.actionName)
                    .SetComponentTypeText(componentTypeName);

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                header
                    .barRightContainer
                    .SetStyleDisplay(DisplayStyle.Flex)
                    .AddChild(GetActionStateIndicator(modyAction));
            }

            return header;
        }

        protected static void ConnectHeaderToExpandCollapseButton(FluidComponentHeader header, FluidToggleIconButton expandCollapseButton)
        {
            header.OnClick += () => expandCollapseButton.isOn = !expandCollapseButton.isOn;
            header.OnPointerEnter += evt => expandCollapseButton.selectionState = SelectionState.Highlighted;
            header.OnPointerLeave += evt => expandCollapseButton.selectionState = SelectionState.Normal;
            header.OnPointerDown += evt => expandCollapseButton.selectionState = SelectionState.Pressed;
            header.OnPointerUp += evt => expandCollapseButton.selectionState = SelectionState.Normal;
        }

        protected static VisualElement GetDrawerHeader(FluidToggleIconButton expandCollapseButton, FluidComponentHeader header, FluidToggleSwitch disableSwitch)
        {
            return
                DesignUtils.row
                    .AddChild(expandCollapseButton)
                    .AddChild(header)
                    .AddChild(disableSwitch);
        }

        protected static VisualElement AnimatedContainerContent(SerializedProperty property)
        {
            VisualElement content =
                DesignUtils.column
                    .AddChild(GetTriggersListView(property))
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild
                    (
                        DesignUtils.row
                            .AddChild(GetStartDelayFluidField(property))
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(GetDurationFluidField(property))
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(GetCooldownFluidField(property))
                    )
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(GetOnStartFluidField(property))
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(GetOnFinishPropertyField(property))
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild
                    (
                        DesignUtils.row
                            .AddChild(GetTimescaleFluidField(property))
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(GetStopAllActionsOnStartFluidField(property))
                    );

            content.Bind(property.serializedObject);
            return content;
        }

        protected static ModyActionStateIndicator GetActionStateIndicator(ModyAction modyAction)
        {
            var stateIndicator = new ModyActionStateIndicator();
            stateIndicator.UpdateTriggeredState(modyAction, ModyAction.TriggeredActionState.Disabled);
            modyAction.onStateChanged = state => stateIndicator.UpdateTriggeredState(modyAction, state);
            return stateIndicator;
        }

        protected static FluidListView GetTriggersListView(SerializedProperty property)
        {
            SerializedProperty actionNameProperty = property.FindPropertyRelative("ActionName");
            SerializedProperty arrayProperty = property.FindPropertyRelative("SignalsReceivers");

            var itemsSource = new List<SerializedProperty>();
            FluidListView fluidListView =
                new FluidListView()
                    .SetListDescription($"Action Triggers")
                    .SetDynamicListHeight(false)
                    .ShowEmptyListPlaceholder(true)
                    .UseSmallEmptyListPlaceholder(true)
                    .HideFooterWhenEmpty(true);

            fluidListView.emptyListPlaceholder.SetIcon(EditorMicroAnimations.EditorUI.Placeholders.EmptyListViewSmall);

            fluidListView.listView.selectionType = SelectionType.None;
            fluidListView.listView.itemsSource = itemsSource;

            #if UNITY_2021_2_OR_NEWER
            fluidListView.listView.fixedItemHeight = 70;
            fluidListView.SetPreferredListHeight((int)fluidListView.listView.fixedItemHeight * 4);
            #else
            fluidListView.listView.itemHeight = 70;
            fluidListView.SetPreferredListHeight(fluidListView.listView.itemHeight * 4);
            #endif

            fluidListView.listView.makeItem = () => new PropertyFluidListViewItem(fluidListView);
            fluidListView.listView.bindItem = (element, i) =>
            {
                var item = (PropertyFluidListViewItem)element;
                item.Update(i, itemsSource[i]);
                item.OnRemoveButtonClick += itemProperty =>
                {
                    int propertyIndex = 0;
                    for (int j = 0; j < arrayProperty.arraySize; j++)
                    {
                        if (itemProperty.propertyPath != arrayProperty.GetArrayElementAtIndex(j).propertyPath)
                            continue;
                        propertyIndex = j;
                        break;
                    }
                    arrayProperty.DeleteArrayElementAtIndex(propertyIndex);
                    arrayProperty.serializedObject.ApplyModifiedProperties();

                    UpdateItemsSource();
                };
            };

            //ADD ITEM BUTTON (plus button)
            fluidListView.addItemButton.SetTooltip($"Add a new Trigger for the '{actionNameProperty.stringValue}' Action");
            fluidListView.AddNewItemButtonCallback += () =>
            {
                arrayProperty.InsertArrayElementAtIndex(0);
                arrayProperty.serializedObject.ApplyModifiedProperties();
                UpdateItemsSource();
            };

            UpdateItemsSource();

            int arraySize = arrayProperty.arraySize;
            fluidListView.schedule.Execute(() =>
            {
                if (arrayProperty.arraySize == arraySize) return;
                arraySize = arrayProperty.arraySize;
                UpdateItemsSource();

            }).Every(100);

            void UpdateItemsSource()
            {
                itemsSource.Clear();
                for (int i = 0; i < arrayProperty.arraySize; i++)
                    itemsSource.Add(arrayProperty.GetArrayElementAtIndex(i));

                fluidListView.Update();
            }

            return fluidListView;
        }

        protected static VisualElement GetTriggersField(SerializedProperty property)
        {
            SerializedProperty signalReceiversProperty = property.FindPropertyRelative("SignalsReceivers");
            SerializedProperty actionNameProperty = property.FindPropertyRelative("ActionName");
            var container = new VisualElement();
            FluidField field = FluidField.Get();
            container.Add(field);

            container.RegisterCallback<AttachToPanelEvent>(evt => Undo.undoRedoPerformed += RefreshFieldContent);
            container.RegisterCallback<DetachFromPanelEvent>(evt => Undo.undoRedoPerformed -= RefreshFieldContent);

            RefreshFieldContent();

            void RefreshFieldContent()
            {
                field.ClearFieldContent();

                property.serializedObject.Update();

                field.AddFieldContent
                (
                    DesignUtils.row
                        .AddChild
                        (
                            new Label($"Action Triggers")
                                .ResetLayout()
                                .SetStyleMarginLeft(DesignUtils.k_Spacing * 2)
                                .SetStyleAlignSelf(Align.Center)
                                .SetStyleFontSize(10)
                                .SetStyleUnityFont(DesignUtils.fieldNameTextFont)
                        )
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild
                        (
                            FluidButton.Get()
                                .SetIcon(EditorMicroAnimations.EditorUI.Icons.Plus)
                                .SetAccentColor(EditorSelectableColors.Default.Add)
                                .SetElementSize(ElementSize.Small)
                                .SetTooltip($"Add a new Trigger for the '{actionNameProperty.stringValue}' Action")
                                .SetOnClick(() =>
                                {
                                    signalReceiversProperty.InsertArrayElementAtIndex(0);
                                    property.serializedObject.ApplyModifiedProperties();
                                    RefreshFieldContent();
                                })
                        )
                );

                for (int i = 0; i < signalReceiversProperty.arraySize; i++)
                {
                    int index = i;
                    SerializedProperty arrayElementAtIndex = signalReceiversProperty.GetArrayElementAtIndex(index);
                    field.AddFieldContent
                    (
                        DesignUtils.row
                            .SetStyleMarginBottom(DesignUtils.k_Spacing / 2f)
                            .AddChild(DesignUtils.NewPropertyField(arrayElementAtIndex.propertyPath))
                            .AddChild
                            (
                                FluidButton.Get()
                                    .SetIcon(EditorMicroAnimations.EditorUI.Icons.Minus)
                                    .SetAccentColor(EditorSelectableColors.Default.Remove)
                                    .SetStyleAlignSelf(Align.Center)
                                    .SetElementSize(ElementSize.Small)
                                    .SetTooltip($"Remove Trigger")
                                    .SetOnClick(() =>
                                    {
                                        signalReceiversProperty.DeleteArrayElementAtIndex(index);
                                        property.serializedObject.ApplyModifiedProperties();
                                        RefreshFieldContent();
                                    }))
                    );
                }

                field.Bind(property.serializedObject);
            }

            return container;
        }

        private static FluidField TimeField(SerializedProperty property, string targetPropertyName, Color enabledColor, string fieldLabelText, string fieldTooltip, IEnumerable<Texture2D> fieldIconTextures)
        {
            SerializedProperty targetProperty = property.FindPropertyRelative(targetPropertyName);
            EnabledIndicator indicator = EnabledIndicator.Get().SetIcon(fieldIconTextures).SetEnabledColor(enabledColor).SetSize(18);
            FloatField floatField = new FloatField().ResetLayout().BindToProperty(targetProperty).SetStyleFlexGrow(1);
            floatField.RegisterValueChangedCallback(evt => indicator.Toggle(evt.newValue > 0, true));
            indicator.Toggle(targetProperty.floatValue > 0, false);

            return FluidField.Get()
                .SetLabelText(fieldLabelText)
                .SetTooltip(fieldTooltip)
                .SetElementSize(ElementSize.Small)
                .AddFieldContent
                (
                    DesignUtils.row.SetStyleFlexGrow(0)
                        .AddChild(indicator.SetStyleMarginRight(DesignUtils.k_Spacing))
                        .AddChild(floatField)
                );
        }

        protected static FluidField GetStartDelayFluidField(SerializedProperty property) =>
            TimeField
            (
                property,
                "ActionStartDelay",
                startDelayColor,
                "Start Delay",
                "Time interval before the Action executes its task, after it started running",
                EditorMicroAnimations.EditorUI.Icons.StartDelay
            );

        protected static FluidField GetDurationFluidField(SerializedProperty property) =>
            TimeField
            (
                property,
                "ActionDuration",
                runningColor,
                "Duration",
                "Running time from start to finish. Does not include StartDelay. At 0 (zero) the Action's task happens instantly",
                EditorMicroAnimations.EditorUI.Icons.Duration
            );

        protected static FluidField GetCooldownFluidField(SerializedProperty property) =>
            TimeField
            (
                property,
                "ActionCooldown",
                cooldownColor,
                "Cooldown",
                "Cooldown time after the Action ran. During this time, the Action cannot Start running again",
                EditorMicroAnimations.EditorUI.Icons.Cooldown
            );

        protected static FluidField GetTimescaleFluidField(SerializedProperty property)
        {
            SerializedProperty targetProperty = property.FindPropertyRelative("ActionTimescale");
            EnumField enumField =
                new EnumField()
                    .ResetLayout()
                    .BindToProperty(targetProperty)
                    .SetStyleMinWidth(94);

            FluidField field =
                FluidField.Get()
                    .SetLabelText("Timescale")
                    .SetTooltip
                    (
                        "Determine if the Action's timers will be affected by the application's timescale" +
                        "\n\nTimescale.Independent - (Realtime)\nNot affected by the application's timescale value" +
                        "\n\nTimescale.Dependent - (Application Time)\nAffected by the application's timescale value"
                    )
                    .SetElementSize(ElementSize.Small)
                    .SetIcon(EditorMicroAnimations.EditorUI.Icons.TimeScale)
                    .AddFieldContent(enumField);
            return field;
        }

        protected static FluidField GetStopAllActionsOnStartFluidField(SerializedProperty property)
        {
            SerializedProperty targetProperty = property.FindPropertyRelative("ActionOnStartStopOtherActions");
            FluidToggleCheckbox checkbox =
                FluidToggleCheckbox.Get()
                    .SetLabelText("Stop Other Actions")
                    .BindToProperty(targetProperty.propertyPath);

            FluidField field =
                FluidField.Get()
                    .SetLabelText("OnStart")
                    .SetTooltip("Stop for all other Actions, on this Module (MonoBehaviour), when this Action starts running")
                    .SetElementSize(ElementSize.Small)
                    .AddFieldContent(checkbox);

            return field;
        }


        private static FluidField GetOnStartOrOnFinishFluidField(SerializedProperty property, string targetPropertyName, IEnumerable<Texture2D> indicatorIconTextures, string fieldTooltip)
        {
            SerializedProperty targetProperty = property.FindPropertyRelative(targetPropertyName);
            SerializedProperty enabledProperty = targetProperty.FindPropertyRelative("Enabled");
            PropertyField propertyField = DesignUtils.NewPropertyField(targetProperty.propertyPath);
            EnabledIndicator enabledIndicator =
                EnabledIndicator.Get()
                    .SetIcon(indicatorIconTextures)
                    .SetEnabledColor(runningColor).SetSize(20);

            enabledIndicator.Toggle(enabledProperty.boolValue, false);

            FluidField field =
                FluidField.Get()
                    .SetTooltip(fieldTooltip)
                    .SetElementSize(ElementSize.Small)
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(enabledIndicator)
                            .AddChild(propertyField)
                    );

            Toggle invisibleToggle = DesignUtils.NewToggle(enabledProperty.propertyPath, true);
            field.AddFieldContent(invisibleToggle);
            invisibleToggle.RegisterValueChangedCallback(evt => enabledIndicator.Toggle(evt.newValue, true));

            return field;
        }


        protected static FluidField GetOnStartFluidField(SerializedProperty property) =>
            GetOnStartOrOnFinishFluidField
            (
                property,
                "OnStartEvents",
                EditorMicroAnimations.EditorUI.Icons.EventsOnStart,
                "Events triggered when this Action starts running"
            );

        protected static FluidField GetOnFinishPropertyField(SerializedProperty property) =>
            GetOnStartOrOnFinishFluidField
            (
                property,
                "OnFinishEvents",
                EditorMicroAnimations.EditorUI.Icons.EventsOnFinish,
                "Events triggered when this Action finished running"
            );

    }
}
