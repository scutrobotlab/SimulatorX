// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.UIManager.Layouts;
using UnityEngine;
using UnityEngine.UI;

namespace Doozy.Runtime.UIManager.Utils
{
    public static class LayoutUtils
    {
        /// <summary> Returns TRUE if the target RectTransform's parent is a LayoutGroup </summary>
        /// <param name="target"> Target RectTransform </param>
        public static bool IsInLayoutGroup(this RectTransform target)
        {
            if (!target.GetLayoutGroupInParent()) return false;
            LayoutElement layoutElement = target.GetComponent<LayoutElement>();
            if (layoutElement == null) return true;
            return layoutElement.ignoreLayout == false;
        }

        /// <summary> Returns the LayoutGroup of the target RectTransform. Returns null if there is no LayoutGroup </summary>
        /// <param name="target"> Target RectTransform </param>
        public static LayoutGroup GetLayoutGroupInParent(this RectTransform target) =>
            target.parent != null ? target.parent.GetComponent<LayoutGroup>() : null;

        /// <summary>
        /// Returns the UIBehaviourHandler attached to the target RectTransform.
        /// If one does not exist, it gets automatically added.
        /// </summary>
        /// <param name="target"> Target RectTransform </param>
        public static UIBehaviourHandler GetUIBehaviourHandler(this RectTransform target)
        {
            UIBehaviourHandler handler = target.GetComponent<UIBehaviourHandler>() ?? target.gameObject.AddComponent<UIBehaviourHandler>();
            return handler;
        }

        /// <summary>
        /// Returns the UIBehaviourHandler attached to the target LayoutGroup.
        /// If one does not exist, it gets automatically added.
        /// </summary>
        /// <param name="target"> Target LayoutGroup </param>
        public static UIBehaviourHandler GetUIBehaviourHandler(this LayoutGroup target)
        {
            UIBehaviourHandler handler = target.GetComponent<UIBehaviourHandler>() ?? target.gameObject.AddComponent<UIBehaviourHandler>();
            return handler;
        }
    }
}
