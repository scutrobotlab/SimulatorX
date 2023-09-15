// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Animators.Internal;
using Doozy.Runtime.Reactor.Targets;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Animators
{
    [AddComponentMenu("Doozy/Reactor/Animators/Color Animator")]
    public class ColorAnimator : ReactorAnimator
    {
        /// <summary>
        /// Specialized animator component used to animate the Color for a Reactor Color Target.
        /// </summary>
        [SerializeField] private ReactorColorTarget ColorTarget;
        /// <summary> Reference to a color target component </summary>
        public ReactorColorTarget colorTarget => ColorTarget;

        /// <summary> Check if a color target is referenced or not </summary>
        public bool hasTarget => ColorTarget != null;

        [SerializeField] private ColorAnimation Animation;
        /// <summary> Color Animation </summary>
        public new ColorAnimation animation => Animation ?? (Animation = new ColorAnimation(colorTarget));

        #if UNITY_EDITOR
        private void Reset()
        {
            FindTarget();
            Animation ??= new ColorAnimation(colorTarget);
            ResetAnimation();
        }
        #endif

        public void FindTarget()
        {
            if (ColorTarget != null)
            {
                if (animation.colorTarget != ColorTarget)
                    animation.SetTarget(ColorTarget);
                return;
            }

            ColorTarget = ReactorColorTarget.FindTarget(gameObject);
            if (ColorTarget != null) animation.SetTarget(ColorTarget);
        }

        protected override void Awake()
        {
            if (!Application.isPlaying) return;
            base.Awake();
            FindTarget();
        }

        public override void Play(PlayDirection playDirection) =>
            animation.Play(playDirection);

        public override void Play(bool inReverse = false) =>
            animation.Play(inReverse);

        public override void SetTarget(object target) =>
            SetTarget(target as ReactorColorTarget);

        public void SetTarget(ReactorColorTarget target) =>
            animation.SetTarget(target);

        public override void ResetToStartValues(bool forced = false) =>
            animation.ResetToStartValues(forced);

        public override void UpdateSettings()
        {
            if (animation.colorTarget != null) return;
            SetTarget(colorTarget);
            if (animation.isPlaying) UpdateValues();
        }

        public override void UpdateValues() =>
            animation.UpdateValues();

        public override void PlayToProgress(float toProgress) =>
            animation.PlayToProgress(toProgress);

        public override void PlayFromProgress(float fromProgress) =>
            animation.PlayFromProgress(fromProgress);

        public override void PlayFromToProgress(float fromProgress, float toProgress) =>
            animation.PlayFromToProgress(fromProgress, toProgress);

        public override void Stop() =>
            animation.Stop();

        public override void Finish() =>
            animation.Finish();

        public override void Reverse() =>
            animation.Reverse();

        public override void Rewind() =>
            animation.Rewind();

        public override void Pause() =>
            animation.Pause();

        public override void Resume() =>
            animation.Resume();

        public override void SetProgressAtOne() =>
            animation.SetProgressAtOne();

        public override void SetProgressAtZero() =>
            animation.SetProgressAtZero();

        public override void SetProgressAt(float targetProgress) =>
            animation.SetProgressAt(targetProgress);

        protected override void Recycle() =>
            animation?.Recycle();

        private void ResetAnimation()
        {
            animation.animation.Reset();
            animation.animation.enabled = true;
            animation.animation.fromReferenceValue = ReferenceValue.CurrentValue;
            animation.animation.settings.duration = 0.24f;
        }
    }
}
