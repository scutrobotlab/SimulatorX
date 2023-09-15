// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Reactor.Internal;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Reactions
{
    [Serializable]
    public class UIRotateReaction : Vector3Reaction
    {
        public RectTransform rectTransform { get; private set; }

        [SerializeField] private bool Enabled;
        public bool enabled
        {
            get => Enabled;
            set => Enabled = value;
        }

        [SerializeField] private Vector3 StartRotation;
        public Vector3 startRotation
        {
            get => StartRotation;
            set => StartRotation = value;
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

        [SerializeField] private Vector3 FromCustomValue;
        public Vector3 fromCustomValue
        {
            get => FromCustomValue;
            set => FromCustomValue = value;
        }

        [SerializeField] private Vector3 ToCustomValue;
        public Vector3 toCustomValue
        {
            get => ToCustomValue;
            set => ToCustomValue = value;
        }

        [SerializeField] private Vector3 FromOffset;
        public Vector3 fromOffset
        {
            get => FromOffset;
            set => FromOffset = value;
        }

        [SerializeField] private Vector3 ToOffset;
        public Vector3 toOffset
        {
            get => ToOffset;
            set => ToOffset = value;
        }

        public Vector3 currentRotation
        {
            get
            {
                Vector3 localEulerAngles = rectTransform.localEulerAngles;
                float angleX = localEulerAngles.x;
                float angleY = localEulerAngles.y;
                float angleZ = localEulerAngles.z;
                angleX = angleX > 180 ? angleX - 360 : angleX;
                angleY = angleY > 180 ? angleY - 360 : angleY;
                angleZ = angleZ > 180 ? angleZ - 360 : angleZ;
                return new Vector3(angleX, angleY, angleZ);
            }
            set => rectTransform.localEulerAngles = value; // rectTransform.localRotation = Quaternion.Euler(value);
        }

        public override void Reset()
        {
            base.Reset();

            rectTransform = null;

            FromReferenceValue = ReferenceValue.StartValue;
            FromCustomValue = Vector3.zero;
            FromOffset = Vector3.zero;

            ToReferenceValue = ReferenceValue.StartValue;
            ToCustomValue = Vector3.zero;
            ToOffset = Vector3.zero;
        }

        public UIRotateReaction SetTarget(RectTransform target)
        {
            this.SetTargetObject(target);
            rectTransform = target;
            StartRotation = currentRotation;
            getter = () => currentRotation;
            setter = value => currentRotation = value;
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

        private Vector3 GetValue(ReferenceValue referenceValue, Vector3 offset, Vector3 customValue)
        {
            return referenceValue switch
                   {
                       ReferenceValue.StartValue   => StartRotation + offset,
                       ReferenceValue.CurrentValue => currentRotation + offset,
                       ReferenceValue.CustomValue  => customValue,
                       _                           => throw new ArgumentOutOfRangeException()
                   };
        }
    }
}
