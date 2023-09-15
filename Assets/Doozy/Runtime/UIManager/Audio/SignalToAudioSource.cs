// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Signals;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Audio
{
    /// <summary>
    /// Connects to a specific stream and reacts to signals, sent through said stream, that have an AudioClip value payload.
    /// The signal's AudioClip value will be assigned to and played by the target AudioSource. 
    /// </summary>
    [AddComponentMenu("Doozy/UI/Audio/Signal To AudioSource")]
    public class SignalToAudioSource : BaseStreamListener
    {
        [SerializeField] private StreamId StreamId;
        /// <summary> Stream Id </summary>
        public StreamId streamId => StreamId;

        [SerializeField] private AudioSource AudioSource;
        /// <summary> Reference to a target Audio source </summary>
        public AudioSource audioSource => AudioSource;

        /// <summary> Check if a AudioSource is referenced or not </summary>
        public bool hasAudioSource => AudioSource != null;

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
            if (!hasAudioSource)
                return;

            if (signal == null)
                return;

            if (!signal.hasValue)
                return;

            if (signal.valueType != typeof(AudioClip))
                return;
            
            audioSource.Stop();
            audioSource.clip = signal.GetValueUnsafe<AudioClip>();
            if (audioSource.clip != null)
                audioSource.Play();
        }
    }
}
