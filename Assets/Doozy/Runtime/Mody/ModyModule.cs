// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Attributes;
using UnityEngine;

namespace Doozy.Runtime.Mody
{
    /// <summary>
    /// Base Module class for the Mody System.
    /// <para/> Any module that interacts with the system needs to derive from this.
    /// <para/> It can perform various tasks and uses Actions to trigger them.
    /// </summary>
    [Serializable]
    public abstract class ModyModule : MonoBehaviour, IHaveActions
    {
        #region Module Name

        /// <summary>
        /// Name of the Module
        /// </summary>
        [SerializeField] private string ModuleName;

        /// <summary>
        /// Name of the Module
        /// </summary>
        public string moduleName
        {
            get => ModuleName;
            internal set => ModuleName = value;
        }

        #endregion

        #region Module Actions

        /// <summary>
        /// Collection of Actions available for this Module
        /// </summary>
        private HashSet<ModyAction> m_ModuleActions;

        /// <summary>
        /// Collection of Actions available for this Module
        /// </summary>
        public HashSet<ModyAction> actions
        {
            get => m_ModuleActions ?? (m_ModuleActions = new HashSet<ModyAction>());
            protected internal set => m_ModuleActions = value;
        }

        /// <summary>
        /// Names of all the Actions available for this Module
        /// </summary>
        public IEnumerable<string> actionNames => actions.Select(a => a.actionName);

        #endregion

        #region Module States

        /// <summary>
        /// Module current state (disabled, idle or active)
        /// </summary>
        [SerializeField] private ModuleState ModuleCurrentState = ModuleState.Disabled;

        /// <summary>
        /// Module current state (disabled, idle or active)
        /// </summary>
        public ModuleState state
        {
            get => ModuleCurrentState;
            internal set => ModuleCurrentState = value;
        }

        #endregion

        /// <summary>
        /// Returns TRUE if this Module has been initialized
        /// </summary>
        public bool initialized { get; protected set; }

        protected ModyModule(string moduleName)
        {
            ModuleName = moduleName;
            // ReSharper disable once VirtualMemberCallInConstructor
            SetupActions();
        }

        /// <summary>
        /// Initialize the Module
        /// </summary>
        public virtual void Initialize()
        {
            if (initialized)
                return;

            SetupActions();
            initialized = true;
        }

        /// <summary>
        /// Update the current state of the Module
        /// </summary>
        public void UpdateState()
        {
            state = ModuleState.Disabled;

            if (actions.Any(action => action.isActive))
            {
                state = ModuleState.Active;
                return;
            }

            if (actions.Any(action => action.isIdle))
            {
                state = ModuleState.Idle;
            }
        }

        /// <summary>
        /// Activate all the Actions, this Module has, by calling OnActivate for each Action.
        /// <para/> This method is automatically called by the Module in its OnEnable method.
        /// </summary>
        public void ActivateActions()
        {
            foreach (ModyAction action in actions)
                action.OnActivate();
        }

        /// <summary>
        /// Deactivate all the Actions, this Module has, by calling OnDeactivate for each Action.
        /// <para/> This method is automatically called by the Module in its OnDisable method.
        /// </summary>
        public void DeactivateActions()
        {
            foreach (ModyAction action in actions)
                action.OnDeactivate();
        }

        /// <summary>
        /// Start running the Action with the given action name.
        /// </summary>
        /// <param name="actionName"> Name of the Action </param>
        /// <param name="ignoreCooldown"> Ignore cooldown if the Action is in the 'InCooldown' state </param>
        /// <param name="forced"> Execute method even if the Action is not enabled </param>
        public void StartAction(string actionName, bool ignoreCooldown, bool forced = false)
        {
            foreach (ModyAction action in actions.Where(action => action.actionName.Equals(actionName)))
                action.StartRunning(null, ignoreCooldown, forced);
        }

        /// <summary>
        /// Stop running the Action with the given action name.
        /// <para/> The Action needs to be in the 'IsRunning' state for it to stop.
        /// <para/> If the Action is in the 'InCooldown' state, this method does NOT reset or stop its cooldown timer.
        /// <para/> This method does NOT execute the Finisher.
        /// </summary>
        /// <param name="actionName"> Name of the Action </param>
        public void StopAction(string actionName)
        {
            foreach (ModyAction action in actions.Where(action => action.actionName.Equals(actionName)))
                action.StopRunning();
        }

        /// <summary>
        /// Stop running all the Actions this Module has.
        /// <para/> The Action needs to be in the 'IsRunning' state for it to stop.
        /// <para/> If the Action is in the 'InCooldown' state, this method does NOT reset or stop its cooldown timer.
        /// <para/> This method does NOT execute the Finisher.
        /// </summary>
        public void StopAllActions()
        {
            foreach (ModyAction action in actions)
                action.StopRunning();
        }

        /// <summary>
        /// Finish running the Action with the given name, by doing the following:
        /// <para/> >> (method) StartCooldown 
        /// <para/> >> (method) Execute Finisher (if enabled)
        /// <para/> The Action needs to have Enabled set to TRUE for this method to work.
        /// </summary>
        /// <param name="actionName"> Name of the Action </param>
        public void FinishAction(string actionName)
        {
            foreach (ModyAction action in actions.Where(action => action.actionName.Equals(actionName)))
                action.FinishRunning();
        }

        /// <summary>
        /// Finish running all the Actions this Module has, by doing the following for each Action:
        /// <para/> >> (method) StartCooldown 
        /// <para/> >> (method) Execute Finisher (if enabled)
        /// <para/> The Action needs to have Enabled set to TRUE for this method to work.
        /// </summary>
        public void FinishAllActions()
        {
            foreach (ModyAction action in actions)
                action.FinishRunning();
        }

        /// <summary>
        /// Execute the given method on the Action with the given name. The available options are as follows:
        /// <para/> 1. StartRunning
        /// <para/> 2. StopRunning
        /// <para/> 3. FinishRunning
        /// </summary>
        /// <param name="actionName"> Name of the Action </param>
        /// <param name="method"> Method to call </param>
        /// <param name="ignoreCooldown"> Ignore cooldown if the Action is in the 'InCooldown' state (only for the StartRunning option) </param>
        /// <param name="forced"> Execute method even if the Action is not enabled </param>
        public void Execute(string actionName, RunAction method, bool ignoreCooldown = false, bool forced = false)
        {
            ModyAction action = GetAction(actionName);
            action?.ExecuteMethod(method, ignoreCooldown, forced);
        }

        /// <summary>
        /// Get the Action with the given name from the Module (if it exists)
        /// </summary>
        /// <param name="actionName"> Name of the Action </param>
        public ModyAction GetAction(string actionName) =>
            actions.FirstOrDefault(action => action.actionName.Equals(actionName));

        /// <summary>
        /// Returns TRUE if this Module has an Action with the given action name.
        /// </summary>
        /// <param name="actionName"> Name of the Action </param>
        public bool ContainsAction(string actionName) =>
            actions.Any(action => action.actionName.Equals(actionName));

        /// <summary>
        /// Setup Actions for this Module.
        /// <para/> This method is called in the constructor, in Initialize and in OnEnable.
        /// <para/> This is where the code that 'initializes' and adds the Actions for this Module is added.
        /// </summary>
        protected abstract void SetupActions();

        /// <summary>
        /// Validate the Module's settings
        /// </summary>
        public virtual void Validate()
        {
            foreach (ModyAction action in actions)
            {
                action.SetBehaviour(this);
                action.Validate();
            }
        }

        #region Unity Methods

        protected virtual void Awake()
        {
            RegisterToDatabase();
        }

        protected virtual void Start()
        {
            Initialize();
        }

        protected virtual void OnEnable()
        {
            Validate();
            SetupActions();
            actions.Remove(null);
            ActivateActions();
        }

        protected virtual void OnDisable()
        {
            actions.Remove(null);
            DeactivateActions();
        }

        protected virtual void OnDestroy()
        {
            UnregisterFromDatabase();
        }

        #endregion

        #region Database

        /// <summary>
        /// Database that contains all the Modules that are available at runtime at any given point in time.
        /// </summary>
        [ClearOnReload(newInstance: true)]
        private static readonly ListDatabase<GameObject, ModyModule> Database = new ListDatabase<GameObject, ModyModule>();

        /// <summary>
        /// Returns a list of all the Modules available on a target GameObject
        /// </summary>
        /// <param name="target"> Target GameObject </param>
        public static List<ModyModule> GetModules(GameObject target) =>
            Database.GetValues(target);

        /// <summary>
        /// Register this Module to the Database.
        /// <para/> This method is automatically called in Awake.
        /// </summary>
        private void RegisterToDatabase() =>
            Database.Add(gameObject, this);

        /// <summary>
        /// Unregister this Module from the Database.
        /// <para/> This method is automatically called OnDestroy.
        /// </summary>
        private void UnregisterFromDatabase() =>
            Database.Remove(gameObject, this);

        #endregion
    }

    public static class ModyModuleExtensions
    {
        /// <summary>
        /// Set Module Name
        /// </summary>
        /// <param name="target"> Target Module </param>
        /// <param name="value"> Name of the Module </param>
        public static T SetName<T>(this T target, string value) where T : ModyModule
        {
            target.moduleName = value;
            return target;
        }

        /// <summary>
        /// Add the given Action to this Module
        /// </summary>
        /// <param name="target"> Target Module </param>
        /// <param name="action"> Action </param>
        public static T AddAction<T>(this T target, ModyAction action) where T : ModyModule
        {
            target.actions.Add(action);
            return target;
        }

        /// <summary>
        /// Remove the given Action from this Module
        /// </summary>
        /// <param name="target"> Target Module </param>
        /// <param name="action"> Action </param>
        public static T RemoveAction<T>(this T target, ModyAction action) where T : ModyModule
        {
            target.actions.Remove(action);
            return target;
        }

        /// <summary>
        /// Remove all the Actions, with the given action name, from the Module
        /// </summary>
        /// <param name="target"> Target Module </param>
        /// <param name="actionName"> Name of the Action </param>
        public static T RemoveAction<T>(this T target, string actionName) where T : ModyModule
        {
            foreach (ModyAction action in target.actions.ToList())
            {
                if (!actionName.Equals(action.actionName)) continue;
                target.actions.Remove(action);
            }

            return target;
        }
    }
}
