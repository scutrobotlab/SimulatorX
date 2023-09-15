// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Listeners.Internal;
using UnityEngine;
using UnityEngine.Events;
// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault

namespace Doozy.Runtime.UIManager.Listeners
{
    /// <summary>
    /// Connects to a specific stream and reacts to 'pings' (signals) sent through said stream
    /// </summary>
    [AddComponentMenu("Doozy/UI/Listeners/Signal Listener")]
    public class SignalListener : BaseListener
    {
        [SerializeField] private StreamId StreamId;

        /// <summary> Stream Id </summary>
        public StreamId streamId => StreamId;

        /// <summary> UnityAction callback executed when the listener is triggered </summary>
        public UnityAction<Signal> signalCallback { get; }

        public SignalStream stream { get; private set; }

        private void OnEnable() =>
            ConnectReceiver();

        private void OnDisable() =>
            DisconnectReceiver();

        protected override void ConnectReceiver() =>
            stream = SignalStream.Get(streamId.Category, streamId.Name).ConnectReceiver(receiver);

        protected override void DisconnectReceiver() =>
            stream.DisconnectReceiver(receiver);

        protected override void ProcessSignal(Signal signal)
        {
            signalCallback?.Invoke(signal);
            Callback?.Execute(signal);
        }
    }
}
