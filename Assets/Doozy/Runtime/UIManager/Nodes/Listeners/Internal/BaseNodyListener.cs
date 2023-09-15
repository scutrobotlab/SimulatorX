// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Nody;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.ScriptableObjects;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Nodes.Listeners.Internal
{
    public abstract class BaseNodyListener
    {
        protected static bool multiplayerMode => UIManagerInputSettings.instance.multiplayerMode;

        protected SignalReceiver receiver { get; }
        protected FlowNode node { get; }
        protected bool isConnected { get; private set; }

        protected BaseNodyListener(FlowNode node)
        {
            this.node = node;
            isConnected = false;
            receiver = new SignalReceiver().SetOnSignalCallback(ProcessSignal);
        }

        public virtual void Start()
        {
            Stop();
            if (isConnected) return;
            ConnectReceiver();
            isConnected = true;
        }

        public virtual void Stop()
        {
            if (!isConnected) return;
            DisconnectReceiver();
            isConnected = false;
        }

        protected abstract void ConnectReceiver();
        protected abstract void DisconnectReceiver();
        protected abstract void ProcessSignal(Signal signal);
    }
}
