// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.Common.Extensions;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Events;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Windows;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Pooler;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Animators;
using Doozy.Runtime.Reactor.ScriptableObjects;
using Doozy.Runtime.Reactor.ScriptableObjects.Internal;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.Reactor.Drawers
{
    [CustomPropertyDrawer(typeof(UIAnimation), true)]
    public class UIAnimationDrawer : PropertyDrawer
    {
        private static Color accentColor => EditorColors.Reactor.Red;
        private static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Reactor.Red;

        private static Color disabledColor => EditorColors.Default.Placeholder;
        private static Color moveColor => EditorColors.Reactor.Move;
        private static Color rotateColor => EditorColors.Reactor.Rotate;
        private static Color scaleColor => EditorColors.Reactor.Scale;
        private static Color fadeColor => EditorColors.Reactor.Fade;
        private static Color presetsColor => EditorColors.Reactor.Red;

        private static EditorSelectableColorInfo moveSColor => EditorSelectableColors.Reactor.Move;
        private static EditorSelectableColorInfo rotateSColor => EditorSelectableColors.Reactor.Rotate;
        private static EditorSelectableColorInfo scaleSColor => EditorSelectableColors.Reactor.Scale;
        private static EditorSelectableColorInfo fadeSColor => EditorSelectableColors.Reactor.Fade;

        private static IEnumerable<Texture2D> uiAnimationIconTextures => EditorMicroAnimations.Reactor.Icons.UIAnimation;
        private static IEnumerable<Texture2D> uiAnimationPresetIconTextures => EditorMicroAnimations.Reactor.Icons.UIAnimationPreset;
        private static IEnumerable<Texture2D> moveIconTextures => EditorMicroAnimations.Reactor.Icons.Move;
        private static IEnumerable<Texture2D> rotateIconTextures => EditorMicroAnimations.Reactor.Icons.Rotate;
        private static IEnumerable<Texture2D> scaleIconTextures => EditorMicroAnimations.Reactor.Icons.Scale;
        private static IEnumerable<Texture2D> fadeIconTextures => EditorMicroAnimations.Reactor.Icons.Fade;
        private static IEnumerable<Texture2D> unityEventIconTextures => EditorMicroAnimations.EditorUI.Icons.UnityEvent;
        private static IEnumerable<Texture2D> resetIconTextures => EditorMicroAnimations.EditorUI.Icons.Reset;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var drawer = new VisualElement();
            if (property == null) return drawer;
            var target = property.GetTargetObjectOfProperty() as UIAnimation;

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

            //ANIMATION TYPE
            SerializedProperty propertyAnimationType = property.FindPropertyRelative("AnimationType");
            //MOVE
            SerializedProperty propertyMove = property.FindPropertyRelative("Move");
            SerializedProperty propertyMoveEnabled = propertyMove.FindPropertyRelative("Enabled");
            SerializedProperty propertyMoveAnimationType = propertyMove.FindPropertyRelative("AnimationType");
            //ROTATE
            SerializedProperty propertyRotate = property.FindPropertyRelative("Rotate");
            SerializedProperty propertyRotateEnabled = propertyRotate.FindPropertyRelative("Enabled");
            //SCALE
            SerializedProperty propertyScale = property.FindPropertyRelative("Scale");
            SerializedProperty propertyScaleEnabled = propertyScale.FindPropertyRelative("Enabled");
            //FADE
            SerializedProperty propertyFade = property.FindPropertyRelative("Fade");
            SerializedProperty propertyFadeEnabled = propertyFade.FindPropertyRelative("Enabled");
            //CALLBACKS            
            SerializedProperty propertyOnPlayCallback = property.FindPropertyRelative("OnPlayCallback");
            SerializedProperty propertyOnStopCallback = property.FindPropertyRelative("OnStopCallback");
            SerializedProperty propertyOnFinishCallback = property.FindPropertyRelative("OnFinishCallback");

            #endregion

            #region ComponentHeader

            EnumField fieldAnimationTypeEnum = new EnumField().SetBindingPath(propertyAnimationType.propertyPath).ResetLayout().SetStyleAlignSelf(Align.Center).SetStyleWidth(80);

            FluidComponentHeader componentHeader =
                FluidComponentHeader.Get()
                    .SetSecondaryIcon(uiAnimationIconTextures.ToList())
                    .SetAccentColor(accentColor)
                    .SetElementSize(ElementSize.Tiny)
                    .AddElement(fieldAnimationTypeEnum.SetStyleMargins(DesignUtils.k_Spacing * 2, 0, DesignUtils.k_Spacing * 2, 0))
                    .AddManualButton("www.bit.ly/DoozyKnowledgeBase4")
                    .AddYouTubeButton();

            poolables.Add(componentHeader);

            #endregion

            #region Tabs

            VisualElement tabsContainer = DesignUtils.row.SetStyleJustifyContent(Justify.Center).SetStyleMargins(12, 0, 12, 0);
            FluidToggleButtonTab moveTabButton, rotateTabButton, scaleTabButton, fadeTabButton, callbacksTabButton, presetsTabButton;
            EnabledIndicator moveTabIndicator, rotateTabIndicator, scaleTabIndicator, fadeTabIndicator, callbacksTabIndicator, presetsTabIndicator;
            VisualElement moveTabContainer, rotateTabContainer, scaleTabContainer, fadeTabContainer, callbacksTabContainer, presetsTabContainer;

            (moveTabButton, moveTabIndicator, moveTabContainer) = DesignUtils.GetTabButtonForComponentSectionWithEnabledIndicator(moveIconTextures, moveSColor, moveColor);
            (rotateTabButton, rotateTabIndicator, rotateTabContainer) = DesignUtils.GetTabButtonForComponentSectionWithEnabledIndicator(rotateIconTextures, rotateSColor, rotateColor);
            (scaleTabButton, scaleTabIndicator, scaleTabContainer) = DesignUtils.GetTabButtonForComponentSectionWithEnabledIndicator(scaleIconTextures, scaleSColor, scaleColor);
            (fadeTabButton, fadeTabIndicator, fadeTabContainer) = DesignUtils.GetTabButtonForComponentSectionWithEnabledIndicator(fadeIconTextures, fadeSColor, fadeColor);
            (callbacksTabButton, callbacksTabIndicator, callbacksTabContainer) = DesignUtils.GetTabButtonForComponentSectionWithEnabledIndicator(unityEventIconTextures, DesignUtils.callbackSelectableColor, DesignUtils.callbacksColor);
            (presetsTabButton, presetsTabIndicator, presetsTabContainer) = DesignUtils.GetTabButtonForComponentSectionWithEnabledIndicator(uiAnimationPresetIconTextures, selectableAccentColor, presetsColor);

            moveTabIndicator.Toggle(propertyMoveEnabled.boolValue, false);
            rotateTabIndicator.Toggle(propertyRotateEnabled.boolValue, false);
            scaleTabIndicator.Toggle(propertyScaleEnabled.boolValue, false);
            fadeTabIndicator.Toggle(propertyFadeEnabled.boolValue, false);

            poolables.Add(moveTabIndicator);
            poolables.Add(rotateTabIndicator);
            poolables.Add(scaleTabIndicator);
            poolables.Add(fadeTabIndicator);
            poolables.Add(callbacksTabIndicator);
            poolables.Add(presetsTabIndicator);

            const float tabButtonWidth = 68;
            moveTabButton.SetLabelText("Move").SetStyleWidth(tabButtonWidth);
            rotateTabButton.SetLabelText("Rotate").SetStyleWidth(tabButtonWidth);
            scaleTabButton.SetLabelText("Scale").SetStyleWidth(tabButtonWidth);
            fadeTabButton.SetLabelText("Fade").SetStyleWidth(tabButtonWidth);
            callbacksTabButton.SetLabelText("Callbacks");
            presetsTabButton.SetLabelText("Presets");

            poolables.Add(moveTabButton);
            poolables.Add(rotateTabButton);
            poolables.Add(scaleTabButton);
            poolables.Add(fadeTabButton);
            poolables.Add(callbacksTabButton);
            poolables.Add(presetsTabButton);


            FluidToggleGroup showToggleGroup =
                FluidToggleGroup.Get()
                    .SetControlMode(FluidToggleGroup.ControlMode.OneToggleOn, animateChange: false);

            poolables.Add(showToggleGroup);

            moveTabButton.AddToToggleGroup(showToggleGroup);
            rotateTabButton.AddToToggleGroup(showToggleGroup);
            scaleTabButton.AddToToggleGroup(showToggleGroup);
            fadeTabButton.AddToToggleGroup(showToggleGroup);
            callbacksTabButton.AddToToggleGroup(showToggleGroup);
            presetsTabButton.AddToToggleGroup(showToggleGroup);

            FluidAnimatedContainer moveAnimatedContainer = new FluidAnimatedContainer().SetName("Move").SetClearOnHide(true);
            FluidAnimatedContainer rotateAnimatedContainer = new FluidAnimatedContainer().SetName("Rotate").SetClearOnHide(true);
            FluidAnimatedContainer scaleAnimatedContainer = new FluidAnimatedContainer().SetName("Scale").SetClearOnHide(true);
            FluidAnimatedContainer fadeAnimatedContainer = new FluidAnimatedContainer().SetName("Fade").SetClearOnHide(true);

            disposables.Add(moveAnimatedContainer);
            disposables.Add(rotateAnimatedContainer);
            disposables.Add(scaleAnimatedContainer);
            disposables.Add(fadeAnimatedContainer);

            moveAnimatedContainer.OnShowCallback = () => moveAnimatedContainer.AddContent(GetMoveContent(propertyMove, propertyMoveEnabled, propertyAnimationType));
            rotateAnimatedContainer.OnShowCallback = () => rotateAnimatedContainer.AddContent(GetRotateContent(propertyRotate, propertyRotateEnabled));
            scaleAnimatedContainer.OnShowCallback = () => scaleAnimatedContainer.AddContent(GetScaleContent(propertyScale, propertyScaleEnabled));
            fadeAnimatedContainer.OnShowCallback = () => fadeAnimatedContainer.AddContent(GetFadeContent(propertyFade, propertyFadeEnabled));

            tabsContainer
                .AddChild(moveTabContainer)
                .AddSpace(DesignUtils.k_Spacing, 0)
                .AddChild(rotateTabContainer)
                .AddSpace(DesignUtils.k_Spacing, 0)
                .AddChild(scaleTabContainer)
                .AddSpace(DesignUtils.k_Spacing, 0)
                .AddChild(fadeTabContainer)
                .AddSpace(DesignUtils.k_Spacing * 4, 0)
                .AddChild(callbacksTabContainer)
                .AddSpace(DesignUtils.k_Spacing * 4, 0)
                .AddChild(DesignUtils.flexibleSpace)
                .AddChild(presetsTabContainer);

            #endregion

            #region Fields

            //PRESETS
            FluidAnimatedContainer presetsAnimatedContainer = new FluidAnimatedContainer().SetName("Presets").SetClearOnHide(true);
            disposables.Add(presetsAnimatedContainer);

            presetsAnimatedContainer.OnShowCallback =
                () => presetsAnimatedContainer.AddContent(GetPresetsContent(target, property, propertyMoveEnabled, propertyRotateEnabled, propertyScaleEnabled, propertyFadeEnabled, propertyAnimationType));

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

            moveTabButton.SetOnValueChanged(evt => TabSetOnValueChanged(evt, moveAnimatedContainer, moveTabButton));
            rotateTabButton.SetOnValueChanged(evt => TabSetOnValueChanged(evt, rotateAnimatedContainer, rotateTabButton));
            scaleTabButton.SetOnValueChanged(evt => TabSetOnValueChanged(evt, scaleAnimatedContainer, scaleTabButton));
            fadeTabButton.SetOnValueChanged(evt => TabSetOnValueChanged(evt, fadeAnimatedContainer, fadeTabButton));

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
            presetsTabButton.OnValueChanged = evt => TabSetOnValueChanged(evt, presetsAnimatedContainer, presetsTabButton);

            drawer
                .AddChild(componentHeader)
                .AddChild(tabsContainer)
                .AddSpace(0, DesignUtils.k_Spacing * 2)
                .AddChild(presetsAnimatedContainer)
                .AddChild(moveAnimatedContainer)
                .AddChild(rotateAnimatedContainer)
                .AddChild(scaleAnimatedContainer)
                .AddChild(fadeAnimatedContainer)
                .AddChild(callbacksAnimatedContainer);

            SyncAnimationType();

            UIAnimationType CurrentAnimationType() => (UIAnimationType)propertyAnimationType.enumValueIndex;

            UpdateComponentHeaderComponentName(CurrentAnimationType());
            
            void UpdateComponentHeaderComponentName(UIAnimationType animationType) =>
                componentHeader.SetComponentNameText($"{animationType} Animation");

            #region Dynamic Setup

            drawer.RegisterCallback<AttachToPanelEvent>(evt => Undo.undoRedoPerformed += SyncAnimationType);
            drawer.RegisterCallback<DetachFromPanelEvent>(evt => Undo.undoRedoPerformed -= SyncAnimationType);

            void SyncAnimationType() => propertyMoveAnimationType.enumValueIndex = propertyAnimationType.enumValueIndex;

            fieldAnimationTypeEnum.RegisterValueChangedCallback(changeEvent =>
            {
                try
                {
                    propertyMoveAnimationType.enumValueIndex = (int)(UIAnimationType)changeEvent.newValue;
                    UpdateComponentHeaderComponentName((UIAnimationType)changeEvent.newValue);

                    presetsTabButton.SetIsOn(newValue: false, animateChange: false);
                    moveAnimatedContainer.Hide(false);
                    rotateAnimatedContainer.Hide(false);
                    scaleAnimatedContainer.Hide(false);
                    fadeAnimatedContainer.Hide(false);
                }
                catch
                {
                    // ignored
                }
            });

            #region Invisible Fields

            Toggle invisibleMoveEnabledToggle = DesignUtils.NewToggle(propertyMoveEnabled.propertyPath, invisibleField: true);
            Toggle invisibleRotateEnabledToggle = DesignUtils.NewToggle(propertyRotateEnabled.propertyPath, invisibleField: true);
            Toggle invisibleScaleEnabledToggle = DesignUtils.NewToggle(propertyScaleEnabled.propertyPath, invisibleField: true);
            Toggle invisibleFadeEnabledToggle = DesignUtils.NewToggle(propertyFadeEnabled.propertyPath, invisibleField: true);

            drawer
                .AddChild(invisibleMoveEnabledToggle)
                .AddChild(invisibleRotateEnabledToggle)
                .AddChild(invisibleScaleEnabledToggle)
                .AddChild(invisibleFadeEnabledToggle);

            invisibleMoveEnabledToggle.RegisterValueChangedCallback(evt => UpdateIndicators(true));
            invisibleRotateEnabledToggle.RegisterValueChangedCallback(evt => UpdateIndicators(true));
            invisibleScaleEnabledToggle.RegisterValueChangedCallback(evt => UpdateIndicators(true));
            invisibleFadeEnabledToggle.RegisterValueChangedCallback(evt => UpdateIndicators(true));

            UpdateIndicators(false);

            void UpdateIndicator(EnabledIndicator indicator, bool enabled, bool animateChange = true)
            {
                indicator.Toggle(enabled, animateChange);
            }

            void UpdateIndicators(bool animateChange)
            {
                drawer.schedule.Execute(() =>
                {
                    UpdateIndicator(moveTabIndicator, propertyMoveEnabled.boolValue, animateChange);
                    UpdateIndicator(rotateTabIndicator, propertyRotateEnabled.boolValue, animateChange);
                    UpdateIndicator(scaleTabIndicator, propertyScaleEnabled.boolValue, animateChange);
                    UpdateIndicator(fadeTabIndicator, propertyFadeEnabled.boolValue, animateChange);
                    UpdateIndicator(callbacksTabIndicator, HasCallbacks(), animateChange);
                });
            }

            #endregion

            #endregion

            return drawer;
        }

        private static VisualElement GetPresetsContent
        (
            UIAnimation target,
            SerializedProperty property,
            SerializedProperty propertyMoveEnabled,
            SerializedProperty propertyRotateEnabled,
            SerializedProperty propertyScaleEnabled,
            SerializedProperty propertyFadeEnabled,
            SerializedProperty propertyAnimationType
        )
        {
            var animationType = (UIAnimationType)propertyAnimationType.enumValueIndex;

            FluidButton GetNewPresetButton(string text, IEnumerable<Texture2D> textures, EditorSelectableColorInfo selectableColor, string tooltip = "") =>
                DesignUtils.GetNewTinyButton(text, textures, selectableColor, tooltip);

            FluidToggleButtonTab buttonNewPreset =
                FluidToggleButtonTab.Get("New Preset", EditorMicroAnimations.EditorUI.Icons.Plus, selectableAccentColor, "Create a new preset")
                    .SetTabPosition(TabPosition.FloatingTab).SetElementSize(ElementSize.Tiny);

            FluidToggleButtonTab buttonSavePresetAtCustomLocation =
                FluidToggleButtonTab.Get("Custom Save Location", EditorMicroAnimations.EditorUI.Icons.Binoculars, selectableAccentColor, "Save the new preset at a custom location in your project")
                    .SetTabPosition(TabPosition.FloatingTab).SetElementSize(ElementSize.Tiny).SetStyleDisplay(DisplayStyle.None);

            FluidButton buttonLoadPreset = GetNewPresetButton("Load Preset", EditorMicroAnimations.EditorUI.Arrows.ArrowDown, selectableAccentColor, "Load selected preset");
            FluidButton buttonDeletePreset = GetNewPresetButton("", EditorMicroAnimations.EditorUI.Icons.Minus, selectableAccentColor, "Delete selected preset");
            FluidButton buttonSavePreset = GetNewPresetButton("Save Preset", EditorMicroAnimations.EditorUI.Icons.Save, selectableAccentColor, "Save current settings as a new preset").SetStyleDisplay(DisplayStyle.None);
            FluidButton buttonOpenReactor = GetNewPresetButton("Reactor", EditorMicroAnimations.Reactor.Icons.Reactor, selectableAccentColor, "Open the Reactor Window").SetOnClick(ReactorWindow.Open);
            FluidButton buttonRefresh = GetNewPresetButton("", EditorMicroAnimations.EditorUI.Icons.Refresh, selectableAccentColor, "Refresh Presets Database").SetOnClick(() => UIAnimationPresetDatabase.instance.RefreshDatabase());
            FluidButton buttonFindPreset = GetNewPresetButton("Find Preset Location", EditorMicroAnimations.EditorUI.Icons.Location, EditorSelectableColors.Default.ButtonIcon, "Ping the selected preset in the Project view");

            FluidToggleButtonTab buttonNewCategoryName =
                FluidToggleButtonTab.Get("", EditorMicroAnimations.EditorUI.Icons.Edit, selectableAccentColor, "Create a new preset category")
                    .SetTabPosition(TabPosition.FloatingTab).SetElementSize(ElementSize.Tiny).SetStyleDisplay(DisplayStyle.None);


            TextField textFieldNewCategoryName = new TextField().ResetLayout().SetStyleFlexGrow(1).SetStyleDisplay(DisplayStyle.None);
            TextField textFieldNewPresetName = new TextField().ResetLayout().SetStyleFlexGrow(1).SetStyleDisplay(DisplayStyle.None);

            UIAnimationPresetDatabase database = UIAnimationPresetDatabase.instance;
            UIAnimationPresetGroup selectedPresetGroup = database.GetPresetGroup(animationType);

            var categories = selectedPresetGroup.categoryNames.ToList();
            // string selectedCategory = UIAnimationPresetGroup.defaultCategoryName;
            var popupCategories = new PopupField<string>(categories, 0).ResetLayout().SetStyleFlexGrow(1);

            var presetNames = selectedPresetGroup.GetPresetNames(popupCategories.value).ToList();
            // string selectedPresetName = UIAnimationPresetGroup.defaultPresetName;
            var popupPresetNames = new PopupField<string>(presetNames, 0).ResetLayout().SetStyleFlexGrow(1);

            popupCategories.RegisterValueChangedCallback(evt => PresetsUpdatePresetNames());

            buttonLoadPreset.SetOnClick(() =>
            {
                UIAnimationPreset preset = UIAnimationPresetDatabase.instance.GetPreset(animationType, popupCategories.value, popupPresetNames.value);
                if (preset == null) return;
                Undo.RecordObjects(property.serializedObject.targetObjects, "Load Preset");
                if (property.serializedObject.isEditingMultipleObjects)
                {
                    try
                    {
                        foreach (UIAnimator targetAnimator in property.serializedObject.targetObjects.Cast<UIAnimator>())
                            preset.SetAnimationSettings(targetAnimator.animation);
                    }
                    catch
                    {
                        preset.SetAnimationSettings(target);
                    }
                    return;
                }
                preset.SetAnimationSettings(target);
            });

            buttonSavePreset.SetOnClick(() =>
            {
                if (!propertyMoveEnabled.boolValue & !propertyRotateEnabled.boolValue & !propertyScaleEnabled.boolValue & !propertyFadeEnabled.boolValue)
                {
                    EditorUtility.DisplayDialog("Cannot Save Preset", "Enable at least one animation move, rotate, scale or fade, to save a new preset", "Ok");
                    return;
                }

                string category = buttonNewCategoryName.isOn ? textFieldNewCategoryName.value : popupCategories.value;
                string presetName = textFieldNewPresetName.value;

                bool canAddPreset;
                string message;
                (canAddPreset, message) = UIAnimationPresetDatabase.instance.CanAddPreset(animationType, category, presetName);
                if (!canAddPreset)
                {
                    EditorUtility.DisplayDialog("Cannot Save Preset", message, "Ok");
                    return;
                }

                string path = buttonSavePresetAtCustomLocation.isOn
                    ? EditorUtility.SaveFilePanelInProject("Save New Preset", UIAnimationPreset.DataFileName(animationType, category, presetName, ""), "asset", "Select Message")
                    : "";

                var newPreset = UIAnimationPreset.NewPreset(target, category, presetName, path);

                buttonNewPreset.isOn = false;

                buttonSavePreset.schedule.Execute(() =>
                {
                    UIAnimationPresetDatabase.instance.RefreshDatabase();
                    PresetsUpdateCategories();
                    PresetsUpdatePresetNames();
                    popupCategories.value = category;
                    popupPresetNames.value = presetName;
                });

                Debug.Log($"{animationType} Preset Created - [ {category} {presetName} ] - at '{AssetDatabase.GetAssetPath(newPreset)}'");

            });

            buttonDeletePreset.SetOnClick(() =>
            {
                if (popupCategories.value.Equals(UIAnimationPresetGroup.defaultCategoryName) ||
                    popupPresetNames.value.Equals(UIAnimationPresetGroup.defaultPresetName))
                {
                    EditorUtility.DisplayDialog(
                        "Delete Preset",
                        $"Cannot delete preset: '{popupCategories.value} - {popupPresetNames.value}'",
                        "Ok");
                    return;
                }

                if (!EditorUtility.DisplayDialog(
                    "Delete Preset",
                    $"Are you sure you want to delete the selected preset? '{popupCategories.value} - {popupPresetNames.value}'",
                    "Continue", "Cancel"))
                    return;

                UIAnimationPreset preset = UIAnimationPresetDatabase.instance.GetPreset(animationType, popupCategories.value, popupPresetNames.value);
                if (preset == null)
                {
                    EditorUtility.DisplayDialog(
                        "Delete Preset",
                        $"Preset not found: '{popupCategories.value} - {popupPresetNames.value}'",
                        "Ok");

                    UIAnimationPresetDatabase.instance.RefreshDatabase();
                    // tabPresets.isOn = false;
                    PresetsUpdateCategories();
                    PresetsUpdatePresetNames();
                }

                // tabPresets.isOn = false;
                AssetDatabase.MoveAssetToTrash(AssetDatabase.GetAssetPath(preset));
                UIAnimationPresetDatabase.instance.RefreshDatabase();
                PresetsUpdateCategories();
                PresetsUpdatePresetNames();

            });

            void PresetsUpdateCategories()
            {
                selectedPresetGroup = database.GetPresetGroup(animationType);
                categories.Clear();
                categories.AddRange(selectedPresetGroup.categoryNames);
                popupCategories.value = categories[0];
            }

            void PresetsUpdatePresetNames()
            {
                selectedPresetGroup = database.GetPresetGroup(animationType);
                presetNames.Clear();
                presetNames.AddRange(selectedPresetGroup.GetPresetNames(popupCategories.value));
                popupPresetNames.value = presetNames[0];
            }

            var fieldCategory =
                FluidField.Get("Preset Category",
                    DesignUtils.row
                        .AddChild(popupCategories)
                        .AddChild(textFieldNewCategoryName)
                        .AddChild(buttonNewCategoryName.SetStyleMarginLeft(DesignUtils.k_Spacing)));

            var fieldPresetName =
                FluidField.Get("Preset Name",
                    DesignUtils.row
                        .AddChild(popupPresetNames)
                        .AddChild(textFieldNewPresetName)
                        .AddSpace(DesignUtils.k_Spacing, 0)
                        .AddChild(buttonDeletePreset)
                        .AddSpace(DesignUtils.k_Spacing, 0)
                        .AddChild(buttonLoadPreset)
                        .AddChild(buttonSavePreset));

            buttonNewPreset.OnValueChanged += evt =>
            {
                if (evt.newValue)
                {
                    fieldPresetName.SetLabelText("New Preset Name");
                    textFieldNewPresetName.value = popupPresetNames.value;

                    buttonLoadPreset.SetStyleDisplay(DisplayStyle.None);
                    popupPresetNames.SetStyleDisplay(DisplayStyle.None);
                    buttonDeletePreset.SetStyleDisplay(DisplayStyle.None);


                    textFieldNewPresetName.SetStyleDisplay(DisplayStyle.Flex);
                    buttonNewCategoryName.SetStyleDisplay(DisplayStyle.Flex);
                    buttonSavePresetAtCustomLocation.SetStyleDisplay(DisplayStyle.Flex);
                    buttonSavePreset.SetStyleDisplay(DisplayStyle.Flex);

                    FluidButtonExtensions.DisableElement(buttonFindPreset);
                }
                else
                {
                    fieldPresetName.SetLabelText("Preset Name");
                    buttonNewCategoryName.SetIsOn(false, evt.animateChange);

                    buttonLoadPreset.SetStyleDisplay(DisplayStyle.Flex);
                    popupPresetNames.SetStyleDisplay(DisplayStyle.Flex);
                    buttonDeletePreset.SetStyleDisplay(DisplayStyle.Flex);

                    textFieldNewPresetName.SetStyleDisplay(DisplayStyle.None);
                    buttonNewCategoryName.SetStyleDisplay(DisplayStyle.None);
                    buttonSavePresetAtCustomLocation.SetStyleDisplay(DisplayStyle.None);
                    buttonSavePreset.SetStyleDisplay(DisplayStyle.None);

                    FluidButtonExtensions.EnableElement(buttonFindPreset);
                }
            };

            buttonNewCategoryName.OnValueChanged += evt =>
            {
                popupCategories.SetStyleDisplay(evt.newValue ? DisplayStyle.None : DisplayStyle.Flex);
                textFieldNewCategoryName.SetStyleDisplay(evt.newValue ? DisplayStyle.Flex : DisplayStyle.None);
                fieldCategory.SetLabelText(evt.newValue ? "New Category Name" : "Preset Category");
            };

            buttonFindPreset.SetOnClick(() =>
            {
                UIAnimationPreset preset = UIAnimationPresetDatabase.instance.GetPreset(animationType, popupCategories.value, popupPresetNames.value);
                if (preset == null) return;
                EditorGUIUtility.PingObject(preset);
            });

            FluidField foldoutContent =
                FluidField.Get().AddFieldContent
                    (
                        DesignUtils.row
                            .SetStyleMarginBottom(DesignUtils.k_Spacing)
                            .AddChild(buttonOpenReactor)
                            .AddSpace(DesignUtils.k_Spacing, 0)
                            .AddChild(buttonRefresh)
                            .AddSpace(DesignUtils.k_Spacing, 0)
                            .AddChild(buttonFindPreset)
                            .AddSpace(DesignUtils.k_Spacing, 0)
                            .AddChild(DesignUtils.flexibleSpace)
                            .AddChild(buttonNewPreset)
                            .AddSpace(DesignUtils.k_Spacing, 0)
                            .AddChild(buttonSavePresetAtCustomLocation)
                    )
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(fieldCategory)
                            .AddChild(fieldPresetName)
                    );



            return foldoutContent;
        }

        private static VisualElement GetMoveContent
        (
            SerializedProperty propertyMove,
            SerializedProperty propertyMoveEnabled,
            SerializedProperty propertyAnimationType
        )
        {
            SerializedProperty propertyMoveFromReferenceValue = propertyMove.FindPropertyRelative("FromReferenceValue");
            SerializedProperty propertyMoveFromCustomValue = propertyMove.FindPropertyRelative("FromCustomValue");
            SerializedProperty propertyMoveFromOffset = propertyMove.FindPropertyRelative("FromOffset");
            SerializedProperty propertyMoveFromDirection = propertyMove.FindPropertyRelative("FromDirection");

            SerializedProperty propertyMoveToReferenceValue = propertyMove.FindPropertyRelative("ToReferenceValue");
            SerializedProperty propertyMoveToCustomValue = propertyMove.FindPropertyRelative("ToCustomValue");
            SerializedProperty propertyMoveToOffset = propertyMove.FindPropertyRelative("ToOffset");
            SerializedProperty propertyMoveToDirection = propertyMove.FindPropertyRelative("ToDirection");

            SerializedProperty propertyMoveSettings = propertyMove.FindPropertyRelative("Settings");

            var content = new VisualElement();
            content.SetEnabled(propertyMoveEnabled.boolValue);
            var enableSwitch = NewEnableAnimationSwitch("Move", moveSColor, propertyMoveEnabled, content);

            var fieldMoveFromReferenceValue = new EnumField().SetBindingPath(propertyMoveFromReferenceValue.propertyPath).ResetLayout();
            var fieldMoveFromOffset = FluidField.Get("From Offset", new Vector3Field().SetBindingPath(propertyMoveFromOffset.propertyPath).ResetLayout());
            var fieldMoveFromCustomValue = FluidField.Get("From Custom Position", new Vector3Field().SetBindingPath(propertyMoveFromCustomValue.propertyPath).ResetLayout());
            var fieldMoveFromDirection = new PropertyField().SetBindingPath(propertyMoveFromDirection.propertyPath).ResetLayout();
            var fieldMoveFrom = FluidField.Get("Move From").AddFieldContent(fieldMoveFromReferenceValue).AddFieldContent(fieldMoveFromDirection);

            var fieldMoveToReferenceValue = new EnumField().SetBindingPath(propertyMoveToReferenceValue.propertyPath).ResetLayout();
            var fieldMoveToOffset = FluidField.Get("To Offset", new Vector3Field().SetBindingPath(propertyMoveToOffset.propertyPath).ResetLayout());
            var fieldMoveToCustomValue = FluidField.Get("To Custom Position", new Vector3Field().SetBindingPath(propertyMoveToCustomValue.propertyPath).ResetLayout());
            var fieldMoveToDirection = new PropertyField().SetBindingPath(propertyMoveToDirection.propertyPath).ResetLayout();
            var fieldMoveTo = FluidField.Get("Move To").AddFieldContent(fieldMoveToReferenceValue).AddFieldContent(fieldMoveToDirection);


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
                                            .AddChild(fieldMoveFrom.SetStyleFlexGrow(1))
                                            .AddSpace(0, DesignUtils.k_Spacing)
                                            .AddChild(fieldMoveFromOffset.SetStyleFlexGrow(1).SetStyleHeight(42).SetStyleMaxHeight(42))
                                            .AddChild(fieldMoveFromCustomValue.SetStyleHeight(42).SetStyleFlexGrow(1).SetStyleMaxHeight(42))
                                    )
                                    .AddSpace(DesignUtils.k_Spacing, 0)
                                    .AddChild
                                    (
                                        DesignUtils.flexibleSpace
                                            .AddChild(fieldMoveTo.SetStyleFlexGrow(1))
                                            .AddSpace(0, DesignUtils.k_Spacing)
                                            .AddChild(fieldMoveToOffset.SetStyleFlexGrow(1).SetStyleHeight(42).SetStyleMaxHeight(42))
                                            .AddChild(fieldMoveToCustomValue.SetStyleHeight(42).SetStyleFlexGrow(1).SetStyleMaxHeight(42))
                                    )
                            )
                            .AddSpace(0, DesignUtils.k_Spacing)
                            .AddChild(new PropertyField().SetBindingPath(propertyMoveSettings.propertyPath))
                    )
                    .AddSpace(0, DesignUtils.k_EndOfLineSpacing);

            void Update()
            {
                var animationType = (UIAnimationType)propertyAnimationType.enumValueIndex;

                switch (animationType)
                {
                    case UIAnimationType.Button:
                    case UIAnimationType.State:
                    case UIAnimationType.Reset:
                    case UIAnimationType.Loop:
                    case UIAnimationType.Custom:
                    {
                        #region Move - From

                        fieldMoveFromReferenceValue.SetStyleDisplay(DisplayStyle.Flex);
                        fieldMoveFromDirection.SetStyleDisplay(DisplayStyle.None);

                        switch ((ReferenceValue)propertyMoveFromReferenceValue.enumValueIndex)
                        {
                            case ReferenceValue.StartValue:
                                fieldMoveFromOffset.SetStyleDisplay(DisplayStyle.Flex);
                                fieldMoveFromCustomValue.SetStyleDisplay(DisplayStyle.None);
                                break;
                            case ReferenceValue.CurrentValue:
                                fieldMoveFromOffset.SetStyleDisplay(DisplayStyle.Flex);
                                fieldMoveFromCustomValue.SetStyleDisplay(DisplayStyle.None);
                                break;
                            case ReferenceValue.CustomValue:
                                fieldMoveFromOffset.SetStyleDisplay(DisplayStyle.None);
                                fieldMoveFromCustomValue.SetStyleDisplay(DisplayStyle.Flex);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        #endregion

                        #region Move - To

                        fieldMoveToReferenceValue.SetStyleDisplay(DisplayStyle.Flex);
                        fieldMoveToDirection.SetStyleDisplay(DisplayStyle.None);

                        switch ((ReferenceValue)propertyMoveToReferenceValue.enumValueIndex)
                        {
                            case ReferenceValue.StartValue:
                                fieldMoveToOffset.SetStyleDisplay(DisplayStyle.Flex);
                                fieldMoveToCustomValue.SetStyleDisplay(DisplayStyle.None);
                                break;
                            case ReferenceValue.CurrentValue:
                                fieldMoveToOffset.SetStyleDisplay(DisplayStyle.Flex);
                                fieldMoveToCustomValue.SetStyleDisplay(DisplayStyle.None);
                                break;
                            case ReferenceValue.CustomValue:
                                fieldMoveToOffset.SetStyleDisplay(DisplayStyle.None);
                                fieldMoveToCustomValue.SetStyleDisplay(DisplayStyle.Flex);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        #endregion

                        break;
                    }
                    case UIAnimationType.Show:
                    {
                        #region Move - From

                        fieldMoveFromReferenceValue.SetStyleDisplay(DisplayStyle.None);
                        fieldMoveFromDirection.SetStyleDisplay(DisplayStyle.Flex);
                        fieldMoveFromOffset.SetStyleDisplay((MoveDirection)propertyMoveFromDirection.enumValueIndex == MoveDirection.CustomPosition ? DisplayStyle.None : DisplayStyle.Flex);
                        fieldMoveFromCustomValue.SetStyleDisplay((MoveDirection)propertyMoveFromDirection.enumValueIndex == MoveDirection.CustomPosition ? DisplayStyle.Flex : DisplayStyle.None);

                        #endregion

                        #region Move - To

                        fieldMoveToReferenceValue.SetStyleDisplay(DisplayStyle.Flex);
                        fieldMoveToDirection.SetStyleDisplay(DisplayStyle.None);
                        fieldMoveToOffset.SetStyleDisplay((ReferenceValue)propertyMoveToReferenceValue.enumValueIndex == ReferenceValue.CustomValue ? DisplayStyle.None : DisplayStyle.Flex);
                        fieldMoveToCustomValue.SetStyleDisplay((ReferenceValue)propertyMoveToReferenceValue.enumValueIndex == ReferenceValue.CustomValue ? DisplayStyle.Flex : DisplayStyle.None);

                        #endregion

                        break;
                    }
                    case UIAnimationType.Hide:
                    {
                        #region Move - From

                        fieldMoveFromReferenceValue.SetStyleDisplay(DisplayStyle.Flex);
                        fieldMoveFromDirection.SetStyleDisplay(DisplayStyle.None);
                        fieldMoveFromOffset.SetStyleDisplay((ReferenceValue)propertyMoveFromReferenceValue.enumValueIndex == ReferenceValue.CustomValue ? DisplayStyle.None : DisplayStyle.Flex);
                        fieldMoveFromCustomValue.SetStyleDisplay((ReferenceValue)propertyMoveFromReferenceValue.enumValueIndex == ReferenceValue.CustomValue ? DisplayStyle.Flex : DisplayStyle.None);

                        #endregion

                        #region Move - To

                        fieldMoveToReferenceValue.SetStyleDisplay(DisplayStyle.None);
                        fieldMoveToDirection.SetStyleDisplay(DisplayStyle.Flex);
                        fieldMoveToOffset.SetStyleDisplay((MoveDirection)propertyMoveToDirection.enumValueIndex == MoveDirection.CustomPosition ? DisplayStyle.None : DisplayStyle.Flex);
                        fieldMoveToCustomValue.SetStyleDisplay((MoveDirection)propertyMoveToDirection.enumValueIndex == MoveDirection.CustomPosition ? DisplayStyle.Flex : DisplayStyle.None);

                        #endregion

                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var fromDirection = (MoveDirection)propertyMoveFromDirection.enumValueIndex;
                var toDirection = (MoveDirection)propertyMoveToDirection.enumValueIndex;

                switch (animationType)
                {
                    case UIAnimationType.Button:
                    case UIAnimationType.State:
                    case UIAnimationType.Reset:
                    case UIAnimationType.Custom:
                    case UIAnimationType.Loop:
                        break;
                    case UIAnimationType.Show:
                        if (toDirection != MoveDirection.CustomPosition)
                        {
                            propertyMoveToDirection.enumValueIndex = (int)MoveDirection.CustomPosition;
                            propertyMove.serializedObject.ApplyModifiedProperties();
                        }
                        break;
                    case UIAnimationType.Hide:
                        if (fromDirection != MoveDirection.CustomPosition)
                        {
                            propertyMoveFromDirection.enumValueIndex = (int)MoveDirection.CustomPosition;
                            propertyMove.serializedObject.ApplyModifiedProperties();
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }


            //Move - FromReferenceValue
            var invisibleFieldMoveFromReferenceValueEnum = new EnumField { bindingPath = propertyMoveFromReferenceValue.propertyPath }.SetStyleDisplay(DisplayStyle.None);
            foldoutContent.AddChild(invisibleFieldMoveFromReferenceValueEnum);
            invisibleFieldMoveFromReferenceValueEnum.RegisterValueChangedCallback(changeEvent => Update());

            //Move - FromDirection
            var invisibleFieldMoveFromDirectionEnum = new EnumField { bindingPath = propertyMoveFromDirection.propertyPath }.SetStyleDisplay(DisplayStyle.None);
            foldoutContent.AddChild(invisibleFieldMoveFromDirectionEnum);
            invisibleFieldMoveFromDirectionEnum.RegisterValueChangedCallback(changeEvent => Update());

            //Move - ToReferenceValue
            var invisibleFieldMoveToReferenceValueEnum = new EnumField { bindingPath = propertyMoveToReferenceValue.propertyPath }.SetStyleDisplay(DisplayStyle.None);
            foldoutContent.AddChild(invisibleFieldMoveToReferenceValueEnum);
            invisibleFieldMoveToReferenceValueEnum.RegisterValueChangedCallback(changeEvent => Update());

            //Move - ToDirection
            var invisibleFieldMoveToDirectionEnum = new EnumField { bindingPath = propertyMoveToDirection.propertyPath }.SetStyleDisplay(DisplayStyle.None);
            foldoutContent.AddChild(invisibleFieldMoveToDirectionEnum);
            invisibleFieldMoveToDirectionEnum.RegisterValueChangedCallback(changeEvent => Update());

            foldoutContent.Bind(propertyMove.serializedObject);

            Update();
            return foldoutContent;
        }

        private static VisualElement GetRotateContent
        (
            SerializedProperty propertyRotate,
            SerializedProperty propertyRotateEnabled
        )
        {
            SerializedProperty propertyRotateFromReferenceValue = propertyRotate.FindPropertyRelative("FromReferenceValue");
            SerializedProperty propertyRotateFromCustomValue = propertyRotate.FindPropertyRelative("FromCustomValue");
            SerializedProperty propertyRotateFromOffset = propertyRotate.FindPropertyRelative("FromOffset");

            SerializedProperty propertyRotateToReferenceValue = propertyRotate.FindPropertyRelative("ToReferenceValue");
            SerializedProperty propertyRotateToCustomValue = propertyRotate.FindPropertyRelative("ToCustomValue");
            SerializedProperty propertyRotateToOffset = propertyRotate.FindPropertyRelative("ToOffset");

            SerializedProperty propertyRotateSettings = propertyRotate.FindPropertyRelative("Settings");

            var content = new VisualElement();
            content.SetEnabled(propertyRotateEnabled.boolValue);
            var enableSwitch = NewEnableAnimationSwitch("Rotate", rotateSColor, propertyRotateEnabled, content);

            var fieldRotateFromReferenceValue = new EnumField().SetBindingPath(propertyRotateFromReferenceValue.propertyPath).ResetLayout();
            var fieldRotateFromOffset = FluidField.Get("From Offset", new Vector3Field().SetBindingPath(propertyRotateFromOffset.propertyPath).ResetLayout());
            var fieldRotateFromCustomValue = FluidField.Get("From Custom Rotation", new Vector3Field().SetBindingPath(propertyRotateFromCustomValue.propertyPath).ResetLayout());
            var fieldRotateFrom = FluidField.Get("Rotate From").AddFieldContent(fieldRotateFromReferenceValue);

            var fieldRotateToReferenceValue = new EnumField().SetBindingPath(propertyRotateToReferenceValue.propertyPath).ResetLayout();
            var fieldRotateToOffset = FluidField.Get("To Offset", new Vector3Field().SetBindingPath(propertyRotateToOffset.propertyPath).ResetLayout());
            var fieldRotateToCustomValue = FluidField.Get("To Custom Rotation", new Vector3Field().SetBindingPath(propertyRotateToCustomValue.propertyPath).ResetLayout());
            var fieldRotateTo = FluidField.Get("Rotate To").AddFieldContent(fieldRotateToReferenceValue);

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
                        content.AddChild
                            (
                                DesignUtils.row
                                    .AddChild
                                    (
                                        DesignUtils.flexibleSpace
                                            .AddChild(fieldRotateFrom.SetStyleFlexGrow(1))
                                            .AddSpace(0, DesignUtils.k_Spacing)
                                            .AddChild(fieldRotateFromOffset.SetStyleFlexGrow(1).SetStyleHeight(42).SetStyleMaxHeight(42))
                                            .AddChild(fieldRotateFromCustomValue.SetStyleHeight(42).SetStyleFlexGrow(1).SetStyleMaxHeight(42))
                                    )
                                    .AddSpace(DesignUtils.k_Spacing, 0)
                                    .AddChild
                                    (
                                        DesignUtils.flexibleSpace
                                            .AddChild(fieldRotateTo.SetStyleFlexGrow(1))
                                            .AddSpace(0, DesignUtils.k_Spacing)
                                            .AddChild(fieldRotateToOffset.SetStyleFlexGrow(1).SetStyleHeight(42).SetStyleMaxHeight(42))
                                            .AddChild(fieldRotateToCustomValue.SetStyleHeight(42).SetStyleFlexGrow(1).SetStyleMaxHeight(42))
                                    )
                            )
                            .AddSpace(0, DesignUtils.k_Spacing)
                            .AddChild(new PropertyField().SetBindingPath(propertyRotateSettings.propertyPath))
                            .AddSpace(0, DesignUtils.k_EndOfLineSpacing)
                    );

            void Update()
            {
                fieldRotateFromOffset.SetStyleDisplay((ReferenceValue)propertyRotateFromReferenceValue.enumValueIndex == ReferenceValue.CustomValue ? DisplayStyle.None : DisplayStyle.Flex);
                fieldRotateFromCustomValue.SetStyleDisplay((ReferenceValue)propertyRotateFromReferenceValue.enumValueIndex != ReferenceValue.CustomValue ? DisplayStyle.None : DisplayStyle.Flex);
                fieldRotateToOffset.SetStyleDisplay((ReferenceValue)propertyRotateToReferenceValue.enumValueIndex == ReferenceValue.CustomValue ? DisplayStyle.None : DisplayStyle.Flex);
                fieldRotateToCustomValue.SetStyleDisplay((ReferenceValue)propertyRotateToReferenceValue.enumValueIndex != ReferenceValue.CustomValue ? DisplayStyle.None : DisplayStyle.Flex);
            }

            //Rotate - FromReferenceValue
            var invisibleFieldRotateFromReferenceValueEnum = new EnumField { bindingPath = propertyRotateFromReferenceValue.propertyPath }.SetStyleDisplay(DisplayStyle.None);
            foldoutContent.AddChild(invisibleFieldRotateFromReferenceValueEnum);
            invisibleFieldRotateFromReferenceValueEnum.RegisterValueChangedCallback(changeEvent => Update());

            //Rotate - ToReferenceValue
            var invisibleFieldRotateToReferenceValueEnum = new EnumField { bindingPath = propertyRotateToReferenceValue.propertyPath }.SetStyleDisplay(DisplayStyle.None);
            foldoutContent.AddChild(invisibleFieldRotateToReferenceValueEnum);
            invisibleFieldRotateToReferenceValueEnum.RegisterValueChangedCallback(changeEvent => Update());

            foldoutContent.Bind(propertyRotate.serializedObject);

            Update();
            return foldoutContent;
        }

        private static VisualElement GetScaleContent
        (
            SerializedProperty propertyScale,
            SerializedProperty propertyScaleEnabled
        )
        {
            SerializedProperty propertyScaleFromReferenceValue = propertyScale.FindPropertyRelative("FromReferenceValue");
            SerializedProperty propertyScaleFromCustomValue = propertyScale.FindPropertyRelative("FromCustomValue");
            SerializedProperty propertyScaleFromOffset = propertyScale.FindPropertyRelative("FromOffset");

            SerializedProperty propertyScaleToReferenceValue = propertyScale.FindPropertyRelative("ToReferenceValue");
            SerializedProperty propertyScaleToCustomValue = propertyScale.FindPropertyRelative("ToCustomValue");
            SerializedProperty propertyScaleToOffset = propertyScale.FindPropertyRelative("ToOffset");

            SerializedProperty propertyScaleSettings = propertyScale.FindPropertyRelative("Settings");

            var content = new VisualElement();
            content.SetEnabled(propertyScaleEnabled.boolValue);
            var enableSwitch = NewEnableAnimationSwitch("Scale", scaleSColor, propertyScaleEnabled, content);

            var fieldScaleFromReferenceValue = new EnumField().SetBindingPath(propertyScaleFromReferenceValue.propertyPath).ResetLayout();
            var fieldScaleFromOffset = FluidField.Get("From Offset", new Vector3Field().SetBindingPath(propertyScaleFromOffset.propertyPath).ResetLayout());
            var fieldScaleFromCustomValue = FluidField.Get("From Custom Scale", new Vector3Field().SetBindingPath(propertyScaleFromCustomValue.propertyPath).ResetLayout());
            var fieldScaleFrom = FluidField.Get("Scale From").AddFieldContent(fieldScaleFromReferenceValue);

            var fieldScaleToReferenceValue = new EnumField().SetBindingPath(propertyScaleToReferenceValue.propertyPath).ResetLayout();
            var fieldScaleToOffset = FluidField.Get("To Offset", new Vector3Field().SetBindingPath(propertyScaleToOffset.propertyPath).ResetLayout());
            var fieldScaleToCustomValue = FluidField.Get("To Custom Scale", new Vector3Field().SetBindingPath(propertyScaleToCustomValue.propertyPath).ResetLayout());
            var fieldScaleTo = FluidField.Get("Scale To").AddFieldContent(fieldScaleToReferenceValue);

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
                                            .AddChild(fieldScaleFrom.SetStyleFlexGrow(1))
                                            .AddSpace(0, DesignUtils.k_Spacing)
                                            .AddChild(fieldScaleFromOffset.SetStyleFlexGrow(1).SetStyleHeight(42).SetStyleMaxHeight(42))
                                            .AddChild(fieldScaleFromCustomValue.SetStyleHeight(42).SetStyleFlexGrow(1).SetStyleMaxHeight(42))
                                    )
                                    .AddSpace(DesignUtils.k_Spacing, 0)
                                    .AddChild
                                    (
                                        DesignUtils.flexibleSpace
                                            .AddChild(fieldScaleTo.SetStyleFlexGrow(1))
                                            .AddSpace(0, DesignUtils.k_Spacing)
                                            .AddChild(fieldScaleToOffset.SetStyleFlexGrow(1).SetStyleHeight(42).SetStyleMaxHeight(42))
                                            .AddChild(fieldScaleToCustomValue.SetStyleHeight(42).SetStyleFlexGrow(1).SetStyleMaxHeight(42))
                                    )
                            )
                            .AddSpace(0, DesignUtils.k_Spacing)
                            .AddChild(new PropertyField().SetBindingPath(propertyScaleSettings.propertyPath))
                            .AddSpace(0, DesignUtils.k_EndOfLineSpacing)
                    );


            void Update()
            {
                fieldScaleFromOffset.SetStyleDisplay((ReferenceValue)propertyScaleFromReferenceValue.enumValueIndex == ReferenceValue.CustomValue ? DisplayStyle.None : DisplayStyle.Flex);
                fieldScaleFromCustomValue.SetStyleDisplay((ReferenceValue)propertyScaleFromReferenceValue.enumValueIndex != ReferenceValue.CustomValue ? DisplayStyle.None : DisplayStyle.Flex);
                fieldScaleToOffset.SetStyleDisplay((ReferenceValue)propertyScaleToReferenceValue.enumValueIndex == ReferenceValue.CustomValue ? DisplayStyle.None : DisplayStyle.Flex);
                fieldScaleToCustomValue.SetStyleDisplay((ReferenceValue)propertyScaleToReferenceValue.enumValueIndex != ReferenceValue.CustomValue ? DisplayStyle.None : DisplayStyle.Flex);
            }

            //Scale - FromReferenceValue
            var invisibleFieldScaleFromReferenceValueEnum = new EnumField { bindingPath = propertyScaleFromReferenceValue.propertyPath }.SetStyleDisplay(DisplayStyle.None);
            foldoutContent.AddChild(invisibleFieldScaleFromReferenceValueEnum);
            invisibleFieldScaleFromReferenceValueEnum.RegisterValueChangedCallback(changeEvent => Update());

            //Scale - ToReferenceValue
            var invisibleFieldScaleToReferenceValueEnum = new EnumField { bindingPath = propertyScaleToReferenceValue.propertyPath }.SetStyleDisplay(DisplayStyle.None);
            foldoutContent.AddChild(invisibleFieldScaleToReferenceValueEnum);
            invisibleFieldScaleToReferenceValueEnum.RegisterValueChangedCallback(changeEvent => Update());

            foldoutContent.Bind(propertyScale.serializedObject);

            Update();
            return foldoutContent;
        }

        private static VisualElement GetFadeContent
        (
            SerializedProperty propertyFade,
            SerializedProperty propertyFadeEnabled
        )
        {
            SerializedProperty propertyFadeFromReferenceValue = propertyFade.FindPropertyRelative("FromReferenceValue");
            SerializedProperty propertyFadeFromCustomValue = propertyFade.FindPropertyRelative("FromCustomValue");
            SerializedProperty propertyFadeFromOffset = propertyFade.FindPropertyRelative("FromOffset");

            SerializedProperty propertyFadeToReferenceValue = propertyFade.FindPropertyRelative("ToReferenceValue");
            SerializedProperty propertyFadeToCustomValue = propertyFade.FindPropertyRelative("ToCustomValue");
            SerializedProperty propertyFadeToOffset = propertyFade.FindPropertyRelative("ToOffset");

            SerializedProperty propertyFadeSettings = propertyFade.FindPropertyRelative("Settings");

            var content = new VisualElement();
            content.SetEnabled(propertyFadeEnabled.boolValue);
            var enableSwitch = NewEnableAnimationSwitch("Fade", fadeSColor, propertyFadeEnabled, content);

            var fieldFadeFromReferenceValue = new EnumField().SetBindingPath(propertyFadeFromReferenceValue.propertyPath).ResetLayout();
            var fieldFadeFromOffset = FluidField.Get("From Offset", new FloatField().SetBindingPath(propertyFadeFromOffset.propertyPath).ResetLayout());
            var fieldFadeFromCustomValue = FluidField.Get("From Custom Fade", new FloatField().SetBindingPath(propertyFadeFromCustomValue.propertyPath).ResetLayout());
            var fieldFadeFrom = FluidField.Get("Fade From").AddFieldContent(fieldFadeFromReferenceValue);

            var fieldFadeToReferenceValue = new EnumField().SetBindingPath(propertyFadeToReferenceValue.propertyPath).ResetLayout();
            var fieldFadeToOffset = FluidField.Get("To Offset", new FloatField().SetBindingPath(propertyFadeToOffset.propertyPath).ResetLayout());
            var fieldFadeToCustomValue = FluidField.Get("To Custom Fade", new FloatField().SetBindingPath(propertyFadeToCustomValue.propertyPath).ResetLayout());
            var fieldFadeTo = FluidField.Get("Fade To").AddFieldContent(fieldFadeToReferenceValue);

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
                                            .AddChild(fieldFadeFrom.SetStyleFlexGrow(1))
                                            .AddSpace(0, DesignUtils.k_Spacing)
                                            .AddChild(fieldFadeFromOffset.SetStyleFlexGrow(1).SetStyleHeight(42).SetStyleMaxHeight(42))
                                            .AddChild(fieldFadeFromCustomValue.SetStyleHeight(42).SetStyleFlexGrow(1).SetStyleMaxHeight(42))
                                    )
                                    .AddSpace(DesignUtils.k_Spacing, 0)
                                    .AddChild
                                    (
                                        DesignUtils.flexibleSpace
                                            .AddChild(fieldFadeTo.SetStyleFlexGrow(1))
                                            .AddSpace(0, DesignUtils.k_Spacing)
                                            .AddChild(fieldFadeToOffset.SetStyleFlexGrow(1).SetStyleHeight(42).SetStyleMaxHeight(42))
                                            .AddChild(fieldFadeToCustomValue.SetStyleHeight(42).SetStyleFlexGrow(1).SetStyleMaxHeight(42))
                                    ))
                            .AddSpace(0, DesignUtils.k_Spacing)
                            .AddChild(new PropertyField().SetBindingPath(propertyFadeSettings.propertyPath))
                            .AddSpace(0, DesignUtils.k_EndOfLineSpacing)
                    );

            void Update()
            {
                fieldFadeFromOffset.SetStyleDisplay((ReferenceValue)propertyFadeFromReferenceValue.enumValueIndex == ReferenceValue.CustomValue ? DisplayStyle.None : DisplayStyle.Flex);
                fieldFadeFromCustomValue.SetStyleDisplay((ReferenceValue)propertyFadeFromReferenceValue.enumValueIndex != ReferenceValue.CustomValue ? DisplayStyle.None : DisplayStyle.Flex);
                fieldFadeToOffset.SetStyleDisplay((ReferenceValue)propertyFadeToReferenceValue.enumValueIndex == ReferenceValue.CustomValue ? DisplayStyle.None : DisplayStyle.Flex);
                fieldFadeToCustomValue.SetStyleDisplay((ReferenceValue)propertyFadeToReferenceValue.enumValueIndex != ReferenceValue.CustomValue ? DisplayStyle.None : DisplayStyle.Flex);
            }

            //Fade - FromReferenceValue
            var invisibleFieldFadeFromReferenceValueEnum = new EnumField { bindingPath = propertyFadeFromReferenceValue.propertyPath }.SetStyleDisplay(DisplayStyle.None);
            foldoutContent.AddChild(invisibleFieldFadeFromReferenceValueEnum);
            invisibleFieldFadeFromReferenceValueEnum.RegisterValueChangedCallback(changeEvent => Update());

            //Fade - ToReferenceValue
            var invisibleFieldFadeToReferenceValueEnum = new EnumField { bindingPath = propertyFadeToReferenceValue.propertyPath }.SetStyleDisplay(DisplayStyle.None);
            foldoutContent.AddChild(invisibleFieldFadeToReferenceValueEnum);
            invisibleFieldFadeToReferenceValueEnum.RegisterValueChangedCallback(changeEvent => Update());

            foldoutContent.Bind(propertyFade.serializedObject);

            Update();
            return foldoutContent;
        }

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
