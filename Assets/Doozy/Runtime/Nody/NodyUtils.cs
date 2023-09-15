// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Nody
{
    public static class NodyUtils
    {
        /// <summary> True if the given port (id) is found in the node's connections </summary>
        /// <param name="port"> Target port </param>
        /// <param name="node"> Target node </param>
        public static bool IsPortConnectedToNode(FlowPort port, FlowNode node)
        {
            if (port == null) return false; //null port -> nope
            if (node == null) return false; //null node -> nope
            List<string> connections =
                port.direction switch
                {

                    PortDirection.Input  => node.outputConnections,
                    PortDirection.Output => node.inputConnections,
                    _                    => throw new ArgumentOutOfRangeException()
                };
            return connections.Contains(port.portId);
        }

        /// <summary> True if at least one port from n1 is connected to another port in n2 </summary>
        /// <param name="n1"> Node 1 </param>
        /// <param name="n2"> Node 2 </param>
        /// <returns></returns>
        public static bool IsNodeConnectedToNode(FlowNode n1, FlowNode n2)
        {
            if (n1 == null) return false; //null node -> nope
            if (n2 == null) return false; //null node -> nope
            return 
                n1.ports.Any(port => IsPortConnectedToNode(port, n2)) || 
                n2.ports.Any(port => IsPortConnectedToNode(port, n1));
        }

        /// <summary> True if the ports can connect </summary>
        /// <param name="p1"> Port 1 </param>
        /// <param name="p2"> Port 2 </param>
        public static bool CanConnect(FlowPort p1, FlowPort p2)
        {
            if (p1 == null) return false;                      //port is null -> stop
            if (p2 == null) return false;                      //port is null -> stop
            if (p1 == p2) return false;                        //port cannot connect to itself -> stop
            if (p1.IsConnectedToPort(p2.portId)) return false; //ports are already connected -> stop
            if (p1.direction == p2.direction) return false;    //ports have the same direction -> stop
            if (p1.portId == p2.portId) return false;          //ports have the same id -> stop (wtf moment)
            if (p1.nodeId == p2.nodeId) return false;          //ports belong to the same node -> stop
            return true;                                       //allow connection
        }

        /// <summary> Remove all connections for the given port </summary>
        /// <param name="port"> Target port </param>
        /// <param name="graph"> Parent graph </param>
        public static bool DisconnectPort(FlowPort port, FlowGraph graph)
        {
            if (port == null) return false;  //null port -> cannot disconnect
            if (graph == null) return false; //null graph -> cannot disconnect
            List<FlowPort> ports =
                port.direction switch
                {

                    PortDirection.Input  => graph.outputPorts,
                    PortDirection.Output => graph.inputPorts,
                    _                    => throw new ArgumentOutOfRangeException()
                };

            ports.ForEach(p =>
            {
                p.RemoveConnection(port.portId);
                FlowPort otherPort = graph.GetPortById(port.portId);
                p.onDisconnected?.Invoke(otherPort);
                otherPort.onDisconnected?.Invoke(p);
            });
            port.connections.Clear();
            return true;
        }

        /// <summary> Remove all connections for the given port </summary>
        /// <param name="p1"> Port 1 </param>
        /// <param name="p2"> Port 2 </param>
        public static bool DisconnectPortFromPort(FlowPort p1, FlowPort p2)
        {
            if (p1 == null) return false;                       //null port -> cannot disconnect
            if (p2 == null) return false;                       //null port -> cannot disconnect
            if (!p1.IsConnectedToPort(p2.portId)) return false; //not connected -> cannot disconnect
            p1.RemoveConnection(p2.portId);
            p1.onDisconnected?.Invoke(p2);
            p2.RemoveConnection(p1.portId);
            p2.onDisconnected?.Invoke(p1);
            return true;
        }

        /// <summary> Remove all connections from the target port to the given node </summary>
        /// <param name="port"> Target port </param>
        /// <param name="node"> Target node </param>
        public static bool DisconnectPortFromNode(FlowPort port, FlowNode node)
        {
            if (port == null) return false; //null port -> cannot disconnect
            if (node == null) return false; //null node -> cannot disconnect
            List<FlowPort> ports =
                port.direction switch
                {

                    PortDirection.Input  => node.outputPorts,
                    PortDirection.Output => node.inputPorts,
                    _                    => throw new ArgumentOutOfRangeException()
                };
            ports.ForEach(nodePort =>
            {
                nodePort.RemoveConnection(port.portId);
                nodePort.onDisconnected?.Invoke(port);
                port.RemoveConnection(nodePort.portId);
                port.onDisconnected?.Invoke(nodePort);
            });
            return true;
        }

        /// <summary> Remove all connections from the given node </summary>
        /// <param name="node"> Target node </param>
        /// <param name="graph"> Parent graph </param>
        public static bool DisconnectNode(FlowNode node, FlowGraph graph)
        {
            if (node == null) return false;  //null node -> cannot disconnect
            if (graph == null) return false; //null graph -> cannot disconnect
            node.ports.ForEach(port =>
            {
                DisconnectPort(port, graph);
            });
            return true;
        }

        /// <summary> Remove all connections between the two nodes </summary>
        /// <param name="n1"> Node 1 </param>
        /// <param name="n2"> Node 2</param>
        public static bool DisconnectNodeFromNode(FlowNode n1, FlowNode n2)
        {
            if (n1 == null) return false; //null node -> cannot disconnect
            if (n2 == null) return false; //null node -> cannot disconnect
            n1.ports.ForEach(port => DisconnectPortFromNode(port, n2));
            n2.ports.ForEach(port => DisconnectPortFromNode(port, n1));
            return true;
        }

    }
}
