// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
// ReSharper disable UnusedType.Global

namespace Doozy.Runtime.Common.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary> Convert IEnumerable to HashSet </summary>
        /// <param name="source"> Source IEnumerable </param>
        /// <param name="comparer"></param>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer = null) =>
            new HashSet<T>(source, comparer);
    }
}
