// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Nody.Nodes;
using Doozy.Runtime.Nody.Nodes.PortData;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Nody.Nodes.PortData
{
    public class RandomNodeOutputPortDataView : PoolableElement<RandomNodeOutputPortDataView>
    {
        public override void Reset()
        {
            port = null;
            data = null;
            label.SetText(string.Empty);
        }

        public override void Recycle(bool debug = false)
        {
            base.Recycle(debug);
            Reset();
            RemoveFromHierarchy();
        }

        private RandomNode node => (RandomNode)port.node;

        private FlowPort port { get; set; }
        private RandomNodeOutputPortData data { get; set; }
        private Label label { get; }

        public RandomNodeOutputPortDataView()
        {
            label =
                DesignUtils.fieldLabel
                    .SetStyleAlignSelf(Align.Center)
                    .SetStyleTextAlign(TextAnchor.MiddleRight)
                    .SetStyleMarginRight(DesignUtils.k_Spacing)
                    .SetStyleWidth(24);

            Add(label);
        }

        public RandomNodeOutputPortDataView SetPort(FlowPort port)
        {
            this.port = port;
            RefreshData();
            return this;
        }

        public RandomNodeOutputPortDataView RefreshData()
        {
            node.UpdateMaxChance();
            data = port.GetValue<RandomNodeOutputPortData>();
            float chance = data.Weight / (float)node.maxChance * 100f;
            chance = chance.Round(0);
            chance = port.isConnected ? chance : 0f;
            label.SetText($"{chance}%");
            return this;
        }
    }
}
