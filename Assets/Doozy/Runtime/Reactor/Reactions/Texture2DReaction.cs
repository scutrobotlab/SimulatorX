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
    public class Texture2DReaction : DynamicReaction<Texture2D, int>
    {
        private List<Texture2D> textures { get; set; }
        public int numberOfFrames => textures?.Count ?? 0;
        public int firstFrame => 0;
        public int lastFrame => textures == null ? 0 : numberOfFrames - 1;
        public int currentFrame => CurrentValue;
        public Texture2D current => textures?[Mathf.Clamp(currentFrame, 0, lastFrame)];

        public ReactionCallback<Texture2D> OnFrameChangedCallback;

        public Texture2DReaction()
        {
            this.SetEase(Ease.Linear);
            FromValue = firstFrame;
            ToValue = lastFrame;
        }

        public Texture2DReaction(IEnumerable<Texture2D> textures) : this()
        {
            SetTextures(textures);
            FromValue = firstFrame;
            ToValue = lastFrame;
        }

        public override void Reset()
        {
            base.Reset();
            this.SetEase(Ease.Linear);
            textures ??= new List<Texture2D>();
            textures.Clear();
            textures.Add(null);
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
        /// Set the texture at the given frameNumber.
        /// <para/> If the animation is playing, it will stop.
        /// </summary>
        /// <param name="frameNumber"> Value clamped between first and last frames </param>
        public Texture2DReaction SetFrame(int frameNumber) =>
            (Texture2DReaction)SetValue(frameNumber);


        /// <summary> Order descending the textures by filename </summary>
        public Texture2DReaction ReverseTexturesOrder()
        {
            textures = textures.OrderByDescending(t => t.name).ToList();
            setter?.Invoke(current);
            return this;
        }

        /// <summary> Set the textures for this frame by frame animation reaction </summary>
        /// <param name="textures2D"> Textures </param>
        public Texture2DReaction SetTextures(IEnumerable<Texture2D> textures2D)
        {
            _ = textures2D ?? throw new ArgumentNullException(nameof(textures2D));
            if (isActive) Stop(true);
            textures = textures2D.ToList();
            ToValue = lastFrame;
            SetFirstFrame();
            return this;
        }

        /// <summary>
        /// Set the frame at the given progress value.
        /// <para/> If the animation is playing, it will stop.
        /// </summary>
        /// <param name="targetProgress"> Target progress value </param>
        public Texture2DReaction SetFrameAtProgress(float targetProgress) =>
            SetFrame((int)(lastFrame * targetProgress));

        /// <summary>
        /// Set the frame at the first frame (progress = 0)
        /// <para/> If the animation is playing, it will stop.
        /// </summary>
        public Texture2DReaction SetFirstFrame() =>
            SetFrame(firstFrame);

        /// <summary> Set the frame at the last frame (progress = 1) </summary>
        public Texture2DReaction SetLastFrame() =>
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

    public static class Texture2DReactionExtensions
    {
        #region getter

        public static T SetGetter<T>(this T target, PropertyGetter<Texture2D> getter) where T : Texture2DReaction
        {
            target.getter = getter;
            return target;
        }

        public static T ClearGetter<T>(this T target) where T : Texture2DReaction =>
            target.SetGetter(null);

        #endregion

        #region setter

        public static T SetSetter<T>(this T target, PropertySetter<Texture2D> setter) where T : Texture2DReaction
        {
            target.setter = setter;
            return target;
        }

        public static T ClearSetter<T>(this T target) where T : Texture2DReaction =>
            target.SetSetter(null);

        #endregion

        #region OnValueChanged

        public static T SetOnValueChangedCallback<T>(this T target, ReactionCallback<int> callback) where T : Texture2DReaction
        {
            if (callback == null) return target;
            target.OnValueChangedCallback = callback;
            return target;
        }

        public static T AddOnValueChangedCallback<T>(this T target, ReactionCallback<int> callback) where T : Texture2DReaction
        {
            if (callback == null) return target;
            target.OnValueChangedCallback += callback;
            return target;
        }

        public static T ClearOnValueChangedCallback<T>(this T target) where T : Texture2DReaction
        {
            target.OnValueChangedCallback = null;
            return target;
        }

        #endregion

        #region OnFrameChanged

        public static T SetOnFrameChangedCallback<T>(this T target, ReactionCallback<Texture2D> callback) where T : Texture2DReaction
        {
            if (callback == null) return target;
            target.OnFrameChangedCallback = callback;
            return target;
        }

        public static T AddOnFrameChangedCallback<T>(this T target, ReactionCallback<Texture2D> callback) where T : Texture2DReaction
        {
            if (callback == null) return target;
            target.OnFrameChangedCallback += callback;
            return target;
        }

        public static T ClearOnFrameChangedCallback<T>(this T target) where T : Texture2DReaction
        {
            target.OnFrameChangedCallback = null;
            return target;
        }

        #endregion
    }
}
