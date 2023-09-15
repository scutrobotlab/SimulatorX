// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using Doozy.Runtime.Common;
using Doozy.Runtime.Mody;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Signals
{
    public abstract partial class SignalProvider : MonoBehaviour, ISignalProvider
    {
        public ProviderAttributes attributes { get; }
        public SignalStream stream { get; private set; }
        public bool isConnected { get; private set; }

        #region Provider State

        /// <summary> Provider current state (disabled, idle, running or cooldown) </summary>
        [SerializeField] private ProviderState ProviderCurrentState;

        /// <summary> Provider current state (disabled, idle, running or cooldown) </summary>
        public ProviderState currentState
        {
            get => ProviderCurrentState;
            private set
            { 
                ProviderCurrentState = value;
                onStateChanged?.Invoke(value);
            }
        }

        public UnityAction<ProviderState> onStateChanged { get; set; }
        
        /// <summary> Provider is ready and can be triggered </summary>
        public bool isIdle => currentState == ProviderState.Idle;

        /// <summary> Provider has started and is running </summary>
        public bool isRunning => currentState == ProviderState.IsRunning;

        /// <summary> Provider is in the 'InCooldown' state and cannot be triggered again during this time </summary>
        public bool inCooldown => currentState == ProviderState.InCooldown;

        /// <summary> Provider has started running and is either in 'IsRunning' or 'InCooldown' state </summary>
        public bool isActive => isRunning || inCooldown;

        #endregion
        
        #region Signal Cooldown

        /// <summary> Cooldown time after a signal was sent. During this time, no Signal will be sent </summary>
        [SerializeField] private float SignalCooldown;
        
        /// <summary> Cooldown time after a signal was sent. During this time, no Signal will be sent </summary>
        public float cooldown
        {
            get => SignalCooldown > 0 ? SignalCooldown : 0;
            set => SignalCooldown = value > 0 ? value : 0;
        }

        #endregion
        
        #region Signal Timescale Independent

        /// <summary>
        /// Determine if the Signal's timers will be affected by the application's timescale
        /// <para/> Timescale is the scale at which time passes 
        /// <para/> Timescale.Independent - (Realtime) Not affected by the application's timescale value
        /// <para/> Timescale.Dependent - (Application Time) Affected by the application's timescale value
        /// </summary>
        [SerializeField] private Timescale SignalTimescale;

        /// <summary>
        /// Determine if the Signal's timers will be affected by the application's timescale
        /// <para/> Timescale is the scale at which time passes 
        /// <para/> TRUE - Timescale.Independent - (Realtime) Not affected by the application's timescale value
        /// <para/> FALSE - Timescale.Dependent - (Application Time) Affected by the application's timescale value
        /// </summary>
        public bool isTimescaleIndependent
        {
            get => SignalTimescale == Timescale.Independent;
            internal set => SignalTimescale = value ? Timescale.Independent : Timescale.Dependent;
        }

        #endregion
        
        #region Coroutines

        private Coroutine m_CooldownCoroutine;

        #endregion
        
        protected SignalProvider(ProviderType providerType, string providerCategory, string providerName, Type typeOfProvider)
        {
            attributes = new ProviderAttributes(providerType, providerCategory, providerName, typeOfProvider);
            stream = null;

            ProviderCurrentState = ProviderState.Disabled;
            SignalCooldown = 0;
            SignalTimescale = Timescale.Independent;
        }

        public void OpenStream()
        {
            if (isConnected) return;
            stream = SignalsService.GetStream().SetSignalProvider(this);
            SignalsService.AddProvider(this);
            isConnected = true;
            currentState = ProviderState.Idle;
        }

        public void CloseStream()
        {
            if (!isConnected) return;
            SignalsService.CloseStream(stream);
            stream = null;
            isConnected = false;
            currentState = ProviderState.Disabled;
        }

        protected virtual void Awake()
        {
            stream = null;
            isConnected = false;
            OpenStream();
        }

        protected virtual void OnEnable()
        {
            currentState = isConnected ? ProviderState.Idle : ProviderState.Disabled;
        }

        protected virtual void OnDisable()
        {
            StopCooldown();
            currentState = ProviderState.Disabled;
        }

        protected virtual void OnDestroy() =>
            CloseStream();

        /// <summary> Send a Signal on this provider's stream </summary>
        public bool SendSignal() =>
            SendSignal(string.Empty);

        /// <summary> Send a Signal on this provider's stream </summary>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public bool SendSignal(string message)
        {
            bool result;
            switch (currentState)
            {
                case ProviderState.Disabled:
                case ProviderState.InCooldown:
                    return false;
                case ProviderState.Idle:
                case ProviderState.IsRunning:
                    currentState = ProviderState.IsRunning;
                    result = isConnected && stream.SendSignal(this, message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            StartCooldown();
            return result;
        }

        /// <summary> Send a MetaSignal with a T signal value on this provider's stream </summary>
        /// <param name="signalValue"> Signal value </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public bool SendSignal<T>(T signalValue) =>
            SendSignal<T>(signalValue, string.Empty);

        /// <summary> Send a MetaSignal with a T signal value on this provider's stream </summary>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public bool SendSignal<T>(T signalValue, string message)
        {
            bool result;
            switch (currentState)
            {
                case ProviderState.Disabled:
                case ProviderState.InCooldown:
                    return false;
                case ProviderState.Idle:
                case ProviderState.IsRunning:
                    currentState = ProviderState.IsRunning;
                    result = isConnected && stream.SendSignal(signalValue, this, message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            StartCooldown();
            return result;
        }

        /// <summary>
        /// Start the Provider's cooldown timer, that makes it unable to start running again until the timer finishes.
        /// <para/> If the Provider is in the 'Disabled' state, this method will NOT do anything.
        /// <para/> If the Provider is in the 'InCooldown' state, this method will restart the cooldown timer.
        /// </summary>
        public void StartCooldown()
        {
            if (currentState == ProviderState.Disabled)
            {
                return;
            }
            
            if (cooldown == 0)
            {
                currentState = ProviderState.Idle;
                return;
            }
            
            if (currentState == ProviderState.InCooldown)
            {
                StopCooldown();
            }
            
            m_CooldownCoroutine = StartCoroutine(ExecuteCooldown());
        }
        
        /// <summary>
        /// Executes the cooldown cycle as follows:
        /// <para/> >> (time interval) Cooldown
        /// <para/> >> (method) StopCooldown
        /// </summary>
        protected IEnumerator ExecuteCooldown()
        {
            currentState = ProviderState.InCooldown;

            if (isTimescaleIndependent)
            {
                yield return new WaitForSecondsRealtime(cooldown);
            }
            else
            {
                yield return new WaitForSeconds(cooldown);
            }

            StopCooldown();
        }

        /// <summary>
        /// Stop the Provider's cooldown timer and set it ready to start running again.
        /// </summary>
        public void StopCooldown()
        {
            if (m_CooldownCoroutine != null)
            {
                StopCoroutine(m_CooldownCoroutine);
                m_CooldownCoroutine = null;
            }

            currentState = isActiveAndEnabled && isConnected ? ProviderState.Idle : ProviderState.Disabled;
        }
    }
}
