// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.Reactor.Targets;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Animations
{
    [Serializable]
    public class SpriteAnimation : ReactorAnimation
    {
        /// <summary> Reference to a sprite target component </summary>
        public ReactorSpriteTarget spriteTarget { get; private set; }

        [SerializeField] private List<Sprite> Sprites = new List<Sprite>();
        public List<Sprite> sprites => Sprites;

        /// <summary> Check if a sprite target is referenced or not </summary>
        public bool hasTarget => spriteTarget != null;

        [SerializeField] private SpriteTargetReaction Animation;
        public SpriteTargetReaction animation => Animation ?? (Animation = Reaction.Get<SpriteTargetReaction>());

        /// <summary> Animation start frame </summary>
        public int startFrame
        {
            get => animation.startFrame;
            set => animation.startFrame = value;
        }

        public override bool isEnabled => animation.enabled;
        public override bool isIdle => animation.isIdle;
        public override bool isActive => animation.isActive;
        public override bool isPaused => animation.isPaused;
        public override bool isPlaying => animation.isPlaying;
        public override bool inStartDelay => animation.inStartDelay;
        public override bool inLoopDelay => animation.inLoopDelay;

        public SpriteAnimation(ReactorSpriteTarget target = null)
        {
            if (target == null)
                return;

            SetTarget(target);
        }

        public SpriteAnimation SetSprites(IEnumerable<Sprite> spriteEnumerable)
        {
            if (spriteEnumerable == null) return this;
            Sprites.Clear();
            Sprites.AddRange(spriteEnumerable);
            return UpdateAnimationSprites();
        }

        public SpriteAnimation UpdateAnimationSprites()
        {
            animation.SetSprites(Sprites, false);
            return this;
        }

        public SpriteAnimation SortSpritesAz()
        {
            Sprites = Sprites.OrderBy(item => item.name).ToList();
            return UpdateAnimationSprites();
        }

        public SpriteAnimation SortSpritesZa()
        {
            Sprites = Sprites.OrderByDescending(item => item.name).ToList();
            return UpdateAnimationSprites();
        }

        public void SetTarget(ReactorSpriteTarget target)
        {
            spriteTarget = null;
            _ = target ? target : throw new NullReferenceException(nameof(target));
            spriteTarget = target;

            Initialize();
        }

        public void Initialize()
        {
            animation?.Stop(true);
            Animation ??= Reaction.Get<SpriteTargetReaction>();
            animation?.SetTarget(spriteTarget);

            UpdateValues();
        }

        public override void Recycle() =>
            animation?.Recycle();

        public SpriteAnimation SetSprites(List<Sprite> spriteList)
        {
            if (spriteList == null || spriteList.Count == 0)
                return this;
            animation.SetSprites(spriteList);
            return this;
        }

        public override void UpdateValues()
        {
            UpdateAnimationSprites();
            animation.UpdateValues();
        }

        public override void StopAllReactionsOnTarget() =>
            Reaction.StopAllReactionsByTargetObject(spriteTarget, true);


        public override void SetProgressAt(float targetProgress)
        {
            base.SetProgressAt(targetProgress);
            if (!animation.enabled)
                return;
            UpdateAnimationSprites();
            animation.SetProgressAt(targetProgress);
        }

        public override void PlayToProgress(float toProgress)
        {
            base.PlayToProgress(toProgress);
            if (!animation.enabled)
                return;
            UpdateAnimationSprites();
            animation.PlayToProgress(toProgress);
        }

        public override void PlayFromProgress(float fromProgress)
        {
            base.PlayFromProgress(fromProgress);
            if (!animation.enabled)
                return;
            UpdateAnimationSprites();
            animation.PlayFromProgress(fromProgress);
        }

        public override void PlayFromToProgress(float fromProgress, float toProgress)
        {
            base.PlayFromToProgress(fromProgress, toProgress);
            if (!animation.enabled)
                return;
            UpdateAnimationSprites();
            animation.PlayFromToProgress(fromProgress, toProgress);
        }

        public override void Play(bool inReverse = false)
        {
            if (spriteTarget == null)
                return;

            RegisterCallbacks();
            if (!isActive)
            {
                StopAllReactionsOnTarget();
                // ResetToStartValues();
            }

            if (!animation.enabled)
                return;
            UpdateAnimationSprites();
            animation.Play(inReverse);
        }

        public override void ResetToStartValues(bool forced = false)
        {
            if (!(forced | !animation.enabled))
                return;
            UpdateAnimationSprites();
            animation.SetValue(startFrame);
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
            if (!animation.enabled)
                return;
            UpdateAnimationSprites();
            animation.Rewind();
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
