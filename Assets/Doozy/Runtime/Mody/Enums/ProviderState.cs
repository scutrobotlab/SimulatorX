// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Runtime.Mody
{
    /// <summary> Defines all the states a SignalProvider can be in </summary>
    public enum ProviderState
    {
        /// <summary> Provider is disabled </summary>
        Disabled = 10,
        
        /// <summary> Provider is active and ready </summary>
        Idle = 20,
        
        /// <summary> Provider is running </summary>
        IsRunning = 40,
        
        /// <summary> Provider is in cooldown and cannot run </summary>
        InCooldown = 50
    }
}
