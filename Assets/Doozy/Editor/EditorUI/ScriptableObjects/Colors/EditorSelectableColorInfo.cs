// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Common.Extensions;
using UnityEngine;

namespace Doozy.Editor.EditorUI.ScriptableObjects.Colors
{
    [Serializable]
    public class EditorSelectableColorInfo
    {
        public string ColorName;

        public EditorThemeColor Normal;
        public Color normalColor => Normal.color;

        public EditorThemeColor Highlighted;
        public Color highlightedColor => Highlighted.color;

        public EditorThemeColor Pressed;
        public Color pressedColor => Pressed.color;

        public EditorThemeColor Selected;
        public Color selectedColor => Selected.color;

        public EditorThemeColor Disabled;
        public Color disabledColor => Disabled.color;

        public EditorSelectableColorInfo()
        {
            Normal = new EditorThemeColor();
            Highlighted = new EditorThemeColor();
            Pressed = new EditorThemeColor();
            Selected = new EditorThemeColor();
            Disabled = new EditorThemeColor();
        }

        public Color GetColor(SelectionState state)
        {
            switch (state)
            {
                case SelectionState.Normal: return normalColor;
                case SelectionState.Highlighted: return highlightedColor;
                case SelectionState.Pressed: return pressedColor;
                case SelectionState.Selected: return selectedColor;
                case SelectionState.Disabled: return disabledColor;
                default: throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public EditorThemeColor GetThemeColor(SelectionState state)
        {
            switch (state)
            {
                case SelectionState.Normal: return Normal;
                case SelectionState.Highlighted: return Highlighted;
                case SelectionState.Pressed: return Pressed;
                case SelectionState.Selected: return Selected;
                case SelectionState.Disabled: return Disabled;
                default: throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public EditorSelectableColorInfo GenerateAllColorVariantsFromNormalColor()
        {
            GenerateOnDarkColorVariantsFromNormalColor();//OnDark
            GenerateOnLightColorVariantsFromNormalColor();//OnLight
            return this;
        }

        public EditorSelectableColorInfo GenerateOnDarkColorVariantsFromNormalColor()
        {
            //OnDark
            // Highlighted.ColorOnDark = Normal.ColorOnDark.SetHSLHue(Normal.ColorOnDark.Hue() - 0.02f).WithRGBTint(0.1f);
            Highlighted.ColorOnDark = Normal.ColorOnDark.SetHSLHue(Normal.ColorOnDark.Hue() - 0.02f).WithRGBShade(0.2f);
            Pressed.ColorOnDark = Highlighted.ColorOnDark.WithRGBShade(0.2f);
            Selected.ColorOnDark = Highlighted.ColorOnDark.WithRGBShade(0.1f);
            Disabled.ColorOnDark = Normal.ColorOnDark.WithAlpha(0.6f);
            return this;
        }
        
        public EditorSelectableColorInfo GenerateOnLightColorVariantsFromNormalColor()
        {
            //OnLight
            Highlighted.ColorOnLight = Normal.ColorOnLight.SetHSLHue(Normal.ColorOnLight.Hue() - 0.02f).WithRGBShade(0.2f);
            Pressed.ColorOnLight = Highlighted.ColorOnLight.WithRGBShade(0.1f);
            Selected.ColorOnLight = Highlighted.ColorOnLight.WithRGBTint(0.1f);
            Disabled.ColorOnLight = Normal.ColorOnLight.WithAlpha(0.6f);
            return this;
        }
        
        public void ValidateName() =>
            ColorName = ColorName.RemoveWhitespaces().RemoveAllSpecialCharacters();
    }
}
