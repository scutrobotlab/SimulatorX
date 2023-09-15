// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine.Events;

namespace Doozy.Editor.EditorUI.WindowLayouts
{
    public abstract class EditorUIDatabaseWindowLayout : FluidWindowLayout
    {
        protected FluidButton refreshDatabaseButton { get; private set; }

        protected void InitializeRefreshDatabaseButton(string labelText, string tooltipText, EditorSelectableColorInfo selectableColor, UnityAction onClickCallback)
        {
            refreshDatabaseButton = GetRefreshDatabaseButton(labelText, tooltipText, selectableColor, onClickCallback);
        }

        private static FluidButton GetRefreshDatabaseButton
        (
            string labelText,
            string tooltipText,
            EditorSelectableColorInfo selectableColor,
            UnityAction onClickCallback
        ) =>
            FluidButton.Get()
                .SetLabelText(labelText)
                .SetTooltip(tooltipText)
                .SetAccentColor(selectableColor)
                .SetOnClick(onClickCallback)
                .SetButtonStyle(ButtonStyle.Contained)
                .SetElementSize(ElementSize.Small)
                .SetIcon(EditorMicroAnimations.EditorUI.Icons.Refresh);
    }
}
