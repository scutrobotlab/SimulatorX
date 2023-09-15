// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections;
using System.Linq;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;
using UnityEngine.EventSystems;

#if INPUT_SYSTEM_PACKAGE
using UnityEngine.InputSystem.UI;
#endif

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Input
{
    /// <summary>
    /// The ‘Back’ Button functionality injects itself into the Input System and listens for the ‘Cancel’ action.
    /// It does that by automatically attaching a Input To Signal to the Event System.
    /// This is an automated system that is activated by any UI component in DoozyUI.
    /// </summary>
    [AddComponentMenu("Doozy/UI/Input/Back Button")]
    [DisallowMultipleComponent]
    public class BackButton : SingletonBehaviour<BackButton>
    {
        #if LEGACY_INPUT_MANAGER
        public const KeyCode k_BackButtonKeyCode = KeyCode.Escape;
        public const string k_BackButtonVirtualButtonName = "Cancel";
        #endif

        public const string k_StreamCategory = "Input";
        public const string k_StreamName = nameof(BackButton);
        public const string k_ButtonName = "Back"; //ToDo: maybe allow for different button names to be THE 'Back' button

        [ClearOnReload]
        private static SignalStream s_stream;
        [ClearOnReload]
        private static SignalStream s_streamOnEnabled;
        [ClearOnReload]
        private static SignalStream s_streamOnDisabled;

        /// <summary> Stream that sends signals when the 'Back' button is fired </summary>
        public static SignalStream stream => s_stream ??= SignalsService.GetStream(k_StreamCategory, k_StreamName);

        /// <summary> Stream that sends signals when the 'Back' button functionality was enabled (from the disabled state) </summary>
        public static SignalStream streamOnEnabled => s_streamOnEnabled ??= SignalsService.GetStream(k_StreamCategory, $"{k_StreamName}.Enabled");

        /// <summary> Stream that sends signals when the 'Back' button functionality was disabled (from the enabled state) </summary>
        public static SignalStream streamOnDisabled => s_streamOnDisabled ??= SignalsService.GetStream(k_StreamCategory, $"{k_StreamName}.Disabled");

        [ClearOnReload]
        private static SignalReceiver inputStreamReceiver { get; set; }
        private static void ConnectToInputStream()
        {
            InputStream.Start();
            InputStream.stream.ConnectReceiver(inputStreamReceiver);
        }
        private static void DisconnectFromInputStream()
        {
            InputStream.Stop();
            InputStream.stream.DisconnectReceiver(inputStreamReceiver);
        }

        [ClearOnReload]
        private static SignalReceiver buttonStreamReceiver { get; set; }
        private static void ConnectToButtonStream() => UIButton.stream.ConnectReceiver(buttonStreamReceiver);
        private static void DisconnectFromButtonStream() => UIButton.stream.DisconnectReceiver(buttonStreamReceiver);

        /// <summary> Reference to the UIManager Input Settings </summary>
        public static UIManagerInputSettings inputSettings => UIManagerInputSettings.instance;

        /// <summary> True Multiplayer Mode is enabled </summary>
        public static bool multiplayerMode => inputSettings.multiplayerMode;

        private int m_BackButtonDisableLevel;           //This is an additive bool so if == 0 --> false (the 'Back' button is NOT disabled) and if > 0 --> true (the 'Back' button is disabled).
        private double m_LastTimeBackButtonWasExecuted; //Internal variable used to keep track when the 'Back' button was executed the last time

        /// <summary> Cooldown after the 'Back' button was fired (to prevent spamming and accidental double execution) </summary>
        public static float cooldown => inputSettings.backButtonCooldown;

        /// <summary> True if the 'Back' button functionality is disabled </summary>
        public bool isDisabled
        {
            get
            {
                if (m_BackButtonDisableLevel < 0) m_BackButtonDisableLevel = 0;
                return m_BackButtonDisableLevel != 0;
            }
        }

        /// <summary> True if the 'Back' button functionality is enabled </summary>
        public bool isEnabled => !isDisabled;

        /// <summary> True if the 'Back' button can be triggered again </summary>
        public bool inCooldown => Time.realtimeSinceStartup - m_LastTimeBackButtonWasExecuted < cooldown;

        /// <summary> True if the 'Back' button functionality is enabled and is not in cooldown </summary>
        public bool canFire => isEnabled && !inCooldown;

        /// <summary> Flag marked as True if a InputToSignal set to listen for the 'Back' button exists in the scene </summary>
        public bool hasInput { get; private set; }

        private bool initialized { get; set; }

        public static void Initialize()
        {
            if (applicationIsQuitting)
                return;

            _ = instance;
        }

        protected override void Awake()
        {
            base.Awake();
            initialized = false;
            m_LastTimeBackButtonWasExecuted = Time.realtimeSinceStartup;
        }

        private IEnumerator Start()
        {
            yield return null;
            CheckForInput();
            initialized = hasInput;

            inputStreamReceiver = new SignalReceiver().SetOnSignalCallback(signal =>
            {
                if (!signal.hasValue) return;

                #if INPUT_SYSTEM_PACKAGE
                if (!(signal.valueAsObject is InputSignalData { inputActionName: UIInputActionName.Cancel } data)) return;
                Fire(data);
                #endif

                #if LEGACY_INPUT_MANAGER
                if (!(signal.valueAsObject is InputSignalData data)) return;
                switch (data.inputMode)
                {
                    case LegacyInputMode.KeyCode:
                        if (data.keyCode == BackButton.k_BackButtonKeyCode)
                            Fire(data);
                        break;
                    case LegacyInputMode.VirtualButton:
                        if (data.virtualButtonName == BackButton.k_BackButtonVirtualButtonName)
                            Fire(data);
                        break;
                    case LegacyInputMode.None: break;
                    default: throw new System.ArgumentOutOfRangeException();
                }
                #endif


            });
            ConnectToInputStream();

            buttonStreamReceiver = new SignalReceiver().SetOnSignalCallback(signal =>
            {
                if (!signal.hasValue)
                    return;

                if (!(signal.valueAsObject is UIButtonSignalData data))
                    return;

                if (!data.buttonName.Equals(k_ButtonName))
                    return;

                #if INPUT_SYSTEM_PACKAGE
                Fire(new InputSignalData(UIInputActionName.Cancel, data.playerIndex));
                #endif

                #if LEGACY_INPUT_MANAGER
                Fire(new InputSignalData(LegacyInputMode.KeyCode, BackButton.k_BackButtonKeyCode, BackButton.k_BackButtonVirtualButtonName, data.playerIndex));
                #endif

            });

            ConnectToButtonStream();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            DisconnectFromInputStream();
            DisconnectFromButtonStream();
        }

        //ToDo: maybe -> add option to set default action name for 'Back' button (instead of Cancel)
        public void CheckForInput()
        {
            hasInput = false;

            if (EventSystem.current == null && !multiplayerMode)
            {
                Debug.LogWarning
                (
                    $"{nameof(EventSystem)}.current is null. " +
                    $"Add it to the scene to fix this issue."
                );
                return;
            }

            if (EventSystem.current != null)
            {
                AddInputToSignalToGameObject(EventSystem.current.gameObject);
                hasInput = true;
            }

            if (!multiplayerMode) //multiplayer mode disabled -> stop here
                return;

            #if INPUT_SYSTEM_PACKAGE
            MultiplayerEventSystem[] multiplayerEventSystems = FindObjectsOfType<MultiplayerEventSystem>();
            if (!hasInput || multiplayerEventSystems == null || multiplayerEventSystems.Length == 0)
            {
                Debug.LogWarning
                (
                    $"MultiplayerMode -> No {nameof(MultiplayerEventSystem)} found. " +
                    $"Add at least one to the scene to fix this issue."
                );
                return;
            }

            foreach (MultiplayerEventSystem eventSystem in multiplayerEventSystems)
                AddInputToSignalToGameObject(eventSystem.gameObject);
            #endif

            hasInput = true;
        }

        /// <summary>
        /// Execute the 'Back' button event, only if can fire and is enabled.
        /// This method is used to simulate a 'Back' button 
        /// </summary>
        public static void Fire(InputSignalData data)
        {
            if (applicationIsQuitting) return;
            if (!instance.canFire) return;
            stream.SendSignal(data); //this sends a MetaSignal (with input data)
            instance.m_LastTimeBackButtonWasExecuted = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// Execute the 'Back' button event, only if can fire and is enabled.
        /// This method is used to simulate a 'Back' button 
        /// </summary>
        public static void Fire()
        {
            if (applicationIsQuitting) return;
            if (!instance.canFire) return;
            stream.SendSignal(); //this sends a Signal (ping)
            instance.m_LastTimeBackButtonWasExecuted = Time.realtimeSinceStartup;
        }

        /// <summary> True if the 'Back' button functionality is enabled </summary>
        public static bool IsEnabled() => !applicationIsQuitting && instance.isEnabled;

        /// <summary> True if the 'Back' button functionality is disabled </summary>
        public static bool IsDisabled() => !applicationIsQuitting && instance.isDisabled;

        /// <summary> Disable the 'Back' button functionality </summary>
        public static void Disable()
        {
            if (applicationIsQuitting) return;
            // Debug.Log($"{nameof(BackButton)}.{nameof(Disable)}");
            if (instance.isEnabled) streamOnDisabled.SendSignal($"{nameof(BackButton)}.{nameof(Disable)}");
            instance.m_BackButtonDisableLevel++; //if == 0 --> false (back button is not disabled) if > 0 --> true (back button is disabled)
        }

        /// <summary> Enable the 'Back' button functionality </summary>
        public static void Enable()
        {
            if (applicationIsQuitting) return;
            // Debug.Log($"{nameof(BackButton)}.{nameof(Enable)}");
            instance.m_BackButtonDisableLevel--; //if == 0 --> false (back button is not disabled) if > 0 --> true (back button is disabled)
            if (instance.m_BackButtonDisableLevel < 0) instance.m_BackButtonDisableLevel = 0;
            if (instance.isEnabled) streamOnEnabled.SendSignal($"{nameof(BackButton)}.{nameof(Enable)}");
        }

        /// <summary> Enable the 'Back' button functionality by resetting the additive bool to zero. backButtonDisableLevel = 0. Use this ONLY for special cases when something wrong happens and the back button is stuck in disabled mode </summary>
        public static void EnableByForce()
        {
            if (applicationIsQuitting) return;
            // Debug.Log($"{nameof(BackButton)}.{nameof(EnableByForce)}");
            instance.m_BackButtonDisableLevel = 0;
            streamOnEnabled.SendSignal($"{nameof(BackButton)}.{nameof(EnableByForce)}");
        }

        /// <summary>
        /// Checks if the given target has an InputToSignal that triggers the 'Back' button.
        /// If no such InputToSignal is found, one is added automatically 
        /// </summary>
        /// <param name="target"> Target gameObject </param>
        private static void AddInputToSignalToGameObject(GameObject target)
        {
            #if !INPUT_SYSTEM_PACKAGE && !LEGACY_INPUT_MANAGER
            return;
            #endif

            #pragma warning disable CS0162
            //search that there is at least one InputToSignal able to trigger the 'Back' button; if not, add it
            InputToSignal[] inputsToSignal = target.GetComponents<InputToSignal>();
            if
            (
                inputsToSignal == null ||
                inputsToSignal.Length == 0 ||
                !inputsToSignal.Any(i => i.SendsBackButtonSignal())
            )
            {
                #if INPUT_SYSTEM_PACKAGE
                target
                    .AddComponent<InputToSignal>()
                    .ConnectToAction(UIInputActionName.Cancel);
                #endif

                #if LEGACY_INPUT_MANAGER
                InputToSignal its = target.AddComponent<InputToSignal>();
                its.inputMode = LegacyInputMode.KeyCode;
                its.keyCode = BackButton.k_BackButtonKeyCode;
                its.virtualButtonName = BackButton.k_BackButtonVirtualButtonName;
                #endif
            }
            #pragma warning restore CS0162
        }


    }

    public static class BackButtonExtras
    {
        public static bool SendsBackButtonSignal<T>(this T target) where T : InputToSignal
        {

            #if INPUT_SYSTEM_PACKAGE
            return target != null &&
                target.isConnected &&
                target.inputActionName.Equals(UIInputActionName.Cancel.ToString());
            #endif

            #if LEGACY_INPUT_MANAGER
            return
                target != null &&
                target.inputMode switch
                {
                    LegacyInputMode.None          => false,
                    LegacyInputMode.KeyCode       => target.keyCode == BackButton.k_BackButtonKeyCode,
                    LegacyInputMode.VirtualButton => target.virtualButtonName == BackButton.k_BackButtonVirtualButtonName,
                    _                             => false
                };
            #endif

            #pragma warning disable CS0162
            return false;
            #pragma warning restore CS0162
        }
    }
}
