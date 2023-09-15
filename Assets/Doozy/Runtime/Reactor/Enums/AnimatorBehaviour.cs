// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Runtime.Reactor
{
    /// <summary> Defines how an Animator can behave without any outside input </summary>
    public enum AnimatorBehaviour
    {
        /// <summary> Do nothing </summary>
        Disabled,

        /// <summary> Play the animation forward (from 0 to 1) </summary>
        PlayForward,

        /// <summary> Play the animation in reverse (from 1 to 0) </summary>
        PlayReverse,

        /// <summary> Set the animation at 'from' value (at the start value of the animation) </summary>
        SetFromValue,

        /// <summary> Set the animation at 'to' value (at the end value of the animation) </summary>
        SetToValue
    }
}
