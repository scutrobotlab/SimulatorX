using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Infrastructure.Input
{
    /// <summary>
    /// This is an example for how to override the default display behavior of bindings. The component
    /// hooks into <see cref="RebindActionUI.updateBindingUIEvent"/> which is triggered when UI display
    /// of a binding should be refreshed. It then checks whether we have an icon for the current binding
    /// and if so, replaces the default text display with an icon.
    /// </summary>
    public class GamepadIcons : MonoBehaviour
    {
        public GamepadIconsStruct xbox;
        public GamepadIconsStruct ps4;

        protected void OnEnable()
        {
            // Hook into all updateBindingUIEvents on all RebindActionUI components in our hierarchy.
            /*
            var rebindUIComponents = transform.GetComponentsInChildren<RebindActionUI>();
            foreach (var component in rebindUIComponents)
            {
                component.updateBindingUIEvent.AddListener(OnUpdateBindingDisplay);
                component.UpdateBindingDisplay();
            }
            */
            var component = GetComponent<RebindActionUI>();
            component.updateBindingUIEvent.AddListener(OnUpdateBindingDisplay);
            component.UpdateBindingDisplay();
        }

        protected void OnUpdateBindingDisplay(RebindActionUI component, string bindingDisplayString,
            string deviceLayoutName, string controlPath)
        {
            if (string.IsNullOrEmpty(deviceLayoutName) || string.IsNullOrEmpty(controlPath))
                return;

            var icon = default(Sprite);
            if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "DualShockGamepad"))
                icon = ps4.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Gamepad"))
                icon = xbox.GetSprite(controlPath);

            var textComponent = component.bindingText;

            // Grab Image component.
            var imageGO = textComponent.transform.parent.Find("Icon");
            var imageComponent = imageGO.GetComponent<Image>();

            if (icon != null)
            {
                textComponent.gameObject.SetActive(false);
                imageComponent.sprite = icon;
                imageComponent.gameObject.SetActive(true);
            }
            else
            {
                textComponent.gameObject.SetActive(true);
                imageComponent.gameObject.SetActive(false);
            }
        }

        [Serializable]
        public struct GamepadIconsStruct
        {
            public Sprite buttonSouth;
            public Sprite buttonNorth;
            public Sprite buttonEast;
            public Sprite buttonWest;
            public Sprite startButton;
            public Sprite selectButton;
            public Sprite leftTrigger;
            public Sprite rightTrigger;
            public Sprite leftShoulder;
            public Sprite rightShoulder;
            public Sprite dpad;
            public Sprite dpadUp;
            public Sprite dpadDown;
            public Sprite dpadLeft;
            public Sprite dpadRight;
            public Sprite leftStick;
            public Sprite rightStick;
            public Sprite leftStickPress;
            public Sprite rightStickPress;

            public Sprite GetSprite(string controlPath)
            {
                // From the input system, we get the path of the control on device. So we can just
                // map from that to the sprites we have for gamepads.
                return controlPath switch
                {
                    "buttonSouth" => buttonSouth,
                    "buttonNorth" => buttonNorth,
                    "buttonEast" => buttonEast,
                    "buttonWest" => buttonWest,
                    "start" => startButton,
                    "select" => selectButton,
                    "leftTrigger" => leftTrigger,
                    "rightTrigger" => rightTrigger,
                    "leftShoulder" => leftShoulder,
                    "rightShoulder" => rightShoulder,
                    "dpad" => dpad,
                    "dpad/up" => dpadUp,
                    "dpad/down" => dpadDown,
                    "dpad/left" => dpadLeft,
                    "dpad/right" => dpadRight,
                    "leftStick" => leftStick,
                    "rightStick" => rightStick,
                    "leftStickPress" => leftStickPress,
                    "rightStickPress" => rightStickPress,
                    _ => null
                };
            }
        }
    }
}