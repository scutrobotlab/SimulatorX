// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Pooler;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI.Components
{
    public abstract class PoolableElement<T> : VisualElement, IPoolable where T : VisualElement, IPoolable, new()
    {
        private static readonly PoolService<T> PoolService = new PoolService<T>();

        /// <summary> Retrieve an item from the Pool. If one does not exist, a new one will be created instead and returned. </summary>
        /// <param name="debug"> Print relevant debug messages to the console </param>
        public static T Get(bool debug = false)
        {
            if (!debug) return PoolService.Get();

            T item = PoolService.Get();
            Debug.Log($"{nameof(PoolService)}.{nameof(PoolService.Get)} > [{nameof(Get)}] > {item.GetType().Name}");
            return item;
        }

        /// <summary> Returns TRUE if the item is in the Pool </summary>
        public bool inPool { get; set; }

        /// <summary>
        /// Reset to the default settings
        /// <para/> Needs to be set up in a custom manner for each component type
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Remove from Hierarchy and Reset
        /// <para/> This does not add to the Pool and is intended for the GC to clean it up
        /// </summary>
        public virtual void Dispose()
        {
            RemoveFromHierarchy();
            Reset();
        }

        /// <summary>
        /// Remove from Hierarchy and add to the Pool
        /// <para/> This also triggers a Reset, on the item, prior to being added to the Pool
        /// </summary>
        /// <param name="debug"> Print relevant debug messages to the console </param>
        public virtual void Recycle(bool debug = false)
        {
            if (debug) Debug.Log($"{nameof(PoolService)}.{nameof(PoolService.AddToPool)} < [{nameof(Recycle)}] < {GetType().Name}");
            RemoveFromHierarchy();
            PoolService.AddToPool(this as T);
        }
    }
}
