using UnityEngine;
using UnityEngine.Events;

namespace Michsky.UI.Shift
{
    public class HoldKeyEvent : MonoBehaviour
    {
        [Header("Key")]
        [SerializeField]
        public KeyCode hotkey;

        [Header("Action")]
        [SerializeField]
        public UnityEvent holdAction;
        [SerializeField]
        public UnityEvent releaseAction;

        private bool isOn = false;
        private bool isHolding = false;

        void Update()
        {
            if (Input.GetKey(hotkey))
            {
                isHolding = true;
                isOn = false;
            }

            else
            {
                isHolding = false;
                isOn = true;
            }

            if (isOn == true && isHolding == false)
            {
                releaseAction.Invoke();
                isHolding = false;
                isOn = false;
            }

            else if (isOn == false && isHolding == true)
            {
                holdAction.Invoke();
                isHolding = true;
            }
        }
    }
}