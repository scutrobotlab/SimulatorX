// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Nody;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.UIManager.Nodes.Listeners.Internal;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Nodes.Listeners
{
    public class UIViewNodyListener: BaseNodyListener
    {
        private UnityAction<UIViewSignalData> callback { get; }

        public UIViewNodyListener(FlowNode node, UnityAction<UIViewSignalData> callback) : base(node) =>
            this.callback = callback;

        protected override void ConnectReceiver() =>
            UIView.stream.ConnectReceiver(receiver);

        protected override void DisconnectReceiver() =>
            UIView.stream.DisconnectReceiver(receiver);

        protected override void ProcessSignal(Signal signal)
        {
            if (signal.hasValue && signal.valueAsObject is UIViewSignalData data)
                callback?.Invoke(data);
        }
    }
}
