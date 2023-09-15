// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Targets;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Reactions
{
    [Serializable]
    public class SpriteTargetReaction : SpriteReaction
    {
        public ReactorSpriteTarget spriteTarget { get; private set; }

        #region Enabled

        [SerializeField] private bool Enabled;
        public bool enabled
        {
            get => Enabled;
            set => Enabled = value;
        }

        #endregion

        #region Start Frame

        [SerializeField] private int StartFrame;
        public int startFrame
        {
            get => StartFrame;
            set => StartFrame = value;
        }

        #endregion

        #region From/To Frame Reference Value

        [SerializeField] private FrameReferenceValue FromReferenceValue = FrameReferenceValue.FirstFrame;
        public FrameReferenceValue fromReferenceValue
        {
            get => FromReferenceValue;
            set => FromReferenceValue = value;
        }

        [SerializeField] private FrameReferenceValue ToReferenceValue = FrameReferenceValue.LastFrame;
        public FrameReferenceValue toReferenceValue
        {
            get => ToReferenceValue;
            set => ToReferenceValue = value;
        }

        #endregion

        #region From/To Custom Value

        [SerializeField] private int FromCustomValue;
        public int fromCustomValue
        {
            get => FromCustomValue;
            set => FromCustomValue = Mathf.Clamp(value, firstFrame, lastFrame);
        }

        [SerializeField] private int ToCustomValue;
        public int toCustomValue
        {
            get => ToCustomValue;
            set => ToCustomValue = Mathf.Clamp(value, firstFrame, lastFrame);
        }

        #endregion

        #region From/To Frame Offset Value

        [SerializeField] private int FromFrameOffset;
        public int fromFrameOffset
        {
            get => FromFrameOffset;
            set => FromFrameOffset = Mathf.Clamp(value, firstFrame, lastFrame);
        }

        [SerializeField] private int ToFrameOffset;
        public int toFrameOffset
        {
            get => ToFrameOffset;
            set => ToFrameOffset = Mathf.Clamp(value, firstFrame, lastFrame);
        }

        #endregion

        #region From/To Custom Progress

        [SerializeField] private float FromCustomProgress;
        public float fromCustomProgress
        {
            get => FromCustomProgress;
            set => FromCustomProgress = Mathf.Clamp01(value);
        }

        [SerializeField] private float ToCustomProgress;
        public float toCustomProgress
        {
            get => ToCustomProgress;
            set => ToCustomProgress = Mathf.Clamp01(value);
        }

        #endregion

        #region Current Sprite

        public Sprite currentSprite
        {
            get => spriteTarget.sprite;
            set => spriteTarget.sprite = value;
        }

        #endregion

        public override void Reset()
        {
            base.Reset();

            spriteTarget = null;

            FromReferenceValue = FrameReferenceValue.FirstFrame;
            ToReferenceValue = FrameReferenceValue.LastFrame;

            FromCustomValue = 0;
            ToCustomValue = 0;

            FromFrameOffset = 0;
            ToFrameOffset = 0;

            FromCustomProgress = 0f;
            ToCustomProgress = 1f;
        }

        public SpriteTargetReaction SetTarget(ReactorSpriteTarget target)
        {
            this.SetTargetObject(target);
            spriteTarget = target;
            startFrame = currentFrame;
            getter = () => currentSprite;
            setter = value => currentSprite = value;
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
                    firstFrame,
                    lastFrame,
                    currentFrame,
                    FromCustomValue,
                    FromFrameOffset,
                    FromCustomProgress
                )
            );

            SetTo
            (
                GetValue
                (
                    ToReferenceValue,
                    firstFrame,
                    lastFrame,
                    currentFrame,
                    ToCustomValue,
                    ToFrameOffset,
                    ToCustomProgress
                )
            );
        }

        public int GetValue
        (
            FrameReferenceValue referenceValue,
            int refFirstFrame,
            int refLastFrame,
            int refCurrentFrame,
            int refCustomFrame,
            int refFrameOffset,
            float refCustomProgress
        )
        {

            int value = referenceValue switch
                        {
                            FrameReferenceValue.FirstFrame     => refFirstFrame + refFrameOffset,
                            FrameReferenceValue.LastFrame      => refLastFrame + refFrameOffset,
                            FrameReferenceValue.CurrentFrame   => refCurrentFrame + refFrameOffset,
                            FrameReferenceValue.CustomFrame    => refCustomFrame,
                            FrameReferenceValue.CustomProgress => GetFrameAtProgress(refCustomProgress),
                            _                                  => throw new ArgumentOutOfRangeException(nameof(referenceValue), referenceValue, null)
                        };

            return Mathf.Clamp(value, firstFrame, lastFrame);
            ;
        }
    }
}
