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
            menuName = "Doozy/EditorUI/Selectable Color Palette"
        )
    ]
    public class EditorDataSelectableColorPalette : ScriptableObject
    {
        private const string DEFAULT_ASSET_FILENAME = "_SelectableColorPalette";
        private static string AssetFileName(string paletteName) => $"{DEFAULT_ASSET_FILENAME}_{paletteName.RemoveWhitespaces()}";

        [SerializeField] private string PaletteName;
        internal string paletteName => PaletteName;

        [SerializeField] private List<EditorSelectableColorInfo> SelectableColors = new List<EditorSelectableColorInfo>();
        internal List<EditorSelectableColorInfo> selectableColors => SelectableColors;

        internal void AddNewItem() =>
            SelectableColors.Insert(0, new EditorSelectableColorInfo());

        internal void SortByColorName() =>
            SelectableColors = SelectableColors.OrderBy(item => item.ColorName).ToList();

        internal void SortByHue() =>
            SelectableColors = SelectableColors.OrderByDescending(item => item.Normal.ColorOnDark.Hue()).ToList();

        internal EditorSelectableColorInfo GetSelectableColorInfo(string colorName, bool silent = false)
        {
            string cleanName = colorName.RemoveWhitespaces().RemoveAllSpecialCharacters();

            SelectableColors = SelectableColors.Where(item => item != null).ToList();

            // foreach (EditorSelectableColorInfo selectableColorInfo in SelectableColors.Where(item => item.ColorName.RemoveWhitespaces().RemoveAllSpecialCharacters().Equals(cleanName)))
            foreach (EditorSelectableColorInfo selectableColorInfo in SelectableColors.Where(item => item.ColorName.Equals(cleanName)))
                return selectableColorInfo;

            if (!silent)
            {
                Debug.LogWarning($"SelectableColor '{colorName}' not found! Returned null");
            }

            return null;
        }

        internal Color GetColor(string colorName, SelectionState state)
        {
            EditorSelectableColorInfo selectableColorInfo = GetSelectableColorInfo(colorName);
            if (selectableColorInfo != null)
                return selectableColorInfo.GetColor(state);
            Debug.LogWarning($"SelectableColor '{colorName}' not found! Returned Color.magenta");
            return Color.magenta;
        }

        internal EditorThemeColor GetThemeColor(string colorName, SelectionState state)
        {
            EditorSelectableColorInfo selectableColorInfo = GetSelectableColorInfo(colorName);
            if (selectableColorInfo != null)
                return selectableColorInfo.GetThemeColor(state);
            Debug.LogWarning($"SelectableColor '{colorName}' not found! Returned null");
            return null;
        }

        internal void Validate()
        {
            string assetPath = AssetDatabase.GetAssetPath(this);
            if (SelectableColors == null || SelectableColors.Count == 0)
            {
                // AssetDatabase.MoveAssetToTrash(assetPath);
                return;
            }

            PaletteName = PaletteName.RemoveWhitespaces().RemoveAllSpecialCharacters();

            foreach (EditorSelectableColorInfo selectableColorInfo in SelectableColors)
                selectableColorInfo.ValidateName();

            AssetDatabase.RenameAsset(assetPath, AssetFileName(paletteName));
            EditorUtility.SetDirty(this);
        }

        public static void RegenerateDefaultSelectableColorPalette()
        {
            const string defaultColorPaletteName = "EditorUI";
            string defaultColorPalettePath = $"{EditorPath.path}/EditorUI/Colors/{AssetFileName(defaultColorPaletteName)}.asset";

            EditorDataSelectableColorPalette data = AssetDatabase.LoadAssetAtPath<EditorDataSelectableColorPalette>(defaultColorPalettePath);
            bool defaultPaletteNotFound = data == null;
            if (!defaultPaletteNotFound) return;
            data = CreateInstance<EditorDataSelectableColorPalette>();
            data.PaletteName = defaultColorPaletteName;
            data.name = AssetFileName(defaultColorPaletteName);
            data.SelectableColors = new List<EditorSelectableColorInfo>();
            EditorDataColorPalette defaultColorPalette = EditorDataColorDatabase.GetColorPalette("EditorUI");
            foreach (EditorColorInfo colorInfo in defaultColorPalette.colors)
            {
                if (colorInfo.ColorName.Equals("Black")) continue;
                if (colorInfo.ColorName.Equals("White")) continue;
                if (colorInfo.ColorName.Equals("Gray")) continue;
                data.SelectableColors.Add
                (
                    new EditorSelectableColorInfo
                        {
                            ColorName = colorInfo.ColorName.RemoveWhitespaces().RemoveAllSpecialCharacters(),
                            Normal = new EditorThemeColor
                            {
                                ColorOnDark = colorInfo.ThemeColor.ColorOnDark,
                                ColorOnLight = colorInfo.ThemeColor.ColorOnLight
                            }
                        }
                        .GenerateAllColorVariantsFromNormalColor()
                );
            }
            data.SortByHue();

            AssetDatabase.CreateAsset(data, defaultColorPalettePath);
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
