// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Editor.EditorUI
{
    public enum InfoDotState
    {
        /// <summary>
        /// An invisible states makes the dot not visible in the UI layout
        /// </summary>
        Invisible = 0,
        
        /// <summary>
        /// An info state communicates a description or usage message
        /// </summary>
        Info = 1,
        
        /// <summary>
        /// A correct state communicates that current system settings are ok 
        /// </summary>
        Correct = 2,
        
        /// <summary>
        /// A warning state communicates system possible issue
        /// </summary>
        Warning = 3,
        
        /// <summary>
        /// An error state communicates a user or system mistake
        /// </summary>
        Error = 4
    }
}
