// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Runtime.Reactor
{
    public enum UIAnimationType
    {
        /// <summary>
        /// Show animation (enter view)
        /// </summary>
        Show = 0,
        
        /// <summary>
        /// Hide animation (exit view)
        /// </summary>
        Hide = 1,
        
        /// <summary>
        /// Loop animation (forces infinite loops by setting the loops counter to -1)
        /// </summary>
        Loop = 2,
        
        /// <summary>
        /// Button animation (click animation)
        /// </summary>
        Button = 3,
        
        /// <summary>
        /// State animation
        /// </summary>
        State = 4,
        
        /// <summary>
        /// Reset animation (set/animate all reactions to their respective start values)
        /// </summary>
        Reset = 5,
        
        /// <summary>
        /// Custom animation, allows for all settings to be used
        /// </summary>
        Custom = 6,
    }
}
