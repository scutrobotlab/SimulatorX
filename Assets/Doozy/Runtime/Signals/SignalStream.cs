// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Runtime.Pooler;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace Doozy.Runtime.Signals
{
    public class SignalStream
    {
        public const string k_None = "None";
        public const string k_DefaultCategory = k_None;
        public const string k_DefaultName = k_None;

        /// <summary> Stream key (unique) Guid) </summary>
        public Guid key { get; }

        /// <summary> Stream category </summary>
        public string category { get; private set; } = k_DefaultCategory;

        /// <summary> Stream name </summary>
        public string name { get; private set; } = k_DefaultName;

        /// <summary> Number of signals sent through this stream since its creation </summary>
        public int signalsCounter { get; private set; }

        /// <summary> Stream receivers </summary>
        public List<ISignalReceiver> receivers { get; } = new List<ISignalReceiver>();

        /// <summary> Number of registered receivers listening to this stream </summary>
        public int receiversCount => receivers.Count;

        /// <summary> Action invoked every time a receiver connects to this stream </summary>
        public UnityAction<ISignalReceiver> OnReceiverConnected;

        /// <summary> Action invoked every time a receiver disconnects from this stream </summary>
        public UnityAction<ISignalReceiver> OnReceiverDisconnected;

        /// <summary> Reference to the previously sent signal (can be null) </summary>
        public Signal previousSignal { get; protected set; }

        /// <summary> Reference to the current signal (cen be null) </summary>
        public Signal currentSignal { get; protected set; }

        /// <summary> Action invoked every time a Signal is sent through this stream </summary>
        public UnityAction<Signal> OnSignal;

        /// <summary> Reference to the SignalProvider that created this stream (can be null) </summary>
        public SignalProvider signalProvider { get; protected set; }

        /// <summary> Returns TRUE if this stream has a signal provider reference </summary>
        public bool hasProvider => signalProvider != null;

        /// <summary> Text message used to display custom info about this stream </summary>
        public string infoMessage { get; protected set; }

        private HashSet<ISignalReceiver> sendTempList { get; } = new HashSet<ISignalReceiver>();
        private HashSet<ISignalReceiver> disconnectTempList { get; } = new HashSet<ISignalReceiver>();

        internal SignalStream(Guid streamKey)
        {
            key = streamKey;
        }

        internal SignalStream SetCategory(string streamCategory)
        {
            category = streamCategory;
            return this;
        }

        internal SignalStream SetName(string streamName)
        {
            name = streamName;
            return this;
        }

        internal SignalStream SetSignalProvider(SignalProvider provider)
        {
            signalProvider = provider;
            return this;
        }

        internal SignalStream SetInfoMessage(string message)
        {
            infoMessage = message;
            return this;
        }

        /// <summary> Connect a receiver to this stream </summary>
        /// <param name="receiver"> Target receiver </param>
        public virtual SignalStream ConnectReceiver(ISignalReceiver receiver)
        {
            if (receiver == null) return this;
            if (receivers.Contains(receiver)) return this;
            receivers.Add(receiver);
            OnReceiverConnected?.Invoke(receiver);
            return this;
        }

        /// <summary> Disconnect a receiver from this stream </summary>
        /// <param name="receiver"> Target receiver </param>
        public virtual void DisconnectReceiver(ISignalReceiver receiver)
        {
            if (receiver == null) return;
            if (!receivers.Contains(receiver)) return;
            receivers.Remove(receiver);
            if (receiver.stream != this) return;
            receiver.Disconnect();
            OnReceiverDisconnected?.Invoke(receiver);
        }

        /// <summary> Disconnect all receivers from this stream </summary>
        public virtual void DisconnectAllReceivers()
        {
            receivers.Remove(null);
            foreach (ISignalReceiver receiver in receivers.ToArray())
            {
                if (receiver == null)
                    continue;
                DisconnectReceiver(receiver);
            }
            receivers.Clear();
        }

        /// <summary> Nullify all callbacks </summary>
        public virtual void ClearCallbacks()
        {
            OnSignal = null;
        }

        /// <summary> Close this stream by disconnecting all receivers and nullifying all callbacks </summary>
        public virtual void Close()
        {
            DisconnectAllReceivers();
            ClearCallbacks();
        }

        #region Signal

        /// <summary> Send a Signal on this stream </summary>
        public virtual bool SendSignal(string message = "") => SendSignal(null, null, null, message);

        /// <summary> Send a Signal on this stream, with a reference to the GameObject from where it was sent </summary>
        /// <param name="signalSource"> Reference to the GameObject from where this Signal is sent from </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public virtual bool SendSignal(GameObject signalSource, string message = "") => SendSignal(signalSource, null, null, message);

        /// <summary> Send a Signal on this stream, with a reference to the SignalProvider that sent it </summary>
        /// <param name="provider"> Reference to the SignalProvider that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public virtual bool SendSignal(SignalProvider provider, string message = "") => SendSignal(null, provider, null, message);

        /// <summary> Send a Signal on this stream, with a reference to the Object that sent it </summary>
        /// <param name="signalSender"> Reference to the Object that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public virtual bool SendSignal(Object signalSender, string message = "") => SendSignal(null, null, signalSender, message);

        /// <summary> Send a Signal with everything and the kitchen sink </summary>
        /// <param name="signalSource"> Reference to the GameObject from where this Signal is sent from </param>
        /// <param name="provider"> Reference to the SignalProvider that sends this Signal </param>
        /// <param name="signalSender"> Reference to the Object that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public virtual bool SendSignal(GameObject signalSource, SignalProvider provider, Object signalSender, string message = "")
        {
            Signal signal = SignalPool.Get<Signal>().Reset();
            if (provider != null)
            {
                signal.SetSignalSource(provider.gameObject);
                signal.SetSignalProvider(provider);
                signal.SetSignalSender(provider);
            }
            if (signalSource != null) signal.SetSignalSource(signalSource);
            if (signalSender != null) signal.SetSignalSender(signalSender);
            signal.SetMessage(message);
            return Send(signal);
        }

        #endregion

        #region MetaSignal

        /// <summary> Send a MetaSignal on this stream </summary>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public virtual bool SendSignal<T>(T signalValue, string message = "") => SendSignal(signalValue, null, null, null, message);

        /// <summary> Send a MetaSignal on this stream, with a reference to the GameObject from where it was sent </summary>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="signalSource"> Reference to the GameObject from where this Signal is sent from </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public virtual bool SendSignal<T>(T signalValue, GameObject signalSource, string message = "") => SendSignal(signalValue, signalSource, null, null, message);

        /// <summary> Send a MetaSignal on this stream, with a reference to the SignalProvider that sent it </summary>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="provider"> Reference to the SignalProvider that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public virtual bool SendSignal<T>(T signalValue, SignalProvider provider, string message = "") => SendSignal(signalValue, null, provider, null, message);

        /// <summary> Send a MetaSignal on this stream, with a reference to the Object that sent it </summary>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="signalSender"> Reference to the Object that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public virtual bool SendSignal<T>(T signalValue, Object signalSender, string message = "") => SendSignal(signalValue, null, null, signalSender, message);

        /// <summary> Send a MetaSignal with everything and the kitchen sink </summary>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="signalSource"> Reference to the GameObject from where this Signal is sent from </param>
        /// <param name="provider"> Reference to the SignalProvider that sends this Signal </param>
        /// <param name="signalSender"> Reference to the Object that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public virtual bool SendSignal<T>(T signalValue, GameObject signalSource, SignalProvider provider, Object signalSender, string message = "")
        {
            MetaSignal<T> metaSignal = SignalPool.Get<MetaSignal<T>>().Reset();
            metaSignal.SetSignalValue(signalValue);
            if (provider != null)
            {
                metaSignal.SetSignalSource(provider.gameObject);
                metaSignal.SetSignalProvider(provider);
                metaSignal.SetSignalSender(provider);
            }
            if (signalSource != null) metaSignal.SetSignalSource(signalSource);
            if (signalSender != null) metaSignal.SetSignalSender(signalSender);
            metaSignal.SetMessage(message);
            return Send(metaSignal);
        }

        #endregion

        private bool Send(Signal signal)
        {
            // Debug.Log($"{nameof(SignalStream)}.{nameof(Send)}({nameof(Signal)}): {signal}");
            signal.SetStream(this);
            signalsCounter++;
            if (previousSignal != null)
            {
                //if the value is poolable -> recycle it (save memory allocations)
                if (previousSignal.hasValue && previousSignal.valueAsObject is IPoolable poolable)
                    poolable.Recycle();

                previousSignal.Recycle();
            }
            previousSignal = currentSignal;
            currentSignal = signal;
            OnSignal?.Invoke(currentSignal);
            SignalsService.OnSignal?.Invoke(currentSignal);
            receivers.Remove(null);
            foreach (ISignalReceiver receiver in receivers.ToArray())
                receiver?.OnSignal(currentSignal);
            return true;
        }

        #region Static Methods

        /// <summary> Create a new stream and get a reference to it </summary>
        public static SignalStream Get() =>
            SignalsService.GetStream();

        /// <summary>
        /// Get the stream with the given stream category and name or,
        /// if not found, create a new stream and return a reference to it
        /// </summary>
        /// <param name="streamCategory"> Stream category </param>
        /// <param name="streamName"> Stream name </param>
        public static SignalStream Get(string streamCategory, string streamName) =>
            SignalsService.GetStream(streamCategory, streamName);

        /// <summary> Get the stream with the given stream key. If not found, this method, returns null </summary>
        /// <param name="streamKey"> Stream key to search for </param>
        public static SignalStream Get(Guid streamKey) =>
            SignalsService.FindStream(streamKey);

        #endregion
    }
}
