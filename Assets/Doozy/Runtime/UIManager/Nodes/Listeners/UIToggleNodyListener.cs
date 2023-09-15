// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Nody;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Nodes.Listeners.Internal;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Nodes.Listeners
{
    public class UIToggleNodyListener : BaseNodyListener
    {
        private UnityAction<UIToggleSignalData> callback { get; }

        public UIToggleNodyListener(FlowNode node, UnityAction<UIToggleSignalData> callback) : base(node) =>
            this.callback = callback;

        protected override void ConnectReceiver() =>
            UIToggle.stream.ConnectReceiver(receiver);

        protected override void DisconnectReceiver() =>
            UIToggle.stream.DisconnectReceiver(receiver);

        protected override void ProcessSignal(Signal signal)
        {
            if (signal.hasValue && signal.valueAsObject is UIToggleSignalData data)
                callback?.Invoke(data);
        }
    }
}
