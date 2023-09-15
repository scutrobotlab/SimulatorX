// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Signals
{
    [Serializable]
    public class SignalReceiver : ISignalReceiver
    {
        [SerializeField] private StreamConnection ConnectionMode;
        [SerializeField] private ProviderId SignalProviderId;
        [SerializeField] private SignalProvider ProviderReference;
        [SerializeField] private StreamId StreamId;

        public UnityAction<Signal> onSignal { get; set; }
      
        public StreamConnection streamConnection { get => ConnectionMode; protected internal set => ConnectionMode = value; }
        public ProviderId providerId { get => SignalProviderId; protected internal set => SignalProviderId = value; }
        public SignalProvider providerReference { get => ProviderReference; protected internal set => ProviderReference = value; }
        public StreamId streamId { get => StreamId; protected internal set => StreamId = value; }
        
        public GameObject signalSource { get; protected internal set; }
        
        public SignalStream stream { get; private set; }
        public bool isConnected { get; private set; }
        public bool isDisconnecting { get; private set; }

        public SignalReceiver()
        {
           Reset();
        }

        public ISignalReceiver Reset()
        {
            ConnectionMode = StreamConnection.None;
            SignalProviderId = new ProviderId();
            ProviderReference = null;
            StreamId = new StreamId();

            signalSource = null;
            stream = null;
            isConnected = false;
            isDisconnecting = false;

            return this;
        }
        
        public virtual void OnSignal(Signal signal) =>
            onSignal?.Invoke(signal);

        public void Connect()
        {
            if (!Application.isPlaying)
            {
                Disconnect();
                return;
            }
            
            if (isConnected) 
                return;
            
            switch (streamConnection)
            {
                case StreamConnection.None: return;
                case StreamConnection.ProviderId:
                    providerReference = (SignalProvider)SignalsService.GetProvider(providerId, signalSource);
                    if (providerReference == null)
                    {
                        Debug.Log("Provider not found!");
                        return;
                    }
                    if (!providerReference.isConnected) providerReference.OpenStream();
                    stream = providerReference.stream;
                    break;
                case StreamConnection.ProviderReference:
                    if (providerReference == null)
                    {
                        Debug.Log("Provider not referenced!");
                        return;
                    }
                    if (!providerReference.isConnected) providerReference.OpenStream();
                    stream = providerReference.stream;
                    break;
                case StreamConnection.StreamId:
                    if (streamId.Category.Equals(SignalStream.k_DefaultCategory) ||
                        streamId.Name.Equals(SignalStream.k_DefaultName))
                    {
                        Debug.Log($"Will not connect to {streamId.Category} > {streamId.Name}");
                        return;
                    }
                    stream = SignalsService.GetStream(streamId.Category, streamId.Name);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (stream == null) //ToDo: check this particular case when the stream is null
            {
                return;
            }
            
            stream.ConnectReceiver(this);
            streamId.SetStream(stream);
            isConnected = true;
        }

        public void Disconnect()
        {
            if (!isConnected) 
                return;
            
            SignalStream streamReference = stream;
            stream = null;
            isConnected = false;
            streamReference.DisconnectReceiver(this);
        }
    }

    public static class SignalReceiverExtensions
    {
        public static T SetStreamConnection<T>(this T target, StreamConnection streamConnection) where T : SignalReceiver
        {
            target.streamConnection = streamConnection;
            return target;
        }
        
        public static T SetProviderId<T>(this T target, ProviderId providerId, bool updateStreamConnection = true) where T : SignalReceiver
        {
            target.providerId = providerId;
            return updateStreamConnection ? target.SetStreamConnection(StreamConnection.ProviderId) : target;
        }

        public static T SetProviderReference<T>(this T target, SignalProvider providerReference, bool updateStreamConnection = true) where T : SignalReceiver
        {
            target.providerReference = providerReference;
            return updateStreamConnection ? target.SetStreamConnection(StreamConnection.ProviderReference) : target;
        }

        public static T SetStreamId<T>(this T target, StreamId streamId, bool updateStreamConnection = true) where T : SignalReceiver
        {
            target.streamId = streamId;
            return updateStreamConnection ? target.SetStreamConnection(StreamConnection.StreamId) : target;
        }

        public static T SetSignalSource<T>(this T target, GameObject signalSource) where T : SignalReceiver
        {
            if (target.isConnected) return target;
            target.signalSource = signalSource;
            return target;
        }

        public static T SetOnSignalCallback<T>(this T target, UnityAction<Signal> callback) where T : SignalReceiver
        {
            target.onSignal = callback;
            return target;
        }
        
        public static T AddOnSignalCallback<T>(this T target, UnityAction<Signal> callback) where T : SignalReceiver
        {
            target.onSignal += callback;
            return target;
        }
        
        public static T RemoveOnSignalCallback<T>(this T target, UnityAction<Signal> callback) where T : SignalReceiver
        {
            target.onSignal -= callback;
            return target;
        }
        
        public static T ClearOnSignalCallback<T>(this T target) where T : SignalReceiver
        {
            target.onSignal = null;
            return target;
        }
    } 
}
