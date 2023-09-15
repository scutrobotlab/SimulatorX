// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;

namespace Doozy.Runtime.Nody.Nodes.Internal
{
    [Serializable]
    public abstract class SimpleNode : FlowNode
    {
        protected SimpleNode() : base(NodeType.Simple)
        {
        }
    }
}
