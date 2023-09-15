// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.UIManager.ScriptableObjects;

namespace Doozy.Runtime.UIManager
{
    [Serializable]
    public struct UIViewSignalData
    {
        public static UIManagerInputSettings inputSettings => UIManagerInputSettings.instance;
        public static bool multiplayerMode => inputSettings.multiplayerMode;

        public string viewCategory { get; private set; }
        public string viewName { get; private set; }
        public ShowHideExecute execute { get; private set; }
        public int playerIndex { get; private set; }

        /// <summary>
        /// View category is null or empty.
        /// <para/> This is a global command used to show/hide all registered views
        /// <para/> Bypass checks for both view category and view name, then execute show/hide
        /// </summary>
        public bool globalCommand => viewCategory.IsNullOrEmpty();
        
        /// <summary>
        /// View name is null or empty
        /// <para/> This is a category command used to show/hide entire categories
        /// <para/> Check view category and bypass view name check, then execute show/hide
        /// </summary>
        public bool categoryCommand => viewName.IsNullOrEmpty();

        public UIViewSignalData(string viewCategory, string viewName, ShowHideExecute execute, int playerIndex)
        {
            this.viewCategory = viewCategory;
            this.viewName = viewName;
            this.execute = execute;
            this.playerIndex = playerIndex;
        }

        public override string ToString()
        {
            string message =
                multiplayerMode && playerIndex != inputSettings.defaultPlayerIndex
                    ? $"Player {playerIndex} > "
                    : string.Empty;

            message += $"({ObjectNames.NicifyVariableName(execute.ToString())}) ";

            return globalCommand
                ? $"{message} All Views"
                : categoryCommand
                    ? $"{message} {viewCategory} category"
                    : $"{message} {viewCategory} / {viewName}";
        }
    }
}
