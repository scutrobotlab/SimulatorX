// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Linq;
using System.Text;
using Doozy.Editor.Common.Utils;
using Doozy.Editor.EditorUI.ScriptableObjects.Fonts;
using Doozy.Runtime.Common.Extensions;
using UnityEditor;

namespace Doozy.Editor.EditorUI.Automation.Generators
{
    internal static class EditorFontsGenerator
    {
        // [InitializeOnLoadMethod]
        // private static void Tester() => Run();

        private static string templateName => nameof(EditorFontsGenerator).Replace("Generator", "");
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

        private static string fontFamilyTemplateFilePath => $"{EditorPath.path}/EditorUI/Automation/Templates/{nameof(EditorDataFontFamily)}.cst";

        private readonly struct WeightInfo
        {
            public readonly string Name;
            public readonly int Value;
            public WeightInfo(string name, int value)
            {
                Name = name;
                Value = value;
            }
        }

        private static string InjectContent(string data)
        {
            var families = EditorDataFontDatabase.instance.Database.ToList();
            families = families.Distinct().ToList();
            families = families.OrderBy(fi => fi.fontName).ToList();

            var contentStringBuilder = new StringBuilder();
            for (int index = 0; index < families.Count; index++)
            {
                string fontName = families[index].fontName.RemoveAllSpecialCharacters().RemoveWhitespaces();
                if (fontName.IsNullOrEmpty()) continue;

                string familyData = FileGenerator.GetFile(fontFamilyTemplateFilePath);
                familyData = familyData.Replace("//FONT_NAME//", fontName);

                var weightInfoList = families[index].fonts.Select(fontInfo => new WeightInfo(fontInfo.Weight.ToString(), (int)fontInfo.Weight)).ToList();
                weightInfoList = weightInfoList.OrderBy(w => w.Value).ToList();

                var weightsStringBuilder = new StringBuilder();
                var cacheStringBuilder = new StringBuilder().AppendLine();
                {
                    for (int i = 0; i < weightInfoList.Count; i++)
                    {
                        WeightInfo info = weightInfoList[i];
                        weightsStringBuilder.AppendLine($"                {info.Name} = {info.Value}{(i < weightInfoList.Count - 1 ? "," : "")}");

                        cacheStringBuilder.AppendLine($"            private static Font s_{info.Name};");
                        cacheStringBuilder.AppendLine($"            public static Font {info.Name} => s_{info.Name} ? s_{info.Name} : s_{info.Name} = GetFont(FontWeight.{info.Name});");
                    }

                    familyData = familyData.Replace("//WEIGHTS//", weightsStringBuilder.ToString().RemoveAllEmptyLines());
                    familyData = familyData.Replace("//CACHE//", cacheStringBuilder.ToString());
                }

                contentStringBuilder.Append(familyData);

                contentStringBuilder.AppendLine();
                if (index < families.Count - 1)
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
