// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.Nody.Nodes.Internal;
using Doozy.Runtime.Nody.Nodes;
using UnityEditor;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Nody.Nodes
{
    [CustomEditor(typeof(StickyNoteNode))]
    public class StickyNoteNodeEditor : FlowNodeEditor
    {
        public override Color nodeAccentColor => EditorColors.Nody.StickyNote;
        public override IEnumerable<Texture2D> nodeIconTextures => EditorMicroAnimations.Nody.Icons.StickyNoteNode;
        
        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(StickyNoteNode)))
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048510532/Sticky+Note+Node?atlOrigin=eyJpIjoiZTQ5OTRhMDljYWMzNGIxZDk5MGIyMzAzZWJjMWJiN2IiLCJwIjoiYyJ9")
                .AddYouTubeButton();
        }
    }
}
