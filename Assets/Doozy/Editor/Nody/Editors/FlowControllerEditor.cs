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
using Doozy.Runtime.Nody;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Input;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
// ReSharper disable UnusedMember.Local

namespace Doozy.Editor.Nody.Editors
{
    [CustomEditor(typeof(FlowController), true)]
    public class FlowControllerEditor : UnityEditor.Editor
    {
        private static IEnumerable<Texture2D> graphControllerIconTextures => EditorMicroAnimations.Nody.Icons.GraphController;
        private static IEnumerable<Texture2D> settingsIconTextures => EditorMicroAnimations.EditorUI.Icons.Settings;
        private static IEnumerable<Texture2D> unityIconTextures => EditorMicroAnimations.EditorUI.Icons.UnityEvent;

        private static Color accentColor => EditorColors.Nody.Color;
        private static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Nody.Color;

        private FlowController castedTarget => (FlowController)target;
        private IEnumerable<FlowController> castedTargets => targets.Cast<FlowController>();

        private bool hasOnStartCallback => castedTarget.onStart?.GetPersistentEventCount() > 0;
        private bool hasOnStopCallback => castedTarget.onStop?.GetPersistentEventCount() > 0;
        private bool hasCallbacks => hasOnStartCallback | hasOnStopCallback;

        private VisualElement root { get; set; }
        private FluidComponentHeader componentHeader { get; set; }

        private FluidToggleGroup toggleGroup { get; set; }
        private FluidToggleButtonTab settingsTabButton { get; set; }
        private FluidToggleButtonTab callbacksTabButton { get; set; }
        private EnabledIndicator callbacksTabIndicator { get; set; }
        private FluidAnimatedContainer callbacksAnimatedContainer { get; set; }
        private FluidAnimatedContainer settingsAnimatedContainer { get; set; }
        private VisualElement callbacksTab { get; set; }

        private ObjectField flowObjectField { get; set; }
        private EnumField flowTypeEnumField { get; set; }
        private ObjectField multiplayerInfoObjectField { get; set; }

        private FluidField flowField { get; set; }
        private FluidField flowTypeField { get; set; }
        private FluidToggleSwitch dontDestroyOnSceneChangeSwitch { get; set; }
        private FluidField multiplayerInfoField { get; set; }

        private SerializedProperty propertyFlow { get; set; }
        private SerializedProperty propertyFlowType { get; set; }
        private SerializedProperty propertyOnStart { get; set; }
        private SerializedProperty propertyOnStop { get; set; }
        private SerializedProperty propertyDontDestroyOnSceneChange { get; set; }
        private SerializedProperty propertyMultiplayerInfo { get; set; }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        private void OnDestroy()
        {
            componentHeader?.Recycle();

            toggleGroup?.Recycle();
            settingsTabButton?.Recycle();
            callbacksTabButton?.Recycle();
            callbacksTabIndicator?.Recycle();
            callbacksAnimatedContainer?.Dispose();
            settingsAnimatedContainer?.Dispose();

            flowField?.Recycle();
            flowTypeField?.Recycle();
            dontDestroyOnSceneChangeSwitch?.Recycle();
            multiplayerInfoField?.Recycle();
        }

        private void FindProperties()
        {
            propertyFlow = serializedObject.FindProperty("Flow");
            propertyFlowType = serializedObject.FindProperty("FlowType");
            propertyOnStart = serializedObject.FindProperty("OnStart");
            propertyOnStop = serializedObject.FindProperty("OnStop");
            propertyDontDestroyOnSceneChange = serializedObject.FindProperty("DontDestroyOnSceneChange");
            propertyMultiplayerInfo = serializedObject.FindProperty("MultiplayerInfo");
        }

        private void InitializeEditor()
        {
            FindProperties();

            root = new VisualElement();

            componentHeader =
                FluidComponentHeader.Get()
                    .SetElementSize(ElementSize.Large)
                    .SetAccentColor(accentColor)
                    .SetComponentNameText((ObjectNames.NicifyVariableName(nameof(FlowController))))
                    .SetIcon(graphControllerIconTextures.ToList())
                    .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048477732/Flow+Controller?atlOrigin=eyJpIjoiMzY3OGYxY2U4YTQ0NDI1Njk4MjVjNmVkMmI5ODAxZGEiLCJwIjoiYyJ9")
                    .AddYouTubeButton();

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

            flowObjectField =
                DesignUtils.NewObjectField(propertyFlow, typeof(FlowGraph))
                    .SetStyleFlexGrow(1);

            flowTypeEnumField =
                DesignUtils.NewEnumField(propertyFlowType)
                    .SetStyleWidth(60)
                    .SetStyleFlexShrink(0);

            multiplayerInfoObjectField =
                DesignUtils.NewObjectField(propertyMultiplayerInfo, typeof(MultiplayerInfo))
                    .SetStyleFlexGrow(1);

            dontDestroyOnSceneChangeSwitch =
                FluidToggleSwitch.Get()
                    .SetStyleMarginLeft(40)
                    .SetLabelText("Don't destroy controller on scene change")
                    .BindToProperty(propertyDontDestroyOnSceneChange);

            dontDestroyOnSceneChangeSwitch.SetEnabled(castedTarget.transform.parent == null);
            if (castedTarget.transform.parent != null && propertyDontDestroyOnSceneChange.boolValue)
            {
                propertyDontDestroyOnSceneChange.boolValue = false;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }

            flowField =
                FluidField.Get()
                    .SetLabelText("Flow Graph")
                    .AddFieldContent(flowObjectField);

            flowTypeField =
                FluidField.Get()
                    .SetStyleFlexGrow(0)
                    .SetLabelText("Flow Type")
                    .AddFieldContent(flowTypeEnumField);

            multiplayerInfoField =
                FluidField.Get()
                    .SetLabelText("Player Index")
                    .AddFieldContent(multiplayerInfoObjectField)
                    .SetStyleMarginTop(DesignUtils.k_Spacing2X)
                    .SetStyleDisplay(UIManagerInputSettings.instance.multiplayerMode ? DisplayStyle.Flex : DisplayStyle.None);

        }

        private void InitializeCallbacks()
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
                            .SetIcon(EditorMicroAnimations.EditorUI.Icons.UnityEvent)
                            .SetLabelText("Controller started controlling a flow graph")
                            .AddFieldContent(DesignUtils.NewPropertyField(propertyOnStart))
                    )
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild
                    (
                        FluidField.Get()
                            .SetElementSize(ElementSize.Large)
                            .SetIcon(EditorMicroAnimations.EditorUI.Icons.UnityEvent)
                            .SetLabelText("Controller stopped controlling a flow graph")
                            .AddFieldContent(DesignUtils.NewPropertyField(propertyOnStop))
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

        private void Compose()
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
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(callbacksAnimatedContainer)
                .AddChild
                (
                    settingsAnimatedContainer
                        .AddContent
                        (
                            DesignUtils.column
                                .AddChild(dontDestroyOnSceneChangeSwitch)
                                .AddChild(DesignUtils.spaceBlock2X)
                                .AddChild
                                (
                                    DesignUtils.row
                                        .AddChild(flowField)
                                        .AddChild(DesignUtils.spaceBlock)
                                        .AddChild(flowTypeField)
                                )
                                .AddChild(DesignUtils.spaceBlock)
                        )
                )
                .AddChild(DesignUtils.endOfLineBlock)
                ;
        }
    }
}
