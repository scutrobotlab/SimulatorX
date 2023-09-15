// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.Nody
{
    /// <summary> Base class for all flow ports </summary>
    [Serializable]
    public class FlowPort
    {
        [SerializeField] private string NodeId;
        /// <summary> Node Id </summary>
        public string nodeId
        {
            get => NodeId;
            internal set => NodeId = value;
        }

        [SerializeField] private string PortId;
        /// <summary> Port Id </summary>
        public string portId => PortId;

        [SerializeField] private PortDirection Direction;
        /// <summary> Port direction (Input/Output) - the port's connections direction </summary>
        public PortDirection direction
        {
            get => Direction;
            internal set => Direction = value;
        }

        [SerializeField] private PortCapacity Capacity;
        /// <summary> Port capacity (Single/Multi) - the number of connections this port can have </summary>
        public PortCapacity capacity
        {
            get => Capacity;
            internal set => Capacity = value;
        }

        [SerializeField] private List<string> Connections;
        /// <summary>
        /// All the connections this port has.
        /// These are the ids of ports that this port is connected to
        /// </summary>
        public List<string> connections => Connections;

        /// <summary>
        /// Get the first connection of this port.
        /// This is the first port id that this port is connected to.
        /// If one does not exit, this returns null
        /// </summary>
        public string firstConnection => connections.FirstOrDefault();

        /// <summary> True if this port has at least one connections (checks connections count) </summary>
        public bool isConnected => Connections.Count > 0;

        /// <summary> True if this port's direction is Input </summary>
        public bool isInput => Direction == PortDirection.Input;

        /// <summary> True if this port's direction is Output </summary>
        public bool isOutput => Direction == PortDirection.Output;

        /// <summary> True if this port accents only one connection (overrides the previous one) </summary>
        public bool acceptsOnlyOneConnection => Capacity == PortCapacity.Single;

        /// <summary> True if this port accepts multiple connections </summary>
        public bool acceptsMultipleConnections => Capacity == PortCapacity.Multi;

        [SerializeField] private string Value;
        /// <summary> Returns the port's value as a string </summary>
        public string value
        {
            get => Value;
            set => Value = value;
        }

        [SerializeField] private Type m_ValueType;
        /// <summary> Returns the value type this port has and automatically updates the m_valueType if needed </summary>
        public Type valueType
        {
            get
            {
                if (m_ValueType != null) return m_ValueType;
                if (string.IsNullOrEmpty(ValueTypeQualifiedName)) return null;
                m_ValueType = Type.GetType(ValueTypeQualifiedName, false);
                return m_ValueType;
            }
            private set
            {
                m_ValueType = value;
                if (value == null) return;
                ValueTypeQualifiedName = value.AssemblyQualifiedName;
            }
        }

        [SerializeField] private string ValueTypeQualifiedName;
        /// <summary> Returns the value TypeQualifiedName and automatically updates the m_valueType if needed </summary>
        private string valueTypeQualifiedName
        {
            get { return ValueTypeQualifiedName; }
            set
            {
                ValueTypeQualifiedName = value;
                m_ValueType = Type.GetType(value, false);
            }
        }

        [SerializeField] private bool CanBeDeleted;
        /// <summary>
        /// [Editor] True if this port can be deleted.
        /// Used to prevent special ports from being deleted in the editor
        /// </summary>
        public bool canBeDeleted
        {
            get => CanBeDeleted;
            internal set => CanBeDeleted = value;
        }

        [SerializeField] private bool CanBeReordered;
        /// <summary>
        /// [Editor] True if this port can be reordered.
        /// Used to prevent special ports from being reordered in the editor
        /// </summary>
        public bool canBeReordered
        {
            get => CanBeReordered;
            internal set => CanBeReordered = value;
        }

        /// <summary> [Editor] Ping this port </summary>
        public UnityAction<FlowDirection> ping { get; set; }

        /// <summary> [Editor] Refresh this port's editor </summary>
        public UnityAction refreshPortEditor { get; set; }
        
        /// <summary> [Editor] Refresh this port's view </summary>
        public UnityAction refreshPortView { get; set; }
        
        /// <summary> [Editor] Called on port connected (references the other port) </summary>
        public UnityAction<FlowPort> onConnected { get; set; }
        
        /// <summary> [Editor] Called on port disconnected (references the other port) </summary>
        public UnityAction<FlowPort> onDisconnected { get; set; }
        
        /// <summary> Parent node reference </summary>
        public FlowNode node { get; set; }

        /// <summary> Construct a port that needs node id, direction and capacity to be set </summary>
        public FlowPort()
        {
            PortId = Guid.NewGuid().ToString();
            Connections = new List<string>();
            CanBeDeleted = true;
            CanBeReordered = true;
        }

        /// <summary> Construct a port </summary>
        /// <param name="node"> Node that this port belongs to </param>
        /// <param name="direction"> Port direction (Input/Output) - the port's connections direction </param>
        /// <param name="capacity"> Port capacity (Single/Multi) - the number of connections this port can have </param>
        public FlowPort(FlowNode node, PortDirection direction, PortCapacity capacity) : this()
        {
            NodeId = node.nodeId;
            Direction = direction;
            Capacity = capacity;

            valueType = valueType;
            valueTypeQualifiedName = valueType.AssemblyQualifiedName;
            value = JsonUtility.ToJson(Activator.CreateInstance(valueType));
        }

        /// <summary> Create a deep copy of another port </summary>
        /// <param name="other"></param>
        public FlowPort(FlowPort other)
        {
            PortId = other.portId;
            NodeId = other.nodeId;
            Direction = other.direction;
            Capacity = other.capacity;
            Connections = new List<string>(other.connections);
            CanBeDeleted = other.CanBeDeleted;
            CanBeReordered = other.CanBeReordered;
            
            valueType = other.valueType;
            valueTypeQualifiedName = other.valueTypeQualifiedName;
            value = other.value;
        }

        /// <summary> Get the port value </summary>
        /// <typeparam name="T"> Data Type </typeparam>
        public T GetValue<T>() => (T)JsonUtility.FromJson(value, valueType);

        /// <summary> Set port value </summary>
        /// <param name="data"> Port data </param>
        /// <typeparam name="T"> Data type </typeparam>
        public FlowPort SetValue<T>(T data)
        {
            valueType = typeof(T);
            value = JsonUtility.ToJson(data);
            return this;
        }
        
        /// <summary> Get the port value as a scriptable object </summary>
        public ScriptableObject GetScriptableObjectValue(ScriptableObject targetScriptableObject)
        {
            JsonUtility.FromJsonOverwrite(value, targetScriptableObject);
            return targetScriptableObject;
        } 
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class FlowPortExtensions
    {
        /// <summary> Remove connection by deleting the given port id from the connections list </summary>
        /// <param name="target"> Target port </param>
        /// <param name="portId"> Port id to search for </param>
        public static T RemoveConnection<T>(this T target, string portId) where T : FlowPort
        {
            if (target.connections.Contains(portId))
                target.connections.Remove(portId);

            return target;
        }

        /// <summary> True if this port is connected to another port with the given port id </summary>
        /// <param name="target"> Target port </param>
        /// <param name="otherPortId"> Port id to search for to determine if this port is connected to it or not </param>
        public static bool IsConnectedToPort<T>(this T target, string otherPortId) where T : FlowPort =>
            target.direction switch
            {
                PortDirection.Input  => target.connections.Any(c => c.Equals(otherPortId)), //Input -> look at the outputPortId
                PortDirection.Output => target.connections.Any(c => c.Equals(otherPortId)), //Output -> look at the inputPortId
                _                    => throw new ArgumentOutOfRangeException()             //wtf
            };

        /// <summary> [Editor] Set the port's node id </summary>
        /// <param name="target"> Target port </param>
        /// <param name="nodeId"> Node id of the node that contains this port </param>
        public static T SetNodeId<T>(this T target, string nodeId) where T : FlowPort
        {
            target.nodeId = nodeId;
            return target;
        }

        /// <summary> [Editor] Set the port direction (Input/Output) - the port's connections direction </summary>
        /// <param name="target"> Target port </param>
        /// <param name="direction"> Port direction </param>
        public static T SetDirection<T>(this T target, PortDirection direction) where T : FlowPort
        {
            target.direction = direction;
            return target;
        }

        /// <summary> [Editor] Set the port capacity (Single/Multi) - the number of connections this port can have </summary>
        /// <param name="target"> Target port </param>
        /// <param name="capacity"> Port capacity </param>
        public static T SetCapacity<T>(this T target, PortCapacity capacity) where T : FlowPort
        {
            target.capacity = capacity;
            return target;
        }

        /// <summary> [Editor] Set True if this port can be deleted. Used to prevent special ports from being deleted in the editor </summary>
        /// <param name="target"> Target port </param>
        /// <param name="canBeDeleted"> True if this port can be deleted </param>
        public static T SetCanBeDeleted<T>(this T target, bool canBeDeleted) where T : FlowPort
        {
            target.canBeDeleted = canBeDeleted;
            return target;
        }

        /// <summary>
        /// [Editor] Set True if this port can be reordered.
        /// Used to prevent special ports from being reordered in the editor
        /// </summary>
        /// <param name="target"> Target port </param>
        /// <param name="canBeReordered"> True if this port can be reordered </param>
        public static T SetCanBeReordered<T>(this T target, bool canBeReordered) where T : FlowPort
        {
            target.canBeReordered = canBeReordered;
            return target;
        }

        /// <summary> [Editor] Ping port </summary>
        /// <param name="target"> Target port </param>
        /// <param name="flowDirection">  Flow direction (back flow is when returning to the previous node) (</param>
        public static T Ping<T>(this T target, FlowDirection flowDirection) where T : FlowPort
        {
            target.ping?.Invoke(flowDirection);
            return target;
        }
        
        /// <summary> [Editor] Refresh port's editor </summary>
        /// <param name="target"> Target port </param>
        public static T RefreshPortEditor<T>(this T target) where T : FlowPort
        {
            target.refreshPortEditor?.Invoke();
            return target;
        }
        
        /// <summary> [Editor] Refresh port's view </summary>
        /// <param name="target"> Target port </param>
        public static T RefreshPortView<T>(this T target) where T : FlowPort
        {
            target.refreshPortView?.Invoke();
            return target;
        }

    }
}
