// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Common.Events
{
    /// <summary> UnityEvent with a Quaternion parameter </summary>
    [Serializable]
    public class QuaternionEvent : UnityEvent<Quaternion>
    {
    }
}
