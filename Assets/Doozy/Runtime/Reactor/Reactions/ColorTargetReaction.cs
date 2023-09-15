// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Colors.Models;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Targets;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Reactions
{
    [Serializable]
    public class ColorTargetReaction : ColorReaction
    {
        public ReactorColorTarget colorTarget { get; private set; }

        #region Enabled

        [SerializeField] private bool Enabled;
        public bool enabled
        {
            get => Enabled;
            set => Enabled = value;
        }

        #endregion

        #region Start Color

        [SerializeField] private Color StartColor;
        public Color startColor
        {
            get => StartColor;
            set => StartColor = value;
        }

        #endregion

        #region From/To Reference Value

        [SerializeField] private ReferenceValue FromReferenceValue = ReferenceValue.StartValue;
        public ReferenceValue fromReferenceValue
        {
            get => FromReferenceValue;
            set => FromReferenceValue = value;
        }

        [SerializeField] private ReferenceValue ToReferenceValue = ReferenceValue.StartValue;
        public ReferenceValue toReferenceValue
        {
            get => ToReferenceValue;
            set => ToReferenceValue = value;
        }

        #endregion

        #region From/To Custom Value

        [SerializeField] private Color FromCustomValue = Color.white;
        public Color fromCustomValue
        {
            get => FromCustomValue;
            set => FromCustomValue = value;
        }

        [SerializeField] private Color ToCustomValue = Color.white;
        public Color toCustomValue
        {
            get => ToCustomValue;
            set => ToCustomValue = value;
        }

        #endregion

        #region From/To Hue Offset

        [SerializeField] private float FromHueOffset;
        public float fromHueOffset
        {
            get => FromHueOffset;
            set => FromHueOffset = Mathf.Clamp(value, -HSL.H.MAX, HSL.H.MAX);
        }

        [SerializeField] private float ToHueOffset;
        public float toHueOffset
        {
            get => ToHueOffset;
            set => ToHueOffset = Mathf.Clamp(value, -HSL.H.MAX, HSL.H.MAX);
        }

        #endregion

        #region From/To Saturation Offset

        [SerializeField] private float FromSaturationOffset;
        public float fromSaturationOffset
        {
            get => FromSaturationOffset;
            set => FromSaturationOffset = Mathf.Clamp(value, -HSL.S.MAX, HSL.S.MAX);
        }

        [SerializeField] private float ToSaturationOffset;
        public float toSaturationOffset
        {
            get => ToSaturationOffset;
            set => ToSaturationOffset = Mathf.Clamp(value, -HSL.S.MAX, HSL.S.MAX);
        }

        #endregion

        #region From/To Lightness Offset

        [SerializeField] private float FromLightnessOffset;
        public float fromLightnessOffset
        {
            get => FromLightnessOffset;
            set => FromLightnessOffset = Mathf.Clamp(value, -HSL.L.MAX, HSL.L.MAX);
        }

        [SerializeField] private float ToLightnessOffset;
        public float toLightnessOffset
        {
            get => ToLightnessOffset;
            set => ToLightnessOffset = Mathf.Clamp(value, -HSL.L.MAX, HSL.L.MAX);
        }

        #endregion

        #region From/To Alpha Offset

        [SerializeField] private float FromAlphaOffset;
        public float fromAlphaOffset
        {
            get => FromAlphaOffset;
            set => FromAlphaOffset = Mathf.Clamp(value, -1f, 1f);
        }

        [SerializeField] private float ToAlphaOffset;
        public float toAlphaOffset
        {
            get => ToAlphaOffset;
            set => ToAlphaOffset = Mathf.Clamp(value, -1f, 1f);
        }

        #endregion

        #region Current Color

        public Color currentColor
        {
            get => colorTarget.color;
            set => colorTarget.color = value;
        }

        #endregion

        public override void Reset()
        {
            base.Reset();

            colorTarget = null;

            FromReferenceValue = ReferenceValue.StartValue;
            ToReferenceValue = ReferenceValue.StartValue;

            FromCustomValue = Color.white;
            ToCustomValue = Color.white;

            fromHueOffset = 0f;
            toHueOffset = 0f;

            fromSaturationOffset = 0f;
            toSaturationOffset = 0f;

            fromLightnessOffset = 0f;
            toLightnessOffset = 0f;

            fromAlphaOffset = 0f;
            toAlphaOffset = 0f;
        }

        public ColorTargetReaction SetTarget(ReactorColorTarget target)
        {
            this.SetTargetObject(target);
            colorTarget = target;
            StartColor = currentColor;
            getter = () => currentColor;
            setter = value => currentColor = value;
            return this;
        }

        public override void Play(bool inReverse = false)
        {
            if (!isActive)
            {
                UpdateValues();
                SetValue(inReverse ? ToValue : FromValue);
            }
            base.Play(inReverse);
        }

        public override void PlayFromProgress(float fromProgress)
        {
            UpdateValues();
            base.PlayFromProgress(fromProgress);
        }

        public override void SetProgressAt(float targetProgress)
        {
            UpdateValues();
            base.SetProgressAt(targetProgress);
        }

        public void UpdateValues()
        {
            SetFrom
            (
                GetValue
                (
                    FromReferenceValue,
                    startColor,
                    currentColor,
                    FromCustomValue,
                    FromHueOffset,
                    FromSaturationOffset,
                    FromLightnessOffset,
                    FromAlphaOffset
                )
            );

            SetTo
            (
                GetValue
                (
                    ToReferenceValue,
                    startColor,
                    currentColor,
                    ToCustomValue,
                    ToHueOffset,
                    ToSaturationOffset,
                    ToLightnessOffset,
                    ToAlphaOffset
                )
            );
        }

        public Color GetValue(ReferenceValue referenceValue, Color refStartValue, Color refCurrentValue, Color refCustomValue, float hueOffset, float saturationOffset, float lightnessOffset, float alphaOffset)
        {
            Color value;

            switch (referenceValue)
            {
                case ReferenceValue.StartValue:
                    value = refStartValue;
                    break;
                case ReferenceValue.CurrentValue:
                    value = refCurrentValue;
                    break;
                case ReferenceValue.CustomValue:
                    return refCustomValue;
                default:
                    throw new ArgumentOutOfRangeException(nameof(referenceValue), referenceValue, null);
            }

            if (hueOffset == 0 & saturationOffset == 0 && lightnessOffset == 0 && alphaOffset == 0)
                return value;

            var hsl = value.ToHSL();
            {
                hsl.h += hueOffset;
                hsl.s += saturationOffset;
                hsl.l += lightnessOffset;

                hsl.h =
                    hsl.h < 0
                        ? hsl.h + 1
                        : hsl.h > 1
                            ? hsl.h - 1
                            : hsl.h;

                hsl.Validate();
            }

            float alpha = Mathf.Clamp01(value.a + alphaOffset);

            value = hsl.ToColor().WithAlpha(alpha);

            return value;
        }
    }
}
