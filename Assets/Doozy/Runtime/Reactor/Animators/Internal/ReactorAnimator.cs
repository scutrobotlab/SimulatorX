// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Reactor.Animators.Internal
{
    [Serializable]
    public abstract class ReactorAnimator : MonoBehaviour
    {
        /// <summary> Animator name </summary>
        public string AnimatorName;

        /// <summary> animator behaviour on Start </summary>
        public AnimatorBehaviour OnStartBehaviour = AnimatorBehaviour.Disabled;

        /// <summary> animator behaviour on Enable </summary>
        public AnimatorBehaviour OnEnableBehaviour = AnimatorBehaviour.Disabled;

        protected Coroutine initializeLater { get; set; }
        protected bool animatorInitialized { get; set; }

        protected virtual void Awake()
        {
            if (!Application.isPlaying) return;
            animatorInitialized = false;
        }

        protected virtual void OnEnable()
        {
            if (!Application.isPlaying) return;
            Initialize();
            RunBehaviour(OnEnableBehaviour);
        }

        protected virtual void Start()
        {
            if (!Application.isPlaying) return;
            RunBehaviour(OnStartBehaviour);
        }

        protected virtual void OnDestroy()
        {
            if (!Application.isPlaying) return;
            Recycle();
        }

        protected virtual void Initialize()
        {
            if (animatorInitialized) return;
            if (initializeLater != null)
            {
                StopCoroutine(initializeLater);
                initializeLater = null;
            }
            initializeLater = StartCoroutine(InitializeLater());
        }

        protected IEnumerator InitializeLater()
        {
            yield return new WaitForEndOfFrame();
            InitializeAnimator();
        }

        protected virtual void InitializeAnimator()
        {
            UpdateSettings();
            animatorInitialized = true;
        }

        protected void RunBehaviour(AnimatorBehaviour behaviour)
        {
            if (behaviour == AnimatorBehaviour.Disabled)
                return;

            if (!animatorInitialized)
            {
                DelayExecution(() => RunBehaviour(behaviour));
                return;
            }

            switch (behaviour)
            {
                case AnimatorBehaviour.PlayForward:
                    Play(PlayDirection.Forward);
                    return;

                case AnimatorBehaviour.PlayReverse:
                    Play(PlayDirection.Reverse);
                    return;

                case AnimatorBehaviour.SetFromValue:
                    SetProgressAtZero();
                    return;

                case AnimatorBehaviour.SetToValue:
                    SetProgressAtOne();
                    return;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected void DelayExecution(UnityAction action) =>
            StartCoroutine(ExecuteAfterAnimatorInitialized(action));

        protected IEnumerator ExecuteAfterAnimatorInitialized(UnityAction action)
        {
            yield return new WaitUntil(() => animatorInitialized);
            action?.Invoke();
        }

        public abstract void Play(bool inReverse = false);
        public abstract void Play(PlayDirection playDirection);

        public abstract void SetTarget(object target);
        public abstract void ResetToStartValues(bool forced = false);
        public abstract void UpdateSettings();
        public abstract void UpdateValues();

        public abstract void PlayToProgress(float toProgress);
        public abstract void PlayFromProgress(float fromProgress);
        public abstract void PlayFromToProgress(float fromProgress, float toProgress);

        public abstract void Stop();
        public abstract void Finish();
        public abstract void Reverse();
        public abstract void Rewind();
        public abstract void Pause();
        public abstract void Resume();

        public abstract void SetProgressAtOne();
        public abstract void SetProgressAtZero();
        public abstract void SetProgressAt(float targetProgress);
        protected abstract void Recycle();
    }
}
