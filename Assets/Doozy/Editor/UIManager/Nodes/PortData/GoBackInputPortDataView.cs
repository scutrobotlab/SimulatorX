// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Nody;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Nodes.PortData;
using UnityEditor;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Nodes.PortData
{
    public class GoBackInputPortDataView : PoolableElement<GoBackInputPortDataView>
    {
        public override void Reset()
        {
            port = null;
            data = null;

            goBackTab
                .ClearOnClick()
                .ClearOnValueChanged()
                .SetIsOn(false, false);
        }
        
        public override void Dispose()
        {
            base.Dispose();

            goBackTab.Recycle();

            enabledIndicator.Dispose();
        }

        public override void Recycle(bool debug = false)
        {
            base.Recycle(debug);
            Reset();
            RemoveFromHierarchy();
        }

        private FlowPort port { get; set; }
        private GoBackInputPortData data { get; set; }

        private FluidToggleIconButton goBackTab { get; }
        private EnabledIndicator enabledIndicator { get; }

        public GoBackInputPortDataView()
        {
            goBackTab =
                FluidToggleIconButton.Get()
                    .SetElementSize(ElementSize.Tiny)
                    .SetIcon(EditorMicroAnimations.EditorUI.Icons.Back)
                    .SetToggleAccentColor(EditorSelectableColors.Nody.BackFlow)
                    .SetTooltip("Toggle the 'Back' button functionality for this node")
                    .SetStyleMarginLeft(DesignUtils.k_Spacing);

            enabledIndicator =
                EnabledIndicator.Get()
                    .SetEnabledColor(EditorColors.Nody.BackFlow)
                    .SetIcon(EditorMicroAnimations.Nody.Effects.BackFlowIndicator)
                    .SetPickingMode(PickingMode.Ignore)
                    .SetStylePosition(Position.Absolute)
                    .SetSize(1, 30)
                    .SetStyleLeft(-16)
                    .SetStyleTop(-8);

            enabledIndicator.iconReaction.ReverseTexturesOrder();

            Add(goBackTab);
            Add(enabledIndicator);
        }

        public GoBackInputPortDataView SetPort(FlowPort p)
        {
            port = p;
            data = port.GetValue<GoBackInputPortData>();

            enabledIndicator
                .Toggle(data.CanGoBack, false);

            goBackTab
                .SetIsOn(data.CanGoBack, false)
                .SetOnClick(() =>
                {
                    Undo.RecordObject(port.node, "Set GoBack");
                    data.CanGoBack = !data.CanGoBack;
                    port.SetValue(data);
                    EditorUtility.SetDirty(port.node);
                    enabledIndicator.Toggle(data.CanGoBack, true);
                });
            return this;
        }
    }
}
