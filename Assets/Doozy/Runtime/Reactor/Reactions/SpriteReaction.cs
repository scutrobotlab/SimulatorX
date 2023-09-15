// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Reactor.Easings;
using Doozy.Runtime.Reactor.Internal;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Doozy.Runtime.Reactor.Reactions
{
    [Serializable]
    public class SpriteReaction : DynamicReaction<Sprite, int>
    {
        private List<Sprite> sprites { get; set; } = new List<Sprite>();
        public int firstFrame => 0;
        public int lastFrame => sprites == null || sprites.Count == 0 ? 0 : sprites.Count - 1;
        public int currentFrame => CurrentValue;
        public Sprite current =>
            sprites == null
                ? null
                : sprites.Count == 0
                    ? null
                    : sprites?[Mathf.Clamp(currentFrame, 0, lastFrame)];

        public ReactionCallback<Sprite> OnFrameChangedCallback;

        public SpriteReaction()
        {
            this.SetEase(Ease.Linear);
            FromValue = firstFrame;
            ToValue = lastFrame;
        }

        public SpriteReaction(IEnumerable<Sprite> sprites) : this()
        {
            SetSprites(sprites.ToList());
            FromValue = firstFrame;
            ToValue = lastFrame;
        }

        public override void Reset()
        {
            base.Reset();
            this.SetEase(Ease.Linear);
            OnFrameChangedCallback = null;
        }

        public override float GetProgressAtValue(int value) =>
            Mathf.Clamp01(Mathf.InverseLerp(FromValue, ToValue, value));

        public override void UpdateCurrentValue()
        {
            // CurrentValue = (int)Mathf.Lerp(FromValue, ToValue, easedProgress);
            CurrentValue = (int)Mathf.Lerp(cycleFrom, cycleTo, currentCycleEasedProgress);
            CurrentValue = Mathf.Clamp(CurrentValue, firstFrame, lastFrame);
            setter?.Invoke(current);
            OnValueChangedCallback?.Invoke(CurrentValue);
            OnFrameChangedCallback?.Invoke(current);
        }

        /// <summary> Set the current frame to the given value (frameNumber) </summary>
        /// <param name="value"> Frame Number (value clamped between first and last frames) </param>
        public sealed override Reaction SetValue(int value)
        {
            value = Mathf.Clamp(value, firstFrame, lastFrame);
            base.SetValue(value);
            setter?.Invoke(current);
            return this;
        }

        public override Reaction SetFrom(int value, bool relative = false)
        {
            FromValue = value;
            if (relative) FromValue += CurrentValue;
            FromValue = Mathf.Clamp(FromValue, firstFrame, lastFrame);
            if (isActive) ComputePlayMode();
            return this;
        }

        public override Reaction SetTo(int value, bool relative = false)
        {
            ToValue = value;
            if (relative) ToValue += CurrentValue;
            ToValue = Mathf.Clamp(ToValue, firstFrame, lastFrame);
            if (isActive) ComputePlayMode();
            return this;
        }

        public override void Play(bool inReverse = false)
        {
            FromValue = firstFrame;
            ToValue = lastFrame;
            base.Play(inReverse);
        }

        public override void PlayFromToProgress(float fromProgress, float toProgress)
        {
            FromValue = firstFrame;
            ToValue = lastFrame;
            base.PlayFromToProgress(fromProgress, toProgress);
        }


        public override void PlayToProgress(float toProgress)
        {
            FromValue = firstFrame;
            ToValue = lastFrame;
            base.PlayToProgress(toProgress);
        }

        public override void PlayFromProgress(float fromProgress)
        {
            FromValue = firstFrame;
            ToValue = lastFrame;
            base.PlayFromProgress(fromProgress);
        }

        /// <summary>
        /// Set the sprite at the given frameNumber.
        /// <para/> If the animation is playing, it will stop.
        /// </summary>
        /// <param name="frameNumber"> Value clamped between first and last frames </param>
        public SpriteReaction SetFrame(int frameNumber) =>
            (SpriteReaction)SetValue(frameNumber);


        /// <summary> Order descending the sprites by filename </summary>
        public SpriteReaction ReverseTexturesOrder()
        {
            sprites = sprites.OrderByDescending(t => t.name).ToList();
            setter?.Invoke(current);
            return this;
        }

        /// <summary> Set the sprites for this frame by frame animation reaction </summary>
        /// <param name="spriteList"> Sprites </param>
        /// <param name="setFirstFrame"> Set first frame </param>
        public SpriteReaction SetSprites(List<Sprite> spriteList, bool setFirstFrame = true)
        {
            _ = spriteList ?? throw new ArgumentNullException(nameof(spriteList));
            if (isActive) Stop(true);
            if (sprites == null) sprites = new List<Sprite>();
            sprites.Clear();
            sprites.AddRange(spriteList);
            ToValue = lastFrame;
            if (setFirstFrame)
                SetFirstFrame();
            return this;
        }

        /// <summary>
        /// Set the frame at the given progress value.
        /// <para/> If the animation is playing, it will stop.
        /// </summary>
        /// <param name="targetProgress"> Target progress value </param>
        public SpriteReaction SetFrameAtProgress(float targetProgress) =>
            SetFrame((int)(lastFrame * targetProgress));

        /// <summary> Get the frame number at the given progress value </summary>
        /// <param name="targetProgress"> Target progress value </param>
        public int GetFrameAtProgress(float targetProgress) =>
            Mathf.Clamp((int)(lastFrame * Mathf.Clamp01(targetProgress)), firstFrame, lastFrame);

        /// <summary> Get the Sprite at the given progress value </summary>
        /// <param name="targetProgress"> Target progress value </param>
        public Sprite GetSpriteAtProgress(float targetProgress) =>
            sprites == null || sprites.Count == 0
                ? null
                : sprites[GetFrameAtProgress(targetProgress)];

        /// <summary>
        /// Set the frame at the first frame (progress = 0)
        /// <para/> If the animation is playing, it will stop.
        /// </summary>
        public SpriteReaction SetFirstFrame() =>
            SetFrame(firstFrame);

        /// <summary> Set the frame at the last frame (progress = 1) </summary>
        public SpriteReaction SetLastFrame() =>
            SetFrame(lastFrame);

        /// <summary> Get the progress value for the given frameNumber </summary>
        /// <param name="frameNumber"> Frame number </param>
        public float GetProgressAtFrame(int frameNumber) =>
            (float)Mathf.Clamp(frameNumber, firstFrame, lastFrame) / lastFrame;

        /// <summary> Get the progress value for the current frame </summary>
        public float GetCurrentFrameProgress() =>
            GetProgressAtFrame(currentFrame);

        protected override void ComputeSpring()
        {
            base.ComputeSpring();
            float springForce = settings.strength;
            float forceReduction = springForce / (numberOfCycles - 1);
            for (int i = 0; i < numberOfCycles; i++)
            {
                cycleValues[i] = (int)(FromValue + ToValue * (i % 2 == 0 ? springForce : -springForce * settings.elasticity));
                cycleValues[i] = Mathf.Clamp(cycleValues[i], firstFrame, lastFrame);
                springForce -= forceReduction;
            }
            cycleValues[numberOfCycles - 1] = FromValue;
        }

        protected override void ComputeShake()
        {
            base.ComputeShake();
            for (int i = 0; i < numberOfCycles; i++)
            {
                if (i % 2 == 0)
                {
                    cycleValues[i] = FromValue;
                    continue;
                }
                float random = Random.value;
                cycleValues[i] = (int)(FromValue + ToValue * random * settings.strength);
                cycleValues[i] = Mathf.Clamp(cycleValues[i], firstFrame, lastFrame);
            }
            cycleValues[numberOfCycles - 1] = FromValue;
        }
    }

    public static class SpriteReactionExtensions
    {
        #region getter

        public static T SetGetter<T>(this T target, PropertyGetter<Sprite> getter) where T : SpriteReaction
        {
            target.getter = getter;
            return target;
        }

        public static T ClearGetter<T>(this T target) where T : SpriteReaction =>
            target.SetGetter(null);

        #endregion

        #region setter

        public static T SetSetter<T>(this T target, PropertySetter<Sprite> setter) where T : SpriteReaction
        {
            target.setter = setter;
            return target;
        }

        public static T ClearSetter<T>(this T target) where T : SpriteReaction =>
            target.SetSetter(null);

        #endregion

        #region OnValueChanged

        public static T SetOnValueChangedCallback<T>(this T target, ReactionCallback<int> callback) where T : SpriteReaction
        {
            if (callback == null) return target;
            target.OnValueChangedCallback = callback;
            return target;
        }

        public static T AddOnValueChangedCallback<T>(this T target, ReactionCallback<int> callback) where T : SpriteReaction
        {
            if (callback == null) return target;
            target.OnValueChangedCallback += callback;
            return target;
        }

        public static T ClearOnValueChangedCallback<T>(this T target) where T : SpriteReaction
        {
            target.OnValueChangedCallback = null;
            return target;
        }

        #endregion

        #region OnFrameChanged

        public static T SetOnFrameChangedCallback<T>(this T target, ReactionCallback<Sprite> callback) where T : SpriteReaction
        {
            if (callback == null) return target;
            target.OnFrameChangedCallback = callback;
            return target;
        }

        public static T AddOnFrameChangedCallback<T>(this T target, ReactionCallback<Sprite> callback) where T : SpriteReaction
        {
            if (callback == null) return target;
            target.OnFrameChangedCallback += callback;
            return target;
        }

        public static T ClearOnFrameChangedCallback<T>(this T target) where T : SpriteReaction
        {
            target.OnFrameChangedCallback = null;
            return target;
        }

        #endregion
    }
}
