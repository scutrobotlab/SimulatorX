// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Runtime.Reactor
{
    /// <summary> Reset options for the value of a Progressor </summary>
    public enum ResetValue
    {
        /// <summary> Value does not reset </summary>
        Disabled,
        
        /// <summary> Value resets to the From value </summary>
        FromValue,
        
        /// <summary> Value resets to the To value </summary>
        EndValue,
        
        /// <summary> Value resets to a Custom value </summary>
        CustomValue
    }
}
