// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Nody
{
    /// <summary> Base class for all flow nodes </summary>
    [Serializable]
    public abstract class FlowNode : ScriptableObject
    {
        public bool multiplayerMode => UIManagerInputSettings.instance.multiplayerMode & flowGraph.controller.hasMultiplayerInfo;
        public int playerIndex => flowGraph.controller.playerIndex;
        
        [SerializeField] private string FlowGraphId;
        /// <summary> Flow Graph Id this node belongs to </summary>
        public string flowGraphId
        {
            get => FlowGraphId;
            internal set => FlowGraphId = value;
        }

        /// <summary> Reference to the flow graph this node belongs to </summary>
        public FlowGraph flowGraph { get; internal set; }

        [SerializeField] private string NodeId;
        /// <summary> Node Id </summary>
        public string nodeId
        {
            get => NodeId;
            internal set => NodeId = value;
        }

        [SerializeField] private NodeType NodeType;
        /// <summary> Type of node </summary>
        public NodeType nodeType => NodeType;

        [SerializeField] private string NodeName;
        /// <summary> Name of this node </summary>
        public string nodeName
        {
            get => NodeName;
            internal set => NodeName = value;
        }

        [SerializeField] private string NodeDescription;
        /// <summary> Description for this node </summary>
        public string nodeDescription
        {
            get => NodeDescription;
            internal set => NodeDescription = value;
        }

        [SerializeField] private List<FlowPort> InputPorts;
        /// <summary> All the input ports this node has </summary>
        public List<FlowPort> inputPorts
        {
            get => InputPorts;
            internal set => InputPorts = value;
        }

        /// <summary> Get the first input port. If one does not exit, this returns null </summary>
        public FlowPort firstInputPort => inputPorts.FirstOrDefault();

        /// <summary> Get the last input port. If one does not exit, this returns null </summary>
        public FlowPort lastInputPort => inputPorts.Last();

        /// <summary> List of all the input port ids connected to this node </summary>
        public List<string> inputConnections
        {
            get
            {
                var list = new List<string>();
                foreach (FlowPort port in inputPorts)
                    list.AddRange(port.connections);
                return list;
            }
        }

        [SerializeField] private List<FlowPort> OutputPorts;
        /// <summary> All the output ports this node has </summary>
        public List<FlowPort> outputPorts
        {
            get => OutputPorts;
            internal set => OutputPorts = value;
        }

        /// <summary> Get the first output port. If one does not exit, this returns null </summary>
        public FlowPort firstOutputPort => outputPorts.FirstOrDefault();

        /// <summary> Get the last output port. If one does not exit, this returns null </summary>
        public FlowPort lastOutputPort => outputPorts.Last();

        /// <summary> List of all the output port ids connected to this node </summary>
        public List<string> outputConnections
        {
            get
            {
                var list = new List<string>();
                foreach (FlowPort port in outputPorts)
                    list.AddRange(port.connections);
                return list;
            }
        }

        /// <summary> All the ports this node had (input and output) </summary>
        public List<FlowPort> ports
        {
            get
            {
                var list = new List<FlowPort>();
                list.AddRange(inputPorts);
                list.AddRange(outputPorts);
                return list;
            }
        }

        /// <summary> List of all the port ids connected to this node (input and output) </summary>
        public List<string> connections
        {
            get
            {
                var list = new List<string>();
                list.AddRange(inputConnections);
                list.AddRange(outputConnections);
                return list;
            }
        }


        /// <summary>
        /// Minimum number of input ports this node can have.
        /// Value checked when deleting ports to prevent deletion of important ports
        /// </summary>
        public virtual int minNumberOfInputPorts => 0;

        /// <summary>
        /// Minimum number of output ports this node can have.
        /// Value checked when deleting ports to prevent deletion of important ports
        /// </summary>
        public virtual int minNumberOfOutputPorts => 0;

        [SerializeField] private bool RunUpdate;
        /// <summary> Run Update when the node is active (default: false) </summary>
        public bool runUpdate
        {
            get => RunUpdate;
            set => RunUpdate = value;
        }

        [SerializeField] private bool RunFixedUpdate;
        /// <summary> Run FixedUpdate when the node is active (default: false) </summary>
        public bool runFixedUpdate
        {
            get => RunFixedUpdate;
            set => RunFixedUpdate = value;
        }

        [SerializeField] private bool RunLateUpdate;
        /// <summary> Run LateUpdate when the node is active (default: false) </summary>
        public bool runLateUpdate
        {
            get => RunLateUpdate;
            set => RunLateUpdate = value;
        }

        [SerializeField] protected NodeState NodeState = NodeState.Idle;
        /// <summary> Current node state </summary>
        public NodeState nodeState
        {
            get => NodeState;
            set
            {
                NodeState = value;
                onStateChanged?.Invoke(value);
            }
        }

        /// <summary> Triggered every time the node changes its state </summary>
        public UnityAction<NodeState> onStateChanged { get; set; }

        [SerializeField] private bool CanBeDeleted;
        /// <summary>
        /// [Editor] True if this node can be deleted (default: true)
        /// Used to prevent special nodes from being deleted in the editor
        /// </summary>
        public bool canBeDeleted
        {
            get => CanBeDeleted;
            internal set => CanBeDeleted = value;
        }
        
        /// <summary> Show Passthrough On/Off switch in the editor (default: false) </summary>
        public virtual bool showPassthroughInEditor => false;
        [SerializeField] private bool Passthrough;
        /// <summary> Allow the graph to bypass this node when going back (default: true) </summary>
        public bool passthrough
        {
            get => Passthrough;
            set => Passthrough = value;
        }
        
        /// <summary> Show ClearGraphHistory On/Off switch in the editor (default: false) </summary>
        public virtual bool showClearGraphHistoryInEditor => false;
        [SerializeField] private bool ClearGraphHistory;
        /// <summary> OnEnter, clear graph history and remove the possibility of being able to go back to previously active nodes (default: false) </summary>
        public bool clearGraphHistory
        {
            get => ClearGraphHistory;
            set => ClearGraphHistory = value;
        }
        
        [SerializeField] private Vector2 Position = Vector2.zero;
        /// <summary> [Editor] Position of the node in the graph </summary>
        public Vector2 position
        {
            get => Position;
            internal set => Position = value;
        }

        /// <summary> [Editor] Ping this node </summary>
        public UnityAction<FlowDirection> ping { get; set; }

        /// <summary> [Editor] Refresh this node's editor </summary>
        public UnityAction refreshNodeEditor { get; set; }
        
        /// <summary> [Editor] Refresh this node's view </summary>
        public UnityAction refreshNodeView { get; set; }
        
        /// <summary> [Editor] Called when OnEnter is triggered </summary>
        public UnityAction onEnter { get; set; }

        /// <summary> [Editor] Called when OnExit is triggered </summary>
        public UnityAction onExit { get; set; }

        /// <summary> [Editor] Called when Start is triggered </summary>
        public UnityAction onStart { get; set; }

        /// <summary> [Editor] Called when Stop is triggered </summary>
        public UnityAction onStop { get; set; }

        protected FlowNode(NodeType type)
        {
            FlowGraphId = string.Empty;
            
            NodeId = Guid.NewGuid().ToString();
            NodeType = type;
            
            NodeName = ObjectNames.NicifyVariableName(GetType().Name.Replace("Node", ""));
            NodeDescription = string.Empty;
            
            InputPorts = new List<FlowPort>();
            OutputPorts = new List<FlowPort>();
            
            RunUpdate = false;
            RunFixedUpdate = false;
            RunLateUpdate = false;

            CanBeDeleted = true;
            Passthrough = true;
            ClearGraphHistory = false;
        }

        /// <summary> Called when the parent graph started and this is global node </summary>
        public virtual void Start()
        {
            nodeState = NodeState.Running;
            onStart?.Invoke();
        }

        /// <summary> Called when the parent graph stopped and this is a global node </summary>
        public virtual void Stop()
        {
            nodeState = NodeState.Idle;
            onStop?.Invoke();
        }

        /// <summary>
        /// Called on the frame when this node becomes active,
        /// just before any of the node's Update methods are called for the first time
        /// </summary>
        /// <param name="previousNode"> Source Node </param>
        /// <param name="previousPort"> Source Port </param>
        public virtual void OnEnter(FlowNode previousNode = null, FlowPort previousPort = null)
        {
            nodeState = NodeState.Active;
            onEnter?.Invoke();
            if (clearGraphHistory) flowGraph.ClearHistory();
        }               

        /// <summary> Called just before this node becomes idle </summary>
        public virtual void OnExit()
        {
            nodeState = NodeState.Idle;
            onExit?.Invoke();
        }

        /// <summary> Called every frame, if the node is enabled and active (Update Method) </summary>
        public virtual void Update() {}

        /// <summary> Called every frame, if the node is enabled and active (FixedUpdate Method) </summary>
        public virtual void FixedUpdate() {}

        /// <summary> Called every frame, if the node is enabled and active (LateUpdate Method) </summary>
        public virtual void LateUpdate() {}

        /// <summary> Clone this node </summary>
        public virtual FlowNode Clone() =>
            Instantiate(this);

        /// <summary> Go to the next node, by accessing the first connection of the given output port </summary>
        /// <param name="outputPort"> Output port to activate </param>
        protected virtual void GoToNextNode(FlowPort outputPort)
        {
            if (flowGraph == null) return;                                     //graph null check
            FlowPort fromPort = outputPort;                                    //get from port -> this node's target output port
            FlowPort toPort = flowGraph.GetPortById(fromPort.firstConnection); //get to port -> other node's target input port
            if (toPort == null) return;                                        //next port null check
            if (toPort.node == null) return;                                   //next node null check
            flowGraph.SetActiveNode(toPort.node, fromPort);                    //activate next node
        }

        public virtual void ResetNode()
        {
            nodeState = NodeState.Idle;
        }

        /// <summary> Add a new port to this node </summary>
        /// <param name="direction"> Port direction (Input/Output) - the port's connections direction </param>
        /// <param name="capacity"> Port capacity (Single/Multi) - the number of connections this port can have </param>
        public virtual FlowPort AddPort(PortDirection direction, PortCapacity capacity)
        {
            FlowPort port = new FlowPort().SetNodeId(nodeId).SetDirection(direction).SetCapacity(capacity);
            switch (direction)
            {
                case PortDirection.Input:
                    inputPorts.Add(port);
                    break;
                case PortDirection.Output:
                    outputPorts.Add(port);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
            return port;
        }

        /// <summary> Add a new input port to this node </summary>
        /// <param name="capacity"> Port capacity (Single/Multi) - the number of connections this port can have </param>
        public virtual FlowPort AddInputPort(PortCapacity capacity = PortCapacity.Multi) =>
            AddPort(PortDirection.Input, capacity);

        /// <summary> Add a new output port to this node </summary>
        /// <param name="capacity"> Port capacity (Single/Multi) - the number of connections this port can have </param>
        public virtual FlowPort AddOutputPort(PortCapacity capacity = PortCapacity.Single) =>
            AddPort(PortDirection.Output, capacity);
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class FlowNodeExtensions
    {
        /// <summary>
        /// Get the input port with the given port id.
        /// If one is not found, this method returns null
        /// </summary>
        /// <param name="target"> Target node </param>
        /// <param name="portId"> Port id to search for </param>
        public static FlowPort GetInputPortFromId<T>(this T target, string portId) where T : FlowNode =>
            target.inputPorts.FirstOrDefault(port => port.portId.Equals(portId));

        /// <summary>
        /// Get the output port with the given port id.
        /// If one is not found, this method returns null
        /// </summary>
        /// <param name="target"> Target node </param>
        /// <param name="portId"> Port id to search for </param>
        public static FlowPort GetOutputPortFromId<T>(this T target, string portId) where T : FlowNode =>
            target.outputPorts.FirstOrDefault(port => port.portId.Equals(portId));

        /// <summary>
        /// Get the port with the given port id. It can be either an input or an output port.
        /// If one is not found, this method returns null 
        /// </summary>
        /// <param name="target"> Target node </param>
        /// <param name="portId"> Port id to search for </param>
        public static FlowPort GetPortFromId<T>(this T target, string portId) where T : FlowNode =>
            target.ports.FirstOrDefault(port => port.portId.Equals(portId));

        /// <summary> True if this node has at least one connected port </summary>
        /// <param name="target"> Target node </param>
        public static bool IsConnected<T>(this T target) where T : FlowNode =>
            target.inputPorts.Any(p => p.isConnected) || target.outputPorts.Any(p => p.isConnected);

        /// <summary> True if this node is connected to a node that has a port with the given port id </summary>
        /// <param name="target"> Target node </param>
        /// <param name="portId"> Port id to search for </param>
        public static bool IsConnectedToPort<T>(this T target, string portId) where T : FlowNode =>
            target.inputPorts.Any(port => port.IsConnectedToPort(portId)) || target.outputPorts.Any(port => port.IsConnectedToPort(portId));

        /// <summary> True if the given port can be deleted from this node </summary>
        /// <param name="target"> Target node </param>
        /// <param name="portId"> Port id to search for </param>
        public static bool CanDeletePort<T>(this T target, string portId) where T : FlowNode
        {
            FlowPort port = target.GetPortFromId(portId);
            if (port == null) return false;       //port node found -> false
            if (!port.canBeDeleted) return false; //port marked as do not delete -> false
            switch (port.direction)
            {
                case PortDirection.Input:
                    if (target.inputPorts.Count <= target.minNumberOfInputPorts) return false; //deleting the port would invalidate the min number of ports -> false
                    break;
                case PortDirection.Output:
                    if (target.outputPorts.Count <= target.minNumberOfOutputPorts) return false; //deleting the port would invalidate the min number of ports -> false
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return true;
        }

        /// <summary> True if the port was deleted from this node </summary>
        /// <param name="target"> Target node </param>
        /// <param name="portId"> Port id to search for </param>
        public static bool DeletePort<T>(this T target, string portId) where T : FlowNode
        {
            if (!target.CanDeletePort(portId)) return false;
            FlowPort port = target.GetPortFromId(portId);
            switch (port.direction)
            {
                case PortDirection.Input:
                    target.inputPorts.Remove(port);
                    break;
                case PortDirection.Output:
                    target.outputPorts.Remove(port);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return true;
        }

        /// <summary> Set the flow graph for this node (also updates the node's flowGraphId) </summary>
        /// <param name="target"> Target node </param>
        /// <param name="flowGraph"> Target flow graph </param>
        public static T SetFlowGraph<T>(this T target, FlowGraph flowGraph) where T : FlowNode
        {
            target.flowGraph = flowGraph;
            target.flowGraphId = flowGraph != null ? flowGraph.id : string.Empty;
            target.ports.ForEach(port => port.node = target);
            return target;
        }

        /// <summary> Set a new name for this node </summary>
        /// <param name="target"> Target node </param>
        /// <param name="nodeName"> New node name </param>
        public static T SetNodeName<T>(this T target, string nodeName) where T : FlowNode
        {
            target.nodeName = nodeName;
            return target;
        }

        /// <summary> Set a new description for this node </summary>
        /// <param name="target"> Target node </param>
        /// <param name="nodeDescription"> New node description </param>
        public static T SetNodeDescription<T>(this T target, string nodeDescription) where T : FlowNode
        {
            target.nodeDescription = nodeDescription;
            return target;
        }

        /// <summary> [Editor] Set the position of the node in the graph </summary>
        /// <param name="target"> Target node </param>
        /// <param name="position"> Position in graph </param>
        public static T SetPosition<T>(this T target, Vector2 position) where T : FlowNode
        {
            target.position = position;
            return target;
        }
        
        /// <summary> [Editor] Ping node </summary>
        /// <param name="target"> Target node </param>
        /// <param name="flowDirection">  Flow direction (back flow is when returning to the previous node) (</param>
        public static T Ping<T>(this T target, FlowDirection flowDirection) where T : FlowNode
        {
            target.ping?.Invoke(flowDirection);
            return target;
        }
        
        /// <summary> [Editor] Refresh node's editor </summary>
        /// <param name="target"> Target node </param>
        public static T RefreshNodeEditor<T>(this T target) where T : FlowNode
        {
            target.refreshNodeEditor?.Invoke();
            return target;
        }
        
        /// <summary> [Editor] Refresh node's view </summary>
        /// <param name="target"> Target node </param>
        public static T RefreshNodeView<T>(this T target) where T : FlowNode
        {
            target.refreshNodeView?.Invoke();
            return target;
        }
    }
}
