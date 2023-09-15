// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Signals;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody
{
    [Serializable]
    public abstract class MetaModyAction<T> : ModyAction
    {
        public T ActionValue;

        public UnityAction<T> actionCallback { get; private set; }

        protected MetaModyAction(MonoBehaviour behaviour, string actionName, UnityAction<T> callback) : base(behaviour, actionName)
        {
            actionCallback = callback;
            HasValue = true;
            ValueType = typeof(T);

            IgnoreSignalValue = false;
            ReactToAnySignal = false;
        }

        protected override void Run(Signal signal)
        {
            if (ReactToAnySignal)
            {
                if (IgnoreSignalValue)
                {
                    actionCallback?.Invoke(ActionValue); //invoke callbacks
                    return;
                }

                //do not react to any signal --> check for valid signal to update the value
                if (signal != null && signal.valueType == ValueType)
                    ActionValue = signal.GetValueUnsafe<T>(); //get value from signal

                actionCallback?.Invoke(ActionValue); //invoke callbacks
                return;
            }

            //do not react to any signal --> check for valid signal
            if (signal == null) return; //signal is null --> return
            if (!signal.hasValue) return; //signal does not have value --> return
            if (signal.valueType != ValueType) return; //signal value type does not match the action value type --> return

            if (IgnoreSignalValue)
            {
                actionCallback?.Invoke(ActionValue); //invoke callbacks
                return;
            }

            ActionValue = signal.GetValueUnsafe<T>(); //get value from signal
            actionCallback?.Invoke(ActionValue); //invoke callbacks
        }

        public void SetValue(T value)
        {
            ActionValue = value;
        }

        public override bool SetValue(object objectValue) => 
            SetValue(objectValue, true);
        
        internal override bool SetValue(object objectValue, bool restrictValueType)
        {
            if (objectValue == null) return false;
            if (restrictValueType && objectValue.GetType() != ValueType)
                return false;
            try
            {
                SetValue((T)objectValue);
                return true;
            }
            catch
            {
                // ignored
                return false;
            }
        }
    }
}
