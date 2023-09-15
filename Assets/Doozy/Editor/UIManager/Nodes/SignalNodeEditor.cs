// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Nody.Nodes.Internal;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Nodes;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.UIManager.Nodes
{
    [CustomEditor(typeof(SignalNode))]
    public class SignalNodeEditor : FlowNodeEditor
    {
        public override IEnumerable<Texture2D> nodeIconTextures => EditorMicroAnimations.Nody.Icons.SignalNode;

        private SerializedProperty propertyPayload { get; set; }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyPayload = serializedObject.FindProperty("Payload");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader.SetComponentNameText(ObjectNames.NicifyVariableName(nameof(SignalNode)))
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048117290/Signal+Node?atlOrigin=eyJpIjoiM2Y5NmExNzVmNDg1NGM3Y2ExZTM5MGZjYjA2MDg3YzEiLCJwIjoiYyJ9")
                .AddYouTubeButton();
        }

        protected override void Compose()
        {
            base.Compose();

            root
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(DesignUtils.NewPropertyField(propertyPayload));
        }
    }
}
