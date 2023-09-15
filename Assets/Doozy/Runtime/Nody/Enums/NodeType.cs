// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Runtime.Nody
{
    /// <summary> Describes node types </summary>
    public enum NodeType
    {
        /// <summary> Simple node is a normal node (most nodes are simple nodes) </summary>
        Simple,
        
        /// <summary> Global node is a node that that is always running </summary>
        Global,
        
        /// <summary> System node type is reserved for nodes that perform specific system generic tasks </summary>
        System
    }
}
