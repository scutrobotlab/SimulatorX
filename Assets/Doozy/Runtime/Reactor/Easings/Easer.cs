// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Reactor.Easings
{
    public class Easer
    {
        public EaseMode easeMode { get; private set; }
        public IEasing easing { get; private set; }
        public AnimationCurve animationCurve { get; private set; }

        public Easer() =>
            SetEase(Ease.Linear);

        public float Evaluate(float time)
        {
            return easeMode switch
                   {
                       EaseMode.Ease           => easing.Evaluate(time),
                       EaseMode.AnimationCurve => animationCurve.Evaluate(time),
                       _                       => throw new ArgumentOutOfRangeException()
                   };
        }

        public void SetEase(Ease ease)
        {
            easing = EaseFactory.GetEase(ease);
            easeMode = EaseMode.Ease;
        }

        public void SetAnimationCurve(AnimationCurve curve)
        {
            animationCurve = curve;
            easeMode = EaseMode.AnimationCurve;
        }
    }
}
