// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Doozy.Runtime.UIManager.Layouts.Internal
{
    /// <summary>
    /// Base class for layout groups
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public abstract class UILayoutGroup : LayoutGroup
    {
        /// <summary> Reference to the UIManager Input Settings </summary>
        public static UIManagerInputSettings inputSettings => UIManagerInputSettings.instance;
    }
}
