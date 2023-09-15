// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Runtime.UIManager
{
    /// <summary> Defines how a Container can behave without any outside input </summary>
    public enum ContainerBehaviour
    {
        /// <summary> Do nothing </summary>
        Disabled,

        /// <summary> Instant Hide (no animation) </summary>
        InstantHide,

        /// <summary> Instant Show (no animation) </summary>
        InstantShow,

        /// <summary> Hide (animated) </summary>
        Hide,

        /// <summary> Show (animated) </summary>
        Show
    }
}
