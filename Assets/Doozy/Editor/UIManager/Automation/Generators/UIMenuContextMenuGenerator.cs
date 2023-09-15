// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Text;
using Doozy.Editor.Common.Utils;
using Doozy.Editor.UIManager.UIMenu;
using Doozy.Runtime.Common.Extensions;
using UnityEditor;

namespace Doozy.Editor.UIManager.Automation.Generators
{
    public static class UIMenuContextMenuGenerator
    {
        private static string templateName => nameof(UIMenuContextMenuGenerator).Replace("Generator", "");
        private static string templateNameWithExtension => $"{templateName}.cst";
        private static string templateFilePath => $"{EditorPath.path}/UIManager/Automation/Templates/{templateNameWithExtension}";

        private static string targetFileNameWithExtension => $"{templateName}.cs";
        private static string targetFilePath => $"{EditorPath.path}/UIManager/UIMenu/{targetFileNameWithExtension}";

        public static bool Run(bool saveAssets = true, bool refreshAssetDatabase = false, bool silent = false)
        {
            string data = FileGenerator.GetFile(templateFilePath);
            if (!UIMenuItemsDatabase.instance.isEmpty)
                data = InjectContent(data);
            bool result = FileGenerator.WriteFile(targetFilePath, data, silent);
            if (!result) return false;
            if (saveAssets) AssetDatabase.SaveAssets();
            if (refreshAssetDatabase) AssetDatabase.Refresh();
            return true;
        }

        private static string InjectContent(string templateData)
        {
            var menuStringBuilder = new StringBuilder();

            List<string> listOfPrefabTypeNames = UIMenuItemsDatabase.GetPrefabTypes();
            foreach (string prefabTypeName in listOfPrefabTypeNames)
            {
                string cleanPrefabTypeName = prefabTypeName.RemoveWhitespaces().RemoveAllSpecialCharacters();
                menuStringBuilder.AppendLine();
                menuStringBuilder.AppendLine($"        public static class {cleanPrefabTypeName}");
                menuStringBuilder.AppendLine("        {");
                menuStringBuilder.AppendLine($"            private const string TYPE_NAME = \"{prefabTypeName}\";");
                menuStringBuilder.AppendLine($"            private const string TYPE_MENU_PATH = MENU_PATH + \"/\" + TYPE_NAME + \"/\";");

                List<string> listOfPrefabCategories = UIMenuItemsDatabase.GetCategories(prefabTypeName);
                foreach (string prefabCategory in listOfPrefabCategories)
                {
                    string cleanPrefabCategory = prefabCategory.RemoveWhitespaces().RemoveAllSpecialCharacters();
                    menuStringBuilder.AppendLine();
                    menuStringBuilder.AppendLine($"            public static class {cleanPrefabCategory}");
                    menuStringBuilder.AppendLine("            {");
                    menuStringBuilder.AppendLine($"                private const string CATEGORY_NAME = \"{prefabCategory}\";");
                    menuStringBuilder.AppendLine($"                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + \"/\";");

                    List<string> listOfPrefabNames = UIMenuItemsDatabase.GetPrefabNames(prefabTypeName, cleanPrefabCategory);
                    foreach (string prefabName in listOfPrefabNames)
                    {
                        string cleanPrefabName = prefabName.RemoveWhitespaces().RemoveAllSpecialCharacters();

                        menuStringBuilder.AppendLine();
                        menuStringBuilder.AppendLine($"                [MenuItem(CATEGORY_MENU_PATH + \"{prefabName}\", false, MENU_ITEM_PRIORITY)]");
                        menuStringBuilder.AppendLine($"                public static void Create{cleanPrefabName}(MenuCommand command) => {nameof(UIMenuUtils)}.{nameof(UIMenuUtils.AddToScene)}(TYPE_NAME, CATEGORY_NAME, \"{cleanPrefabName}\");");
                    }

                    menuStringBuilder.AppendLine("            }");
                }

                menuStringBuilder.AppendLine("        }");
            }

            templateData = templateData.Replace("//MENU//", menuStringBuilder.ToString().RemoveLast(Environment.NewLine.Length));
            return templateData;
        }
    }
}
