// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.UIManager.Input;
using Doozy.Runtime.UIManager.ScriptableObjects;

#if INPUT_SYSTEM_PACKAGE
using UnityEngine.InputSystem;
#endif

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace Doozy.Runtime.UIManager
{
    [Serializable]
    public struct InputSignalData
    {
        public static UIManagerInputSettings inputSettings => UIManagerInputSettings.instance;
        public static bool multiplayerMode => inputSettings.multiplayerMode;

        public int playerIndex { get; }
        
        #if INPUT_SYSTEM_PACKAGE
        public InputAction.CallbackContext callbackContext { get; }
        public PlayerInput playerInput { get; }
        public bool hasPlayerInput => playerInput != null;
        public bool ignorePlayerIndex => playerIndex == inputSettings.defaultPlayerIndex;
        public UIInputActionName inputActionName { get; }

        public InputSignalData(UIInputActionName inputActionName, int playerIndex) : this(inputActionName, new InputAction.CallbackContext(), playerIndex, null) {}

        public InputSignalData(UIInputActionName inputActionName, InputAction.CallbackContext callbackContext, int playerIndex, PlayerInput playerInput = null)
        {
            this.inputActionName = inputActionName;
            this.callbackContext = callbackContext;
            this.playerIndex = playerIndex;
            this.playerInput = playerInput;
        }

        public override string ToString()
        {
            string message = callbackContext.action != null ? $"'{callbackContext.action.name}'" : inputActionName.ToString();
            if (multiplayerMode && !ignorePlayerIndex) message += $" called by Player {playerIndex}";
            return message;
        }
        #endif

        #if LEGACY_INPUT_MANAGER
        public LegacyInputMode inputMode { get; }
        public UnityEngine.KeyCode keyCode { get; }
        public string virtualButtonName { get; }
        public bool ignorePlayerIndex => playerIndex == inputSettings.defaultPlayerIndex;

        public InputSignalData(LegacyInputMode inputMode, UnityEngine.KeyCode keyCode, string virtualButtonName, int playerIndex)
        {
            this.virtualButtonName = virtualButtonName;
            this.keyCode = keyCode;
            this.playerIndex = playerIndex;
            this.inputMode = inputMode;
        }

        public override string ToString()
        {
            string message;
            switch (inputMode)
            {
                case LegacyInputMode.None:
                    message = string.Empty;
                    break;
                case LegacyInputMode.KeyCode:
                    message = $"'{keyCode}'";
                    break;
                case LegacyInputMode.VirtualButton:
                    message = $"'{virtualButtonName}'";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (multiplayerMode && !ignorePlayerIndex) message += $" called by Player {playerIndex}";
            return message;
        }
        #endif
    }
}
