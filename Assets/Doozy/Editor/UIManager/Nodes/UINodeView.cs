// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.Nody;
using Doozy.Editor.UIManager.Nodes.PortData;
using Doozy.Runtime.Nody;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Nodes;
using UnityEngine;

namespace Doozy.Editor.UIManager.Nodes
{
    public sealed class UINodeView : FlowNodeView
    {
        public override void Dispose()
        {
            base.Dispose();
            
            goBackInputPortDataView.Recycle();
            portDataViews.ForEach(item => item?.Recycle());
        }

        public override Type nodeType => typeof(UINode);
        public override Texture2D nodeIconTexture => EditorTextures.Nody.Icons.UINode;
        public override IEnumerable<Texture2D> nodeIconTextures => EditorMicroAnimations.Nody.Icons.UINode;

        private List<UIOutputPortDataView> portDataViews { get; set; } = new List<UIOutputPortDataView>();
        private GoBackInputPortDataView goBackInputPortDataView { get; set; }

        public UINodeView(FlowGraphView graphView, FlowNode node) : base(graphView, node)
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

            goBackInputPortDataView?.Recycle();
            inputPortViews[0].AddChild
            (
                goBackInputPortDataView =
                    GoBackInputPortDataView.Get()
                        .SetPort(flowNode.firstInputPort)
            );
            
            portDataViews.ForEach(item => item?.Recycle());
            portDataViews.Clear();

            for (int i = 0; i < flowNode.outputPorts.Count; i++)
            {
                UIOutputPortDataView portDataView =
                    UIOutputPortDataView.Get()
                        .SetPort(flowNode.outputPorts[i]);
                
                portDataViews.Add(portDataView);
                outputPortViews[i].Insert(0, portDataView);

                if (!portDataView.isBackButton)
                    continue;
                outputPortViews[i].portColor = EditorColors.Nody.BackFlow;
                outputPortViews[i].SetEnabled(false);
                portDataView.iconReaction.SetTextures(EditorMicroAnimations.EditorUI.Icons.Back);
                portDataView.iconReaction.Play();
            }

            RefreshData();

            InjectAddOutputButton();
        }
    }
}
