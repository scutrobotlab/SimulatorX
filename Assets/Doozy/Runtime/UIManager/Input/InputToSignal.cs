// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;

#if INPUT_SYSTEM_PACKAGE
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
#endif

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable UnusedMember.Global
// ReSharper disable CollectionNeverQueried.Global

namespace Doozy.Runtime.UIManager.Input
{
    /// <summary>
    /// Bridge that connects the Input System with the Input Stream.
    /// It does that by listening for when a specific action has been performed and sends a meta-signal on the Input Stream.
    /// </summary>
    [AddComponentMenu("Doozy/UI/Input/Input To Signal")]
    public class InputToSignal : MonoBehaviour
    {
        /// <summary> Reference to the UIManager Input Settings </summary>
        public static UIManagerInputSettings inputSettings => UIManagerInputSettings.instance;

        [ClearOnReload(newInstance: true)]
        // ReSharper disable once CollectionNeverUpdated.Global
        public static HashSet<InputToSignal> database { get; } = new HashSet<InputToSignal>();
        public static void CleanDatabase() => database.Remove(null);

        #if INPUT_SYSTEM_PACKAGE
        [SerializeField] private bool AutoConnect;
        [SerializeField] private InputSystemUIInputModule UIInputModule;
        [SerializeField] private PlayerInput PlayerInput;
        [SerializeField] private int PlayerIndex;
        [SerializeField] private UIInputActionName InputActionName;
        [SerializeField] private string CustomInputActionName;

        private InputAction m_Action;
        private bool m_IsConnected;

        public PlayerInput playerInput => PlayerInput;
        public InputSystemUIInputModule uiInputModule => UIInputModule;
        public InputAction action => m_Action;
        public int playerIndex => hasPlayerInput ? PlayerInput.playerIndex : PlayerIndex;
        public string inputActionName => hasCustomActionName ? CustomInputActionName : InputActionName.ToString();
        public bool autoConnect => AutoConnect;
        public bool isConnected => m_IsConnected;
        public bool hasPlayerInput => playerInput != null;
        public bool hasUIInputModule => uiInputModule != null;
        public bool hasCustomActionName => InputActionName == UIInputActionName.CustomActionName;

        private void GetReferences()
        {
            PlayerInput ??= GetComponent<PlayerInput>();
            if (PlayerInput != null)
            {
                UIInputModule = PlayerInput.uiInputModule;
            }
            else
            {
                UIInputModule ??= GetComponent<InputSystemUIInputModule>();
            }
        }

        private void Reset()
        {
            AutoConnect = true;
            PlayerIndex = inputSettings.defaultPlayerIndex;
            InputActionName = UIInputActionName.Cancel;
            CustomInputActionName = string.Empty;
            GetReferences();
        }

        private void Awake()
        {
            database.Add(this);
            GetReferences();
            if (UIInputModule != null) return;
            enabled = false;
        }

        private void OnEnable()
        {
            CleanDatabase();
            if (autoConnect) Connect();
        }

        private void OnDisable()
        {
            CleanDatabase();
            Disconnect();
        }

        private void OnDestroy()
        {
            database.Remove(this);
        }

        public InputToSignal Connect()
        {
            if (isConnected) return this;
            if (action != null)
            {
                action.performed -= OnActionPerformed;
                m_Action = null;
            }
            (bool isValid, string message) = IsValid();
            if (!isValid)
            {
                Debug.Log(message);
                return this;
            }
            action.performed += OnActionPerformed;
            m_IsConnected = true;
            return this;
        }

        public InputToSignal Disconnect()
        {
            if (!isConnected) return this;
            if (action == null) return this;
            action.performed -= OnActionPerformed;
            m_IsConnected = false;
            return this;
        }

        public InputToSignal ConnectToAction(UIInputActionName actionName)
        {
            Disconnect();
            m_Action = null;
            InputActionName = actionName;
            CustomInputActionName = string.Empty;
            Connect();
            return this;
        }

        public InputToSignal ConnectToCustomAction(string actionName)
        {
            Disconnect();
            m_Action = null;
            InputActionName = UIInputActionName.CustomActionName;
            CustomInputActionName = actionName;
            Connect();
            return this;
        }

        public InputToSignal ConnectToCustomAction(InputAction inputAction)
        {
            if (inputAction == null) return this;
            Disconnect();
            InputActionName = UIInputActionName.CustomActionName;
            CustomInputActionName = inputAction.name;
            m_Action = inputAction;
            Connect();
            return this;
        }

        private void OnActionPerformed(InputAction.CallbackContext context)
        {
            InputStream.stream.SendSignal(new InputSignalData(InputActionName, context, playerIndex, playerInput));
        }

        private (bool, string) IsValid()
        {
            if (m_Action != null) return (true, "Valid");
            if (UIInputModule == null) return (false, $"Not Valid: {nameof(UIInputModule)} is null");
            m_Action = null;

            m_Action = InputActionName switch
                       {
                           UIInputActionName.Point                    => UIInputModule.point.action,
                           UIInputActionName.Click                    => UIInputModule.leftClick.action,
                           UIInputActionName.MiddleClick              => UIInputModule.middleClick.action,
                           UIInputActionName.RightClick               => UIInputModule.rightClick.action,
                           UIInputActionName.ScrollWheel              => UIInputModule.scrollWheel.action,
                           UIInputActionName.Navigate                 => UIInputModule.move.action,
                           UIInputActionName.Submit                   => UIInputModule.submit.action,
                           UIInputActionName.Cancel                   => UIInputModule.cancel.action,
                           UIInputActionName.TrackedDevicePosition    => UIInputModule.trackedDevicePosition.action,
                           UIInputActionName.TrackedDeviceOrientation => UIInputModule.trackedDeviceOrientation.action,
                           UIInputActionName.CustomActionName         => UIInputModule.actionsAsset.FindAction(CustomInputActionName),
                           _                                          => throw new ArgumentOutOfRangeException()
                       };

            return
                m_Action == null
                    ? (false, $"Not Valid: {nameof(m_Action)} is null")
                    : (true, "Valid");
        }
        #endif

        #if LEGACY_INPUT_MANAGER

        public const KeyCode k_BackButtonKeyCode = KeyCode.Escape;
        public const string k_BackButtonVirtualButtonName = "Cancel";
        
        [SerializeField] private LegacyInputMode InputMode;
        [SerializeField] private KeyCode KeyCode;
        [SerializeField] private string VirtualButtonName;
        [SerializeField] private int PlayerIndex;
        
        public LegacyInputMode inputMode
        {
            get => InputMode;
            set => InputMode = value;
        }
        public KeyCode keyCode
        {
            get => KeyCode;
            set => KeyCode = value;
        }
        public string virtualButtonName
        {
            get => VirtualButtonName;
            set => VirtualButtonName = value;
        }
        public int playerIndex
        {
            get => PlayerIndex;
            set => PlayerIndex = value;
        }

        private void Reset()
        {
            InputMode = LegacyInputMode.None;
            KeyCode = default;
            VirtualButtonName = string.Empty;
            PlayerIndex = inputSettings.defaultPlayerIndex;
        }

        private void Update()
        {
            if (UISettings.interactionsDisabled) return;
            bool execute;
            switch (InputMode)
            {
                case LegacyInputMode.None:
                    execute = false;
                    break;
                case LegacyInputMode.KeyCode:
                    execute = UnityEngine.Input.GetKeyDown(keyCode);
                    break;
                case LegacyInputMode.VirtualButton:
                    execute = UnityEngine.Input.GetButtonDown(virtualButtonName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (!execute) return;
            InputStream.stream.SendSignal(new InputSignalData(inputMode, keyCode, virtualButtonName, playerIndex));
        }
        #endif
    }
}
