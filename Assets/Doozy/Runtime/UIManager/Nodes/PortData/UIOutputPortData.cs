// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Input;

namespace Doozy.Runtime.UIManager.Nodes.PortData
{
    [Serializable]
    public class UIOutputPortData
    {
        public enum TriggerCondition
        {
            TimeDelay,
            Signal,
            UIButton,
            UIToggle,
            UIView
        }

        public TriggerCondition Trigger;
        public float TimeDelay;
        public SignalPayload SignalPayload;
        public UIButtonId ButtonId;
        public UIToggleId ToggleId;
        public CommandToggle CommandToggle;
        public UIViewId ViewId;
        public CommandShowHide CommandShowHide;

        public bool isBackButton => Trigger == TriggerCondition.UIButton && ButtonId.Name.Equals(BackButton.k_ButtonName);
        public bool viewsCategory => Trigger == TriggerCondition.UIView && ViewId.Name.IsNullOrEmpty();
        public bool allViews => Trigger == TriggerCondition.UIView && ViewId.Category.IsNullOrEmpty() && ViewId.Name.IsNullOrEmpty();
        
        public UIOutputPortData()
        {
            Trigger = TriggerCondition.TimeDelay;
            TimeDelay = 3f;
            SignalPayload = new SignalPayload();
            ButtonId = new UIButtonId();
            ToggleId = new UIToggleId();
            CommandToggle = CommandToggle.Any;
            ViewId = new UIViewId();
            CommandShowHide = CommandShowHide.Show;
        }

        public override string ToString() =>
            Trigger switch
            {
                TriggerCondition.TimeDelay => $"{TimeDelay.Round(2)} s",
                TriggerCondition.Signal    => $"{SignalPayload}",
                TriggerCondition.UIButton  => ButtonId.Name.Equals(BackButton.k_ButtonName) ? "'Back'" : $"{ButtonId}",
                TriggerCondition.UIToggle  => $"({CommandToggle}) {ToggleId}",
                TriggerCondition.UIView    => $"({CommandShowHide}) " + $"{(ViewId.Category.IsNullOrEmpty() & ViewId.Name.IsNullOrEmpty() ? "All Views" : ViewId.Name.IsNullOrEmpty() ? $"{ViewId.Category} category" : $"{ViewId}")}",
                _                          => throw new ArgumentOutOfRangeException()
            };
    }
}
