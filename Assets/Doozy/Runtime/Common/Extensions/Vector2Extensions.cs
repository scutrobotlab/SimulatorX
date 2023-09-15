// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;

namespace Doozy.Runtime.Common.Extensions
{
    public static class Vector2Extensions
    {
        /// <summary> Round x and y to the given number of decimals </summary>
        /// <param name="target"> Target Vector2 </param>
        /// <param name="decimals"> Number of decimals </param>
        public static Vector2 Round(this Vector2 target, int decimals = 1) =>
            new Vector2
            (
                (float)Math.Round(target.x, decimals),
                (float)Math.Round(target.y, decimals)
            );

        /// <summary> Clamp x and y between the given min and max </summary>
        /// <param name="target"> Target Vector2 </param>
        /// <param name="min"> Min Vector2 </param>
        /// <param name="max"> Max Vector2 </param>
        public static Vector2 Clamp(this Vector2 target, Vector2 min, Vector2 max)
        {
            target.x = Mathf.Clamp(target.x, min.x, max.x);
            target.y = Mathf.Clamp(target.y, min.y, max.y);
            return target;
        }

        /// <summary> Clamp x and y between 0 and 1 </summary>
        /// <param name="target"> Target Vector2 </param>
        public static Vector2 Clamp01(this Vector2 target)
        {
            target.x = Mathf.Clamp01(target.x);
            target.y = Mathf.Clamp01(target.y);
            return target;
        }

        /// <summary>  Compares two Vector2 values and returns true if they are similar </summary>
        public static bool Approximately(this Vector2 target, Vector2 other) =>
            Mathf.Approximately(target.x, other.x)
            && Mathf.Approximately(target.y, other.y);

        public static float InverseLerp(Vector2 a, Vector2 b, Vector2 value)
        {
            Vector2 ab = b - a;
            Vector2 av = value - a;
            
            float dot = Vector3.Dot(av, ab);
            float f = Vector3.Dot(ab, ab);
            if (f == 0) return 0f;
            return dot / f;
            // Debug.Log($"a: {a} / b: {b} / value: {value} / ab: {ab} / av: {av} / dot: {dot} / f: {f}");
        }
    }
}
