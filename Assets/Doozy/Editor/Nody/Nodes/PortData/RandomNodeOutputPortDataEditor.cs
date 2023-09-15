// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Nody.Nodes.PortData;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.Nody.Nodes.PortData
{
    public class RandomNodeOutputPortDataEditor : VisualElement, IDisposable
    {
        public void Dispose()
        {
            connectionIndicator?.Recycle();
        }

        private FlowNodeView nodeView { get; }
        private FlowPort port { get; }
        private RandomNodeOutputPortData data { get; set; }
        private SliderInt slider { get; set; }
        private EnabledIndicator connectionIndicator { get; set; }
        private Label valueLabel { get; set; }

        public RandomNodeOutputPortDataEditor(FlowPort port, FlowNodeView nodeView)
        {
            this.port = port;
            this.nodeView = nodeView;
            data = this.port.GetValue<RandomNodeOutputPortData>();
            
            InitializeEditor();
            Compose();
            
            Undo.undoRedoPerformed += RefreshData;
        }

        private void RefreshData()
        {
            if (port == null) return;
            if (port.node == null) return;
            connectionIndicator.Toggle(port.isConnected, true);
            data = port.GetValue<RandomNodeOutputPortData>();
            {
                valueLabel.SetText($"{data.Weight}");
                slider.SetValueWithoutNotify(data.Weight);
            }
            port.RefreshPortEditor();
            port.RefreshPortView();
        }

        private void InitializeEditor()
        {
            slider = new SliderInt(0, 100).ResetLayout().SetStyleFlexGrow(1);
            slider.value = data.Weight;

            connectionIndicator =
                EnabledIndicator.Get()
                    .SetIcon(EditorMicroAnimations.Nody.Icons.One)
                    .SetEnabledColor(EditorColors.Nody.Output)
                    .SetSize(20)
                    .Toggle(port.isConnected, true);

            valueLabel =
                DesignUtils.fieldLabel
                    .ResetLayout()
                    .SetText($"{slider.value}")
                    .SetStyleAlignSelf(Align.Center)
                    .SetStyleTextAlign(TextAnchor.MiddleRight)
                    .SetStyleWidth(24);
                
            slider.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(port.node, "Set weight");
                valueLabel.SetText($"{evt.newValue}");
                data.Weight = evt.newValue;
                port.SetValue(data);
                EditorUtility.SetDirty(port.node);
                port.RefreshPortView();
            });
        }
        
        private void Compose()
        {
            this
                .SetName("Port Data")
                .SetStyleFlexDirection(FlexDirection.Row)
                .SetStyleBackgroundColor(EditorColors.Default.FieldBackground)
                .SetStyleBorderRadius(DesignUtils.k_Spacing)
                .SetStylePadding(DesignUtils.k_Spacing)
                .SetStyleMarginBottom(DesignUtils.k_Spacing)
                .AddChild(connectionIndicator)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(valueLabel)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(slider)
                .AddChild
                (
                    FluidButton.Get(EditorMicroAnimations.EditorUI.Icons.Reset)
                        .SetElementSize(ElementSize.Tiny)
                        .SetTooltip("Reset weight")
                        .SetOnClick(() =>
                        {
                            slider.value = RandomNodeOutputPortData.k_DefaultWeight;
                            port.RefreshPortEditor();
                            port.RefreshPortView();
                        })
                );
        }
    }
}
