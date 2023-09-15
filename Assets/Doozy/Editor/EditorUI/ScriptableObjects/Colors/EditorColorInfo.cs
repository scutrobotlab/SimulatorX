// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Common.Extensions;
using UnityEngine;

namespace Doozy.Editor.EditorUI.ScriptableObjects.Colors
{
    [Serializable]
    public class EditorColorInfo
    {
        public string ColorName;
        
        public EditorThemeColor ThemeColor;
        public Color color => ThemeColor.color;

        public EditorColorInfo(string colorName, Color colorOnDark, Color colorOnLight)
        {
            ColorName = colorName;
            ValidateName();
            ThemeColor = new EditorThemeColor
            {
                ColorOnDark = colorOnDark,
                ColorOnLight = colorOnLight
            };
        }
        
        public EditorColorInfo()
        {
            ColorName = string.Empty;
            ThemeColor = new EditorThemeColor();
        }

        public void ValidateName() =>
            ColorName = ColorName.RemoveWhitespaces().RemoveAllSpecialCharacters();
    }
}
