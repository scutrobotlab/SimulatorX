using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Michsky.UI.Shift
{
    public class PointerEnterEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Events")]
        public UnityEvent enterEvent;
        public UnityEvent exitEvent;

        public void OnPointerEnter(PointerEventData eventData)
        {
            enterEvent.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            exitEvent.Invoke();
        }
    }
}