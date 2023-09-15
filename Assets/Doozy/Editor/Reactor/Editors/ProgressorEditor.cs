// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Events;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Components;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Reactor.Editors
{
    [CustomEditor(typeof(Progressor), true)]
    public class ProgressorEditor : UnityEditor.Editor
    {
        public Progressor castedTarget => (Progressor)target;
        public IEnumerable<Progressor> castedTargets => targets.Cast<Progressor>();

        public static Color accentColor => EditorColors.Reactor.Red;
        public static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Reactor.Red;

        private static IEnumerable<Texture2D> unityIconTextures => EditorMicroAnimations.EditorUI.Icons.UnityEvent;
        private static IEnumerable<Texture2D> behaviourIconTextures => EditorMicroAnimations.EditorUI.Icons.UIBehaviour;
        private static IEnumerable<Texture2D> settingsIconTextures => EditorMicroAnimations.EditorUI.Icons.Settings;
        private static IEnumerable<Texture2D> progressorIconTextures => EditorMicroAnimations.Reactor.Icons.Progressor;

        private bool hasOnValueChangedCallback => castedTarget != null && castedTarget.OnValueChanged?.GetPersistentEventCount() > 0;
        private bool hasOnProgressChangedCallback => castedTarget != null && castedTarget.OnProgressChanged?.GetPersistentEventCount() > 0;
        private bool hasCallbacks => hasOnValueChangedCallback | hasOnProgressChangedCallback;

        private VisualElement root { get; set; }
        private FluidComponentHeader componentHeader { get; set; }
        private ReactionControls controls { get; set; }

        private FluidToggleGroup tabsGroup { get; set; }
        private FluidToggleButtonTab settingsTabButton { get; set; }
        private FluidToggleButtonTab callbacksTabButton { get; set; }

        private VisualElement callbacksTab { get; set; }
        private EnabledIndicator callbacksTabIndicator { get; set; }

        private FluidAnimatedContainer settingsAnimatedContainer { get; set; }
        private FluidAnimatedContainer callbacksAnimatedContainer { get; set; }

        public FluidListView progressTargetsFluidListView { get; private set; }
        private SerializedProperty progressTargetsArrayProperty { get; set; }
        private List<SerializedProperty> progressTargetsItemsSource { get; set; }

        private SerializedProperty propertyProgressTargets { get; set; }
        private SerializedProperty propertyFromValue { get; set; }
        private SerializedProperty propertyToValue { get; set; }
        private SerializedProperty propertyCustomResetValue { get; set; }
        private SerializedProperty propertyReaction { get; set; }
        private SerializedProperty propertyResetValueOnEnable { get; set; }
        private SerializedProperty propertyResetValueOnDisable { get; set; }
        private SerializedProperty propertyOnValueChanged { get; set; }
        private SerializedProperty propertyOnProgressChanged { get; set; }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        private void OnDestroy()
        {
            componentHeader?.Recycle();

        }

        private void FindProperties()
        {
            propertyProgressTargets = serializedObject.FindProperty("ProgressTargets");
            propertyFromValue = serializedObject.FindProperty("FromValue");
            propertyToValue = serializedObject.FindProperty("ToValue");
            propertyCustomResetValue = serializedObject.FindProperty("CustomResetValue");
            propertyReaction = serializedObject.FindProperty("Reaction");
            propertyResetValueOnEnable = serializedObject.FindProperty("ResetValueOnEnable");
            propertyResetValueOnDisable = serializedObject.FindProperty("ResetValueOnDisable");
            propertyOnValueChanged = serializedObject.FindProperty("OnValueChanged");
            propertyOnProgressChanged = serializedObject.FindProperty("OnProgressChanged");
        }

        private void InitializeEditor()
        {
            FindProperties();

            root = new VisualElement();

            componentHeader = FluidComponentHeader.Get()
                .SetAccentColor(accentColor)
                .SetComponentNameText(nameof(Progressor))
                .SetIcon(progressorIconTextures.ToList())
                .SetElementSize(ElementSize.Large)
                .AddManualButton()
                .AddYouTubeButton();

            #region Controls

            controls =
                new ReactionControls()
                    .SetStyleDisplay(EditorApplication.isPlaying ? DisplayStyle.Flex : DisplayStyle.None)
                    .SetFirstFrameButtonCallback(() =>
                    {
                        if (serializedObject.isEditingMultipleObjects)
                        {
                            foreach (Progressor progressor in castedTargets)
                                progressor.SetProgressAtZero();
                            return;
                        }
                        castedTarget.SetProgressAtZero();
                    })
                    .SetPlayForwardButtonCallback(() =>
                    {
                        if (serializedObject.isEditingMultipleObjects)
                        {
                            foreach (Progressor progressor in castedTargets)
                                progressor.Play(PlayDirection.Forward);
                            return;
                        }
                        castedTarget.Play(PlayDirection.Forward);
                    })
                    .SetStopButtonCallback(() =>
                    {
                        if (serializedObject.isEditingMultipleObjects)
                        {
                            foreach (Progressor progressor in castedTargets)
                                progressor.Stop();
                            return;
                        }
                        castedTarget.Stop();
                    })
                    .SetPlayReverseButtonCallback(() =>
                    {
                        if (serializedObject.isEditingMultipleObjects)
                        {
                            foreach (Progressor progressor in castedTargets)
                                progressor.Play(PlayDirection.Reverse);
                            return;
                        }
                        castedTarget.Play(PlayDirection.Reverse);
                    })
                    .SetReverseButtonCallback(() =>
                    {
                        if (serializedObject.isEditingMultipleObjects)
                        {
                            foreach (Progressor progressor in castedTargets)
                                progressor.Reverse();
                            return;
                        }
                        castedTarget.Reverse();
                    })
                    .SetLastFrameButtonCallback(() =>
                    {
                        if (serializedObject.isEditingMultipleObjects)
                        {
                            foreach (Progressor progressor in castedTargets)
                                progressor.SetProgressAtOne();
                            return;
                        }
                        castedTarget.SetProgressAtOne();
                    });

            #endregion

            settingsAnimatedContainer = new FluidAnimatedContainer().SetName("Settings").SetClearOnHide(false).Show(false);
            callbacksAnimatedContainer = new FluidAnimatedContainer().SetName("Callbacks").SetClearOnHide(false).Hide(false);

            tabsGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.OneToggleOnEnforced);

            settingsTabButton =
                DesignUtils.GetTabButtonForComponentSection(settingsIconTextures)
                    .SetLabelText("Settings")
                    .SetIsOn(true, false)
                    .SetOnValueChanged(evt => settingsAnimatedContainer.Toggle(evt.newValue));

            settingsTabButton.AddToToggleGroup(tabsGroup);

            InitializeSettings();
            InitializeCallbacks();
            InitializeProgressTargetsListView();
        }

        private void InitializeSettings()
        {
            FluidField fromValueFluidField = FluidField.Get<FloatField>("FromValue", "From Value");
            FluidField toValueFluidField = FluidField.Get<FloatField>("ToValue", "To Value");
            EnumField resetValueOnEnableEnumField = DesignUtils.NewEnumField(propertyResetValueOnEnable).SetStyleWidth(120);
            FluidField resetValueOnEnableFluidField = FluidField.Get("OnEnable reset value to").AddFieldContent(resetValueOnEnableEnumField).SetStyleFlexGrow(0);
            FluidField customResetValueFluidField = FluidField.Get<FloatField>("CustomResetValue", "Custom Reset Value");
            FluidField reactionFluidField = FluidField.Get<PropertyField>("Reaction.Settings");
            resetValueOnEnableEnumField.RegisterValueChangedCallback(evt =>
            {
                if (evt?.newValue == null) return;
                customResetValueFluidField.SetEnabled((ResetValue)evt.newValue == ResetValue.CustomValue);
            });
            root.schedule.Execute(() =>
            {
                if (customResetValueFluidField == null) return;
                if (resetValueOnEnableEnumField?.value == null) return;
                customResetValueFluidField.SetEnabled((ResetValue)resetValueOnEnableEnumField.value == ResetValue.CustomValue);
            });


            settingsAnimatedContainer
                .fluidContainer
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(fromValueFluidField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(toValueFluidField)
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(reactionFluidField)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(resetValueOnEnableFluidField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(customResetValueFluidField)
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(DesignUtils.endOfLineBlock)
                ;
        }

        private void InitializeCallbacks()
        {
            (callbacksTabButton, callbacksTabIndicator, callbacksTab) =
                DesignUtils.GetTabButtonForComponentSectionWithEnabledIndicator(unityIconTextures, DesignUtils.callbackSelectableColor, DesignUtils.callbacksColor);

            callbacksAnimatedContainer
                .fluidContainer
                .AddChild
                (
                    FluidField.Get()
                        .SetElementSize(ElementSize.Large)
                        .SetIcon(EditorMicroAnimations.EditorUI.Icons.UnityEvent)
                        .SetLabelText("Value Changed - [From, To]")
                        .AddFieldContent(DesignUtils.NewPropertyField(propertyOnValueChanged))
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    FluidField.Get()
                        .SetElementSize(ElementSize.Large)
                        .SetIcon(EditorMicroAnimations.EditorUI.Icons.UnityEvent)
                        .SetLabelText("Progress Changed - [0, 1]")
                        .AddFieldContent(DesignUtils.NewPropertyField(propertyOnProgressChanged))
                )
                .AddChild(DesignUtils.endOfLineBlock);

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
                .AddToToggleGroup(tabsGroup);
        }

        private void InitializeProgressTargetsListView()
        {
            progressTargetsArrayProperty = serializedObject.FindProperty("ProgressTargets");
            progressTargetsFluidListView = new FluidListView();
            progressTargetsFluidListView.SetListTitle("Progress Targets");
            progressTargetsFluidListView.SetListDescription("Targets controlled by this Progressor");
            progressTargetsFluidListView.listView.selectionType = SelectionType.None;
            progressTargetsItemsSource = new List<SerializedProperty>();

            progressTargetsFluidListView.listView.itemsSource = progressTargetsItemsSource;
            progressTargetsFluidListView.listView.makeItem = () => new ObjectFluidListViewItem(progressTargetsFluidListView, typeof(ProgressTarget));

            progressTargetsFluidListView.listView.bindItem = (element, i) =>
            {
                var item = (ObjectFluidListViewItem)element;
                item.Update(i, progressTargetsItemsSource[i]);
                item.OnRemoveButtonClick += itemProperty =>
                {
                    int propertyIndex = 0;
                    for (int j = 0; j < progressTargetsArrayProperty.arraySize; j++)
                    {
                        if (itemProperty.propertyPath != progressTargetsArrayProperty.GetArrayElementAtIndex(j).propertyPath)
                            continue;
                        propertyIndex = j;
                        break;
                    }
                    progressTargetsArrayProperty.DeleteArrayElementAtIndex(propertyIndex);
                    progressTargetsArrayProperty.serializedObject.ApplyModifiedProperties();

                    UpdateItemsSource();
                };
                // item.schedule.Execute(() => item.propertyField.Children().First().SetStyleDisplay(DisplayStyle.None));
            };
            #if UNITY_2021_2_OR_NEWER
            progressTargetsFluidListView.listView.fixedItemHeight = 30;
            progressTargetsFluidListView.SetPreferredListHeight((int)progressTargetsFluidListView.listView.fixedItemHeight * 6);
            #else
            progressTargetsFluidListView.listView.itemHeight = 30;
            progressTargetsFluidListView.SetPreferredListHeight(progressTargetsFluidListView.listView.itemHeight * 6);
            #endif
            progressTargetsFluidListView.SetDynamicListHeight(false);
            progressTargetsFluidListView.HideFooterWhenEmpty(true);
            progressTargetsFluidListView.UseSmallEmptyListPlaceholder(true);
            progressTargetsFluidListView.emptyListPlaceholder.SetIcon(EditorMicroAnimations.EditorUI.Placeholders.EmptyListViewSmall);

            //ADD ITEM BUTTON (plus button)
            progressTargetsFluidListView.AddNewItemButtonCallback += () =>
            {
                progressTargetsArrayProperty.InsertArrayElementAtIndex(0);
                progressTargetsArrayProperty.GetArrayElementAtIndex(0).objectReferenceValue = null;
                progressTargetsArrayProperty.serializedObject.ApplyModifiedProperties();
                UpdateItemsSource();
            };

            UpdateItemsSource();

            int arraySize = progressTargetsArrayProperty.arraySize;
            progressTargetsFluidListView.schedule.Execute(() =>
            {
                if (progressTargetsArrayProperty == null) return;
                if (progressTargetsArrayProperty.arraySize == arraySize) return;
                arraySize = progressTargetsArrayProperty.arraySize;
                UpdateItemsSource();

            }).Every(100);

        }

        private void UpdateItemsSource()
        {
            progressTargetsItemsSource.Clear();
            for (int i = 0; i < progressTargetsArrayProperty.arraySize; i++)
                progressTargetsItemsSource.Add(progressTargetsArrayProperty.GetArrayElementAtIndex(i));

            progressTargetsFluidListView?.Update();
        }

        private void Compose()
        {
            root.AddChild(componentHeader)
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
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(controls)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(settingsAnimatedContainer)
                .AddChild(callbacksAnimatedContainer)
                .AddChild(progressTargetsFluidListView)
                .AddChild(DesignUtils.endOfLineBlock)
                ;
        }
    }
}
