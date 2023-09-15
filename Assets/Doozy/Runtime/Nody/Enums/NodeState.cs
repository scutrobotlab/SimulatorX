// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Runtime.Nody
{
    /// <summary> Defines the states of a node </summary>
    public enum NodeState
    {
        /// <summary> Node is waiting to become active </summary>
        Idle,

        /// <summary> Node is running, but it's not the active node (only one node can be active) </summary>
        Running,

        /// <summary> Node is running and active (only one node can be active) </summary>
        Active
    }
}
