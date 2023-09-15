// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Signals;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody
{
    /// <summary>
    /// Base Action class for the Mody System.
    /// <para/> Any action that interacts with the system needs to derive from this.
    /// <para/> It is used to start, stop and finish Module's (MonoBehaviour's) tasks.
    /// <para/> It can be triggered manually (via code) or by any Trigger registered to the system.
    /// </summary>
    [Serializable]
    public abstract class ModyAction : MultiSignalsReceiver<SignalReceiver>
    {
        #region Action Behaviour Reference

        /// <summary>
        /// The MonoBehaviour (Module) that this Action belongs to.
        /// <para/> Needs to implement the IHaveActions interface.
        /// </summary>
        [SerializeField] private MonoBehaviour ActionBehaviourReference;

        /// <summary>
        /// The MonoBehaviour (Module) that this Action belongs to.
        /// <para/> Needs to implement the IHaveActions interface.
        /// </summary>
        public MonoBehaviour actionBehaviourReference
        {
            get => ActionBehaviourReference;
            internal set
            {
                ActionBehaviourReference = value;
                m_BehaviourIsModule = false;
                if (!(ActionBehaviourReference is ModyModule module)) return;
                m_Module = module;
                m_BehaviourIsModule = true;
            }
        }

        #endregion

        #region Action Name

        /// <summary> Name of the Action </summary>
        [SerializeField] private string ActionName;

        /// <summary> Name of the Action </summary>
        public string actionName => ActionName;

        #endregion

        #region Action State

        /// <summary> Action current state (disabled, idle, running or cooldown) </summary>
        [SerializeField] private ActionState ActionCurrentState;

        /// <summary> Action current state (disabled, idle, running or cooldown) </summary>
        public ActionState currentState
        {
            get => ActionCurrentState;
            private set
            {
                // Debug.Log($"{ActionName} - CurrentState - From: {ActionCurrentState} To: {value}");
                ActionCurrentState = value;
                if (!m_BehaviourIsModule) return;
                m_Module.UpdateState();
            }
        }


        /// <summary> Action is ready and can be triggered </summary>
        public bool isIdle => currentState == ActionState.Idle;

        /// <summary> Action has started and is preparing to run </summary>
        public bool inStartDelay => currentState == ActionState.InStartDelay;

        /// <summary> Action has started and is running </summary>
        public bool isRunning => currentState == ActionState.IsRunning;

        /// <summary> Action is in the 'InCooldown' state and cannot be triggered again during this time </summary>
        public bool inCooldown => currentState == ActionState.InCooldown;

        /// <summary> Action has started running and is either in 'IsStartDelay', 'IsRunning' or 'InCooldown' state </summary>
        public bool isActive => inStartDelay || isRunning || inCooldown;

        #endregion

        #region Action Enabled

        /// <summary> If TRUE the Action can run, FALSE otherwise </summary>
        [SerializeField] private bool ActionEnabled;

        /// <summary> If TRUE the Action can run, FALSE otherwise </summary>
        public bool enabled
        {
            get => ActionEnabled;
            set
            {
                if (value)
                {
                    OnActivate();
                    ActionEnabled = true;
                    currentState = ActionState.Idle;
                    onStateChanged?.Invoke(TriggeredActionState.Idle);
                    return;
                }

                OnDeactivate();
                ActionEnabled = false;
                currentState = ActionState.Disabled;
                onStateChanged?.Invoke(TriggeredActionState.Disabled);
            }
        }

        #endregion

        #region Action Start Delay

        /// <summary> Time interval before the Action executes its task, after it started running </summary>
        [SerializeField] private float ActionStartDelay;

        /// <summary> Time interval before the Action executes its task, after it started running </summary>
        public float startDelay
        {
            get => ActionStartDelay > 0 ? ActionStartDelay : 0;
            internal set => ActionStartDelay = value > 0 ? value : 0;
        }

        #endregion

        #region Action Duration

        /// <summary>
        /// Running time from start to finish Does not include StartDelay.
        /// <para/> At 0 (zero) the Action's task happens instantly.
        /// </summary>
        [SerializeField] private float ActionDuration;

        /// <summary>
        /// Running time from start to finish Does not include StartDelay.
        /// <para/> At 0 (zero) the Action's task happens instantly.
        /// </summary>
        public float duration
        {
            get => ActionDuration > 0 ? ActionDuration : 0;
            internal set => ActionDuration = value > 0 ? value : 0;
        }

        #endregion

        #region Action Total Duration

        /// <summary>
        /// Total running time for the Action Cooldown is not taken into account.
        /// <para/> StartDelay + Duration 
        /// </summary>
        public float totalDuration => startDelay + duration;

        #endregion

        #region Action Cooldown

        /// <summary> Cooldown time after the Action ran. During this time, the Action cannot Start running again </summary>
        [SerializeField] private float ActionCooldown;

        /// <summary> Cooldown time after the Action ran. During this time, the Action cannot Start running again </summary>
        public float cooldown
        {
            get => ActionCooldown > 0 ? ActionCooldown : 0;
            internal set => ActionCooldown = value > 0 ? value : 0;
        }

        #endregion

        #region Action Timescale Independent

        /// <summary>
        /// Determine if the Action's timers will be affected by the application's timescale
        /// <para/> Timescale is the scale at which time passes 
        /// <para/> Timescale.Independent - (Realtime) Not affected by the application's timescale value
        /// <para/> Timescale.Dependent - (Application Time) Affected by the application's timescale value
        /// </summary>
        [SerializeField] private Timescale ActionTimescale;

        /// <summary>
        /// Determine if the Action's timers will be affected by the application's timescale
        /// <para/> Timescale is the scale at which time passes 
        /// <para/> TRUE - Timescale.Independent - (Realtime) Not affected by the application's timescale value
        /// <para/> FALSE - Timescale.Dependent - (Application Time) Affected by the application's timescale value
        /// </summary>
        public bool isTimescaleIndependent
        {
            get => ActionTimescale == Timescale.Independent;
            internal set => ActionTimescale = value ? Timescale.Independent : Timescale.Dependent;
        }

        #endregion

        #region Action OnStart Stop Other Actions 

        /// <summary> Stop for all other Actions, on the Module (MonoBehaviour), when this Action starts running </summary>
        [SerializeField] private bool ActionOnStartStopOtherActions;

        /// <summary> Stop for all other Actions, on this Module (MonoBehaviour), when this Action starts running </summary>
        public bool onStartStopOtherActions
        {
            get => ActionOnStartStopOtherActions;
            internal set => ActionOnStartStopOtherActions = value;
        }

        #endregion

        #region Action OnStart Callback

        /// <summary> Events triggered when this Action starts running </summary>
        [SerializeField] private ModyEvent OnStartEvents;

        /// <summary> Events triggered when this Action starts running </summary>
        public ModyEvent onStartEvents => OnStartEvents;

        #endregion

        #region Action OnFinish Callback

        /// <summary> Events triggered when this Action finished running </summary>
        [SerializeField] private ModyEvent OnFinishEvents;

        /// <summary> Events triggered when this Action finished running </summary>
        public ModyEvent onFinishEvents => OnFinishEvents;

        #endregion

        #region Coroutines

        private Coroutine m_RunCoroutine;

        private Coroutine m_CooldownCoroutine;

        #endregion

        #region TriggeredActionState

        public enum TriggeredActionState
        {
            Disabled,
            Idle,
            StartDelay,
            OnStart,
            Run,
            OnFinish,
            Cooldown
        }

        public UnityAction<TriggeredActionState> onStateChanged { get; set; }

        #endregion

        
        private bool m_BehaviourIsModule;
        private ModyModule m_Module;

        public bool HasValue;
        public Type ValueType;
        public bool IgnoreSignalValue;
        public bool ReactToAnySignal;

        //ToDo - add IgnoreSignalValue to editor options
        //ToDo - add ReactToAnySignal to editor options
        
        protected ModyAction(MonoBehaviour behaviour, string actionName)
        {
            actionBehaviourReference = behaviour;

            ActionName = actionName.RemoveWhitespaces().RemoveAllSpecialCharacters();

            currentState = ActionState.Disabled;
            ActionEnabled = false;

            ActionStartDelay = 0;
            ActionDuration = 0;
            ActionCooldown = 0;

            ActionTimescale = Timescale.Independent;
            ActionOnStartStopOtherActions = true;

            OnStartEvents = new ModyEvent("OnStart");
            OnFinishEvents = new ModyEvent("OnFinish");

            HasValue = false;
            ValueType = null;
            IgnoreSignalValue = true;
            ReactToAnySignal = true;
        }

        protected override void OnSignal(Signal signal)
        {
            StartRunning(signal);
        }

        /// <summary>
        /// OnActivate should be called before this Action is used to perform its initial setup.
        /// <para/> It is called automatically when the Action's Enabled state changes to TRUE.
        /// <para/> This method should be called in the OnEnable method of the controlling MonoBehaviour (Module).
        /// </summary>
        public virtual void OnActivate()
        {
            if (!enabled) return;
            Validate();
            ConnectReceivers();
            StopRunning();
            StopCooldown();
            currentState = ActionState.Idle;
            onStateChanged?.Invoke(TriggeredActionState.Idle);
        }

        /// <summary>
        /// OnDeactivate should be called to clean up the Action after it has been used / activated.
        /// <para/> It is called automatically when the Action's Enabled state changes to FALSE.
        /// <para/> This method should be called in the OnDisable method of the controlling MonoBehaviour (Module).
        /// </summary>
        public virtual void OnDeactivate()
        {
            DisconnectReceivers();
            StopRunning();
            StopCooldown();
        }

        /// <summary>
        /// Validate the Action's settings
        /// </summary>
        public virtual void Validate()
        {
            UpdateSignalReceivers();
        }

        /// <summary>
        /// Start running this Action.
        /// <para/> The Action needs to have Enabled set to TRUE for this method to work.
        /// <para/> If the Action is in the 'InCooldown' state, this method will NOT work.
        /// </summary>
        public void StartRunning() =>
            StartRunning(null, false);

        /// <summary>
        /// Start running this Action.
        /// <para/> The Action needs to have Enabled set to TRUE for this method to work.
        /// </summary>
        /// <param name="ignoreCooldown"> Ignore cooldown if the Action is in the 'InCooldown' state </param>
        public void StartRunning(bool ignoreCooldown) =>
            StartRunning(null, ignoreCooldown);

        /// <summary>
        /// Start running this Action.
        /// <para/> The Action needs to have Enabled set to TRUE for this method to work.
        /// <para/> If the Action is in the 'InCooldown' state, this method will NOT work.
        /// </summary>
        /// <param name="signal"> Signal used to pass data </param>
        public void StartRunning(Signal signal) =>
            StartRunning(signal, false);

        /// <summary>
        /// Start running this Action.
        /// <para/> The Action needs to have Enabled set to TRUE for this method to work.
        /// </summary>
        /// <param name="signal"> Signal used to pass data </param>
        /// <param name="ignoreCooldown"> Ignore cooldown if the Action is in the 'InCooldown' state </param>
        /// <param name="forced"> Execute method even if the Action is not enabled </param>
        public void StartRunning(Signal signal, bool ignoreCooldown, bool forced = false)
        {
            if (!forced && !enabled) return;

            if (onStartStopOtherActions)
            {
                StopAllOtherActions();
            }

            if (currentState == ActionState.InCooldown)
            {
                if (!ignoreCooldown)
                {
                    return;
                }

                StopCooldown();
            }

            if (currentState == ActionState.IsRunning)
            {
                StopRunning();
            }

            if (totalDuration == 0)
            {
                if (onStartEvents is { Enabled: true })
                {
                    onStartEvents.Execute();
                    onStateChanged?.Invoke(TriggeredActionState.OnStart);
                }
                currentState = ActionState.IsRunning;
                onStateChanged?.Invoke(TriggeredActionState.Run);
                onStartEvents?.Execute();
                Run(signal);
                FinishRunning();
                return;
            }

            m_RunCoroutine = actionBehaviourReference.StartCoroutine(ExecuteRun(signal));
        }

        /// <summary>
        /// Executes the run cycle as follows:
        /// <para/> >> (time interval) StartDelay
        /// <para/> >> (custom method) Run
        /// <para/> >> (time interval) Duration
        /// <para/> >> (method) FinishRunning 
        /// </summary>
        /// <param name="signal"> Signal used to pass data </param>
        protected IEnumerator ExecuteRun(Signal signal)
        {
            if (ActionStartDelay > 0)
            {
                currentState = ActionState.InStartDelay;
                onStateChanged?.Invoke(TriggeredActionState.StartDelay);

                if (isTimescaleIndependent)
                {
                    yield return new WaitForSecondsRealtime(startDelay);
                }
                else
                {
                    yield return new WaitForSeconds(startDelay);
                }
            }

            if (onStartEvents is { Enabled: true })
            {
                onStartEvents.Execute();
                onStateChanged?.Invoke(TriggeredActionState.OnStart);
            }

            currentState = ActionState.IsRunning;
            onStateChanged?.Invoke(TriggeredActionState.Run);

            Run(signal);

            if (duration > 0)
            {
                if (isTimescaleIndependent)
                {
                    yield return new WaitForSecondsRealtime(duration);
                }
                else
                {
                    yield return new WaitForSeconds(duration);
                }
            }

            FinishRunning();
        }

        /// <summary>
        /// Stops running this Action.
        /// <para/> The Action needs to be in the 'IsRunning' state for this method to work.
        /// <para/> If the Action is in the 'InCooldown' state, this method does NOT reset or stop the cooldown timer.
        /// <para/> This method does NOT execute the Finisher.
        /// </summary>
        public void StopRunning()
        {
            if (m_RunCoroutine != null)
            {
                actionBehaviourReference.StopCoroutine(m_RunCoroutine);
                m_RunCoroutine = null;
            }

            switch (currentState)
            {
                case ActionState.Disabled:
                case ActionState.InCooldown:
                    return;
                default:
                    currentState = ActionState.Idle;
                    // onStateChanged?.Invoke(TriggeredActionState.Idle);
                    break;
            }
        }

        /// <summary>
        /// Start the Action's cooldown timer, that makes it unable to start running again until the timer finishes.
        /// <para/> The Action needs to have Enabled set to TRUE for this method to work.
        /// <para/> If the Action is in the 'IsRunning' state, this method will also Stop the Action from running.
        /// <para/> If the Action is in the 'Disabled' state, this method will NOT do anything.
        /// <para/> If the Action is in the 'InCooldown' state, this method will restart the cooldown timer.
        /// </summary>
        public void StartCooldown()
        {
            if (!enabled) return;

            if (currentState == ActionState.IsRunning)
            {
                StopRunning();
            }

            if (currentState == ActionState.Disabled)
            {
                return;
            }

            if (cooldown == 0)
            {
                currentState = ActionState.Idle;
                onStateChanged?.Invoke(TriggeredActionState.Idle);
                return;
            }

            if (currentState == ActionState.InCooldown)
            {
                StopCooldown();
            }

            m_CooldownCoroutine = actionBehaviourReference.StartCoroutine(ExecuteCooldown());
        }

        /// <summary>
        /// Executes the cooldown cycle as follows:
        /// <para/> >> (time interval) Cooldown
        /// <para/> >> (method) StopCooldown
        /// </summary>
        protected IEnumerator ExecuteCooldown()
        {
            currentState = ActionState.InCooldown;
            onStateChanged?.Invoke(TriggeredActionState.Cooldown);

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
        /// Stop the Action's cooldown timer and set it ready to start running again.
        /// </summary>
        public void StopCooldown()
        {
            if (m_CooldownCoroutine != null)
            {
                actionBehaviourReference.StopCoroutine(m_CooldownCoroutine);
                m_CooldownCoroutine = null;
            }

            currentState = ActionState.Idle;
            onStateChanged?.Invoke(TriggeredActionState.Idle);
        }

        /// <summary>
        /// Finish running the Action by doing the following:
        /// <para/> >> (method) Execute Finisher (if enabled)
        /// <para/> >> (method) StartCooldown 
        /// <para/> The Action needs to have Enabled set to TRUE for this method to work.
        /// </summary>
        public void FinishRunning()
        {
            if (!isActive && !enabled) return;

            if (onFinishEvents is { Enabled: true })
            {
                onFinishEvents.Execute();
                onStateChanged?.Invoke(TriggeredActionState.OnFinish);
            }

            if (cooldown > 0)
            {
                StartCooldown();
                return;
            }

            currentState = ActionState.Idle;
            onStateChanged?.Invoke(TriggeredActionState.Idle);
        }

        /// <summary>
        /// Execute the given method The available options are as follows:
        /// <para/> 1 StartRunning
        /// <para/> 2 StopRunning
        /// <para/> 3 FinishRunning
        /// </summary>
        /// <param name="method"> Method to call </param>
        /// <param name="ignoreCooldown"> Ignore cooldown if the Action is in the 'InCooldown' state (only for the StartRunning option) </param>
        /// <param name="forced"> Execute method even if the Action is not enabled </param>
        public void ExecuteMethod(RunAction method, bool ignoreCooldown = false, bool forced = false)
        {
            switch (method)
            {
                case RunAction.Start:
                    StartRunning(null, ignoreCooldown, forced);
                    break;
                case RunAction.Stop:
                    StopRunning();
                    break;
                case RunAction.Finish:
                    FinishRunning();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
        }

        /// <summary>
        /// Stop all the other Actions that are running on the controlling MonoBehaviour (Module)
        /// </summary>
        public void StopAllOtherActions()
        {
            if (!enabled) return;
            ((IHaveActions)actionBehaviourReference)?.StopAllActions();
        }

        /// <summary>
        /// Run the code that executes the task this Action was designed to do.
        /// <para/> This method is called after the Action has started running and the StartDelay time interval has passed.
        /// <para/> This is where the code that 'does things' is added in derived classes.
        /// </summary>
        /// <param name="signal"> Signal used to pass data </param>
        protected abstract void Run(Signal signal);

        /// <summary> Try to set a value to the MetaSignal. Returns TRUE if the operation was successful </summary>
        /// <param name="objectValue"> Value </param>
        public abstract bool SetValue(object objectValue);
        
        /// <summary> Try to set a value to the MetaSignal. Returns TRUE if the operation was successful </summary>
        /// <param name="objectValue"> Value </param>
        /// <param name="restrictValueType"> Check if the passed object type is the same as the action's ValueType </param>
        internal abstract bool SetValue(object objectValue, bool restrictValueType);
        
        /// <summary>
        /// Update all the signal receivers references.
        /// </summary>
        private void UpdateSignalReceivers()
        {
            foreach (SignalReceiver receiver in SignalsReceivers)
            {
                switch (receiver.streamConnection)
                {
                    case StreamConnection.None:
                        receiver.SetSignalSource(actionBehaviourReference.gameObject);
                        break;
                    case StreamConnection.ProviderId:
                        receiver.SetSignalSource
                        (
                            receiver.providerId.Type == ProviderType.Local
                                ? actionBehaviourReference.gameObject
                                : Signals.Signals.instance.gameObject
                        );
                        break;
                    case StreamConnection.ProviderReference:
                        if (receiver.providerReference != null)
                            receiver.SetSignalSource(receiver.providerReference.gameObject);
                        break;
                    case StreamConnection.StreamId:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    public static class ModyActionExtensions
    {
        /// <summary>
        /// Set the Action's Enabled state.
        /// </summary>
        /// <param name="target"> Target Action </param>
        /// <param name="value"> Is enabled </param>
        public static T SetEnabled<T>(this T target, bool value) where T : ModyAction
        {
            target.enabled = value;
            return target;
        }

        /// <summary>
        /// Set the MonoBehaviour that controls the Action.
        /// </summary>
        /// <param name="target"> Target Action </param>
        /// <param name="behaviour">
        /// The MonoBehaviour (Module) that this Action belongs to.
        /// <para/> Needs to implement the IHaveActions interface.
        /// </param>
        public static T SetBehaviour<T>(this T target, MonoBehaviour behaviour) where T : ModyAction
        {
            target.actionBehaviourReference = behaviour;
            return target;
        }

        /// <summary>
        /// Set if when the Action starts running, all the other Actions on the Module (MonoBehaviour controller) are stopped.
        /// </summary>
        /// <param name="target"> Target Action </param>
        /// <param name="value"> Stop other Actions when this Action starts running </param>
        public static T SetStopAllActionsOnStart<T>(this T target, bool value) where T : ModyAction
        {
            target.onStartStopOtherActions = value;
            return target;
        }

        /// <summary>
        /// Set the Action's StartDelay.
        /// </summary>
        /// <param name="target"> Target Action </param>
        /// <param name="value"> Time interval before the Action executes its task, after it started running </param>
        public static T SetStartDelay<T>(this T target, float value) where T : ModyAction
        {
            target.startDelay = value;
            return target;
        }

        /// <summary>
        /// Set the Action's Duration.
        /// </summary>
        /// <param name="target"> Target Action </param>
        /// <param name="value">
        /// Running time from start to finish Does not include StartDelay.
        /// <para/> At 0 (zero) the Action's task happens instantly.
        /// </param>
        public static T SetDuration<T>(this T target, float value) where T : ModyAction
        {
            target.duration = value;
            return target;
        }

        /// <summary>
        /// Set the Action's Cooldown.
        /// </summary>
        /// <param name="target"> Target Action </param>
        /// <param name="value"> Cooldown time after the Action ran During this time, the Action cannot Start running again </param>
        public static T SetCooldown<T>(this T target, float value) where T : ModyAction
        {
            target.cooldown = value;
            return target;
        }

        /// <summary>
        /// Set how TimeScale influences the Action's timers.
        /// </summary>
        /// <param name="target"> Target Action </param>
        /// <param name="value">
        /// Determine if the Action's timers will be Timescale independent and thus not affected by the scale at which time passes
        /// <para/> TRUE - Timescale independent
        /// <para/> FALSE - Timescale dependent (affected by Time settings)
        /// </param>
        public static T SetTimescaleIndependent<T>(this T target, bool value) where T : ModyAction
        {
            target.isTimescaleIndependent = value;
            return target;
        }
        }
}
