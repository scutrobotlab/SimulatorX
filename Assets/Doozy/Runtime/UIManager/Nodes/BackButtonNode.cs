// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Nody.Nodes.Internal;
using Doozy.Runtime.UIManager.Input;
// ReSharper disable RedundantOverriddenMember

namespace Doozy.Runtime.UIManager.Nodes
{
    /// <summary>
    /// The Back Button Node enables or disables the ‘Back’ button functionality and jumps instantly to the next node in the Graph.
    /// </summary>
    [Serializable]
    [NodyMenuPath("UI Manager", "Back Button")]
    public sealed class BackButtonNode : SimpleNode
    {
        public enum Command
        {
            Disable,
            Enable,
            EnableByForce
        }

        public Command NodeCommand = Command.Enable;

        public BackButtonNode()
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

            switch (NodeCommand)
            {
                case Command.Disable:
                    BackButton.Disable();
                    break;
                case Command.Enable:
                    BackButton.Enable();
                    break;
                case Command.EnableByForce:
                    BackButton.EnableByForce();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            GoToNextNode(firstOutputPort);
        }
    }
}
