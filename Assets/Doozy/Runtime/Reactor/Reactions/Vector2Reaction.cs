// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Internal;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Doozy.Runtime.Reactor.Reactions
{
    [Serializable]
    public class Vector2Reaction : DynamicReaction<Vector2, Vector2>
    {
        public Vector2Reaction()
        {
            FromValue = Vector2.zero;
            ToValue = Vector2.one;
        }

        public override float GetProgressAtValue(Vector2 value) =>
            Vector2Extensions.InverseLerp(fromValue, toValue, value);

        public override void UpdateCurrentValue()
        {
            // CurrentValue = Vector2.LerpUnclamped(FromValue, ToValue, easedProgress);
            CurrentValue = Vector2.LerpUnclamped(cycleFrom, cycleTo, currentCycleEasedProgress);
            setter?.Invoke(CurrentValue);
            OnValueChangedCallback?.Invoke(CurrentValue);
        }

        public override Reaction SetValue(Vector2 value)
        {
            base.SetValue(value);
            setter?.Invoke(value);
            return this;
        }

        public override Reaction SetFrom(Vector2 value, bool relative = false)
        {
            FromValue = value;
            if (relative) FromValue += CurrentValue;
            if (isActive) ComputePlayMode();
            return this;
        }

        public override Reaction SetTo(Vector2 value, bool relative = false)
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
                Vector2 random = Random.insideUnitCircle;
                cycleValues[i] = FromValue + new Vector2(ToValue.x * random.x, ToValue.y * random.y) * settings.strength;
                cycleValues[i] = cycleValues[i].Round(4);
            }
            cycleValues[numberOfCycles - 1] = FromValue;
        }
    }

    public static class Vector2ReactionExtensions
    {
        #region getter

        public static T SetGetter<T>(this T target, PropertyGetter<Vector2> getter) where T : Vector2Reaction
        {
            target.getter = getter;
            return target;
        }

        public static T ClearGetter<T>(this T target) where T : Vector2Reaction =>
            target.SetGetter(null);

        #endregion

        #region setter

        public static T SetSetter<T>(this T target, PropertySetter<Vector2> setter) where T : Vector2Reaction
        {
            target.setter = setter;
            return target;
        }

        public static T ClearSetter<T>(this T target) where T : Vector2Reaction =>
            target.SetSetter(null);

        #endregion

        #region OnValueChanged

        public static T SetOnValueChangedCallback<T>(this T target, ReactionCallback<Vector2> callback) where T : Vector2Reaction
        {
            if (callback == null) return target;
            target.OnValueChangedCallback = callback;
            return target;
        }

        public static T AddOnValueChangedCallback<T>(this T target, ReactionCallback<Vector2> callback) where T : Vector2Reaction
        {
            if (callback == null) return target;
            target.OnValueChangedCallback += callback;
            return target;
        }

        public static T ClearOnValueChangedCallback<T>(this T target) where T : Vector2Reaction
        {
            target.OnValueChangedCallback = null;
            return target;
        }

        #endregion
    }
}
