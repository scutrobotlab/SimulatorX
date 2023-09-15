// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Nody.Nodes.Internal;
using Doozy.Runtime.Nody.Nodes;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Nody.Nodes
{
    [CustomEditor(typeof(DebugNode))]
    public class DebugNodeEditor : FlowNodeEditor
    {
        public override IEnumerable<Texture2D> nodeIconTextures => EditorMicroAnimations.Nody.Icons.DebugNode;

        private FluidField messageField { get; set; }
        private TextField messageTextField { get; set; }
        
        private SerializedProperty propertyMessage { get; set; }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyMessage = serializedObject.FindProperty("Message");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(DebugNode)))
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048248389/Debug+Node?atlOrigin=eyJpIjoiYTJiMzM2YmNhNTEyNDU3OGE0ZmMyZmNiMzkzODI0ZmYiLCJwIjoiYyJ9")
                .AddYouTubeButton();

            messageTextField =
                DesignUtils
                    .NewTextField(propertyMessage)
                    .SetMultiline(true)
                    .SetStyleFlexGrow(1);
            
            messageField =
                FluidField.Get()
                    .SetLabelText("Debug Message")
                    .AddFieldContent(messageTextField);
        }

        protected override void Compose()
        {
            base.Compose();
            root
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(messageField);
        }
    }
}
