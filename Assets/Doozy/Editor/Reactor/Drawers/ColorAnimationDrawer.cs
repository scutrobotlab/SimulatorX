// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Doozy.Editor.Common.Extensions;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Events;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Pooler;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Doozy.Editor.Reactor.Drawers
{
    [CustomPropertyDrawer(typeof(ColorAnimation), true)]
    public class ColorAnimationDrawer : PropertyDrawer
    {
        private static Color accentColor => EditorColors.Reactor.Red;
        private static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Reactor.Red;

        private static IEnumerable<Texture2D> colorAnimationIconTextures => EditorMicroAnimations.Reactor.Icons.ColorAnimation;
        private static IEnumerable<Texture2D> unityEventIconTextures => EditorMicroAnimations.EditorUI.Icons.UnityEvent;
        private static IEnumerable<Texture2D> resetIconTextures => EditorMicroAnimations.EditorUI.Icons.Reset;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var drawer = new VisualElement();
            if (property == null) return drawer;
            var target = property.GetTargetObjectOfProperty() as ColorAnimation;

            var poolables = new List<IPoolable>();
            var disposables = new List<IDisposable>();

            void Dispose()
            {
                foreach (IPoolable poolable in poolables)
                    poolable?.Recycle();

                foreach (IDisposable disposable in disposables)
                    disposable?.Dispose();

                drawer.Clear();
            }

            drawer.RegisterCallback<DetachFromPanelEvent>(evt => Dispose());

            #region SerializedProperties

            //Animation
            SerializedProperty propertyAnimation = property.FindPropertyRelative("Animation");
            SerializedProperty propertyAnimationEnabled = propertyAnimation.FindPropertyRelative("Enabled");
            //CALLBACKS            
            SerializedProperty propertyOnPlayCallback = property.FindPropertyRelative("OnPlayCallback");
            SerializedProperty propertyOnStopCallback = property.FindPropertyRelative("OnStopCallback");
            SerializedProperty propertyOnFinishCallback = property.FindPropertyRelative("OnFinishCallback");

            #endregion

            #region ComponentHeader

            FluidComponentHeader componentHeader =
                FluidComponentHeader.Get()
                    .SetAccentColor(accentColor)
                    .SetElementSize(ElementSize.Tiny)
                    .SetSecondaryIcon(colorAnimationIconTextures.ToList())
                    .SetComponentNameText("Color Animation")
                    .AddManualButton("www.bit.ly/DoozyKnowledgeBase4")
                    .AddYouTubeButton();

            poolables.Add(componentHeader);

            #endregion

            #region Tabs

            VisualElement tabsContainer = DesignUtils.row.SetStyleJustifyContent(Justify.Center).SetStyleMargins(12, 0, 12, 0);
            FluidToggleButtonTab animationTabButton, callbacksTabButton;
            EnabledIndicator animationTabIndicator, callbacksTabIndicator;
            VisualElement animationTabContainer, callbacksTabContainer;

            (animationTabButton, animationTabIndicator, animationTabContainer) = DesignUtils.GetTabButtonForComponentSectionWithEnabledIndicator(colorAnimationIconTextures, selectableAccentColor, accentColor);
            (callbacksTabButton, callbacksTabIndicator, callbacksTabContainer) = DesignUtils.GetTabButtonForComponentSectionWithEnabledIndicator(unityEventIconTextures, DesignUtils.callbackSelectableColor, DesignUtils.callbacksColor);

            animationTabIndicator.Toggle(propertyAnimationEnabled.boolValue, false);

            poolables.Add(animationTabIndicator);
            poolables.Add(callbacksTabIndicator);

            animationTabButton.SetLabelText("Animation");
            callbacksTabButton.SetLabelText("Callbacks");

            poolables.Add(animationTabButton);
            poolables.Add(callbacksTabButton);

            FluidToggleGroup showToggleGroup =
                FluidToggleGroup.Get()
                    .SetControlMode(FluidToggleGroup.ControlMode.OneToggleOn, animateChange: false);

            poolables.Add(showToggleGroup);

            animationTabButton.AddToToggleGroup(showToggleGroup);
            callbacksTabButton.AddToToggleGroup(showToggleGroup);

            FluidAnimatedContainer animationAnimatedContainer = new FluidAnimatedContainer().SetName("Animation");

            disposables.Add(animationAnimatedContainer);

            animationAnimatedContainer.OnShowCallback = () => animationAnimatedContainer.AddContent(GetAnimationContent(propertyAnimation, propertyAnimationEnabled));

            tabsContainer
                .AddChild(animationTabContainer)
                .AddSpace(DesignUtils.k_Spacing * 4, 0)
                .AddChild(callbacksTabContainer)
                .AddSpace(DesignUtils.k_Spacing * 4, 0)
                .AddChild(DesignUtils.flexibleSpace);

            #endregion

            #region Fields

            bool HasOnPlayCallback() => target?.OnPlayCallback?.GetPersistentEventCount() > 0;

            bool HasOnStopCallback() => target?.OnStopCallback?.GetPersistentEventCount() > 0;

            bool HasOnFinishCallback() => target?.OnFinishCallback?.GetPersistentEventCount() > 0;

            bool HasCallbacks() => HasOnPlayCallback() | HasOnStopCallback() | HasOnFinishCallback();

            callbacksTabIndicator.Toggle(HasCallbacks(), false);

            //OnPlayCallback
            var onPlayCallbackFoldout = new FluidFoldout("OnPlay");
            disposables.Add(onPlayCallbackFoldout);

            onPlayCallbackFoldout.animatedContainer.OnShowCallback = () =>
            {
                onPlayCallbackFoldout.ResetContentLeftPadding();
                onPlayCallbackFoldout.AddContent(DesignUtils.NewPropertyField(propertyOnPlayCallback.propertyPath));
                onPlayCallbackFoldout.Bind(property.serializedObject);
            };

            //OnStopCallback
            var onStopCallbackFoldout = new FluidFoldout("OnStop");
            disposables.Add(onStopCallbackFoldout);

            onStopCallbackFoldout.animatedContainer.OnShowCallback = () =>
            {
                onStopCallbackFoldout.ResetContentLeftPadding();
                onStopCallbackFoldout.AddContent(DesignUtils.NewPropertyField(propertyOnStopCallback.propertyPath));
                onStopCallbackFoldout.Bind(property.serializedObject);
            };


            //OnFinishCallback
            var onFinishCallbackFoldout = new FluidFoldout("OnFinish");
            disposables.Add(onFinishCallbackFoldout);

            onFinishCallbackFoldout.animatedContainer.OnShowCallback = () =>
            {
                onFinishCallbackFoldout.ResetContentLeftPadding();
                onFinishCallbackFoldout.AddContent(DesignUtils.NewPropertyField(propertyOnFinishCallback.propertyPath));
                onFinishCallbackFoldout.Bind(property.serializedObject);
            };


            //Callbacks container
            FluidAnimatedContainer callbacksAnimatedContainer = new FluidAnimatedContainer().SetName("Callbacks");
            disposables.Add(callbacksAnimatedContainer);

            callbacksAnimatedContainer.OnShowCallback = () =>
            {
                callbacksAnimatedContainer.AddContent
                (
                    new VisualElement()
                        .AddChild(onPlayCallbackFoldout)
                        .AddSpace(0, DesignUtils.k_Spacing)
                        .AddChild(onStopCallbackFoldout)
                        .AddSpace(0, DesignUtils.k_Spacing)
                        .AddChild(onFinishCallbackFoldout)
                );
            };

            callbacksAnimatedContainer.OnHideCallback =
                () =>
                {
                    if (onPlayCallbackFoldout.isOn) onPlayCallbackFoldout.SetIsOn(false, animateChange: true);
                    if (onStopCallbackFoldout.isOn) onStopCallbackFoldout.SetIsOn(false, animateChange: true);
                    if (onFinishCallbackFoldout.isOn) onFinishCallbackFoldout.SetIsOn(false, animateChange: true);
                };

            #endregion

            void TabSetOnValueChanged(FluidBoolEvent evt, FluidAnimatedContainer animatedContainer, FluidToggleButtonTab tab) =>
                animatedContainer.Toggle(evt.newValue);

            animationTabButton.SetOnValueChanged(evt => TabSetOnValueChanged(evt, animationAnimatedContainer, animationTabButton));

            bool previousHasCallbacks = !HasCallbacks();
            IVisualElementScheduledItem callbacksScheduler =
                callbacksTabButton.schedule.Execute(() =>
                {
                    bool hasCallbacks = HasCallbacks();
                    if (previousHasCallbacks == hasCallbacks) return;
                    UpdateIndicators(true);
                    previousHasCallbacks = hasCallbacks;
                }).Every(250);
            callbacksScheduler.Pause();

            callbacksTabButton.OnValueChanged = evt =>
            {
                TabSetOnValueChanged(evt, callbacksAnimatedContainer, callbacksTabButton);
                if (evt.newValue)
                    callbacksScheduler?.Resume();
                else
                    callbacksScheduler?.Pause();
            };

            drawer
                .AddChild(componentHeader)
                .AddChild(tabsContainer)
                .AddSpace(0, DesignUtils.k_Spacing * 2)
                .AddChild(animationAnimatedContainer)
                .AddChild(callbacksAnimatedContainer);

            #region Dynamic Setup

            #region Invisible Fields

            Toggle invisibleAnimationEnabledToggle = DesignUtils.NewToggle(propertyAnimationEnabled.propertyPath, invisibleField: true);

            drawer
                .AddChild(invisibleAnimationEnabledToggle);

            invisibleAnimationEnabledToggle.RegisterValueChangedCallback(evt => UpdateIndicators(true));

            UpdateIndicators(false);

            void UpdateIndicator(EnabledIndicator indicator, bool enabled, bool animateChange = true)
            {
                indicator.Toggle(enabled, animateChange);
            }

            void UpdateIndicators(bool animateChange)
            {
                drawer.schedule.Execute(() =>
                {
                    UpdateIndicator(animationTabIndicator, propertyAnimationEnabled.boolValue, animateChange);
                    UpdateIndicator(callbacksTabIndicator, HasCallbacks(), animateChange);
                });
            }

            #endregion

            #endregion

            return drawer;
        }

        private static VisualElement GetAnimationContent
        (
            SerializedProperty propertyAnimation,
            SerializedProperty propertyAnimationEnabled
        )
        {
            SerializedProperty propertyFromReferenceValue = propertyAnimation.FindPropertyRelative("FromReferenceValue");
            SerializedProperty propertyFromCustomValue = propertyAnimation.FindPropertyRelative("FromCustomValue");
            SerializedProperty propertyFromHueOffset = propertyAnimation.FindPropertyRelative("FromHueOffset");
            SerializedProperty propertyFromSaturationOffset = propertyAnimation.FindPropertyRelative("FromSaturationOffset");
            SerializedProperty propertyFromLightnessOffset = propertyAnimation.FindPropertyRelative("FromLightnessOffset");
            SerializedProperty propertyFromAlphaOffset = propertyAnimation.FindPropertyRelative("FromAlphaOffset");

            SerializedProperty propertyToReferenceValue = propertyAnimation.FindPropertyRelative("ToReferenceValue");
            SerializedProperty propertyToCustomValue = propertyAnimation.FindPropertyRelative("ToCustomValue");
            SerializedProperty propertyToHueOffset = propertyAnimation.FindPropertyRelative("ToHueOffset");
            SerializedProperty propertyToSaturationOffset = propertyAnimation.FindPropertyRelative("ToSaturationOffset");
            SerializedProperty propertyToLightnessOffset = propertyAnimation.FindPropertyRelative("ToLightnessOffset");
            SerializedProperty propertyToAlphaOffset = propertyAnimation.FindPropertyRelative("ToAlphaOffset");

            SerializedProperty propertySettings = propertyAnimation.FindPropertyRelative("Settings");

            var content = new VisualElement();
            content.SetEnabled(propertyAnimationEnabled.boolValue);
            var enableSwitch = NewEnableAnimationSwitch(string.Empty, selectableAccentColor, propertyAnimationEnabled, content);

            
            var fieldFromReferenceValue = new EnumField().SetBindingPath(propertyFromReferenceValue.propertyPath).ResetLayout();

            #region From Hue Offset

            var fromHueOffsetLabel = GetOffsetLabel(() => (propertyFromHueOffset.floatValue * 360).Round(0).ToString(CultureInfo.InvariantCulture));
            var fromHueOffsetSlider = new Slider(-1f, 1f).SetBindingPath(propertyFromHueOffset.propertyPath).ResetLayout().SetStyleFlexGrow(1);
            fromHueOffsetSlider.RegisterValueChangedCallback(evt => fromHueOffsetLabel.SetText((evt.newValue * 360).Round(0).ToString(CultureInfo.InvariantCulture)));
            var fieldFromHueOffset =
                GetOffsetField
                (
                    "Hue Offset",
                    fromHueOffsetLabel,
                    fromHueOffsetSlider,
                    () =>
                    {
                        propertyFromHueOffset.floatValue = 0f;
                        propertyFromHueOffset.serializedObject.ApplyModifiedProperties();
                    });

            #endregion

            #region From Saturation Offset

            var fromSaturationOffsetLabel = GetOffsetLabel(() => $"{(propertyFromSaturationOffset.floatValue * 100).Round(0)}%");
            var fromSaturationOffsetSlider = new Slider(-1f, 1f).SetBindingPath(propertyFromSaturationOffset.propertyPath).ResetLayout().SetStyleFlexGrow(1);
            fromSaturationOffsetSlider.RegisterValueChangedCallback(evt => fromSaturationOffsetLabel.SetText($"{(evt.newValue * 100).Round(0)}%"));
            var fieldFromSaturationOffset =
                GetOffsetField
                (
                    "Saturation Offset",
                    fromSaturationOffsetLabel,
                    fromSaturationOffsetSlider,
                    () =>
                    {
                        propertyFromSaturationOffset.floatValue = 0f;
                        propertyFromSaturationOffset.serializedObject.ApplyModifiedProperties();
                    });

            #endregion

            #region From Lightness Offset

            var fromLightnessOffsetLabel = GetOffsetLabel(() => $"{(propertyFromLightnessOffset.floatValue * 100).Round(0)}%");
            var fromLightnessOffsetSlider = new Slider(-1f, 1f).SetBindingPath(propertyFromLightnessOffset.propertyPath).ResetLayout().SetStyleFlexGrow(1);
            fromLightnessOffsetSlider.RegisterValueChangedCallback(evt => fromLightnessOffsetLabel.SetText($"{(evt.newValue * 100).Round(0)}%"));
            var fieldFromLightnessOffset =
                GetOffsetField
                (
                    "Lightness Offset",
                    fromLightnessOffsetLabel,
                    fromLightnessOffsetSlider,
                    () =>
                    {
                        propertyFromLightnessOffset.floatValue = 0f;
                        propertyFromLightnessOffset.serializedObject.ApplyModifiedProperties();
                    });

            #endregion

            #region From Alpha Offset

            var fromAlphaOffsetLabel = GetOffsetLabel(() => $"{(propertyFromAlphaOffset.floatValue * 100).Round(0)}%");
            var fromAlphaOffsetSlider = new Slider(-1f, 1f).SetBindingPath(propertyFromAlphaOffset.propertyPath).ResetLayout().SetStyleFlexGrow(1);
            fromAlphaOffsetSlider.RegisterValueChangedCallback(evt => fromAlphaOffsetLabel.SetText($"{(evt.newValue * 100).Round(0)}%"));
            var fieldFromAlphaOffset =
                GetOffsetField
                (
                    "Alpha Offset",
                    fromAlphaOffsetLabel,
                    fromAlphaOffsetSlider,
                    () =>
                    {
                        propertyFromAlphaOffset.floatValue = 0f;
                        propertyFromAlphaOffset.serializedObject.ApplyModifiedProperties();
                    });

            #endregion

            var fromOffsetContainer =
                DesignUtils.column
                    .SetName("From Offset")
                    .AddChild(fieldFromHueOffset)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(fieldFromSaturationOffset)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(fieldFromLightnessOffset)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(fieldFromAlphaOffset);

            var fieldFromCustomValue = FluidField.Get("From Custom Color", new ColorField().SetBindingPath(propertyFromCustomValue.propertyPath).ResetLayout());
            
            var fieldFrom = FluidField.Get("Color From")
                .AddFieldContent(fieldFromReferenceValue);
            
            var fieldToReferenceValue = new EnumField().SetBindingPath(propertyToReferenceValue.propertyPath).ResetLayout();

            #region To Hue Offset

            var toHueOffsetLabel = GetOffsetLabel(() => (propertyToHueOffset.floatValue * 360).Round(0).ToString(CultureInfo.InvariantCulture));
            var toHueOffsetSlider = new Slider(-1f, 1f).SetBindingPath(propertyToHueOffset.propertyPath).ResetLayout().SetStyleFlexGrow(1);
            toHueOffsetSlider.RegisterValueChangedCallback(evt => toHueOffsetLabel.SetText((evt.newValue * 360).Round(0).ToString(CultureInfo.InvariantCulture)));
            var fieldToHueOffset =
                GetOffsetField
                (
                    "Hue Offset",
                    toHueOffsetLabel,
                    toHueOffsetSlider,
                    () =>
                    {
                        propertyToHueOffset.floatValue = 0f;
                        propertyToHueOffset.serializedObject.ApplyModifiedProperties();
                    });

            #endregion

            #region To Saturation Offset

            var toSaturationOffsetLabel = GetOffsetLabel(() => $"{(propertyToSaturationOffset.floatValue * 100).Round(0)}%");
            var toSaturationOffsetSlider = new Slider(-1f, 1f).SetBindingPath(propertyToSaturationOffset.propertyPath).ResetLayout().SetStyleFlexGrow(1);
            toSaturationOffsetSlider.RegisterValueChangedCallback(evt => toSaturationOffsetLabel.SetText($"{(evt.newValue * 100).Round(0)}%"));
            var fieldToSaturationOffset =
                GetOffsetField
                (
                    "Saturation Offset",
                    toSaturationOffsetLabel,
                    toSaturationOffsetSlider,
                    () =>
                    {
                        propertyToSaturationOffset.floatValue = 0f;
                        propertyToSaturationOffset.serializedObject.ApplyModifiedProperties();
                    });

            #endregion

            #region To Lightness Offset

            var toLightnessOffsetLabel = GetOffsetLabel(() => $"{(propertyToLightnessOffset.floatValue * 100).Round(0)}%");
            var toLightnessOffsetSlider = new Slider(-1f, 1f).SetBindingPath(propertyToLightnessOffset.propertyPath).ResetLayout().SetStyleFlexGrow(1);
            toLightnessOffsetSlider.RegisterValueChangedCallback(evt => toLightnessOffsetLabel.SetText($"{(evt.newValue * 100).Round(0)}%"));
            var fieldToLightnessOffset =
                GetOffsetField
                (
                    "Lightness Offset",
                    toLightnessOffsetLabel,
                    toLightnessOffsetSlider,
                    () =>
                    {
                        propertyToLightnessOffset.floatValue = 0f;
                        propertyToLightnessOffset.serializedObject.ApplyModifiedProperties();
                    });

            #endregion

            #region To Alpha Offset

            var toAlphaOffsetLabel = GetOffsetLabel(() => $"{(propertyToAlphaOffset.floatValue * 100).Round(0)}%");
            var toAlphaOffsetSlider = new Slider(-1f, 1f).SetBindingPath(propertyToAlphaOffset.propertyPath).ResetLayout().SetStyleFlexGrow(1);
            toAlphaOffsetSlider.RegisterValueChangedCallback(evt => toAlphaOffsetLabel.SetText($"{(evt.newValue * 100).Round(0)}%"));
            var fieldToAlphaOffset =
                GetOffsetField
                (
                    "Alpha Offset",
                    toAlphaOffsetLabel,
                    toAlphaOffsetSlider,
                    () =>
                    {
                        propertyToAlphaOffset.floatValue = 0f;
                        propertyToAlphaOffset.serializedObject.ApplyModifiedProperties();
                    });

            #endregion

            var toOffsetContainer =
                DesignUtils.column
                    .SetName("To Offset")
                    .AddChild(fieldToHueOffset)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(fieldToSaturationOffset)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(fieldToLightnessOffset)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(fieldToAlphaOffset);

            var fieldToCustomValue = FluidField.Get("To Custom Color", new ColorField().SetBindingPath(propertyToCustomValue.propertyPath).ResetLayout());
            var fieldTo = FluidField.Get("Color To").AddFieldContent(fieldToReferenceValue);

            VisualElement foldoutContent =
                new VisualElement()
                    .AddChild
                    (
                        DesignUtils.row
                            .AddChild(enableSwitch)
                            .AddChild(DesignUtils.flexibleSpace)
                    )
                    .AddChild
                    (
                        content
                            .AddChild
                            (
                                DesignUtils.row
                                    .AddChild
                                    (
                                        DesignUtils.flexibleSpace
                                            .AddChild(fieldFrom.SetStyleFlexGrow(1))
                                            .AddSpace(0, DesignUtils.k_Spacing)
                                            .AddChild(fromOffsetContainer.SetStyleFlexGrow(1))
                                            .AddChild(fieldFromCustomValue.SetStyleHeight(42).SetStyleFlexGrow(1).SetStyleMaxHeight(42))
                                    )
                                    .AddSpace(DesignUtils.k_Spacing, 0)
                                    .AddChild
                                    (
                                        DesignUtils.flexibleSpace
                                            .AddChild(fieldTo.SetStyleFlexGrow(1))
                                            .AddSpace(0, DesignUtils.k_Spacing)
                                            .AddChild(toOffsetContainer.SetStyleFlexGrow(1))
                                            .AddChild(fieldToCustomValue.SetStyleHeight(42).SetStyleFlexGrow(1).SetStyleMaxHeight(42))
                                    ))
                            .AddSpace(0, DesignUtils.k_Spacing)
                            .AddChild(new PropertyField().SetBindingPath(propertySettings.propertyPath))
                            .AddSpace(0, DesignUtils.k_EndOfLineSpacing)
                    );

            void Update()
            {
                fromOffsetContainer.SetStyleDisplay((ReferenceValue)propertyFromReferenceValue.enumValueIndex == ReferenceValue.CustomValue ? DisplayStyle.None : DisplayStyle.Flex);
                fieldFromCustomValue.SetStyleDisplay((ReferenceValue)propertyFromReferenceValue.enumValueIndex != ReferenceValue.CustomValue ? DisplayStyle.None : DisplayStyle.Flex);
                toOffsetContainer.SetStyleDisplay((ReferenceValue)propertyToReferenceValue.enumValueIndex == ReferenceValue.CustomValue ? DisplayStyle.None : DisplayStyle.Flex);
                fieldToCustomValue.SetStyleDisplay((ReferenceValue)propertyToReferenceValue.enumValueIndex != ReferenceValue.CustomValue ? DisplayStyle.None : DisplayStyle.Flex);
            }

            //FromReferenceValue
            var invisibleFieldFromReferenceValueEnum = new EnumField().SetBindingPath(propertyFromReferenceValue.propertyPath).SetStyleDisplay(DisplayStyle.None);
            foldoutContent.AddChild(invisibleFieldFromReferenceValueEnum);
            invisibleFieldFromReferenceValueEnum.RegisterValueChangedCallback(changeEvent => Update());

            //ToReferenceValue
            var invisibleFieldToReferenceValueEnum = new EnumField().SetBindingPath(propertyToReferenceValue.propertyPath).SetStyleDisplay(DisplayStyle.None);
            foldoutContent.AddChild(invisibleFieldToReferenceValueEnum);
            invisibleFieldToReferenceValueEnum.RegisterValueChangedCallback(changeEvent => Update());

            foldoutContent.Bind(propertyAnimation.serializedObject);

            Update();
            return foldoutContent;
        }

        private static Label GetOffsetLabel(Func<string> value) =>
            DesignUtils.fieldLabel
                .ResetLayout()
                .SetText(value.Invoke())
                .SetStyleAlignSelf(Align.Center)
                .SetStyleTextAlign(TextAnchor.MiddleRight)
                .SetStyleWidth(24);

        private static FluidField GetOffsetField(string labelText, VisualElement label, VisualElement slider, UnityAction onClickCallback) =>
            FluidField.Get()
                .SetLabelText(labelText)
                .AddFieldContent
                (
                    DesignUtils.row
                        .SetStyleJustifyContent(Justify.Center)
                        .AddChild(label)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(slider)
                        .AddChild
                        (
                            FluidButton.Get(resetIconTextures)
                                .SetElementSize(ElementSize.Tiny)
                                .SetTooltip("Reset")
                                .SetOnClick(onClickCallback)
                        )
                );

        private static FluidToggleSwitch NewEnableAnimationSwitch(string animationName, EditorSelectableColorInfo sColor, SerializedProperty propertyEnabled, VisualElement content)
        {
            FluidToggleSwitch fluidSwitch =
                FluidToggleSwitch.Get($"Enable {animationName}")
                    .SetToggleAccentColor(sColor)
                    .BindToProperty(propertyEnabled.propertyPath);

            fluidSwitch.SetOnValueChanged(evt => Update(evt.newValue));

            Update(propertyEnabled.boolValue);

            void Update(bool enabled)
            {
                fluidSwitch.SetLabelText($"{animationName}{(animationName.IsNullOrEmpty() ? "" : " ")}{(enabled ? "Enabled" : "Disabled")}");
                content.SetEnabled(enabled);
            }

            return fluidSwitch;
        }
    }
}
