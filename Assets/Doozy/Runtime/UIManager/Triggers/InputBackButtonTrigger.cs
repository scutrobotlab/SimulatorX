// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Input;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Triggers
{
    [AddComponentMenu("Doozy/UI/Triggers/Input/Back Button")]
    public class InputBackButtonTrigger : SignalProvider
    {
        private SignalReceiver receiver { get; set; }
        
        public InputBackButtonTrigger() : base(ProviderType.Global, "Input", "BackButton", typeof(InputBackButtonTrigger)) {}

        protected override void Awake()
        {
            base.Awake();
            receiver = new SignalReceiver().SetOnSignalCallback(ProcessSignal);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            BackButton.stream.ConnectReceiver(receiver);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            BackButton.stream.DisconnectReceiver(receiver);
        }

        private void ProcessSignal(Signal signal)
        {
            SendSignal(signal);
        }
    }
}
