// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Runtime.Nody
{
    /// <summary> Defines the states of a graph </summary>
    public enum GraphState
    {
        /// <summary> Graph is disabled </summary>
        Disabled,
        
        /// <summary> Graph is waiting to become active </summary>
        Idle,
        
        /// <summary> Graph is active </summary>
        Active
    }
}
