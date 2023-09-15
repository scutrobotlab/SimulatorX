// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Runtime.Common.Attributes;

namespace Doozy.Runtime.Signals
{
    internal static class SignalPool
    {
        private static HashSet<Signal> pool { get; set; }

        [ClearOnReload(resetValue: false)]
        private static bool initialized { get; set; }

        [ExecuteOnReload]
        private static void Initialize()
        {
            if (initialized) return;
            pool = new HashSet<Signal>();
            initialized = true;
        }

        /// <summary> Get a signal from the given signal type, either from the pool or a new one </summary>
        /// <typeparam name="T"> Signal Type </typeparam>
        public static T Get<T>() where T : Signal, new()
        {
            Initialize();
            pool.Remove(null);
            T signal = null;
            foreach (Signal pooledItem in pool)
            {
                if (!(pooledItem is T castedItem)) continue;
                signal = castedItem;
                pool.Remove(castedItem);
                break;
            }

            signal ??= Activator.CreateInstance<T>();
            return signal;
        }

        /// <summary> Return the given reaction to the pool </summary>
        public static void AddToPool<T>(this T signal) where T : Signal
        {
            Initialize();
            signal.Reset();
            pool.Add(signal);
        }
    }
}
