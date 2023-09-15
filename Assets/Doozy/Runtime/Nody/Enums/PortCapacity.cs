// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Runtime.Nody
{
    /// <summary> Specify how many connections a port can have </summary>
    public enum PortCapacity
    {
        /// <summary> Port can only have a single connection </summary>
        Single,

        /// <summary> Port can have multiple connections </summary>
        Multi,
    }
}
