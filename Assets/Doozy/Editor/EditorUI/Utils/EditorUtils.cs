// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Doozy.Editor.EditorUI.Utils
{
    public static class EditorUtils
    {
         /// <summary>
        /// Sort the component order in the Inspector by component name
        /// <para/> This method reorders any components found on the selected target gameObject (in the Inspector)
        /// <para/> The custom sorted component names defines a fixed (custom) order before the sorted order
        /// </summary>
        /// <param name="gameObject"> Target (selected) gameObject </param>
        /// <param name="customSortedComponentNames">
        /// First component names set in a custom order
        /// <para/> Example (strings): "RectTransform", "Canvas", "UIView", "Miau"
        /// <para/> Example (class names): nameof(RectTransform), nameof(Canvas), nameof(UIView), nameof(Miau)
        /// </param>
        public static void SortComponents(GameObject gameObject, params string[] customSortedComponentNames)
        {
            Component[] initialComponentsArray = gameObject.GetComponents(typeof(Component));
            var componentsList = initialComponentsArray.ToList();

            var sortedComponentsList = new List<Component>();

            void TransferComponent(string componentName, ICollection<Component> sourceList, ICollection<Component> targetList)
            {
                foreach (Component c in sourceList.Where(c => c.GetType().Name.Equals(componentName)))
                {
                    targetList.Add(c);
                    sourceList.Remove(c);
                    break;
                }
            }

            if (customSortedComponentNames != null)
                foreach (string componentName in customSortedComponentNames)
                    TransferComponent(componentName, componentsList, sortedComponentsList);

            var list = componentsList.Select(c => new KeyValuePair<Component, string>(c, c.GetType().Name)).ToList();
            list.Sort((a, b) => string.Compare(a.Value, b.Value, StringComparison.Ordinal));
            sortedComponentsList.AddRange(list.Select(pair => pair.Key));

            for (int i = 0; i < sortedComponentsList.Count; i++)
            {
                Component component = sortedComponentsList[i];
                int initialIndex = Array.FindIndex(initialComponentsArray, c => c == component);
                int indexDifference = initialIndex - i;
                if (indexDifference <= 0) continue;
                for (int j = 0; j < indexDifference; j++) UnityEditorInternal.ComponentUtility.MoveComponentUp(component);
            }
        }
    }
}
