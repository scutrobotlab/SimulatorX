// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Nody.Nodes.Internal;
using Doozy.Runtime.Reactor.Easings;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.UIManager.Nodes.Listeners;
using Doozy.Runtime.UIManager.Nodes.PortData;
// ReSharper disable RedundantOverriddenMember

namespace Doozy.Runtime.UIManager.Nodes
{
    [Serializable]
    [NodyMenuPath("UI Manager", "UI")]
    public sealed class UINode : SimpleNode
    {
        public List<UIViewShowHideOption> OnEnterShowViews = new List<UIViewShowHideOption>();
        public List<UIViewShowHideOption> OnEnterHideViews = new List<UIViewShowHideOption>();
        public List<UIViewShowHideOption> OnExitShowViews = new List<UIViewShowHideOption>();
        public List<UIViewShowHideOption> OnExitHideViews = new List<UIViewShowHideOption>();
        public bool OnEnterHideAllViews;
        public bool OnExitHideAllViews;

        private List<StreamNodyListener> streamListeners { get; set; }

        private BackButtonNodyListener backButtonListener { get; set; }
        private UIButtonNodyListener uiButtonListener { get; set; }
        private UIToggleNodyListener uiToggleListener { get; set; }
        private UIViewNodyListener uiViewListener { get; set; }
        private FloatReaction timerReaction { get; set; }

        public override bool showPassthroughInEditor => true;
        public override bool showClearGraphHistoryInEditor => true;

        private bool canGoBack => firstInputPort.GetValue<GoBackInputPortData>().CanGoBack;

        public UINode()
        {
            AddInputPort()
                .SetValue(new GoBackInputPortData())
                .SetCanBeDeleted(false)
                .SetCanBeReordered(false);

            AddOutputPort()
                .SetCanBeDeleted(false)
                .SetCanBeReordered(false);

            passthrough = false;
        }

        public override void OnEnter(FlowNode previousNode = null, FlowPort previousPort = null)
        {
            base.OnEnter(previousNode, previousPort);

            if (OnEnterHideAllViews) UIView.HideAllViews();
            OnEnterShowViews.ForEach(v => v.Show(flowGraph.controller.playerIndex));
            OnEnterHideViews.ForEach(v => v.Hide(flowGraph.controller.playerIndex));
            
            StartListeners();
            StartTimer();
        }

        public override void OnExit()
        {
            base.OnExit();

            StopTimer();
            StopListeners();

            if (OnExitHideAllViews) UIView.HideAllViews();
            OnExitShowViews.ForEach(v => v.Show(flowGraph.controller.playerIndex));
            OnExitHideViews.ForEach(v => v.Hide(flowGraph.controller.playerIndex));
        }

        private void StartTimer()
        {
            FlowPort targetPort = null; //port reference (to go to next node)
            float minDuration = 10000;  //random big number to compare to
            foreach (FlowPort port in outputPorts)
            {
                UIOutputPortData portData = port.GetValue<UIOutputPortData>();
                if (portData.Trigger != UIOutputPortData.TriggerCondition.TimeDelay)
                    continue;
                if (!(minDuration > portData.TimeDelay))
                    continue;
                minDuration = portData.TimeDelay; //update min duration
                targetPort = port;                //update port reference 
            }

            if (targetPort == null) //no port was found -> do not initialize a reaction (no need)
                return;

            if (minDuration <= 0)
            {
                GoToNextNode(targetPort);
                return;
            }

            timerReaction =
                Reaction
                    .Get<FloatReaction>()
                    .SetEase(Ease.Linear)
                    .SetDuration(minDuration)
                    .SetOnFinishCallback(() => GoToNextNode(targetPort));

            timerReaction.Play(); //play reaction
        }

        private void StopTimer()
        {
            timerReaction?.Recycle(); //recycle this s@%t
        }


        private void StartListeners()
        {
            backButtonListener ??= new BackButtonNodyListener(this, OnBackButton);
            backButtonListener.Start();

            uiButtonListener ??= new UIButtonNodyListener(this, OnUIButtonSignal);
            uiButtonListener.Start();

            uiToggleListener ??= new UIToggleNodyListener(this, OnUIToggleSignal);
            uiToggleListener.Start();

            uiViewListener ??= new UIViewNodyListener(this, OnUIViewSignal);
            uiViewListener.Start();

            streamListeners ??= new List<StreamNodyListener>();
            foreach (FlowPort port in outputPorts)
            {
                UIOutputPortData portData = port.GetValue<UIOutputPortData>();
                if (portData.Trigger != UIOutputPortData.TriggerCondition.Signal)
                    continue;
                var listener = new StreamNodyListener(this, portData.SignalPayload, () => GoToNextNode(port));
                streamListeners.Add(listener);
                listener.Start();
            }
        }

        private void StopListeners()
        {
            backButtonListener?.Stop();

            uiButtonListener?.Stop();
            uiToggleListener?.Stop();
            uiViewListener?.Stop();

            streamListeners?.ForEach(listener => listener.Stop());
            streamListeners?.Clear();
        }

        private void OnBackButton(Signal signal)
        {
            if (multiplayerMode && signal.hasValue && signal.valueAsObject is InputSignalData data)
            {
                if (canGoBack)
                {
                    flowGraph.GoBack(data.playerIndex);
                    return;
                }

                foreach (FlowPort port in outputPorts)
                {
                    UIOutputPortData portData = port.GetValue<UIOutputPortData>();
                    if (portData.Trigger != UIOutputPortData.TriggerCondition.UIButton) continue;
                    if (!portData.isBackButton) continue;
                    GoToNextNode(port);
                    return;
                }
                return;
            }

            if (canGoBack)
            {
                flowGraph.GoBack();
                return;
            }
            
            foreach (FlowPort port in outputPorts)
            {
                UIOutputPortData portData = port.GetValue<UIOutputPortData>();
                if (portData.Trigger != UIOutputPortData.TriggerCondition.UIButton) continue;
                if (!portData.isBackButton) continue;
                GoToNextNode(port);
                return;
            }
        }

        private void OnUIButtonSignal(UIButtonSignalData data)
        {
            foreach (FlowPort port in outputPorts)
            {
                UIOutputPortData portData = port.GetValue<UIOutputPortData>();
                if (portData.Trigger != UIOutputPortData.TriggerCondition.UIButton) continue;

                if (portData.isBackButton && data.isBackButton)
                {
                    if (multiplayerMode && playerIndex != data.playerIndex)
                        continue;

                    if (canGoBack)
                    {
                        flowGraph.GoBack(data.playerIndex);
                        break;
                    }
                    GoToNextNode(port);
                    break;
                }

                if (!portData.ButtonId.Category.Equals(data.buttonCategory)) continue;
                if (!portData.ButtonId.Name.Equals(data.buttonName)) continue;
                if (multiplayerMode && playerIndex != data.playerIndex) continue;

                GoToNextNode(port);
                break;
            }
        }

        private void OnUIToggleSignal(UIToggleSignalData data)
        {
            foreach (FlowPort port in outputPorts)
            {
                UIOutputPortData portData = port.GetValue<UIOutputPortData>();
                if (portData.Trigger != UIOutputPortData.TriggerCondition.UIToggle) continue;
                if (portData.CommandToggle != CommandToggle.Any && portData.CommandToggle != data.state) continue;
                if (!portData.ToggleId.Category.Equals(data.toggleCategory)) continue;
                if (!portData.ToggleId.Name.Equals(data.toggleName)) continue;
                if (multiplayerMode && playerIndex != data.playerIndex) continue;

                GoToNextNode(port);
                break;
            }
        }

        private void OnUIViewSignal(UIViewSignalData data)
        {
            foreach (FlowPort port in outputPorts)
            {
                UIOutputPortData portData = port.GetValue<UIOutputPortData>();
                if (portData.Trigger != UIOutputPortData.TriggerCondition.UIView) continue;

                switch (portData.CommandShowHide)
                {
                    case CommandShowHide.Show:
                        switch (data.execute)
                        {
                            case ShowHideExecute.Hide:
                            case ShowHideExecute.InstantHide:
                            case ShowHideExecute.ReverseShow:
                            case ShowHideExecute.ReverseHide:
                                continue;
                        }
                        break;
                    case CommandShowHide.Hide:
                        switch (data.execute)
                        {
                            case ShowHideExecute.Show:
                            case ShowHideExecute.InstantShow:
                            case ShowHideExecute.ReverseShow:
                            case ShowHideExecute.ReverseHide:
                                continue;
                        }
                        break;
                }

                if (!portData.ViewId.Category.Equals(data.viewCategory)) continue;
                if (!portData.ViewId.Name.Equals(data.viewName)) continue;
                if (multiplayerMode && playerIndex != data.playerIndex) continue;

                GoToNextNode(port);
                break;
            }
        }

        public override FlowPort AddOutputPort(PortCapacity capacity = PortCapacity.Single) =>
            base.AddOutputPort(capacity)
                .SetValue(new UIOutputPortData());
    }
}
