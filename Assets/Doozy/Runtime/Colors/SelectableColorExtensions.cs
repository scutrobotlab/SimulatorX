// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Doozy.Runtime.Colors
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class SelectableColorExtensions
    {
        public static T SetState<T>(this T target, SelectionState state) where T : SelectableColor
        {
            target.SetSelectionState(state);
            return target;
        }

        public static T SetNormalColor<T>(this T target, Color color) where T : SelectableColor =>
            target.SetNormalColor(color, color);

        public static T SetNormalColor<T>(this T target, Color colorOnDark, Color colorOnLight) where T : SelectableColor
        {
            target.Normal.ColorOnDark = colorOnDark;
            target.Normal.ColorOnLight = colorOnLight;

            if (target.currentState == SelectionState.Normal)
                target.SelectionStateChanged(target.normalColor);

            return target;
        }

        public static T SetHighlightedColor<T>(this T target, Color color) where T : SelectableColor =>
            target.SetHighlightedColor(color, color);

        public static T SetHighlightedColor<T>(this T target, Color colorOnDark, Color colorOnLight) where T : SelectableColor
        {
            target.Highlighted.ColorOnDark = colorOnDark;
            target.Highlighted.ColorOnLight = colorOnLight;

            if (target.currentState == SelectionState.Highlighted)
                target.SelectionStateChanged(target.highlightedColor);

            return target;
        }

        public static T SetPressedColor<T>(this T target, Color color) where T : SelectableColor =>
            target.SetPressedColor(color, color);

        public static T SetPressedColor<T>(this T target, Color colorOnDark, Color colorOnLight) where T : SelectableColor
        {
            target.Pressed.ColorOnDark = colorOnDark;
            target.Pressed.ColorOnLight = colorOnLight;

            if (target.currentState == SelectionState.Pressed)
                target.SelectionStateChanged(target.pressedColor);

            return target;
        }

        public static T SetSelectedColor<T>(this T target, Color color) where T : SelectableColor =>
            target.SetSelectedColor(color, color);

        public static T SetSelectedColor<T>(this T target, Color colorOnDark, Color colorOnLight) where T : SelectableColor
        {
            target.Selected.ColorOnDark = colorOnDark;
            target.Selected.ColorOnLight = colorOnLight;

            if (target.currentState == SelectionState.Selected)
                target.SelectionStateChanged(target.selectedColor);

            return target;
        }

        public static T SetDisabledColor<T>(this T target, Color color) where T : SelectableColor =>
            target.SetDisabledColor(color, color);

        public static T SetDisabledColor<T>(this T target, Color colorOnDark, Color colorOnLight) where T : SelectableColor
        {
            target.Disabled.ColorOnDark = colorOnDark;
            target.Disabled.ColorOnLight = colorOnLight;

            if (target.currentState == SelectionState.Disabled)
                target.SelectionStateChanged(target.disabledColor);

            return target;
        }
    }
}
