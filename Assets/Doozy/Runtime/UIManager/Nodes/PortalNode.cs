// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Nody.Nodes.Internal;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Input;
using Doozy.Runtime.UIManager.Nodes.Listeners;
// ReSharper disable RedundantOverriddenMember

namespace Doozy.Runtime.UIManager.Nodes
{
    /// <summary>
    /// The Portal Node is a global node that listens for a set trigger and, when triggered, activates the node connected to it.
    /// A global node is active as long as its parent Graph is active.
    /// This particular node allows for jumping from one part of the UI flow to another, without the need of a direct connection.
    /// Due to the way it works, this node can also be used as a ‘virtual connection’ between multiple active Graphs.
    /// </summary>
    [Serializable]
    [NodyMenuPath("UI Manager", "Portal")]
    public sealed class PortalNode : GlobalNode
    {
        public enum TriggerCondition
        {
            Signal,
            UIButton,
            UIToggle,
            UIView
        }

        public TriggerCondition Trigger;
        public SignalPayload SignalPayload;
        public UIButtonId ButtonId;
        public UIToggleId ToggleId;
        public CommandToggle CommandToggle;
        public UIViewId ViewId;
        public CommandShowHide CommandShowHide;

        public bool isBackButton => Trigger == TriggerCondition.UIButton && ButtonId.Name.Equals(BackButton.k_ButtonName);
        public bool viewsCategory => Trigger == TriggerCondition.UIView && ViewId.Name.IsNullOrEmpty();
        public bool allViews => Trigger == TriggerCondition.UIView && ViewId.Category.IsNullOrEmpty() && ViewId.Name.IsNullOrEmpty();

        private StreamNodyListener streamListener { get; set; }
        private UIButtonNodyListener uiButtonListener { get; set; }
        private UIToggleNodyListener uiToggleListener { get; set; }
        private UIViewNodyListener uiViewListener { get; set; }

        public override bool showClearGraphHistoryInEditor => true;

        public PortalNode()
        {
            Trigger = TriggerCondition.Signal;
            SignalPayload = new SignalPayload();
            ButtonId = new UIButtonId();
            ToggleId = new UIToggleId();
            CommandToggle = CommandToggle.Any;
            ViewId = new UIViewId();
            CommandShowHide = CommandShowHide.Show;

            AddOutputPort()
                .SetCanBeDeleted(false)
                .SetCanBeReordered(false);
        }

        public override void OnEnter(FlowNode previousNode = null, FlowPort previousPort = null)
        {
            base.OnEnter(previousNode, previousPort);
            GoToNextNode(firstOutputPort);
        }

        public override void OnExit()
        {
            base.OnExit();
            nodeState = NodeState.Running;
        }

        public override void Start()
        {
            base.Start();
            StartListeners();
        }

        public override void Stop()
        {
            base.Stop();
            StopListeners();
        }

        private void StartListeners()
        {
            streamListener ??= new StreamNodyListener(this, SignalPayload, OnSignal);
            streamListener.Start();

            uiButtonListener ??= new UIButtonNodyListener(this, OnUIButtonSignal);
            uiButtonListener.Start();

            uiToggleListener ??= new UIToggleNodyListener(this, OnUIToggleSignal);
            uiToggleListener.Start();

            uiViewListener ??= new UIViewNodyListener(this, OnUIViewSignal);
            uiViewListener.Start();
        }

        private void StopListeners()
        {
            streamListener?.Stop();
            uiButtonListener?.Stop();
            uiToggleListener?.Stop();
            uiViewListener?.Stop();
        }

        private void OnSignal()
        {
            if (Trigger != TriggerCondition.Signal) return;
            flowGraph.SetActiveNode(this);
        }

        private void OnUIButtonSignal(UIButtonSignalData data)
        {
            if (Trigger != TriggerCondition.UIButton) return;

            if (isBackButton && data.isBackButton)
            {
                if (multiplayerMode && playerIndex != data.playerIndex)
                    return;

                flowGraph.SetActiveNode(this);
                return;
            }

            if (!ButtonId.Category.Equals(data.buttonCategory)) return;
            if (!ButtonId.Name.Equals(data.buttonName)) return;
            if (multiplayerMode && playerIndex != data.playerIndex) return;

            flowGraph.SetActiveNode(this);
        }

        private void OnUIToggleSignal(UIToggleSignalData data)
        {
            if (Trigger != TriggerCondition.UIToggle) return;
            if (CommandToggle != data.state) return;
            if (!ToggleId.Category.Equals(data.toggleCategory)) return;
            if (!ToggleId.Name.Equals(data.toggleName)) return;
            if (multiplayerMode && playerIndex != data.playerIndex) return;

            flowGraph.SetActiveNode(this);
        }

        private void OnUIViewSignal(UIViewSignalData data)
        {
            if (Trigger != TriggerCondition.UIView) return;

            switch (CommandShowHide)
            {
                case CommandShowHide.Show:
                    switch (data.execute)
                    {
                        case ShowHideExecute.Hide:
                        case ShowHideExecute.InstantHide:
                        case ShowHideExecute.ReverseShow:
                        case ShowHideExecute.ReverseHide:
                            return;
                    }
                    break;
                case CommandShowHide.Hide:
                    switch (data.execute)
                    {
                        case ShowHideExecute.Show:
                        case ShowHideExecute.InstantShow:
                        case ShowHideExecute.ReverseShow:
                        case ShowHideExecute.ReverseHide:
                            return;
                    }
                    break;
            }

            if (!ViewId.Category.Equals(data.viewCategory)) return;
            if (!ViewId.Name.Equals(data.viewName)) return;
            if (multiplayerMode && playerIndex != data.playerIndex) return;

            flowGraph.SetActiveNode(this);
        }

        public string InfoString() =>
            Trigger switch
            {
                TriggerCondition.Signal   => $"{SignalPayload}",
                TriggerCondition.UIButton => ButtonId.Name.Equals(BackButton.k_ButtonName) ? "'Back'" : $"{ButtonId}",
                TriggerCondition.UIToggle => $"({CommandToggle}) {ToggleId}",
                TriggerCondition.UIView   => $"({CommandShowHide}) " + $"{(ViewId.Category.IsNullOrEmpty() & ViewId.Name.IsNullOrEmpty() ? "All Views" : ViewId.Name.IsNullOrEmpty() ? $"{ViewId.Category} category" : $"{ViewId}")}",
                _                         => throw new ArgumentOutOfRangeException()
            };
    }
}
