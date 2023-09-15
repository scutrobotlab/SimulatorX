// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections;
using Doozy.Runtime.Signals;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Doozy.Runtime.UIManager.Triggers
{
    [RequireComponent(typeof(GraphicRaycaster))]
    [AddComponentMenu("Doozy/UI/Triggers/Pointer/Pointer Long Click")]
    public class PointerLongClickTrigger : SignalProvider, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        private const float LONG_CLICK_REGISTER_INTERVAL = 0.5f;

        private float m_LongClickTriggerTime;
        private Coroutine run { get; set; }
        
        public PointerLongClickTrigger() : base(ProviderType.Local, "Pointer", "Long Click", typeof(PointerLongClickTrigger))
        {
            Reset();
        }

        public void Reset()
        {
            if (run != null)
            {
                StopCoroutine(run);
                run = null;
            }
            
            m_LongClickTriggerTime = 0;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            Reset();
            m_LongClickTriggerTime = Time.unscaledTime + LONG_CLICK_REGISTER_INTERVAL;
            run = StartCoroutine(Run(eventData));
        }
        
        public void OnPointerUp(PointerEventData eventData) =>
            Reset();

        public void OnPointerExit(PointerEventData eventData) =>
            Reset();

        private IEnumerator Run(PointerEventData eventData)
        {
            while (m_LongClickTriggerTime > Time.unscaledTime)
                yield return null;

            if (UISettings.interactionsDisabled) yield break;
            SendSignal(eventData);
            Reset();
        }
    }
}
