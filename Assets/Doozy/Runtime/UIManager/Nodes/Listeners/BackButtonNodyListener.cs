// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Nody;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Input;
using Doozy.Runtime.UIManager.Nodes.Listeners.Internal;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Nodes.Listeners
{
    public class BackButtonNodyListener : BaseNodyListener
    {
        private UnityAction<Signal> callback { get; }

        public BackButtonNodyListener(FlowNode node, UnityAction<Signal> callback) : base(node)
        {
            this.callback = callback;
        }

        protected override void ConnectReceiver() =>
            BackButton.stream.ConnectReceiver(receiver);

        protected override void DisconnectReceiver() =>
            BackButton.stream.DisconnectReceiver(receiver);

        protected override void ProcessSignal(Signal signal)
        {
            callback?.Invoke(signal); //moved the back button functionality on the receiver
            
            // if (multiplayerMode && signal.hasValue && signal.valueAsObject is InputSignalData data)
            // {
            //     node.flowGraph.GoBack(data.playerIndex);
            //     return;
            // }
            //
            // node.flowGraph.GoBack();
        }

    }
}
