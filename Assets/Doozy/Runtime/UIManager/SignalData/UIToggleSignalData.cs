// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.ScriptableObjects;

namespace Doozy.Runtime.UIManager
{
    [Serializable]
    public struct UIToggleSignalData
    {
        public static UIManagerInputSettings inputSettings => UIManagerInputSettings.instance;
        public static bool multiplayerMode => inputSettings.multiplayerMode;
        
        public string toggleCategory { get; private set; }
        public string toggleName { get; private set; }
        public CommandToggle state { get; private set; }
        public int playerIndex { get; private set; }
        public UIToggle toggle { get; private set; }

        public UIToggleSignalData(string toggleCategory, string toggleName, CommandToggle state, int playerIndex, UIToggle toggle)
        {
            this.toggleCategory = toggleCategory;
            this.toggleName = toggleName;
            this.state = state;
            this.playerIndex = playerIndex;
            this.toggle = toggle;
        }
        
        public override string ToString()
        {
            string message =
                multiplayerMode && playerIndex != inputSettings.defaultPlayerIndex
                    ? $"Player {playerIndex} > "
                    : string.Empty;

            message += $"({ObjectNames.NicifyVariableName(state.ToString())}) ";

            return $"{message} {toggleCategory} / {toggleName}";
        }
    }
}
