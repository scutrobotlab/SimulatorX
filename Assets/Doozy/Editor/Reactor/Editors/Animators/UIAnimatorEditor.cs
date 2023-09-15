// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Components;
using Doozy.Editor.Reactor.Editors.Animators.Internal;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animators;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Reactor.Editors.Animators
{
    [CustomEditor(typeof(UIAnimator), true)]
    [CanEditMultipleObjects]
    public class UIAnimatorEditor : BaseReactorAnimatorEditor
    {
        public static IEnumerable<Texture2D> uiAnimatorIconTextures => EditorMicroAnimations.Reactor.Icons.UIAnimator;
        public static IEnumerable<Texture2D> uiAnimationIconTextures => EditorMicroAnimations.Reactor.Icons.UIAnimation;

        private UIAnimator castedTarget => (UIAnimator)target;
        private IEnumerable<UIAnimator> castedTargets => targets.Cast<UIAnimator>();

        private VisualElement animationTabContainer { get; set; }
        private UIAnimatorIndicators animationTabIndicators { get; set; }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            animationTabButton?.Recycle();
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(UIAnimator)))
                .SetIcon(uiAnimatorIconTextures.ToList())
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048150050/UI+Animator?atlOrigin=eyJpIjoiYzMxOTZiNGQwYmRjNGVkNTkxOTA1MGYyNzBlMGFmZWIiLCJwIjoiYyJ9")
                .AddYouTubeButton();

            animationTabContainer =
                new VisualElement();

            animationTabButton
                .SetIcon(uiAnimationIconTextures);

            animationTabIndicators =
                new UIAnimatorIndicators()
                    .Initialize(propertyAnimation, animationTabButton, animationTabContainer);

            controls
                .SetFirstFrameButtonCallback(() =>
                {
                    if (serializedObject.isEditingMultipleObjects)
                    {
                        foreach (UIAnimator animator in castedTargets)
                            animator.SetProgressAtZero();
                        return;
                    }
                    castedTarget.SetProgressAtZero();
                })
                .SetPlayForwardButtonCallback(() =>
                {
                    if (serializedObject.isEditingMultipleObjects)
                    {
                        foreach (UIAnimator animator in castedTargets)
                            animator.Play(PlayDirection.Forward);
                        return;
                    }
                    castedTarget.Play(PlayDirection.Forward);
                })
                .SetStopButtonCallback(() =>
                {
                    if (serializedObject.isEditingMultipleObjects)
                    {
                        foreach (UIAnimator animator in castedTargets)
                            animator.Stop();
                        return;
                    }
                    castedTarget.Stop();
                })
                .SetPlayReverseButtonCallback(() =>
                {
                    if (serializedObject.isEditingMultipleObjects)
                    {
                        foreach (UIAnimator animator in castedTargets)
                            animator.Play(PlayDirection.Reverse);
                        return;
                    }
                    castedTarget.Play(PlayDirection.Reverse);
                })
                .SetReverseButtonCallback(() =>
                {
                    if (serializedObject.isEditingMultipleObjects)
                    {
                        foreach (UIAnimator animator in castedTargets)
                            animator.Reverse();
                        return;
                    }
                    castedTarget.Reverse();
                })
                .SetLastFrameButtonCallback(() =>
                {
                    if (serializedObject.isEditingMultipleObjects)
                    {
                        foreach (UIAnimator animator in castedTargets)
                            animator.SetProgressAtOne();
                        return;
                    }
                    castedTarget.SetProgressAtOne();
                });
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
                        .AddChild(animationTabContainer)
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
                .AddChild(DesignUtils.endOfLineBlock);
        }
    }
}
