// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Nody.Nodes.Internal;
// ReSharper disable RedundantOverriddenMember

namespace Doozy.Runtime.Nody.Nodes
{
    /// <summary>
    /// Exits play mode (if in the Unity Editor) or quits the application if in build mode
    /// </summary>
    [Serializable]
    [NodyMenuPath("System", "Application Quit")]
    public sealed class ApplicationQuitNode : SimpleNode
    {
        public ApplicationQuitNode()
        {
            AddInputPort()
                .SetCanBeDeleted(false)
                .SetCanBeReordered(false);
        }

        public override void OnEnter(FlowNode previousNode = null, FlowPort previousPort = null)
        {
            base.OnEnter(previousNode, previousPort);
            #if UNITY_EDITOR
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
            #else
            {
                UnityEngine.Application.Quit();
            }
            #endif
        }
    }
}
