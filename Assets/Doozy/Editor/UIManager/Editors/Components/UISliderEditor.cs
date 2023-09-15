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
    [CustomEditor(typeof(UISlider), true)]
    [CanEditMultipleObjects]
    public class UISliderEditor : UISelectableBaseEditor
    {
        public override Color accentColor => EditorColors.UIManager.UIComponent;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.UIComponent;

        public UISlider castedTarget => (UISlider)target;
        public IEnumerable<UISlider> castedTargets => targets.Cast<UISlider>();

        public static IEnumerable<Texture2D> unityEventIconTextures => EditorMicroAnimations.EditorUI.Icons.UnityEvent;
        public static IEnumerable<Texture2D> uiSliderIconTextures => EditorMicroAnimations.UIManager.Icons.UISlider;

        private EnabledIndicator callbacksTabIndicator { get; set; }

        private EnumField directionEnumField { get; set; }

        private FloatField maxValueFloatField { get; set; }
        private FloatField minValueFloatField { get; set; }
        private FloatField valueFloatField { get; set; }

        private FluidAnimatedContainer callbacksAnimatedContainer { get; set; }

        private FluidField directionField { get; set; }
        private FluidField fieldRectField { get; set; }
        private FluidField handleRectField { get; set; }
        private FluidField idField { get; set; }
        private FluidField maxValueField { get; set; }
        private FluidField minValueField { get; set; }
        private FluidField valueField { get; set; }
        private FluidField wholeNumbersField { get; set; }

        private FluidToggleCheckbox wholeNumbersToggle { get; set; }

        private ObjectField fieldRectObjectField { get; set; }
        private ObjectField handleRectObjectField { get; set; }

        private PropertyField behavioursPropertyField { get; set; }
        private PropertyField idPropertyField { get; set; }

        private Slider valueSlider { get; set; }

        private VisualElement callbacksTab { get; set; }

        private SerializedProperty propertyId { get; set; }
        private SerializedProperty propertyBehaviours { get; set; }
        private SerializedProperty propertyOnValueChanged { get; set; }
        private SerializedProperty propertyFillRect { get; set; }
        private SerializedProperty propertyHandleRect { get; set; }
        private SerializedProperty propertyDirection { get; set; }
        private SerializedProperty propertyMinValue { get; set; }
        private SerializedProperty propertyMaxValue { get; set; }
        private SerializedProperty propertyWholeNumbers { get; set; }
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
            fieldRectField?.Recycle();
            handleRectField?.Recycle();
            idField?.Recycle();
            maxValueField?.Recycle();
            minValueField?.Recycle();
            valueField?.Recycle();
            wholeNumbersField?.Recycle();

            wholeNumbersToggle?.Recycle();

        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyId = serializedObject.FindProperty(nameof(UISlider.Id));
            propertyBehaviours = serializedObject.FindProperty("Behaviours");
            propertyOnValueChanged = serializedObject.FindProperty(nameof(UISlider.OnValueChangedCallback));
            propertyFillRect = serializedObject.FindProperty("FillRect");
            propertyHandleRect = serializedObject.FindProperty("HandleRect");
            propertyDirection = serializedObject.FindProperty("Direction");
            propertyMinValue = serializedObject.FindProperty("MinValue");
            propertyMaxValue = serializedObject.FindProperty("MaxValue");
            propertyWholeNumbers = serializedObject.FindProperty("WholeNumbers");
            propertyValue = serializedObject.FindProperty("Value");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetAccentColor(accentColor)
                .SetComponentNameText((ObjectNames.NicifyVariableName(nameof(UISlider))))
                .SetIcon(uiSliderIconTextures.ToList())
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1046577351/UISlider?atlOrigin=eyJpIjoiNDM1Yzg5NzdjZDIwNGY1YmI0ZTNmZWQ2YjViYTg5YzEiLCJwIjoiYyJ9")
                .AddYouTubeButton();

            idPropertyField =
                DesignUtils.NewPropertyField(propertyId);

            idField =
                FluidField.Get()
                    .AddFieldContent(idPropertyField);

            behavioursPropertyField =
                DesignUtils.NewPropertyField(propertyBehaviours);

            fieldRectObjectField =
                DesignUtils.NewObjectField(propertyFillRect, typeof(RectTransform))
                    .SetStyleFlexGrow(1);

            fieldRectField =
                FluidField.Get()
                    .SetLabelText("Fill")
                    .AddFieldContent(fieldRectObjectField);

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

            minValueFloatField =
                DesignUtils.NewFloatField(propertyMinValue)
                    .SetStyleFlexGrow(1);

            minValueField =
                FluidField.Get()
                    .SetLabelText("Min Value")
                    .AddFieldContent(minValueFloatField);

            maxValueFloatField =
                DesignUtils.NewFloatField(propertyMaxValue)
                    .SetStyleFlexGrow(1);

            maxValueField =
                FluidField.Get()
                    .SetLabelText("Max Value")
                    .AddFieldContent(maxValueFloatField);

            wholeNumbersToggle =
                FluidToggleCheckbox.Get()
                    .SetToggleAccentColor(selectableAccentColor)
                    .BindToProperty(propertyWholeNumbers)
                    .SetStyleMargins(-2f, -2f, 0f, -4f);

            wholeNumbersField =
                FluidField.Get()
                    .SetLabelText("Whole Numbers")
                    .AddFieldContent(wholeNumbersToggle)
                    .SetStyleFlexGrow(0);

            valueSlider =
                new Slider()
                    .ResetLayout()
                    .SetStyleFlexGrow(1)
                    .BindToProperty(propertyValue);

            valueFloatField =
                DesignUtils.NewFloatField(propertyValue)
                    .SetStyleWidth(80);

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

            valueSlider.lowValue = propertyMinValue.floatValue;
            valueSlider.highValue = propertyMaxValue.floatValue;
            minValueFloatField.RegisterValueChangedCallback(evt => valueSlider.lowValue = evt.newValue);
            maxValueFloatField.RegisterValueChangedCallback(evt => valueSlider.highValue = evt.newValue);
            valueFloatField.RegisterValueChangedCallback(evt =>
            {
                if (castedTarget == null) return;
                if (castedTarget.wholeNumbers)
                    valueFloatField.value = Mathf.Round(valueFloatField.value);
                castedTarget.UpdateVisuals();
            });

            wholeNumbersToggle.SetOnValueChanged(evt =>
            {
                if (!evt.newValue) return;
                valueFloatField.value = Mathf.Round(valueFloatField.value);
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
                    foreach (UISlider slider in castedTargets)
                        slider.UpdateVisuals();
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
                foreach (UISlider slider in castedTargets)
                    slider.SetDirection(direction, true);
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
                    .AddChild(GetField(propertyOnValueChanged, uiSliderIconTextures, "Slider Value Changed", "Callbacks triggered then the slider value changed"))
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
                                nameof(UISlider)
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
                                        .AddChild(fieldRectField)
                                        .AddChild(DesignUtils.spaceBlock)
                                        .AddChild(handleRectField)
                                )
                                .AddChild(DesignUtils.spaceBlock2X)
                                .AddChild
                                (
                                    DesignUtils.row
                                        .AddChild(directionField)
                                        .AddChild(DesignUtils.spaceBlock)
                                        .AddChild(minValueField)
                                        .AddChild(DesignUtils.spaceBlock)
                                        .AddChild(maxValueField)
                                        .AddChild(DesignUtils.spaceBlock)
                                        .AddChild(wholeNumbersField)
                                )
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(valueField)
                                .AddChild(DesignUtils.spaceBlock2X)
                                .AddChild(idField)
                                .AddChild(DesignUtils.spaceBlock)
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
