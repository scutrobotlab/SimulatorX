// Copyright (c) 2015 - 2021 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

//.........................
//.....Generated Class.....
//.........................
//.......Do not edit.......
//.........................

using System.Diagnostics.CodeAnalysis;
using Doozy.Editor.EditorUI.ScriptableObjects.Fonts;
using Doozy.Editor.EditorUI.Utils;
using UnityEngine;

namespace Doozy.Editor.EditorUI
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class EditorFonts
    {
        public static class Default
        {
            public static Font GetFont() =>
                DesignUtils.unityDefaultFont;
        }

        public static class Inter
        {
            private static EditorDataFontFamily s_fontFamily;
            private static EditorDataFontFamily fontFamily =>
                  s_fontFamily != null
                     ? s_fontFamily
                     : s_fontFamily = EditorDataFontDatabase.GetFontFamily("Inter");

            public static Font GetFont(FontWeight weight) =>
                fontFamily.GetFont((int) weight);

            public enum FontWeight
            {
                Thin = 100,
                ExtraLight = 200,
                Light = 300,
                Regular = 400,
                Medium = 500,
                SemiBold = 600,
                Bold = 700,
                ExtraBold = 800,
                Black = 900                
            }
            

            private static Font s_Thin;
            public static Font Thin => s_Thin ? s_Thin : s_Thin = GetFont(FontWeight.Thin);
            private static Font s_ExtraLight;
            public static Font ExtraLight => s_ExtraLight ? s_ExtraLight : s_ExtraLight = GetFont(FontWeight.ExtraLight);
            private static Font s_Light;
            public static Font Light => s_Light ? s_Light : s_Light = GetFont(FontWeight.Light);
            private static Font s_Regular;
            public static Font Regular => s_Regular ? s_Regular : s_Regular = GetFont(FontWeight.Regular);
            private static Font s_Medium;
            public static Font Medium => s_Medium ? s_Medium : s_Medium = GetFont(FontWeight.Medium);
            private static Font s_SemiBold;
            public static Font SemiBold => s_SemiBold ? s_SemiBold : s_SemiBold = GetFont(FontWeight.SemiBold);
            private static Font s_Bold;
            public static Font Bold => s_Bold ? s_Bold : s_Bold = GetFont(FontWeight.Bold);
            private static Font s_ExtraBold;
            public static Font ExtraBold => s_ExtraBold ? s_ExtraBold : s_ExtraBold = GetFont(FontWeight.ExtraBold);
            private static Font s_Black;
            public static Font Black => s_Black ? s_Black : s_Black = GetFont(FontWeight.Black);
            
        }


        public static class Sansation
        {
            private static EditorDataFontFamily s_fontFamily;
            private static EditorDataFontFamily fontFamily =>
                  s_fontFamily != null
                     ? s_fontFamily
                     : s_fontFamily = EditorDataFontDatabase.GetFontFamily("Sansation");

            public static Font GetFont(FontWeight weight) =>
                fontFamily.GetFont((int) weight);

            public enum FontWeight
            {
                Light = 300,
                Regular = 400,
                Bold = 700                
            }
            

            private static Font s_Light;
            public static Font Light => s_Light ? s_Light : s_Light = GetFont(FontWeight.Light);
            private static Font s_Regular;
            public static Font Regular => s_Regular ? s_Regular : s_Regular = GetFont(FontWeight.Regular);
            private static Font s_Bold;
            public static Font Bold => s_Bold ? s_Bold : s_Bold = GetFont(FontWeight.Bold);
            
        }


        public static class Ubuntu
        {
            private static EditorDataFontFamily s_fontFamily;
            private static EditorDataFontFamily fontFamily =>
                  s_fontFamily != null
                     ? s_fontFamily
                     : s_fontFamily = EditorDataFontDatabase.GetFontFamily("Ubuntu");

            public static Font GetFont(FontWeight weight) =>
                fontFamily.GetFont((int) weight);

            public enum FontWeight
            {
                Light = 300,
                Regular = 400,
                Medium = 500,
                Bold = 700                
            }
            

            private static Font s_Light;
            public static Font Light => s_Light ? s_Light : s_Light = GetFont(FontWeight.Light);
            private static Font s_Regular;
            public static Font Regular => s_Regular ? s_Regular : s_Regular = GetFont(FontWeight.Regular);
            private static Font s_Medium;
            public static Font Medium => s_Medium ? s_Medium : s_Medium = GetFont(FontWeight.Medium);
            private static Font s_Bold;
            public static Font Bold => s_Bold ? s_Bold : s_Bold = GetFont(FontWeight.Bold);
            
        }

    }
}