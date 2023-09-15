// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
using Object = UnityEngine.Object;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.Signals
{
    /// <summary>
    /// Object, used by the Signals system, that is sent from SignalProviders to SignalReceivers
    /// </summary>
    public partial class Signal
    {
        /// <summary> The stream this Signal was sent through </summary>
        public SignalStream stream { get; protected internal set; }

        /// <summary> Reference to the signal provider that sent this Signal </summary>
        public SignalProvider signalProvider { get; protected internal set; }

        /// <summary> Signal provider type </summary>
        public Type providerType { get; protected internal set; }

        /// <summary> Returns TRUE if this Signal was sent by a signal provider </summary>
        public bool hasProvider => signalProvider != null;

        /// <summary> Reference to the Object that sent this this Signal </summary>
        public Object signalSenderObject { get; protected internal set; }

        /// <summary> Sender type </summary>
        public Type senderType { get; protected internal set; }

        /// <summary> Returns TRUE if this Signal has a reference to its sender </summary>
        public bool hasSenderObject => signalSenderObject != null;

        /// <summary> Source GameObject from where this Signal originates from (can be null) </summary>
        public GameObject sourceGameObject { get; protected internal set; }

        /// <summary> Returns TRUE if this Signal has a source gameObject </summary>
        public bool hasSourceGameObject => sourceGameObject != null;

        /// <summary> Returns TRUE if this is a MetaSignal (has value) </summary>
        public bool hasValue { get; protected internal set; }

        /// <summary> Signal value type (if MetaSignal) </summary>
        public Type valueType { get; protected internal set; }

        /// <summary> Signal value boxed as an object (used by MetaSignal) </summary>
        public object valueAsObject { get; protected internal set; }
        
        /// <summary> Was the signal used </summary>
        public bool used { get; protected internal set; }

        /// <summary> Timestamp for the signal </summary>
        public float timestamp { get; protected internal set; }

        /// <summary> Text message used to pass info about this Signal </summary>
        public string message { get; protected internal set; }

        public Signal()
        {
            stream = null;

            signalProvider = null;
            providerType = null;

            signalSenderObject = null;
            senderType = null;

            sourceGameObject = null;

            hasValue = false;
            valueType = null;
            valueAsObject = null;

            used = false;

            timestamp = Time.time;

            message = string.Empty;
        }

        /// <summary> Create a Signal </summary>
        /// <param name="stream"> Reference to the SignalStream this Signal is sent through </param>
        /// <param name="hasValue"> Indicator if a Signal has a value payload (FALSE for Signal and TRUE for MetaSignal) </param>
        /// <param name="valueType"> Type of the value (if the Signal has one) - used by MetaSignals </param>
        internal Signal(SignalStream stream, bool hasValue = false, Type valueType = default) : this(stream, null, null, null, hasValue, valueType) {}

        /// <summary> Create a Signal and set a reference to the GameObject from where it is sent </summary>
        /// <param name="stream"> Reference to the SignalStream this Signal is sent through </param>
        /// <param name="signalSource"> Reference to the GameObject from where this Signal is sent </param>
        /// <param name="hasValue"> Indicator if a Signal has a value payload (FALSE for Signal and TRUE for MetaSignal) </param>
        /// <param name="valueType"> Type of the value (if the Signal has one) - used by MetaSignals </param>
        internal Signal(SignalStream stream, GameObject signalSource, bool hasValue = false, Type valueType = default) : this(stream, signalSource, null, null, hasValue, valueType) {}

        /// <summary> Create a Signal and set a reference to the SignalProvider that sends it </summary>
        /// <param name="stream"> Reference to the SignalStream this Signal is sent through </param>
        /// <param name="signalProvider"> Reference to the SignalProvider that sends this Signal </param>
        /// <param name="hasValue"> Indicator if a Signal has a value payload (FALSE for Signal and TRUE for MetaSignal) </param>
        /// <param name="valueType"> Type of the value (if the Signal has one) - used by MetaSignals </param>
        internal Signal(SignalStream stream, SignalProvider signalProvider, bool hasValue = false, Type valueType = default) : this(stream, null, signalProvider, null, hasValue, valueType) {}

        /// <summary> Create a Signal and set a reference to the Object that sends it </summary>
        /// <param name="stream"> Reference to the SignalStream this Signal is sent through </param>
        /// <param name="senderObject"> Reference to the Object that sends this Signal </param>
        /// <param name="hasValue"> Indicator if a Signal has a value payload (FALSE for Signal and TRUE for MetaSignal) </param>
        /// <param name="valueType"> Type of the value (if the Signal has one) - used by MetaSignals </param>
        internal Signal(SignalStream stream, Object senderObject, bool hasValue = false, Type valueType = default) : this(stream, null, null, senderObject, hasValue, valueType) {}

        internal Signal(SignalStream stream, GameObject signalSource, SignalProvider signalProvider, Object signalSender, bool hasValue = false, Type valueType = default)
        {
            this
                .Reset()
                .SetStream(stream)
                .SetValueType(hasValue, valueType);

            if (signalProvider != null)
            {
                this.SetSignalSource(signalProvider.gameObject);
                this.SetSignalProvider(signalProvider);
                this.SetSignalSender(signalProvider);
            }
            if (signalSource != null) this.SetSignalSource(signalSource);
            if (signalSender != null) this.SetSignalSender(signalSender);
        }

        internal void Recycle() =>
            this.AddToPool();

        /// <summary> Mark this signal as used </summary>
        public void Use() =>
            used = true;

        /// <summary> Try to get the signal value (if this signal is a MetaSignal) </summary>
        /// <param name="value"> Signal value </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public bool TryGetValue<T>(out T value)
        {
            if (hasValue)
            {
                try
                {
                    value = ((MetaSignal<T>)this).value;
                    return true;
                }
                catch
                {
                    // ignored
                }
            }
            value = default;
            return false;
        }

        /// <summary>
        /// Get the signal value via a direct cast. Use TryGetValue as a safe option (to perform the operation in a try catch block).
        /// This is an unsafe method, so use with care.
        /// </summary>
        /// <typeparam name="T"> Value type </typeparam>
        public T GetValueUnsafe<T>() =>
            ((MetaSignal<T>)this).value;

        /// <summary> Try to get the signal value type (if this signal is a MetaSignal) </summary>
        /// <param name="type"> Signal value type </param>
        public bool TryGetValueType(out Type type)
        {
            if (hasValue)
            {
                type = valueType;
                return true;
            }
            type = default;
            return false;
        }

        #region Static Methods

        #region Send Signal - using a stream Category and Name

        /// <summary> Send a Signal on the stream with the given stream category and name </summary>
        /// <param name="streamCategory"> Target stream category </param>
        /// <param name="streamName"> Target stream name </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool Send(string streamCategory, string streamName, string message = "") =>
            SignalsService.SendSignal(streamCategory, streamName, message);

        /// <summary> Send a Signal on the stream with the given stream category and name, with a reference to the GameObject from where it is sent </summary>
        /// <param name="streamCategory"> Target stream category </param>
        /// <param name="streamName"> Target stream name </param>
        /// <param name="signalSource"> Reference to the GameObject from where this Signal is sent </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool Send(string streamCategory, string streamName, GameObject signalSource, string message = "") =>
            SignalsService.SendSignal(streamCategory, streamName, signalSource, message);

        /// <summary> Send a Signal on the stream with the given stream category and name, with a reference to the SignalProvider that sent it </summary>
        /// <param name="streamCategory"> Target stream category </param>
        /// <param name="streamName"> Target stream name </param>
        /// <param name="signalProvider"> Reference to the SignalProvider that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool Send(string streamCategory, string streamName, SignalProvider signalProvider, string message = "") =>
            SignalsService.SendSignal(streamCategory, streamName, signalProvider, message);

        /// <summary> Send a Signal on the stream with the given stream category and name, with a reference to the Object that sent it </summary>
        /// <param name="streamCategory"> Target stream category </param>
        /// <param name="streamName"> Target stream name </param>
        /// <param name="signalSender"> Reference to the Object that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool Send(string streamCategory, string streamName, Object signalSender, string message = "") =>
            SignalsService.SendSignal(streamCategory, streamName, signalSender, message);

        #endregion

        #region Send Signal - using a Guid streamKey

        /// <summary> Send a Signal on the stream with the given stream key (Guid) </summary>
        /// <param name="streamKey"> Target stream key (Guid) </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool Send(Guid streamKey, string message = "") =>
            SignalsService.SendSignal(streamKey, message);

        /// <summary> Send a Signal on the stream with the given stream key (Guid), with a reference to the GameObject from where it was sent </summary>
        /// <param name="streamKey"> Target stream key (Guid) </param>
        /// <param name="signalSource"> Reference to the GameObject from where this Signal is sent from </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool Send(Guid streamKey, GameObject signalSource, string message = "") =>
            SignalsService.SendSignal(streamKey, signalSource, message);

        /// <summary> Send a Signal on the stream with the given stream key (Guid), with a reference to the SignalProvider that sent it </summary>
        /// <param name="streamKey"> Target stream key (Guid) </param>
        /// <param name="signalProvider"> Reference to the SignalProvider that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool Send(Guid streamKey, SignalProvider signalProvider, string message = "") =>
            SignalsService.SendSignal(streamKey, signalProvider, message);

        /// <summary> Send a Signal on the stream with the given stream key (Guid), with a reference to the Object that sent it </summary>
        /// <param name="streamKey"> Target stream key (Guid) </param>
        /// <param name="signalSender"> Reference to the Object that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool Send(Guid streamKey, Object signalSender, string message = "") =>
            SignalsService.SendSignal(streamKey, signalSender, message);

        #endregion

        #region Send Signal - using a SignalStream reference

        /// <summary> Send a Signal on the given stream </summary>
        /// <param name="stream"> Target signal stream </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool Send(SignalStream stream, string message = "") =>
            SignalsService.SendSignal(stream, message);

        /// <summary> Send a Signal on the given stream, with a reference to the GameObject from where it was sent </summary>
        /// <param name="stream"> Target signal stream </param>
        /// <param name="signalSource"> Reference to the GameObject from where this Signal is sent from </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool Send(SignalStream stream, GameObject signalSource, string message = "") =>
            SignalsService.SendSignal(stream, signalSource, message);

        /// <summary> Send a Signal on the given stream, with a reference to the SignalProvider that sent it </summary>
        /// <param name="stream"> Target signal stream </param>
        /// <param name="signalProvider"> Reference to the SignalProvider that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool Send(SignalStream stream, SignalProvider signalProvider, string message = "") =>
            SignalsService.SendSignal(stream, signalProvider, message);

        /// <summary> Send a Signal on the given stream, with a reference to the Object that sent it </summary>
        /// <param name="stream"> Target signal stream </param>
        /// <param name="signalSender"> Reference to the Object that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool Send(SignalStream stream, Object signalSender, string message = "") =>
            SignalsService.SendSignal(stream, signalSender, message);

        #endregion

        #region Send MetaSignal - using a stream Category and Name

        /// <summary> Send a MetaSignal on the stream with the given stream category and name </summary>
        /// <param name="streamCategory"> Target stream category </param>
        /// <param name="streamName"> Target stream name </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool Send<T>(string streamCategory, string streamName, T signalValue, string message = "") =>
            SignalsService.SendSignal(streamCategory, streamName, signalValue, message);

        /// <summary> Send a MetaSignal on the stream with the given stream category and name, with a reference to the GameObject from where it is sent </summary>
        /// <param name="streamCategory"> Target stream category </param>
        /// <param name="streamName"> Target stream name </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="signalSource"> Reference to the GameObject from where this Signal is sent </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public static bool Send<T>(string streamCategory, string streamName, T signalValue, GameObject signalSource, string message = "") =>
            SignalsService.SendSignal(streamCategory, streamName, signalValue, signalSource, message);

        /// <summary> Send a MetaSignal on the stream with the given stream category and name, with a reference to the SignalProvider that sent it </summary>
        /// <param name="streamCategory"> Target stream category </param>
        /// <param name="streamName"> Target stream name </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="signalProvider"> Reference to the SignalProvider that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public static bool Send<T>(string streamCategory, string streamName, T signalValue, SignalProvider signalProvider, string message = "") =>
            SignalsService.SendSignal(streamCategory, streamName, signalValue, signalProvider, message);

        /// <summary> Send a MetaSignal on the stream with the given stream category and name, with a reference to the Object that sent it </summary>
        /// <param name="streamCategory"> Target stream category </param>
        /// <param name="streamName"> Target stream name </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="signalSender"> Reference to the Object that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public static bool Send<T>(string streamCategory, string streamName, T signalValue, Object signalSender, string message = "") =>
            SignalsService.SendSignal(streamCategory, streamName, signalValue, signalSender, message);

        #endregion

        #region Send MetaSignal - using a Guid streamKey

        /// <summary> Send a MetaSignal on the stream with the given stream key (Guid) </summary>
        /// <param name="streamKey"> Target stream key (Guid) </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public static bool Send<T>(Guid streamKey, T signalValue, string message = "") =>
            SignalsService.SendSignal(streamKey, signalValue, message);

        /// <summary> Send a MetaSignal on the stream with the given stream key (Guid), with a reference to the GameObject from where it was sent </summary>
        /// <param name="streamKey"> Target stream key (Guid) </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="signalSource"> Reference to the GameObject from where this Signal is sent from </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public static bool Send<T>(Guid streamKey, T signalValue, GameObject signalSource, string message = "") =>
            SignalsService.SendSignal(streamKey, signalValue, signalSource, message);

        /// <summary> Send a MetaSignal on the stream with the given stream key (Guid), with a reference to the SignalProvider that sent it </summary>
        /// <param name="streamKey"> Target stream key (Guid) </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="signalProvider"> Reference to the SignalProvider that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public static bool Send<T>(Guid streamKey, T signalValue, SignalProvider signalProvider, string message = "") =>
            SignalsService.SendSignal(streamKey, signalValue, signalProvider, message);

        /// <summary> Send a MetaSignal on the stream with the given stream key (Guid), with a reference to the Object that sent it </summary>
        /// <param name="streamKey"> Target stream key (Guid) </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="signalSender"> Reference to the Object that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public static bool Send<T>(Guid streamKey, T signalValue, Object signalSender, string message = "") =>
            SignalsService.SendSignal(streamKey, signalValue, signalSender, message);

        #endregion

        #region Send MetaSignal - using a SignalStream reference

        /// <summary> Send a MetaSignal on the given stream </summary>
        /// <param name="stream"> Target signal stream </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public static bool Send<T>(SignalStream stream, T signalValue, string message = "") =>
            SignalsService.SendSignal(stream, signalValue, message);

        /// <summary> Send a MetaSignal on the given stream, with a reference to the GameObject from where it was sent </summary>
        /// <param name="stream"> Target signal stream </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="signalSource"> Reference to the GameObject from where this Signal is sent from </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public static bool Send<T>(SignalStream stream, T signalValue, GameObject signalSource, string message = "") =>
            SignalsService.SendSignal(stream, signalValue, signalSource, message);

        /// <summary> Send a MetaSignal on the given stream, with a reference to the SignalProvider that sent it </summary>
        /// <param name="stream"> Target signal stream </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="signalProvider"> Reference to the SignalProvider that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public static bool Send<T>(SignalStream stream, T signalValue, SignalProvider signalProvider, string message = "") =>
            SignalsService.SendSignal(stream, signalValue, signalProvider, message);

        /// <summary> Send a MetaSignal on the given stream, with a reference to the Object that sent it </summary>
        /// <param name="stream"> Target signal stream </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="signalSender"> Reference to the Object that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public static bool Send<T>(SignalStream stream, T signalValue, Object signalSender, string message = "") =>
            SignalsService.SendSignal(stream, signalValue, signalSender, message);

        #endregion

        #endregion
    }

    public static class SignalExtensions
    {
        internal static T Reset<T>(this T target) where T : Signal
        {
            target.stream = null;

            target.signalProvider = null;
            target.providerType = null;

            target.signalSenderObject = null;
            target.senderType = null;

            target.sourceGameObject = null;

            target.used = false;

            target.timestamp = Time.time;

            target.hasValue = false;
            target.valueType = null;
            target.valueAsObject = null;

            if (target is MetaSignal<T> metaSignal)
                metaSignal.ResetValue();

            return target;
        }

        internal static T SetValueType<T>(this T target, bool hasValue = false, Type valueType = default) where T : Signal
        {
            target.hasValue = hasValue;
            target.valueType = valueType;
            return target;
        }

        internal static T SetStream<T>(this T target, SignalStream stream) where T : Signal
        {
            target.stream = stream;
            return target;
        }

        internal static T SetSignalProvider<T>(this T target, SignalProvider signalProvider) where T : Signal
        {
            target.signalProvider = signalProvider;
            target.providerType = signalProvider != null ? signalProvider.GetType() : null;
            return target;
        }

        internal static T SetSignalSender<T>(this T target, Object signalSender) where T : Signal
        {
            target.signalSenderObject = signalSender;
            target.senderType = signalSender != null ? signalSender.GetType() : null;
            return target;
        }

        internal static T SetSignalSource<T>(this T target, GameObject sourceGameObject) where T : Signal
        {
            target.sourceGameObject = sourceGameObject;
            return target;
        }

        internal static T SetTimestamp<T>(this T target) where T : Signal
        {
            target.timestamp = Time.time;
            return target;
        }

        internal static T SetMessage<T>(this T target, string message) where T : Signal
        {
            target.message = message;
            return target;
        }
    }
}
