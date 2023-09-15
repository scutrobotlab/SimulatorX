// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

//.........................
//.....Generated Class.....
//.........................
//.......Do not edit.......
//.........................

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable ClassNeverInstantiated.Local

namespace Doozy.Editor.Nody
{
    public class NodyNodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private Texture2D transparentIcon { get; set; }
        public FlowGraphView graphView { get; private set; }

        public NodyNodeSearchWindow SetGraphView(FlowGraphView view)
        {
            graphView = view;
            
            // Transparent icon to trick search window into indenting items
            transparentIcon = new Texture2D(1, 1);
            transparentIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            transparentIcon.Apply();
            
            return this;
        }

        private void OnDestroy()
        {
            if (transparentIcon == null)
                return;
            DestroyImmediate(transparentIcon);
            transparentIcon = null;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node", EditorTextures.Nody.Icons.Infinity)),
                new SearchTreeGroupEntry(new GUIContent("System"), 1),
                new SearchTreeEntry(new GUIContent("Application Quit", transparentIcon)) { userData = new NodeTypeInfo(typeof(Doozy.Runtime.Nody.Nodes.ApplicationQuitNode)), level = 2 },
                new SearchTreeGroupEntry(new GUIContent("Time"), 1),
                new SearchTreeEntry(new GUIContent("TimeScale", transparentIcon)) { userData = new NodeTypeInfo(typeof(Doozy.Runtime.Nody.Nodes.TimeScaleNode)), level = 2 },
                new SearchTreeGroupEntry(new GUIContent("UI Manager"), 1),
                new SearchTreeEntry(new GUIContent("Back Button", transparentIcon)) { userData = new NodeTypeInfo(typeof(Doozy.Runtime.UIManager.Nodes.BackButtonNode)), level = 2 },
                new SearchTreeEntry(new GUIContent("Portal", transparentIcon)) { userData = new NodeTypeInfo(typeof(Doozy.Runtime.UIManager.Nodes.PortalNode)), level = 2 },
                new SearchTreeEntry(new GUIContent("Signal", transparentIcon)) { userData = new NodeTypeInfo(typeof(Doozy.Runtime.UIManager.Nodes.SignalNode)), level = 2 },
                new SearchTreeEntry(new GUIContent("UI", transparentIcon)) { userData = new NodeTypeInfo(typeof(Doozy.Runtime.UIManager.Nodes.UINode)), level = 2 },
                new SearchTreeGroupEntry(new GUIContent("Utils"), 1),
                new SearchTreeEntry(new GUIContent("Debug", transparentIcon)) { userData = new NodeTypeInfo(typeof(Doozy.Runtime.Nody.Nodes.DebugNode)), level = 2 },
                new SearchTreeEntry(new GUIContent("Pivot", transparentIcon)) { userData = new NodeTypeInfo(typeof(Doozy.Runtime.Nody.Nodes.PivotNode)), level = 2 },
                new SearchTreeEntry(new GUIContent("Random", transparentIcon)) { userData = new NodeTypeInfo(typeof(Doozy.Runtime.Nody.Nodes.RandomNode)), level = 2 },
                new SearchTreeEntry(new GUIContent("Sticky Note", transparentIcon)) { userData = new NodeTypeInfo(typeof(Doozy.Runtime.Nody.Nodes.StickyNoteNode)), level = 2 },
            };
            return tree;
        }
        
        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            if (!(searchTreeEntry.userData is NodeTypeInfo nodeInfo))
                return false;
            
            if(nodeInfo.type == typeof(Doozy.Runtime.Nody.Nodes.ApplicationQuitNode)) { graphView.CreateNode(typeof(Doozy.Runtime.Nody.Nodes.ApplicationQuitNode), true); return true;}
            if(nodeInfo.type == typeof(Doozy.Runtime.Nody.Nodes.TimeScaleNode)) { graphView.CreateNode(typeof(Doozy.Runtime.Nody.Nodes.TimeScaleNode), true); return true;}
            if(nodeInfo.type == typeof(Doozy.Runtime.UIManager.Nodes.BackButtonNode)) { graphView.CreateNode(typeof(Doozy.Runtime.UIManager.Nodes.BackButtonNode), true); return true;}
            if(nodeInfo.type == typeof(Doozy.Runtime.UIManager.Nodes.PortalNode)) { graphView.CreateNode(typeof(Doozy.Runtime.UIManager.Nodes.PortalNode), true); return true;}
            if(nodeInfo.type == typeof(Doozy.Runtime.UIManager.Nodes.SignalNode)) { graphView.CreateNode(typeof(Doozy.Runtime.UIManager.Nodes.SignalNode), true); return true;}
            if(nodeInfo.type == typeof(Doozy.Runtime.UIManager.Nodes.UINode)) { graphView.CreateNode(typeof(Doozy.Runtime.UIManager.Nodes.UINode), true); return true;}
            if(nodeInfo.type == typeof(Doozy.Runtime.Nody.Nodes.DebugNode)) { graphView.CreateNode(typeof(Doozy.Runtime.Nody.Nodes.DebugNode), true); return true;}
            if(nodeInfo.type == typeof(Doozy.Runtime.Nody.Nodes.PivotNode)) { graphView.CreateNode(typeof(Doozy.Runtime.Nody.Nodes.PivotNode), true); return true;}
            if(nodeInfo.type == typeof(Doozy.Runtime.Nody.Nodes.RandomNode)) { graphView.CreateNode(typeof(Doozy.Runtime.Nody.Nodes.RandomNode), true); return true;}
            if(nodeInfo.type == typeof(Doozy.Runtime.Nody.Nodes.StickyNoteNode)) { graphView.CreateNode(typeof(Doozy.Runtime.Nody.Nodes.StickyNoteNode), true); return true;}
                
            return false;
        }

        private class NodeTypeInfo
        {
            public Type type { get; }
            
            public NodeTypeInfo(Type type)
            {
                this.type = type;
            }
        }
    }
}