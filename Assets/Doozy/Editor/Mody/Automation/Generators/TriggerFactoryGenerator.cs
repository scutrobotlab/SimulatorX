// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Text;
using Doozy.Editor.Common.Utils;
using Doozy.Runtime;
using Doozy.Runtime.Common.Extensions;
using UnityEditor;

namespace Doozy.Editor.Mody.Automation.Generators
{
    internal static class TriggerFactoryGenerator
    {
        // [InitializeOnLoadMethod]
        // private static void Tester() => Run();

        private static string templateName => nameof(TriggerFactoryGenerator).Replace("Generator", "");
        private static string templateNameWithExtension => $"{templateName}.cst";
        private static string templateFilePath => $"{EditorPath.path}/Mody/Automation/Templates/{templateNameWithExtension}";

        private static string targetFileNameWithExtension => $"{templateName}.cs";
        private static string targetFilePath => $"{RuntimePath.path}/Mody/{targetFileNameWithExtension}";

        private static string InjectContent(string data)
        {
            var localTriggers = new StringBuilder();
            var globalTriggers = new StringBuilder();
            return
                data
                    .Replace("//LOCAL_TRIGGERS//", localTriggers.ToString())
                    .Replace("//GLOBAL_TRIGGERS//", globalTriggers.ToString());
        }

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
    }
}
