// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Internal;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Nodes.PortData;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Nodes.PortData
{
    public class UIOutputPortDataView : PoolableElement<UIOutputPortDataView>
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
        
        private FlowPort port { get; set; }
        private UIOutputPortData data { get; set; }
        
        public Image icon { get; }
        public Texture2DReaction iconReaction { get; }
        private Label label { get; }

        public bool isBackButton => data.isBackButton;
        
        public UIOutputPortDataView()
        {
            icon =
                new Image()
                    .SetName("icon")
                    .ResetLayout()
                    .SetStyleSize(16)
                    .SetStyleMarginRight(DesignUtils.k_Spacing)
                    .SetStyleBackgroundImageTintColor(DesignUtils.fieldNameTextColor);

            iconReaction =
                icon.GetTexture2DReaction().SetEditorHeartbeat();
            
            label =
                DesignUtils.fieldLabel
                    .SetStyleAlignSelf(Align.Center)
                    .SetStyleTextAlign(TextAnchor.MiddleRight)
                    .SetStyleMarginLeft(DesignUtils.k_Spacing)
                    .SetStyleMarginRight(DesignUtils.k_Spacing);

            RegisterCallback<PointerEnterEvent>(evt => iconReaction?.Play());
            this.AddManipulator(new Clickable(() => iconReaction?.Play()));

            this
                .SetStyleFlexDirection(FlexDirection.Row)
                .SetStyleAlignItems(Align.Center)
                .AddChild(label)
                .AddChild(icon);
        }

        public UIOutputPortDataView SetPort(FlowPort p)
        {
            port = p;
            RefreshData();
            return this;
        }
        
        public UIOutputPortDataView RefreshData()
        {
            data = port.GetValue<UIOutputPortData>();
            icon.SetTooltip($"{data.Trigger}");
            iconReaction.SetTextures(GetTextures(data.Trigger)).Play();
            label.SetText(data.ToString());
            return this;
        }

        private static IEnumerable<Texture2D> GetTextures(UIOutputPortData.TriggerCondition trigger)
        {
            switch (trigger)
            {
                case UIOutputPortData.TriggerCondition.TimeDelay:
                    return EditorMicroAnimations.EditorUI.Icons.Hourglass;
                case UIOutputPortData.TriggerCondition.Signal:
                    return EditorMicroAnimations.Signals.Icons.SignalStream;
                case UIOutputPortData.TriggerCondition.UIButton:
                    return EditorMicroAnimations.UIManager.Icons.Buttons;
                case UIOutputPortData.TriggerCondition.UIToggle:
                    return EditorMicroAnimations.UIManager.Icons.UIToggleCheckbox;
                case UIOutputPortData.TriggerCondition.UIView:
                    return EditorMicroAnimations.UIManager.Icons.Views;
                default:
                    throw new ArgumentOutOfRangeException(nameof(trigger), trigger, null);
            }
        }
    }
}
