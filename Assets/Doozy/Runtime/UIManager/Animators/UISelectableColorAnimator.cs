// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Colors.Models;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.UIManager.Components;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Animators
{
    /// <summary>
    /// Specialized animator component used to animate the Color for a Reactor Color Target by listening to a UISelectable (controller) selection state changes.
    /// </summary>
    [AddComponentMenu("Doozy/UI/Animators/Selectable/UI Selectable Color Animator")]
    public class UISelectableColorAnimator : BaseUISelectableAnimator
    {
        [SerializeField] private ReactorColorTarget ColorTarget;
        /// <summary> Reference to a color target component </summary>
        public ReactorColorTarget colorTarget => ColorTarget;

        /// <summary> Check if a color target is referenced or not </summary>
        public bool hasColorTarget => ColorTarget != null;

        [SerializeField] private ColorAnimation NormalAnimation;
        /// <summary> Animation for the Normal selection state </summary>
        public ColorAnimation normalAnimation => NormalAnimation;

        [SerializeField] private ColorAnimation HighlightedAnimation;
        /// <summary> Animation for the Highlighted selection state </summary>
        public ColorAnimation highlightedAnimation => HighlightedAnimation;

        [SerializeField] private ColorAnimation PressedAnimation;
        /// <summary> Animation for the Pressed selection state </summary>
        public ColorAnimation pressedAnimation => PressedAnimation;

        [SerializeField] private ColorAnimation SelectedAnimation;
        /// <summary> Animation for the Selected selection state </summary>
        public ColorAnimation selectedAnimation => SelectedAnimation;

        [SerializeField] private ColorAnimation DisabledAnimation;
        /// <summary> Animation for the Disabled selection state </summary>
        public ColorAnimation disabledAnimation => DisabledAnimation;

        /// <summary> Get the animation triggered by the given selection state </summary>
        /// <param name="state"> Target selection state </param>
        public ColorAnimation GetAnimation(UISelectionState state) =>
            state switch
            {
                UISelectionState.Normal      => NormalAnimation,
                UISelectionState.Highlighted => HighlightedAnimation,
                UISelectionState.Pressed     => PressedAnimation,
                UISelectionState.Selected    => SelectedAnimation,
                UISelectionState.Disabled    => DisabledAnimation,
                _                            => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };

        #if UNITY_EDITOR
        protected override void Reset()
        {
            FindTarget();

            NormalAnimation ??= new ColorAnimation(colorTarget);
            HighlightedAnimation ??= new ColorAnimation(colorTarget);
            PressedAnimation ??= new ColorAnimation(colorTarget);
            SelectedAnimation ??= new ColorAnimation(colorTarget);
            DisabledAnimation ??= new ColorAnimation(colorTarget);

            foreach (UISelectionState state in UISelectable.uiSelectionStates)
                ResetAnimation(state);

            base.Reset();
        }
        #endif

        public void FindTarget()
        {
            if (ColorTarget != null)
                return;

            ColorTarget = ReactorColorTarget.FindTarget(gameObject);
            UpdateSettings();
        }

        protected override void Awake()
        {
            base.Awake();
            FindTarget();
            UpdateSettings();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (UISelectionState state in UISelectable.uiSelectionStates)
                GetAnimation(state)?.Recycle();
        }

        public override bool IsStateEnabled(UISelectionState state)
        {
            ColorAnimation colorAnimation = GetAnimation(state);
            return colorAnimation is { isEnabled: true };
        }

        public override void UpdateSettings()
        {
            if (colorTarget == null)
                return;

            foreach (UISelectionState state in UISelectable.uiSelectionStates)
                GetAnimation(state)?.SetTarget(colorTarget);
        }

        public override void StopAllReactions()
        {
            foreach (UISelectionState state in UISelectable.uiSelectionStates)
                GetAnimation(state)?.Stop();
        }

        public override void Play(UISelectionState state) =>
            GetAnimation(state)?.Play();

        private void ResetAnimation(UISelectionState state)
        {
            ColorAnimation a = GetAnimation(state);

            a.animation.Reset();
            a.animation.enabled = true;
            a.animation.fromReferenceValue = ReferenceValue.CurrentValue;
            a.animation.settings.duration = UISelectable.k_DefaultAnimationDuration;

            switch (state)
            {
                case UISelectionState.Normal:
                    a.animation.settings.duration = UISelectable.k_DefaultAnimationDuration * 0.5f;
                    break;
                case UISelectionState.Highlighted:
                    a.animation.toLightnessOffset = 0.1f;
                    break;
                case UISelectionState.Pressed:
                    a.animation.toLightnessOffset = -0.1f;
                    a.animation.settings.duration = UISelectable.k_DefaultAnimationDuration * 0.25f;
                    break;
                case UISelectionState.Selected:
                    a.animation.toHueOffset = -10f / HSL.H.F;
                    a.animation.toLightnessOffset = 0.2f;
                    break;
                case UISelectionState.Disabled:
                    a.animation.toSaturationOffset = -0.5f;
                    a.animation.toAlphaOffset = -0.5f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        /// <summary> Set the Start Color for all animations </summary>
        /// <param name="color"> New start color </param>
        public void SetStartColor(Color color)
        {
            foreach (UISelectionState state in UISelectable.uiSelectionStates)
            {
                ColorAnimation colorAnimation = GetAnimation(state);
                if (colorAnimation == null) continue;
                colorAnimation.startColor = color;
            }

            if (controller == null) return;
            controller.RefreshState();
        }

        /// <summary> Set the Start Color for the target selection state </summary>
        /// <param name="color"> New start color </param>
        /// <param name="selectionState"> Target selection state </param>
        public void SetStartColor(Color color, UISelectionState selectionState)
        {
            ColorAnimation colorAnimation = GetAnimation(selectionState);
            if (colorAnimation == null) return;
            colorAnimation.startColor = color;
            if (controller == null) return;
            if (controller.currentUISelectionState != selectionState) return;
            controller.RefreshState();
        }

    }
}
