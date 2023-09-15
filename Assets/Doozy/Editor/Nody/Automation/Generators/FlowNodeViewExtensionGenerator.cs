// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Doozy.Editor.Common.Utils;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Nody;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Nody.Automation.Generators
{
    public static class FlowNodeViewExtensionGenerator
    {
        private static string templateName => nameof(FlowNodeViewExtensionGenerator).Replace("Generator", "");
        private static string templateNameWithExtension => $"{templateName}.cst";
        private static string templateFilePath => $"{EditorPath.path}/Nody/Automation/Templates/{templateNameWithExtension}";

        private static string targetFileNameWithExtension => $"{templateName}.cs";
        private static string targetFilePath => $"{EditorPath.path}/Nody/{targetFileNameWithExtension}";

        public static bool Run(bool saveAssets = true, bool refreshAssetDatabase = false, bool silent = false)
        {
            string data = FileGenerator.GetFile(templateFilePath);
            data = InjectContent(data);
            bool result = FileGenerator.WriteFile(targetFilePath, data, silent);
            if (!result) return false;
            if (saveAssets) AssetDatabase.SaveAssets();
            if (refreshAssetDatabase) AssetDatabase.Refresh();
            return true;
        }

        private static string InjectContent(string templateData)
        {
            var nodesStringBuilder = new StringBuilder();
            IEnumerable<Type> nodeTypeCollection = TypeCache.GetTypesDerivedFrom<FlowNode>().Where(t => !t.IsAbstract);
            IEnumerable<Type> nodeViewTypeCollection = TypeCache.GetTypesDerivedFrom<FlowNodeView>().Where(t => !t.IsAbstract);

            foreach (Type nodeType in nodeTypeCollection)
            {
                string nodeTypeFullName = nodeType.FullName;
                string nodeViewTypeFullName = string.Empty;

                foreach (Type nodeViewType in nodeViewTypeCollection)
                {
                    if (nodeViewType.Name.Equals($"{nodeType.Name}View"))
                        nodeViewTypeFullName = nodeViewType.FullName;
                }

                if (nodeViewTypeFullName.IsNullOrEmpty())
                {
                    Debug.LogWarning
                    (
                        $"Could not find the '{nameof(FlowNodeView)}' node view for the '{nodeType.Name}' node. " +
                        $"Searching for '{nodeType.Name}View' failed so the node type was not added to Nody." +
                        $"To fix this, create a node view for the '{nodeType.Name}' node and name it '{nodeType.Name}View'"
                    );
                    continue;
                }

                nodesStringBuilder.AppendLine($"                {nodeTypeFullName} _ => new {nodeViewTypeFullName}(graphView, node),");
            }
            templateData = templateData.Replace("//NODES//", nodesStringBuilder.ToString().RemoveLast(Environment.NewLine.Length));
            return templateData;
        }
    }
}
