// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Nody.Nodes.Internal;
using Doozy.Runtime.Signals;
// ReSharper disable RedundantOverriddenMember

namespace Doozy.Runtime.UIManager.Nodes
{
    /// <summary>
    /// The Signal Node sends a signal on the given stream (with or without a custom payload), activates the node connected to it.
    /// </summary>
    [Serializable]
    [NodyMenuPath("UI Manager", "Signal")]
    public sealed class SignalNode : SimpleNode
    {
        public SignalPayload Payload;
        
        public SignalNode()
        {
            AddInputPort()                 
                .SetCanBeDeleted(false)    
                .SetCanBeReordered(false); 

            AddOutputPort()                
                .SetCanBeDeleted(false)    
                .SetCanBeReordered(false);
        }

        public override void OnEnter(FlowNode previousNode = null, FlowPort previousPort = null)
        {
            base.OnEnter(previousNode, previousPort);
            Payload?.SendSignal();
            GoToNextNode(firstOutputPort);
        }
    }
}