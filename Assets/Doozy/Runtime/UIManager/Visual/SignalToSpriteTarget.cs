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
    /// Connects to a specific stream and reacts to signals, sent through said stream, that have a Sprite value payload.
    /// The signal's Sprite value will be assigned to the target SpriteTarget. 
    /// </summary>
    [AddComponentMenu("Doozy/UI/Visual/Signal To Sprite Target")]
    public class SignalToSpriteTarget : BaseStreamListener
    {
        [SerializeField] private StreamId StreamId;
        /// <summary> Stream Id </summary>
        public StreamId streamId => StreamId;

        [SerializeField] private ReactorSpriteTarget SpriteTarget;
        /// <summary> Reference to a sprite target component </summary>
        public ReactorSpriteTarget spriteTarget => SpriteTarget;

        /// <summary> Check if a sprite target is referenced or not </summary>
        public bool hasSpriteTarget => SpriteTarget != null;

        public SignalStream stream { get; private set; }
        
        #if UNITY_EDITOR
        private void Reset()
        {
            FindTarget();
        }
        #endif

        public void FindTarget()
        {
            if (SpriteTarget != null)
                return;

            SpriteTarget = ReactorSpriteTarget.FindTarget(gameObject);
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
            if (!hasSpriteTarget)
                return;

            if (signal == null)
                return;

            if (!signal.hasValue)
                return;

            if (signal.valueType != typeof(Sprite))
                return;
            
            SetSprite(signal.GetValueUnsafe<Sprite>());
        }
        
        public void SetSprite(Sprite sprite)
        {
            if (SpriteTarget != null)
                SpriteTarget.SetSprite(sprite);
        }
    }
}
