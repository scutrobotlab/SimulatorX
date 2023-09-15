using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Michsky.UI.Shift
{
    public class PressKeyEvent : MonoBehaviour
    {
        [Header("Key")] [SerializeField] public Key hotkey;

        public bool pressAnyKey;
        public bool invokeAtStart;

        [Header("Action")] [SerializeField] public UnityEvent pressAction;

        void Start()
        {
            if (invokeAtStart == true)
                pressAction.Invoke();
        }

        void Update()
        {
            if (pressAnyKey == true)
            {
                if ((Keyboard.current?.anyKey.isPressed ?? false)
                    || (Mouse.current?.leftButton.isPressed ?? false)
                    || (Gamepad.current?.leftStick.IsPressed() ?? false))
                    pressAction.Invoke();
            }

            else
            {
                if (Keyboard.current[hotkey].isPressed)
                    pressAction.Invoke();
            }
        }
    }
}