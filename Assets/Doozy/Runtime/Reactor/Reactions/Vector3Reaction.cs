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
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [Serializable]
    public class Vector3Reaction : DynamicReaction<Vector3, Vector3>
    {
        public Vector3Reaction()
        {
            FromValue = Vector3.zero;
            ToValue = Vector3.one;
        }

        public override float GetProgressAtValue(Vector3 value) =>
            Vector3Extensions.InverseLerp(fromValue, toValue, value);

        public override void UpdateCurrentValue()
        {
            CurrentValue = Vector3.LerpUnclamped(cycleFrom, cycleTo, currentCycleEasedProgress);
            // CurrentValue = Vector3.SlerpUnclamped(cycleFrom, cycleTo, currentCycleEasedProgress);
            setter?.Invoke(CurrentValue);
            OnValueChangedCallback?.Invoke(CurrentValue);
        }

        public override Reaction SetValue(Vector3 value)
        {
            base.SetValue(value);
            setter?.Invoke(value);
            return this;
        }

        public override Reaction SetFrom(Vector3 value, bool relative = false)
        {
            FromValue = value;
            if (relative) FromValue += CurrentValue;
            if (isActive) ComputePlayMode();
            return this;
        }

        public override Reaction SetTo(Vector3 value, bool relative = false)
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
                Vector3 random = Random.insideUnitSphere;
                cycleValues[i] = FromValue + new Vector3(ToValue.x * random.x, ToValue.y * random.y, ToValue.z * random.z) * settings.strength;
                cycleValues[i] = cycleValues[i].Round(4);
            }
            cycleValues[numberOfCycles - 1] = FromValue;
        }
    }

    public static class Vector3ReactionExtensions
    {
        #region getter

        public static T SetGetter<T>(this T target, PropertyGetter<Vector3> getter) where T : Vector3Reaction
        {
            target.getter = getter;
            return target;
        }

        public static T ClearGetter<T>(this T target) where T : Vector3Reaction =>
            target.SetGetter(null);

        #endregion

        #region setter

        public static T SetSetter<T>(this T target, PropertySetter<Vector3> setter) where T : Vector3Reaction
        {
            target.setter = setter;
            return target;
        }

        public static T ClearSetter<T>(this T target) where T : Vector3Reaction =>
            target.SetSetter(null);

        #endregion

        #region OnValueChanged

        public static T SetOnValueChangedCallback<T>(this T target, ReactionCallback<Vector3> callback) where T : Vector3Reaction
        {
            if (callback == null) return target;
            target.OnValueChangedCallback = callback;
            return target;
        }

        public static T AddOnValueChangedCallback<T>(this T target, ReactionCallback<Vector3> callback) where T : Vector3Reaction
        {
            if (callback == null) return target;
            target.OnValueChangedCallback += callback;
            return target;
        }

        public static T ClearOnValueChangedCallback<T>(this T target) where T : Vector3Reaction
        {
            target.OnValueChangedCallback = null;
            return target;
        }

        #endregion
    }
}
