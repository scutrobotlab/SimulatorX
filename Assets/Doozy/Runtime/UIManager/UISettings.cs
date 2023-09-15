// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Common.Attributes;
using UnityEngine;

namespace Doozy.Runtime.UIManager
{
    public static class UISettings
    {
        [ClearOnReload(resetValue: false)]
        private static bool initialized { get; set; }
        private static void Initialize()
        {
            if (initialized) return;
            initialized = true;
        }

        /// <summary>
        /// Internal variable used to keep track if the UI interactions are disabled or not. Additive bool.
        /// <para/> = 0 : FALSE (UI interactions are NOT disabled)
        /// <para/> > 0 : TRUE (UI interactions are disabled)
        /// </summary>
        [ClearOnReload(resetValue: 0)]
        private static int s_interactionsDisableLevel;

        /// <summary> TRUE if the UI interactions are disabled </summary>
        public static bool interactionsDisabled => s_interactionsDisableLevel > 0;

        /// <summary> Enables UI interactions (by decreasing the additive bool by one level) </summary>
        /// <param name="byForce">
        /// Enables UI interactions by resetting the additive bool to zero.
        /// Use this ONLY in special cases when something wrong happens and the UI interactions are stuck in disabled mode.
        /// </param>
        public static void EnableUIInteractions(bool byForce = false) =>
            s_interactionsDisableLevel = byForce ? 0 : Mathf.Max(0, s_interactionsDisableLevel - 1);

        /// <summary> Disables UI Interactions (by increasing the additive bool by one level) </summary>
        public static void DisableUIInteractions() =>
            s_interactionsDisableLevel++;

    }
}
