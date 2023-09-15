using UnityEngine;
using UnityEngine.EventSystems;

namespace Michsky.UI.Shift
{
    public class VirtualCursorAnimate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Resources")]
        public VirtualCursor virtualCursor;

        void Start()
        {
            if (virtualCursor == null)
            {
                try
                {
                    var vCursor = (VirtualCursor)GameObject.FindObjectsOfType(typeof(VirtualCursor))[0];
                    virtualCursor = vCursor;
                }

                catch { this.enabled = false; }         
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (virtualCursor != null)
                virtualCursor.AnimateCursorIn();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (virtualCursor != null)
                virtualCursor.AnimateCursorOut();
        }
    }
}