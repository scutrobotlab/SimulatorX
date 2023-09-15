// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;

namespace Doozy.Runtime.Common.Extensions
{
    public static class FloatExtensions
    {
        /// <summary> Round number with the given number of decimals </summary>
        /// <param name="target"> Target number </param>
        /// <param name="decimals"> Number of decimals </param>
        public static float Round(this float target, int decimals = 1) =>
            (float)Math.Round(target, decimals);

        /// <summary> Clamp the value between the given minimum float and maximum float values </summary>
        /// <param name="target"> Target float </param>
        /// <param name="min"> The minimum floating point value to compare against </param>
        /// <param name="max"> The maximum floating point value to compare against </param>
        public static float Clamp(this float target, float min, float max) =>
            Mathf.Clamp(target, min, max);

        /// <summary> Clamp value between 0 and 1 </summary>
        /// <param name="target"> Target float </param>
        public static float Clamp01(this float target) =>
            Mathf.Clamp01(target);


        /// <summary> Compare two floating point values and returns true if they are similar </summary>
        /// <param name="target"> Target floating point </param>
        /// <param name="other"> Other floating point to compare against </param>
        public static bool Approximately(this float target, float other) =>
            Mathf.Approximately(target, other);
    }
}
