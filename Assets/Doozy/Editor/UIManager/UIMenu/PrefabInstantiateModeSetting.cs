// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Editor.UIManager.UIMenu
{
    public enum PrefabInstantiateModeSetting
    {
        /// <summary> Instantiate according to the menu item's setting </summary>
        Default,
        
        /// <summary> Instantiate a disconnected clone of the prefab </summary>
        Clone,
        
        /// <summary> Instantiate a linked clone of the prefab </summary>
        Link
        
    }
}
