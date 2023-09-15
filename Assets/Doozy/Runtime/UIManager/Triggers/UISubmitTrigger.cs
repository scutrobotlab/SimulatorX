// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Signals;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Doozy.Runtime.UIManager.Triggers
{
    [RequireComponent(typeof(GraphicRaycaster))]
    [AddComponentMenu("Doozy/UI/Triggers/UI/Submit")]
    public class UISubmitTrigger : SignalProvider, ISubmitHandler
    {
        public UISubmitTrigger() : base(ProviderType.Local, "UI", "Submit", typeof(UISubmitTrigger)) {}

        public void OnSubmit(BaseEventData eventData)
        {
            if (UISettings.interactionsDisabled) return;
            SendSignal(eventData);
        }
    }
}
