// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

// ReSharper disable UnusedMemberInSuper.Global
namespace Doozy.Runtime.Mody
{
	public interface IHaveActions
	{
		/// <summary> Get the Action with the given Action name. </summary>
		/// <param name="actionName"> Name of the Action to search for </param>
		ModyAction GetAction(string actionName);
		
		/// <summary> Returns TRUE if an Action with the given Action name is found. </summary>
		/// <param name="actionName"> Name of the Action to search for </param>
		bool ContainsAction(string actionName);

		/// <summary> Activate all the Actions. Method usually called OnEnable. </summary>
		void ActivateActions();
		
		/// <summary> Deactivate all the Actions. Method usually called OnDisable. </summary>
		void DeactivateActions();

		/// <summary> Execute a method on an Action with the given Action name. </summary>
		/// <param name="actionName"> Name of the Action </param>
		/// <param name="method"> Method to execute </param>
		/// <param name="ignoreCooldown"> Ignore cooldown if the Action is in the 'InCooldown' state </param>
		/// <param name="forced"> Execute method even if the Action is not enabled </param>
		void Execute(string actionName, RunAction method, bool ignoreCooldown = false, bool forced = false);

		/// <summary> Start running an Action. If the Action is in the 'InCooldown' state and the given ignoreCooldown value is FALSE, the target Action will not start. </summary>
		/// <param name="actionName"> Name of the Action </param>
		/// <param name="ignoreCooldown"> Ignore cooldown if the Action is in the 'InCooldown' state </param>
		/// <param name="forced"> Execute method even if the Action is not enabled </param>
		void StartAction(string actionName, bool ignoreCooldown, bool forced = false);
		
		/// <summary> Stop running an Action. For an Action to stop it has to be in the 'IsRunning' state. </summary>
		/// <param name="actionName"> Name of the Action </param>
		void StopAction(string actionName);
		
		/// <summary> Finishes running an Action (if the Action is in the 'IsRunning' state), by stopping it and then triggering the Finisher (if it has one). </summary>
		/// <param name="actionName"> Name of the Action </param>
		void FinishAction(string actionName);
		
		/// <summary> Stop all the running Actions. For an Action to stop it has to be in the 'IsRunning' state. </summary>
		void StopAllActions();
		
		/// <summary> Finish all the running Actions and trigger the Finisher for each one (if they have one). </summary>
		void FinishAllActions();
	}
}