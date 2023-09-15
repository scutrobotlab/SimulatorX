// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.Nody.Nodes.Internal;
using Doozy.Runtime.Nody.Nodes;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Nody.Nodes
{
    [CustomEditor(typeof(ApplicationQuitNode))]
    public class ApplicationQuitNodeEditor : FlowNodeEditor
    {
        public override IEnumerable<Texture2D> nodeIconTextures => EditorMicroAnimations.Nody.Icons.ApplicationQuitNode;

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(ApplicationQuitNode)))
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048805446/Application+Quit+Node?atlOrigin=eyJpIjoiYmIzODEzYTM2YTE0NGNmOWE3ODExMmM3ZDc0MzE1NmIiLCJwIjoiYyJ9")
                .AddYouTubeButton(); 
        }
    }
}