// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Reactor.Internal;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Reactions
{
    [Serializable]
    public class UIMoveReaction : Vector3Reaction
    {
        public RectTransform rectTransform { get; private set; }

        internal bool UseCustomLocalScale = false;
        internal Vector3 CustomFromLocalScale = Vector3.one;
        internal Vector3 CustomToLocalScale = Vector3.one;

        internal bool UseCustomLocalRotation = false;
        internal Vector3 CustomFromLocalRotation = Vector3.zero;
        internal Vector3 CustomToLocalRotation = Vector3.zero;

        [SerializeField] private bool Enabled;
        public bool enabled
        {
            get => Enabled;
            set => Enabled = value;
        }

        [SerializeField] private UIAnimationType AnimationType = UIAnimationType.Custom;
        public UIAnimationType animationType
        {
            get => AnimationType;
            set => AnimationType = value;
        }

        [SerializeField] private Vector3 StartPosition;
        public Vector3 startPosition
        {
            get => StartPosition;
            set => StartPosition = value;
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
        
        [SerializeField] private MoveDirection FromDirection = MoveDirection.Left;
        public MoveDirection fromDirection
        {
            get => FromDirection;
            set
            {
                FromDirection = value;
                if (FromDirection == MoveDirection.CustomPosition) return;
                ToDirection = MoveDirection.CustomPosition;
            }
        }

        [SerializeField] private MoveDirection ToDirection = MoveDirection.Left;
        public MoveDirection toDirection
        {
            get => ToDirection;
            set
            {
                ToDirection = value;
                if (ToDirection == MoveDirection.CustomPosition) return;
                FromDirection = MoveDirection.CustomPosition;
            }
        }

        public Vector3 currentPosition
        {
            get => rectTransform.anchoredPosition3D;
            set => rectTransform.anchoredPosition3D = value;
        }

        public override void Reset()
        {
            base.Reset();

            rectTransform = null;
            AnimationType = UIAnimationType.Custom;

            FromDirection = MoveDirection.Left;
            FromReferenceValue = ReferenceValue.StartValue;
            FromCustomValue = Vector3.zero;
            FromOffset = Vector3.zero;

            ToDirection = MoveDirection.Left;
            ToReferenceValue = ReferenceValue.StartValue;
            ToCustomValue = Vector3.zero;
            ToOffset = Vector3.zero;
        }

        public UIMoveReaction SetTarget(RectTransform target)
        {
            this.SetTargetObject(target);
            rectTransform = target;
            StartPosition = currentPosition;
            getter = () => currentPosition;
            setter = value => currentPosition = value;
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
            switch (animationType)
            {
                case UIAnimationType.Show:
                    SetTo(GetValue(ToReferenceValue, ToOffset, ToCustomValue));
                    SetFrom
                    (
                        fromDirection == MoveDirection.CustomPosition
                            ? FromCustomValue
                            : ReactorUtils.GetMoveInPosition(
                                rectTransform,
                                fromDirection,
                                ToValue,
                                UseCustomLocalScale ? CustomFromLocalScale : rectTransform.localScale,
                                UseCustomLocalRotation ? CustomFromLocalRotation : rectTransform.localEulerAngles)
                            + FromOffset
                    );
                    break;
                case UIAnimationType.Hide:
                    SetFrom(GetValue(FromReferenceValue, FromOffset, FromCustomValue));
                    SetTo
                    (
                        toDirection == MoveDirection.CustomPosition
                            ? ToCustomValue
                            : ReactorUtils.GetMoveOutPosition(
                                rectTransform,
                                toDirection,
                                FromValue,
                                UseCustomLocalScale ? CustomToLocalScale : rectTransform.localScale,
                                UseCustomLocalRotation ? CustomToLocalRotation : rectTransform.localEulerAngles)
                            + ToOffset
                    );
                    break;
                case UIAnimationType.Loop:
                case UIAnimationType.Custom:
                case UIAnimationType.Button:
                case UIAnimationType.State:
                case UIAnimationType.Reset:
                    SetFrom(GetValue(FromReferenceValue, FromOffset, FromCustomValue));
                    SetTo(GetValue(ToReferenceValue, ToOffset, ToCustomValue));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Vector3 GetValue(ReferenceValue referenceValue, Vector3 offset, Vector3 customValue)
        {
            return referenceValue switch
                   {
                       ReferenceValue.StartValue   => StartPosition + offset,
                       ReferenceValue.CurrentValue => currentPosition + offset,
                       ReferenceValue.CustomValue  => customValue,
                       _                           => throw new ArgumentOutOfRangeException()
                   };
        }
    }
}
