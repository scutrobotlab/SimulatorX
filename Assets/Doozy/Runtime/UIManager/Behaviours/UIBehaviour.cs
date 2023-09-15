// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;
using UnityEngine;

namespace Doozy.Runtime.UIManager
{
    [Serializable]
    public partial class UIBehaviour : ModyEvent
    {
        [SerializeField] private Name BehaviourName;
        public Name behaviourName => BehaviourName;

        [SerializeField] private SignalReceiver Receiver;
        public SignalReceiver receiver => Receiver;

        [SerializeField] protected float Cooldown;
        public float cooldown => Cooldown;

        [SerializeField] private UISelectable Selectable;
        public UISelectable selectable => Selectable;

        public UIBehaviour() : this(Name.PointerClick, null) {}

        public UIBehaviour(Name behaviourName, GameObject target)
        {
            BehaviourName = behaviourName;
            EventName = behaviourName.ToString();
            Enabled = true;
            Receiver =
                new SignalReceiver()
                    .SetSignalSource(target)
                    .SetProviderId(UIBehaviour.GetProvideId(behaviourName));
        }

        public void Connect()
        {
            if (!Enabled) 
                return;
            
            if (receiver.isConnected) 
                return;
            
            receiver.Connect();
            
            if(!receiver.isConnected)
                return;
            
            receiver.providerReference.cooldown = cooldown;
            receiver.onSignal += Execute;
        }

        public void Disconnect()
        {
            if (!receiver.isConnected) 
                return;
            
            receiver.Disconnect();
            receiver.onSignal -= Execute;
        }

        public UIBehaviour SetSelectable(UISelectable uiSelectable)
        {
            Selectable = uiSelectable;
            return this;
        }

        public UIBehaviour ClearSelectable() =>
            SetSelectable(null);

        public override void Execute(Signal signal = null)
        {
            if (!Enabled)
                return;

            if (selectable != null && !selectable.IsActive() | !selectable.IsInteractable())
                return;

            foreach (ModyActionRunner runner in Runners)
                runner?.Execute();
            
            Event?.Invoke();
        }
    }

    public static class UIBehaviourExtensions
    {
        public static T SetSignalSource<T>(this T target, GameObject signalSource) where T : UIBehaviour
        {
            target.receiver.SetSignalSource(signalSource);
            return target;
        }

        public static T SetBehaviourName<T>(this T target, UIBehaviour.Name behaviourName) where T : UIBehaviour
        {
            target.receiver.SetProviderId(UIBehaviour.GetProvideId(behaviourName));
            return target;
        }

    }
}
