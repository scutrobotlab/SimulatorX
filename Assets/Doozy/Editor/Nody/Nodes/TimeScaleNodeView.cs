// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Nody.Nodes;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.Nody.Nodes
{
    public class TimeScaleNodeView : FlowNodeView
    {
        public override Type nodeType => typeof(TimeScaleNode);
        public override IEnumerable<Texture2D> nodeIconTextures => EditorMicroAnimations.Nody.Icons.TimeScaleNode;

        private SerializedProperty propertyTargetValue { get; }
        private Label targetValueLabel { get; }
        
        public TimeScaleNodeView(FlowGraphView graphView, FlowNode node) : base(graphView, node)
        {
            propertyTargetValue = serializedObject.FindProperty(nameof(TimeScaleNode.TargetValue));

            targetValueLabel =
                DesignUtils.NewFieldNameLabel($"{propertyTargetValue.floatValue.Round(2)}")
                    .SetStyleFontSize(12);

            FloatField iTargetValueFloatField = DesignUtils.NewFloatField(propertyTargetValue, true);
            iTargetValueFloatField.RegisterValueChangedCallback(evt =>
            {
                if (evt?.newValue == null) return;
                targetValueLabel.SetText($"{evt.newValue.Round(2)}");
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
                .AddChild(iTargetValueFloatField)
                .AddChild(targetValueLabel);

            portDivider.Bind(serializedObject);
        }
    }
}
