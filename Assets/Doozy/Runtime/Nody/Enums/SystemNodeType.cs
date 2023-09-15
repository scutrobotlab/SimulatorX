// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Runtime.Nody
{
    /// <summary> Describes the types of system nodes </summary>
    public enum SystemNodeType
    {
        /// <summary> First node that becomes active (start node) </summary>
        Start = 0,
        
        /// <summary> First node that becomes active when entering a sub-flow </summary>
        Enter = 1,
        
        /// <summary> Last node that becomes active when exiting a sub-flow </summary>
        Exit = 2
    }
}
