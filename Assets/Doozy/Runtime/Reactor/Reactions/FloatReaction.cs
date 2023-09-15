// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Diagnostics.CodeAnalysis;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Internal;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Doozy.Runtime.Reactor.Reactions
{
    [Serializable]
    public class FloatReaction : DynamicReaction<float, float>
    {
        public FloatReaction()
        {
            FromValue = 0;
            ToValue = 1;
        }

        public override float GetProgressAtValue(float value) =>
            Mathf.Clamp01(Mathf.InverseLerp(FromValue, ToValue, value));

        public override void UpdateCurrentValue()
        {
            CurrentValue = Mathf.LerpUnclamped(cycleFrom, cycleTo, currentCycleEasedProgress);
            setter?.Invoke(CurrentValue);
            OnValueChangedCallback?.Invoke(CurrentValue);
        }

        public override Reaction SetValue(float value)
        {
            base.SetValue(value);
            setter?.Invoke(value);
            return this;
        }

        public override Reaction SetFrom(float value, bool relative = false)
        {
            FromValue = value;
            if (relative) FromValue += CurrentValue;
            if (isActive) ComputePlayMode();
            return this;
        }

        public override Reaction SetTo(float value, bool relative = false)
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
                cycleValues[i] = FromValue + ToValue * (i % 2 == 0 ? springForce : -springForce * settings.elasticity);
                cycleValues[i] = cycleValues[i].Round(4);
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
                cycleValues[i] = FromValue + ToValue * random * settings.strength;
                cycleValues[i] = cycleValues[i].Round(4);
            }
            cycleValues[numberOfCycles - 1] = FromValue;
        }
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class FloatReactionExtensions
    {
        #region getter

        public static T SetGetter<T>(this T target, PropertyGetter<float> getter) where T : FloatReaction
        {
            target.getter = getter;
            return target;
        }

        public static T ClearGetter<T>(this T target) where T : FloatReaction =>
            target.SetGetter(null);

        #endregion

        #region setter

        public static T SetSetter<T>(this T target, PropertySetter<float> setter) where T : FloatReaction
        {
            target.setter = setter;
            return target;
        }

        public static T ClearSetter<T>(this T target) where T : FloatReaction =>
            target.SetSetter(null);

        #endregion

        #region OnValueChanged

        public static T SetOnValueChangedCallback<T>(this T target, ReactionCallback<float> callback) where T : FloatReaction
        {
            if (callback == null) return target;
            target.OnValueChangedCallback = callback;
            return target;
        }

        public static T AddOnValueChangedCallback<T>(this T target, ReactionCallback<float> callback) where T : FloatReaction
        {
            if (callback == null) return target;
            target.OnValueChangedCallback += callback;
            return target;
        }

        public static T ClearOnValueChangedCallback<T>(this T target) where T : FloatReaction
        {
            target.OnValueChangedCallback = null;
            return target;
        }

        #endregion
    }
}
