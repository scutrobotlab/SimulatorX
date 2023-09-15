// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.Nody.Nodes.PortData;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Nody.Nodes;
using UnityEngine;

namespace Doozy.Editor.Nody.Nodes
{
    public sealed class RandomNodeView : FlowNodeView
    {
        public override Type nodeType => typeof(RandomNode);
        public override Texture2D nodeIconTexture => EditorTextures.Nody.Icons.RandomNode;
        public override IEnumerable<Texture2D> nodeIconTextures => EditorMicroAnimations.Nody.Icons.RandomNode;

        private List<RandomNodeOutputPortDataView> portDataViews { get; set; } = new List<RandomNodeOutputPortDataView>();

        public RandomNodeView(FlowGraphView graphView, FlowNode node) : base(graphView, node)
        {
        }

        public override void RefreshData()
        {
            base.RefreshData();
            portDataViews.ForEach(item => item?.RefreshData());
        }
        
        public override void RefreshPortsViews()
        {
            base.RefreshPortsViews();
            
            portDataViews.ForEach(item => item?.Recycle());
            portDataViews.Clear();
            
            for (int i = 0; i < flowNode.outputPorts.Count; i++)
            {
                var portDataView = 
                    RandomNodeOutputPortDataView.Get()
                        .SetPort(flowNode.outputPorts[i]);
                
                portDataViews.Add(portDataView);
                outputPortViews[i].Insert(0, portDataView);
            }

            RefreshData();
            
            InjectAddOutputButton();
        }
    }
}
