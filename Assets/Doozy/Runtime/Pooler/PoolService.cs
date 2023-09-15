// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnassignedField.Global

namespace Doozy.Runtime.Pooler
{
    public class PoolService<T> where T : IPoolable, new()
    {
        /// <summary> Maximum number of items this pool can manage </summary>
        public int maxPoolSize { get; private set; }

        /// <summary>
        /// Action invoked every time an operation is performed on the pool
        /// <para/> ActionsEnabled needs to be TRUE for this action to work
        /// </summary>
        public UnityAction OnPoolUpdate;

        /// <summary>
        /// Action invoked every time an item is added to the pool
        /// <para/> ActionsEnabled needs to be TRUE for this action to work
        /// </summary>
        public UnityAction<T> OnItemAddedToPool;

        /// <summary>
        /// Action invoked every time an item is retrieved from the pool (includes newly created items)
        /// <para/> ActionsEnabled needs to be TRUE for this action to work
        /// </summary>
        public UnityAction<T> OnItemRetrievedFromPool;

        /// <summary> Flag used to enable/disable the pool's callbacks </summary>
        public bool CallbacksEnabled = false;

        /// <summary> All pooled items. Use Get() to retrieve an item for the pool! </summary>
        private List<T> pool { get; set; } = new List<T>();

        private bool initialized { get; set; }

        private void Initialize()
        {
            if (initialized) return;
            pool ??= new List<T>();
            maxPoolSize = -1;
            initialized = true;
        }

        /// <summary> Get an item either from the pool or a new one, if the pool does not contain it </summary>
        public T Get()
        {
            Initialize();
            RemoveNulls();
            var item = default(T);
            bool itemFoundInPool = false;
            foreach (T pooledItem in pool)
            {
                if (!(pooledItem is {} castedItem)) continue;
                pool.Remove(castedItem);
                item = castedItem;
                itemFoundInPool = true;
                break;
            }

            // item = itemFoundInPool ? item : Activator.CreateInstance<T>();
            item = itemFoundInPool ? item : new T();
            item.inPool = false;
            item.Reset();

            if (!CallbacksEnabled)
                return item;
            OnPoolUpdate?.Invoke();
            OnItemRetrievedFromPool?.Invoke(item);
            return item;
        }

        /// <summary> Add item to pool </summary>
        public void AddToPool(T item)
        {
            Initialize();

            if (item == null)
                return;

            if (maxPoolSize > 0 && pool.Count > maxPoolSize)
            {
                item.Dispose();
                return;
            }

            item.Reset();
            if (!pool.Contains(item)) pool.Add(item);
            item.inPool = true;

            if (!CallbacksEnabled)
                return;
            OnPoolUpdate?.Invoke();
            OnItemAddedToPool?.Invoke(item);
        }

        /// <summary> Create a given number of new items and add them to the pool </summary>
        /// <param name="numberOfItems"> Number of items to be created and added to the pool </param>
        public void PreloadPool(int numberOfItems)
        {
            Initialize();
            numberOfItems = Mathf.Max(0, numberOfItems);
            for (int i = 0; i < numberOfItems; i++)
            {
                // T newItem = Activator.CreateInstance<T>();
                var newItem = new T
                {
                    inPool = false
                };
                AddToPool(newItem);
            }

            if (CallbacksEnabled) OnPoolUpdate?.Invoke();
        }

        /// <summary> Resize the pool to the given size. Destroy and remove items from the pool until it reaches the desired size. </summary>
        /// <param name="targetPoolSize"> Maximum number of items left in the pool after this method run </param>
        public void TrimPool(int targetPoolSize)
        {
            Initialize();
            targetPoolSize = Mathf.Max(0, targetPoolSize);
            if (pool.Count <= targetPoolSize) return;
            while (pool.Count > targetPoolSize)
            {
                int lastPoolItemIndex = pool.Count - 1;
                pool[lastPoolItemIndex].Dispose();
                pool.RemoveAt(lastPoolItemIndex);
            }

            if (CallbacksEnabled) OnPoolUpdate?.Invoke();
        }

        /// <summary> Set a maximum pool size. If the pool size reaches the maximum value, any new item added to the pool will get automatically destroyed </summary>
        /// <param name="size"> This also resizes the pool (to save memory) </param>
        public void SetMaximumPoolSize(int size)
        {
            Initialize();
            maxPoolSize = size;
            TrimPool(maxPoolSize);

            if (CallbacksEnabled) OnPoolUpdate?.Invoke();
        }

        /// <summary> Clears the maximum pool size limitation </summary>
        public void ClearMaxPoolSize()
        {
            Initialize();
            maxPoolSize = -1;
        }

        /// <summary> Call Destroy on all pooled items and then clear the pool </summary>
        /// <param name="clearMaxPoolSize"> Clear the maximum pool size limitation </param>
        public void ClearPool(bool clearMaxPoolSize = false)
        {
            Initialize();
            foreach (T item in pool)
                item.Dispose();
            pool.Clear();

            if (clearMaxPoolSize) ClearMaxPoolSize();

            if (CallbacksEnabled) OnPoolUpdate?.Invoke();
        }

        private void RemoveNulls()
        {
            Initialize();
            for (int i = pool.Count - 1; i >= 0; i--)
                if (pool[i] == null)
                    pool.RemoveAt(i);
        }
    }
}
