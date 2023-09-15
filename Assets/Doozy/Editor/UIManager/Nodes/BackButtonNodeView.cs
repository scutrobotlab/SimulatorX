// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Nody;
using Doozy.Runtime.Nody;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Nodes;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Nodes
{
    public class BackButtonNodeView : FlowNodeView
    {
        public override Type nodeType => typeof(BackButtonNode);
        public override Texture2D nodeIconTexture => EditorTextures.Nody.Icons.BackButtonNode;
        public override IEnumerable<Texture2D> nodeIconTextures => EditorMicroAnimations.Nody.Icons.BackButtonNode;

        private SerializedProperty propertyNodeCommand { get; }
        private Label actionNameLabel { get; }

        public BackButtonNodeView(FlowGraphView graphView, FlowNode node) : base(graphView, node)
        {
            propertyNodeCommand = serializedObject.FindProperty(nameof(BackButtonNode.NodeCommand));

            actionNameLabel =
                DesignUtils.NewFieldNameLabel(GetCommandName((BackButtonNode.Command)propertyNodeCommand.enumValueIndex))
                    .SetStyleFontSize(8);
            
            EnumField invisibleEnumField = DesignUtils.NewEnumField(propertyNodeCommand, true);
            invisibleEnumField.RegisterValueChangedCallback(evt =>
            {
                if (evt?.newValue == null) return;
                actionNameLabel.SetText(GetCommandName((BackButtonNode.Command)evt.newValue));
            });

            portDivider
                .SetStyleBackgroundColor(EditorColors.Nody.MiniMapBackground)
                .SetStyleMarginLeft(DesignUtils.k_Spacing)
                .SetStyleMarginRight(DesignUtils.k_Spacing)
                .SetStylePadding(DesignUtils.k_Spacing)
                .SetStyleBorderRadius(DesignUtils.k_Spacing)
                .SetStyleJustifyContent(Justify.Center)
                .SetStyleAlignItems(Align.Center);

            portDivider
                .AddChild(invisibleEnumField)
                .AddChild(actionNameLabel);

            portDivider.Bind(serializedObject);
        }

        private string GetCommandName(BackButtonNode.Command command) =>
            ObjectNames.NicifyVariableName(command.ToString());
    }
}
