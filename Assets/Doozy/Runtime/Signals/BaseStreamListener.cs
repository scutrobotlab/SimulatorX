// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Signals
{
    public abstract class BaseStreamListener : MonoBehaviour
    {
        public SignalReceiver receiver { get; protected set; }
        public bool isConnected { get; protected set; }

        protected BaseStreamListener()
        {
            isConnected = false;
            receiver = new SignalReceiver().SetOnSignalCallback(ProcessSignal);
        }
        
        public virtual void Connect()
        {
            if (isConnected) return;
            ConnectReceiver();
            isConnected = true;
        }

        public virtual void Disconnect()
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
