// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using static UnityEngine.Mathf;

namespace Doozy.Runtime.Reactor.Easings.Eases
{
    public class SpringEasing : IEasing
    {
        private static float Calculate(float start, float end, float progress)
        {
            progress = Clamp01(progress);
            progress = (Sin(progress * PI * (0.2f + 2.5f * Pow(progress, 3))) * Pow(1f - progress, 2.2f) + progress) * (1f + 1.2f * (1f - progress));
            return LerpUnclamped(start, end, progress);
        }

        public float Evaluate(float progress) =>
            Calculate(0, 1, Clamp01(progress));
    }
}
