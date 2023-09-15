// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Reactor.Internal;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Reactions
{
    [Serializable]
    public class UIFadeReaction : FloatReaction
    {
        public RectTransform rectTransform { get; private set; }
        public CanvasGroup canvasGroup { get; private set; }

        [SerializeField] private bool Enabled;
        public bool enabled
        {
            get => Enabled;
            set => Enabled = value;
        }

        [SerializeField] private float StartAlpha;
        public float startAlpha
        {
            get => StartAlpha;
            set => StartAlpha = value;
        }

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

        [SerializeField] private float FromCustomValue;
        public float fromCustomValue
        {
            get => FromCustomValue;
            set => FromCustomValue = value;
        }

        [SerializeField] private float ToCustomValue;
        public float toCustomValue
        {
            get => ToCustomValue;
            set => ToCustomValue = value;
        }

        [SerializeField] private float FromOffset;
        public float fromOffset
        {
            get => FromOffset;
            set => FromOffset = value;
        }

        [SerializeField] private float ToOffset;
        public float toOffset
        {
            get => ToOffset;
            set => ToOffset = value;
        }

        public float currentAlpha
        {
            get => canvasGroup.alpha;
            set => canvasGroup.alpha = Mathf.Clamp01(value);
        }

        public override void Reset()
        {
            base.Reset();

            rectTransform = null;
            canvasGroup = null;

            FromReferenceValue = ReferenceValue.StartValue;
            FromCustomValue = 1f;
            FromOffset = 0f;

            ToReferenceValue = ReferenceValue.StartValue;
            ToCustomValue = 1f;
            ToOffset = 0f;
        }

        public UIFadeReaction SetTarget(RectTransform targetRectTransform, CanvasGroup targetCanvasGroup)
        {
            this.SetTargetObject(targetRectTransform);
            rectTransform = targetRectTransform;
            canvasGroup = targetCanvasGroup;
            StartAlpha = currentAlpha;
            getter = () => currentAlpha;
            setter = value => currentAlpha = value;
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
            SetFrom(GetValue(FromReferenceValue, FromOffset, FromCustomValue));
            SetTo(GetValue(ToReferenceValue, ToOffset, ToCustomValue));
        }

        private float GetValue(ReferenceValue referenceValue, float offset, float customValue)
        {
            float value = referenceValue switch
                          {
                              ReferenceValue.StartValue   => StartAlpha + offset,
                              ReferenceValue.CurrentValue => currentAlpha + offset,
                              ReferenceValue.CustomValue  => customValue,
                              _                           => throw new ArgumentOutOfRangeException()
                          };
            return Mathf.Clamp01(value);
        }
    }
}
