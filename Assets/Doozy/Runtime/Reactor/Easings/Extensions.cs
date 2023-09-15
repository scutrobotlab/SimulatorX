// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Reactor.Easings
{
    public static class Extensions
    {
        /// <summary>
        /// Evaluate the easing at the given time
        /// </summary>
        /// <param name="easing"> Target easing </param>
        /// <param name="startValue"> Start value </param>
        /// <param name="targetValue"> Target value </param>
        /// <param name="time"> Time in seconds </param>
        public static float Evaluate(this IEasing easing, float startValue, float targetValue, float time) =>
            startValue + (targetValue - startValue) * easing.Evaluate(time);

    }
}
