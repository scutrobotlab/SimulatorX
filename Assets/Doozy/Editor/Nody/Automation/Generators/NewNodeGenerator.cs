// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.Common.Utils;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Nody;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Nody.Automation.Generators
{
    public static class NewNodeGenerator
    {
        private static string templateNewNode => nameof(NewNodeGenerator).Replace("Generator", "");
        private static string templateNewNodeWithExtension => $"{templateNewNode}.cst";
        private static string templateNewNodeFilePath => $"{EditorPath.path}/Nody/Automation/Templates/{templateNewNodeWithExtension}";

        private static string templateNewNodeEditor => nameof(NewNodeGenerator).Replace("Generator", "Editor");
        private static string templateNewNodeEditorWithExtension => $"{templateNewNodeEditor}.cst";
        private static string templateNewNodeEditorFilePath => $"{EditorPath.path}/Nody/Automation/Templates/{templateNewNodeEditorWithExtension}";

        private static string templateNewNodeView => nameof(NewNodeGenerator).Replace("Generator", "View");
        private static string templateNewNodeViewWithExtension => $"{templateNewNodeView}.cst";
        private static string templateNewNodeViewFilePath => $"{EditorPath.path}/Nody/Automation/Templates/{templateNewNodeViewWithExtension}";

        private static string GetNodeName(string name) =>
            name.RemoveWhitespaces().RemoveAllSpecialCharacters().Replace("NodeNode", "Node").AppendSuffixIfMissing("Node");

        private static string GetNodeEditorName(string name) =>
            GetNodeName(name).AppendSuffixIfMissing("Editor");

        private static string GetNodeViewName(string name) =>
            GetNodeName(name).AppendSuffixIfMissing("View");

        private static string targetNodeFileNameWithExtension => $"{nodeName}.cs";
        private static string targetNodeFilePath => $"{runtimeTargetPath}/{targetNodeFileNameWithExtension}";

        private static string targetNodeEditorFileNameWithExtension => $"{nodeEditorName}.cs";
        private static string targetNodeEditorFilePath => $"{editorTargetPath}/{targetNodeEditorFileNameWithExtension}";

        private static string targetNodeViewFileNameWithExtension => $"{nodeViewName}.cs";
        private static string targetNodeViewFilePath => $"{editorTargetPath}/{targetNodeViewFileNameWithExtension}";

        private static string nodeName { get; set; }
        private static string nodeEditorName { get; set; }
        private static string nodeViewName { get; set; }

        private static NodeType targetNodeType { get; set; }
        private static bool targetCanBeDeleted { get; set; }
        private static bool targetRunUpdate { get; set; }
        private static bool targetRunFixedUpdate { get; set; }
        private static bool targetRunLateUpdate { get; set; }

        private static string runtimeTargetPath { get; set; }
        private static string editorTargetPath { get; set; }

        public static void CreateNode
        (
            string name, NodeType nodeType,
            bool canBeDeleted, bool runUpdate, bool runFixedUpdate, bool runLateUpdate,
            string runtimePath, string editorPath
        )
        {
            nodeName = GetNodeName(name);
            nodeEditorName = GetNodeEditorName(name);
            nodeViewName = GetNodeViewName(name);

            targetNodeType = nodeType;
            targetCanBeDeleted = canBeDeleted;
            targetRunUpdate = runUpdate;
            targetRunFixedUpdate = runFixedUpdate;
            targetRunLateUpdate = runLateUpdate;

            runtimeTargetPath = runtimePath;
            editorTargetPath = editorPath;

            if (string.IsNullOrEmpty(nodeName)) throw new ArgumentException($"{nameof(name)} cannot be null or empty");
            if (string.IsNullOrEmpty(runtimePath)) throw new ArgumentException($"{nameof(runtimePath)} cannot be null or empty");
            if (string.IsNullOrEmpty(editorPath)) throw new ArgumentException($"{nameof(editorPath)} cannot be null or empty");

            Run();
        }

        private static bool newNodeWasCreated { get; set; }

        private static bool Run(bool saveAssets = true, bool refreshAssetDatabase = false, bool silent = false)
        {
            newNodeWasCreated = false;
            bool result = CreateNode(silent) & CreateNodeEditor(silent) & CreateNodeView(silent);
            if (!result) return false;
            if (saveAssets) AssetDatabase.SaveAssets();
            if (refreshAssetDatabase) AssetDatabase.Refresh();
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
            Debug.Log($"'{nodeName}' created - To finalize the process run: Tools > Doozy > Nody > Refresh\nThis step is needed to inject the new node in the system");
            return true;
        }

        private static bool CreateNode(bool silent = false)
        {
            string data = FileGenerator.GetFile(templateNewNodeFilePath);
            {
                data = data.Replace("//NodeName//", nodeName);
                data = data.Replace("//MenuNodeName//", nodeName.RemoveSuffix("Node"));
                data = data.Replace("//NodeType//", targetNodeType.ToString());
                data = data.Replace("//canBeDeleted//", targetCanBeDeleted.ToString().ToLowerInvariant());
                data = data.Replace("//runUpdate//", targetRunUpdate.ToString().ToLowerInvariant());
                data = data.Replace("//runFixedUpdate//", targetRunFixedUpdate.ToString().ToLowerInvariant());
                data = data.Replace("//runLateUpdate//", targetRunLateUpdate.ToString().ToLowerInvariant());
            }
            bool result = FileGenerator.WriteFile(targetNodeFilePath, data, silent);
            return result;
        }

        private static bool CreateNodeEditor(bool silent = false)
        {
            string data = FileGenerator.GetFile(templateNewNodeEditorFilePath);
            {
                data = data.Replace("//NodeEditorName//", nodeEditorName);
                data = data.Replace("//NodeName//", nodeName);
            }
            bool result = FileGenerator.WriteFile(targetNodeEditorFilePath, data, silent);
            return result;
        }

        private static bool CreateNodeView(bool silent = false)
        {
            string data = FileGenerator.GetFile(templateNewNodeViewFilePath);
            {
                data = data.Replace("//NodeViewName//", nodeViewName);
                data = data.Replace("//NodeName//", nodeName);
            }
            bool result = FileGenerator.WriteFile(targetNodeViewFilePath, data, silent);
            return result;
        }
    }
}
