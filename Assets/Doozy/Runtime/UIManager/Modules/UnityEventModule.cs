// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Mody.Actions;
using UnityEngine;
using UnityEngine.Events;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Modules
{
    [AddComponentMenu("Doozy/UI/Modules/UnityEvent Module")]
    public class UnityEventModule : ModyModule
    {
        public const string k_DefaultModuleName = "UnityEvent";

        public UnityEvent Event;

        public SimpleModyAction InvokeEvent;

        public UnityEventModule() : this(k_DefaultModuleName) {}

        public UnityEventModule(string moduleName) : base(moduleName.IsNullOrEmpty() ? k_DefaultModuleName : moduleName) {}

        protected override void SetupActions()
        {
            this.AddAction(InvokeEvent ??= new SimpleModyAction(this, nameof(InvokeEvent), ExecuteInvokeEvent));
        }

        public void ExecuteInvokeEvent()
        {
            Event?.Invoke();
        }
    }
}
