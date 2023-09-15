// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Nody.Nodes.System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Doozy.Editor.Nody.Nodes
{
    public class StartNodeView : FlowNodeView
    {
        public override Type nodeType => typeof(StartNode);
        public override Texture2D nodeIconTexture => EditorTextures.Nody.Icons.StartNode;
        public override IEnumerable<Texture2D> nodeIconTextures => EditorMicroAnimations.Nody.Icons.StartNode;
        public override Color nodeAccentColor => EditorColors.Nody.StickyNote;
        
        public StartNodeView(FlowGraphView graphView, FlowNode node) : base(graphView, node)
        {
            capabilities &= ~Capabilities.Movable;
            capabilities &= ~Capabilities.Deletable;
        }
        
        protected override void InitializeView()
        {
            base.InitializeView();

            this.SetIcon(EditorMicroAnimations.Nody.Icons.StartNode);
        }
    }
}
