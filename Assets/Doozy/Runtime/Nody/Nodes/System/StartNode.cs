// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using Doozy.Runtime.Nody.Nodes.Internal;

namespace Doozy.Runtime.Nody.Nodes.System
{
    /// <summary>
    /// This is the first node that gets activated in any graph.
    /// It’s treated as the root node and it cannot be moved.
    /// </summary>
    [Serializable]
    public sealed class StartNode : SystemNode
    {
        public override int minNumberOfInputPorts => 0;
        public override int minNumberOfOutputPorts => 1;

        public StartNode() : base(SystemNodeType.Start)
        {
            AddOutputPort();
            lastOutputPort
                .SetCanBeDeleted(false)
                .SetCanBeReordered(false);

            clearGraphHistory = true;
        }

        public override void OnEnter(FlowNode previousNode = null, FlowPort previousPort = null)
        {
            base.OnEnter(previousNode, previousPort);
            GoToNextNode(firstOutputPort);
        }

        public override FlowNode Clone() =>
            Instantiate(this);
    }
}
