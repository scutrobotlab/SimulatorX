// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIManager.Editors.Components.Internal;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Components;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Components
{
    [CustomEditor(typeof(UIToggleGroup), true)]
    [CanEditMultipleObjects]
    public class UIToggleGroupEditor : UISelectableBaseEditor
    {
        public override Color accentColor => EditorColors.UIManager.UIComponent;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.UIComponent;

        public UIToggle castedTarget => (UIToggle)target;
        public IEnumerable<UIToggle> castedTargets => targets.Cast<UIToggle>();

        public static IEnumerable<Texture2D> toggleIconTextures => EditorMicroAnimations.UIManager.Icons.UIToggleCheckbox;
        public static IEnumerable<Texture2D> toggleGroupIconTextures => EditorMicroAnimations.UIManager.Icons.UIToggleGroup;
        public static IEnumerable<Texture2D> unityEventIconTextures => EditorMicroAnimations.EditorUI.Icons.UnityEvent;
        public static IEnumerable<Texture2D> buttonClickIconTextures => EditorMicroAnimations.EditorUI.Icons.ButtonClick;
        public static IEnumerable<Texture2D> sortIconTextures => EditorMicroAnimations.EditorUI.Icons.SortAz;
        public static IEnumerable<Texture2D> toggleOnIconTextures => EditorMicroAnimations.EditorUI.Icons.ToggleON;
        public static IEnumerable<Texture2D> toggleOffIconTextures => EditorMicroAnimations.EditorUI.Icons.ToggleOFF;
        public static IEnumerable<Texture2D> toggleMixedIconTextures => EditorMicroAnimations.EditorUI.Icons.ToggleMixed;

        private VisualElement callbacksTab { get; set; }
        private EnabledIndicator callbacksTabIndicator { get; set; }

        private FluidAnimatedContainer callbacksAnimatedContainer { get; set; }

        private FluidField idField { get; set; }
        private FluidField toggleGroupField { get; set; }
        private FluidField toggleModeField { get; set; }
        private FluidField autoSortField { get; set; }

        private FluidToggleCheckbox overrideInteractabilityForTogglesToggle { get; set; }
        private FluidToggleCheckbox isOnToggle { get; set; }
        private FluidToggleCheckbox hasMixedValuesToggle { get; set; }

        private ObjectField toggleGroupObjectField { get; set; }

        private EnumField toggleGroupValueEnumField { get; set; }
        private EnumField modeEnumField { get; set; }
        private EnumField autoSortEnumField { get; set; }

        private ObjectField firstToggleObjectField { get; set; }
        private FluidField firstToggleField { get; set; }

        private SerializedProperty propertyId { get; set; }
        private SerializedProperty propertyBehaviours { get; set; }
        private SerializedProperty propertyOverrideInteractabilityForToggles { get; set; }
        private SerializedProperty propertyIsOn { get; set; }
        private SerializedProperty propertyToggleGroup { get; set; }
        private SerializedProperty propertyOnToggleOnCallback { get; set; }
        private SerializedProperty propertyOnInstantToggleOnCallback { get; set; }
        private SerializedProperty propertyOnToggleOffCallback { get; set; }
        private SerializedProperty propertyOnInstantToggleOffCallback { get; set; }
        private SerializedProperty propertyOnValueChangedCallback { get; set; }
        private SerializedProperty propertyOnToggleGroupMixedValuesCallback { get; set; }

        private SerializedProperty propertyHasMixedValues { get; set; }
        private SerializedProperty propertyToggleGroupValue { get; set; }
        private SerializedProperty propertyMode { get; set; }
        private SerializedProperty propertyAutoSort { get; set; }
        private SerializedProperty propertyFirstToggle { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            callbacksTabIndicator?.Recycle();

            idField?.Recycle();
            toggleGroupField?.Recycle();
            toggleModeField?.Recycle();
            autoSortField?.Recycle();

            firstToggleField?.Recycle();

            overrideInteractabilityForTogglesToggle?.Recycle();

            isOnToggle?.Recycle();
            hasMixedValuesToggle?.Recycle();

            callbacksAnimatedContainer?.Dispose();
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyId = serializedObject.FindProperty(nameof(UIToggleGroup.Id));
            propertyBehaviours = serializedObject.FindProperty("Behaviours");
            propertyOverrideInteractabilityForToggles = serializedObject.FindProperty("OverrideInteractabilityForToggles");
            propertyIsOn = serializedObject.FindProperty("IsOn");
            propertyToggleGroup = serializedObject.FindProperty("ToggleGroup");
            propertyOnToggleOnCallback = serializedObject.FindProperty(nameof(UIToggleGroup.OnToggleOnCallback));
            propertyOnInstantToggleOnCallback = serializedObject.FindProperty(nameof(UIToggleGroup.OnInstantToggleOnCallback));
            propertyOnToggleOffCallback = serializedObject.FindProperty(nameof(UIToggleGroup.OnToggleOffCallback));
            propertyOnInstantToggleOffCallback = serializedObject.FindProperty(nameof(UIToggleGroup.OnInstantToggleOffCallback));
            propertyOnValueChangedCallback = serializedObject.FindProperty(nameof(UIToggleGroup.OnValueChangedCallback));
            propertyOnToggleGroupMixedValuesCallback = serializedObject.FindProperty(nameof(UIToggleGroup.OnToggleGroupMixedValuesCallback));
            propertyHasMixedValues = serializedObject.FindProperty("HasMixedValues");
            propertyToggleGroupValue = serializedObject.FindProperty("ToggleGroupValue");
            propertyMode = serializedObject.FindProperty("Mode");
            propertyAutoSort = serializedObject.FindProperty("AutoSort");
            propertyFirstToggle = serializedObject.FindProperty(nameof(UIToggleGroup.FirstToggle));
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetAccentColor(accentColor)
                .SetComponentNameText((ObjectNames.NicifyVariableName(nameof(UIToggleGroup))))
                .SetIcon(toggleGroupIconTextures.ToList())
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048576017/UIToggleGroup?atlOrigin=eyJpIjoiOGM5NzYxYzhhOTJiNDQ4MTg0MTNiNmIwZTIwNjVmNGEiLCJwIjoiYyJ9")
                .AddYouTubeButton();

            idField = FluidField.Get().AddFieldContent(DesignUtils.NewPropertyField(propertyId));
            overrideInteractabilityForTogglesToggle = FluidToggleCheckbox.Get().SetLabelText("Override interactabilty for connected UIToggles").BindToProperty(propertyOverrideInteractabilityForToggles).SetTooltip("Override and control the interactable state for all the connected UIToggles");
            isOnToggle = FluidToggleCheckbox.Get().SetLabelText("Is On").BindToProperty(propertyIsOn);
            hasMixedValuesToggle = FluidToggleCheckbox.Get().SetLabelText("Has Mixed Values").BindToProperty(propertyHasMixedValues);
            hasMixedValuesToggle.SetEnabled(false);
            toggleGroupValueEnumField = DesignUtils.NewEnumField(propertyToggleGroupValue).SetStyleWidth(120);
            toggleGroupValueEnumField.SetEnabled(false);
            modeEnumField = DesignUtils.NewEnumField(propertyMode).SetStyleFlexGrow(1);
            {
                void UpdateToggleModeTooltip(UIToggleGroup.ControlMode value)
                {
                    switch (value)
                    {
                        case UIToggleGroup.ControlMode.Passive:
                            modeEnumField.SetTooltip("Toggle values are not enforced in any way (allows for all toggles to be OFF)");
                            break;
                        case UIToggleGroup.ControlMode.OneToggleOn:
                            modeEnumField.SetTooltip("Only one Toggle can be ON at any given time (allows for all toggles to be OFF)");
                            break;
                        case UIToggleGroup.ControlMode.OneToggleOnEnforced:
                            modeEnumField.SetTooltip("Only one Toggle can be ON at any given time (one Toggle will be forced ON at all times)");
                            break;
                        case UIToggleGroup.ControlMode.AnyToggleOnEnforced:
                            modeEnumField.SetTooltip("At least one Toggle needs to be ON at any given time (one Toggle will be forced ON at all times)");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(value), value, null);
                    }
                }

                UpdateToggleModeTooltip((UIToggleGroup.ControlMode)propertyMode.enumValueIndex);
                modeEnumField.RegisterValueChangedCallback(evt =>
                {
                    if (evt?.newValue == null) return;
                    UpdateToggleModeTooltip((UIToggleGroup.ControlMode)evt.newValue);
                });
            }
            toggleModeField = FluidField.Get().SetLabelText("Control Mode").SetIcon(toggleGroupIconTextures).AddFieldContent(modeEnumField);

            autoSortEnumField = DesignUtils.NewEnumField(propertyAutoSort).SetStyleFlexGrow(1);
            {
                void UpdateAutoSortTooltip(UIToggleGroup.SortMode value)
                {
                    switch (value)
                    {
                        case UIToggleGroup.SortMode.Disabled:
                            autoSortEnumField.SetTooltip("Auto sort is disabled");
                            break;
                        case UIToggleGroup.SortMode.Hierarchy:
                            autoSortEnumField.SetTooltip("Auto sort by sibling index (the order toggles appear in the Hierarchy)");
                            break;
                        case UIToggleGroup.SortMode.GameObjectName:
                            autoSortEnumField.SetTooltip("Auto sort by Toggle's GameObject name");
                            break;
                        case UIToggleGroup.SortMode.ToggleName:
                            autoSortEnumField.SetTooltip("Auto sort by Toggle Id Name (ignores category)");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(value), value, null);
                    }
                }

                UpdateAutoSortTooltip((UIToggleGroup.SortMode)propertyAutoSort.enumValueIndex);
                autoSortEnumField.RegisterValueChangedCallback(evt =>
                {
                    if (evt?.newValue == null) return;
                    UpdateAutoSortTooltip((UIToggleGroup.SortMode)evt.newValue);
                });
            }
            autoSortField = FluidField.Get().SetLabelText("Auto Sort").SetIcon(sortIconTextures).AddFieldContent(autoSortEnumField);

            toggleGroupObjectField =
                DesignUtils.NewObjectField(propertyToggleGroup, typeof(UIToggleGroup))
                    .SetStyleFlexGrow(1);

            toggleGroupObjectField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == null || evt.newValue != castedTarget)
                    return;
                toggleGroupObjectField.value = null;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            });

            toggleGroupField =
                FluidField.Get()
                    .SetLabelText("Toggle Group")
                    .SetIcon(toggleGroupIconTextures)
                    .AddFieldContent(toggleGroupObjectField);

            firstToggleObjectField =
                DesignUtils.NewObjectField(propertyFirstToggle, typeof(UIToggle))
                    .SetStyleFlexGrow(1);

            firstToggleField =
                FluidField.Get()
                    .SetLabelText("First Toggle")
                    .SetIcon(toggleIconTextures)
                    .AddFieldContent(firstToggleObjectField);

            InitializeCallbacks();
        }

        private void InitializeCallbacks()
        {
            callbacksAnimatedContainer = new FluidAnimatedContainer().SetName("Callbacks").SetClearOnHide(true).Hide(false);

            (callbacksTabButton, callbacksTabIndicator, callbacksTab) =
                DesignUtils.GetTabButtonForComponentSectionWithEnabledIndicator(unityEventIconTextures, DesignUtils.callbackSelectableColor, DesignUtils.callbacksColor);

            callbacksAnimatedContainer.SetOnShowCallback(() =>
            {
                FluidField GetField(SerializedProperty property, IEnumerable<Texture2D> iconTextures, string fieldLabelText, string fieldTooltip) =>
                    FluidField.Get()
                        .SetLabelText(fieldLabelText)
                        .SetTooltip(fieldTooltip)
                        .SetIcon(iconTextures)
                        .SetElementSize(ElementSize.Small)
                        .AddFieldContent(DesignUtils.NewPropertyField(property));

                callbacksAnimatedContainer
                    .fluidContainer
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(GetField(propertyOnToggleOnCallback, toggleOnIconTextures, "Toggle ON - Toggle became ON", "Callbacks triggered then the toggle value changed from OFF to ON"))
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(GetField(propertyOnInstantToggleOnCallback, toggleOnIconTextures, "Instant Toggle ON - Toggle became ON (without animations)", "Callbacks triggered then the toggle value changed from OFF to ON (without animations)"))
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(GetField(propertyOnToggleOffCallback, toggleOffIconTextures, "Toggle OFF - Toggle became OFF", "Callbacks triggered then the toggle value changed from ON to OFF"))
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(GetField(propertyOnInstantToggleOffCallback, toggleOffIconTextures, "Instant Toggle OFF - Toggle became OFF (without animations)", "Callbacks triggered then the toggle value changed from ON to OFF (without animations)"))
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(GetField(propertyOnValueChangedCallback, toggleIconTextures, "Toggle Value Changed", "Callbacks triggered then the toggle changes value"))
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(GetField(propertyOnToggleGroupMixedValuesCallback, toggleMixedIconTextures, "Toggle Group has Mixed Values - Executed when hasMixedValues becomes TRUE", "Callbacks triggered then the toggle group hasMixedValues becomes TRUE"))
                    .AddChild(DesignUtils.endOfLineBlock);

                callbacksAnimatedContainer.Bind(serializedObject);
            });

            callbacksTabIndicator.Toggle(hasCallbacks, false);

            IVisualElementScheduledItem callbacksScheduler =
                callbacksTabButton.schedule
                    .Execute(() => callbacksTabIndicator.Toggle(hasCallbacks, true))
                    .Every(250);

            callbacksScheduler.Pause();

            callbacksTabButton
                .SetLabelText("Callbacks")
                .SetOnValueChanged(evt =>
                {
                    callbacksAnimatedContainer.Toggle(evt.newValue);
                    callbacksTabIndicator.Toggle(hasCallbacks, true);
                    if (evt.newValue)
                        callbacksScheduler.Resume();
                    else
                        callbacksScheduler.Pause();
                });

            callbacksTabButton.AddToToggleGroup(toggleGroup);
        }

        private bool hasOnToggleOnCallback => castedTarget != null && castedTarget.OnToggleOnCallback != null && castedTarget.OnToggleOnCallback.Enabled && castedTarget.OnToggleOnCallback.hasEvents | castedTarget.OnToggleOnCallback.hasRunners;
        private bool hasOnInstantToggleOnCallback => castedTarget != null && castedTarget.OnInstantToggleOnCallback is { Enabled: true } && castedTarget.OnInstantToggleOnCallback.hasEvents | castedTarget.OnInstantToggleOnCallback.hasRunners;
        private bool hasOnToggleOffCallback => castedTarget != null && castedTarget.OnToggleOffCallback != null && castedTarget.OnToggleOffCallback.Enabled && castedTarget.OnToggleOffCallback.hasEvents | castedTarget.OnToggleOffCallback.hasRunners;
        private bool hasOnInstantToggleOffCallback => castedTarget != null && castedTarget.OnInstantToggleOffCallback is { Enabled: true } && castedTarget.OnInstantToggleOffCallback.hasEvents | castedTarget.OnInstantToggleOffCallback.hasRunners;
        private bool hasOnValueChangedCallback => castedTarget != null && castedTarget.OnValueChangedCallback?.GetPersistentEventCount() > 0;
        private bool hasCallbacks =>
            hasOnToggleOnCallback |
            hasOnInstantToggleOnCallback |
            hasOnToggleOffCallback |
            hasOnInstantToggleOffCallback |
            hasOnValueChangedCallback;

        protected override void Compose()
        {
            if (castedTarget == null)
                return;

            root
                .AddChild(componentHeader)
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleMargins(50, -4, DesignUtils.k_Spacing2X, DesignUtils.k_Spacing2X)
                        .AddChild(settingsTabButton)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(statesTabButton)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(navigationTabButton)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(callbacksTab)
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild
                        (
                            DesignUtils.SystemButton_RenameComponent
                            (
                                castedTarget.gameObject,
                                () => $"Toggle Group - {castedTarget.Id.Name}"
                            )
                        )
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild
                        (
                            DesignUtils.SystemButton_SortComponents
                            (
                                castedTarget.gameObject,
                                nameof(RectTransform),
                                nameof(Canvas),
                                nameof(CanvasGroup),
                                nameof(GraphicRaycaster),
                                nameof(UIToggleGroup)
                            )
                        )
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    settingsAnimatedContainer
                        .AddContent
                        (
                            DesignUtils.column
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(overrideInteractabilityForTogglesToggle)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild
                                (
                                    DesignUtils.row
                                        .AddChild(interactableCheckbox)
                                        .AddChild(DesignUtils.spaceBlock)
                                        .AddChild(deselectAfterPressCheckbox)
                                        .AddChild(DesignUtils.spaceBlock)
                                        .AddChild(GetStateButtons())
                                )
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild
                                (
                                    DesignUtils.row
                                        .AddChild(isOnToggle)
                                        .AddChild(DesignUtils.spaceBlock)
                                        .AddChild(hasMixedValuesToggle)
                                        .AddChild(DesignUtils.spaceBlock)
                                        .AddChild(toggleGroupValueEnumField)
                                )
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild
                                (
                                    DesignUtils.row
                                        .AddChild(toggleModeField)
                                        .AddChild(DesignUtils.spaceBlock)
                                        .AddChild(autoSortField)
                                )
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(firstToggleField)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(idField)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(toggleGroupField)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(DesignUtils.NewPropertyField(propertyBehaviours))
                                .AddChild(DesignUtils.endOfLineBlock)
                        )
                )
                .AddChild(statesAnimatedContainer)
                .AddChild(navigationAnimatedContainer)
                .AddChild(callbacksAnimatedContainer);
        }
    }
}
