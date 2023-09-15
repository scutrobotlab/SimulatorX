// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Linq;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Easings;
using Doozy.Runtime.Reactor.Ticker;
using UnityEngine;
using static UnityEngine.Mathf;

namespace Doozy.Runtime.Reactor.Internal
{
    [Serializable]
    public abstract class Reaction
    {
        #region Reaction State

        /// <summary> Reaction current state </summary>
        public ReactionState state { get; internal set; }

        /// <summary> Reaction state before it was paused (internal use only) </summary>
        public ReactionState stateBeforePause { get; internal set; }

        /// <summary> Reaction is in the Pool </summary>
        public bool isPooled => state == ReactionState.Pooled;

        /// <summary> Reaction is ready to run </summary>
        public bool isIdle => state == ReactionState.Idle;

        /// <summary> Reaction is running (is not in the pool and is not idle) </summary>
        public bool isActive => !isPooled & !isIdle;

        /// <summary> Reaction is running, but it is paused </summary>
        public bool isPaused => state == ReactionState.Paused;

        /// <summary> Reaction is running and is playing the animation </summary>
        public bool isPlaying => state == ReactionState.Playing;

        /// <summary> Reaction is running and is waiting to start playing the animation </summary>
        public bool inStartDelay => state == ReactionState.StartDelay;

        /// <summary> Reaction is running and is waiting the start playing the next loop </summary>
        public bool inLoopDelay => state == ReactionState.LoopDelay;

        #endregion

        /// <summary> Reaction settings </summary>
        [SerializeField] private ReactionSettings Settings;

        /// <summary> Reaction settings </summary>
        public ReactionSettings settings
        {
            get => Settings;
            internal set => Settings = value;
        }

        private float m_LastProgress;
        /// <summary> Reaction calculated progress = elapsedDuration / duration </summary>
        public float progress => m_LastProgress = Clamp01((float)(elapsedDuration / duration));

        /// <summary> Reaction calculated eased progress. Progress value with the ease modifier applied </summary>
        public float easedProgress => Settings.CalculateEasedProgress(progress);

        /// <summary>
        /// Reaction current play direction
        /// <para/> Forward - progress goes from 0 to 1
        /// <para/> Reverse - progress goes from 1 to 0
        /// </summary>
        public PlayDirection direction { get; internal set; }

        /// <summary> Reaction heartbeat (ticker) </summary>
        public Heartbeat heartbeat { get; private set; }

        /// <summary> Current start delay </summary>
        public float startDelay { get; internal set; }
        /// <summary> Current elapsed start delay </summary>
        public double elapsedStartDelay { get; private set; }
        /// <summary>
        /// Special constant used to simulate zero duration. It is needed because we calculate progress = elapsedDuration / duration.
        /// If duration becomes zero, we get NaN (Not a Number) error. Miau!
        /// </summary>
        private const float MIN_DURATION = 0.0001f;
        /// <summary> Current duration </summary>
        public float duration { get; internal set; }
        /// <summary> Current elapsed duration </summary>
        public double elapsedDuration { get; protected set; }
        /// <summary> Current start duration (needed to reverse the reaction flow) </summary>
        protected float startDuration { get; set; }
        /// <summary> Current target duration (needed to reverse the reaction flow) </summary>
        protected float targetDuration { get; set; }
        /// <summary> Flag used to mark that the reaction is not playing from start to finish, but from a FROM progress value to a TO progress value </summary>
        protected bool customStartDuration { get; set; }
        /// <summary> Current loops count </summary>
        public int loops { get; internal set; }
        /// <summary> Current elapsed loops count </summary>
        public int elapsedLoops { get; private set; }
        /// <summary> Current loop delay (delay between loops) </summary>
        public float loopDelay { get; internal set; }
        /// <summary> Current elapsed loop delay (elapsed delay between loops) </summary>
        public double elapsedLoopDelay { get; private set; }

        /// <summary> Callback invoked when the Reaction starts playing </summary>
        public ReactionCallback OnPlayCallback;
        /// <summary> Callback invoked when the Reaction stops playing </summary>
        public ReactionCallback OnStopCallback;
        /// <summary> Callback invoked when the Reaction finished playing </summary>
        public ReactionCallback OnFinishCallback;
        /// <summary> Callback invoked when the Reaction finished playing a loop (invoked every loop) </summary>
        public ReactionCallback OnLoopCallback;
        /// <summary> Callback invoked when the Reaction was paused (the Reaction is still running) </summary>
        public ReactionCallback OnPauseCallback;
        /// <summary> Callback invoked when the Reaction resumed playing </summary>
        public ReactionCallback OnResumeCallback;
        /// <summary> Callback invoked when the Reaction updates </summary>
        public ReactionCallback OnUpdateCallback;

        #region Cycle Variables

        protected float currentCycleEasedProgress => Settings.CalculateEasedProgress(currentCycleProgress);
        protected float[] cycleDurations { get; set; }
        protected int numberOfCycles { get; set; }
        protected int previousCycleIndex { get; set; }
        protected int currentCycleIndex { get; set; }
        protected float currentCycleDuration
        {
            get
            {
                if (cycleDurations == null || currentCycleIndex != cycleDurations.Length)
                    ComputePlayMode();
                return cycleDurations[currentCycleIndex];
            }
        }
        protected float currentCycleElapsedDuration
        {
            get
            {
                if (currentCycleIndex == 0) return (float)elapsedDuration;
                float cycleStartDuration = cycleDurations.TakeWhile((t, i) => currentCycleIndex != i).Sum();
                return Clamp((float)(elapsedDuration - cycleStartDuration), 0, targetDuration);
            }
        }

        protected float currentCycleProgress
        {
            get
            {
                float cycleProgress = Clamp01(currentCycleElapsedDuration / currentCycleDuration);
                return Approximately(0, cycleProgress) ? 0f : Approximately(cycleProgress, 1f) ? 1f : cycleProgress;
            }
        }

        #endregion

        /// <summary> Reset all callbacks </summary>
        public void ResetCallbacks()
        {
            OnUpdateCallback = null;
            OnPlayCallback = null;
            OnStopCallback = null;
            OnFinishCallback = null;
            OnLoopCallback = null;
            OnPauseCallback = null;
            OnResumeCallback = null;
        }

        protected Reaction()
        {
            Settings = new ReactionSettings();
            this.SetRuntimeHeartbeat();
        }

        /// <summary> Reset the reaction </summary>
        public virtual void Reset()
        {
            if (isActive) Stop(true);
            ClearIds();
            this.ClearCallbacks();
            Settings ??= new ReactionSettings();
            Settings.Reset();
        }

        /// <summary>
        /// Clear the reaction's ids
        /// </summary>
        private void ClearIds()
        {
            objectId = null;
            stringId = null;
            intId = k_DefaultIntId;
            targetObject = null;
        }

        /// <summary> Reverse the reaction's play direction (works if the reaction is running) </summary>
        public void Reverse()
        {
            if (!isActive) return;
            if (inStartDelay)
            {
                Stop();
                return;
            }
            direction = (PlayDirection)((int)direction * -1f);
        }

        /// <summary> Rewind the reaction to the start (works in both play directions) </summary>
        public void Rewind()
        {
            elapsedDuration = direction == PlayDirection.Forward ? 0f : targetDuration;
        }

        /// <summary> Pause the reaction (if it's active) </summary>
        /// <param name="silent"> If TRUE, callbacks will not be invoked </param>
        public void Pause(bool silent = false)
        {
            if (!isActive) return;
            stateBeforePause = state;
            state = ReactionState.Paused;
            if (!silent) OnPauseCallback?.Invoke();
        }

        /// <summary> Resume the reaction (if it's paused) </summary>
        /// <param name="silent"> If TRUE, callbacks will not be invoked </param>
        public void Resume(bool silent = false)
        {
            if (!isPaused) return;
            state = stateBeforePause;
            if (isActive & !heartbeat.isActive)
                heartbeat.RegisterToTickService();
            if (!silent) OnResumeCallback?.Invoke();
        }

        /// <summary> Play the reaction, in the given direction </summary>
        /// <param name="playDirection"> Play direction </param>
        public void Play(PlayDirection playDirection) =>
            Play(playDirection == PlayDirection.Reverse);

        /// <summary> Play the reaction </summary>
        /// <param name="inReverse"> If TRUE, the reaction will play in reverse </param>
        public virtual void Play(bool inReverse = false)
        {
            if (isActive)
            {
                switch (direction)
                {
                    case PlayDirection.Forward:
                        if (inReverse)
                        {
                            if (inStartDelay)
                            {
                                Stop();
                                return;
                            }
                            Reverse();
                            return;
                        }
                        break;
                    case PlayDirection.Reverse:
                        if (!inReverse)
                        {
                            if (inStartDelay)
                            {
                                Stop();
                                return;
                            }
                            Reverse();
                            return;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (isActive) Stop(true);
            ResetElapsedValues();
            RefreshSettings();

            direction = inReverse ? PlayDirection.Reverse : PlayDirection.Forward;

            customStartDuration = false;
            startDuration = 0f;
            targetDuration = duration;

            elapsedDuration = direction == PlayDirection.Forward ? startDuration : targetDuration;
            m_LastProgress = progress;

            ComputePlayMode();
            OnPlayCallback?.Invoke();

            if (startDelay <= 0 & duration <= MIN_DURATION)
            {
                elapsedDuration = direction == PlayDirection.Forward ? targetDuration : startDuration;
                m_LastProgress = progress;
                UpdateReaction();
                return;
            }

            state = startDelay > 0 & direction == PlayDirection.Forward ? ReactionState.StartDelay : ReactionState.Playing;
            heartbeat.RegisterToTickService();
        }

        /// <summary> Play the reaction from the given start progress (from) to the given end progress (to) </summary>
        /// <param name="fromProgress"> From (start) progress </param>
        /// <param name="toProgress"> To (end) progress </param>
        public virtual void PlayFromToProgress(float fromProgress, float toProgress)
        {
            if (isActive) Stop(true);
            ResetElapsedValues();
            RefreshSettings();

            direction = fromProgress <= toProgress ? PlayDirection.Forward : PlayDirection.Reverse;

            customStartDuration = true;
            
            float fromDuration = GetDurationAtProgress(fromProgress, duration);
            float toDuration = GetDurationAtProgress(toProgress, duration);
            
            startDuration = direction == PlayDirection.Forward ? fromDuration : toDuration;
            targetDuration = direction == PlayDirection.Forward ? toDuration : fromDuration;;
            
            elapsedDuration = direction == PlayDirection.Forward ? startDuration : targetDuration;
            m_LastProgress = progress;

            ComputePlayMode();
            OnPlayCallback?.Invoke();

            if (duration <= MIN_DURATION)
            {
                elapsedDuration = direction == PlayDirection.Forward ? targetDuration : startDuration;
                m_LastProgress = progress;
                UpdateReaction();
                return;
            }

            state = ReactionState.Playing;
            heartbeat.RegisterToTickService();
        }

        /// <summary> Play the reaction from the current progress to the given end progress (to) </summary>
        /// <param name="toProgress"> To (end) progress </param>
        public virtual void PlayToProgress(float toProgress) =>
            PlayFromToProgress(m_LastProgress, toProgress);

        /// <summary> Play the reaction from the given start progress (from) to the current progress </summary>
        /// <param name="fromProgress"> From (start) progress </param>
        public virtual void PlayFromProgress(float fromProgress) =>
            PlayFromToProgress(fromProgress, m_LastProgress);

        /// <summary> Get the elapsedDuration value at the given target progress </summary>
        /// <param name="targetProgress"> Target progress </param>
        /// <param name="totalDuration"> Duration </param>
        protected float GetDurationAtProgress(float targetProgress, float totalDuration)
        {
            targetProgress = Clamp01(targetProgress);
            totalDuration = Max(0, totalDuration);
            totalDuration = totalDuration == 0 ? 1 : totalDuration;
            return Clamp(totalDuration * targetProgress, 0, totalDuration).Round(4);
        }

        /// <summary> Set the reaction's progress at the given target progress </summary>
        /// <param name="targetProgress"> Target progress </param>
        public virtual void SetProgressAt(float targetProgress)
        {
            if (isActive) Stop(true);
            ResetElapsedValues();
            RefreshSettings();

            direction = PlayDirection.Forward;
            // startDuration = 0f;
            // targetDuration = 1f;

            //save current settings
            EaseMode easeMode = settings.easeMode;
            Ease ease = settings.ease;
            AnimationCurve curve = settings.curve;

            //apply linear lease
            settings.easeMode = EaseMode.Ease;
            settings.ease = Ease.Linear;

            //update the progress
            // elapsedDuration = GetDurationAtProgress(targetProgress, targetDuration);
            elapsedDuration = Clamp01(targetProgress) * duration;
            // elapsedDuration = Clamp01((float)elapsedDuration);
            m_LastProgress = progress;

            if (heartbeat.isActive) heartbeat.UnregisterFromTickService();

            if (settings.playMode != PlayMode.Normal)
                ComputePlayMode(); //This operation needed here because if we set the progress before the reaction ran, we have no values and can get NaN (Not a Number) values
            UpdateCurrentCycleIndex();
            UpdateCurrentValue();
            OnUpdateCallback?.Invoke();

            //restore previous settings
            settings.ease = ease;
            settings.curve = curve;
            settings.easeMode = easeMode;
        }

        /// <summary> Set the reaction's progress at 1 (end) </summary>
        public void SetProgressAtOne() =>
            SetProgressAt(1f);

        /// <summary> Set the reaction's progress at 0 (start) </summary>
        public void SetProgressAtZero() =>
            SetProgressAt(0f);

        /// <summary>
        /// Update the reaction (called by the reaction's Heartbeat to update all the values)
        /// </summary>
        internal void UpdateReaction()
        {
            if (isPooled)
            {
                if (heartbeat.isActive)
                {
                    heartbeat.UnregisterFromTickService();
                }
                return;
            }

            if (isIdle & heartbeat.isActive)
            {
                heartbeat.UnregisterFromTickService();
            }

            if (IsPaused()) return;
            if (InStartDelay()) return;
            if (InLoopDelay()) return;

            elapsedDuration = elapsedDuration < 0f ? 0f : elapsedDuration;
            elapsedDuration = elapsedDuration > duration ? duration : elapsedDuration;
            m_LastProgress = progress;

            UpdateCurrentCycleIndex();
            UpdateCurrentValue();
            OnUpdateCallback?.Invoke();

            switch (direction)
            {
                case PlayDirection.Forward:
                    if (elapsedDuration < targetDuration)
                    {
                        elapsedDuration += heartbeat.deltaTime * (int)direction;
                        return;
                    }
                    break;
                case PlayDirection.Reverse:
                    if (elapsedDuration > startDuration)
                    {
                        elapsedDuration += heartbeat.deltaTime * (int)direction;
                        return;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            elapsedLoops++;

            if (loops < 0 || loops != 0 && elapsedLoops <= loops)
            {
                if (!customStartDuration)
                {
                    duration = Max(MIN_DURATION, Settings.GetDuration());
                    startDuration = 0f;
                    targetDuration = duration;
                    ComputePlayMode();
                }
                elapsedDuration = direction == PlayDirection.Forward ? startDuration : targetDuration;
                m_LastProgress = progress;

                loopDelay = Settings.GetLoopDelay();

                if (loopDelay > 0)
                {
                    state = ReactionState.LoopDelay;
                    return;
                }

                OnLoopCallback?.Invoke();
                state = ReactionState.Playing;
                return;
            }

            elapsedDuration = direction == PlayDirection.Forward ? targetDuration : startDuration;
            m_LastProgress = progress;
            UpdateCurrentCycleIndex();
            UpdateCurrentValue();
            OnUpdateCallback?.Invoke();
            Finish();
        }

        /// <summary> Returns TRUE if the reaction is paused and updates the lastUpdateTime for the heartbeat </summary>
        private bool IsPaused()
        {
            if (!isPaused) return false;
            heartbeat.lastUpdateTime = heartbeat.timeSinceStartup;
            return true;
        }

        /// <summary> Returns TRUE if the reaction is in start delay and updates the start delay related variables </summary>
        private bool InStartDelay()
        {
            if (!inStartDelay) return false;
            elapsedStartDelay += heartbeat.deltaTime;
            elapsedStartDelay = Clamp((float)elapsedStartDelay, 0, startDelay);
            if (startDelay - elapsedStartDelay > 0) return true;
            state = ReactionState.Playing;
            elapsedStartDelay = 0f;
            return false;
        }

        /// <summary> Returns TRUE if the reaction is in loop delay and updates the loop delay related variables </summary>
        private bool InLoopDelay()
        {
            if (!inLoopDelay) return false;
            elapsedLoopDelay += heartbeat.deltaTime;
            elapsedLoopDelay = Clamp((float)elapsedLoopDelay, 0, loopDelay);
            if (loopDelay - elapsedLoopDelay > 0f) return true;
            OnLoopCallback?.Invoke();
            state = ReactionState.Playing;
            elapsedLoopDelay = 0f;
            return false;
        }

        /// <summary> Update the reaction's current value </summary>
        public abstract void UpdateCurrentValue();

        /// <summary> Stop the reaction from playing (does not call finish) </summary>
        /// <param name="silent"> If TRUE, callbacks will not be invoked </param>
        /// <param name="recycle"> If TRUE, it will try recycle this reaction, by returning it to the pool </param>
        public virtual void Stop(bool silent = false, bool recycle = false)
        {
            if (heartbeat.isActive) heartbeat.UnregisterFromTickService();
            if (isPooled) return;
            if (!silent) OnStopCallback?.Invoke();
            state = ReactionState.Idle;
            if (recycle) Recycle();
        }

        /// <summary> Finish the reaction by stopping it, calling callbacks (stop and then finish) and then (if reusable) returns it to the pool </summary>
        /// <param name="silent"> If TRUE, callbacks will not be invoked </param>
        /// <param name="endAnimation"> If TRUE, the animation ends in the To value (set the progress to 1 (one)) </param>
        /// <param name="recycle"> If TRUE, it will try recycle this reaction, by returning it to the pool </param>
        public virtual void Finish(bool silent = false, bool endAnimation = false, bool recycle = false)
        {
            if (!isActive) return;
            // ReSharper disable once RedundantArgumentDefaultValue
            Stop(silent, false);
            if (!silent) OnFinishCallback?.Invoke();
            if (endAnimation) SetProgressAtOne();
            if (recycle) Recycle();
        }

        /// <summary> Set the heartbeat for this reaction and connect the UpdateReaction to it </summary>
        /// <param name="h"> New heartbeat </param>
        public void SetHeartbeat(Heartbeat h)
        {
            heartbeat = h ?? new RuntimeHeartbeat();
            heartbeat.onTickCallback = UpdateReaction;
        }

        /// <summary>
        /// Reset all relevant elapsed values
        /// <para/> Used mostly for reset purposes
        /// </summary>
        private void ResetElapsedValues()
        {
            elapsedStartDelay = 0;
            elapsedDuration = 0;
            elapsedLoops = 0;
            elapsedLoopDelay = 0;
        }

        /// <summary>
        /// Refresh the reaction's current play settings.
        /// <para/> Refreshes the play settings (gets new random values if they are used)
        /// <para/> Calls the appropriate Compute method for the current play mode 
        /// </summary>
        public void RefreshSettings()
        {
            settings.Validate();

            startDelay = Settings.GetStartDelay();
            duration = Max(MIN_DURATION, Settings.GetDuration()); //avoid zero duration as it creates NaN values
            loops = Settings.GetLoops();
            loopDelay = Settings.GetLoopDelay();

            ComputePlayMode();
        }

        /// <summary> Call the appropriate Compute method depending on the current play mode </summary>
        public void ComputePlayMode()
        {
            switch (Settings.playMode)
            {
                case PlayMode.Normal:
                    ComputeNormal();
                    break;
                case PlayMode.PingPong:
                    ComputePingPong();
                    break;
                case PlayMode.Spring:
                    ComputeSpring();
                    break;
                case PlayMode.Shake:
                    ComputeShake();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary> Update the current cycle index </summary>
        private void UpdateCurrentCycleIndex()
        {
            previousCycleIndex = currentCycleIndex;

            switch (direction)
            {
                case PlayDirection.Forward:
                {
                    float compoundDuration = 0f;
                    for (int i = 0; i < cycleDurations.Length; i++)
                    {
                        currentCycleIndex = i;
                        compoundDuration += cycleDurations[i];
                        if (elapsedDuration <= compoundDuration) return;
                    }
                }
                    break;
                case PlayDirection.Reverse:
                {
                    // float compoundDuration = targetDuration;
                    float compoundDuration = duration;
                    for (int i = cycleDurations.Length - 1; i >= 0; i--)
                    {
                        currentCycleIndex = i;
                        compoundDuration -= cycleDurations[i];
                        if (elapsedDuration > compoundDuration) return;
                    }
                }
                    break;
            }
        }

        /// <summary> Compute normal play mode cycle </summary>
        protected virtual void ComputeNormal()
        {
            currentCycleIndex = 0;
            numberOfCycles = 1;
            cycleDurations = new[] { duration };
        }

        /// <summary> Compute ping-pong play mode cycles </summary>
        protected virtual void ComputePingPong()
        {
            currentCycleIndex = 0;
            numberOfCycles = 2;
            float halfDuration = duration / 2f;
            cycleDurations = new[] { halfDuration, halfDuration };
        }

        /// <summary> Compute spring play mode cycles </summary>
        protected virtual void ComputeSpring()
        {
            currentCycleIndex = 0;
            numberOfCycles = Max(1, settings.vibration + (int)(settings.vibration * duration));
            if (numberOfCycles % 2 != 0) numberOfCycles++;
            cycleDurations = new float[numberOfCycles];

            float compoundDuration = 0f;
            for (int i = 0; i < numberOfCycles; i++)
            {
                cycleDurations[i] = duration * ((float)(i + 1) / numberOfCycles);
                cycleDurations[i] = cycleDurations[i].Round(4);
                compoundDuration += cycleDurations[i];
            }

            float durationRatio = duration / compoundDuration;
            for (int i = 0; i < numberOfCycles; i++)
                cycleDurations[i] *= durationRatio;
        }

        /// <summary> Compute shake play mode cycles </summary>
        protected virtual void ComputeShake()
        {
            currentCycleIndex = 0;
            numberOfCycles = Max(1, settings.vibration + (int)(settings.vibration * duration));
            if (numberOfCycles % 2 == 0) numberOfCycles++;
            cycleDurations = new float[numberOfCycles];

            float compoundDuration = 0f;
            for (int i = 0; i < numberOfCycles; i++)
            {
                if (settings.fadeOutShake)
                {
                    float ofCycles = ((float)(i + 1) / numberOfCycles);
                    cycleDurations[i] = EaseFactory.GetEase(Ease.OutExpo).Evaluate(ofCycles) * duration;
                }
                else
                {
                    cycleDurations[i] = duration / numberOfCycles;
                }

                compoundDuration += cycleDurations[i];
            }

            float durationRatio = duration / compoundDuration;
            for (int i = 0; i < numberOfCycles; i++)
                cycleDurations[i] *= durationRatio;

            float tempDuration = 0f;
            for (int i = 0; i < numberOfCycles - 1; i++)
                tempDuration += cycleDurations[i];

            cycleDurations[numberOfCycles - 1] = duration - tempDuration;
        }

        public void Recycle()
        {
            this.AddToPool();
        }

        #region Static Methods

        /// <summary> Get a reaction from the given reaction type, either from the pool or a new one </summary>
        /// <typeparam name="T"> Reaction Type </typeparam>
        public static T Get<T>() where T : Reaction => ReactionPool.Get<T>();

        #endregion

        #region IDs

        public const int k_DefaultIntId = -1234;

        [ClearOnReload(true)]
        internal static readonly ReactionDictionary<object> ReactionByObjectId = new ReactionDictionary<object>();
        [ClearOnReload(true)]
        internal static readonly ReactionDictionary<string> ReactionByStringId = new ReactionDictionary<string>();
        [ClearOnReload(true)]
        internal static readonly ReactionDictionary<int> ReactionByIntId = new ReactionDictionary<int>();
        [ClearOnReload(true)]
        internal static readonly ReactionDictionary<object> ReactionByTargetObject = new ReactionDictionary<object>();

        public bool hasObjectId { get; internal set; }
        public object objectId { get; internal set; }

        public string stringId { get; internal set; }
        public bool hasStringId { get; internal set; }

        public int intId { get; internal set; }
        public bool hasIntId { get; internal set; }

        /// <summary> The object this reaction is attached to </summary>
        public object targetObject { get; internal set; }

        public bool hasTargetObject { get; internal set; }

        public static void StopAllReactionsByObjectId(object id, bool silent = false)
        {
            foreach (Reaction reaction in ReactionByObjectId.GetReactions(id))
                reaction.Stop(silent);
        }

        public static void StopAllReactionsByStringId(string id, bool silent = false)
        {
            foreach (Reaction reaction in ReactionByStringId.GetReactions(id))
                reaction.Stop(silent);
        }

        public static void StopAllReactionsByIntId(int id, bool silent = false)
        {
            foreach (Reaction reaction in ReactionByIntId.GetReactions(id))
                reaction.Stop(silent);
        }

        public static void StopAllReactionsByTargetObject(object target, bool silent = false)
        {
            foreach (Reaction reaction in ReactionByTargetObject.GetReactions(target))
                reaction.Stop(silent);
        }

        #endregion

        public override string ToString() =>
            $"[{(heartbeat != null ? heartbeat.GetType().Name : "No Heartbeat")}] " +
            $"[{GetType().Name}] " +
            $"[{state}] > " +
            $"[{direction}] > " +
            $"[{elapsedDuration.Round(3):0.000} / {duration} seconds] " +
            $"[{nameof(progress)}: {progress.Round(2):0.00} {(progress.Round(2) * 100f).Round(0):000}%]";
    }
}