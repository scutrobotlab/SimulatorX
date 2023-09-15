// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.UIManager.Containers;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Animators
{
    /// <summary>
    /// Specialized animator component used to animate the Color for a Reactor Color Target by listening to a UIContainer (controller) show/hide commands.
    /// </summary>
    [AddComponentMenu("Doozy/UI/Animators/Container/UI Container Color Animator")]
    public class UIContainerColorAnimator : BaseUIContainerAnimator
    {
        [SerializeField] private ReactorColorTarget ColorTarget;
        /// <summary> Reference to a color target component </summary>
        public ReactorColorTarget colorTarget => ColorTarget;

        /// <summary> Check if a color target is referenced or not </summary>
        public bool hasColorTarget => ColorTarget != null;

        [SerializeField] private ColorAnimation ShowAnimation;
        /// <summary> Container Show Animation </summary>
        public ColorAnimation showAnimation => ShowAnimation;

        [SerializeField] private ColorAnimation HideAnimation;
        /// <summary> Container Hide Animation </summary>
        public ColorAnimation hideAnimation => HideAnimation;

        #if UNITY_EDITOR
        protected override void Reset()
        {
            FindTarget();

            ShowAnimation ??= new ColorAnimation(colorTarget);
            HideAnimation ??= new ColorAnimation(colorTarget);

            ResetAnimation(ShowAnimation);
            ResetAnimation(HideAnimation);

            base.Reset();
        }
        #endif

        /// <summary> Find ColorTarget </summary>
        public void FindTarget()
        {
            if (ColorTarget != null)
                return;

            ColorTarget = ReactorColorTarget.FindTarget(gameObject);
            UpdateSettings();
        }

        protected override void Awake()
        {
            FindTarget();
            UpdateSettings();
            base.Awake();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ShowAnimation?.Recycle();
            HideAnimation?.Recycle();
        }

        protected override void ConnectToController()
        {
            base.ConnectToController();
            if (!controller) return;

            controller.showReactions.Add(ShowAnimation.animation);
            controller.hideReactions.Add(HideAnimation.animation);
        }

        protected override void DisconnectFromController()
        {
            base.DisconnectFromController();
            if (!controller) return;

            controller.showReactions.Remove(ShowAnimation.animation);
            controller.hideReactions.Remove(HideAnimation.animation);
        }

        /// <summary> Play the show animation </summary>
        public override void Show() =>
            ShowAnimation?.Play(PlayDirection.Forward);

        /// <summary> Play the hide animation </summary>
        public override void Hide() =>
            HideAnimation?.Play(PlayDirection.Forward);

        /// <summary> Set show animation's progress at one </summary>
        public override void InstantShow() =>
            ShowAnimation?.SetProgressAtOne();

        /// <summary> Set hide animation's progress at one </summary>
        public override void InstantHide() =>
            HideAnimation?.SetProgressAtOne();

        /// <summary> Reverse the show animation (if playing) </summary>
        public override void ReverseShow() =>
            ShowAnimation?.Reverse();

        /// <summary> Reverse the hide animation (if playing) </summary>
        public override void ReverseHide() =>
            HideAnimation?.Reverse();

        /// <summary> Update the animations settings (if a colorTarget is referenced) </summary>
        public override void UpdateSettings()
        {
            if (colorTarget == null)
                return;

            ShowAnimation?.SetTarget(colorTarget);
            HideAnimation?.SetTarget(colorTarget);
        }

        /// <summary> Stop all animations </summary>
        public override void StopAllReactions()
        {
            ShowAnimation?.Stop();
            HideAnimation?.Stop();
        }

        private static void ResetAnimation(ColorAnimation target)
        {
            target.animation.Reset();
            target.animation.enabled = true;
            target.animation.fromReferenceValue = ReferenceValue.CurrentValue;
            target.animation.settings.duration = UIContainer.k_DefaultAnimationDuration;
        }

        /// <summary> Set the Start Color for all animations </summary>
        /// <param name="color"> New start color </param>
        public void SetStartColor(Color color)
        {
            SetStartColorForShow(color);
            SetStartColorForHide(color);
        }

        /// <summary> Set the Start Color for the Show animation </summary>
        /// <param name="color"> New start color </param>
        public void SetStartColorForShow(Color color)
        {
            if (ShowAnimation == null) return;
            ShowAnimation.startColor = color;
            if (controller == null) return;
            switch (controller.visibilityState)
            {
                case VisibilityState.Visible:
                    ShowAnimation.SetProgressAtOne();
                    break;
                case VisibilityState.IsShowing:
                    ShowAnimation.Play();
                    break;
            }
        }

        /// <summary> Set the Start Color for the Hide animation </summary>
        /// <param name="color"> New start color </param>
        public void SetStartColorForHide(Color color)
        {
            if (ShowAnimation == null) return;
            ShowAnimation.startColor = color;
            switch (controller.visibilityState)
            {
                case VisibilityState.Hidden:
                    HideAnimation.SetProgressAtOne();
                    break;
                case VisibilityState.IsHiding:
                    HideAnimation.Play();
                    break;
            }
        }
    }
}
