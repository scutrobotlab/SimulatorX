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
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.Nody.Nodes
{
    [CustomEditor(typeof(PivotNode))]
    public class PivotNodeEditor : FlowNodeEditor
    {
        public override IEnumerable<Texture2D> nodeIconTextures => EditorMicroAnimations.Nody.Icons.PivotNode;

        private EnumField pivotOrientationEnumField { get; set; }
        private FluidField pivotOrientationField { get; set; }
        private SerializedProperty propertyPivotOrientation { get; set; }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyPivotOrientation = serializedObject.FindProperty("PivotOrientation");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(PivotNode)))
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048871142/Pivot+Node?atlOrigin=eyJpIjoiZTY5MzhiZGJmOGRlNGM4NDlmNjllY2IwN2E1OTcwNjYiLCJwIjoiYyJ9")
                .AddYouTubeButton();

            nodeNameField.SetEnabled(false);

            nodeDescriptionField
                .SetStyleDisplay(DisplayStyle.None)
                .SetEnabled(false);

            pivotOrientationEnumField =
                DesignUtils.NewEnumField(propertyPivotOrientation)
                    .SetStyleFlexGrow(1);

            pivotOrientationField =
                FluidField.Get()
                    .SetLabelText("Pivot Orientation")
                    .AddFieldContent(pivotOrientationEnumField);

            pivotOrientationEnumField.RegisterValueChangedCallback(evt =>
            {
                if (nodeView == null)
                    return;
                
                ((PivotNodeView)nodeView).OnOrientationChanged((PivotNode.Orientation)evt.newValue);
            });
        }

        protected override void Compose()
        {
            base.Compose();

            root
                .AddChild(pivotOrientationField);
        }
    }
}
