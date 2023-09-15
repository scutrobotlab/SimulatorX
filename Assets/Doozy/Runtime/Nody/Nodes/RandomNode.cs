// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Runtime.Nody.Nodes.Internal;
using Doozy.Runtime.Nody.Nodes.PortData;
using Random = UnityEngine.Random;
// ReSharper disable RedundantOverriddenMember

namespace Doozy.Runtime.Nody.Nodes
{
    /// <summary>
    /// The Random Node picks a weighted random connection in a memory-efficient way.
    /// It does that by picking a random number between 1 and the sum of all the weights (similar to a raffle).
    /// The result is that the higher weighted connections are selected more often than lower weighted ones.
    /// </summary>
    [Serializable]
    [NodyMenuPath("Utils", "Random")]
    public sealed class RandomNode : SimpleNode
    {
        private readonly List<int> m_SelectChances = new List<int>();
        public int maxChance { get; private set; }

        public override bool showClearGraphHistoryInEditor => true;
        
        public RandomNode()
        {
            AddInputPort()
                .SetCanBeDeleted(false)
                .SetCanBeReordered(false);

            AddOutputPort()
                .SetCanBeDeleted(false)
                .SetCanBeReordered(false);

            AddOutputPort()
                .SetCanBeDeleted(false)
                .SetCanBeReordered(false);
        }

        public override void OnEnter(FlowNode previousNode = null, FlowPort previousPort = null)
        {
            base.OnEnter(previousNode, previousPort);
            SelectRandomOutput();
        }

        public void UpdateMaxChance()
        {
            maxChance = 0;
            foreach (FlowPort port in outputPorts)
            {
                if (!port.isConnected) continue;                                   //port not connected -> ignore
                int portWeight = port.GetValue<RandomNodeOutputPortData>().Weight; //get port weight
                if (portWeight <= 0) continue;                                     //value is the weight 
                maxChance += portWeight;                                           //add the weight to the pot
            }
        }

        private void SelectRandomOutput()
        {
            m_SelectChances.Clear();
            maxChance = 0;

            foreach (FlowPort port in outputPorts)
            {
                if (!port.isConnected)
                    continue;

                int weight = port.GetValue<RandomNodeOutputPortData>().Weight;
                if (weight <= 0)
                {
                    m_SelectChances.Add(-1);
                }
                else
                {
                    maxChance += weight;
                    m_SelectChances.Add(maxChance);
                }
            }

            int randomPortIndex = 0;
            int randomChance = Random.Range(0, maxChance);
            for (int i = 0; i < m_SelectChances.Count; i++)
            {
                if (m_SelectChances[i] == -1) continue;
                if (m_SelectChances[i] < randomChance) continue;
                randomPortIndex = i;
                break;
            }

            GoToNextNode(outputPorts[randomPortIndex]);
        }

        public override FlowPort AddOutputPort(PortCapacity capacity = PortCapacity.Single) =>
            base.AddOutputPort(capacity).SetValue(new RandomNodeOutputPortData());
    }
}
