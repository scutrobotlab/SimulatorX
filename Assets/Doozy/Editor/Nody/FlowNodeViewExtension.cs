// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

//.........................
//.....Generated Class.....
//.........................
//.......Do not edit.......
//.........................

using Doozy.Runtime.Nody;
namespace Doozy.Editor.Nody
{
    public partial class FlowNodeView
    {
        public static FlowNodeView GetView(FlowGraphView graphView, FlowNode node) =>
            node switch
            {
                Doozy.Runtime.UIManager.Nodes.BackButtonNode _ => new Doozy.Editor.UIManager.Nodes.BackButtonNodeView(graphView, node),
                Doozy.Runtime.UIManager.Nodes.PortalNode _ => new Doozy.Editor.UIManager.Nodes.PortalNodeView(graphView, node),
                Doozy.Runtime.UIManager.Nodes.SignalNode _ => new Doozy.Editor.UIManager.Nodes.SignalNodeView(graphView, node),
                Doozy.Runtime.UIManager.Nodes.UINode _ => new Doozy.Editor.UIManager.Nodes.UINodeView(graphView, node),
                Doozy.Runtime.Nody.Nodes.ApplicationQuitNode _ => new Doozy.Editor.Nody.Nodes.ApplicationQuitNodeView(graphView, node),
                Doozy.Runtime.Nody.Nodes.DebugNode _ => new Doozy.Editor.Nody.Nodes.DebugNodeView(graphView, node),
                Doozy.Runtime.Nody.Nodes.PivotNode _ => new Doozy.Editor.Nody.Nodes.PivotNodeView(graphView, node),
                Doozy.Runtime.Nody.Nodes.RandomNode _ => new Doozy.Editor.Nody.Nodes.RandomNodeView(graphView, node),
                Doozy.Runtime.Nody.Nodes.StickyNoteNode _ => new Doozy.Editor.Nody.Nodes.StickyNoteNodeView(graphView, node),
                Doozy.Runtime.Nody.Nodes.TimeScaleNode _ => new Doozy.Editor.Nody.Nodes.TimeScaleNodeView(graphView, node),
                Doozy.Runtime.Nody.Nodes.System.EnterNode _ => new Doozy.Editor.Nody.Nodes.EnterNodeView(graphView, node),
                Doozy.Runtime.Nody.Nodes.System.ExitNode _ => new Doozy.Editor.Nody.Nodes.ExitNodeView(graphView, node),
                Doozy.Runtime.Nody.Nodes.System.StartNode _ => new Doozy.Editor.Nody.Nodes.StartNodeView(graphView, node),               
                _           => new FlowNodeView(graphView, node)
            };
    }
}