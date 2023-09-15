// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;

namespace Doozy.Runtime.Signals
{
    /// <summary> Object containing a value (or reference), used by the Signals system </summary>
    /// <typeparam name="T"> Signal value type </typeparam>
    public class MetaSignal<T> : Signal
    {
        public T value { get; private set; }

        /// <summary> Create a MetaSignal </summary>
        public MetaSignal() : base(null, true, typeof(T))
        {
            SetSignalValue(default);
        }

        /// <summary> Create a MetaSignal and set a reference to the GameObject from where it is sent </summary>
        /// <param name="stream"> Reference to the SignalStream this Signal is sent through </param>
        /// <param name="signalSource"> Reference to the GameObject from where this Signal is sent </param>
        internal MetaSignal(SignalStream stream, GameObject signalSource) : base(stream, signalSource, true, typeof(T))
        {
            SetSignalValue(default);
        }

        /// <summary> Create a MetaSignal and set a reference to the SignalProvider that sends it </summary>
        /// <param name="stream"> Reference to the SignalStream this Signal is sent through </param>
        /// <param name="signalProvider"> Reference to the SignalProvider that sends this Signal </param>
        internal MetaSignal(SignalStream stream, SignalProvider signalProvider) : base(stream, signalProvider, true, typeof(T))
        {
            SetSignalValue(default);
        }

        /// <summary> Create a MetaSignal and set a reference to the Object that sends it </summary>
        /// <param name="stream"> Reference to the SignalStream this Signal is sent through </param>
        /// <param name="senderObject"> Reference to the Object that sends this Signal </param>
        internal MetaSignal(SignalStream stream, Object senderObject) : base(stream, senderObject, true, typeof(T))
        {
            SetSignalValue(default);
        }
        
        internal void SetSignalValue(T signalValue)
        {
            this.SetValueType(true, typeof(T));
            value = signalValue;
            valueAsObject = signalValue;
        }

        internal void ResetValue()
        {
            hasValue = false;
            valueType = null;
            value = default;
            valueAsObject = null;
        }
    }
}
