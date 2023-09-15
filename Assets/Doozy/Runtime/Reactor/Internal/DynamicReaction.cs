// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Internal
{
    [Serializable]
    public abstract class DynamicReaction<T1, T2> : Reaction
    {
        internal Type typeOfPropertyType { get; set; }
        internal Type typeOfValueType { get; set; }

        [SerializeField] protected T2 FromValue;
        [SerializeField] protected T2 ToValue;
        [SerializeField] protected T2 CurrentValue;

        /// <summary> Start value </summary>
        public T2 fromValue => FromValue;

        /// <summary> End (target) value </summary>
        public T2 toValue => ToValue;

        /// <summary> Current value </summary>
        public T2 currentValue => CurrentValue;

        /// <summary> Getter for the reaction's from value </summary>
        public PropertyGetter<T1> getter { get; set; }

        /// <summary> Setter for the reaction's current value </summary>
        public PropertySetter<T1> setter { get; set; }

        /// <summary> Computed target values for each cycle </summary>
        protected T2[] cycleValues { get; set; }

        /// <summary> Current cycle start (from) value </summary>
        protected T2 cycleFrom => currentCycleIndex == 0 ? FromValue : cycleValues[currentCycleIndex - 1];

        /// <summary> Current cycle target (to) value </summary>
        protected T2 cycleTo => currentCycleIndex == 0 ? ToValue : cycleValues[currentCycleIndex];

        public ReactionCallback<T2> OnValueChangedCallback;

        protected DynamicReaction()
        {
            typeOfPropertyType = typeof(T1);
            typeOfValueType = typeof(T2);
        }

        public abstract float GetProgressAtValue(T2 value);

        public override void Reset()
        {
            base.Reset();

            getter = null;
            setter = null;

            OnValueChangedCallback = null;
        }

        /// <summary> Set from (start) value </summary>
        /// <param name="value"> From value </param>
        /// <param name="relative"> If TRUE, FromValue = CurrentValue + value </param>
        public abstract Reaction SetFrom(T2 value, bool relative = false);

        /// <summary> Set to (end) value </summary>
        /// <param name="value"> To value </param>
        /// <param name="relative">If TRUE, ToValue = CurrentValue + value </param>
        public abstract Reaction SetTo(T2 value, bool relative = false);

        /// <summary>
        /// Set current value. If the reaction is active, it will stop.
        /// </summary>
        /// <param name="value"> Current value</param>
        public virtual Reaction SetValue(T2 value)
        {
            if (isActive) Stop();
            CurrentValue = value;
            // SetProgressAt(GetProgressAtValue(value));
            return this;
        }

        /// <summary>
        /// Play the reaction from the current value to the given absolute or relative value
        /// </summary>
        /// <param name="value"> To value </param>
        /// <param name="relative"> If TRUE, ToValue = CurrentValue + value </param>
        public virtual Reaction PlayToValue(T2 value, bool relative = false)
        {
            if (isActive) Stop();
            SetFrom(CurrentValue);
            SetTo(value, relative);
            Play();
            return this;
        }

        /// <summary>
        /// Play the reaction from the given absolute or relative value to the current value 
        /// </summary>
        /// <param name="value"> From value </param>
        /// <param name="relative"> If TRUE, FromValue = CurrentValue + value </param>
        public virtual Reaction PlayFromValue(T2 value, bool relative = false)
        {
            if (isActive) Stop();
            SetFrom(value, relative);
            SetTo(CurrentValue);
            Play();
            return this;
        }

        /// <summary>
        /// Play the reaction from the given start from value to the end to value  
        /// </summary>
        /// <param name="from"> From value </param>
        /// <param name="to"> To value </param>
        /// <param name="reversed"> Play in reverse? </param>
        public virtual Reaction Play(T2 from, T2 to, bool reversed = false)
        {
            SetFrom(from);
            SetTo(to);
            Play(reversed);
            return this;
        }

        public override void Stop(bool silent = false, bool recycle = false)
        {
            switch (settings.playMode)
            {
                case PlayMode.Normal:
                    // elapsedDuration = direction == PlayDirection.Forward ? targetDuration : startDuration;
                    break;
                case PlayMode.PingPong:
                    // elapsedDuration = direction == PlayDirection.Forward ? startDuration : targetDuration;
                    break;
                case PlayMode.Spring:
                case PlayMode.Shake:
                    if (isPlaying)
                    {
                        elapsedDuration = direction == PlayDirection.Forward ? startDuration : targetDuration;
                        UpdateCurrentValue();
                    }
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
            base.Stop(silent, recycle);
        }

        protected override void ComputeNormal()
        {
            base.ComputeNormal();
            cycleValues = new[] { ToValue };
        }

        protected override void ComputePingPong()
        {
            base.ComputePingPong();
            cycleValues = new[] { ToValue, FromValue };
        }

        protected override void ComputeSpring()
        {
            base.ComputeSpring();
            cycleValues = new T2[numberOfCycles];
        }

        protected override void ComputeShake()
        {
            base.ComputeShake();
            cycleValues = new T2[numberOfCycles];
        }
    }
}
