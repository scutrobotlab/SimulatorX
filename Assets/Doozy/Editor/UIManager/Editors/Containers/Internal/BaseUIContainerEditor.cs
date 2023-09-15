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
using Doozy.Editor.UIElements;
using Doozy.Runtime.Mody;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager;
using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.UIManager.Events;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Containers.Internal
{
    public abstract class BaseUIContainerEditor : UnityEditor.Editor
    {
        public static IEnumerable<Texture2D> arrowDownIconTextures => EditorMicroAnimations.EditorUI.Arrows.ArrowDown;
        public static IEnumerable<Texture2D> arrowUpIconTextures => EditorMicroAnimations.EditorUI.Arrows.ArrowUp;
        public static IEnumerable<Texture2D> hideIconTextures => EditorMicroAnimations.EditorUI.Icons.Hide;
        public static IEnumerable<Texture2D> resetIconTextures => EditorMicroAnimations.EditorUI.Icons.Reset;
        public static IEnumerable<Texture2D> settingsIconTextures => EditorMicroAnimations.EditorUI.Icons.Settings;
        public static IEnumerable<Texture2D> showIconTextures => EditorMicroAnimations.EditorUI.Icons.Show;
        public static IEnumerable<Texture2D> unityIconTextures => EditorMicroAnimations.EditorUI.Icons.UnityEvent;

        protected virtual Color accentColor => EditorColors.UIManager.UIComponent;
        protected virtual EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.UIComponent;

        protected UIContainer uiContainer => (UIContainer)target;
        protected IEnumerable<UIContainer> uiContainers => targets.Cast<UIContainer>();

        protected ModyEvent onShowCallback => uiContainer.OnShowCallback;
        protected bool hasOnShowCallback => onShowCallback is { Enabled: true } && onShowCallback.hasEvents | onShowCallback.hasRunners;

        protected ModyEvent onVisibleCallback => uiContainer.OnVisibleCallback;
        protected bool hasOnVisibleCallback => onVisibleCallback is { Enabled: true } && onVisibleCallback.hasEvents | onVisibleCallback.hasRunners;

        protected ModyEvent onHideCallback => uiContainer.OnHideCallback;
        protected bool hasOnHideCallback => onHideCallback is { Enabled: true } && onHideCallback.hasEvents | onHideCallback.hasRunners;

        protected ModyEvent onHiddenCallback => uiContainer.OnHiddenCallback;
        protected bool hasOnHiddenCallback => onHiddenCallback is { Enabled: true } && onHiddenCallback.hasEvents | onHiddenCallback.hasRunners;

        protected VisibilityStateEvent onVisibilityChangedCallback => uiContainer.OnVisibilityChangedCallback;
        protected bool hasOnVisibilityChangedCallback => onVisibilityChangedCallback != null && onVisibilityChangedCallback.GetPersistentEventCount() > 0;

        protected bool hasCallbacks => hasOnShowCallback | hasOnVisibleCallback | hasOnHideCallback | hasOnHiddenCallback | hasOnVisibilityChangedCallback;

        protected VisualElement root { get; set; }
        protected FluidComponentHeader componentHeader { get; set; }

        protected EnabledIndicator callbacksTabIndicator { get; set; }

        protected FluidAnimatedContainer callbacksAnimatedContainer { get; set; }
        protected FluidAnimatedContainer settingsAnimatedContainer { get; set; }

        protected FluidButton getCustomPositionButton { get; set; }
        protected FluidButton resetCustomPositionButton { get; set; }
        protected FluidButton setCustomPositionButton { get; set; }

        protected FluidField autoHideAfterShowField { get; set; }
        protected FluidField customStartPositionField { get; set; }
        protected FluidField onStartBehaviourField { get; set; }
        protected FluidField whenHiddenField { get; set; }
        protected FluidField clearSelectedField { get; set; }
        protected FluidField autoSelectAfterShowField { get; set; }

        protected FluidToggleButtonTab callbacksTabButton { get; set; }
        protected FluidToggleButtonTab settingsTabButton { get; set; }

        protected FluidToggleGroup toggleGroup { get; set; }

        protected FluidToggleSwitch autoHideAfterShowSwitch { get; set; }
        protected FluidToggleSwitch disableCanvasWhenHiddenSwitch { get; set; }
        protected FluidToggleSwitch disableGameObjectWhenHiddenSwitch { get; set; }
        protected FluidToggleSwitch disableGraphicRaycasterWhenHiddenSwitch { get; set; }
        protected FluidToggleSwitch useCustomStartPositionSwitch { get; set; }
        protected FluidToggleSwitch clearSelectedOnHideSwitch { get; set; }
        protected FluidToggleSwitch clearSelectedOnShowSwitch { get; set; }
        protected FluidToggleSwitch autoSelectAfterShowSwitch { get; set; }

        protected ObjectField autoSelectTargetObjectField { get; set; }

        protected VisualElement callbacksTab { get; set; }

        protected SerializedProperty propertyOnStartBehaviour { get; set; }
        protected SerializedProperty propertyOnShowCallback { get; set; }
        protected SerializedProperty propertyOnVisibleCallback { get; set; }
        protected SerializedProperty propertyOnHideCallback { get; set; }
        protected SerializedProperty propertyOnHiddenCallback { get; set; }
        protected SerializedProperty propertyOnVisibilityChangedCallback { get; set; }
        protected SerializedProperty propertyCustomStartPosition { get; set; }
        protected SerializedProperty propertyUseCustomStartPosition { get; set; }
        protected SerializedProperty propertyAutoHideAfterShow { get; set; }
        protected SerializedProperty propertyAutoHideAfterShowDelay { get; set; }
        protected SerializedProperty propertyDisableGameObjectWhenHidden { get; set; }
        protected SerializedProperty propertyDisableCanvasWhenHidden { get; set; }
        protected SerializedProperty propertyDisableGraphicRaycasterWhenHidden { get; set; }
        protected SerializedProperty propertyClearSelectedOnHide { get; set; }
        protected SerializedProperty propertyClearSelectedOnShow { get; set; }
        protected SerializedProperty propertyAutoSelectAfterShow { get; set; }
        protected SerializedProperty propertyAutoSelectTarget { get; set; }

        protected virtual void OnDestroy()
        {
            componentHeader?.Recycle();

            callbacksTabIndicator?.Recycle();

            settingsAnimatedContainer?.Dispose();
            callbacksAnimatedContainer?.Dispose();

            getCustomPositionButton?.Recycle();
            resetCustomPositionButton?.Recycle();
            setCustomPositionButton?.Recycle();

            toggleGroup?.Recycle();

            autoHideAfterShowField?.Recycle();
            customStartPositionField?.Recycle();
            onStartBehaviourField?.Recycle();
            whenHiddenField?.Recycle();
            clearSelectedField?.Recycle();
            autoSelectAfterShowField?.Recycle();

            callbacksTabButton?.Recycle();
            settingsTabButton?.Recycle();

            autoHideAfterShowSwitch?.Recycle();
            disableCanvasWhenHiddenSwitch?.Recycle();
            disableGameObjectWhenHiddenSwitch?.Recycle();
            disableGraphicRaycasterWhenHiddenSwitch?.Recycle();
            useCustomStartPositionSwitch?.Recycle();
            clearSelectedOnHideSwitch?.Recycle();
            clearSelectedOnShowSwitch?.Recycle();
            autoSelectAfterShowSwitch?.Recycle();
        }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        protected virtual void FindProperties()
        {
            propertyOnStartBehaviour = serializedObject.FindProperty(nameof(UIContainer.OnStartBehaviour));
            propertyOnShowCallback = serializedObject.FindProperty(nameof(UIContainer.OnShowCallback));
            propertyOnVisibleCallback = serializedObject.FindProperty(nameof(UIContainer.OnVisibleCallback));
            propertyOnHideCallback = serializedObject.FindProperty(nameof(UIContainer.OnHideCallback));
            propertyOnHiddenCallback = serializedObject.FindProperty(nameof(UIContainer.OnHiddenCallback));
            propertyOnVisibilityChangedCallback = serializedObject.FindProperty(nameof(UIContainer.OnVisibilityChangedCallback));
            propertyCustomStartPosition = serializedObject.FindProperty(nameof(UIContainer.CustomStartPosition));
            propertyUseCustomStartPosition = serializedObject.FindProperty(nameof(UIContainer.UseCustomStartPosition));
            propertyAutoHideAfterShow = serializedObject.FindProperty(nameof(UIContainer.AutoHideAfterShow));
            propertyAutoHideAfterShowDelay = serializedObject.FindProperty(nameof(UIContainer.AutoHideAfterShowDelay));
            propertyDisableGameObjectWhenHidden = serializedObject.FindProperty(nameof(UIContainer.DisableGameObjectWhenHidden));
            propertyDisableCanvasWhenHidden = serializedObject.FindProperty(nameof(UIContainer.DisableCanvasWhenHidden));
            propertyDisableGraphicRaycasterWhenHidden = serializedObject.FindProperty(nameof(UIContainer.DisableGraphicRaycasterWhenHidden));
            propertyClearSelectedOnHide = serializedObject.FindProperty(nameof(UIContainer.ClearSelectedOnHide));
            propertyClearSelectedOnShow = serializedObject.FindProperty(nameof(UIContainer.ClearSelectedOnShow));
            propertyAutoSelectAfterShow = serializedObject.FindProperty(nameof(UIContainer.AutoSelectAfterShow));
            propertyAutoSelectTarget = serializedObject.FindProperty(nameof(UIContainer.AutoSelectTarget));
        }

        protected virtual void InitializeEditor()
        {
            FindProperties();

            root = new VisualElement();

            componentHeader =
                FluidComponentHeader.Get()
                    .SetAccentColor(accentColor)
                    .SetElementSize(ElementSize.Large);

            callbacksAnimatedContainer =
                new FluidAnimatedContainer()
                    .SetName("Callbacks")
                    .SetClearOnHide(true)
                    .Hide(false);

            settingsAnimatedContainer =
                new FluidAnimatedContainer()
                    .SetName("Settings")
                    .SetClearOnHide(false)
                    .Show(false);

            toggleGroup =
                FluidToggleGroup.Get()
                    .SetControlMode(FluidToggleGroup.ControlMode.OneToggleOnEnforced);

            settingsTabButton =
                DesignUtils.GetTabButtonForComponentSection(settingsIconTextures, selectableAccentColor);

            settingsTabButton
                .SetLabelText("Settings")
                .SetOnValueChanged(evt => settingsAnimatedContainer.Toggle(evt.newValue))
                .SetIsOn(true, false)
                .AddToToggleGroup(toggleGroup);


            InitializeCallbacks();
            InitializeBehaviours();
            InitializeAutoHideAfterShow();
            InitializeCustomStartPosition();
            InitializeWhenHidden();
            InitializeSelected();
        }

        protected virtual VisualElement GetShowHideControls()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                return new VisualElement().SetStyleDisplay(DisplayStyle.None);

            FluidField field = FluidField.Get();
            VisualElement row = DesignUtils.row;
            field.AddFieldContent(row);

            var instantAnimationSwitch = FluidToggleSwitch.Get("Instant");

            FluidButton GetButton() =>
                FluidButton.Get()
                    .SetElementSize(ElementSize.Small)
                    .SetButtonStyle(ButtonStyle.Contained);

            FluidButton showButton = GetButton()
                .SetLabelText("Show")
                .SetIcon(showIconTextures)
                .SetOnClick(() =>
                {
                    bool instantAnimation = instantAnimationSwitch.isOn;

                    if (serializedObject.isEditingMultipleObjects)
                    {
                        foreach (UIContainer container in uiContainers)
                        {
                            if (instantAnimation)
                            {
                                container.InstantShow();
                                continue;
                            }
                            container.Show();
                        }
                        return;
                    }

                    if (instantAnimation)
                    {
                        uiContainer.InstantShow();
                        return;
                    }

                    uiContainer.Show();

                });

            FluidButton hideButton = GetButton()
                .SetLabelText("Hide")
                .SetIcon(hideIconTextures)
                .SetOnClick(() =>
                {
                    bool instantAnimation = instantAnimationSwitch.isOn;

                    if (serializedObject.isEditingMultipleObjects)
                    {
                        foreach (UIContainer container in uiContainers)
                        {
                            if (instantAnimation)
                            {
                                container.InstantHide();
                                continue;
                            }
                            container.Hide();
                        }
                        return;
                    }

                    if (instantAnimation)
                    {
                        uiContainer.InstantHide();
                        return;
                    }
                    uiContainer.Hide();

                });

            row
                .AddChild(showButton)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(hideButton)
                .AddChild(DesignUtils.flexibleSpace)
                .AddChild(instantAnimationSwitch);

            return field
                .SetStyleMarginBottom(DesignUtils.k_Spacing);
        }

        protected virtual void InitializeCallbacks()
        {
            (callbacksTabButton, callbacksTabIndicator, callbacksTab) =
                DesignUtils.GetTabButtonForComponentSectionWithEnabledIndicator(unityIconTextures, DesignUtils.callbackSelectableColor, DesignUtils.callbacksColor);

            callbacksAnimatedContainer.SetOnShowCallback(() =>
            {
                callbacksAnimatedContainer.fluidContainer
                    .AddChild
                    (
                        FluidField.Get()
                            .SetElementSize(ElementSize.Large)
                            .SetIcon(EditorMicroAnimations.EditorUI.Icons.Show)
                            .SetLabelText("Show animation started")
                            .AddFieldContent(DesignUtils.NewPropertyField(propertyOnShowCallback))
                    )
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild
                    (
                        FluidField.Get()
                            .SetElementSize(ElementSize.Large)
                            .SetIcon(EditorMicroAnimations.EditorUI.Icons.Show)
                            .SetLabelText("Visible - Show animation finished")
                            .AddFieldContent(DesignUtils.NewPropertyField(propertyOnVisibleCallback))
                    )
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild
                    (
                        FluidField.Get()
                            .SetElementSize(ElementSize.Large)
                            .SetIcon(EditorMicroAnimations.EditorUI.Icons.Hide)
                            .SetLabelText("Hide animation started")
                            .AddFieldContent(DesignUtils.NewPropertyField(propertyOnHideCallback))
                    )
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild
                    (
                        FluidField.Get()
                            .SetElementSize(ElementSize.Large)
                            .SetIcon(EditorMicroAnimations.EditorUI.Icons.Hide)
                            .SetLabelText("Hidden - Hide animation finished")
                            .AddFieldContent(DesignUtils.NewPropertyField(propertyOnHiddenCallback))
                    )
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild
                    (
                        FluidField.Get()
                            .SetElementSize(ElementSize.Large)
                            .SetIcon(EditorMicroAnimations.EditorUI.Icons.VisibilityChanged)
                            .SetLabelText("Visibility changed")
                            .AddFieldContent(DesignUtils.NewPropertyField(propertyOnVisibilityChangedCallback))
                    )
                    .AddChild(DesignUtils.endOfLineBlock);

                callbacksAnimatedContainer.Bind(serializedObject);
            });

            callbacksTabIndicator.Toggle(hasCallbacks, false);

            bool previousHasCallbacks = !hasCallbacks;
            IVisualElementScheduledItem callbacksScheduler =
                callbacksTabButton.schedule.Execute(() =>
                {
                    if (previousHasCallbacks == hasCallbacks) return;
                    callbacksTabIndicator.Toggle(hasCallbacks, true);
                    previousHasCallbacks = hasCallbacks;
                }).Every(250);

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
                })
                .AddToToggleGroup(toggleGroup);
        }

        protected virtual void InitializeBehaviours()
        {
            EnumField onStartBehaviourEnumField = DesignUtils.NewEnumField(propertyOnStartBehaviour.propertyPath).SetStyleFlexGrow(1);
            onStartBehaviourEnumField.SetTooltip(GetContainerBehaviourTooltip((ContainerBehaviour)propertyOnStartBehaviour.enumValueIndex));
            onStartBehaviourEnumField.RegisterValueChangedCallback(evt =>
            {
                if (evt?.newValue == null) return;
                onStartBehaviourEnumField.SetTooltip(GetContainerBehaviourTooltip((ContainerBehaviour)evt.newValue));
            });
            onStartBehaviourField = FluidField.Get("OnStart Behaviour").AddFieldContent(onStartBehaviourEnumField);
            onStartBehaviourField.fieldContent.SetStyleFlexGrow(0);

            string GetContainerBehaviourTooltip(ContainerBehaviour behaviour)
            {
                switch (behaviour)
                {
                    case ContainerBehaviour.Disabled: return "Do nothing";
                    case ContainerBehaviour.InstantHide: return "Instant Hide (no animation)";
                    case ContainerBehaviour.InstantShow: return "Instant Show (no animation)";
                    case ContainerBehaviour.Hide: return "Hide (animated)";
                    case ContainerBehaviour.Show: return "Show (animated)";
                    default: throw new ArgumentOutOfRangeException(nameof(behaviour), behaviour, null);
                }
            }
        }

        protected virtual void InitializeAutoHideAfterShow()
        {
            Label hideDelayLabel =
                DesignUtils.NewLabel("Auto Hide Delay")
                    .SetStyleMarginRight(DesignUtils.k_Spacing);

            FloatField hideDelayPropertyField =
                DesignUtils.NewFloatField(propertyAutoHideAfterShowDelay)
                    .SetTooltip("Time interval after which Hide is triggered")
                    .SetStyleWidth(40)
                    .SetStyleMarginRight(DesignUtils.k_Spacing);

            Label secondsLabel = DesignUtils.NewLabel("seconds");

            hideDelayLabel.SetEnabled(propertyAutoHideAfterShow.boolValue);
            hideDelayPropertyField.SetEnabled(propertyAutoHideAfterShow.boolValue);
            secondsLabel.SetEnabled(propertyAutoHideAfterShow.boolValue);

            autoHideAfterShowSwitch =
                FluidToggleSwitch.Get()
                    .BindToProperty(propertyAutoHideAfterShow)
                    .SetTooltip("If TRUE, after Show, Hide it will get automatically triggered after the AutoHideAfterShowDelay time interval has passed")
                    .SetOnValueChanged(evt =>
                    {
                        if (evt?.newValue == null) return;
                        hideDelayLabel.SetEnabled(evt.newValue);
                        hideDelayPropertyField.SetEnabled(evt.newValue);
                        secondsLabel.SetEnabled(evt.newValue);

                    })
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetStyleMarginRight(DesignUtils.k_Spacing);

            autoHideAfterShowField =
                FluidField.Get("Auto Hide after Show")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(autoHideAfterShowSwitch)
                            .AddChild(hideDelayLabel)
                            .AddChild(hideDelayPropertyField)
                            .AddChild(secondsLabel)
                            .AddChild(DesignUtils.flexibleSpace)
                    );
        }

        protected virtual void InitializeCustomStartPosition()
        {
            PropertyField customStartPositionPropertyField =
                DesignUtils.NewPropertyField(propertyCustomStartPosition)
                    .TryToHideLabel()
                    .SetTooltip("AnchoredPosition3D to snap to on Awake")
                    .SetStyleAlignSelf(Align.Center);

            useCustomStartPositionSwitch =
                FluidToggleSwitch.Get()
                    .SetToggleAccentColor(selectableAccentColor)
                    .BindToProperty(propertyUseCustomStartPosition)
                    .SetTooltip("If TRUE, this view will 'snap' to the custom start position on Awake");

            FluidButton GetButton(IEnumerable<Texture2D> iconTextures, string labelText, string tooltip) =>
                FluidButton.Get()
                    .SetIcon(iconTextures)
                    .SetLabelText(labelText)
                    .SetTooltip(tooltip)
                    .SetElementSize(ElementSize.Tiny)
                    .SetButtonStyle(ButtonStyle.Contained);

            getCustomPositionButton =
                GetButton(arrowDownIconTextures, "Get", "Set the current RectTransform anchoredPosition3D as the custom start position")
                    .SetOnClick(() =>
                    {
                        propertyCustomStartPosition.vector3Value = uiContainer.rectTransform.anchoredPosition3D;
                        serializedObject.ApplyModifiedProperties();
                    });

            setCustomPositionButton =
                GetButton(arrowUpIconTextures, "Set", "Snap the RectTransform current anchoredPosition3D to the set custom start position")
                    .SetOnClick(() =>
                    {
                        if (serializedObject.isEditingMultipleObjects)
                        {
                            // ReSharper disable once CoVariantArrayConversion
                            Undo.RecordObjects(uiContainers.Select(ct => ct.rectTransform).ToArray(), "Set Position");
                            foreach (UIContainer container in uiContainers)
                                container.rectTransform.anchoredPosition3D = container.CustomStartPosition;
                            return;
                        }

                        Undo.RecordObject(uiContainer.rectTransform, "Set Position");
                        uiContainer.rectTransform.anchoredPosition3D = uiContainer.CustomStartPosition;
                    });

            resetCustomPositionButton =
                GetButton(resetIconTextures, "Reset", "Reset the custom start position to (0,0,0)")
                    .SetOnClick(() =>
                    {
                        propertyCustomStartPosition.vector3Value = Vector3.zero;
                        serializedObject.ApplyModifiedProperties();
                    });

            customStartPositionField =
                FluidField.Get("Custom Start Position")
                    .AddFieldContent
                    (
                        DesignUtils.row.SetStyleAlignItems(Align.Center)
                            .AddChild(useCustomStartPositionSwitch)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(customStartPositionPropertyField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(getCustomPositionButton)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(setCustomPositionButton)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(resetCustomPositionButton)
                    );

            customStartPositionPropertyField.SetEnabled(propertyUseCustomStartPosition.boolValue);
            getCustomPositionButton.SetEnabled(propertyUseCustomStartPosition.boolValue);
            setCustomPositionButton.SetEnabled(propertyUseCustomStartPosition.boolValue);
            resetCustomPositionButton.SetEnabled(propertyUseCustomStartPosition.boolValue);

            useCustomStartPositionSwitch.SetOnValueChanged(callback: evt =>
            {
                customStartPositionPropertyField.SetEnabled(evt.newValue);
                getCustomPositionButton.SetEnabled(evt.newValue);
                setCustomPositionButton.SetEnabled(evt.newValue);
                resetCustomPositionButton.SetEnabled(evt.newValue);
            });
        }

        protected virtual void InitializeWhenHidden()
        {

            disableGameObjectWhenHiddenSwitch =
                FluidToggleSwitch.Get("GameObject")
                    .BindToProperty(propertyDisableGameObjectWhenHidden)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetTooltip("If TRUE, after Hide, the GameObject this component is attached to, will be disabled");

            disableCanvasWhenHiddenSwitch =
                FluidToggleSwitch.Get("Canvas")
                    .BindToProperty(propertyDisableCanvasWhenHidden)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetTooltip("If TRUE, after Hide, the Canvas component found on the same GameObject this component is attached to, will be disabled");

            disableGraphicRaycasterWhenHiddenSwitch =
                FluidToggleSwitch.Get("GraphicRaycaster")
                    .BindToProperty(propertyDisableGraphicRaycasterWhenHidden)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetTooltip("If TRUE, after Hide, the GraphicRaycaster component found on the same GameObject this component is attached to, will be disabled");

            whenHiddenField = FluidField.Get("When Hidden, disable")
                .AddFieldContent
                (
                    DesignUtils.row
                        .AddChild(disableGameObjectWhenHiddenSwitch)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(disableCanvasWhenHiddenSwitch)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(disableGraphicRaycasterWhenHiddenSwitch)
                        .AddChild(DesignUtils.flexibleSpace)
                );
        }

        protected virtual void InitializeSelected()
        {
            clearSelectedOnShowSwitch =
                FluidToggleSwitch.Get()
                    .SetLabelText("Show")
                    .SetTooltip("If TRUE, when this container is shown, any GameObject that is selected by the EventSystem.current will get deselected")
                    .BindToProperty(propertyClearSelectedOnShow)
                    .SetToggleAccentColor(selectableAccentColor);

            clearSelectedOnHideSwitch =
                FluidToggleSwitch.Get()
                    .SetLabelText("Hide")
                    .SetTooltip("If TRUE, when this container is hidden, any GameObject that is selected by the EventSystem.current will get deselected")
                    .BindToProperty(propertyClearSelectedOnHide)
                    .SetToggleAccentColor(selectableAccentColor);

            clearSelectedField =
                FluidField.Get()
                    .SetLabelText("Clear Selected on")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(clearSelectedOnShowSwitch)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(clearSelectedOnHideSwitch)
                    )
                    .SetStyleMinWidth(150);


            autoSelectAfterShowSwitch =
                FluidToggleSwitch.Get()
                    .SetTooltip("If TRUE, after this container has been shown, the referenced selectable GameObject will get automatically selected by EventSystem.current")
                    .BindToProperty(propertyAutoSelectAfterShow)
                    .SetToggleAccentColor(selectableAccentColor);

            autoSelectTargetObjectField =
                DesignUtils.NewObjectField(propertyAutoSelectTarget, typeof(GameObject))
                    .SetTooltip("Reference to the GameObject that should be selected after this view has been shown. Works only if AutoSelectAfterShow is TRUE");

            autoSelectTargetObjectField.SetEnabled(propertyAutoSelectAfterShow.boolValue);
            autoSelectAfterShowSwitch.SetOnValueChanged(evt =>
            {
                if (evt?.newValue == null) return;
                autoSelectTargetObjectField.SetEnabled(evt.newValue);
            });

            autoSelectAfterShowField =
                FluidField.Get()
                    .SetLabelText("Auto select GameObject after Show")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(autoSelectAfterShowSwitch)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(autoSelectTargetObjectField)
                    );
        }

        protected virtual void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleMargins(50, -4, DesignUtils.k_Spacing2X, DesignUtils.k_Spacing2X)
                        .AddChild(settingsTabButton)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(callbacksTab)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild
                        (
                            DesignUtils.SystemButton_SortComponents
                            (
                                uiContainer.gameObject,
                                nameof(RectTransform),
                                nameof(Canvas),
                                nameof(CanvasGroup),
                                nameof(GraphicRaycaster),
                                nameof(UIContainer)
                            )
                        )
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(GetShowHideControls())
                .AddChild(callbacksAnimatedContainer)
                .AddChild
                (
                    settingsAnimatedContainer
                        .AddContent
                        (
                            DesignUtils.column
                                .AddChild
                                (
                                    DesignUtils.row
                                        .AddChild(onStartBehaviourField)
                                        .AddChild(DesignUtils.spaceBlock)
                                        .AddChild(autoHideAfterShowField)
                                )
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(customStartPositionField)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(whenHiddenField)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild
                                (
                                    DesignUtils.row
                                        .AddChild(clearSelectedField)
                                        .AddChild(DesignUtils.spaceBlock)
                                        .AddChild(autoSelectAfterShowField)
                                )
                                .AddChild(DesignUtils.spaceBlock)
                        )
                )
                .AddChild(DesignUtils.endOfLineBlock)
                ;
        }
    }
}
