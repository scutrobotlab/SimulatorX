// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Runtime.UIManager
{
    /// <summary> Visibility states for components that implement IShowHide </summary>
    public enum VisibilityState
    {
        /// <summary> Is Visible </summary>
        Visible,
        
        /// <summary> Is Hidden </summary>
        Hidden,
        
        /// <summary> Show animation - is in the process (transition) of becoming visible </summary>
        IsShowing,
        
        /// <summary> Hide animation - is in the process (transition) of becoming hidden </summary>
        IsHiding
    }
}
