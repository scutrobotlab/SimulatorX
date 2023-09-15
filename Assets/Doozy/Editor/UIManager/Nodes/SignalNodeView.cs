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
    public class SignalNodeView : FlowNodeView
    {
        public override Type nodeType => typeof(SignalNode);
        public override Texture2D nodeIconTexture => EditorTextures.Nody.Icons.SignalNode;
        public override IEnumerable<Texture2D> nodeIconTextures => EditorMicroAnimations.Nody.Icons.SignalNode;

        private SerializedProperty propertyStreamIdCategory { get; }
        private SerializedProperty propertyStreamIdName { get; }
        private SerializedProperty propertySignalValueType { get; }

        private Label signalIdLabel { get; }

        public SignalNodeView(FlowGraphView graphView, FlowNode node) : base(graphView, node)
        {
            propertyStreamIdCategory = serializedObject.FindProperty("Payload.StreamId.Category");
            propertyStreamIdName = serializedObject.FindProperty("Payload.StreamId.Name");
            propertySignalValueType = serializedObject.FindProperty("Payload.SignalValueType");

            TextField iCategoryTextField = DesignUtils.NewTextField(propertyStreamIdCategory, false, true);
            TextField iNameTextField = DesignUtils.NewTextField(propertyStreamIdName, false, true);
            EnumField iSignalValueTypeEnumField = DesignUtils.NewEnumField(propertySignalValueType, true);

            iCategoryTextField.RegisterValueChangedCallback(evt => RefreshData());
            iNameTextField.RegisterValueChangedCallback(evt => RefreshData());
            iSignalValueTypeEnumField.RegisterValueChangedCallback(evt => RefreshData());

            signalIdLabel = DesignUtils.fieldLabel;

            portDivider
                .SetStyleBackgroundColor(EditorColors.Nody.MiniMapBackground)
                .SetStyleMarginLeft(DesignUtils.k_Spacing)
                .SetStyleMarginRight(DesignUtils.k_Spacing)
                .SetStylePadding(DesignUtils.k_Spacing)
                .SetStyleBorderRadius(DesignUtils.k_Spacing)
                .SetStyleJustifyContent(Justify.Center)
                .SetStyleAlignItems(Align.Center);

            portDivider
                .AddChild(iCategoryTextField)
                .AddChild(iNameTextField)
                .AddChild(iSignalValueTypeEnumField)
                .AddChild(signalIdLabel);

            portDivider.Bind(serializedObject);
        }

        public override void RefreshData()
        {
            base.RefreshData();
            schedule.Execute(() => signalIdLabel.SetText(((SignalNode)flowNode).Payload.ToString()));
        }
    }
}
