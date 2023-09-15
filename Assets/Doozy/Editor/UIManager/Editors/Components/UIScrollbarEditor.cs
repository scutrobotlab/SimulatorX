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
using Doozy.Editor.UIElements;
using Doozy.Editor.UIManager.Editors.Components.Internal;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager;
using Doozy.Runtime.UIManager.Components;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UIElements.Slider;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Components
{
    [CustomEditor(typeof(UIScrollbar), true)]
    [CanEditMultipleObjects]
    public class UIScrollbarEditor : UISelectableBaseEditor
    {
        public override Color accentColor => EditorColors.UIManager.UIComponent;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.UIComponent;

        public UIScrollbar castedTarget => (UIScrollbar)target;
        public IEnumerable<UIScrollbar> castedTargets => targets.Cast<UIScrollbar>();

        public static IEnumerable<Texture2D> unityEventIconTextures => EditorMicroAnimations.EditorUI.Icons.UnityEvent;
        public static IEnumerable<Texture2D> uiScrollbarIconTextures => EditorMicroAnimations.UIManager.Icons.UIScrollbar;

        private EnabledIndicator callbacksTabIndicator { get; set; }

        private EnumField directionEnumField { get; set; }

        private IntegerField numberOfStepsIntegerField { get; set; }
        private FloatField sizeFloatField { get; set; }
        private FloatField valueFloatField { get; set; }

        private FluidAnimatedContainer callbacksAnimatedContainer { get; set; }

        private FluidField directionField { get; set; }
        private FluidField handleRectField { get; set; }
        private FluidField numberOfStepsField { get; set; }
        private FluidField sizeField { get; set; }
        private FluidField valueField { get; set; }

        private ObjectField handleRectObjectField { get; set; }

        private PropertyField behavioursPropertyField { get; set; }

        private SliderInt numberOfStepsSliderInt { get; set; }
        private Slider sizeSlider { get; set; }
        private Slider valueSlider { get; set; }

        private VisualElement callbacksTab { get; set; }

        private SerializedProperty propertyBehaviours { get; set; }
        private SerializedProperty propertyOnValueChanged { get; set; }
        private SerializedProperty propertyHandleRect { get; set; }
        private SerializedProperty propertyDirection { get; set; }
        private SerializedProperty propertyNumberOfSteps { get; set; }
        private SerializedProperty propertySize { get; set; }
        private SerializedProperty propertyValue { get; set; }

        private bool hasOnValueChangedCallback =>
            castedTarget != null && castedTarget.OnValueChangedCallback?.GetPersistentEventCount() > 0;

        private bool hasCallbacks =>
            hasOnValueChangedCallback;

        protected override void OnDestroy()
        {
            base.OnDestroy();

            callbacksTabIndicator?.Recycle();

            callbacksAnimatedContainer?.Dispose();

            directionField?.Recycle();
            handleRectField?.Recycle();
            numberOfStepsField?.Recycle();
            sizeField?.Recycle();
            valueField?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyBehaviours = serializedObject.FindProperty("Behaviours");
            propertyOnValueChanged = serializedObject.FindProperty(nameof(UIScrollbar.OnValueChangedCallback));
            propertyHandleRect = serializedObject.FindProperty("HandleRect");
            propertyDirection = serializedObject.FindProperty("Direction");
            propertyNumberOfSteps = serializedObject.FindProperty("NumberOfSteps");
            propertySize = serializedObject.FindProperty("Size");
            propertyValue = serializedObject.FindProperty("Value");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetAccentColor(accentColor)
                .SetComponentNameText((ObjectNames.NicifyVariableName(nameof(UIScrollbar))))
                .SetIcon(uiScrollbarIconTextures.ToList())
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048936453/UIScrollbar?atlOrigin=eyJpIjoiZGVkNTE4NjRkMjA3NGZjY2FjYTVjNzhhMzE4ZDRkMmMiLCJwIjoiYyJ9")
                .AddYouTubeButton();

            behavioursPropertyField =
                DesignUtils.NewPropertyField(propertyBehaviours);

            handleRectObjectField =
                DesignUtils.NewObjectField(propertyHandleRect, typeof(RectTransform))
                    .SetStyleFlexGrow(1);

            handleRectField =
                FluidField.Get()
                    .SetLabelText("Handle")
                    .AddFieldContent(handleRectObjectField);

            directionEnumField =
                DesignUtils.NewEnumField(propertyDirection)
                    .SetStyleWidth(120);

            directionField =
                FluidField.Get()
                    .SetLabelText("Direction")
                    .AddFieldContent(directionEnumField)
                    .SetStyleFlexGrow(0);

            numberOfStepsSliderInt =
                new SliderInt()
                    .ResetLayout()
                    .SetStyleFlexGrow(1)
                    .BindToProperty(propertyNumberOfSteps);

            numberOfStepsIntegerField =
                DesignUtils.NewIntegerField(propertyNumberOfSteps)
                    .SetStyleWidth(30);

            numberOfStepsField =
                FluidField.Get()
                    .SetLabelText("Number of Steps")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(numberOfStepsSliderInt)
                            .AddChild(DesignUtils.spaceBlock2X)
                            .AddChild(numberOfStepsIntegerField)
                    );

            sizeSlider =
                new Slider()
                    .ResetLayout()
                    .SetStyleFlexGrow(1)
                    .BindToProperty(propertySize);

            sizeFloatField =
                DesignUtils.NewFloatField(propertySize)
                    .SetStyleWidth(60);

            sizeField =
                FluidField.Get()
                    .SetLabelText("Size")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(sizeSlider)
                            .AddChild(DesignUtils.spaceBlock2X)
                            .AddChild(sizeFloatField)
                    );

            valueSlider =
                new Slider()
                    .ResetLayout()
                    .SetStyleFlexGrow(1)
                    .BindToProperty(propertyValue);

            valueFloatField =
                DesignUtils.NewFloatField(propertyValue)
                    .SetStyleWidth(60);

            valueField =
                FluidField.Get()
                    .SetLabelText("Value")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(valueSlider)
                            .AddChild(DesignUtils.spaceBlock2X)
                            .AddChild(valueFloatField)
                    );

            InitializeCallbacks();

            numberOfStepsSliderInt.lowValue = UIScrollbar.k_MINNumberOfSteps;
            numberOfStepsSliderInt.highValue = UIScrollbar.k_MAXNumberOfSteps;
            numberOfStepsIntegerField.RegisterValueChangedCallback(evt =>
            {
                if (castedTarget == null) return;
                numberOfStepsIntegerField.value = numberOfStepsIntegerField.value.Clamp(UIScrollbar.k_MINNumberOfSteps, UIScrollbar.k_MAXNumberOfSteps);
                castedTarget.UpdateVisuals();
            });

            sizeSlider.lowValue = UIScrollbar.k_MINValue;
            sizeSlider.highValue = UIScrollbar.k_MAXValue;
            sizeFloatField.RegisterValueChangedCallback(evt =>
            {
                if (castedTarget == null) return;
                sizeFloatField.value = sizeFloatField.value.Clamp01().Round(3);
                castedTarget.UpdateVisuals();
            });

            valueSlider.lowValue = UIScrollbar.k_MINValue;
            valueSlider.highValue = UIScrollbar.k_MAXValue;
            valueFloatField.RegisterValueChangedCallback(evt =>
            {
                if (castedTarget == null) return;
                int steps = propertyNumberOfSteps.intValue;
                float newValue = evt.newValue;
                newValue = steps > 0 ? Mathf.Round(newValue * (steps - 1)) / (steps - 1) : newValue;
                newValue = newValue.Clamp01().Round(3);
                if (!propertyValue.floatValue.Approximately(newValue))
                {
                    propertyValue.floatValue = newValue;
                    serializedObject.ApplyModifiedPropertiesWithoutUndo();
                }
                castedTarget.UpdateVisuals();
            });

            directionEnumField.RegisterValueChangedCallback(evt =>
            {
                if (evt?.newValue == null) return;
                UpdateDirection((SlideDirection)evt.newValue);
            });

            root.schedule.Execute(() =>
            {
                if (castedTarget == null) return;
                if (serializedObject.isEditingMultipleObjects)
                {
                    foreach (UIScrollbar scrollbar in castedTargets)
                        scrollbar.UpdateVisuals();
                    return;
                }
                castedTarget.UpdateVisuals();
            }).Every(100);

            root.schedule.Execute(() =>
            {
                UpdateDirection((SlideDirection)propertyDirection.enumValueIndex);
            });
        }

        private void UpdateDirection(SlideDirection direction)
        {
            if (serializedObject.isEditingMultipleObjects)
            {
                foreach (UIScrollbar scrollbar in castedTargets)
                    scrollbar.SetDirection(direction, true);
                return;
            }

            castedTarget.SetDirection(direction, true);
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
                    .AddChild(GetField(propertyOnValueChanged, uiScrollbarIconTextures, "Scrollbar Value Changed", "Callbacks triggered then the scrollbar value changed"))
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
                            DesignUtils.SystemButton_SortComponents
                            (
                                castedTarget.gameObject,
                                nameof(RectTransform),
                                nameof(Canvas),
                                nameof(CanvasGroup),
                                nameof(GraphicRaycaster),
                                nameof(UIScrollbar)
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
                                .AddChild
                                (
                                    DesignUtils.row
                                        .AddChild(handleRectField)
                                )
                                .AddChild(DesignUtils.spaceBlock2X)
                                .AddChild
                                (
                                    DesignUtils.row
                                        .AddChild(directionField)
                                        .AddChild(DesignUtils.spaceBlock)
                                        .AddChild(numberOfStepsField)
                                        .AddChild(DesignUtils.spaceBlock)
                                        .AddChild(sizeField)
                                )
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(valueField)
                                .AddChild(DesignUtils.spaceBlock2X)
                                .AddChild(behavioursPropertyField)
                                .AddChild(DesignUtils.endOfLineBlock)
                        )
                )
                .AddChild(statesAnimatedContainer)
                .AddChild(navigationAnimatedContainer)
                .AddChild(callbacksAnimatedContainer);
        }
    }
}
