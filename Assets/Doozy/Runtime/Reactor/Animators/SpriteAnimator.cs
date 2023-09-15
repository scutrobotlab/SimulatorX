// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Animators.Internal;
using Doozy.Runtime.Reactor.Targets;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Animators
{
    [AddComponentMenu("Doozy/Reactor/Animators/Sprite Animator")]
    public class SpriteAnimator : ReactorAnimator
    {
        /// <summary>
        /// Specialized animator component used to animate a set of Sprites for a Reactor Sprite Target.
        /// </summary>
        [SerializeField] private ReactorSpriteTarget SpriteTarget;
        /// <summary> Reference to a sprite target component </summary>
        public ReactorSpriteTarget spriteTarget => SpriteTarget;

        /// <summary> Check if a sprite target is referenced or not </summary>
        public bool hasTarget => SpriteTarget != null;

        [SerializeField] private SpriteAnimation Animation;
        /// <summary> Sprite frame by frame Animation </summary>
        public new SpriteAnimation animation => Animation ?? (Animation = new SpriteAnimation(spriteTarget));

        #if UNITY_EDITOR
        private void Reset()
        {
            FindTarget();
            Animation ??= new SpriteAnimation(spriteTarget);
            ResetAnimation();
        }
        #endif

        public void FindTarget()
        {
            if (SpriteTarget != null)
            {
                if (animation.spriteTarget != SpriteTarget)
                    animation.SetTarget(SpriteTarget);

                return;
            }

            SpriteTarget = ReactorSpriteTarget.FindTarget(gameObject);
            if (SpriteTarget != null) animation.SetTarget(SpriteTarget);
        }

        protected override void Awake()
        {
            if (!Application.isPlaying) return;
            base.Awake();
            animation.UpdateAnimationSprites();
            FindTarget();
        }

        public override void Play(PlayDirection playDirection) =>
            animation.Play(playDirection);

        public override void Play(bool inReverse = false) =>
            animation.Play(inReverse);

        public override void SetTarget(object target) =>
            SetTarget(target as ReactorSpriteTarget);

        public void SetTarget(ReactorSpriteTarget target) =>
            animation.SetTarget(target);

        public override void ResetToStartValues(bool forced = false) =>
            animation.ResetToStartValues(forced);

        public override void UpdateSettings()
        {
            if (animation.spriteTarget != null) return;
            SetTarget(spriteTarget);
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
            animation.animation.settings.duration = 1f;
        }

    }
}
