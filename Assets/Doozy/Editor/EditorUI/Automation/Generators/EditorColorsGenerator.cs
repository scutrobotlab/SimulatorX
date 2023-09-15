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
    internal static class EditorColorsGenerator
    {
        // [InitializeOnLoadMethod]
        // private static void Tester() => Run();

        private static string templateName => nameof(EditorColorsGenerator).Replace("Generator", "");
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

        private static string colorPaletteTemplateFilePath =>
            $"{EditorPath.path}/EditorUI/Automation/Templates/{nameof(EditorDataColorPalette)}.cst";

        private static string InjectContent(string data)
        {
            var palettes = EditorDataColorDatabase.instance.Database.ToList();
            palettes = palettes.Distinct().ToList();
            palettes = palettes.OrderBy(item => item.paletteName).ToList();

            var contentStringBuilder = new StringBuilder();
            for (int paletteIndex = 0; paletteIndex < palettes.Count; paletteIndex++)
            {
                EditorDataColorPalette colorPalette = palettes[paletteIndex];

                string paletteName =
                    colorPalette.paletteName
                        .RemoveAllSpecialCharacters()
                        .RemoveWhitespaces();

                if (paletteName.IsNullOrEmpty()) continue;

                string paletteData = FileGenerator.GetFile(colorPaletteTemplateFilePath);
                paletteData = paletteData.Replace("//PALETTE_NAME//", paletteName);

                var colorNames = new List<string>();
                foreach (EditorColorInfo colorInfo in colorPalette.colors)
                {
                    string colorName =
                        colorInfo.ColorName
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

                    cacheStringBuilder.AppendLine($"            private static Color? s_{colorName};");
                    cacheStringBuilder.AppendLine($"            public static Color {colorName} => (Color) (s_{colorName} ?? (s_{colorName} = GetColor(ColorName.{colorName})));");
                }

                paletteData = paletteData.Replace("//NAMES//", namesStringBuilder.ToString().RemoveAllEmptyLines());
                paletteData = paletteData.Replace("//CACHE//", cacheStringBuilder.ToString());

                contentStringBuilder.Append(paletteData);
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
