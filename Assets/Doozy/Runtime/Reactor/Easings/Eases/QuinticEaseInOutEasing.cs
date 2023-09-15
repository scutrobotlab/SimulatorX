// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using static UnityEngine.Mathf;

namespace Doozy.Runtime.Reactor.Easings.Eases
{
    public class QuinticEaseInOutEasing : IEasing
    {
        private static float Calculate(float start, float end, float progress) =>
            LerpUnclamped(start, end, EasingFunction.QuinticEaseInOut(progress));

        public float Evaluate(float progress) =>
            Calculate(0, 1, Clamp01(progress));
    }
}
