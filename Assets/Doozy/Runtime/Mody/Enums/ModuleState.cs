// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Runtime.Mody
{
	/// <summary> Defines all the states a Module can be in </summary>
	public enum ModuleState
	{
		/// <summary> Module is disabled - all of its actions are disabled </summary>
		Disabled = 10,
			
		/// <summary> Module is ready - has at least one idle action and no other action is running or in cooldown </summary>
		Idle = 20,
		
		/// <summary> Module is active - has at least one action running or in cooldown </summary>
		Active = 30,
	}
}