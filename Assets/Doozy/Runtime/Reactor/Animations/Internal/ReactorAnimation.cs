// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine.Events;
// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Reactor.Animations
{
    [Serializable]
    public abstract class ReactorAnimation
    {
        public UnityEvent OnPlayCallback;
        public UnityEvent OnStopCallback;
        public UnityEvent OnFinishCallback;
        
        public abstract bool isEnabled { get; }
        public abstract bool isIdle { get; }
        public abstract bool isActive{ get; }
        public abstract bool isPaused { get; }
        public abstract bool isPlaying { get; }
        public abstract bool inStartDelay { get; }
        public abstract bool inLoopDelay { get; }

        protected int startedReactionsCount { get; set; }
        protected int stoppedReactionsCount { get; set; }
        protected int finishedReactionsCount { get; set; }
        
        protected bool onPlayInvoked { get; set; }
        
        protected void InvokeOnPlay()
        {
            if (startedReactionsCount <= 0) return;
            if (onPlayInvoked) return;
            OnPlayCallback?.Invoke();
            onPlayInvoked = true;
        }
        
        protected void InvokeOnStop()
        {
            if (startedReactionsCount <= 0) return;
            stoppedReactionsCount++;
            if (stoppedReactionsCount < startedReactionsCount) return;
            OnStopCallback?.Invoke();
        }
        
        protected void InvokeOnFinish()
        {
            if (startedReactionsCount <= 0) return;
            finishedReactionsCount++;
            if (finishedReactionsCount < startedReactionsCount) return;
            OnFinishCallback?.Invoke();
        }

        public abstract void Recycle();

        public abstract void UpdateValues();

        public abstract void StopAllReactionsOnTarget();

        public void SetProgressAtOne() =>
            SetProgressAt(1f);

        public void SetProgressAtZero() =>
            SetProgressAt(0f);


        public virtual void SetProgressAt(float targetProgress)
        {
            StopAllReactionsOnTarget();
            UpdateValues();
        }
        
        public virtual void PlayToProgress(float toProgress)
        {
            StopAllReactionsOnTarget();
            UpdateValues();
            RegisterCallbacks();
        }
        
        public virtual void PlayFromProgress(float fromProgress)
        {
            StopAllReactionsOnTarget();
            UpdateValues();
            RegisterCallbacks();
        }
        
        public virtual void PlayFromToProgress(float fromProgress, float toProgress)
        {
            StopAllReactionsOnTarget();
            UpdateValues();
            RegisterCallbacks();
        }
        
        public void Play(PlayDirection playDirection) =>
            Play(playDirection == PlayDirection.Reverse);

        public abstract void Play(bool inReverse = false);
        public abstract void ResetToStartValues(bool forced = false);
        
        public virtual void Stop()
        {
            UnregisterOnPlayCallbacks();
            UnregisterOnStopCallbacks();
        }
        
        public virtual void Finish()
        {
            UnregisterCallbacks();
        }

        public abstract void Reverse();
        public abstract void Rewind();
        public abstract void Pause();
        public abstract void Resume();
        
        protected virtual void RegisterCallbacks()
        {
            UnregisterCallbacks();

            onPlayInvoked = false;
            startedReactionsCount = 0;
            stoppedReactionsCount = 0;
            finishedReactionsCount = 0;
        }
        
        protected void UnregisterCallbacks()
        {
            UnregisterOnPlayCallbacks();
            UnregisterOnStopCallbacks();
            UnregisterOnFinishCallbacks();
        }

        protected abstract void UnregisterOnPlayCallbacks();
        protected abstract void UnregisterOnStopCallbacks();
        protected abstract void UnregisterOnFinishCallbacks();
    }
}
