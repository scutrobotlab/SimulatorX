// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Animations
{
    [Serializable]
    public class UIAnimation : ReactorAnimation
    {
        public RectTransform rectTransform
        {
            get;
            internal set;
        }
        public CanvasGroup canvasGroup
        {
            get;
            internal set;
        }

        [SerializeField] private UIAnimationType AnimationType;
        public UIAnimationType animationType
        {
            get => AnimationType;
            set
            {
                AnimationType = value;
                Move.animationType = value;
            }
        }

        public UIMoveReaction Move;
        public UIRotateReaction Rotate;
        public UIScaleReaction Scale;
        public UIFadeReaction Fade;

        /// <summary> Move start position value (RectTransform.anchoredPosition3D) </summary>
        public Vector3 startPosition
        {
            get => Move.startPosition;
            set => Move.startPosition = value;
        }

        /// <summary> Rotate start rotation value (RectTransform.localEulerAngles) </summary>
        public Vector3 startRotation
        {
            get => Rotate.startRotation;
            set => Rotate.startRotation = value;
        }

        /// <summary> Scale start scale value (RectTransform.localScale) </summary>
        public Vector3 startScale
        {
            get => Scale.startScale;
            set => Scale.startScale = value;
        }

        /// <summary> Fade start alpha value (CanvasGroup.alpha) </summary>
        public float startAlpha
        {
            get => Fade.startAlpha;
            set => Fade.startAlpha = value;
        }

        public override bool isEnabled => Move.enabled | Rotate.enabled | Scale.enabled | Fade.enabled;
        public override bool isIdle => Move.isIdle | Rotate.isIdle | Scale.isIdle | Fade.isIdle;
        public override bool isActive => Move.isActive | Rotate.isActive | Scale.isActive | Fade.isActive;
        public override bool isPaused => Move.isPaused | Rotate.isPaused | Scale.isPaused | Fade.isPaused;
        public override bool isPlaying => Move.isPlaying || Rotate.isPlaying || Scale.isPlaying || Fade.isPlaying;
        public override bool inStartDelay => Move.inStartDelay | Rotate.inStartDelay | Scale.inStartDelay | Fade.inStartDelay;
        public override bool inLoopDelay => Move.inLoopDelay | Rotate.inLoopDelay | Scale.inLoopDelay | Fade.inLoopDelay;

        public UIAnimation(RectTransform targetRectTransform, CanvasGroup targetCanvasGroup = null) =>
            SetTarget(targetRectTransform, targetCanvasGroup);

        public void SetTarget(RectTransform targetRectTransform, CanvasGroup targetCanvasGroup = null)
        {
            rectTransform = null;
            canvasGroup = null;

            _ = targetRectTransform ? targetRectTransform : throw new NullReferenceException(nameof(targetRectTransform));
            rectTransform = targetRectTransform;
            if (targetCanvasGroup == null) targetCanvasGroup = targetRectTransform.gameObject.GetComponent<CanvasGroup>();
            canvasGroup = targetCanvasGroup == null ? targetRectTransform.gameObject.AddComponent<CanvasGroup>() : targetCanvasGroup;

            Initialize();
        }

        public void Initialize()
        {
            Move?.Stop(true);
            Move ??= Reaction.Get<UIMoveReaction>();
            Move.SetTarget(rectTransform);
            Move.animationType = animationType;

            Rotate?.Stop(true);
            Rotate ??= Reaction.Get<UIRotateReaction>();
            Rotate.SetTarget(rectTransform);

            Scale?.Stop(true);
            Scale ??= Reaction.Get<UIScaleReaction>();
            Scale.SetTarget(rectTransform);

            Fade?.Stop(true);
            Fade ??= Reaction.Get<UIFadeReaction>();
            Fade.SetTarget(rectTransform, canvasGroup);

            UpdateValues();
        }

        public override void Recycle()
        {
            Move?.Recycle();
            Rotate?.Recycle();
            Scale?.Recycle();
            Fade?.Recycle();
        }

        public override void UpdateValues()
        {
            if (canvasGroup != null)
                Fade.UpdateValues(); //calculate fade
            Scale.UpdateValues();    //calculate scale
            Rotate.UpdateValues();   // calculate rotation

            //update move settings after calculating scale
            {
                Move.UseCustomLocalScale = Scale.enabled;
                Move.CustomFromLocalScale = Scale.enabled ? Scale.fromValue : startScale;
                Move.CustomToLocalScale = Scale.enabled ? Scale.toValue : startScale;
            }

            //update move settings after rotation
            {
                Move.UseCustomLocalRotation = Rotate.enabled;
                Move.CustomFromLocalRotation = Rotate.enabled ? Rotate.fromValue : startRotation;
                Move.CustomToLocalRotation = Rotate.enabled ? Rotate.toValue : startRotation;
            }

            Move.animationType = animationType; //enforce animation type
            Move.UpdateValues();                //calculate position
        }

        public override void StopAllReactionsOnTarget() =>
            Reaction.StopAllReactionsByTargetObject(rectTransform, true);

        public override void SetProgressAt(float targetProgress)
        {
            base.SetProgressAt(targetProgress);
            if (Fade.enabled) Fade.SetProgressAt(targetProgress);
            if (Scale.enabled) Scale.SetProgressAt(targetProgress);
            if (Rotate.enabled) Rotate.SetProgressAt(targetProgress);
            if (Move.enabled) Move.SetProgressAt(targetProgress);

            if (animationType != UIAnimationType.Custom)
                ResetToStartValues();
        }

        public override void PlayToProgress(float toProgress)
        {
            base.PlayToProgress(toProgress);
            if (Fade.enabled) Fade.PlayToProgress(toProgress);
            if (Scale.enabled) Scale.PlayToProgress(toProgress);
            if (Rotate.enabled) Rotate.PlayToProgress(toProgress);
            if (Move.enabled) Move.PlayToProgress(toProgress);

            if (animationType != UIAnimationType.Custom)
                ResetToStartValues();
        }

        public override void PlayFromProgress(float fromProgress)
        {
            base.PlayFromProgress(fromProgress);
            if (Move.enabled) Move.PlayFromProgress(fromProgress);
            if (Rotate.enabled) Rotate.PlayFromProgress(fromProgress);
            if (Scale.enabled) Scale.PlayFromProgress(fromProgress);
            if (Fade.enabled) Fade.PlayFromProgress(fromProgress);

            if (animationType != UIAnimationType.Custom)
                ResetToStartValues();
        }

        public override void PlayFromToProgress(float fromProgress, float toProgress)
        {
            base.PlayFromToProgress(fromProgress, toProgress);
            if (Move.enabled) Move.PlayFromToProgress(fromProgress, toProgress);
            if (Rotate.enabled) Rotate.PlayFromToProgress(fromProgress, toProgress);
            if (Scale.enabled) Scale.PlayFromToProgress(fromProgress, toProgress);
            if (Fade.enabled) Fade.PlayFromToProgress(fromProgress, toProgress);

            if (animationType != UIAnimationType.Custom)
                ResetToStartValues();
        }

        public override void Play(bool inReverse = false)
        {
            if (rectTransform == null)
                return;

            RegisterCallbacks();
            if (!isActive)
            {
                StopAllReactionsOnTarget();
                ResetToStartValues();
            }

            if (Move.enabled) Move.Play(inReverse);
            if (Rotate.enabled) Rotate.Play(inReverse);
            if (Scale.enabled) Scale.Play(inReverse);
            if (Fade.enabled) Fade.Play(inReverse);

            // if (!isPlaying && animationType != UIAnimationType.Custom)
            //     ResetToStartValues();
        }

        public override void ResetToStartValues(bool forced = false)
        {
            if (forced | !Move.enabled) Move.SetValue(startPosition);
            if (forced | !Rotate.enabled) Rotate.SetValue(startRotation);
            if (forced | !Scale.enabled) Scale.SetValue(startScale);
            if (forced | !Fade.enabled) Fade.SetValue(startAlpha);
        }

        public override void Stop()
        {
            if (Move.isActive || Move.enabled) Move.Stop();
            if (Rotate.isActive || Rotate.enabled) Rotate.Stop();
            if (Scale.isActive || Scale.enabled) Scale.Stop();
            if (Fade.isActive || Fade.enabled) Fade.Stop();
            base.Stop();
        }

        public override void Finish()
        {
            if (Move.isActive || Move.enabled) Move.Finish();
            if (Rotate.isActive || Rotate.enabled) Rotate.Finish();
            if (Scale.isActive || Scale.enabled) Scale.Finish();
            if (Fade.isActive || Fade.enabled) Fade.Finish();
            base.Finish();
        }

        public override void Reverse()
        {
            if (Move.isActive) Move.Reverse();
            else if (Move.enabled) Move.Play(PlayDirection.Reverse);

            if (Rotate.isActive) Rotate.Reverse();
            else if (Rotate.enabled) Rotate.Play(PlayDirection.Reverse);

            if (Scale.isActive) Scale.Reverse();
            else if (Scale.enabled) Scale.Play(PlayDirection.Reverse);

            if (Fade.isActive) Fade.Reverse();
            else if (Fade.enabled) Fade.Play(PlayDirection.Reverse);
        }

        public override void Rewind()
        {
            if (Move.enabled) Move.Rewind();
            if (Rotate.enabled) Rotate.Rewind();
            if (Scale.enabled) Scale.Rewind();
            if (Fade.enabled) Fade.Rewind();
        }

        public override void Pause()
        {
            Move.Pause();
            Rotate.Pause();
            Scale.Pause();
            Fade.Pause();
        }

        public override void Resume()
        {
            Move.Resume();
            Rotate.Resume();
            Scale.Resume();
            Fade.Resume();
        }

        protected override void RegisterCallbacks()
        {
            base.RegisterCallbacks();

            if (Move.enabled)
            {
                startedReactionsCount++;
                Move.OnPlayCallback += InvokeOnPlay;
                Move.OnStopCallback += InvokeOnStop;
                Move.OnFinishCallback += InvokeOnFinish;
            }

            if (Rotate.enabled)
            {
                startedReactionsCount++;
                Rotate.OnPlayCallback += InvokeOnPlay;
                Rotate.OnStopCallback += InvokeOnStop;
                Rotate.OnFinishCallback += InvokeOnFinish;
            }

            if (Scale.enabled)
            {
                startedReactionsCount++;
                Scale.OnPlayCallback += InvokeOnPlay;
                Scale.OnStopCallback += InvokeOnStop;
                Scale.OnFinishCallback += InvokeOnFinish;
            }

            if (Fade.enabled)
            {
                startedReactionsCount++;
                Fade.OnPlayCallback += InvokeOnPlay;
                Fade.OnStopCallback += InvokeOnStop;
                Fade.OnFinishCallback += InvokeOnFinish;
            }
        }

        protected override void UnregisterOnPlayCallbacks()
        {
            if (Move.enabled) Move.OnPlayCallback -= InvokeOnPlay;
            if (Rotate.enabled) Rotate.OnPlayCallback -= InvokeOnPlay;
            if (Scale.enabled) Scale.OnPlayCallback -= InvokeOnPlay;
            if (Fade.enabled) Fade.OnPlayCallback -= InvokeOnPlay;
        }

        protected override void UnregisterOnStopCallbacks()
        {
            if (Move.enabled) Move.OnStopCallback -= InvokeOnStop;
            if (Rotate.enabled) Rotate.OnStopCallback -= InvokeOnStop;
            if (Scale.enabled) Scale.OnStopCallback -= InvokeOnStop;
            if (Fade.enabled) Fade.OnStopCallback -= InvokeOnStop;
        }

        protected override void UnregisterOnFinishCallbacks()
        {
            if (Move.enabled) Move.OnFinishCallback -= InvokeOnFinish;
            if (Rotate.enabled) Rotate.OnFinishCallback -= InvokeOnFinish;
            if (Scale.enabled) Scale.OnFinishCallback -= InvokeOnFinish;
            if (Fade.enabled) Fade.OnFinishCallback -= InvokeOnFinish;
        }
    }
}
