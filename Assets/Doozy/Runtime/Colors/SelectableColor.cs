// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming

namespace Doozy.Runtime.Colors
{
    public class SelectableColor
    {
        private bool m_IsDarkTheme;
        public bool isDarkTheme
        {
            get => m_IsDarkTheme;
            set
            {
                m_IsDarkTheme = value;
                Normal.isDarkTheme = m_IsDarkTheme;
                Highlighted.isDarkTheme = m_IsDarkTheme;
                Pressed.isDarkTheme = m_IsDarkTheme;
                Selected.isDarkTheme = m_IsDarkTheme;
                Disabled.isDarkTheme = m_IsDarkTheme;
                SelectionStateChanged(currentColor);
            }
        }

        public SelectionState currentState
        {
            get;
            private set;
        }

        public readonly ColorEvent onStateChanged;
        public Color currentColor => GetCurrentColor();

        public readonly ThemeColor Normal;
        public Color normalColor => Normal.color;

        public readonly ThemeColor Highlighted;
        public Color highlightedColor => Highlighted.color;

        public readonly ThemeColor Pressed;
        public Color pressedColor => Pressed.color;

        public readonly ThemeColor Selected;
        public Color selectedColor => Selected.color;

        public readonly ThemeColor Disabled;
        public Color disabledColor => Disabled.color;

        public SelectableColor(ColorEvent onStateChanged = null)
        {
            Normal = new ThemeColor { isDarkTheme = isDarkTheme };
            Highlighted = new ThemeColor { isDarkTheme = isDarkTheme };
            Pressed = new ThemeColor { isDarkTheme = isDarkTheme };
            Selected = new ThemeColor { isDarkTheme = isDarkTheme };
            Disabled = new ThemeColor { isDarkTheme = isDarkTheme };

            this.onStateChanged = onStateChanged;
            SetSelectionState(SelectionState.Normal);
        }

        private Color GetCurrentColor()
        {
            switch (currentState)
            {
                case SelectionState.Normal: return normalColor;
                case SelectionState.Highlighted: return highlightedColor;
                case SelectionState.Pressed: return pressedColor;
                case SelectionState.Selected: return selectedColor;
                case SelectionState.Disabled: return disabledColor;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        internal void SetSelectionState(SelectionState state)
        {
            currentState = state;
            SelectionStateChanged(currentColor);
        }

        internal void SelectionStateChanged(Color color)
        {
            onStateChanged?.Invoke(color);
        }
    }
}
