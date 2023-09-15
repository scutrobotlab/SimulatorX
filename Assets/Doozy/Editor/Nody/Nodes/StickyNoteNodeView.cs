// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Nody.Nodes;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.Nody.Nodes
{
    public sealed class StickyNoteNodeView : FlowNodeView
    {
        public override Type nodeType => typeof(StickyNoteNode);
        public override Texture2D nodeIconTexture => EditorTextures.Nody.Icons.StickyNoteNode;
        public override IEnumerable<Texture2D> nodeIconTextures => EditorMicroAnimations.Nody.Icons.StickyNoteNode;
        public override Color nodeAccentColor => EditorColors.Nody.StickyNote;

        public StickyNoteNodeView(FlowGraphView graphView, FlowNode node) : base(graphView, node)
        {
            nodeBorder
                .SetStylePaddingBottom(0);
            
            nodeContent
                .SetStyleDisplay(DisplayStyle.None);


            nodeIcon
                .SetStyleBackgroundImageTintColor(EditorColors.Nody.LineColor);

            nodeTitleLabel
                .SetStyleColor(EditorColors.Nody.NodeBackground);

            nodeDescriptionLabel
                .SetStyleColor(nodeAccentColor);
            
            titleContainer
                .SetStyleBackgroundColor(nodeAccentColor);
        }
    }
}
