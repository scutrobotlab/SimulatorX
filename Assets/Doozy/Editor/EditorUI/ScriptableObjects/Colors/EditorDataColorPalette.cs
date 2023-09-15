// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Common.Extensions;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.EditorUI.ScriptableObjects.Colors
{
    [
        CreateAssetMenu
        (
            fileName = DEFAULT_ASSET_FILENAME,
            menuName = "Doozy/EditorUI/Color Palette"
        )
    ]
    public class EditorDataColorPalette : ScriptableObject
    {
        private const string DEFAULT_ASSET_FILENAME = "_ColorPalette";
        private static string AssetFileName(string paletteName) => $"{DEFAULT_ASSET_FILENAME}_{paletteName.RemoveWhitespaces()}";

        [SerializeField] private string PaletteName;
        internal string paletteName => PaletteName;

        [SerializeField] private List<EditorColorInfo> Colors = new List<EditorColorInfo>();
        internal List<EditorColorInfo> colors => Colors;

        internal void AddNewItem() =>
            Colors.Insert(0, new EditorColorInfo());

        internal void SortByColorName() =>
            Colors = Colors.OrderBy(item => item.ColorName).ToList();

        internal void SortByHue() =>
            Colors = Colors.OrderByDescending(item => item.ThemeColor.ColorOnDark.Hue()).ToList();

        internal Color GetColor(string colorName, bool silent = false)
        {
            string cleanName = colorName.RemoveWhitespaces().RemoveAllSpecialCharacters();

            Colors = Colors.Where(item => item != null).ToList();

            // foreach (EditorColorInfo colorInfo in Colors.Where(item => item.ColorName.RemoveWhitespaces().RemoveAllSpecialCharacters().Equals(cleanName)))
            foreach (EditorColorInfo colorInfo in Colors.Where(item => item.ColorName.Equals(cleanName)))
                return colorInfo.color;

            if (!silent) Debug.LogWarning($"Color '{colorName}' not found! Returned Color.magenta");

            return Color.magenta;
        }

        internal void Validate()
        {
            string assetPath = AssetDatabase.GetAssetPath(this);
            if (Colors == null || Colors.Count == 0)
            {
                // AssetDatabase.MoveAssetToTrash(assetPath);
                return;
            }
            
            PaletteName = PaletteName.RemoveWhitespaces().RemoveAllSpecialCharacters();
            
            foreach (EditorColorInfo colorInfo in Colors)
                colorInfo.ValidateName();

            AssetDatabase.RenameAsset(assetPath, AssetFileName(paletteName));
            EditorUtility.SetDirty(this);
        }

        public static void RegenerateDefaultColorPalette()
        {
            const string defaultColorPaletteName = "EditorUI";
            string defaultColorPalettePath = $"{EditorPath.path}/EditorUI/Colors/{AssetFileName(defaultColorPaletteName)}.asset";

            EditorDataColorPalette data = AssetDatabase.LoadAssetAtPath<EditorDataColorPalette>(defaultColorPalettePath);
            bool defaultPaletteNotFound = data == null;
            if (defaultPaletteNotFound) data = CreateInstance<EditorDataColorPalette>();
            data.PaletteName = defaultColorPaletteName;
            data.name = AssetFileName(defaultColorPaletteName);
            data.Colors = new List<EditorColorInfo>
            {
                new EditorColorInfo("Amber", new Color(1f, 0.77f, 0f), new Color(0.6f, 0.43f, 0f)),
                new EditorColorInfo("Black", Color.black, Color.black),
                new EditorColorInfo("White", Color.white, Color.white),
                new EditorColorInfo("Blue", new Color(0.38f, 0.57f, 0.98f), new Color(0.23f, 0.33f, 0.6f)),
                new EditorColorInfo("Cyan", new Color(0.55f, 0.89f, 1f), new Color(0.29f, 0.46f, 0.45f)),
                new EditorColorInfo("Deep Orange", new Color(0.86f, 0.36f, 0.33f), new Color(0.55f, 0.18f, 0.05f)),
                new EditorColorInfo("Deep Purple", new Color(0.52f, 0.44f, 0.98f), new Color(0.22f, 0.16f, 0.6f)),
                new EditorColorInfo("Gray", new Color(0.35f, 0.35f, 0.35f), new Color(0.54f, 0.54f, 0.54f)),
                new EditorColorInfo("Green", new Color(0.55f, 0.89f, 0.51f), new Color(0.31f, 0.5f, 0.27f)),
                new EditorColorInfo("Indigo", new Color(0.44f, 0.53f, 0.98f), new Color(0.22f, 0.27f, 0.6f)),
                new EditorColorInfo("Light Blue", new Color(0.44f, 0.7f, 0.99f), new Color(0.28f, 0.46f, 0.6f)),
                new EditorColorInfo("Light Green", new Color(0.76f, 0.98f, 0.29f), new Color(0.38f, 0.5f, 0.15f)),
                new EditorColorInfo("Lime", new Color(0.9f, 0.99f, 0.33f), new Color(0.48f, 0.51f, 0.14f)),
                new EditorColorInfo("Orange", new Color(0.89f, 0.58f, 0.16f), new Color(0.54f, 0.33f, 0.09f)),
                new EditorColorInfo("Pink", new Color(0.87f, 0.44f, 0.64f), new Color(0.52f, 0.23f, 0.4f)),
                new EditorColorInfo("Purple", new Color(0.71f, 0.33f, 0.94f), new Color(0.42f, 0.14f, 0.6f)),
                new EditorColorInfo("Red", new Color(0.85f, 0.31f, 0.45f), new Color(0.51f, 0.14f, 0.17f)),
                new EditorColorInfo("Teal", new Color(0.56f, 0.9f, 0.73f), new Color(0.31f, 0.51f, 0.4f)),
                new EditorColorInfo("Yellow", new Color(0.97f, 0.91f, 0.25f), new Color(0.53f, 0.48f, 0.13f)),
            };
            data.SortByHue();

            if (defaultPaletteNotFound)
            {
                AssetDatabase.CreateAsset(data, defaultColorPalettePath);
            }

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();

            if (defaultPaletteNotFound) AssetDatabase.Refresh();
        }
    }

}
