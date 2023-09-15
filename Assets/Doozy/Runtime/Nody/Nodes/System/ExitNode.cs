// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Nody.Nodes.Internal;

namespace Doozy.Runtime.Nody.Nodes.System
{
    [Serializable]
    public sealed class ExitNode : SystemNode
    {
        public override int minNumberOfInputPorts => 1;
        public override int minNumberOfOutputPorts => 0;

        public ExitNode() : base(SystemNodeType.Exit)
        {
            AddInputPort();
            lastInputPort
                .SetCanBeDeleted(false)
                .SetCanBeReordered(false);
        }

        public override FlowNode Clone() =>
            Instantiate(this);
    }
}
