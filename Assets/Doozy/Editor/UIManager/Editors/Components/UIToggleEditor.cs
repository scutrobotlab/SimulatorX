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
    [CustomEditor(typeof(UIToggle), true)]
    [CanEditMultipleObjects]
    public class UIToggleEditor : UISelectableBaseEditor
    {
        public override Color accentColor => EditorColors.UIManager.UIComponent;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.UIComponent;

        public UIToggle castedTarget => (UIToggle)target;
        public IEnumerable<UIToggle> castedTargets => targets.Cast<UIToggle>();

        public static IEnumerable<Texture2D> toggleIconTextures => EditorMicroAnimations.UIManager.Icons.UIToggleCheckbox;
        public static IEnumerable<Texture2D> toggleGroupIconTextures => EditorMicroAnimations.UIManager.Icons.UIToggleGroup;
        public static IEnumerable<Texture2D> unityEventIconTextures => EditorMicroAnimations.EditorUI.Icons.UnityEvent;
        public static IEnumerable<Texture2D> buttonClickIconTextures => EditorMicroAnimations.EditorUI.Icons.ButtonClick;
        public static IEnumerable<Texture2D> toggleOnIconTextures => EditorMicroAnimations.EditorUI.Icons.ToggleON;
        public static IEnumerable<Texture2D> toggleOffIconTextures => EditorMicroAnimations.EditorUI.Icons.ToggleOFF;

        private VisualElement callbacksTab { get; set; }
        private EnabledIndicator callbacksTabIndicator { get; set; }

        private FluidAnimatedContainer callbacksAnimatedContainer { get; set; }

        private FluidField idField { get; set; }
        private FluidField toggleGroupField { get; set; }

        private ObjectField toggleGroupObjectField { get; set; }
        private FluidToggleCheckbox isOnCheckbox { get; set; }

        private SerializedProperty propertyId { get; set; }
        private SerializedProperty propertyBehaviours { get; set; }
        private SerializedProperty propertyIsOn { get; set; }
        private SerializedProperty propertyToggleGroup { get; set; }
        private SerializedProperty propertyOnToggleOnCallback { get; set; }
        private SerializedProperty propertyOnInstantToggleOnCallback { get; set; }
        private SerializedProperty propertyOnToggleOffCallback { get; set; }
        private SerializedProperty propertyOnInstantToggleOffCallback { get; set; }
        private SerializedProperty propertyOnValueChanged { get; set; }

        private bool hasOnToggleOnCallback => castedTarget != null && castedTarget.OnToggleOnCallback is { Enabled: true } && castedTarget.OnToggleOnCallback.hasEvents | castedTarget.OnToggleOnCallback.hasRunners;
        private bool hasOnInstantToggleOnCallback => castedTarget != null && castedTarget.OnInstantToggleOnCallback is { Enabled: true } && castedTarget.OnInstantToggleOnCallback.hasEvents | castedTarget.OnInstantToggleOnCallback.hasRunners;
        private bool hasOnToggleOffCallback => castedTarget != null && castedTarget.OnToggleOffCallback is { Enabled: true } && castedTarget.OnToggleOffCallback.hasEvents | castedTarget.OnToggleOffCallback.hasRunners;
        private bool hasOnInstantToggleOffCallback => castedTarget != null && castedTarget.OnInstantToggleOffCallback is { Enabled: true } && castedTarget.OnInstantToggleOffCallback.hasEvents | castedTarget.OnInstantToggleOffCallback.hasRunners;
        private bool hasOnValueChangedCallback => castedTarget != null && castedTarget.OnValueChangedCallback?.GetPersistentEventCount() > 0;
        private bool hasCallbacks =>
            hasOnToggleOnCallback |
            hasOnInstantToggleOnCallback |
            hasOnToggleOffCallback |
            hasOnInstantToggleOffCallback |
            hasOnValueChangedCallback;

        protected override void OnDestroy()
        {
            base.OnDestroy();

            callbacksTabIndicator?.Recycle();

            idField?.Recycle();
            toggleGroupField?.Recycle();

            isOnCheckbox?.Recycle();

            callbacksAnimatedContainer?.Dispose();
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyId = serializedObject.FindProperty(nameof(UIToggle.Id));
            propertyBehaviours = serializedObject.FindProperty("Behaviours");
            propertyIsOn = serializedObject.FindProperty("IsOn");
            propertyToggleGroup = serializedObject.FindProperty("ToggleGroup");
            propertyOnToggleOnCallback = serializedObject.FindProperty(nameof(UIToggle.OnToggleOnCallback));
            propertyOnInstantToggleOnCallback = serializedObject.FindProperty(nameof(UIToggle.OnInstantToggleOnCallback));
            propertyOnToggleOffCallback = serializedObject.FindProperty(nameof(UIToggle.OnToggleOffCallback));
            propertyOnInstantToggleOffCallback = serializedObject.FindProperty(nameof(UIToggle.OnInstantToggleOffCallback));
            propertyOnValueChanged = serializedObject.FindProperty(nameof(UIToggle.OnValueChangedCallback));
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetAccentColor(accentColor)
                .SetComponentNameText((ObjectNames.NicifyVariableName(nameof(UIToggle))))
                .SetIcon(toggleIconTextures.ToList())
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048084542/UIToggle?atlOrigin=eyJpIjoiNjQ4NmRmNjIyNjY2NDM5YmEyOTBlMzhhZjFmZWI0ZDciLCJwIjoiYyJ9")
                .AddYouTubeButton();


            idField = FluidField.Get().AddFieldContent(DesignUtils.NewPropertyField(propertyId));

            isOnCheckbox =
                FluidToggleCheckbox.Get()
                    .SetLabelText("Is On")
                    .BindToProperty(propertyIsOn);

            toggleGroupObjectField =
                DesignUtils.NewObjectField(propertyToggleGroup, typeof(UIToggleGroup))
                    .SetStyleFlexGrow(1);

            toggleGroupField =
                FluidField.Get()
                    .SetLabelText("Toggle Group")
                    .SetIcon(toggleGroupIconTextures)
                    .AddFieldContent(toggleGroupObjectField);

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
                    .AddChild(GetField(propertyOnValueChanged, toggleIconTextures, "Toggle Value Changed", "Callbacks triggered then the toggle value changed"))
                    .AddChild(DesignUtils.endOfLineBlock);

                callbacksAnimatedContainer.schedule.Execute
                (
                    () =>
                        callbacksAnimatedContainer.Bind(serializedObject)
                );
            });

            callbacksAnimatedContainer.SetOnHideCallback(() =>
            {
                serializedObject.ApplyModifiedProperties();
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
                                () => $"Toggle - {castedTarget.Id.Category} {castedTarget.Id.Name}"
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
                                nameof(UIToggle)
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
                                .AddChild(isOnCheckbox)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(toggleGroupField)
                                .AddChild(DesignUtils.spaceBlock2X)
                                .AddChild(idField)
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
