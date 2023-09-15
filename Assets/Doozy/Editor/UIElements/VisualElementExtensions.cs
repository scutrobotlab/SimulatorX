// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Linq;
using Doozy.Runtime.Pooler;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIElements
{
    public static class VisualElementExtensions
    {
        /// <summary> Checks all the target's Children if they are IPoolable (and calls Recycle()) </summary>
        /// <param name="target"> Target VisualElement </param>
        /// <param name="clear"> If TRUE, calls Clear() on the target after checking its Children </param>
        public static T RecycleIPoolableChildren<T>(this T target, bool clear = false) where T : VisualElement
        {
            foreach (VisualElement child in target.Children().ToList())
            {
                switch (child)
                {
                    case null:
                        continue;
                    case IPoolable poolable:
                        poolable.Recycle();
                        break;
                }
            }

            if(clear) target.Clear();
            return target;
        }

        /// <summary> Checks all the target's Children if they are IDisposable (and calls Dispose()) </summary>
        /// <param name="target"> Target VisualElement </param>
        ///  /// <param name="clear"> If TRUE, calls Clear() on the target after checking its Children </param>
        public static T DisposeIDisposableChildren<T>(this T target, bool clear = false) where T : VisualElement
        {
            foreach (VisualElement child in target.Children().ToList())
            {
                switch (child)
                {
                    case null:
                        continue;
                    case IDisposable disposable:
                        disposable.Dispose();
                        break;
                }
            }

            if(clear) target.Clear();
            return target;
        }

        /// <summary> Checks all the target's Children if they are IPoolable (calls Recycle()) or IDisposable (calls Dispose()) and then calls Clear() on the target </summary>
        /// <param name="target"> Target VisualElement </param>
        public static T RecycleAndClear<T>(this T target) where T : VisualElement
        {
            foreach (VisualElement child in target.Children().ToList())
            {
                switch (child)
                {
                    case null:
                        continue;
                    case IPoolable poolable:
                        poolable.Recycle();
                        continue;
                    case IDisposable disposable:
                        disposable.Dispose();
                        break;
                }
            }
            target.Clear();

            return target;
        }

        #region ObjectField

        public static T SetObjectType<T>(this T target, Type objectType) where T : ObjectField
        {
            target.objectType = objectType;
            return target;
        }

        public static T SetAllowSceneObjects<T>(this T target, bool allowSceneObjects) where T : ObjectField
        {
            target.allowSceneObjects = allowSceneObjects;
            return target;
        }

        #endregion
    }
}
