// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Doozy.Runtime.Mody.Actions
{
    [Serializable]
    public class GenericModyAction : MetaModyAction<Object>
    {
        public GenericModyAction(MonoBehaviour behaviour, string actionName, UnityAction<Object> callback)
            : base(behaviour, actionName, callback)
        {
        }
        
        public override bool SetValue(object objectValue) =>
            SetValue(objectValue, false);
    }
}
