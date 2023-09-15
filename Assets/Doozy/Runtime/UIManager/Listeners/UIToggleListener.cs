// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Input;
using Doozy.Runtime.UIManager.Listeners.Internal;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault

namespace Doozy.Runtime.UIManager.Listeners
{
    /// <summary>
    /// Connects to the UIToggle stream and reacts to ‘pings’ (signals) sent for the toggle with the given id.
    /// </summary>
    [AddComponentMenu("Doozy/UI/Listeners/UI Toggle Listener")]
    public class UIToggleListener : BaseListener, IUseMultiplayerInfo
    {
        /// <summary> Reference to the UIManager Input Settings </summary>
        public static UIManagerInputSettings inputSettings => UIManagerInputSettings.instance;

        /// <summary> True if Multiplayer Mode is enabled </summary>
        public static bool multiplayerMode => inputSettings.multiplayerMode;

        [SerializeField] private UIToggleId ToggleId;
        [SerializeField] private CommandToggle Command;

        /// <summary> UIView Id </summary>
        public UIToggleId toggleId => ToggleId;

        /// <summary> Show/Hide command to react to </summary>
        public CommandToggle command => Command;

        /// <summary> UnityAction callback executed when the listener is triggered </summary>
        public UnityAction<UIToggleSignalData> signalCallback { get; }

        #region Player Index

        [SerializeField] private MultiplayerInfo MultiplayerInfo;
        public MultiplayerInfo multiplayerInfo => MultiplayerInfo;
        public bool hasMultiplayerInfo => multiplayerInfo != null;
        public int playerIndex => multiplayerMode & hasMultiplayerInfo ? multiplayerInfo.playerIndex : inputSettings.defaultPlayerIndex;
        public void SetMultiplayerInfo(MultiplayerInfo info) => MultiplayerInfo = info;

        #endregion

        private void OnEnable() =>
            ConnectReceiver();

        private void OnDisable() =>
            DisconnectReceiver();

        protected override void ConnectReceiver() =>
            UIToggle.stream.ConnectReceiver(receiver);

        protected override void DisconnectReceiver() =>
            UIToggle.stream.DisconnectReceiver(receiver);

        protected override void ProcessSignal(Signal signal)
        {
            if (!signal.hasValue) return;
            if (!(signal.valueAsObject is UIToggleSignalData data)) return;
            if (Command != CommandToggle.Any && Command != data.state) return; //check toggle state
            if (!ToggleId.Category.Equals(data.toggleCategory)) return;        //check category
            if (!ToggleId.Name.Equals(data.toggleName)) return;                //check name
            if (multiplayerMode && playerIndex != data.playerIndex) return;    //check player index (if multiplayer)

            signalCallback?.Invoke(data);
            Callback?.Execute(signal);
        }
    }
}
