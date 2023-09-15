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
    [AddComponentMenu("Doozy/UI/Triggers/Pointer/Pointer Double Click")]
    public class PointerDoubleClickTrigger : SignalProvider, IPointerDownHandler, IPointerUpHandler
    {
        private const float DOUBLE_CLICK_REGISTER_INTERVAL = 0.2f;
        
        private bool m_ClickedOnce;
        private float m_ClickTime;

        public PointerDoubleClickTrigger() : base(ProviderType.Local, "Pointer", "Double Click", typeof(PointerDoubleClickTrigger))
        {
            Reset();
        }

        public void Reset()
        {
            m_ClickedOnce = false;
            m_ClickTime = 0;
        }

        public void OnPointerDown(PointerEventData eventData) {}

        public void OnPointerUp(PointerEventData eventData)
        {
            if (m_ClickedOnce == false)
            {
                // Debug.Log($"clicked once");
                m_ClickedOnce = true;
                m_ClickTime = Time.unscaledTime;
                return;
            }

            if (Time.unscaledTime - m_ClickTime > DOUBLE_CLICK_REGISTER_INTERVAL) //interval passed
            {
                // Debug.Log($"clicked once - after interval passed");
                m_ClickedOnce = true;
                m_ClickTime = Time.unscaledTime;
                return;
            }

            Reset();
            if(UISettings.interactionsDisabled) return;
            SendSignal(eventData);
            // Debug.Log($"clicked twice - SIGNAL");
        }
    }
}
