// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Doozy.Editor.Common.Utils;
using Doozy.Editor.EditorUI.ScriptableObjects.Styles;
using Doozy.Runtime.Common.Extensions;
using UnityEditor;

namespace Doozy.Editor.EditorUI.Automation.Generators
{
    internal static class EditorStylesGenerator
    {
        // [InitializeOnLoadMethod]
        // private static void Tester() => Run();
        
        private static string templateName => nameof(EditorStylesGenerator).Replace("Generator", "");
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

        private static string styleGroupTemplateFilePath => $"{EditorPath.path}/EditorUI/Automation/Templates/{nameof(EditorDataStyleGroup)}.cst";

        private static string InjectContent(string data)
        {
            var groups = EditorDataStyleDatabase.instance.Database.ToList();
            groups = groups.Distinct().ToList();
            groups = groups.OrderBy(item => item.groupName).ToList();
            
            var contentStringBuilder = new StringBuilder();
            for (int groupIndex = 0; groupIndex < groups.Count; groupIndex++)
            {
                EditorDataStyleGroup styleGroup = groups[groupIndex];

                string groupName =
                    styleGroup.groupName
                        .RemoveAllSpecialCharacters()
                        .RemoveWhitespaces();
                
                if(groupName.IsNullOrEmpty()) continue;

                string groupData = FileGenerator.GetFile(styleGroupTemplateFilePath);
                groupData = groupData.Replace("//GROUP_NAME//", groupName);

                var styleNames = new List<string>();
                foreach (EditorStyleInfo styleInfo in styleGroup.styles)
                {
                    string styleName =
                        styleInfo.UssReference.name
                            .RemoveAllSpecialCharacters()
                            .RemoveWhitespaces();
                    
                    if(styleName.IsNullOrEmpty()) continue;
                    styleNames.Add(styleName);
                }

                styleNames = styleNames.Distinct().ToList();
                styleNames.Sort();

                var namesStringBuilder = new StringBuilder();
                var cacheStringBuilder = new StringBuilder().AppendLine();
                for (int i = 0; i < styleNames.Count; i++)
                {
                    string styleName = styleNames[i];
                    namesStringBuilder.AppendLine($"                {styleName}{(i < styleNames.Count - 1 ? "," : "")}");
                    
                    cacheStringBuilder.AppendLine($"            private static StyleSheet s_{styleName};");
                    cacheStringBuilder.AppendLine($"            public static StyleSheet {styleName} => s_{styleName} ? s_{styleName} : s_{styleName} = GetStyleSheet(StyleName.{styleName});");
                }

                groupData = groupData.Replace("//NAMES//", namesStringBuilder.ToString().RemoveAllEmptyLines());
                groupData = groupData.Replace("//CACHE//", cacheStringBuilder.ToString().RemoveAllEmptyLines());

                contentStringBuilder.AppendLine(groupData);
                if (groupIndex < groups.Count - 1)
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
