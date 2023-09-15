// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Signals;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody.Actions
{
    [Serializable]
    public class SimpleModyAction : ModyAction
    {
        public UnityAction actionCallback { get; private set; }

        public SimpleModyAction(MonoBehaviour behaviour, string actionName, UnityAction callback) : base(behaviour, actionName) =>
            actionCallback = callback;

        protected override void Run(Signal signal) => actionCallback?.Invoke();
        
        public override bool SetValue(object objectValue) => false;
        internal override bool SetValue(object objectValue, bool restrictValueType) => false;
    }
}
