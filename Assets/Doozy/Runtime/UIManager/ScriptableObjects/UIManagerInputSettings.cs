// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.UIManager.Input;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.ScriptableObjects
{
    public class UIManagerInputSettings : SingletonRuntimeScriptableObject<UIManagerInputSettings>
    {
        #if INPUT_SYSTEM_PACKAGE
        public const InputHandling k_InputHandling = InputHandling.InputSystemPackage;
        #elif LEGACY_INPUT_MANAGER
        public const InputHandling k_InputHandling = InputHandling.LegacyInputManager;
        #else //CustomInput
        public const InputHandling k_InputHandling = InputHandling.CustomInput;
        #endif
        
        public const int k_LifeTheUniverseAndEverything = 42;
        public const float k_BackButtonCooldown = 0.1f;
        
        [SerializeField] private int DefaultPlayerIndex = -k_LifeTheUniverseAndEverything;
        /// <summary> Default player index value (used for global user) </summary>
        public int defaultPlayerIndex => DefaultPlayerIndex;
        
        [SerializeField] private bool MultiplayerMode;
        /// <summary> True if Multiplayer Mode is enabled </summary>
        public bool multiplayerMode
        {
            get => MultiplayerMode;
            set => MultiplayerMode = value;
        }
        
        [SerializeField] private float BackButtonCooldown = k_BackButtonCooldown;
        /// <summary> Cooldown after the 'Back' button was fired (to prevent spamming and accidental double execution) </summary>
        public float backButtonCooldown
        {
            get => BackButtonCooldown;
            set => BackButtonCooldown = value;
        }

        [SerializeField] private bool SubmitTriggersPointerClick = true;
        /// <summary> If TRUE, the ISubmitHandler will trigger the Pointer Click events </summary>
        public bool submitTriggersPointerClick
        {
            get => SubmitTriggersPointerClick;
            set => SubmitTriggersPointerClick = value;
        }
    }
}
