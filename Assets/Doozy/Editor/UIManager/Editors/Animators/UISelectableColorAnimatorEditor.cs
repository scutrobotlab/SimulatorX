// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Components;
using Doozy.Editor.UIManager.Editors.Animators.Internal;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager;
using Doozy.Runtime.UIManager.Animators;
using Doozy.Runtime.UIManager.Components;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Animators
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UISelectableColorAnimator), true)]
    public class UISelectableColorAnimatorEditor : BaseUISelectableAnimatorEditor
    {
        public UISelectableColorAnimator castedTarget => (UISelectableColorAnimator)target;
        public IEnumerable<UISelectableColorAnimator> castedTargets => targets.Cast<UISelectableColorAnimator>();

        public static IEnumerable<Texture2D> colorAnimatorIconTextures => EditorMicroAnimations.Reactor.Icons.ColorAnimator;
        public static IEnumerable<Texture2D> colorAnimationIconTextures => EditorMicroAnimations.Reactor.Icons.ColorAnimation;
        public static IEnumerable<Texture2D> colorTargetIconTextures => EditorMicroAnimations.Reactor.Icons.ColorTarget;

        private ObjectField colorTargetObjectField { get; set; }
        private FluidField colorTargetFluidField { get; set; }
        private SerializedProperty propertyColorTarget { get; set; }
        private IVisualElementScheduledItem targetFinder { get; set; }

        private ColorAnimationTab normalTab { get; set; }
        private ColorAnimationTab highlightedTab { get; set; }
        private ColorAnimationTab pressedTab { get; set; }
        private ColorAnimationTab selectedTab { get; set; }
        private ColorAnimationTab disabledTab { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            normalTab?.Recycle();
            highlightedTab?.Recycle();
            pressedTab?.Recycle();
            selectedTab?.Recycle();
            disabledTab?.Recycle();

            colorTargetFluidField?.Recycle();
            controllerField?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();
            propertyColorTarget = serializedObject.FindProperty("ColorTarget");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentTypeText("Color Animator")
                .SetIcon(colorAnimatorIconTextures.ToList())
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1046577372/UISelectable+Color+Animator?atlOrigin=eyJpIjoiODkxNmI1NTFiMWI5NGNmMDg1MTczZDQxYzU1ZmE4OTgiLCJwIjoiYyJ9")
                .AddYouTubeButton();

            InitializeColorTarget();

            normalAnimatedContainer.AddOnShowCallback(() => normalAnimatedContainer.Bind(serializedObject));
            highlightedAnimatedContainer.AddOnShowCallback(() => highlightedAnimatedContainer.Bind(serializedObject));
            pressedAnimatedContainer.AddOnShowCallback(() => pressedAnimatedContainer.Bind(serializedObject));
            selectedAnimatedContainer.AddOnShowCallback(() => selectedAnimatedContainer.Bind(serializedObject));
            disabledAnimatedContainer.AddOnShowCallback(() => disabledAnimatedContainer.Bind(serializedObject));

            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {

                targetFinder = root.schedule.Execute(() =>
                {
                    if (castedTarget == null)
                        return;

                    if (castedTarget.colorTarget != null)
                    {
                        foreach (UISelectionState state in UISelectable.uiSelectionStates)
                            castedTarget.GetAnimation(state).SetTarget(castedTarget.colorTarget);

                        targetFinder.Pause();
                        return;
                    }

                    castedTarget.FindTarget();

                }).Every(1000);
            }

            root.schedule.Execute(() =>
            {
                if (castedTarget == null)
                    return;

                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    if (castedTarget.hasColorTarget)
                        castedTarget.UpdateSettings();
                }

                void RefreshTab(ColorAnimationTab tab, ColorAnimation animation)
                {
                    bool isValid = animation.hasTarget && animation.isEnabled;
                    tab.referenceColorElement.SetStyleDisplay(isValid ? DisplayStyle.Flex : DisplayStyle.None);

                    if (!isValid)
                        return;

                    tab.SetReferenceColor
                    (
                        animation.animation.GetValue
                        (
                            animation.animation.toReferenceValue,
                            animation.animation.startColor,
                            animation.animation.currentColor,
                            animation.animation.toCustomValue,
                            animation.animation.toHueOffset,
                            animation.animation.toSaturationOffset,
                            animation.animation.toLightnessOffset,
                            animation.animation.toAlphaOffset
                        )
                    );
                }

                RefreshTab(normalTab, castedTarget.normalAnimation);
                RefreshTab(highlightedTab, castedTarget.highlightedAnimation);
                RefreshTab(pressedTab, castedTarget.pressedAnimation);
                RefreshTab(selectedTab, castedTarget.selectedAnimation);
                RefreshTab(disabledTab, castedTarget.disabledAnimation);

            }).Every(100);
        }

        private void InitializeColorTarget()
        {
            colorTargetObjectField =
                DesignUtils.NewObjectField(propertyColorTarget, typeof(ReactorColorTarget))
                    .SetTooltip("Animation color target")
                    .SetStyleFlexGrow(1);

            colorTargetFluidField =
                FluidField.Get()
                    .SetLabelText("Color Target")
                    .SetIcon(colorTargetIconTextures)
                    .SetStyleFlexShrink(0)
                    .AddFieldContent(colorTargetObjectField);
        }

        protected override void InitializeTabs()
        {
            base.InitializeTabs();

            ColorAnimationTab GetColorAnimationTab(UISelectionState state, FluidAnimatedContainer targetContainer)
            {
                ColorAnimationTab animationTab =
                    ColorAnimationTab.Get()
                        .SetTabAccentColor(selectableAccentColor);

                animationTab.tabButton
                    .SetLabelText(state.ToString())
                    .SetTooltip($"{state} animation")
                    .SetOnValueChanged(evt => targetContainer.Toggle(evt.newValue));

                return animationTab;
            }

            normalTab = GetColorAnimationTab(UISelectionState.Normal, normalAnimatedContainer);
            highlightedTab = GetColorAnimationTab(UISelectionState.Highlighted, highlightedAnimatedContainer);
            pressedTab = GetColorAnimationTab(UISelectionState.Pressed, pressedAnimatedContainer);
            selectedTab = GetColorAnimationTab(UISelectionState.Selected, selectedAnimatedContainer);
            disabledTab = GetColorAnimationTab(UISelectionState.Disabled, disabledAnimatedContainer);

            tabsGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.OneToggleOn);
            normalTab.tabButton.AddToToggleGroup(tabsGroup);
            highlightedTab.tabButton.AddToToggleGroup(tabsGroup);
            pressedTab.tabButton.AddToToggleGroup(tabsGroup);
            selectedTab.tabButton.AddToToggleGroup(tabsGroup);
            disabledTab.tabButton.AddToToggleGroup(tabsGroup);
        }

        protected override void ComposeTabs()
        {
            tabsContainer
                .AddChild(normalTab)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(highlightedTab)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(pressedTab)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(selectedTab)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(disabledTab)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(DesignUtils.flexibleSpace)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    DesignUtils.SystemButton_SortComponents
                    (
                        castedTarget.gameObject,
                        nameof(RectTransform),
                        nameof(Canvas),
                        nameof(CanvasGroup),
                        nameof(GraphicRaycaster)
                    )
                );
        }

        protected override void Compose()
        {
            ComposeTabs();
            ComposeAnimatedContainers();

            root
                .AddChild(componentHeader)
                .AddChild(tabsContainer)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(animatedContainers)
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(controllerField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(colorTargetFluidField)
                )
                .AddChild(DesignUtils.endOfLineBlock);
        }
    }
}
