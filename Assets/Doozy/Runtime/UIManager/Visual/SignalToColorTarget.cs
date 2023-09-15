// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.Signals;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global
namespace Doozy.Runtime.UIManager.Visual
{
    /// <summary>
    /// Connects to a specific stream and reacts to signals, sent through said stream, that have a Color value payload.
    /// The signal's Color value will be assigned to the target ColorTarget. 
    /// </summary>
    [AddComponentMenu("Doozy/UI/Visual/Signal To Color Target")]
    public class SignalToColorTarget : BaseStreamListener
    {
        [SerializeField] private StreamId StreamId;
        /// <summary> Stream Id </summary>
        public StreamId streamId => StreamId;

        [SerializeField] private ReactorColorTarget ColorTarget;
        /// <summary> Reference to a color target component </summary>
        public ReactorColorTarget colorTarget => ColorTarget;
        
        /// <summary> Check if a color target is referenced or not </summary>
        public bool hasColorTarget => ColorTarget != null;

        public SignalStream stream { get; private set; }
        
        #if UNITY_EDITOR
        private void Reset()
        {
            FindTarget();
        }
        #endif

        public void FindTarget()
        {
            if (ColorTarget != null)
                return;

            ColorTarget = ReactorColorTarget.FindTarget(gameObject);
        }

        private void Awake()
        {
            FindTarget();
        }

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
            if (!hasColorTarget)
                return;

            if (signal == null)
                return;

            if (!signal.hasValue)
                return;

            if (signal.valueType != typeof(Color))
                return;
            
            SetColor(signal.GetValueUnsafe<Color>());
        }
        
        public void SetColor(Color color)
        {
            if (ColorTarget != null)
                ColorTarget.SetColor(color);
        }
    }
}
