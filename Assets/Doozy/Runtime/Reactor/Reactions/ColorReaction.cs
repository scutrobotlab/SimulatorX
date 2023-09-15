// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Reactor.Internal;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Doozy.Runtime.Reactor.Reactions
{
    [Serializable]
    public class ColorReaction : DynamicReaction<Color, Color>
    {
        public ColorReaction()
        {
            FromValue = Color.white;
            ToValue = Color.black;
        }

        public override float GetProgressAtValue(Color value) =>
            (value.Hue() - fromValue.Hue()) / (toValue.Hue() - fromValue.Hue());

        public override void UpdateCurrentValue()
        {
            CurrentValue = Color.LerpUnclamped(cycleFrom, cycleTo, currentCycleEasedProgress);
            setter?.Invoke(CurrentValue);
            OnValueChangedCallback?.Invoke(CurrentValue);
        }

        public override Reaction SetValue(Color value)
        {
            base.SetValue(value);
            setter?.Invoke(CurrentValue);
            return this;
        }

        public override Reaction SetFrom(Color value, bool relative = false)
        {
            FromValue = value;
            if (relative) FromValue += CurrentValue;
            if (isActive) ComputePlayMode();
            return this;
        }

        public override Reaction SetTo(Color value, bool relative = false)
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
                Color random = Random.ColorHSV();
                cycleValues[i] = FromValue + ToValue * random * settings.strength;
            }
            cycleValues[numberOfCycles - 1] = FromValue;
        }
    }

    public static class ColorReactionExtensions
    {
        #region getter

        public static T SetGetter<T>(this T target, PropertyGetter<Color> getter) where T : ColorReaction
        {
            target.getter = getter;
            return target;
        }

        public static T ClearGetter<T>(this T target) where T : ColorReaction =>
            target.SetGetter(null);

        #endregion

        #region setter

        public static T SetSetter<T>(this T target, PropertySetter<Color> setter) where T : ColorReaction
        {
            target.setter = setter;
            return target;
        }

        public static T ClearSetter<T>(this T target) where T : ColorReaction =>
            target.SetSetter(null);

        #endregion

        #region OnValueChanged

        public static T SetOnValueChangedCallback<T>(this T target, ReactionCallback<Color> callback) where T : ColorReaction
        {
            if (callback == null) return target;
            target.OnValueChangedCallback = callback;
            return target;
        }

        public static T AddOnValueChangedCallback<T>(this T target, ReactionCallback<Color> callback) where T : ColorReaction
        {
            if (callback == null) return target;
            target.OnValueChangedCallback += callback;
            return target;
        }

        public static T ClearOnValueChangedCallback<T>(this T target) where T : ColorReaction
        {
            target.OnValueChangedCallback = null;
            return target;
        }

        #endregion
    }
}
