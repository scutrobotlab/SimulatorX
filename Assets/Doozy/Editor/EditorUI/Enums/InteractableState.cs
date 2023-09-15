// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Editor.EditorUI
{
    /// <summary>
    /// States communicate the status of UI elements to the user
    /// </summary>
    public enum InteractableState
    {
        /// <summary>
        /// A disabled state communicates a noninteractive component or element
        /// </summary>
        Disabled = 0,
        
        /// <summary>
        /// An enabled state communicates an interactive component or element
        /// </summary>
        Enabled = 1,
        
        
        /// <summary>
        /// A hover state communicates when a user has placed a cursor above an interactive element
        /// </summary>
        Hover = 2,
        
        /// <summary>
        /// A focused state communicates when a user has highlighted an element, using an input method such as a keyboard or voice
        /// </summary>
        Focused = 3,
        
        /// <summary>
        /// A selected state communicates a user choice
        /// </summary>
        Selected = 4,
        
        /// <summary>
        /// An activated state communicates a highlighted destination, whether initiated by the user or by default
        /// </summary>
        Activated = 5,
        
        /// <summary>
        /// A pressed state communicates a user tap
        /// </summary>
        Pressed = 6,
        
        /// <summary>
        /// A dragged state communicates when a user presses and moves an element
        /// </summary>
        Dragged
    }
}
