// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Containers;
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
    /// Connects to the UIView stream and reacts to ‘pings’ (signals) sent for the view with the given id, filtered by Show/Hide commands.
    /// </summary>
    [AddComponentMenu("Doozy/UI/Listeners/UI View Listener")]
    public class UIViewListener : BaseListener, IUseMultiplayerInfo
    {
        /// <summary> Reference to the UIManager Input Settings </summary>
        public static UIManagerInputSettings inputSettings => UIManagerInputSettings.instance;

        /// <summary> True if Multiplayer Mode is enabled </summary>
        public static bool multiplayerMode => inputSettings.multiplayerMode;

        [SerializeField] private UIViewId ViewId;
        [SerializeField] private CommandShowHide Command;

        /// <summary> UIView Id </summary>
        public UIViewId viewId => ViewId;
        
        /// <summary> Show/Hide command to react to </summary>
        public CommandShowHide command => Command;
        
        /// <summary> UnityAction callback executed when the listener is triggered </summary>
        public UnityAction<UIViewSignalData> signalCallback { get; }

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
            UIView.stream.ConnectReceiver(receiver);

        protected override void DisconnectReceiver() =>
            UIView.stream.DisconnectReceiver(receiver);

        protected override void ProcessSignal(Signal signal)
        {
            if (!signal.hasValue) return;
            if (!(signal.valueAsObject is UIViewSignalData data)) return;
            switch (Command)
            {
                case CommandShowHide.Show:
                    switch (data.execute)
                    {
                        case ShowHideExecute.Hide:
                        case ShowHideExecute.InstantHide:
                        case ShowHideExecute.ReverseShow:
                        case ShowHideExecute.ReverseHide:
                            return;
                    }
                    break;
                case CommandShowHide.Hide:
                    switch (data.execute)
                    {
                        case ShowHideExecute.Show:
                        case ShowHideExecute.InstantShow:
                        case ShowHideExecute.ReverseShow:
                        case ShowHideExecute.ReverseHide:
                            return;
                    }
                    break;
            }

            bool globalCommand = ViewId.Category.IsNullOrEmpty();

            if (!globalCommand) //if it's a global command skip category and name check
            {
                if (!ViewId.Category.Equals(data.viewCategory)) //check category
                    return;

                bool categoryCommand = ViewId.Name.IsNullOrEmpty();
                
                if (!categoryCommand) //if it's a category command skip name check
                    if (!ViewId.Name.Equals(data.viewName)) //check name
                        return;
            }

            if (multiplayerMode && playerIndex != data.playerIndex) return; //check player index (if multiplayer)


            
            signalCallback?.Invoke(data);
            Callback?.Execute(signal);
        }
    }
}
