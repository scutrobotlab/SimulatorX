using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Shift
{
    public class ScrollGamepadManager : MonoBehaviour
    {
        [Header("Settings")]
        public float changeValue = 0.05f;
        Scrollbar scrollbarObject;

        [Header("Input")]
        public string inputAxis = "Xbox Right Stick Vertical";
        public bool invertAxis = false;

        void Start()
        {
            try { scrollbarObject = gameObject.GetComponent<Scrollbar>(); }
            catch { Debug.LogWarning("Scrollbar is missing. Scrolling via gamepad won't work."); }
        }

        void Update()
        {
            if (scrollbarObject != null)
            {
                float h = Input.GetAxis(inputAxis);

                if (invertAxis == false)
                {
                    if (h == 1)
                        scrollbarObject.value -= changeValue;
                    else if (h == -1)
                        scrollbarObject.value += changeValue;
                }

                else
                {
                    if (h == 1)
                        scrollbarObject.value += changeValue;
                    else if (h == -1)
                        scrollbarObject.value -= changeValue;
                }
            }
        }
    }
}