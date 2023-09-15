// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Nody.Nodes.Internal;

namespace Doozy.Runtime.Nody.Nodes.System
{
    [Serializable]
    public sealed class EnterNode : SystemNode
    {
        public override int minNumberOfInputPorts => 0;
        public override int minNumberOfOutputPorts => 1;

        public EnterNode() : base(SystemNodeType.Enter)
        {
            AddOutputPort();
            lastOutputPort
                .SetCanBeDeleted(false)
                .SetCanBeReordered(false);
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
