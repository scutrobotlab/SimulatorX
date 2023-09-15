using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Shift
{
    public class GamepadChecker : MonoBehaviour
    {
        [Header("Resources")]
        public VirtualCursor virtualCursor;
        public GameObject virtualCursorHelper;
        public GameObject eventSystem;

        [Header("Settings")]
        [Tooltip("Always update input device. If you turn off this feature, you won't able to change the input device after start, but it might increase the performance.")]
        public bool alwaysSearch = true;

        [Header("Objects")]
        [Tooltip("Objects in this list will be active when gamepad is un-plugged.")]
        public List<GameObject> keyboardObjects = new List<GameObject>();
        [Tooltip("Objects in this list will be active when gamepad is plugged.")]
        public List<GameObject> gamepadObjects = new List<GameObject>();

        Vector3 cursorPos;
        [HideInInspector] public bool gamepadConnected;
        [HideInInspector] public bool gamepadEnabled;
        [HideInInspector] public bool keyboardEnabled;

        void Start()
        {
            if (virtualCursor == null)
            {
                try
                {
                    var vCursor = (VirtualCursor)GameObject.FindObjectsOfType(typeof(VirtualCursor))[0];
                    virtualCursor = vCursor;
                }

                catch
                {
                    this.enabled = false;
                    Debug.LogWarning("<b>[Gamepad Checker]</b> There is no Virtual Cursor component attached.", this);
                }
            }

            virtualCursorHelper = virtualCursor.gameObject;

            if (alwaysSearch == false)
            {
                this.enabled = false;
                Debug.Log("<b>[Gamepad Checker]</b> Always Search is off. Input device won't be updated in case of disconnecting/connecting.");
            }

            else
            {
                this.enabled = true;
                Debug.Log("<b>[Gamepad Checker]</b> Always Search is on. Input device will be updated in case of disconnecting/connecting.");
            }

            string[] names = Input.GetJoystickNames();

            for (int i = 0; i < names.Length; i++)
            {
                if (names[i].Length >= 1)
                    gamepadConnected = true;

                else if (names[i].Length == 0)
                    gamepadConnected = false;
            }

            if (gamepadConnected == true)
                SwitchToGamepad();
            else
                SwitchToKeyboard();
        }

        void Update()
        {
            string[] names = Input.GetJoystickNames();

            for (int i = 0; i < names.Length; i++)
            {
                if (names[i].Length >= 1)
                    gamepadConnected = true;
                else if (names[i].Length == 0)
                    gamepadConnected = false;
            }

            if (gamepadConnected == true && gamepadEnabled == true
                && keyboardEnabled == false && Input.mousePosition != cursorPos)
                SwitchToKeyboard();

            else if (gamepadConnected == true && gamepadEnabled == false
                && keyboardEnabled == true && Input.GetKeyDown(KeyCode.Joystick1Button0))
                SwitchToGamepad();

            else if (gamepadConnected == false && keyboardEnabled == false)
                SwitchToKeyboard();
        }

        public void SwitchToGamepad()
        {
            if (virtualCursor == null)
                return;

            for (int i = 0; i < keyboardObjects.Count; i++)
                keyboardObjects[i].SetActive(false);

            for (int i = 0; i < gamepadObjects.Count; i++)
            {
                gamepadObjects[i].SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(gamepadObjects[i].GetComponentInParent<RectTransform>());
            }

            gamepadEnabled = true;
            keyboardEnabled = false;
            virtualCursor.enabled = true;
            virtualCursorHelper.SetActive(true);
            eventSystem.SetActive(false);
            Cursor.visible = false;
            cursorPos = Input.mousePosition;
        }

        public void SwitchToKeyboard()
        {
            if (virtualCursor == null)
                return;

            for (int i = 0; i < gamepadObjects.Count; i++)
                gamepadObjects[i].SetActive(false);

            for (int i = 0; i < keyboardObjects.Count; i++)
            {
                keyboardObjects[i].SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(keyboardObjects[0].GetComponentInParent<RectTransform>());
            }

            gamepadEnabled = false;
            keyboardEnabled = true;
            virtualCursor.enabled = false;
            virtualCursorHelper.SetActive(false);
            eventSystem.SetActive(true);
            Cursor.visible = true;
        }
    }
}