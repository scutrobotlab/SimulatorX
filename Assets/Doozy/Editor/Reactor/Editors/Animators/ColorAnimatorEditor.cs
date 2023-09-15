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
using Doozy.Editor.Reactor.Editors.Animators.Internal;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Animators;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Reactor.Editors.Animators
{
    [CustomEditor(typeof(ColorAnimator), true)]
    [CanEditMultipleObjects]
    public class ColorAnimatorEditor : BaseReactorAnimatorEditor
    {
        public static IEnumerable<Texture2D> colorAnimatorIconTextures => EditorMicroAnimations.Reactor.Icons.ColorAnimator;
        public static IEnumerable<Texture2D> colorAnimationIconTextures => EditorMicroAnimations.Reactor.Icons.ColorAnimation;
        public static IEnumerable<Texture2D> colorTargetIconTextures => EditorMicroAnimations.Reactor.Icons.ColorTarget;

        private ColorAnimator castedTarget => (ColorAnimator)target;
        private IEnumerable<ColorAnimator> castedTargets => targets.Cast<ColorAnimator>();

        private ColorAnimationTab colorAnimationTab { get; set; }

        private ObjectField colorTargetObjectField { get; set; }
        private FluidField colorTargetFluidField { get; set; }
        private SerializedProperty propertyColorTarget { get; set; }
        private IVisualElementScheduledItem targetFinder { get; set; }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            colorTargetFluidField?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();
            propertyColorTarget = serializedObject.FindProperty("ColorTarget");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            animationTabButton.RemoveFromToggleGroup();
            animationTabButton?.Recycle();

            colorAnimationTab =
                ColorAnimationTab.Get()
                    .SetTabAccentColor(selectableAccentColor);

            colorAnimationTab.tabButton
                .SetIcon(colorAnimationIconTextures)
                .SetLabelText("Animation")
                .SetOnValueChanged(evt => animationAnimatedContainer.Toggle(evt.newValue));

            colorAnimationTab.tabButton.AddToToggleGroup(tabsGroup);
            colorAnimationTab.tabButton.SetIsOn(true, false);

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(ColorAnimator)))
                .SetIcon(colorAnimatorIconTextures.ToList())
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1046675529/Color+Animator?atlOrigin=eyJpIjoiNmIwYjkxNTQwNjdhNGZkYTg1NTNlOTRiOGZlY2I5ZjIiLCJwIjoiYyJ9")
                .AddYouTubeButton();

            controls
                .SetFirstFrameButtonCallback(() =>
                {
                    if (serializedObject.isEditingMultipleObjects)
                    {
                        foreach (ColorAnimator animator in castedTargets)
                            animator.SetProgressAtZero();
                        return;
                    }
                    castedTarget.SetProgressAtZero();
                })
                .SetPlayForwardButtonCallback(() =>
                {
                    if (serializedObject.isEditingMultipleObjects)
                    {
                        foreach (ColorAnimator animator in castedTargets)
                            animator.Play(PlayDirection.Forward);
                        return;
                    }
                    castedTarget.Play(PlayDirection.Forward);
                })
                .SetStopButtonCallback(() =>
                {
                    if (serializedObject.isEditingMultipleObjects)
                    {
                        foreach (ColorAnimator animator in castedTargets)
                            animator.Stop();
                        return;
                    }
                    castedTarget.Stop();
                })
                .SetPlayReverseButtonCallback(() =>
                {
                    if (serializedObject.isEditingMultipleObjects)
                    {
                        foreach (ColorAnimator animator in castedTargets)
                            animator.Play(PlayDirection.Reverse);
                        return;
                    }
                    castedTarget.Play(PlayDirection.Reverse);
                })
                .SetReverseButtonCallback(() =>
                {
                    if (serializedObject.isEditingMultipleObjects)
                    {
                        foreach (ColorAnimator animator in castedTargets)
                            animator.Reverse();
                        return;
                    }
                    castedTarget.Reverse();
                })
                .SetLastFrameButtonCallback(() =>
                {
                    if (serializedObject.isEditingMultipleObjects)
                    {
                        foreach (ColorAnimator animator in castedTargets)
                            animator.SetProgressAtOne();
                        return;
                    }
                    castedTarget.SetProgressAtOne();
                });

            colorTargetObjectField =
                DesignUtils.NewObjectField(propertyColorTarget, typeof(ReactorColorTarget))
                    .SetStyleFlexGrow(1)
                    .SetTooltip("Animation color target");
            colorTargetFluidField =
                FluidField.Get()
                    .SetLabelText("Color Target")
                    .SetIcon(colorTargetIconTextures)
                    .AddFieldContent(colorTargetObjectField);

            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                targetFinder = root.schedule.Execute(() =>
                {
                    if (castedTarget == null)
                        return;

                    if (castedTarget.colorTarget != null)
                    {
                        targetFinder.Pause();
                        return;
                    }

                    castedTarget.FindTarget();

                }).Every(1000);
            }

            root.schedule.Execute(() =>
            {
                if (castedTarget == null) return;

                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    if (castedTarget.hasTarget)
                        castedTarget.SetTarget(castedTarget.colorTarget);
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

                RefreshTab(colorAnimationTab, castedTarget.animation);

            }).Every(100);
        }

        protected override void Compose()
        {
            base.Compose();

            root
                .AddChild(componentHeader)
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleMargins(42, -2, DesignUtils.k_Spacing2X, DesignUtils.k_Spacing2X)
                        .AddChild(colorAnimationTab)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(settingsTab)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(animatorNameTab)
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
                        )
                )
                .AddChild(controls.SetStyleMargins(DesignUtils.k_Spacing))
                .AddChild(animationAnimatedContainer)
                .AddChild(behaviourAnimatedContainer)
                .AddChild(animatorNameAnimatedContainer)
                .AddChild(colorTargetFluidField)
                .AddChild(DesignUtils.endOfLineBlock);
        }
    }
}
