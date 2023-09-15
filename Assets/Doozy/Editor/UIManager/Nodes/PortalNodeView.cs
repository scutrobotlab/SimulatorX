// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Nody;
using Doozy.Editor.Reactor.Internal;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Nodes;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Nodes
{
    public class PortalNodeView : FlowNodeView
    {
        public override Type nodeType => typeof(PortalNode);
        public override Texture2D nodeIconTexture => EditorTextures.Nody.Icons.PortalNode;
        public override IEnumerable<Texture2D> nodeIconTextures => EditorMicroAnimations.Nody.Icons.PortalNode;

        private SerializedProperty propertyTrigger { get; }

        private Image icon { get; }
        private Texture2DReaction iconReaction { get; }

        public PortalNodeView(FlowGraphView graphView, FlowNode node) : base(graphView, node)
        {
            propertyTrigger = serializedObject.FindProperty("Trigger");

            EnumField iTriggerEnumField = DesignUtils.NewEnumField(propertyTrigger, true);
            iTriggerEnumField.RegisterValueChangedCallback(evt =>
            {
                RefreshData();
                iconReaction?.Play();
            });

            icon =
                new Image()
                    .SetName("Icon")
                    .ResetLayout()
                    .SetStyleFlexShrink(0)
                    .SetStyleSize(72)
                    .SetStyleLeft(-4)
                    .SetStyleTop(12)
                    .SetStylePosition(Position.Absolute)
                    .SetStyleBackgroundImageTintColor(EditorColors.Default.FieldIcon);

            icon.transform.rotation = Quaternion.Euler(0,0, 20);
            
            icon.AddManipulator(new Clickable(() => iconReaction?.Play()));
            
            iconReaction =
                icon.GetTexture2DReaction().SetEditorHeartbeat();

            nodeBorder
                .SetStyleOverflow(Overflow.Hidden);

            nodeBorder
                .AddChild(iTriggerEnumField)
                .AddChild(icon);

            nodeBorder.Bind(serializedObject);

            schedule.Execute(RefreshData);
        }

        public override void RefreshData()
        {
            base.RefreshData();
            iconReaction?.SetTextures(GetTextures((PortalNode.TriggerCondition)propertyTrigger.enumValueIndex));
        }

        private static IEnumerable<Texture2D> GetTextures(PortalNode.TriggerCondition trigger)
        {
            switch (trigger)
            {
                case PortalNode.TriggerCondition.Signal:
                    return EditorMicroAnimations.Signals.Icons.SignalStream;
                case PortalNode.TriggerCondition.UIButton:
                    return EditorMicroAnimations.UIManager.Icons.Buttons;
                case PortalNode.TriggerCondition.UIToggle:
                    return EditorMicroAnimations.UIManager.Icons.UIToggleCheckbox;
                case PortalNode.TriggerCondition.UIView:
                    return EditorMicroAnimations.UIManager.Icons.Views;
                default:
                    throw new ArgumentOutOfRangeException(nameof(trigger), trigger, null);
            }
        }
    }
}
