// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Common
{
    public static class ListPool<T>
    {
        // Object pool to avoid allocations.
        private static readonly ObjectPool<List<T>> Pool = new ObjectPool<List<T>>(null, Clear);
        
        private static void Clear(List<T> list) =>
            list.Clear();

        public static List<T> Get() =>
            Pool.Get();

        public static void Release(List<T> toRelease) =>
            Pool.Release(toRelease);
    }
}
