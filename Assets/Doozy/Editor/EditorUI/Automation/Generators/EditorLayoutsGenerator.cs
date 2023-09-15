// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Doozy.Editor.Common.Utils;
using Doozy.Editor.EditorUI.ScriptableObjects.Layouts;
using Doozy.Runtime.Common.Extensions;
using UnityEditor;

namespace Doozy.Editor.EditorUI.Automation.Generators
{
    internal static class EditorLayoutsGenerator
    {
        // [InitializeOnLoadMethod]
        // private static void Tester() => Run();
        
        private static string templateName => nameof(EditorLayoutsGenerator).Replace("Generator", "");
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

        private static string layoutGroupTemplateFilePath => $"{EditorPath.path}/EditorUI/Automation/Templates/{nameof(EditorDataLayoutGroup)}.cst";
        
        private static string InjectContent(string data)
        {
            var groups = EditorDataLayoutDatabase.instance.Database.ToList();
            groups = groups.Distinct().ToList();
            groups = groups.OrderBy(item => item.groupName).ToList();

            var contentStringBuilder = new StringBuilder();
            for (int groupIndex = 0; groupIndex < groups.Count; groupIndex++)
            {
                EditorDataLayoutGroup layoutGroup = groups[groupIndex];

                string groupName =
                    layoutGroup.groupName
                        .RemoveAllSpecialCharacters()
                        .RemoveWhitespaces();
                
                if(groupName.IsNullOrEmpty()) continue;

                string groupData = FileGenerator.GetFile(layoutGroupTemplateFilePath);
                groupData = groupData.Replace("//GROUP_NAME//", groupName);

                var layoutNames = new List<string>();
                foreach (EditorLayoutInfo layoutInfo in layoutGroup.layouts)
                {
                    string layoutName = layoutInfo.UxmlReference.name.RemoveAllSpecialCharacters().RemoveWhitespaces();
                    if(layoutName.IsNullOrEmpty()) continue;
                    layoutNames.Add(layoutName);
                }

                layoutNames = layoutNames.Distinct().ToList();
                layoutNames.Sort();

                var namesStringBuilder = new StringBuilder();
                var cacheStringBuilder = new StringBuilder().AppendLine();
                for (int i = 0; i < layoutNames.Count; i++)
                {
                    string layoutName = layoutNames[i];
                    namesStringBuilder.AppendLine($"                {layoutName}{(i < layoutNames.Count - 1 ? "," : "")}");
                    
                    cacheStringBuilder.AppendLine($"            private static VisualTreeAsset s_{layoutName};");
                    cacheStringBuilder.AppendLine($"            public static VisualTreeAsset {layoutName} => s_{layoutName} ? s_{layoutName} : s_{layoutName} = GetVisualTreeAsset(LayoutName.{layoutName});");
                }

                groupData = groupData.Replace("//NAMES//", namesStringBuilder.ToString().RemoveAllEmptyLines());
                groupData = groupData.Replace("//CACHE//", cacheStringBuilder.ToString());

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
