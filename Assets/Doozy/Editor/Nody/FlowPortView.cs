// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Internal;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using EditorStyles = Doozy.Editor.EditorUI.EditorStyles;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Doozy.Editor.Nody
{
    public class FlowPortView : Port, IDisposable
    {
        public void Dispose()
        {
            capacityIconReaction?.Recycle();
            RemoveFromHierarchy();
        }

        public bool canDeletePort => flowNode.CanDeletePort(flowPort.portId);

        private static Type defaultPortType => typeof(bool);

        public FlowGraphView graphView { get; }
        public FlowNodeView nodeView { get; }

        public FlowGraph flowGraph => graphView.flowGraph;
        public FlowNode flowNode => flowPort.node;
        public FlowPort flowPort { get; }

        public VisualElement connectorBox { get; }
        public VisualElement connectorBoxCap { get; }

        public Image capacityIcon { get; }
        public Texture2DReaction capacityIconReaction { get; }

        public VisualElement portInfoContainer { get; }

        public FlowPortView(FlowGraphView graphView, FlowNodeView nodeView, FlowPort flowPort) : this(graphView, nodeView, flowPort, Orientation.Horizontal, defaultPortType) {}
        public FlowPortView(FlowGraphView graphView, FlowNodeView nodeView, FlowPort flowPort, Orientation portOrientation) : this(graphView, nodeView, flowPort, portOrientation, defaultPortType) {}
        public FlowPortView(FlowGraphView graphView, FlowNodeView nodeView, FlowPort flowPort, Orientation portOrientation, Type type) : base(portOrientation, GetDirection(flowPort.direction), GetCapacity(flowPort.capacity), type)
        {
            this.graphView = graphView;
            this.nodeView = nodeView;
            this.flowPort = flowPort;
            flowPort.node ??= graphView.flowGraph.GetNodeById(flowPort.nodeId);

            this
                .AddStyle(EditorStyles.Nody.FlowPort)
                .SetStyleHeight(StyleKeyword.Auto)
                .SetStylePadding(0, 4, 0, 4)
                .SetStyleAlignSelf(Align.Center)
                .SetStyleFlexDirection(FlexDirection.Row);

            connectorBox = this.Q<VisualElement>("connector");      //get connector
            connectorBoxCap = connectorBox.Q<VisualElement>("cap"); //get cap
            this.Q<Label>().RemoveFromHierarchy();                  //remove label


            //create a container
            portInfoContainer =
                new VisualElement()
                    .SetName(nameof(portInfoContainer))
                    .SetStyleAlignItems(Align.Center)
                    .SetStyleFlexDirection(FlexDirection.Row);

            switch (direction)
            {
                case Direction.Input: //add container to the right
                    this
                        .SetStyleAlignSelf(Align.FlexStart)
                        .SetStyleMarginLeft(DesignUtils.k_Spacing);

                    Add(portInfoContainer);
                    Add(DesignUtils.flexibleSpace);
                    break;
                case Direction.Output: //add container to the left
                    this
                        .SetStyleAlignSelf(Align.FlexEnd)
                        .SetStyleMarginRight(DesignUtils.k_Spacing);

                    Insert(0, portInfoContainer);
                    Insert(0, DesignUtils.flexibleSpace);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            portColor = portDirectionColor;

            capacityIcon =
                new Image()
                    .ResetLayout()
                    .SetStyleFlexShrink(0)
                    .SetStyleSize(capacityIconSize)
                    .SetStyleBackgroundImageTintColor(this.flowPort.isConnected ? connectorBox.GetStyleBackgroundColor() : portColor)
                    .SetTooltip
                    (
                        this.flowPort.capacity == PortCapacity.Single ? "Single connection port" : this.flowPort.capacity == PortCapacity.Multi ? "Multi connections port" : throw new ArgumentOutOfRangeException()
                    );

            capacityIconReaction = capacityIcon
                .GetTexture2DReaction(capacityIconTextures)
                .SetEditorHeartbeat()
                .SetDuration(0.6f);


            RegisterCallback<PointerEnterEvent>(evt =>
            {
                capacityIconReaction?.Play();
                capacityIcon.SetStyleBackgroundImageTintColor(connectorBox.GetStyleBackgroundColor());
            });

            RegisterCallback<PointerLeaveEvent>(evt =>
            {
                capacityIconReaction?.Play();
                UpdateCapacityIcon();
            });

            connectorBox.Add(capacityIcon);

            var connectorListener = new DefaultEdgeConnectorListener();
            m_EdgeConnector = new EdgeConnector<FlowEdgeView>(connectorListener);
            this.AddManipulator(m_EdgeConnector);
            this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
            this.RegisterCallback<CustomStyleResolvedEvent>(evt => UpdateCapacityIcon());
        }

        /// <summary>
        ///   <para>Add menu items to the port contextual menu.</para>
        /// </summary>
        /// <param name="evt">The event holding the menu to populate.</param>
        public void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (!(evt.target is FlowPortView))
                return;
        }

        public FlowEdgeView ConnectTo(FlowPortView otherPortView) =>
            ConnectTo<FlowEdgeView>(otherPortView)
                .SetInputPortView(direction == Direction.Input ? this : otherPortView)
                .SetOutputPortView(direction == Direction.Input ? otherPortView : this);

        public override void Connect(Edge edge)
        {
            base.Connect(edge);
            UpdateCapacityIcon();
            ((FlowPortView)edge.input).nodeView.RefreshData();
            ((FlowPortView)edge.output).nodeView.RefreshData();
        }

        public override void Disconnect(Edge edge)
        {
            base.Disconnect(edge);
            UpdateCapacityIcon();
            ((FlowPortView)edge.input).nodeView.RefreshData();
            ((FlowPortView)edge.output).nodeView.RefreshData();
        }

        public override void DisconnectAll()
        {
            base.DisconnectAll();
            UpdateCapacityIcon();
        }

        public override void OnStopEdgeDragging()
        {
            base.OnStopEdgeDragging();
            UpdateCapacityIcon();
        }

        private void UpdateCapacityIcon()
        {
            schedule.Execute(() =>
            {

                capacityIcon
                    .SetStyleBackgroundImageTintColor
                    (
                        connected
                            ? connectorBox.GetStyleBackgroundColor()
                            : portColor
                    );
            });
        }

        private class DefaultEdgeConnectorListener : IEdgeConnectorListener
        {
            private GraphViewChange m_GraphViewChange;
            private List<Edge> m_EdgesToCreate;
            private List<GraphElement> m_EdgesToDelete;

            public DefaultEdgeConnectorListener()
            {
                m_EdgesToCreate = new List<Edge>();
                m_EdgesToDelete = new List<GraphElement>();
                m_GraphViewChange.edgesToCreate = m_EdgesToCreate;
            }

            public void OnDropOutsidePort(Edge edge, Vector2 position)
            {
            }

            public void OnDrop(GraphView graphView, Edge edge)
            {
                m_EdgesToCreate.Clear();
                m_EdgesToCreate.Add(edge);
                m_EdgesToDelete.Clear();
                if (edge.input.capacity == Capacity.Single)
                {
                    foreach (Edge connection in edge.input.connections)
                    {
                        if (connection != edge)
                            m_EdgesToDelete.Add(connection);
                    }
                }
                if (edge.output.capacity == Capacity.Single)
                {
                    foreach (Edge connection in edge.output.connections)
                    {
                        if (connection != edge)
                            m_EdgesToDelete.Add(connection);
                    }
                }
                if (m_EdgesToDelete.Count > 0)
                    graphView.DeleteElements(m_EdgesToDelete);
                List<Edge> edgesToCreate = m_EdgesToCreate;
                if (graphView.graphViewChanged != null)
                    edgesToCreate = graphView.graphViewChanged(m_GraphViewChange).edgesToCreate;
                foreach (Edge newEdge in edgesToCreate)
                {
                    graphView.AddElement(newEdge);
                    edge.input.Connect(newEdge);
                    edge.output.Connect(newEdge);
                }
            }
        }

        private IEnumerable<Texture2D> capacityIconTextures
        {
            get
            {
                switch (flowPort.capacity)
                {
                    case PortCapacity.Single:
                        return EditorMicroAnimations.Nody.Icons.One;
                    case PortCapacity.Multi:
                        return EditorMicroAnimations.Nody.Icons.Infinity;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private int capacityIconSize
        {
            get
            {
                switch (flowPort.capacity)
                {
                    case PortCapacity.Single:
                        return 8;
                    case PortCapacity.Multi:
                        return 10;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private Color portDirectionColor
        {
            get
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                    return EditorColors.Nody.StateIdle;

                switch (flowPort.direction)
                {
                    case PortDirection.Input:
                        return EditorColors.Nody.Input;
                    case PortDirection.Output:
                        return EditorColors.Nody.Output;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }


        /// <summary> Convert PortDirection (Nody) to Direction (Unity) </summary>
        public static Direction GetDirection(PortDirection portDirection)
        {
            switch (portDirection)
            {
                case PortDirection.Input:
                    return Direction.Input;
                case PortDirection.Output:
                    return Direction.Output;
                default:
                    throw new ArgumentOutOfRangeException(nameof(portDirection), portDirection, null);
            }
        }

        /// <summary> Convert Direction (Unity) to PortDirection (Nody) </summary>
        public static PortDirection GetPortDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Input:
                    return PortDirection.Input;
                case Direction.Output:
                    return PortDirection.Output;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        /// <summary> Convert PortCapacity (Nody) to Capacity (Unity) </summary>
        public static Capacity GetCapacity(PortCapacity portCapacity)
        {
            switch (portCapacity)
            {
                case PortCapacity.Single:
                    return Capacity.Single;
                case PortCapacity.Multi:
                    return Capacity.Multi;
                default:
                    throw new ArgumentOutOfRangeException(nameof(portCapacity), portCapacity, null);
            }
        }

        /// <summary> Convert Capacity (Unity) to PortCapacity (Nody) </summary>
        public static PortCapacity GetCapacity(Capacity portCapacity)
        {
            switch (portCapacity)
            {
                case Capacity.Single:
                    return PortCapacity.Single;
                case Capacity.Multi:
                    return PortCapacity.Multi;
                default:
                    throw new ArgumentOutOfRangeException(nameof(portCapacity), portCapacity, null);
            }
        }

    }
}
