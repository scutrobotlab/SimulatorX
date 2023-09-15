// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Reactor.Internal;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Animations
{
    [Serializable]
    public class UIAnimationSettings
    {
        [SerializeField] private UIAnimationType AnimationType;
        public UIAnimationType animationType => AnimationType;

        public bool MoveEnabled;
        public ReferenceValue MoveFromReferenceValue;
        public ReferenceValue MoveToReferenceValue;
        public Vector3 MoveFromCustomValue;
        public Vector3 MoveToCustomValue;
        public Vector3 MoveFromOffset;
        public Vector3 MoveToOffset;
        public MoveDirection MoveFromDirection;
        public MoveDirection MoveToDirection;
        public ReactionSettings MoveReactionSettings;

        public bool RotateEnabled;
        public ReferenceValue RotateFromReferenceValue;
        public ReferenceValue RotateToReferenceValue;
        public Vector3 RotateFromCustomValue;
        public Vector3 RotateToCustomValue;
        public Vector3 RotateFromOffset;
        public Vector3 RotateToOffset;
        public ReactionSettings RotateReactionSettings;

        public bool ScaleEnabled;
        public ReferenceValue ScaleFromReferenceValue;
        public ReferenceValue ScaleToReferenceValue;
        public Vector3 ScaleFromCustomValue;
        public Vector3 ScaleToCustomValue;
        public Vector3 ScaleFromOffset;
        public Vector3 ScaleToOffset;
        public ReactionSettings ScaleReactionSettings;

        public bool FadeEnabled;
        public ReferenceValue FadeFromReferenceValue;
        public ReferenceValue FadeToReferenceValue;
        public float FadeFromCustomValue;
        public float FadeToCustomValue;
        public float FadeFromOffset;
        public float FadeToOffset;
        public ReactionSettings FadeReactionSettings;

        public UIAnimationSettings() : this(UIAnimationType.Custom) {}

        public UIAnimationSettings(UIAnimationType animationType)
        {
            AnimationType = animationType;
            MoveReactionSettings = new ReactionSettings();
            RotateReactionSettings = new ReactionSettings();
            ScaleReactionSettings = new ReactionSettings();
            FadeReactionSettings = new ReactionSettings();
        }

        public UIAnimationSettings(UIAnimation source) =>
            GetAnimationSettings(source);

        public void SetAnimationSettings(UIAnimation target)
        {
            _ = target ?? throw new NullReferenceException(nameof(target));

            target.animationType = AnimationType;

            target.Move.enabled = MoveEnabled;
            target.Move.animationType = AnimationType;
            target.Move.fromReferenceValue = MoveFromReferenceValue;
            target.Move.toReferenceValue = MoveToReferenceValue;
            target.Move.fromCustomValue = MoveFromCustomValue;
            target.Move.toCustomValue = MoveToCustomValue;
            target.Move.fromOffset = MoveFromOffset;
            target.Move.toOffset = MoveToOffset;
            target.Move.fromDirection = target.animationType == UIAnimationType.Show ? MoveFromDirection : MoveDirection.CustomPosition;
            target.Move.toDirection = target.animationType == UIAnimationType.Hide ? MoveToDirection : MoveDirection.CustomPosition;
            target.Move.ApplyReactionSettings(MoveReactionSettings);

            target.Rotate.enabled = RotateEnabled;
            target.Rotate.fromReferenceValue = RotateFromReferenceValue;
            target.Rotate.toReferenceValue = RotateToReferenceValue;
            target.Rotate.fromCustomValue = RotateFromCustomValue;
            target.Rotate.toCustomValue = RotateToCustomValue;
            target.Rotate.fromOffset = RotateFromOffset;
            target.Rotate.toOffset = RotateToOffset;
            target.Rotate.ApplyReactionSettings(RotateReactionSettings);

            target.Scale.enabled = ScaleEnabled;
            target.Scale.fromReferenceValue = ScaleFromReferenceValue;
            target.Scale.toReferenceValue = ScaleToReferenceValue;
            target.Scale.fromCustomValue = ScaleFromCustomValue;
            target.Scale.toCustomValue = ScaleToCustomValue;
            target.Scale.fromOffset = ScaleFromOffset;
            target.Scale.toOffset = ScaleToOffset;
            target.Scale.ApplyReactionSettings(ScaleReactionSettings);

            target.Fade.enabled = FadeEnabled;
            target.Fade.fromReferenceValue = FadeFromReferenceValue;
            target.Fade.toReferenceValue = FadeToReferenceValue;
            target.Fade.fromCustomValue = FadeFromCustomValue;
            target.Fade.toCustomValue = FadeToCustomValue;
            target.Fade.fromOffset = FadeFromOffset;
            target.Fade.toOffset = FadeToOffset;
            target.Fade.ApplyReactionSettings(FadeReactionSettings);
        }

        public void GetAnimationSettings(UIAnimation source)
        {
            _ = source ?? throw new NullReferenceException(nameof(source));

            AnimationType = source.animationType;

            MoveEnabled = source.Move.enabled;
            MoveFromReferenceValue = source.Move.fromReferenceValue;
            MoveToReferenceValue = source.Move.toReferenceValue;
            MoveFromCustomValue = source.Move.fromCustomValue;
            MoveToCustomValue = source.Move.toCustomValue;
            MoveFromOffset = source.Move.fromOffset;
            MoveToOffset = source.Move.toOffset;
            MoveFromDirection = source.animationType == UIAnimationType.Show ? source.Move.fromDirection : MoveDirection.CustomPosition;
            MoveToDirection = source.animationType == UIAnimationType.Hide ?  source.Move.toDirection : MoveDirection.CustomPosition;;
            MoveReactionSettings = new ReactionSettings(source.Move.settings);

            RotateEnabled = source.Rotate.enabled;
            RotateFromReferenceValue = source.Rotate.fromReferenceValue;
            RotateToReferenceValue = source.Rotate.toReferenceValue;
            RotateFromCustomValue = source.Rotate.fromCustomValue;
            RotateToCustomValue = source.Rotate.toCustomValue;
            RotateFromOffset = source.Rotate.fromOffset;
            RotateToOffset = source.Rotate.toOffset;
            RotateReactionSettings = new ReactionSettings(source.Rotate.settings);

            ScaleEnabled = source.Scale.enabled;
            ScaleFromReferenceValue = source.Scale.fromReferenceValue;
            ScaleToReferenceValue = source.Scale.toReferenceValue;
            ScaleFromCustomValue = source.Scale.fromCustomValue;
            ScaleToCustomValue = source.Scale.toCustomValue;
            ScaleFromOffset = source.Scale.fromOffset;
            ScaleToOffset = source.Scale.toOffset;
            ScaleReactionSettings = new ReactionSettings(source.Scale.settings);

            FadeEnabled = source.Fade.enabled;
            FadeFromReferenceValue = source.Fade.fromReferenceValue;
            FadeToReferenceValue = source.Fade.toReferenceValue;
            FadeFromCustomValue = source.Fade.fromCustomValue;
            FadeToCustomValue = source.Fade.toCustomValue;
            FadeFromOffset = source.Fade.fromOffset;
            FadeToOffset = source.Fade.toOffset;
            FadeReactionSettings = new ReactionSettings(source.Fade.settings);
        }

        public static void SetAnimationSettings(UIAnimationSettings source, UIAnimation target)
        {
            _ = source ?? throw new NullReferenceException(nameof(source));
            _ = target ?? throw new NullReferenceException(nameof(target));
            source.SetAnimationSettings(target);
        }

        public static void GetAnimationSettings(UIAnimation source, UIAnimationSettings target)
        {
            _ = source ?? throw new NullReferenceException(nameof(source));
            _ = target ?? throw new NullReferenceException(nameof(target));
            target.GetAnimationSettings(source);
        }
    }
}
