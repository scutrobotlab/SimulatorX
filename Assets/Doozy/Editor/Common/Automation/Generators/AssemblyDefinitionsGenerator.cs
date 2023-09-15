// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Common.Utils;
using Doozy.Runtime;
using UnityEditor;

namespace Doozy.Editor.Common.Automation.Generators
{
    public static class AssemblyDefinitionsGenerator
    {
        // [InitializeOnLoadMethod]
        // private static void Tester() => Run();

        private static string doozyEditorTemplateName => "Doozy.Editor";
        private static string doozyRuntimeTemplateName => "Doozy.Runtime";

        private static string doozyEditorTemplateNameWithExtension => $"{doozyEditorTemplateName}.txt";
        private static string doozyRuntimeTemplateNameWithExtension => $"{doozyRuntimeTemplateName}.txt";

        private static string doozyEditorTemplateFilePath => $"{EditorPath.path}/Common/Automation/Templates/{doozyEditorTemplateNameWithExtension}";
        private static string doozyRuntimeTemplateFilePath => $"{EditorPath.path}/Common/Automation/Templates/{doozyRuntimeTemplateNameWithExtension}";

        private static string doozyEditorTargetFileNameWithExtension => $"{doozyEditorTemplateName}.asmdef";
        private static string doozyRuntimeTargetFileNameWithExtension => $"{doozyRuntimeTemplateName}.asmdef";

        private static string doozyEditorTargetFilePath => $"{EditorPath.path}/{doozyEditorTargetFileNameWithExtension}";
        private static string doozyRuntimeTargetFilePath => $"{RuntimePath.path}/{doozyRuntimeTargetFileNameWithExtension}";

        internal static bool Run(bool saveAssets = true, bool refreshAssetDatabase = false)
        {
            if (!AssetExists(doozyEditorTargetFilePath))
                FileGenerator.WriteFile(doozyEditorTargetFilePath, FileGenerator.GetFile(doozyEditorTemplateFilePath));

            if (!AssetExists(doozyRuntimeTargetFilePath))
                FileGenerator.WriteFile(doozyRuntimeTargetFilePath, FileGenerator.GetFile(doozyRuntimeTemplateFilePath));

            if (saveAssets) AssetDatabase.SaveAssets();
            if (refreshAssetDatabase) AssetDatabase.Refresh();
            return true;
        }

        private static bool AssetExists(string assetPath) =>
            AssetDatabase.GetMainAssetTypeAtPath(assetPath) != null;
    }
}
