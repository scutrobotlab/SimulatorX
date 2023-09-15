// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Input;
using Doozy.Runtime.UIManager.ScriptableObjects;

namespace Doozy.Runtime.UIManager
{
    [Serializable]
    public struct UIButtonSignalData
    {
        public static UIManagerInputSettings inputSettings => UIManagerInputSettings.instance;
        public static bool multiplayerMode => inputSettings.multiplayerMode;

        public string buttonCategory { get; private set; }
        public string buttonName { get; private set; }
        public ButtonTrigger trigger { get; private set; }
        public int playerIndex { get; private set; }
        public UIButton button { get; private set; }

        public bool isBackButton => buttonName.Equals(BackButton.k_ButtonName);
        
        public UIButtonSignalData(string buttonCategory, string buttonName, ButtonTrigger trigger, int playerIndex, UIButton button = null)
        {
            this.buttonCategory = buttonCategory;
            this.buttonName = buttonName;
            this.trigger = trigger;
            this.playerIndex = playerIndex;
            this.button = button;
        }

        public override string ToString()
        {
            string message =
                multiplayerMode && playerIndex != inputSettings.defaultPlayerIndex
                    ? $"Player {playerIndex} > "
                    : string.Empty;

            message += $"({ObjectNames.NicifyVariableName(trigger.ToString())}) ";

            return $"{message} {buttonCategory} / {buttonName}";
        }
    }
}
