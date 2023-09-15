// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Nody.Nodes.Internal;
using UnityEngine;

namespace Doozy.Runtime.Nody.Nodes
{
    /// <summary>
    /// Print a debug message in the Console
    /// </summary>
    [Serializable]
    [NodyMenuPath("Utils", "Debug")]
    public sealed class DebugNode : SimpleNode
    {
        public override int minNumberOfInputPorts => 1;
        public override int minNumberOfOutputPorts => 1;

        [SerializeField] private string Message;
        public string message
        {
            get => Message;
            set => Message = value;
        }

        public DebugNode()
        {
            AddInputPort();
            lastInputPort
                .SetCanBeDeleted(false)
                .SetCanBeReordered(false);

            AddOutputPort();
            lastOutputPort
                .SetCanBeDeleted(false)
                .SetCanBeReordered(false);
        }

        public override void OnEnter(FlowNode previousNode = null, FlowPort previousPort = null)
        {
            base.OnEnter(previousNode, previousPort);

            if (!Message.IsNullOrEmpty())
                Debug.Log(Message);

            GoToNextNode(firstOutputPort);
        }



        public override FlowNode Clone() =>
            Instantiate(this);

    }
}
