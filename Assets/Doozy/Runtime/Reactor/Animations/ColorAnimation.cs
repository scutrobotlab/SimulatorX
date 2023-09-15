// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.Reactor.Targets;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Animations
{
    [Serializable]
    public class ColorAnimation : ReactorAnimation
    {
        /// <summary> Reference to a color target component </summary>
        public ReactorColorTarget colorTarget { get; private set; }

        /// <summary> Check if a color target is referenced or not </summary>
        public bool hasTarget => colorTarget != null;

        [SerializeField] private ColorTargetReaction Animation;
        /// <summary> Color Animation </summary>
        public ColorTargetReaction animation => Animation ?? (Animation = Reaction.Get<ColorTargetReaction>());

        /// <summary> Animation start color </summary>
        public Color startColor
        {
            get => animation.startColor;
            set => animation.startColor = value;
        }

        public override bool isEnabled => animation.enabled;
        public override bool isIdle => animation.isIdle;
        public override bool isActive => animation.isActive;
        public override bool isPaused => animation.isPaused;
        public override bool isPlaying => animation.isPlaying;
        public override bool inStartDelay => animation.inStartDelay;
        public override bool inLoopDelay => animation.inLoopDelay;

        public ColorAnimation(ReactorColorTarget target = null)
        {
            if (target == null)
                return;

            SetTarget(target);
        }

        public void SetTarget(ReactorColorTarget target)
        {
            colorTarget = null;
            _ = target ? target : throw new NullReferenceException(nameof(target));
            colorTarget = target;

            Initialize();
        }

        public void Initialize()
        {
            animation?.Stop(true);
            Animation ??= Reaction.Get<ColorTargetReaction>();
            animation?.SetTarget(colorTarget);

            UpdateValues();
        }

        public override void Recycle() =>
            animation?.Recycle();

        public override void UpdateValues() =>
            animation.UpdateValues();

        public override void StopAllReactionsOnTarget() =>
            Reaction.StopAllReactionsByTargetObject(colorTarget, true);

        public override void SetProgressAt(float targetProgress)
        {
            base.SetProgressAt(targetProgress);
            if (animation.enabled) animation.SetProgressAt(targetProgress);
        }

        public override void PlayToProgress(float toProgress)
        {
            base.PlayToProgress(toProgress);
            if (animation.enabled) animation.PlayToProgress(toProgress);
        }

        public override void PlayFromProgress(float fromProgress)
        {
            base.PlayFromProgress(fromProgress);
            if (animation.enabled) animation.PlayFromProgress(fromProgress);
        }

        public override void PlayFromToProgress(float fromProgress, float toProgress)
        {
            base.PlayFromToProgress(fromProgress, toProgress);
            if (animation.enabled) animation.PlayFromToProgress(fromProgress, toProgress);
        }

        public override void Play(bool inReverse = false)
        {
            if (colorTarget == null)
                return;

            RegisterCallbacks();
            if (!isActive)
            {
                StopAllReactionsOnTarget();
                // ResetToStartValues();
            }

            if (animation.enabled) animation.Play(inReverse);
        }

        public override void ResetToStartValues(bool forced = false)
        {
            if (forced | !animation.enabled) animation.SetValue(startColor);
        }

        public override void Stop()
        {
            if (animation.isActive || animation.enabled) animation.Stop();
            base.Stop();
        }

        public override void Finish()
        {
            if (animation.isActive || animation.enabled) animation.Finish();
            base.Finish();
        }

        public override void Reverse()
        {
            if (animation.isActive) animation.Reverse();
            else if (animation.enabled) animation.Play(PlayDirection.Reverse);
        }

        public override void Rewind()
        {
            if (animation.enabled) animation.Rewind();
        }

        public override void Pause() =>
            animation.Pause();

        public override void Resume() =>
            animation.Resume();

        protected override void RegisterCallbacks()
        {
            base.RegisterCallbacks();

            if (!animation.enabled)
                return;

            startedReactionsCount++;
            animation.OnPlayCallback += InvokeOnPlay;
            animation.OnStopCallback += InvokeOnStop;
            animation.OnFinishCallback += InvokeOnFinish;
        }

        protected override void UnregisterOnPlayCallbacks()
        {
            if (!animation.enabled)
                return;

            animation.OnPlayCallback -= InvokeOnPlay;
        }

        protected override void UnregisterOnStopCallbacks()
        {
            if (!animation.enabled)
                return;

            animation.OnStopCallback -= InvokeOnStop;
        }

        protected override void UnregisterOnFinishCallbacks()
        {
            if (!animation.enabled)
                return;

            animation.OnFinishCallback -= InvokeOnFinish;
        }
    }
}
