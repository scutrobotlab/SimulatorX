// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Doozy.Editor.Common.Utils;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Runtime.Common.Extensions;
using UnityEditor;

namespace Doozy.Editor.EditorUI.Automation.Generators
{
    internal static class EditorSelectableColorsGenerator
    {
        // [InitializeOnLoadMethod]
        // private static void Tester() => Run();

        private static string templateName => nameof(EditorSelectableColorsGenerator).Replace("Generator", "");
        private static string templateNameWithExtension => $"{templateName}.cst";
        private static string templateFilePath => $"{EditorPath.path}/EditorUI/Automation/Templates/{templateNameWithExtension}";

        private static string targetFileNameWithExtension => $"{templateName}.cs";
        private static string targetFilePath => $"{EditorPath.path}/EditorUI/{targetFileNameWithExtension}";

        internal static bool Run(bool saveAssets = true, bool refreshAssetDatabase = false)
        {
            string data = FileGenerator.GetFile(templateFilePath);
            if (data.IsNullOrEmpty()) return false;
            data = InjectContent(data);
            bool result = FileGenerator.WriteFile(targetFilePath, data);
            if (!result) return false;
            if (saveAssets) AssetDatabase.SaveAssets();
            if (refreshAssetDatabase) AssetDatabase.Refresh();
            return true;
        }

        private static string selectableColorPaletteTemplateFilePath =>
            $"{EditorPath.path}/EditorUI/Automation/Templates/{nameof(EditorDataSelectableColorPalette)}.cst";

        private static string InjectContent(string data)
        {
            var palettes = EditorDataSelectableColorDatabase.instance.Database.ToList();
            palettes = palettes.Distinct().ToList();
            palettes = palettes.OrderBy(item => item.paletteName).ToList();

            var contentStringBuilder = new StringBuilder();
            for (int paletteIndex = 0; paletteIndex < palettes.Count; paletteIndex++)
            {
                EditorDataSelectableColorPalette selectableColorPalette = palettes[paletteIndex];

                string paletteName =
                    selectableColorPalette.paletteName
                        .RemoveAllSpecialCharacters()
                        .RemoveWhitespaces();

                if (paletteName.IsNullOrEmpty()) continue;

                string selectablePaletteData = FileGenerator.GetFile(selectableColorPaletteTemplateFilePath);
                selectablePaletteData = selectablePaletteData.Replace("//PALETTE_NAME//", paletteName);

                var colorNames = new List<string>();
                foreach (EditorSelectableColorInfo selectableColorInfo in selectableColorPalette.selectableColors)
                {
                    string colorName =
                        selectableColorInfo.ColorName
                            .RemoveAllSpecialCharacters()
                            .RemoveWhitespaces();

                    if (colorName.IsNullOrEmpty()) continue;
                    colorNames.Add(colorName);
                }

                colorNames = colorNames.Distinct().ToList();
                colorNames.Sort();

                var namesStringBuilder = new StringBuilder();
                var cacheStringBuilder = new StringBuilder().AppendLine();
                for (int i = 0; i < colorNames.Count; i++)
                {
                    string colorName = colorNames[i];
                    namesStringBuilder.AppendLine($"                {colorName}{(i < colorNames.Count - 1 ? "," : "")}");

                    cacheStringBuilder.AppendLine($"            private static EditorSelectableColorInfo s_{colorName};");
                    cacheStringBuilder.AppendLine($"            public static EditorSelectableColorInfo {colorName} => s_{colorName} ?? (s_{colorName} = GetSelectableColorInfo(ColorName.{colorName}));");
                }

                selectablePaletteData = selectablePaletteData.Replace("//NAMES//", namesStringBuilder.ToString().RemoveAllEmptyLines());
                selectablePaletteData = selectablePaletteData.Replace("//CACHE//", cacheStringBuilder.ToString());

                contentStringBuilder.Append(selectablePaletteData);
                if (paletteIndex < palettes.Count - 1)
                {
                    contentStringBuilder.AppendLine();
                    contentStringBuilder.AppendLine();
                }
            }

            data = data.Replace("//CONTENT//", contentStringBuilder.ToString());

            return data;
        }
    }
}
