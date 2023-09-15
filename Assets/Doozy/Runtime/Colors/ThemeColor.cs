// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;

namespace Doozy.Runtime.Colors
{
    [Serializable]
    public class ThemeColor
    {
        public virtual bool isDarkTheme { get; set; }

        public Color ColorOnDark;
        public Color ColorOnLight;

        public Color color =>
            isDarkTheme
                ? ColorOnDark
                : ColorOnLight;

        public ThemeColor()
        {
            ColorOnDark = ColorOnLight = Color.white;
        }
    }
}
