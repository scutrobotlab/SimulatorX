// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Nody;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.Nody.Editors
{
    [CustomEditor(typeof(FlowGraph), true)]
    public class FlowGraphEditor : UnityEditor.Editor
    {
        private static IEnumerable<Texture2D> flowGraphIconTextures => EditorMicroAnimations.Nody.Icons.FlowGraph;
        
        private static Color accentColor => EditorColors.Nody.Color;
        private static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Nody.Color;
        
        private FlowGraph castedTarget => (FlowGraph)target;
        private IEnumerable<FlowGraph> castedTargets => targets.Cast<FlowGraph>();
        
        private VisualElement root { get; set; }
        
        private FluidComponentHeader componentHeader { get; set; }
        
        private SerializedProperty propertyId { get; set; }
        private SerializedProperty propertyGraphName { get; set; }
        private SerializedProperty propertyGraphDescription { get; set; }
        private SerializedProperty propertyIsSubGraph { get; set; }
        private SerializedProperty propertyNodes { get; set; }
        private SerializedProperty propertyRootNode { get; set; }
        private SerializedProperty propertyActiveNode { get; set; }
        
        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        private void OnDestroy()
        {
            componentHeader?.Recycle();
        }
        
        private void FindProperties()
        {
            propertyId = serializedObject.FindProperty("Id");
            propertyGraphName = serializedObject.FindProperty("GraphName");
            propertyGraphDescription = serializedObject.FindProperty("GraphDescription");
            propertyIsSubGraph = serializedObject.FindProperty("IsSubGraph");
            propertyRootNode = serializedObject.FindProperty("RootNode");
            propertyActiveNode = serializedObject.FindProperty("ActiveNode");
            propertyNodes = serializedObject.FindProperty("Nodes");
        }

        private void InitializeEditor()
        {
            FindProperties();

            root = new VisualElement();

            componentHeader = 
                FluidComponentHeader.Get()
                .SetElementSize(ElementSize.Large)
                .SetAccentColor(accentColor)
                .SetComponentNameText((ObjectNames.NicifyVariableName(nameof(FlowGraph))))
                .SetIcon(flowGraphIconTextures.ToList())
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048182819/Flow+Graph?atlOrigin=eyJpIjoiYWU4MDJiOTg5MWEwNDUzZTg1ZDUyZWU2N2U1NTRkODMiLCJwIjoiYyJ9")
                .AddYouTubeButton();
        }

        private void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(DesignUtils.NewPropertyField(propertyGraphName))
                .AddChild(DesignUtils.NewPropertyField(propertyGraphDescription))
                .AddChild(DesignUtils.NewPropertyField(propertyId).DisableElement())
                .AddChild(DesignUtils.NewPropertyField(propertyIsSubGraph).DisableElement())
                .AddChild(DesignUtils.NewPropertyField(propertyRootNode).DisableElement())
                .AddChild(DesignUtils.NewPropertyField(propertyActiveNode).DisableElement())
                .AddChild(DesignUtils.NewPropertyField(propertyNodes).DisableElement())
                ;
        }
    }
}
