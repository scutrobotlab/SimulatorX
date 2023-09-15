// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;

namespace Doozy.Runtime.Common.Extensions
{
    public static class IntExtensions
    {
        /// <summary> Clamp the value between the given minimum integer and maximum integer values </summary>
        /// <param name="target"> Target integer </param>
        /// <param name="min"> The minimum integer point value to compare against </param>
        /// <param name="max"> The maximum integer point value to compare against </param>
        public static int Clamp(this int target, int min, int max) =>
            Mathf.Clamp(target, min, max);
    }
}
