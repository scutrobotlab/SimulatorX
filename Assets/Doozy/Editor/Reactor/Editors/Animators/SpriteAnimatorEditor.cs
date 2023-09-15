// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Editors.Animators.Internal;
using Doozy.Runtime.Reactor;
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
    [CustomEditor(typeof(SpriteAnimator), true)]
    [CanEditMultipleObjects]
    public class SpriteAnimatorEditor : BaseReactorAnimatorEditor
    {
        public static IEnumerable<Texture2D> spriteAnimatorIconTextures => EditorMicroAnimations.Reactor.Icons.SpriteAnimator;
        public static IEnumerable<Texture2D> spriteAnimationIconTextures => EditorMicroAnimations.Reactor.Icons.SpriteAnimation;
        public static IEnumerable<Texture2D> spriteTargetIconTextures => EditorMicroAnimations.Reactor.Icons.SpriteTarget;

        private SpriteAnimator castedTarget => (SpriteAnimator)target;
        private IEnumerable<SpriteAnimator> castedTargets => targets.Cast<SpriteAnimator>();

        private ObjectField spriteTargetObjectField { get; set; }
        private FluidField spriteTargetFluidField { get; set; }
        private SerializedProperty propertySpriteTarget { get; set; }
        private IVisualElementScheduledItem targetFinder { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            spriteTargetFluidField?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();
            propertySpriteTarget = serializedObject.FindProperty("SpriteTarget");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(SpriteAnimator)))
                .SetIcon(spriteAnimatorIconTextures.ToList())
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/external/1046675529/ZTk4MjcyZjljOWM5NDVlYjkyNDI1NzE0OTFjZWYyYzk?atlOrigin=eyJpIjoiM2RlMWZmMjM0NjgyNGZlZjhiOTBmNGM5MTNjODIwNzYiLCJwIjoiYyJ9")
                .AddYouTubeButton();

            animationTabButton
                .SetIcon(spriteAnimationIconTextures);
            
            controls
                .SetFirstFrameButtonCallback(() =>
                {
                    if (serializedObject.isEditingMultipleObjects)
                    {
                        foreach (SpriteAnimator animator in castedTargets)
                            animator.SetProgressAtZero();
                        return;
                    }
                    castedTarget.SetProgressAtZero();
                })
                .SetPlayForwardButtonCallback(() =>
                {
                    if (serializedObject.isEditingMultipleObjects)
                    {
                        foreach (SpriteAnimator animator in castedTargets)
                            animator.Play(PlayDirection.Forward);
                        return;
                    }
                    castedTarget.Play(PlayDirection.Forward);
                })
                .SetStopButtonCallback(() =>
                {
                    if (serializedObject.isEditingMultipleObjects)
                    {
                        foreach (SpriteAnimator animator in castedTargets)
                            animator.Stop();
                        return;
                    }
                    castedTarget.Stop();
                })
                .SetPlayReverseButtonCallback(() =>
                {
                    if (serializedObject.isEditingMultipleObjects)
                    {
                        foreach (SpriteAnimator animator in castedTargets)
                            animator.Play(PlayDirection.Reverse);
                        return;
                    }
                    castedTarget.Play(PlayDirection.Reverse);
                })
                .SetReverseButtonCallback(() =>
                {
                    if (serializedObject.isEditingMultipleObjects)
                    {
                        foreach (SpriteAnimator animator in castedTargets)
                            animator.Reverse();
                        return;
                    }
                    castedTarget.Reverse();
                })
                .SetLastFrameButtonCallback(() =>
                {
                    if (serializedObject.isEditingMultipleObjects)
                    {
                        foreach (SpriteAnimator animator in castedTargets)
                            animator.SetProgressAtOne();
                        return;
                    }
                    castedTarget.SetProgressAtOne();
                });

            spriteTargetObjectField =
                DesignUtils.NewObjectField(propertySpriteTarget, typeof(ReactorSpriteTarget))
                    .SetStyleFlexGrow(1)
                    .SetTooltip("Animation sprite target");
            spriteTargetFluidField =
                FluidField.Get()
                    .SetLabelText("Sprite Target")
                    .SetIcon(spriteTargetIconTextures)
                    .AddFieldContent(spriteTargetObjectField);

            targetFinder = root.schedule.Execute(() =>
            {
                if (castedTarget == null)
                    return;

                if (castedTarget.spriteTarget != null)
                {
                    castedTarget.animation.SetTarget(castedTarget.spriteTarget);
                    
                    targetFinder.Pause();
                    return;
                }

                castedTarget.FindTarget();

            }).Every(1000);
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
                        .AddChild(animationTabButton)
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
                .AddChild(spriteTargetFluidField)
                .AddChild(DesignUtils.endOfLineBlock);
        }
    }
}
