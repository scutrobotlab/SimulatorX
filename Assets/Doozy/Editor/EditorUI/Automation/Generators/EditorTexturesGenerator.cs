// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Doozy.Editor.Common.Utils;
using Doozy.Editor.EditorUI.ScriptableObjects.Textures;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Extensions;
using UnityEditor;

namespace Doozy.Editor.EditorUI.Automation.Generators
{
    public static class EditorTexturesGenerator
    {
        // [InitializeOnLoadMethod]
        // private static void Tester() => Run();

        private static string templateName => nameof(EditorTexturesGenerator).Replace("Generator", "");
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

        private static string textureGroupTemplateFilePath => $"{EditorPath.path}/EditorUI/Automation/Templates/{nameof(EditorDataTextureGroup)}.cst";

        private static string InjectContent(string data)
        {
            var groups = EditorDataTextureDatabase.instance.Database.ToList();
            groups = groups.Distinct().ToList();
            groups = groups.OrderBy(tg => tg.groupName).ToList();

            foreach (EditorDataTextureGroup textureGroup in groups)
            {
                if (textureGroup.groupCategory.IsNullOrEmpty())
                {
                    Debugger.LogWarning(
                        $"The '{textureGroup.name}' {nameof(EditorDataTextureGroup)} does not have any category assigned. " +
                        $"Please set a Group Category value. The asset file is located at: {AssetDatabase.GetAssetPath(textureGroup)}");
                }
            }

            groups = groups.Where(tg => !tg.groupCategory.IsNullOrEmpty()).ToList();
            groups = groups.OrderBy(tg => tg.groupCategory).ThenBy(tg => tg.groupName).ToList();

            var categories = groups.Select(textureGroup => textureGroup.groupCategory).ToList();
            categories = categories.Distinct().ToList();
            categories.Sort();

            var contentStringBuilder = new StringBuilder();
            for (int categoryIndex = 0; categoryIndex < categories.Count; categoryIndex++)
            {
                string category = categories[categoryIndex];

                string groupCategory =
                    category
                        .RemoveAllSpecialCharacters()
                        .RemoveWhitespaces();

                if (groupCategory.IsNullOrEmpty()) continue;

                var categoryStringBuilder = new StringBuilder();
                categoryStringBuilder.AppendLine($"        public static class {groupCategory}");
                categoryStringBuilder.AppendLine("        {");
                {
                    for (int groupIndex = 0; groupIndex < groups.Count; groupIndex++)
                    {
                        EditorDataTextureGroup textureGroup = groups[groupIndex];
                        if (!textureGroup.groupCategory.Equals(category))
                            continue;

                        string groupName =
                            textureGroup.groupName
                                .RemoveAllSpecialCharacters()
                                .RemoveWhitespaces();

                        if (groupName.IsNullOrEmpty()) continue;
                        string groupData = FileGenerator.GetFile(textureGroupTemplateFilePath);
                        groupData = groupData
                            .Replace("//GROUP_CATEGORY//", groupCategory)
                            .Replace("//GROUP_NAME//", groupName);

                        var textureNames = new List<string>();
                        foreach (EditorTextureInfo textureInfo in textureGroup.textures)
                        {
                            if (textureInfo.TextureReference == null) continue;
                            if (textureInfo.TextureName.IsNullOrEmpty()) continue;
                            textureNames.Add(textureInfo.TextureName);
                        }

                        var namesStringBuilder = new StringBuilder();
                        var cacheStringBuilder = new StringBuilder().AppendLine();
                        for (int i = 0; i < textureNames.Count; i++)
                        {
                            string textureName = textureNames[i];
                            namesStringBuilder.AppendLine($"                    {textureName}{(i < textureNames.Count - 1 ? "," : "")}");
                            
                            cacheStringBuilder.AppendLine($"                private static Texture2D s_{textureName};");
                            cacheStringBuilder.AppendLine($"                public static Texture2D {textureName} => s_{textureName} ? s_{textureName} : s_{textureName} = GetTexture2D(TextureName.{textureName});");
                        }

                        groupData = groupData.Replace("//NAMES//", namesStringBuilder.ToString().RemoveAllEmptyLines());
                        groupData = groupData.Replace("//CACHE//", cacheStringBuilder.ToString());

                        categoryStringBuilder.Append(groupData);
                        if (groupIndex < groups.Count - 1)
                        {
                            categoryStringBuilder.AppendLine();
                            categoryStringBuilder.AppendLine();
                        }
                    }
                }
                categoryStringBuilder.AppendLine();
                categoryStringBuilder.AppendLine("        }");
                contentStringBuilder.Append(categoryStringBuilder);
                contentStringBuilder.AppendLine();
                contentStringBuilder.AppendLine();
            }

            data = data.Replace("//CONTENT//", contentStringBuilder.ToString());

            return data;
        }
    }
}
