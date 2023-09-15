// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody.Actions
{
    [Serializable]
    public class ColorModyAction : MetaModyAction<Color>
    {
        public ColorModyAction(MonoBehaviour behaviour, string actionName, UnityAction<Color> callback)
            : base(behaviour, actionName, callback)
        {
        }
    }
}
