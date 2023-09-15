// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Reactor.Internal;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Doozy.Runtime.Reactor.Reactions
{
    [Serializable]
    public class IntReaction : DynamicReaction<int, int>
    {
        public IntReaction()
        {
            FromValue = 0;
            ToValue = 1;
        }
        
        public override float GetProgressAtValue(int value) =>
            Mathf.Clamp01(Mathf.InverseLerp(FromValue, ToValue, value));

        public override void UpdateCurrentValue()
        {
            CurrentValue = (int)Mathf.LerpUnclamped(cycleFrom, cycleTo, currentCycleEasedProgress);
            setter?.Invoke(CurrentValue);
            OnValueChangedCallback?.Invoke(CurrentValue);
        }

        public override Reaction SetValue(int value)
        {
            base.SetValue(value);
            setter?.Invoke(CurrentValue);
            return this;
        }

        public override Reaction SetFrom(int value, bool relative = false)
        {
            FromValue = value;
            if (relative) FromValue += CurrentValue;
            if (isActive) ComputePlayMode();
            return this;
        }

        public override Reaction SetTo(int value, bool relative = false)
        {
            ToValue = value;
            if (relative) ToValue += CurrentValue;
            if (isActive) ComputePlayMode();
            return this;
        }

        protected override void ComputeSpring()
        {
            base.ComputeSpring();
            float springForce = settings.strength;
            float forceReduction = springForce / (numberOfCycles - 1);
            for (int i = 0; i < numberOfCycles; i++)
            {
                cycleValues[i] = (int)(FromValue + ToValue * (i % 2 == 0 ? springForce : -springForce * settings.elasticity));
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
            }
            cycleValues[numberOfCycles - 1] = FromValue;
        }
    }
    
    public static class IntReactionExtensions
    {
        #region getter

        public static T SetGetter<T>(this T target, PropertyGetter<int> getter) where T : IntReaction
        {
            target.getter = getter;
            return target;
        }

        public static T ClearGetter<T>(this T target) where T : IntReaction =>
            target.SetGetter(null);

        #endregion

        #region setter

        public static T SetSetter<T>(this T target, PropertySetter<int> setter) where T : IntReaction
        {
            target.setter = setter;
            return target;
        }

        public static T ClearSetter<T>(this T target) where T : IntReaction =>
            target.SetSetter(null);

        #endregion

        #region OnValueChanged

        public static T SetOnValueChangedCallback<T>(this T target, ReactionCallback<int> callback) where T : IntReaction
        {
            if (callback == null) return target;
            target.OnValueChangedCallback = callback;
            return target;
        }

        public static T AddOnValueChangedCallback<T>(this T target, ReactionCallback<int> callback) where T : IntReaction
        {
            if (callback == null) return target;
            target.OnValueChangedCallback += callback;
            return target;
        }

        public static T ClearOnValueChangedCallback<T>(this T target) where T : IntReaction
        {
            target.OnValueChangedCallback = null;
            return target;
        }

        #endregion
    }
}
