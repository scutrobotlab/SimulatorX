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
    [AddComponentMenu("Doozy/UI/Triggers/Pointer/Pointer Middle Click")]
    public class PointerMiddleClickTrigger : SignalProvider, IPointerClickHandler
    {
        public PointerMiddleClickTrigger() : base(ProviderType.Local, "Pointer", "Middle Click", typeof(PointerMiddleClickTrigger)) {}

        public void OnPointerClick(PointerEventData eventData)
        {
            if (UISettings.interactionsDisabled) return;
            if (eventData.button != PointerEventData.InputButton.Middle) return;
            SendSignal(eventData);
        }
    }
}
