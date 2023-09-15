// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Nody.Nodes.Internal;
using UnityEngine;
using UnityEngine.Events;
// ReSharper disable RedundantOverriddenMember

namespace Doozy.Runtime.Nody.Nodes
{
    /// <summary>
    /// Special node that has no functional role, but a visual one.
    /// Helps redirect connections from one node to another and reduce visual clutter in the graph.
    /// </summary>
    [Serializable]
    [NodyMenuPath("Utils", "Pivot")]
    public sealed class PivotNode : SimpleNode
    {
        public enum Orientation
        {
            Horizontal,
            HorizontalReversed,
            Vertical,
            VerticalReversed
        }

        [SerializeField] private Orientation PivotOrientation;
        public Orientation pivotOrientation
        {
            get => PivotOrientation;
            set
            {
                PivotOrientation = value;
                onOrientationChanged?.Invoke(value);
            }
        }

        public UnityAction<Orientation> onOrientationChanged { get; set; }
        
        public PivotNode()
        {
            AddInputPort()                
                .SetCanBeDeleted(false)   
                .SetCanBeReordered(false);

            AddOutputPort()               
                .SetCanBeDeleted(false)   
                .SetCanBeReordered(false);
        }

        public override void OnEnter(FlowNode previousNode = null, FlowPort previousPort = null)
        {
            base.OnEnter(previousNode, previousPort);
            GoToNextNode(firstOutputPort);
        }
        
    }
}