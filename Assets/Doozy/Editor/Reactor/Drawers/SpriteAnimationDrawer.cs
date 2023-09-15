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
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Components;
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
using Object = UnityEngine.Object;

namespace Doozy.Editor.Reactor.Drawers
{
    [CustomPropertyDrawer(typeof(SpriteAnimation), true)]
    public class SpriteAnimationDrawer : PropertyDrawer
    {
        private static Color accentColor => EditorColors.Reactor.Red;
        private static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Reactor.Red;

        private static IEnumerable<Texture2D> spriteAnimationIconTextures => EditorMicroAnimations.Reactor.Icons.SpriteAnimation;
        private static IEnumerable<Texture2D> unityEventIconTextures => EditorMicroAnimations.EditorUI.Icons.UnityEvent;
        private static IEnumerable<Texture2D> resetIconTextures => EditorMicroAnimations.EditorUI.Icons.Reset;
        private static IEnumerable<Texture2D> spriteIconTextures => EditorMicroAnimations.EditorUI.Icons.Sprite;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var drawer = new VisualElement();
            if (property == null) return drawer;
            var target = property.GetTargetObjectOfProperty() as SpriteAnimation;

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
            SerializedProperty propertySprites = property.FindPropertyRelative("Sprites");
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
                    .SetSecondaryIcon(spriteAnimationIconTextures.ToList())
                    .SetComponentNameText("Sprite Animation")
                    .AddManualButton("www.bit.ly/DoozyKnowledgeBase4")
                    .AddYouTubeButton();

            poolables.Add(componentHeader);

            #endregion

            #region Tabs

            VisualElement tabsContainer = DesignUtils.row.SetStyleJustifyContent(Justify.Center).SetStyleMargins(12, 0, 12, 0);
            FluidToggleButtonTab animationTabButton, callbacksTabButton, spritesTabButton;
            EnabledIndicator animationTabIndicator, callbacksTabIndicator;
            VisualElement animationTabContainer, callbacksTabContainer;

            (animationTabButton, animationTabIndicator, animationTabContainer) = DesignUtils.GetTabButtonForComponentSectionWithEnabledIndicator(spriteAnimationIconTextures, selectableAccentColor, accentColor);
            (callbacksTabButton, callbacksTabIndicator, callbacksTabContainer) = DesignUtils.GetTabButtonForComponentSectionWithEnabledIndicator(unityEventIconTextures, DesignUtils.callbackSelectableColor, DesignUtils.callbacksColor);
            spritesTabButton = DesignUtils.GetTabButtonForComponentSection(spriteIconTextures, selectableAccentColor);


            animationTabIndicator.Toggle(propertyAnimationEnabled.boolValue, false);

            poolables.Add(animationTabIndicator);
            poolables.Add(callbacksTabIndicator);

            animationTabButton.SetLabelText("Animation");
            callbacksTabButton.SetLabelText("Callbacks");
            spritesTabButton.SetLabelText("Sprites");

            poolables.Add(animationTabButton);
            poolables.Add(callbacksTabButton);
            poolables.Add(spritesTabButton);

            FluidToggleGroup showToggleGroup =
                FluidToggleGroup.Get()
                    .SetControlMode(FluidToggleGroup.ControlMode.OneToggleOn, animateChange: false);

            poolables.Add(showToggleGroup);

            animationTabButton.AddToToggleGroup(showToggleGroup);
            callbacksTabButton.AddToToggleGroup(showToggleGroup);
            spritesTabButton.AddToToggleGroup(showToggleGroup);

            FluidAnimatedContainer animationAnimatedContainer = new FluidAnimatedContainer().SetName("Animation").SetClearOnHide(true);
            FluidAnimatedContainer spritesAnimatedContainer = new FluidAnimatedContainer().SetName("Sprites").SetClearOnHide(true);

            disposables.Add(animationAnimatedContainer);
            disposables.Add(spritesAnimatedContainer);

            animationAnimatedContainer.OnShowCallback = () =>
            {
                animationAnimatedContainer.AddContent(GetAnimationContent(propertyAnimation, propertyAnimationEnabled));
                animationAnimatedContainer.Bind(property.serializedObject);
            };

            spritesAnimatedContainer.OnShowCallback = () =>
            {
                spritesAnimatedContainer.AddContent(GetSpritesContent(propertySprites, target));
                spritesAnimatedContainer.Bind(property.serializedObject);
            };

            tabsContainer
                .AddChild(animationTabContainer)
                .AddSpace(DesignUtils.k_Spacing * 4, 0)
                .AddChild(callbacksTabContainer)
                .AddSpace(DesignUtils.k_Spacing * 4, 0)
                .AddChild(spritesTabButton)
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
            FluidAnimatedContainer callbacksAnimatedContainer = new FluidAnimatedContainer().SetName("Callbacks").SetClearOnHide(true);
            disposables.Add(callbacksAnimatedContainer);

            callbacksAnimatedContainer.OnShowCallback = () =>
            {
                callbacksAnimatedContainer.AddContent
                (
                    new VisualElement()
                        .AddChild(onPlayCallbackFoldout)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(onStopCallbackFoldout)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(onFinishCallbackFoldout)
                        .AddChild(DesignUtils.endOfLineBlock)
                );

                callbacksAnimatedContainer.Bind(property.serializedObject);
            };

            callbacksAnimatedContainer.OnHideCallback =
                () =>
                {
                    if (onPlayCallbackFoldout.isOn) onPlayCallbackFoldout.SetIsOn(false, animateChange: true);
                    if (onStopCallbackFoldout.isOn) onStopCallbackFoldout.SetIsOn(false, animateChange: true);
                    if (onFinishCallbackFoldout.isOn) onFinishCallbackFoldout.SetIsOn(false, animateChange: true);
                };

            #endregion

            animationTabButton.SetOnValueChanged(evt => animationAnimatedContainer.Toggle(evt.newValue));
            spritesTabButton.SetOnValueChanged(evt => spritesAnimatedContainer.Toggle(evt.newValue));

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
                callbacksAnimatedContainer.Toggle(evt.newValue);
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
                .AddChild(callbacksAnimatedContainer)
                .AddChild(spritesAnimatedContainer);

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

        private static VisualElement GetSpritesContent(SerializedProperty arrayProperty, SpriteAnimation animation)
        {
            var animationInfo =
                new SpriteAnimationInfo(arrayProperty)
                    .SetStyleMarginTop(DesignUtils.k_Spacing);

            animationInfo.spriteSetter = sprite =>
            {
                if (sprite == null) return;
                if (animation == null) return;
                if (animation.spriteTarget == null) return;
                Component objectToUndo = animation.spriteTarget.GetComponent(animation.spriteTarget.targetType);
                Undo.RecordObject(objectToUndo, "Set Sprite");
                animation.spriteTarget.SetSprite(sprite);
                EditorUtility.SetDirty(objectToUndo);
            };

            var itemsSource = new List<SerializedProperty>();
            var fluidListView = new FluidListView();

            VisualElement content =
                new VisualElement()
                    .AddChild(fluidListView)
                    .AddChild(animationInfo)
                    .AddChild(DesignUtils.endOfLineBlock);
            content.Bind(arrayProperty.serializedObject);

            animationInfo.SetStyleDisplay(arrayProperty.arraySize > 0 ? DisplayStyle.Flex : DisplayStyle.None);

            content.RegisterCallback<AttachToPanelEvent>(evt =>
            {
                content.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
                content.RegisterCallback<DragPerformEvent>(OnDragPerformEvent);
            });

            content.RegisterCallback<DetachFromPanelEvent>(evy =>
            {
                content.UnregisterCallback<DragUpdatedEvent>(OnDragUpdate);
                content.UnregisterCallback<DragPerformEvent>(OnDragPerformEvent);
            });

            void OnDragUpdate(DragUpdatedEvent evt)
            {
                bool isValid = DragAndDrop.objectReferences.Any(item => item is Texture);
                if (!isValid) //check if it's a folder
                {
                    string assetPath = AssetDatabase.GetAssetPath(DragAndDrop.objectReferences[0]);
                    string[] paths = AssetDatabase.FindAssets($"t:{nameof(Texture)}", new[] { assetPath });
                    isValid = paths.Select(path => AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(path))).Any(sprite => sprite != null);
                }
                if (!isValid) return;
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            }

            void OnDragPerformEvent(DragPerformEvent evt)
            {
                var references = DragAndDrop.objectReferences.Where(item => item != null && item is Texture).OrderBy(item => item.name).ToList();
                if (references.Count == 0) //check if it's a folder
                {
                    string folderPath = AssetDatabase.GetAssetPath(DragAndDrop.objectReferences[0]);
                    string[] guids = AssetDatabase.FindAssets($"t:{nameof(Texture)}", new[] { folderPath });
                    references.AddRange(guids.Select(guid => AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid))).OrderBy(item => item.name));
                }

                if (references.Count == 0)
                    return;

                arrayProperty.ClearArray();
                foreach (Object reference in references)
                {
                    arrayProperty.InsertArrayElementAtIndex(arrayProperty.arraySize);
                    arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize - 1).objectReferenceValue = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GetAssetPath(reference));
                }

                arrayProperty.serializedObject.ApplyModifiedProperties();
                animation.UpdateAnimationSprites();
                animationInfo.Update();
            }

            fluidListView.listView.selectionType = SelectionType.None;
            fluidListView.listView.itemsSource = itemsSource;
            fluidListView.listView.makeItem = () => new ObjectFluidListViewItem(fluidListView, typeof(Sprite));
            fluidListView.listView.bindItem = (element, i) =>
            {
                var item = (ObjectFluidListViewItem)element;
                item.Update(i, itemsSource[i]);
                item.OnRemoveButtonClick += property =>
                {
                    int propertyIndex = 0;
                    for (int j = 0; j < arrayProperty.arraySize; j++)
                    {
                        if (property.propertyPath != arrayProperty.GetArrayElementAtIndex(j).propertyPath)
                            continue;
                        propertyIndex = j;
                        break;
                    }
                    arrayProperty.DeleteArrayElementAtIndex(propertyIndex);
                    arrayProperty.serializedObject.ApplyModifiedProperties();

                    UpdateItemsSource();
                };
            };

            #if UNITY_2021_2_OR_NEWER
            fluidListView.listView.fixedItemHeight = 24;
            fluidListView.SetPreferredListHeight((int)fluidListView.listView.fixedItemHeight * 6);
            #else
            fluidListView.listView.itemHeight = 24;
            fluidListView.SetPreferredListHeight(fluidListView.listView.itemHeight * 6);
            #endif

            fluidListView.SetDynamicListHeight(false);

            //ADD ITEM BUTTON (plus button)
            fluidListView.AddNewItemButtonCallback += () =>
            {
                arrayProperty.InsertArrayElementAtIndex(0);
                arrayProperty.GetArrayElementAtIndex(0).objectReferenceValue = null;
                arrayProperty.serializedObject.ApplyModifiedProperties();
                UpdateItemsSource();
            };

            var sortAzButton =
                FluidListView.Buttons.sortAzButton
                    .SetOnClick(() =>
                    {
                        Undo.RecordObject(arrayProperty.serializedObject.targetObject, "Sort Az");
                        animation.SortSpritesAz();
                        arrayProperty.serializedObject.UpdateIfRequiredOrScript();
                        arrayProperty.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                        UpdateItemsSource();
                        UpdateAnimationInfo();
                    });

            var sortZaButton =
                FluidListView.Buttons.sortZaButton
                    .SetOnClick(() =>
                    {
                        Undo.RecordObject(arrayProperty.serializedObject.targetObject, "Sort Za");
                        animation.SortSpritesZa();
                        arrayProperty.serializedObject.UpdateIfRequiredOrScript();
                        arrayProperty.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                        UpdateItemsSource();
                        UpdateAnimationInfo();
                    });

            var clearButton =
                FluidListView.Buttons.clearButton
                    .SetOnClick(() =>
                    {
                        arrayProperty.ClearArray();
                        arrayProperty.serializedObject.ApplyModifiedProperties();
                    });

            fluidListView.AddToolbarElement(sortAzButton);
            fluidListView.AddToolbarElement(sortZaButton);
            fluidListView.AddToolbarElement(DesignUtils.flexibleSpace);
            fluidListView.AddToolbarElement(clearButton);

            int arraySize = arrayProperty.arraySize;
            fluidListView.schedule.Execute(() =>
            {
                if (arrayProperty.arraySize == arraySize) return;
                arraySize = arrayProperty.arraySize;
                UpdateItemsSource();
                UpdateAnimationInfo();

            }).Every(100);

            void UpdateAnimationInfo()
            {
                if (arrayProperty.arraySize == 0)
                {
                    animationInfo.SetStyleDisplay(DisplayStyle.None);
                    return;
                }

                bool updateAnimationInfo = false;
                for (int i = 0; i < arrayProperty.arraySize; i++)
                {
                    if (arrayProperty.GetArrayElementAtIndex(i).objectReferenceValue == null)
                        continue;
                    updateAnimationInfo = true;
                    break;
                }

                if (!updateAnimationInfo)
                    return;

                animationInfo.SetStyleDisplay(DisplayStyle.Flex);
                animationInfo.Update();
            }

            void UpdateItemsSource()
            {
                itemsSource.Clear();

                for (int i = 0; i < arrayProperty.arraySize; i++)
                    itemsSource.Add(arrayProperty.GetArrayElementAtIndex(i));

                fluidListView?.Update();
            }

            UpdateItemsSource();
            return content;
        }

        private static VisualElement GetAnimationContent(SerializedProperty propertyAnimation, SerializedProperty propertyAnimationEnabled)
        {
            SerializedProperty propertyFromReferenceValue = propertyAnimation.FindPropertyRelative("FromReferenceValue");
            SerializedProperty propertyFromFrameOffset = propertyAnimation.FindPropertyRelative("FromFrameOffset");
            SerializedProperty propertyFromCustomValue = propertyAnimation.FindPropertyRelative("FromCustomValue");
            SerializedProperty propertyFromCustomProgress = propertyAnimation.FindPropertyRelative("FromCustomProgress");

            SerializedProperty propertyToReferenceValue = propertyAnimation.FindPropertyRelative("ToReferenceValue");
            SerializedProperty propertyToFrameOffset = propertyAnimation.FindPropertyRelative("ToFrameOffset");
            SerializedProperty propertyToCustomValue = propertyAnimation.FindPropertyRelative("ToCustomValue");
            SerializedProperty propertyToCustomProgress = propertyAnimation.FindPropertyRelative("ToCustomProgress");

            SerializedProperty propertySettings = propertyAnimation.FindPropertyRelative("Settings");

            var content = new VisualElement();
            content.SetEnabled(propertyAnimationEnabled.boolValue);
            var enableSwitch = NewEnableAnimationSwitch(string.Empty, selectableAccentColor, propertyAnimationEnabled, content);

            var fieldFromReferenceValue = new EnumField().SetBindingPath(propertyFromReferenceValue.propertyPath).ResetLayout();
            var fieldFromFrameOffset = FluidField.Get("Frame Offset", new IntegerField().SetBindingPath(propertyFromFrameOffset.propertyPath).ResetLayout());
            var fieldFromCustomValue = FluidField.Get("Custom Frame Number", new IntegerField().SetBindingPath(propertyFromCustomValue.propertyPath).ResetLayout());

            #region From Custom Progress

            var fromCustomProgressLabel = GetOffsetLabel(() => $"{(propertyFromCustomProgress.floatValue * 100).Round(0)}%");
            var fromCustomProgressSlider = new Slider(0f, 1f).SetBindingPath(propertyFromCustomProgress.propertyPath).ResetLayout().SetStyleFlexGrow(1);
            fromCustomProgressSlider.RegisterValueChangedCallback(evt => fromCustomProgressLabel.SetText($"{(evt.newValue * 100).Round(0)}%"));
            var fieldFromCustomProgress =
                GetOffsetField
                (
                    "Custom Progress",
                    fromCustomProgressLabel,
                    fromCustomProgressSlider,
                    () =>
                    {
                        propertyFromCustomProgress.floatValue = 0f;
                        propertyFromCustomProgress.serializedObject.ApplyModifiedProperties();
                    });

            #endregion

            var fieldFrom = FluidField.Get("From").AddFieldContent(fieldFromReferenceValue);

            var fieldToReferenceValue = new EnumField().SetBindingPath(propertyToReferenceValue.propertyPath).ResetLayout();
            var fieldToFrameOffset = FluidField.Get("Frame Offset", new IntegerField().SetBindingPath(propertyToFrameOffset.propertyPath).ResetLayout());
            var fieldToCustomValue = FluidField.Get("Custom Frame Number", new IntegerField().SetBindingPath(propertyToCustomValue.propertyPath).ResetLayout());

            #region To Custom Progress

            var toCustomProgressLabel = GetOffsetLabel(() => $"{(propertyToCustomProgress.floatValue * 100).Round(0)}%");
            var toCustomProgressSlider = new Slider(0f, 1f).SetBindingPath(propertyToCustomProgress.propertyPath).ResetLayout().SetStyleFlexGrow(1);
            toCustomProgressSlider.RegisterValueChangedCallback(evt => toCustomProgressLabel.SetText($"{(evt.newValue * 100).Round(0)}%"));
            var fieldToCustomProgress =
                GetOffsetField
                (
                    "Custom Progress",
                    toCustomProgressLabel,
                    toCustomProgressSlider,
                    () =>
                    {
                        propertyFromCustomProgress.floatValue = 1f;
                        propertyFromCustomProgress.serializedObject.ApplyModifiedProperties();
                    });

            #endregion

            var fieldTo = FluidField.Get("To").AddFieldContent(fieldToReferenceValue);

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
                                            .AddChild(fieldFromFrameOffset.SetStyleFlexGrow(1).SetStyleHeight(42).SetStyleMaxHeight(42))
                                            .AddChild(fieldFromCustomValue.SetStyleHeight(42).SetStyleFlexGrow(1).SetStyleMaxHeight(42))
                                            .AddChild(fieldFromCustomProgress.SetStyleHeight(42).SetStyleFlexGrow(1).SetStyleMaxHeight(42))
                                    )
                                    .AddSpace(DesignUtils.k_Spacing, 0)
                                    .AddChild
                                    (
                                        DesignUtils.flexibleSpace
                                            .AddChild(fieldTo.SetStyleFlexGrow(1))
                                            .AddSpace(0, DesignUtils.k_Spacing)
                                            .AddChild(fieldToFrameOffset.SetStyleFlexGrow(1).SetStyleHeight(42).SetStyleMaxHeight(42))
                                            .AddChild(fieldToCustomValue.SetStyleHeight(42).SetStyleFlexGrow(1).SetStyleMaxHeight(42))
                                            .AddChild(fieldToCustomProgress.SetStyleHeight(42).SetStyleFlexGrow(1).SetStyleMaxHeight(42))
                                    ))
                            .AddSpace(0, DesignUtils.k_Spacing)
                            .AddChild(new PropertyField().SetBindingPath(propertySettings.propertyPath))
                            .AddSpace(0, DesignUtils.k_EndOfLineSpacing)
                    );

            void Update()
            {
                var fromReferenceValue = (FrameReferenceValue)propertyFromReferenceValue.enumValueIndex;
                bool showFromOffset =
                    fromReferenceValue == FrameReferenceValue.FirstFrame ||
                    fromReferenceValue == FrameReferenceValue.LastFrame ||
                    fromReferenceValue == FrameReferenceValue.CurrentFrame;

                fieldFromFrameOffset.SetStyleDisplay(showFromOffset ? DisplayStyle.Flex : DisplayStyle.None);
                fieldFromCustomValue.SetStyleDisplay(fromReferenceValue == FrameReferenceValue.CustomFrame ? DisplayStyle.Flex : DisplayStyle.None);
                fieldFromCustomProgress.SetStyleDisplay(fromReferenceValue == FrameReferenceValue.CustomProgress ? DisplayStyle.Flex : DisplayStyle.None);

                var toReferenceValue = (FrameReferenceValue)propertyToReferenceValue.enumValueIndex;
                bool showToOffset =
                    toReferenceValue == FrameReferenceValue.FirstFrame ||
                    toReferenceValue == FrameReferenceValue.LastFrame ||
                    toReferenceValue == FrameReferenceValue.CurrentFrame;

                fieldToFrameOffset.SetStyleDisplay(showToOffset ? DisplayStyle.Flex : DisplayStyle.None);
                fieldToCustomValue.SetStyleDisplay(toReferenceValue == FrameReferenceValue.CustomFrame ? DisplayStyle.Flex : DisplayStyle.None);
                fieldToCustomProgress.SetStyleDisplay(toReferenceValue == FrameReferenceValue.CustomProgress ? DisplayStyle.Flex : DisplayStyle.None);
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
