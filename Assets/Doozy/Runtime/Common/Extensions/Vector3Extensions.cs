// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;

namespace Doozy.Runtime.Common.Extensions
{
    public static class Vector3Extensions
    {
        /// <summary> Round x, y and z to the given number of decimals </summary>
        /// <param name="target"> Target Vector3 </param>
        /// <param name="decimals"> Number of decimals </param>
        public static Vector3 Round(this Vector3 target, int decimals = 1) =>
            new Vector3
            (
                (float)Math.Round(target.x, decimals),
                (float)Math.Round(target.y, decimals),
                (float)Math.Round(target.z, decimals)
            );

        /// <summary> Clamp x, y and z between the given min and max </summary>
        /// <param name="target"> Target Vector3 </param>
        /// <param name="min"> Min Vector3 </param>
        /// <param name="max"> Max Vector3 </param>
        public static Vector3 Clamp(this Vector3 target, Vector3 min, Vector3 max)
        {
            target.x = Mathf.Clamp(target.x, min.x, max.x);
            target.y = Mathf.Clamp(target.y, min.y, max.y);
            target.z = Mathf.Clamp(target.z, min.z, max.z);
            return target;
        }

        /// <summary> Clamp x, y and z between 0 and 1 </summary>
        /// <param name="target"> Target Vector3 </param>
        public static Vector3 Clamp01(this Vector3 target)
        {
            target.x = Mathf.Clamp01(target.x);
            target.y = Mathf.Clamp01(target.y);
            target.z = Mathf.Clamp01(target.z);
            return target;
        }

        /// <summary> Compares two Vector3 values and returns true if they are similar </summary>
        public static bool Approximately(this Vector3 target, Vector3 other) =>
            Mathf.Approximately(target.x, other.x)
            && Mathf.Approximately(target.y, other.y)
            && Mathf.Approximately(target.z, other.z);

        public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
        {
            Vector3 ab = b - a;
            Vector3 av = value - a;

            float dot = Vector3.Dot(av, ab);
            float f = Vector3.Dot(ab, ab);
            if (f == 0) return 0f;
            return dot / f;
            // Debug.Log($"a: {a} / b: {b} / value: {value} / ab: {ab} / av: {av} / dot: {dot} / f: {f}");
        }
    }
}
