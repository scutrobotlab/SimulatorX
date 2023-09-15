// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Nody.Nodes.System;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Doozy.Editor.Nody
{
    public class FlowGraphView : GraphView
    {
        public Action<FlowNodeView> OnNodeSelected;

        public static NodyInspectorWindow inspector => NodyInspectorWindow.instance;

        public FlowGraph flowGraph { get; internal set; }
        public NodyNodeSearchWindow searchWindow { get; private set; }

        private GridBackground gridBackground { get; set; }

        public Vector2 cachedMousePosition { get; private set; }
        public Vector2 graphMousePosition => contentViewContainer.WorldToLocal(cachedMousePosition);

        public ContentDragger contentDragger { get; }
        public SelectionDragger selectionDragger { get; }
        public RectangleSelector rectangleSelector { get; }

        public FlowGraphView()
        {
            InjectGridBackground();
            CreateSearchWindow();

            this.SetStyleFlexGrow(1);
            this.AddManipulator(contentDragger = new ContentDragger());
            this.AddManipulator(selectionDragger = new SelectionDragger());
            this.AddManipulator(rectangleSelector = new RectangleSelector());
            SetupZoom(0.1f, 5f);

            RegisterCallback<MouseMoveEvent>(OnMouseMoveEvent);
            Undo.undoRedoPerformed += UndoRedoPerformed;
        }

        private void InjectGridBackground()
        {
            Insert(0, gridBackground = new GridBackground()); //ToDo: check if a simple image would not be better as the background

            // gridBackground.RegisterCallback<CustomStyleResolvedEvent>(evt =>
            // {
            //     typeof(GridBackground).GetField("m_GridBackgroundColor", BindingFlags.NonPublic | BindingFlags.Instance)?
            //         .SetValue(gridBackground, EditorColors.Nody.GridBackground);
            //
            //     typeof(GridBackground).GetField("m_LineColor", BindingFlags.NonPublic | BindingFlags.Instance)?
            //         .SetValue(gridBackground, EditorColors.Nody.LineColor);
            //
            //     typeof(GridBackground).GetField("m_ThickLineColor", BindingFlags.NonPublic | BindingFlags.Instance)?
            //         .SetValue(gridBackground, EditorColors.Nody.ThickLineColor);
            //
            //     typeof(GridBackground).GetField("m_Spacing", BindingFlags.NonPublic | BindingFlags.Instance)?
            //         .SetValue(gridBackground, 12);
            // });

        }

        private void CreateSearchWindow()
        {
            searchWindow =
                ScriptableObject
                    .CreateInstance<NodyNodeSearchWindow>()
                    .SetGraphView(this);

            nodeCreationRequest =
                context =>
                    SearchWindow.Open
                    (
                        new SearchWindowContext(context.screenMousePosition),
                        searchWindow
                    );
        }

        private void OnMouseMoveEvent(IMouseEvent evt)
        {
            cachedMousePosition = evt.mousePosition;
        }

        public void UndoRedoPerformed()
        {
            // Debug.Log($"UndoRedoPerformed");
            ClearGraphView();              //clear graph view
            if (flowGraph == null) return; //stop if no graph is loaded
            flowGraph.UpdateNodes();       //update nodes
            RefreshNodeViews();            //create node views
            RefreshEdges();                //create edges
            EditorUtility.SetDirty(flowGraph);
            // AssetDatabase.SaveAssetIfDirty(flowGraph);
            // AssetDatabase.SaveAssets();
            NodyInspectorWindow.instance.UpdateSelection((FlowNodeView)selection.FirstOrDefault());
        }

        public void ClearGraphView()
        {
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged;

            if (NodyInspectorWindow.isOpen)
                inspector.ClearInspector();
        }

        internal void PopulateView(FlowGraph graph)
        {
            // Debug.Log($"PopulateView");
            ClearGraphView();              // clear the view (from the previous graph)
            flowGraph = graph;             // set graph reference 
            if (flowGraph == null) return; // graph null -> stop
            CleanGraph();                  // clean graph (remove nulls)
            AddRootNode();                 // add root node (if missing)
            flowGraph.UpdateNodes();       // update nodes
            RefreshNodeViews();            // create node views
            RefreshEdges();                // create edges
        }

        public void RefreshNodeViews()
        {
            if (flowGraph == null) return;
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(nodes.ToList());
            graphViewChanged += OnGraphViewChanged;
            flowGraph.nodes.ForEach(node =>
            {
                var nodeView = CreateNodeView(node);
                nodeView.UpdatePresenterPosition();
            });
        }

        public void RefreshEdges()
        {
            if (flowGraph == null) return;
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(edges.ToList());
            graphViewChanged += OnGraphViewChanged;
            flowGraph.outputPorts.ForEach(outputPort =>
            {
                if (!outputPort.isConnected) return;
                FlowPortView outputPortView = GetPortView(outputPort);
                if (outputPortView == null) return;
                foreach (string inputPortId in outputPort.connections)
                {
                    FlowPortView inputPortView = GetPortView(inputPortId);
                    if (inputPortView == null) continue;
                    FlowEdgeView edgeView = outputPortView.ConnectTo(inputPortView);
                    AddElement(edgeView);
                }
            });
        }

        public void CenterGraphOnNode(FlowNode node, bool selectNode = false)
        {
            schedule.Execute(() =>
            {
                if (flowGraph == null) return;
                if (node == null) return;
                FlowNodeView nodeView = GetNodeView(node);
                ClearSelection();
                AddToSelection(nodeView);
                FrameSelection();
                if (selectNode) return;
                ClearSelection();
            });
        }

        private IEnumerable<FlowPortView> portViews => ports.ToList().Cast<FlowPortView>();

        private FlowPortView GetPortView(FlowPort flowPort) =>
            portViews.FirstOrDefault(portView => portView.flowPort == flowPort);

        private FlowPortView GetPortView(string portId) =>
            portViews.FirstOrDefault(portView => portView.flowPort.portId == portId);

        private FlowNodeView GetNodeView(FlowNode flowNode) =>
            GetNodeByGuid(flowNode.nodeId) as FlowNodeView;

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) =>
            ports
                .ToList()
                .Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node)
                .ToList();

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            graphViewChange.elementsToRemove?.ForEach(element =>
            {
                switch (element)
                {
                    case null:
                        return;

                    //Edge Removal
                    case FlowEdgeView edgeView:
                        DisconnectPorts(edgeView);
                        break;

                    //Node Removal
                    case FlowNodeView nodeView:
                        DeleteNode(nodeView);
                        break;
                }

            });

            //Edge Creation
            graphViewChange.edgesToCreate?.ForEach(edge =>
            {
                if (!(edge is FlowEdgeView edgeView))
                    return;

                var outputPortView = (FlowPortView)edgeView.output;
                if (outputPortView == null) return;
                edgeView.outputPortView = outputPortView;

                var inputPortView = (FlowPortView)edgeView.input;
                if (inputPortView == null) return;
                edgeView.inputPortView = inputPortView;

                ConnectPorts(edgeView.outputPortView.flowPort, inputPortView.flowPort);
            });
            
            // AssetDatabase.SaveAssets();
            return graphViewChange;
        }

        private void ConnectPorts(FlowPort outputPort, FlowPort inputPort)
        {
            Undo.RecordObjects(new Object[] { outputPort.node, inputPort.node }, "Connect");

            if (outputPort.acceptsOnlyOneConnection)
                if (outputPort.isConnected)
                    NodyUtils.DisconnectPort(outputPort, flowGraph);

            if (inputPort.acceptsOnlyOneConnection)
                if (inputPort.isConnected)
                    NodyUtils.DisconnectPort(inputPort, flowGraph);

            outputPort.connections.Add(inputPort.portId);
            outputPort.onConnected?.Invoke(inputPort);
            inputPort.connections.Add(outputPort.portId);
            inputPort.onConnected?.Invoke(outputPort);

            EditorUtility.SetDirty(outputPort.node);
            EditorUtility.SetDirty(inputPort.node);
        }

        public override void AddToSelection(ISelectable selectable)
        {
            base.AddToSelection(selectable);
            if (selection.Count > 1)
            {
                OnNodeSelected?.Invoke(null);
                return;
            }

            if (selectable is FlowNodeView nodeView)
                OnNodeSelected?.Invoke(nodeView);
        }

        public override void RemoveFromSelection(ISelectable selectable)
        {
            base.RemoveFromSelection(selectable);
            if (selection.Count > 1)
                OnNodeSelected?.Invoke(null);

            ISelectable last = selection.Last();
            if (last is FlowNodeView nodeView)
            {
                OnNodeSelected?.Invoke(nodeView);
                return;
            }

            OnNodeSelected?.Invoke(null);
        }

        public override EventPropagation DeleteSelection()
        {
            EventPropagation result = base.DeleteSelection();
            OnNodeSelected?.Invoke(null);
            return result;
        }

        public override void ClearSelection()
        {
            base.ClearSelection();
            OnNodeSelected?.Invoke(null);
        }

        public FlowNode CreateNode(Type type, bool createView = false, bool recordUndo = true)
        {
            if (flowGraph == null) return null;
            var node = ScriptableObject.CreateInstance(type) as FlowNode;
            Debug.Assert(node != null, nameof(node) + " != null");
            node.name = ObjectNames.NicifyVariableName(type.Name.Replace("Node", ""));
            if (recordUndo) Undo.RecordObject(flowGraph, $"{ObjectNames.NicifyVariableName(nameof(CreateNode))}");
            flowGraph.nodes.Add(node);
            node.SetFlowGraph(flowGraph);
            node.SetPosition(graphMousePosition);
            EditorUtility.SetDirty(flowGraph);
            AssetDatabase.AddObjectToAsset(node, flowGraph);
            if (recordUndo) Undo.RegisterCreatedObjectUndo(node, $"{ObjectNames.NicifyVariableName(nameof(CreateNode))}");
            // AssetDatabase.SaveAssets();
            AssetDatabase.SaveAssetIfDirty(node);
            AssetDatabase.SaveAssetIfDirty(flowGraph);
            if (createView) CreateNodeView(node);
            return node;
        }

        private void DeleteNode(FlowNodeView nodeView)
        {
            if (flowGraph == null) return;                             //null graph -> stop
            if (nodeView == null) return;                              //null node view -> stop
            if (nodeView.flowNode == null) return;                     //null node reference -> stop
            if (nodeView.flowNode.canBeDeleted == false) return;       //node marked not to be deleted -> stop
            if (nodeView.flowNode.nodeType == NodeType.System) return; //node is system node -> stop
            Undo.RecordObject(flowGraph, "Delete Node");               //save undo for graph
            NodyUtils.DisconnectNode(nodeView.flowNode, flowGraph);    //disconnect node
            flowGraph.nodes.Remove(nodeView.flowNode);                 //remove node from graph
            EditorUtility.SetDirty(flowGraph);                         //mark graph as dirty
            Undo.DestroyObjectImmediate(nodeView.flowNode);            //save undo for node and destroy the node (asset)
            // AssetDatabase.SaveAssets();                                //save assets
            AssetDatabase.SaveAssetIfDirty(flowGraph);
        }

        private void DeletePort(FlowPortView portView)
        {
            if (flowGraph == null) return;                //null graph -> stop
            if (portView?.flowPort == null) return;       //null port reference -> stop
            if (portView?.flowNode == null) return;       //null node reference -> stop
            FlowPort port = portView.flowPort;            //target port
            FlowNode node = portView.flowNode;            //target node
            if (!node.CanDeletePort(port.portId)) return; //cannot delete port -> stop

            //port is valid and can be deleted

            if (port.isConnected) //port is connected -> need to disconnect first and mark everything for undo
            {
                var nodeViews = new List<FlowNodeView>();
                var flowNodes = new List<FlowNode>();
                var otherPorts = new List<FlowPort>();

                foreach (string otherPortId in port.connections.ToList())
                {
                    FlowPortView otherPortView = GetPortView(otherPortId); //get other port's view
                    FlowPort otherPort = otherPortView?.flowPort;          //get a reference to the other port
                    if (otherPort == null) continue;                       //null port -> skip this id
                    otherPorts.Add(otherPort);                             //add other ports (to disconnect)
                    flowNodes.Add(otherPort.node);                         //add other nodes (to save for undo and mark as dirty)
                    nodeViews.Add(otherPortView.nodeView);
                }                    //
                flowNodes.Add(node); //add the node to the undo objects list
                nodeViews.Add(portView.nodeView);
                Undo.RecordObjects(flowNodes.ToArray(), "Delete Port");                                       //save undo for all the nodes
                foreach (FlowPort otherPort in otherPorts) NodyUtils.DisconnectPortFromPort(port, otherPort); //disconnect all ports
                port.connections.Clear();                                                                     //sanity check - remove all connection port ids from target port
                node.DeletePort(port.portId);                                                                 //delete the port
                flowNodes.ForEach(n =>
                {
                    EditorUtility.SetDirty(n);         //mark all nodes as dirty
                    AssetDatabase.SaveAssetIfDirty(n); //save this s$%t
                });
                // AssetDatabase.SaveAssets();                                                                 
                nodeViews.ForEach(nodeView =>
                {
                    nodeView.RefreshNodeView();   //refresh node views
                    nodeView.RefreshPortsViews(); //refresh node port views
                });

                if (NodyInspectorWindow.isOpen)
                    inspector.Refresh();
                return; //stop here
            }

            //port wasn't connected -> mark for undo and delete it

            Undo.RecordObject(node, "Delete Port"); //save undo for the node
            node.DeletePort(port.portId);           //delete the port
            EditorUtility.SetDirty(node);           //mark node as dirty
            AssetDatabase.SaveAssetIfDirty(node);   //save this shit
            portView.nodeView.RefreshNodeView();    //refresh node views
            portView.nodeView.RefreshPortsViews();  //refresh node port views
            if (NodyInspectorWindow.isOpen)
                inspector.Refresh();
        }


        private static void DisconnectPorts(FlowEdgeView edgeView)
        {
            if (edgeView?.outputPortView?.flowPort == null) return;
            if (edgeView.inputPortView?.flowPort == null) return;
            DisconnectPorts(edgeView.outputPortView.flowPort, edgeView.inputPortView.flowPort);
            edgeView.outputPortView.nodeView.RefreshNodeView();
            edgeView.inputPortView.nodeView.RefreshNodeView();
        }

        private static void DisconnectPorts(FlowPort p1, FlowPort p2)
        {
            Undo.RecordObjects(new Object[] { p1.node, p2.node }, "Disconnect");
            NodyUtils.DisconnectPortFromPort(p1, p2);
            EditorUtility.SetDirty(p1.node);
            EditorUtility.SetDirty(p2.node);
        }


        private void AddRootNode() //ToDo: add subgraph settings (EnterNode instead of StartNode)
        {
            if (flowGraph == null) return;
            if (flowGraph.rootNode != null) return;
            foreach (FlowNode node in flowGraph.nodes)
            {
                if (!(node is StartNode rootNode))
                    continue;
                flowGraph.rootNode = rootNode;
                EditorUtility.SetDirty(flowGraph);
                AssetDatabase.SaveAssetIfDirty(flowGraph);
                // AssetDatabase.SaveAssets();
                break;
            }
            if (flowGraph.rootNode != null) return;
            flowGraph.rootNode = CreateNode(typeof(StartNode), false, false).SetPosition(Vector2.zero);
            flowGraph.nodes.Remove(flowGraph.rootNode);
            flowGraph.nodes.Insert(0, flowGraph.rootNode);
            EditorUtility.SetDirty(flowGraph);
            AssetDatabase.SaveAssetIfDirty(flowGraph);
            // AssetDatabase.SaveAssets();
            FrameAll();
        }

        private FlowNodeView CreateNodeView(FlowNode node)
        {
            var nodeView = FlowNodeView.GetView(this, node);
            nodeView.OnNodeSelected = OnNodeSelected;
            AddElement(nodeView);
            return nodeView;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            Vector2 mousePosition = evt.mousePosition;

            if (evt.target is FlowPortView flowPortView)
            {
                evt.menu.AppendAction
                (
                    "Delete",
                    dropdownMenuAction => DeletePort(flowPortView),
                    dropdownMenuAction => flowPortView.canDeletePort ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled
                );
                return;
            }

            base.BuildContextualMenu(evt);
            if (evt.target is GraphView || evt.target is Node || evt.target is Group)
            {
                if (evt.target is FlowNodeView)
                {
                    evt.menu.RemoveItemAt(1);
                    evt.menu.RemoveItemAt(1);
                    evt.menu.RemoveItemAt(1);
                    evt.menu.RemoveItemAt(1);
                    evt.menu.RemoveItemAt(2);
                    evt.menu.RemoveItemAt(2);
                }
                else
                {

                    evt.menu.RemoveItemAt(1);
                    evt.menu.RemoveItemAt(1);
                    evt.menu.RemoveItemAt(1);
                    evt.menu.RemoveItemAt(1);
                    evt.menu.RemoveItemAt(1);
                    evt.menu.RemoveItemAt(3);
                }

            }
        }

        private void CleanGraph()
        {
            if (flowGraph == null) return;

            bool flowIsDirty = false;

            //remove null nodes
            for (int i = flowGraph.nodes.Count - 1; i >= 0; i--)
            {
                if (flowGraph.nodes[i] != null)
                    continue;
                flowGraph.nodes.RemoveAt(i);
                flowIsDirty = true;
            }

            if (flowIsDirty)
            {
                EditorUtility.SetDirty(flowGraph);
                // RefreshGraphView();
            }
        }

    }
}
