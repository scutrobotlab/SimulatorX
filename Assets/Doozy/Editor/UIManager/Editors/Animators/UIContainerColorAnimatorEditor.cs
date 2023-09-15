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
using Doozy.Runtime.UIManager.Animators;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Animators
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIContainerColorAnimator), true)]
    public class UIContainerColorAnimatorEditor : BaseUIContainerAnimatorEditor
    {
        public UIContainerColorAnimator castedTarget => (UIContainerColorAnimator)target;
        public IEnumerable<UIContainerColorAnimator> castedTargets => targets.Cast<UIContainerColorAnimator>();

        public static IEnumerable<Texture2D> colorAnimatorIconTextures => EditorMicroAnimations.Reactor.Icons.ColorAnimator;
        public static IEnumerable<Texture2D> colorAnimationIconTextures => EditorMicroAnimations.Reactor.Icons.ColorAnimation;
        public static IEnumerable<Texture2D> colorTargetIconTextures => EditorMicroAnimations.Reactor.Icons.ColorTarget;
        
        private ObjectField colorTargetObjectField { get; set; }
        private FluidField colorTargetFluidField { get; set; }
        private SerializedProperty propertyColorTarget { get; set; }
        private IVisualElementScheduledItem targetFinder { get; set; }

        private ColorAnimationTab showTab { get; set; }
        private ColorAnimationTab hideTab { get; set; }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();

            showTab?.Recycle();
            hideTab?.Recycle();

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
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048739917/UIContainer+Color+Animator?atlOrigin=eyJpIjoiMmY2YTdiNjcwNGUzNDYzNWI0ZDBmNTAyYWI4YzVjZTEiLCJwIjoiYyJ9")
                .AddYouTubeButton();

            InitializeColorTarget();

            showAnimatedContainer.AddOnShowCallback(() => showAnimatedContainer.Bind(serializedObject));
            hideAnimatedContainer.AddOnShowCallback(() => hideAnimatedContainer.Bind(serializedObject));

            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                targetFinder = root.schedule.Execute(() =>
                {
                    if (castedTarget == null)
                        return;

                    if (castedTarget.colorTarget != null)
                    {
                        castedTarget.showAnimation.SetTarget(castedTarget.colorTarget);
                        castedTarget.hideAnimation.SetTarget(castedTarget.colorTarget);

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
                
                RefreshTab(showTab, castedTarget.showAnimation);
                RefreshTab(hideTab, castedTarget.hideAnimation);

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

            ColorAnimationTab GetColorAnimationTab(FluidAnimatedContainer targetContainer, string labelText, string tooltip)
            {
                ColorAnimationTab animationTab =
                    ColorAnimationTab.Get()
                        .SetTabAccentColor(selectableAccentColor);

                animationTab.tabButton
                    .SetLabelText(labelText)
                    .SetTooltip(tooltip)
                    .SetOnValueChanged(evt => targetContainer.Toggle(evt.newValue));

                return animationTab;
            }

            showTab = GetColorAnimationTab(showAnimatedContainer, "Show", "Show Animation");
            hideTab = GetColorAnimationTab(hideAnimatedContainer, "Hide", "Hide Animation");

            tabsGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.OneToggleOn);
            showTab.tabButton.AddToToggleGroup(tabsGroup);
            hideTab.tabButton.AddToToggleGroup(tabsGroup);
        }

        protected override void ComposeTabs()
        {
            tabsContainer
                .AddChild(showTab)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(hideTab)
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
